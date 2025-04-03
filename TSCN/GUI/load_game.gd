extends Control

# --- UI Node References ---
@onready var game_list_container = $ScrollContainer/VBoxContainer
@onready var delete_button = $DeleteButton
@onready var load_button = $LoadButton

# --- Constants ---
const RES_SAVE_PATH = "res://Games"     # Project resource save path
# Executable path will be calculated dynamically
const SAVE_FILE_PREFIX = "Game_" # Assuming "Game_" based on original code
const SAVE_FILE_SUFFIX = ".txt"

# --- State Variables ---
var selected_game_id: String = ""
var selected_game_type: String = ""
var selected_button_node: Button = null

func _ready():
	# --- Ensure Buttons Exist and Connect Signals ---
	if delete_button:
		delete_button.pressed.connect(_on_delete_selected_pressed)
		delete_button.disabled = true
	else:
		print("Warning: DeleteButton node not found. Deletion will not work.")

	if load_button:
		load_button.pressed.connect(_on_load_selected_pressed)
		load_button.disabled = true
	else:
		print("Warning: LoadButton node not found. Loading selection will not work.")

	# --- Load game files from both locations ---
	load_game_files()


func load_game_files():
	# --- Clear previously added game buttons ---
	for child in game_list_container.get_children():
		if child is Button and child.has_meta("is_game_button"):
			child.queue_free()

	# --- Reset selection state ---
	_clear_selection()

	# --- Define paths ---
	var res_path = RES_SAVE_PATH
	# Calculate path next to executable
	# NOTE: This path is relative to the EDITOR executable when running from editor (F5),
	#       and relative to the EXPORTED game executable when running a build.
	var exe_base_dir = OS.get_executable_path().get_base_dir()
	var exe_path = exe_base_dir.path_join("Games") # Assuming "Games" folder next to exe

	# --- Scan both directories ---
	print("Scanning for game files...")
	_scan_and_add_buttons(res_path, game_list_container)
	_scan_and_add_buttons(exe_path, game_list_container)
	print("Finished scanning.")


# Helper function to scan a directory and add buttons to the container
func _scan_and_add_buttons(dir_path: String, container: VBoxContainer):
	print("Scanning directory: ", dir_path)

	# Check if directory exists before trying to open
	if not DirAccess.dir_exists_absolute(dir_path):
		print("Directory not found (this might be normal): ", dir_path)
		return # Nothing to do if directory doesn't exist

	var dir = DirAccess.open(dir_path)
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()

		while file_name != "":
			if dir.current_is_dir(): # Skip directories
				file_name = dir.get_next()
				continue

			# Check file naming convention
			if file_name.ends_with(SAVE_FILE_SUFFIX) and file_name.begins_with(SAVE_FILE_PREFIX):
				var base_name = file_name.trim_suffix(SAVE_FILE_SUFFIX)
				var game_parts = base_name.split("_") # Split "Game_Type_ID"

				if game_parts.size() == 3: # Expecting ["Game", "Type", "ID"]
					var game_type = game_parts[1] # Bot or PvP or LAN etc.
					var game_id = game_parts[2]
					var source_tag = "res" if dir_path.begins_with("res://") else "local"

					# Create and configure the button
					var button = Button.new()
					button.name = "GameButton_%s_%s" % [source_tag, game_id] # Unique name
					button.text = "ID: %s (%s) [%s]" % [game_id, game_type, source_tag]
					# Store game info needed for actions
					button.set_meta("game_id", game_id)
					button.set_meta("game_type", game_type)
					button.set_meta("file_name", file_name) # Just the filename
					button.set_meta("source_dir", dir_path) # Store the directory it came from!
					button.set_meta("is_game_button", true)

					# Set button properties
					button.size_flags_horizontal = Control.SIZE_EXPAND_FILL

					# Connect to selection handler
					button.pressed.connect(_on_game_button_selected.bind(button))

					# Add button to container
					container.add_child(button)
				else:
					print("Warning: Skipping file with unexpected format: ", file_name, " in ", dir_path)


			file_name = dir.get_next()
		dir.list_dir_end()
	else:
		printerr("Error: Could not open directory (check permissions?): ", dir_path)


