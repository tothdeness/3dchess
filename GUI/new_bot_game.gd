extends Control

# Declare the label and slider variables
@onready var label = $VBoxContainer2/Label
@onready var h_slider = $VBoxContainer2/HSlider
@onready var checkbutton = $VBoxContainer2/CheckButton

var black = "Fekete"
var white = "Fehér"
var depth = 1
var team = -1

# Called when the node enters the scene tree for the first time.
func _ready():
	# Connect the slider's value_changed signal to the _on_h_slider_changed function
	h_slider.connect("value_changed", Callable(self, "_on_h_slider_changed"))
	# Initialize the label text
	_on_h_slider_changed(h_slider.value)
	checkbutton.text = black

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _on_back_pressed():
	SceneSwitcher.switch_scene("res://TSCN/menu.tscn")

func _on_start_pressed():
	var switcher = SceneSwitcher
	switcher.create_scene("res://TSCN/main.tscn")
	switcher.curr.StartNewBotGame(depth,team)
	switcher.switch_scene_withoutparam()

func _on_h_slider_changed(value):
	# Update the label text with the current value of the slider
	depth = value
	label.text = "Mélység: " + str(value)


func _on_check_button_pressed():
	if checkbutton.text == black:
		checkbutton.text = white
		team = 1
	else:
		checkbutton.text = black
		team = -1
