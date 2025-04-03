# (Previous code - same as last provided block)
extends Control

# --- UI Node References ---
@onready var host_join_option_button = $VBoxContainer/OptionButton
@onready var ip_text_edit = $VBoxContainer/ipText
@onready var port_text_edit = $VBoxContainer/portText
@onready var saved_games_dropdown = $VBoxContainer/SavedGamesOptionButton
# @onready var start_game_button = $VBoxContainer/YourStartButton # Example

# --- Constants ---
const RES_SAVE_PATH = "res://Games"     # Project resource save path
# We will calculate the executable path dynamically below.
const SAVE_FILE_PREFIX = "Game_LAN_"
const SAVE_FILE_SUFFIX = ".txt" # Or .json, .dat, etc.

# --- State Variables ---
# (No specific state needed here)


func _ready() -> void:
	# Populate the dropdown with saved games from both locations
	_populate_saved_games_dropdown()

	# Connect signals (if not done in the editor)
	# ... (keep your signal connections)

	# Initial UI state
	_update_ip_visibility()

# Helper function to scan a single directory and add saves to the dropdown
func _scan_directory_for_saves(dir_path: String, dropdown: OptionButton, start_index: int) -> int:
	var current_index = start_index
	print("Scanning directory: ", dir_path)

	# Check if the directory actually exists before trying to open
	if not DirAccess.dir_exists_absolute(dir_path):
		print("Directory not found (this might be normal): ", dir_path)
		return current_index # Return the same index, nothing added

	var dir = DirAccess.open(dir_path)
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()

		while file_name != "":
			if dir.current_is_dir(): # Skip sub-directories
				file_name = dir.get_next()
				continue

			# Check if the file matches our naming convention
			if file_name.begins_with(SAVE_FILE_PREFIX) and file_name.ends_with(SAVE_FILE_SUFFIX):
				# Extract the game ID
				var id_part = file_name.trim_prefix(SAVE_FILE_PREFIX).trim_suffix(SAVE_FILE_SUFFIX)

				if not id_part.is_empty():
					# Check if this ID already exists (optional, prevents duplicates)
					var already_added = false
					for i in range(dropdown.item_count):
						var meta = dropdown.get_item_metadata(i)
						if meta and meta.has("id") and meta["id"] == id_part:
							already_added = true
							break

					if not already_added:
						# Determine source tag for display
						var source_tag = "res" # Assume resource path
						if not dir_path.begins_with("res://"):
							source_tag = "local" # Tag for saves next to executable

						# Add item to the dropdown
						var item_text = "Load Game: %s (%s)" % [id_part, source_tag]
						dropdown.add_item(item_text, current_index)
						# Store the actual game ID in the item's metadata
						dropdown.set_item_metadata(current_index, {"id": id_part})
						current_index += 1
					else:
						print("Skipping duplicate Game ID found: ", id_part, " in ", dir_path)

				else:
					print("Warning: Found save file with empty ID: ", file_name, " in ", dir_path)

			file_name = dir.get_next()

		dir.list_dir_end()
		# DirAccess doesn't need explicit closing in GDScript 4

	else:
		# Log error if opening failed even though directory exists (e.g., permissions)
		printerr("Error: Could not open directory (check permissions?): ", dir_path)

	return current_index # Return the next available index


func _populate_saved_games_dropdown() -> void:
	saved_games_dropdown.clear()

	# Add the default "New Game" option first
	saved_games_dropdown.add_item("Start New LAN Game", 0)
	saved_games_dropdown.set_item_metadata(0, {"id": null})

	var current_index = 1 # Start index for actual saved games

	# --- Scan the resource directory ---
	current_index = _scan_directory_for_saves(RES_SAVE_PATH, saved_games_dropdown, current_index)

	# --- Scan the directory next to the executable ---
	# NOTE: When running from the Godot Editor (F5), this path points relative
	#       to the EDITOR's executable, not your exported game.
	#       It will work as intended in an EXPORTED build.
	var exe_base_dir = OS.get_executable_path().get_base_dir()
	var exe_games_path = exe_base_dir.path_join("Games")
	current_index = _scan_directory_for_saves(exe_games_path, saved_games_dropdown, current_index)

	print("Finished populating dropdown. Total items: ", saved_games_dropdown.item_count)


