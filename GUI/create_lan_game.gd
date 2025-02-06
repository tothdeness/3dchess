extends Control


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_quit_pressed() -> void:
	SceneSwitcher.switch_scene("res://TSCN/GUI/new_pvp.tscn")

func _on_option_button_item_selected(index: int) -> void:
	if index == 1:
		$VBoxContainer/ipText.show();
	else:
		$VBoxContainer/ipText.hide();


func _on_new_game_bot_pressed() -> void:
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/GAME/main.tscn")
	if $VBoxContainer/OptionButton.selected == 0:
		var port_text = $VBoxContainer/portText.text
		switcher.curr.CreateNewLanGame(port_text)
	else:
		switcher.curr.ConnectLanGame($VBoxContainer/ipText.text,$VBoxContainer/portText.text)
	switcher.switch_scene_withoutparam()
