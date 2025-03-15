extends State
class_name PlayerClimb

@export_group("Movement")
@export var climb_speed: float = 3.0
@export var jump_force: float = 10.0
@export var climb_stamina_reduction_rate: float = 16.0

var is_jumping : bool = false

@onready var body: Player = $"../.."
@onready var mesh: Node3D = $"../../Mesh"
@onready var collision_shape: CollisionShape3D = $"../../CollisionShape3D"
@onready var debug_sphere = $"../../Mesh/DebugSphere"

func enter():
	body.velocity = Vector3.ZERO

func exit():
	is_jumping = false
	debug_sphere.global_position = body.global_position
	mesh.global_rotation.x = 0.0
	collision_shape.global_rotation.x = 0.0

func update(delta: float):
	pass

func physics_update(delta: float):
	if body.is_on_floor():
		ChangeState.emit("grounded")
		return
	
	var normal = body.get_average_wall_checks_normal()
	var dot = normal.dot(Vector3.UP)
	if normal == Vector3.ZERO or body.is_exhausted or abs(dot) > 0.75:
		ChangeState.emit("airborne")
		return
	
	# Movement
	var input : Vector2 = body.get_input_dir()
	var wants_to_jump = Input.is_action_just_pressed("jump") and !body.is_paused
	if wants_to_jump and !is_jumping:
		is_jumping = true
		var jump_horizontal_dir = Vector2(normal.x, normal.z).normalized()
		var jump_dir = Vector3(jump_horizontal_dir.x, 1.0, jump_horizontal_dir.y) * jump_force
		body.velocity = jump_dir
		var look_pos = body.global_position + normal
		mesh.look_at(look_pos)
		ChangeState.emit("airborne")
		return
	
	if input != Vector2.ZERO:
		var wall_right = Vector3.UP.cross(normal).normalized() # Right direction relative to the wall
		var wall_up = normal.cross(wall_right).normalized() # Up direction along the wall
		var climb_dir = (wall_right * input.x + wall_up * -input.y)
		body.velocity = climb_dir * climb_speed
		body.velocity += -normal.normalized()
		
		var look_pos = body.global_position - normal
		mesh.look_at(look_pos)
		collision_shape.look_at(look_pos)
		
		body.add_stamina_amount(-climb_stamina_reduction_rate * delta)
	else:
		body.velocity = Vector3.ZERO
	
	#debug_sphere.global_position = body.global_position + normal.normalized() * 3.0
