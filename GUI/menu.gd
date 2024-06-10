extends Control

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_settings_pressed():
	pass

func _on_quit_pressed():
	get_tree().quit()

func _on_new_game_bot_pressed():
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/main.tscn")
	switcher.curr.test()
	switcher.switch_scene_withoutparam()
	
	

func _on_new_game_player_pressed():
	print("todo!")
	
	
	
func _input(event):
	if Input.is_key_pressed(KEY_ESCAPE):
		var switcher = SceneSwitcher
		switcher.create_scene("res://TSCN/main.tscn")
		switcher.curr.SetMesh()
		switcher.switch_scene_withoutparam()
		

