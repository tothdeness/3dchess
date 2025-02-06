extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_new_game_player_pressed():
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/GAME/main.tscn")
	switcher.curr.StartNewPvpGame()
	switcher.switch_scene_withoutparam()


func _on_quit_pressed() -> void:
	SceneSwitcher.switch_scene("res://TSCN/GUI/menu.tscn")


func _on_new_game_bot_pressed() -> void:
	SceneSwitcher.switch_scene("res://TSCN/GUI/create_lan_game.tscn")
