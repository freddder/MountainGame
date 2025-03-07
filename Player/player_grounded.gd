extends State
class_name PlayerGrounded

@export_group("Movement")
@export var max_walk_speed: float = 10.0
@export var walk_turn_speed: float = 7.0
var walk_acceleration := 20.0

@onready var body: Player = $"../.."
@onready var camera_target: Node3D = $"../../CameraTarget"
@onready var mesh: Node3D = $"../../Mesh"
@onready var collision_shape: CollisionShape3D = $"../../CollisionShape3D"
@onready var debug_sphere = $"../../Mesh/DebugSphere"

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
	
	var input_direction = body.get_input_dir()
	var target_horizontal_velocity = Vector3(input_direction.x, 0.0, input_direction.y).rotated(Vector3.UP, camera_target.rotation.y)
	target_horizontal_velocity = target_horizontal_velocity * max_walk_speed
	
	if input_direction != Vector2.ZERO:
		var target_rotation = atan2(-target_horizontal_velocity.x, -target_horizontal_velocity.z)
		var angle_diff = angle_difference(mesh.rotation.y, target_rotation)
		var step = walk_turn_speed * delta
		mesh.rotation.y += clamp(angle_diff, -step, step)
		mesh.rotation.y = fmod(mesh.rotation.y, TAU)
	
	# Horizontal velocity
	var curr_speed = Vector3(body.velocity.x, 0.0, body.velocity.z).length()
	var horizontal_velocity = -mesh.global_basis.z * lerpf(curr_speed, target_horizontal_velocity.length(), walk_acceleration * delta)
	
	# Vertical velocity
	var vertical_velocity = 0.0
	if Input.is_action_pressed("jump"):
		vertical_velocity = 10.0
	elif collision:
		if collision.get_normal().dot(target_horizontal_velocity) < 0:
			vertical_velocity = 10.0
	
	body.velocity = Vector3(horizontal_velocity.x, vertical_velocity, horizontal_velocity.z)
