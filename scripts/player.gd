extends CharacterBody3D

const SPEED = 5.0
const JUMP_VELOCITY = 4.5
const SENSITIVITY = 0.002

# Bobbing settings
const BOB_FREQUENCY = 15.0        # Speed of bob cycle
const BOB_AMPLITUDE_Y = 0.05     # Vertical bob amount
const BOB_AMPLITUDE_X = 0.02     # Horizontal bob amount
var bob_time = 0.0
var default_camera_position = Vector3.ZERO

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	default_camera_position = $Camera.position

func _input(event):
	if event is InputEventMouseMotion:
		$Camera.rotation.x = clamp($Camera.rotation.x - event.relative.y * SENSITIVITY, -1.5, 1.5)
		rotation.y -= event.relative.x * SENSITIVITY

func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity += get_gravity() * delta

	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	var input_dir := Input.get_vector("left", "right", "up", "down")
	var direction := (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()

	# Movement
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()

	# Camera bobbing with side sway
	if direction.length() > 0 and is_on_floor():
		bob_time += delta * BOB_FREQUENCY
		$Camera.position.y = default_camera_position.y + sin(bob_time) * BOB_AMPLITUDE_Y
		$Camera.position.x = default_camera_position.x + cos(bob_time) * BOB_AMPLITUDE_X
	else:
		# Smoothly reset position when idle
		$Camera.position.y = lerp($Camera.position.y, default_camera_position.y, delta * 5.0)
		$Camera.position.x = lerp($Camera.position.x, default_camera_position.x, delta * 5.0)
