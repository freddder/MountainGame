extends State
class_name PlayerClimb

@export_group("Movement")
@export var climb_speed: float = 5.0
@export var jump_force: float = 10.0

var is_jumping : bool = false

@onready var body: Player = $"../.."
@onready var mesh: Node3D = $"../../Mesh"
@onready var wall_check: RayCast3D = $"../../Mesh/WallCheck"
@onready var wall_top_check: RayCast3D = $"../../Mesh/WallTopCheck"

func enter():
	body.velocity = Vector3.ZERO

func exit():
	is_jumping = false

func update(delta: float):
	pass

func physics_update(delta: float):
	if body.is_on_floor():
		ChangeState.emit("grounded")
		return
	
	if !wall_check.is_colliding():
		ChangeState.emit("airborne")
		return
	
	body.get_best_wall_collision()
	
	# Movement
	var input : Vector2 = body.get_input_dir()
	if Input.is_action_pressed("jump") and !is_jumping:
		if input.y > 0.0: # Jump away from wall
			is_jumping = true
			var normal = wall_check.get_collision_normal()
			var jump_horizontal_dir = Vector2(normal.x, normal.z).normalized()
			var jump_dir = Vector3(jump_horizontal_dir.x, 1.0, jump_horizontal_dir.y) * jump_force
			body.velocity = jump_dir
			var look_pos = body.global_position + normal
			mesh.look_at(look_pos)
			ChangeState.emit("airborne")
		else:
			pass # TODO: regular jumping
		return
	
	if wall_check.is_colliding() and !wall_top_check.is_colliding():
		pass # TODO: jump over edge
	
	var normal = (wall_check.get_collision_normal() + wall_top_check.get_collision_normal()) / 2.0
	if input != Vector2.ZERO:
		var rot = -(atan2(normal.z, normal.x) - PI/2)
		var climb_dir = Vector3(input.x, -input.y, 0.0).rotated(Vector3.UP, rot).normalized()
		body.velocity = climb_dir * climb_speed - normal * 0.5
		body.velocity += -normal.normalized() * 0.5
	else:
		body.velocity = -normal.normalized() * 0.5
	
	var look_pos = body.global_position - normal
	mesh.look_at(look_pos)
