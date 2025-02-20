extends State
class_name PlayerGrounded

@export_group("Camera")
@export var camera_sensitivity: float = .35
var camera_input_direction := Vector2.ZERO

@export_group("Movement")
@export var max_walk_speed: float = 10.0
var walk_acceleration := 20.0

@onready var body: CharacterBody3D = $"../.."
@onready var camera_target: Node3D = $"../../CameraTarget"

func enter():
	pass

func exit():
	pass

func update(delta: float):
	pass

func physics_update(delta: float):
	camera_target.rotation.x -= camera_input_direction.y * delta
	camera_target.rotation.x = clamp(camera_target.rotation.x, -PI / 2 + 0.1, PI / 2 - 0.1)
	
	camera_target.rotation.y -= camera_input_direction.x * delta
	
	var input_direction = Vector2.ZERO
	var target_horizontasl_velocity = Vector3.ZERO
	
	if Input.is_action_pressed("move_right"):
		input_direction.x += 1
	if Input.is_action_pressed("move_left"):
		input_direction.x -= 1
	if Input.is_action_pressed("move_back"):
		input_direction.y += 1
	if Input.is_action_pressed("move_forward"):
		input_direction.y -= 1
	
	input_direction = input_direction.normalized()
	target_horizontasl_velocity = camera_target.transform.basis.z * input_direction.y + camera_target.transform.basis.x * input_direction.x
	target_horizontasl_velocity.y = 0
	target_horizontasl_velocity = target_horizontasl_velocity.normalized() * max_walk_speed
	
	#Horizontal velocity
	var horizontal_velocity := Vector3.ZERO
	if body.is_on_floor():
		# velocity = velocity.lerp(target, H\_acc \* delta) 
		horizontal_velocity = body.velocity.lerp(target_horizontasl_velocity, walk_acceleration * delta)
	else:
		horizontal_velocity = body.velocity
		horizontal_velocity.y = 0
