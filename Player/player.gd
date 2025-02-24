extends CharacterBody3D
class_name Player

@export_group("Camera")
@export var camera_sensitivity: float = .35
var camera_input_direction := Vector2.ZERO

@onready var camera_target: Node3D = $CameraTarget
@onready var mesh: Node3D = $Mesh

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func get_input_dir() -> Vector2:
	var input_direction = Vector2.ZERO
	if Input.is_action_pressed("move_right"):
		input_direction.x += 1
	if Input.is_action_pressed("move_left"):
		input_direction.x -= 1
	if Input.is_action_pressed("move_back"):
		input_direction.y += 1
	if Input.is_action_pressed("move_forward"):
		input_direction.y -= 1
	
	return input_direction.normalized()

func _physics_process(delta):
	camera_target.rotation.x -= camera_input_direction.y * delta
	camera_target.rotation.x = clamp(camera_target.rotation.x, -PI / 2 + 0.1, PI / 2 - 0.1)
	camera_target.rotation.y -= camera_input_direction.x * delta
	
	move_and_slide()
	
	var look_pos = global_position + velocity
	look_pos.y = global_position.y
	if look_pos != global_position:
		mesh.look_at(look_pos)
	
	camera_input_direction = Vector2.ZERO

func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		camera_input_direction = event.relative * camera_sensitivity
	
	if event.is_action_pressed("left_click"):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	if event.is_action_pressed("ui_cancel"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

func get_touching_wall() -> KinematicCollision3D:
	if !is_on_wall():
		return null
	
	for i in get_slide_collision_count():
		var collision = get_slide_collision(i)
		if collision.get_angle() < deg_to_rad(50):
			continue
		
		#print(rad_to_deg(collision.get_angle()))
		return collision
	
	return null
