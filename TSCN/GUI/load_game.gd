extends Control


@onready var game_list_container = $VBoxContainer

func _ready():
	load_game_files()

func load_game_files():
	# Clear only previously added game buttons, preserve other children (e.g., Quit)
	for child in game_list_container.get_children():
		if child is Button and child.name.begins_with("GameButton_"): # Identify game buttons
			child.queue_free()


	# Open the Games directory
	var dir = DirAccess.open("res://Games")
	if dir:
		dir.list_dir_begin() # Start listing directory contents
		var file_name = dir.get_next()
		
		while file_name != "":
			# Check if it's a .txt file
			if file_name.ends_with(".txt") and file_name.begins_with("Game_"):
				# Extract game info from filename (e.g., Game_Bot_123456.txt)
				var game_parts = file_name.split("_")
				if game_parts.size() == 3: # Ensure it's Game_{type}_{id}.txt
					var game_type = game_parts[1] # Bot or PvP
					var game_id = game_parts[2].replace(".txt", "") # Remove .txt
					
					# Create a button for this game
					var button = Button.new()
					button.text = "Game ID: %s (%s)" % [game_id, game_type]
					button.connect("pressed", Callable(self, "_on_game_selected").bind(game_id))
					game_list_container.add_child(button)
					game_list_container.move_child(button, 0)
			
			file_name = dir.get_next()
		
		dir.list_dir_end() # End listing
	else:
		print("Error: Could not open Games directory")

# Callback when a game button is pressed
func _on_game_selected(game_id: String):
	print("Selected game ID: ", game_id)
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/GAME/main.tscn")
	switcher.curr.LoadGame(game_id)
	switcher.switch_scene_withoutparam()
	# Add your logic here, e.g., load the game file or switch scenes


func _on_quit_pressed():
	SceneSwitcher.switch_scene("res://TSCN/GUI/menu.tscn") # Replace with function body.
