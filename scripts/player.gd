extends CharacterBody3D

# --- Movement ---
const SPEED := 5.0
const JUMP_VELOCITY := 4.5
const SENSITIVITY := 0.002

# --- Camera bobbing ---
const BOB_FREQ := 15.0
const BOB_Y := 0.05
const BOB_X := 0.02
var bob_time := 0.0
var default_cam_pos := Vector3.ZERO

# --- Multiplayer ---
@onready var sync: MultiplayerSynchronizer = $MultiplayerSynchronizer
var is_local := false

@export var synced_position: Vector3

# --- Other ---
@export var health = 50.0
var health_max = 50.0
var health_regen = 0.05
var health_regen_delay = 3
var time_since_damaged = 0.0

var build_menu_open = true

func _ready() -> void:
	default_cam_pos = $Camera.position
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event: InputEvent) -> void:
	if not is_local:
		return
	if event is InputEventMouseMotion:
		$Camera.rotation.x = clamp($Camera.rotation.x - event.relative.y * SENSITIVITY, -1.5, 1.5)
		rotation.y -= event.relative.x * SENSITIVITY
	if Input.is_action_just_pressed("toggle_hud"):
		$hud.visible = !$hud.visible
	if Input.is_action_just_pressed("toggle_build_menu"):
		toggle_build_menu()

func _physics_process(delta: float) -> void:
	if is_local:
		_process_input(delta)
		_process_health(delta)
		_process_bobbing(delta)

		# Update properties for replication
		synced_position = global_position
	else:
		# Interpolate to received networked values
		global_position = global_position.lerp(synced_position, 0.2)
		$hud/healthbar.value = lerpf($hud/healthbar.value, (health / health_max) * 100.0, 0.2)

func _process_input(delta: float) -> void:
	# Gravity
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Jump
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Movement
	var input_dir := Input.get_vector("left", "right", "up", "down")
	var dir := (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if dir:
		velocity.x = dir.x * SPEED
		velocity.z = dir.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()

func _process_bobbing(delta: float) -> void:
	var input_dir := Input.get_vector("left", "right", "up", "down")
	if input_dir.length() > 0 and is_on_floor():
		bob_time += delta * BOB_FREQ
		$Camera.position.y = default_cam_pos.y + sin(bob_time) * BOB_Y
		$Camera.position.x = default_cam_pos.x + cos(bob_time) * BOB_X
	else:
		$Camera.position.y = lerp($Camera.position.y, default_cam_pos.y, delta * 5.0)
		$Camera.position.x = lerp($Camera.position.x, default_cam_pos.x, delta * 5.0)

func toggle_build_menu():
	build_menu_open = !build_menu_open
	if build_menu_open:
		$hud/build_menu/animations.play("toggle_build_menu")
	else:
		$hud/build_menu/animations.play_backwards("toggle_build_menu")

# --- Health System ---
func _process_health(delta: float) -> void:
	time_since_damaged += TimeManager.day_speed * delta
	if time_since_damaged >= health_regen_delay:
		health = clamp(
			health + ((health_regen * delta) * TimeManager.day_speed),
			0,
			health_max
		)
	$hud/healthbar.value = lerpf($hud/healthbar.value, (health / health_max) * 100.0, 0.2)

func take_damage(damage: float) -> void:
	time_since_damaged = 0
	health = clamp(health - damage, 0, health_max)
