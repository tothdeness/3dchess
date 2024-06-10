# SceneSwitcher.switch_scene("res://level_1.tscn")
extends Node

var current_scene = null
var curr

func _ready() -> void:
	var root = get_tree().root
	current_scene = root.get_child(root.get_child_count() - 1)

func switch_scene(res_path):
	call_deferred("_deferred_switch_scene", res_path)
	
func switch_scene_withoutparam():
	call_deferred("_deferred_switch_scene_withoutparam")	
	
	
func _deferred_switch_scene(res_path):
	current_scene.free()
	var s = load(res_path)
	current_scene = s.instantiate()
	get_tree().root.add_child(current_scene)
	get_tree().current_scene = current_scene
	
func _deferred_switch_scene_withoutparam():
	current_scene.free()
	current_scene = curr
	get_tree().root.add_child(curr)
	get_tree().current_scene = curr
	
	
func create_scene(res_path):
	var s = load(res_path)
	curr = s.instantiate()
