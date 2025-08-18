extends Control

@onready var message = preload("res://ui/message.tscn")

func display_message(text,option,to_call = null):
	var new_message = message.instantiate()
	new_message.get_node("message").text = text
	new_message.get_node("button").text = option
	new_message.to_call = to_call
	get_tree().root.add_child(new_message)
	return new_message
