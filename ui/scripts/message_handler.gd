extends Control

@onready var message = preload("res://ui/message.tscn")

func display_message(text):
	var new_message = message.instantiate()
	new_message.get_node("message").text = text
	get_tree().root.add_child(new_message)
	return new_message
