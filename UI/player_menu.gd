extends Control
class_name PlayerUi

@onready var stamina_wheel: TextureProgressBar = $StaminaWheel
@onready var buttons_menu: ColorRect = $ColorRect
@onready var resume_button: Button = $ColorRect/VBoxContainer/Resume

@onready var main_menu: PackedScene = load("res://main_menu.tscn")

signal PauseToggle

func _ready():
	stamina_wheel.visible = false
	buttons_menu.visible = false

func _process(delta):
	pass

func pause():
	buttons_menu.visible = true
	resume_button.grab_focus()

func unpause():
	buttons_menu.visible = false

func _on_resume_pressed():
	PauseToggle.emit()

func _on_restart_pressed():
	get_tree().reload_current_scene()

func _on_main_menu_pressed():
	get_tree().change_scene_to_packed(main_menu)