func _update_ip_visibility() -> void:
	if host_join_option_button.selected == 1:
		ip_text_edit.show()
	else:
		ip_text_edit.hide()


# --- Signal Handlers ---

func _on_quit_pressed() -> void:
	SceneSwitcher.switch_scene("res://TSCN/GUI/menu.tscn")


func _on_option_button_item_selected(index: int) -> void:
	_update_ip_visibility()


# Optional signal handler
# func _on_saved_games_dropdown_item_selected(index: int): ...


# Main function to start/join/load the game
func _on_start_or_join_lan_game_pressed() -> void:
	var switcher = SceneSwitcher

	# Get data from UI elements
	var port_text: String = port_text_edit.text.strip_edges()
	var ip_text: String = ip_text_edit.text.strip_edges()
	var is_hosting: bool = (host_join_option_button.selected == 0)

	# Get selected save game info
	var selected_save_index: int = saved_games_dropdown.selected
	if selected_save_index < 0:
		printerr("No game selected from the dropdown.")
		return
	var selected_save_metadata = saved_games_dropdown.get_item_metadata(selected_save_index)
	if selected_save_metadata == null or not selected_save_metadata.has("id"):
		printerr("Error retrieving metadata for selected dropdown item index: ", selected_save_index)
		return
	var selected_game_id = selected_save_metadata["id"]


	# --- Input Validation ---
	if port_text.is_empty() or not port_text.is_valid_int():
		printerr("Invalid Port number.")
		return
	var port_num = int(port_text)
	if port_num <= 0 or port_num > 65535:
		printerr("Port number out of valid range (1-65535).")
		return

	if not is_hosting and ip_text.is_empty():
		printerr("IP Address required when joining.")
		return
	# Allow localhost, otherwise check format (basic check)
	if not is_hosting and ip_text.to_lower() != "localhost" and not ip_text.is_valid_ip_address():
		printerr("Invalid IP Address format (or not 'localhost').")
 		# return # Optional: Stop if format is invalid

	# --- Scene Setup and Function Calls ---
	print("Preparing game scene...")
	var game_script = SceneSwitcher
	game_script.create_scene("res://TSCN/GAME/main.tscn")


	# Check if we are loading a saved game or starting/joining a new one
	if selected_game_id == null:
		# --- New Game Logic ---
		print("Starting/Joining NEW LAN game...")
		if is_hosting:
			print("Calling CreateNewLanGame | Port:", port_text)
			if game_script.curr.has_method("CreateNewLanGame"):
				game_script.curr.CreateNewLanGame(port_text)
			else:
				printerr("Method 'CreateNewLanGame' not found in game script.")
				return
		else:
			print("Calling ConnectLanGame | IP:", ip_text, " Port:", port_text)
			if game_script.curr.has_method("ConnectLanGame"):
				game_script.curr.ConnectLanGame(ip_text, port_text)
			else:
				printerr("Method 'ConnectLanGame' not found in game script.")
				return

	else:
		# --- Load Saved Game Logic ---
		print("Loading existing LAN game ID:", selected_game_id)
		if is_hosting:
			print("Calling LoadNewLanGame | Port:", port_text, " ID:", selected_game_id)
			if game_script.curr.has_method("LoadNewLanGame"):
				game_script.curr.LoadNewLanGame(port_text, selected_game_id)
			else:
				printerr("Method 'LoadNewLanGame' not found in game script.")
				return
		else:
			print("Calling ConnectLoadLanGame | IP:", ip_text, " Port:", port_text, " ID:", selected_game_id)
			if game_script.curr.has_method("ConnectLoadLanGame"):
				game_script.curr.ConnectLoadLanGame(ip_text, port_text, selected_game_id)
			else:
				printerr("Method 'ConnectLoadLanGame' not found in game script.")
				return

	# Switch to the prepared game scene
	print("Switching scene...")
	game_script.switch_scene_withoutparam()
