extends Control

@onready var game_list_container = $ScrollContainer/VBoxContainer

func _ready():
	load_game_files()

func load_game_files():
	# Clear only previously added game buttons, preserve other children (e.g., Quit)
	for child in game_list_container.get_children():
		if child is Button and child.name.begins_with("GameButton_"):
			child.queue_free()

	# Open the Games directory
	var dir = DirAccess.open("res://Games")
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		
		# Variables for manual button positioning
		var y_pos = 0
		var button_height = 30  # Fixed height for each button
		var margin = 10        # Space between buttons
		
		while file_name != "":
			if file_name.ends_with(".txt") and file_name.begins_with("Game_"):
				var game_parts = file_name.split("_")
				if game_parts.size() == 3:
					var game_type = game_parts[1]  # Bot or PvP
					var game_id = game_parts[2].replace(".txt", "")  # Remove .txt
					
					# Create and configure the button
					var button = Button.new()
					button.name = "GameButton_" + game_id
					button.text = "Game ID: %s (%s)" % [game_id, game_type]
					button.connect("pressed", Callable(self, "_on_game_selected").bind(game_id))
					
					# Set button position and size
					button.position = Vector2(0, y_pos)
					button.size = Vector2(game_list_container.size.x, button_height)
					
					# Add button to VScrollBar
					game_list_container.add_child(button)
					
					# Update y position for the next button
					y_pos += button_height + margin
			
			file_name = dir.get_next()
		
		dir.list_dir_end()
	else:
		print("Error: Could not open Games directory")

# Callback when a game button is pressed
func _on_game_selected(game_id: String):
	print("Selected game ID: ", game_id)
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/GAME/main.tscn")
	switcher.curr.LoadGame(game_id)
	switcher.switch_scene_withoutparam()

func _on_quit_pressed():
	SceneSwitcher.switch_scene("res://TSCN/GUI/menu.tscn")
