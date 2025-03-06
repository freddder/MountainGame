extends CharacterBody3D
class_name Player

@export_group("Camera")
@export var camera_sensitivity: float = .01

@onready var camera_target: Node3D = $CameraTarget
@onready var top_checks_parent: Node3D = $Mesh/UpperChecks
@onready var bot_checks_parent: Node3D = $Mesh/BottomChecks
var wall_checks: Array[RayCast3D] = []

func _ready():
	for check in top_checks_parent.get_children():
		if check is RayCast3D:
			wall_checks.push_back(check)
	
	for check in bot_checks_parent.get_children():
		if check is RayCast3D:
			wall_checks.push_back(check)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func get_input_dir() -> Vector2:
	var input_direction = Input.get_vector("move_left", "move_right", "move_forward", "move_back").limit_length(1.0)
	return input_direction

func _physics_process(delta):
	
	move_and_slide()
	


func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		camera_target.rotation.x -= event.relative.y * camera_sensitivity
		camera_target.rotation.x = clampf(camera_target.rotation.x, -PI / 2 + 0.1, PI / 2 - 0.1)
		camera_target.rotation.y += -event.relative.x * camera_sensitivity
	
	if event.is_action_pressed("left_click"):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	if event.is_action_pressed("ui_cancel"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

func get_average_wall_checks_normal() -> Vector3:
	var checks_count = 0
	var normal_sum = Vector3.ZERO
	for check in wall_checks:
		if !check.is_colliding():
			continue
		
		normal_sum += check.get_collision_normal()
		checks_count += 1
	
	if checks_count == 0:
		return Vector3.ZERO
	return normal_sum / checks_count

func get_best_wall_collision() -> KinematicCollision3D:	
	var highest_dot = 0
	var best_collision = null
	for i in get_slide_collision_count():
		var collision = get_slide_collision(i)
		if collision.get_angle() < deg_to_rad(46):
			continue
		
		var input_dir = get_input_dir()
		var target_horizontal = camera_target.transform.basis.z * input_dir.y + camera_target.transform.basis.x * input_dir.x
		target_horizontal.y = 0
		var dot = target_horizontal.normalized().dot(-collision.get_normal().normalized())
		if dot > 0.7 and dot > highest_dot: # TODO: make 0.7 a variable in degrees
			highest_dot = dot
			best_collision = collision
	return best_collision
