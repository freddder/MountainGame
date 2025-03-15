extends CharacterBody3D
class_name Player

@export_group("Camera")
@export var camera_mouse_sensitivity: float = .003
@export var camera_controller_sensitivity: float = .035

@export_group("Stamina")
@export var max_stamina = 100.0
@export var stamina_recovery_rate = 25.0
var stamina: float = max_stamina
var is_exhausted: bool = false
var wheel_visible_timer = 2.0

@onready var camera_target: Node3D = $CameraTarget
@onready var top_checks_parent: Node3D = $Mesh/UpperChecks
@onready var bot_checks_parent: Node3D = $Mesh/BottomChecks
@onready var stamina_wheel: TextureProgressBar = $PlayerUi/StaminaWheel
var wall_checks: Array[RayCast3D] = []

@onready var player_ui: PlayerUi = $PlayerUi
var is_paused: bool = false

func _ready():
	for check in top_checks_parent.get_children():
		if check is RayCast3D:
			wall_checks.push_back(check)
	
	for check in bot_checks_parent.get_children():
		if check is RayCast3D:
			wall_checks.push_back(check)
	
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	
	player_ui.PauseToggle.connect(toggle_pause)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if !is_paused:
		var camera_stick = Input.get_vector("camera_left", "camera_right", "camera_forward", "camera_back")
		camera_target.rotation.x -= camera_stick.y * camera_controller_sensitivity
		camera_target.rotation.x = clampf(camera_target.rotation.x, -deg_to_rad(85), deg_to_rad(65))
		camera_target.rotation.y += -camera_stick.x * camera_controller_sensitivity
	
	if stamina_wheel.value == 100:
		if stamina_wheel.visible:
			wheel_visible_timer -= delta
			if wheel_visible_timer < 0.0:
				stamina_wheel.visible = false
				wheel_visible_timer = 2.0
	else:
		stamina_wheel.visible = true
	
	if Input.is_action_just_pressed("pause"):
		toggle_pause()

func get_input_dir() -> Vector2:
	if is_paused:
		return Vector2.ZERO
	
	var input_direction = Input.get_vector("move_left", "move_right", "move_forward", "move_back").limit_length(1.0)
	return input_direction

func _physics_process(delta):
	
	move_and_slide()
	

func _input(event):
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		camera_target.rotation.x -= event.relative.y * camera_mouse_sensitivity
		camera_target.rotation.x = clampf(camera_target.rotation.x, -deg_to_rad(85), deg_to_rad(65))
		camera_target.rotation.y += -event.relative.x * camera_mouse_sensitivity

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

func add_stamina_amount(amount: float):
	stamina += amount
	
	if amount < 0.0 and stamina < 0.0:
		is_exhausted = true
		stamina_wheel.modulate = "c90000"
	elif amount > 0.0 and stamina > max_stamina:
		is_exhausted = false
		stamina_wheel.modulate = "00d000"
	
	stamina = clamp(stamina, 0.0, max_stamina)
	stamina_wheel.value = stamina

func toggle_pause():
	if is_paused: #unpause
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
		is_paused = false
		player_ui.unpause()
	else: #pause
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		is_paused = true
		player_ui.pause()
