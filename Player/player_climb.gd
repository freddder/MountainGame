extends State
class_name PlayerClimb

@export_group("Movement")
@export var climb_speed: float = 5.0
@export var jump_force: float = 10.0

var is_jumping : bool = false

@onready var body: Player = $"../.."

func enter():
	body.velocity = Vector3.ZERO

func exit():
	is_jumping = false

func update(delta: float):
	pass

func physics_update(delta: float):
	var collision = body.get_touching_wall()
	if !collision:
		if body.is_on_floor():
			ChangeState.emit("gounded")
		else:
			ChangeState.emit("airborne")
		return
	
	# Movement
	var input : Vector2 = body.get_input_dir()
	
	if Input.is_action_pressed("jump") and !is_jumping:
		if input.y > 0.0: # Jump away from wall
			is_jumping = true
			var normal = collision.get_normal()
			print(normal)
			var jump_horizontal_dir = Vector2(normal.x, normal.z).normalized()
			var jump_dir = Vector3(jump_horizontal_dir.x, 1.0, jump_horizontal_dir.y) * jump_force
			body.velocity = jump_dir
