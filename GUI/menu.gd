extends Control

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

func _on_settings_pressed():
	print("todo!")

func _on_quit_pressed():
	get_tree().quit()

func _on_new_game_bot_pressed():
	SceneSwitcher.switch_scene("res://TSCN/new_bot_game.tscn")
	
	
func _on_new_game_player_pressed():
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/main.tscn")
	switcher.curr.StartNewPvpGame()
	switcher.switch_scene_withoutparam()
	
	
func _input(event):
	if Input.is_key_pressed(KEY_ESCAPE):
		var switcher = SceneSwitcher
		switcher.create_scene("res://TSCN/main.tscn")
		switcher.curr.SetMesh()
		switcher.switch_scene_withoutparam()
		
