extends CharacterBody3D

@export_group("Camera")
@export var camera_sensitivity: float = .35
var camera_input_direction := Vector2.ZERO

@export_group("Movement")
@export var max_walk_speed: float = 10.0
@export var fall_acceleration: float = 45.0
@export var max_fall_speed: float = 75.0

var walk_acceleration := 20.0

@onready var camera_target: Node3D = $CameraTarget

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _physics_process(delta):
	camera_target.rotation.x -= camera_input_direction.y * delta
	camera_target.rotation.x = clamp(camera_target.rotation.x, -PI / 2 + 0.1, PI / 2 - 0.1)
	
	camera_target.rotation.y -= camera_input_direction.x * delta
	
	move_and_slide()
	
	camera_input_direction = Vector2.ZERO

func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		camera_input_direction = event.relative * camera_sensitivity
	
	if event.is_action_pressed("left_click"):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	if event.is_action_pressed("ui_cancel"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
