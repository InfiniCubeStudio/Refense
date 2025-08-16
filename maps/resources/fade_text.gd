extends Label

@export var fade_duration: float = 3.0
@export var hold_duration: float = 1.0  # Time to stay fully visible before fading out

func _ready():
	modulate.a = 0.0  # Start fully transparent

func fade_in_and_out():
	var tween = create_tween()
	# Fade in
	tween.tween_property(self, "modulate:a", 1.0, fade_duration)
	# Wait for hold_duration, then fade out
	tween.tween_interval(hold_duration)
	tween.tween_property(self, "modulate:a", 0.0, fade_duration)
