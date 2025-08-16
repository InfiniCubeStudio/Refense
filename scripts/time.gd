extends Control

@onready var day_marker = $Path2D/time  # attach your sprite here
@onready var time_label = $timer
@onready var day_label = $day

var in_game_time = 0.2085;   # 0.0 → start of day, 1.0 → end of day
var day_length = 480.0;
var day_speed = 1;
var cloud_position = 0.0;

var day = 0;
var cycle = "midnight";

func _process(delta):
	# Advance time: 1 full day every 60 seconds (for example)
	in_game_time += (delta / day_length) * day_speed
	if in_game_time > 1.0:
		onTimeMidnight()
	if in_game_time > 0.2084 and cycle != "day" and cycle != "night":
		onTimeDawn()
	if in_game_time > 0.7916 and cycle != "night":
		onTimeNight()
	if Input.is_key_pressed(KEY_BACKSPACE):
		day_speed = 100;
	else:
		day_speed = 1;
		
	cloud_position += .008*day_speed

	# Move sprite along curve
	day_marker.progress_ratio = in_game_time
	time_label.text = str(get_time_string(in_game_time))

	_update_sun()
	_update_shader()

func get_time_string(in_game_time: float) -> String:
	var total_hours = in_game_time * 24.0
	var hours = int(total_hours) % 24
	var minutes = int((total_hours - hours) * 60.0)

	var display_hour = hours % 12
	if display_hour == 0:
		display_hour = 12

	var am_pm = "AM" if hours < 12 else "PM"
	return str(display_hour) + ":" + str(minutes).pad_zeros(2) + " " + am_pm

@onready var sun : DirectionalLight3D = $"../../../Sun"
@onready var environment : WorldEnvironment = $"../../../Skybox"

func _update_sun() -> void:
	if is_instance_valid(sun):
		var day_progress : float = in_game_time
		# Sun rotates around x-axis (0.0 sunrise → 0.5 sunset → 1.0 back to sunrise)
		sun.rotation.x = (day_progress * 2.0 - 0.5) * -PI
		# Simple horizon check to disable light below horizon
		var sun_dir = sun.to_global(Vector3.FORWARD).normalized()
		sun.light_energy = max(0.0, sun_dir.y) * 2.0 # adjust multiplier for brightness

func _update_shader() -> void:
	if is_instance_valid(environment):
		var mat : ShaderMaterial = environment.environment.sky.sky_material
		if mat:
			mat.set_shader_parameter("overwritten_time", cloud_position)

func onTimeNight() -> void:
	$day.text = "Night " + str(day);
	cycle = "night";

func onTimeMidnight() -> void:
	in_game_time = 0.0
	cycle = "midnight";

func onTimeDawn() -> void:
	cycle = "day";
	day += 1;
	$day.text = "Day " + str(day);
	$"../next_day_text".text = "Day " + str(day);
	$"../next_day_text".fade_in_and_out()
