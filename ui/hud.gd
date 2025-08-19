extends Control

@onready var time_label = $clock/timer
@onready var day_text = $clock/day_text
@onready var next_day_text = $"next_day_text"
@onready var day_marker = $clock/Path2D/time

func _ready() -> void:
	TimeManager.connect("time_updated", Callable(self, "_on_time_updated"))

func _on_time_updated(in_game_time: float, day: int, cycle: String) -> void:
	# Move clock hand
	day_marker.progress_ratio = in_game_time
	# Update text
	time_label.text = TimeManager.get_time_string(in_game_time)
	if cycle == "night":
		day_text.text = "Night " + str(day)
	elif cycle == "day":
		day_text.text = "Day " + str(day)
		next_day_text.text = "Day " + str(day)
		next_day_text.fade_in_and_out()
# called every frame. easy fix but i need to redo it
