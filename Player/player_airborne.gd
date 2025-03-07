extends State
class_name PlayerAirborne

@export_group("Movement")
@export var fall_acceleration: float = 45.0
@export var max_fall_speed: float = 75.0
@export_group("Glide")
@export var glide_fall_speed: float = 4.0
@export var glide_horizontal_speed: float = 10.0
@export var glide_horizontal_acceleration: float = 2.0
@export var glide_turn_acceleration: float = 1.0
var is_gliding: bool = false
var glide_turn_speed: float = 0.0
var is_glide_action_buffer: bool = false
var glide_action_buffer_timer: float = 0.3

@onready var body: Player = $"../.."
@onready var mesh: Node3D = $"../../Mesh"
@onready var camera_target: Node3D = $"../../CameraTarget"
@onready var wings: MeshInstance3D = $"../../Mesh/Wings"


func enter():
	pass

func exit():
	is_gliding = false
	is_glide_action_buffer = false
	wings.visible = false
	reset_glide_action_buffer()

func update(delta: float):
	pass

func physics_update(delta: float):
	if body.is_on_floor():
		ChangeState.emit("grounded")
		return
	
	var collision = body.get_best_wall_collision()
	if collision and body.velocity.y < 0.0:
		ChangeState.emit("climb")
		return
	
	if body.velocity.y < 0.0 and (Input.is_action_just_pressed("jump") or is_glide_action_buffer):
		is_gliding = !is_gliding
		wings.visible = !wings.visible
		reset_glide_action_buffer()
	elif Input.is_action_just_pressed("jump") and !is_glide_action_buffer:
		is_glide_action_buffer = true
	
	if is_glide_action_buffer:
		glide_action_buffer_timer -= delta
		if glide_action_buffer_timer <= 0.0:
			reset_glide_action_buffer()
	
	var vel = body.velocity
	if is_gliding:
		var input_direction = body.get_input_dir()
		var target_horizontal_velocity = Vector3.ZERO
		var curr_horizontal_velocity = body.velocity
		curr_horizontal_velocity.y = 0
		if input_direction != Vector2.ZERO:
			target_horizontal_velocity = camera_target.transform.basis.z * input_direction.y + camera_target.transform.basis.x * input_direction.x
			target_horizontal_velocity.y = 0
			target_horizontal_velocity = target_horizontal_velocity * glide_horizontal_speed
			
			var look_angle = atan2(-target_horizontal_velocity.x, -target_horizontal_velocity.z)
			mesh.rotation.y = lerp_angle(mesh.rotation.y, look_angle, glide_turn_acceleration * delta) 
		
		vel = curr_horizontal_velocity.lerp(target_horizontal_velocity, glide_horizontal_acceleration * delta)
		vel.y = body.velocity.y + -(fall_acceleration * delta)
		vel.y = clamp(vel.y, -glide_fall_speed, glide_fall_speed)
	else:
		vel.y = body.velocity.y + -(fall_acceleration * delta)
		vel.y = clamp(vel.y, -max_fall_speed, max_fall_speed)
	
	body.velocity = vel

func reset_glide_action_buffer():
	is_glide_action_buffer = false
	glide_action_buffer_timer = 0.3
