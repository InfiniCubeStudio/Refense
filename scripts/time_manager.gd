extends Node

signal time_updated(in_game_time, day, cycle)

var in_game_time := 0.2085   # 0.0 → start of day, 1.0 → end of day
var day_length := 480.0
var day_speed := 1.0
var cloud_position := 0.0

var day := 0
var cycle := "midnight"

var sun = null
var environment = null

func _process(delta: float) -> void:
	# Advance time
	in_game_time += (delta / day_length) * day_speed
	if in_game_time > 1.0:
		onTimeMidnight()
	if in_game_time > 0.2084 and cycle != "day" and cycle != "night":
		onTimeDawn()
	if in_game_time > 0.7916 and cycle != "night":
		onTimeNight()

	# Fast forward test
	if Input.is_key_pressed(KEY_BACKSPACE):
		day_speed = 100
	else:
		day_speed = 1
		
	cloud_position += (0.8 * day_speed) * delta

	_update_sun()
	_update_shader()

	# Notify anyone listening (like HUD)
	emit_signal("time_updated", in_game_time, day, cycle)

func get_time_string(time: float) -> String:
	var total_hours = time * 24.0
	var hours = int(total_hours) % 24
	var minutes = int((total_hours - hours) * 60.0)

	var display_hour = hours % 12
	if display_hour == 0:
		display_hour = 12

	var am_pm = "AM" if hours < 12 else "PM"
	return str(display_hour) + ":" + str(minutes).pad_zeros(2) + " " + am_pm

func _update_sun() -> void:
	if is_instance_valid(sun):
		var day_progress : float = in_game_time
		sun.rotation.x = (day_progress * 2.0 - 0.5) * -PI
		var sun_dir = sun.to_global(Vector3.FORWARD).normalized()
		sun.light_energy = max(0.0, sun_dir.y) * 2.0

func _update_shader() -> void:
	if is_instance_valid(environment):
		var mat : ShaderMaterial = environment.environment.sky.sky_material
		if mat:
			mat.set_shader_parameter("overwritten_time", cloud_position)

func onTimeNight() -> void:
	cycle = "night"

func onTimeMidnight() -> void:
	in_game_time = 0.0
	cycle = "midnight"

func onTimeDawn() -> void:
	cycle = "day"
	day += 1