# --- Selection and Action Functions ---

func _on_game_button_selected(button: Button):
	# Deselect previous button
	if selected_button_node and is_instance_valid(selected_button_node):
		selected_button_node.modulate = Color(1, 1, 1) # Reset color

	# Store info from the newly selected button
	selected_button_node = button
	selected_game_id = button.get_meta("game_id", "") # Use default value if meta not found
	selected_game_type = button.get_meta("game_type", "")

	# Visually indicate selection
	selected_button_node.modulate = Color(0.8, 0.9, 1.0) # Light blue tint

	# Enable action buttons
	if delete_button:
		# *** IMPORTANT: Only enable deletion if the file is NOT in res:// ***
		var source_dir = button.get_meta("source_dir", "")
		if source_dir.begins_with("res://"):
			delete_button.disabled = true
			delete_button.tooltip_text = "Cannot delete files from project resources."
		else:
			delete_button.disabled = false
			delete_button.tooltip_text = "" # Clear tooltip
	if load_button:
		load_button.disabled = false

	print("Selected Game ID: ", selected_game_id, " Type: ", selected_game_type, " Source: ", button.get_meta("source_dir","?"))

# --- Loading ---
func _on_load_selected_pressed():
	if not selected_button_node or selected_game_id == "":
		print("No game selected to load.")
		return

	print("Loading selected game ID: ", selected_game_id)
	var switcher = SceneSwitcher # Assuming SceneSwitcher is an autoload/singleton

	# --- Prepare and switch scene ---
	# Check if create_scene was successful
	switcher.create_scene("res://TSCN/GAME/main.tscn")
	switcher.curr.LoadGame(selected_game_id)
	switcher.switch_scene_withoutparam()



# --- Deletion ---
func _on_delete_selected_pressed():
	if not selected_button_node or selected_game_id == "":
		print("No game selected to delete.")
		return

	# Get file details from the selected button's metadata
	var file_name_to_delete = selected_button_node.get_meta("file_name", "")
	var source_dir = selected_button_node.get_meta("source_dir", "")

	if file_name_to_delete.is_empty() or source_dir.is_empty():
		printerr("Error: Missing file details in selected button metadata.")
		return

	# *** CRITICAL: Prevent deleting from res:// ***
	if source_dir.begins_with("res://"):
		printerr("Action blocked: Cannot delete files from project resources (res://).")
		# Optionally show a user-friendly message in the UI
		return

	# Construct the absolute path for deletion
	var file_path = source_dir.path_join(file_name_to_delete)

	print("Attempting to delete: ", file_path)

	# Use DirAccess.remove_absolute() for non-res paths
	var err = DirAccess.remove_absolute(file_path)

	if err == OK:
		print("Successfully deleted: ", file_path)
		# Remove the button from the list visually
		selected_button_node.queue_free()
		# Clear selection state
		_clear_selection()
	else:
		printerr("Error deleting file: ", file_path, ". Error code: ", err, ". Check permissions?")
		# Optionally show user an error message

# --- Utility Functions ---
func _clear_selection():
	"""Resets the selection state and disables action buttons."""
	if selected_button_node and is_instance_valid(selected_button_node):
		selected_button_node.modulate = Color(1, 1, 1) # Reset visual style

	selected_game_id = ""
	selected_game_type = ""
	selected_button_node = null

	if delete_button:
		delete_button.disabled = true
		delete_button.tooltip_text = "" # Clear tooltip
	if load_button:
		load_button.disabled = true

# --- Navigation ---
func _on_quit_pressed():
	var switcher = SceneSwitcher
	switcher.switch_scene("res://TSCN/GUI/menu.tscn")
