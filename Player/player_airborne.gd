extends State
class_name PlayerAirborne

@export_group("Movement")
@export var fall_acceleration: float = 45.0
@export var max_fall_speed: float = 75.0

@onready var body: Player = $"../.."
@onready var mesh: Node3D = $"../../Mesh"
@onready var wall_check: RayCast3D = $"../../Mesh/WallCheck"

func enter():
	pass

func exit():
	pass

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
	
	var vertical_velocity = 0.0
	vertical_velocity = body.velocity.y + -(fall_acceleration * delta)
	vertical_velocity = clamp(vertical_velocity, -max_fall_speed, max_fall_speed)
	
	body.velocity.y = vertical_velocity
	
	#var look_pos = body.global_position + body.velocity
	#look_pos.y = body.global_position.y
	#mesh.look_at(look_pos)
