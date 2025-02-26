extends CharacterBody3D
class_name Player

@export_group("Camera")
@export var camera_sensitivity: float = .35
var camera_input_direction := Vector2.ZERO

@onready var camera_target: Node3D = $CameraTarget
@onready var wall_check: RayCast3D = $Mesh/WallCheck
@onready var wall_top_check: RayCast3D = $Mesh/WallTopCheck

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
	
	camera_input_direction = Vector2.ZERO

func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		camera_input_direction = event.relative * camera_sensitivity
	
	if event.is_action_pressed("left_click"):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	if event.is_action_pressed("ui_cancel"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

func get_best_wall_collision() -> KinematicCollision3D:	
	var highest_dot = 0
	var best_collision = null
	for i in get_slide_collision_count():
		var collision = get_slide_collision(i)
		if collision.get_angle() < deg_to_rad(50):
			continue
		
		var input_dir = get_input_dir()
		var target_horizontal = camera_target.transform.basis.z * input_dir.y + camera_target.transform.basis.x * input_dir.x
		target_horizontal.y = 0
		var dot = target_horizontal.normalized().dot(-collision.get_normal().normalized()) # wrong as hell
		if get_slide_collision_count() > 2:
			print(dot)
		if dot > highest_dot:
			highest_dot = dot
			best_collision = collision
	print("")
	return best_collision
