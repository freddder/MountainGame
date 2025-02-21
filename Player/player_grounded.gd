extends State
class_name PlayerGrounded

@export_group("Movement")
@export var max_walk_speed: float = 10.0
var walk_acceleration := 20.0

@onready var body: CharacterBody3D = $"../.."
@onready var camera_target: Node3D = $"../../CameraTarget"

func enter():
	print("ground")

func exit():
	pass

func update(delta: float):
	pass

func physics_update(delta: float):
	if !body.is_on_floor():
		ChangeState.emit("airborne")
		return
	
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
	
	# Horizontal velocity
	# velocity = velocity.lerp(target, H\_acc \* delta) 
	var horizontal_velocity := body.velocity.lerp(target_horizontasl_velocity, walk_acceleration * delta)
	
	# Vertical velocity
	var vertical_velocity = 0.0
	if Input.is_action_pressed("jump"):
		vertical_velocity = 10.0
	
	body.velocity = Vector3(horizontal_velocity.x, vertical_velocity, horizontal_velocity.z)
