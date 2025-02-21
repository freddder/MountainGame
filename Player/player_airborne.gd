extends State
class_name PlayerAirborne

@export_group("Movement")
@export var fall_acceleration: float = 45.0
@export var max_fall_speed: float = 75.0

@onready var body: CharacterBody3D = $"../.."
@onready var camera_target: Node3D = $"../../CameraTarget"

func enter():
	print("air")

func exit():
	pass

func update(delta: float):
	pass

func physics_update(delta: float):
	if body.is_on_floor():
		ChangeState.emit("grounded")
		return
	
	var vertical_velocity = 0.0
	vertical_velocity = body.velocity.y + -(fall_acceleration * delta)
	vertical_velocity = clamp(vertical_velocity, -max_fall_speed, max_fall_speed)
	
	body.velocity.y = vertical_velocity
