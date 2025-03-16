extends Node3D

@onready var main_level: PackedScene = load("res://terrain_level.tscn")
@onready var test_level: PackedScene = load("res://main_level.tscn")

@onready var first_button: Button = $Control/VBoxContainer/MainLevelButton
@onready var controls_panel: ColorRect = $Control/ColorRect
@onready var controls_back_button: Button = $Control/ColorRect/BackButton
@onready var level_buttons: VBoxContainer = $Control/VBoxContainer

# Called when the node enters the scene tree for the first time.
func _ready():
	first_button.grab_focus()
	controls_panel.visible = false

func _on_main_level_button_pressed():
	print(get_tree().change_scene_to_packed(main_level))

func _on_test_level_button_pressed():
	get_tree().change_scene_to_packed(test_level)

func _on_controls_button_pressed():
	controls_panel.visible = true
	level_buttons.visible = false
	controls_back_button.grab_focus()

func _on_back_button_pressed():
	controls_panel.visible = false
	level_buttons.visible = true
	first_button.grab_focus()
