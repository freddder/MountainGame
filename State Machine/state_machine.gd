extends Node
class_name StateMachine

@export var initial_state : State = null
var current_state : State
var states : Dictionary = {}

func _ready():
	for child in get_children():
		if child is State:
			states[child.name.to_lower()] = child
			child.ChangeState.connect(transition_to_state)
	
	if initial_state:
		initial_state.enter()

func _process(delta):
	return
	if current_state:
		current_state.update(delta)

func _physics_process(delta):
	return
	if current_state:
		current_state.physics_update(delta)

func transition_to_state(new_state_name : String):
	var new_state = states.get(new_state_name.to_lower())
	if !new_state:
		return
	
	if current_state:
		current_state.exit()
	
	new_state.enter()
	current_state = new_state
