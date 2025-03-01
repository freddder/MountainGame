extends State
class_name PlayerGrounded

@export_group("Movement")
@export var max_walk_speed: float = 10.0
var walk_acceleration := 20.0

@onready var body: Player = $"../.."
@onready var camera_target: Node3D = $"../../CameraTarget"
@onready var mesh: Node3D = $"../../Mesh"
@onready var collision_shape: CollisionShape3D = $"../../CollisionShape3D"

func enter():
	var look_pos = body.global_position + body.velocity
	look_pos.y = body.global_position.y
	mesh.look_at(look_pos)
	collision_shape.look_at(look_pos)

func exit():
	pass

func update(delta: float):
	pass

func physics_update(delta: float):
	if !body.is_on_floor():
		ChangeState.emit("airborne")
		return
	
	var collision = body.get_best_wall_collision()
	if !body.is_on_floor() and collision and body.velocity.y < 0.0:
		ChangeState.emit("climb")
		return
	
	var input_direction = body.get_input_dir()
	var target_horizontal_velocity = camera_target.transform.basis.z * input_direction.y + camera_target.transform.basis.x * input_direction.x
	target_horizontal_velocity.y = 0
	target_horizontal_velocity = target_horizontal_velocity.normalized() * max_walk_speed
	
	# Horizontal velocity
	# velocity = velocity.lerp(target, H\_acc \* delta) 
	var horizontal_velocity := body.velocity.lerp(target_horizontal_velocity, walk_acceleration * delta)
	
	# Vertical velocity
	var vertical_velocity = 0.0
	if Input.is_action_pressed("jump"):
		vertical_velocity = 10.0
	elif collision:
		if collision.get_normal().dot(target_horizontal_velocity) < 0:
			vertical_velocity = 10.0
	
	body.velocity = Vector3(horizontal_velocity.x, vertical_velocity, horizontal_velocity.z)
	
	if input_direction != Vector2.ZERO:
		var look_pos = body.global_position + body.velocity
		look_pos.y = body.global_position.y
		mesh.look_at(look_pos)
