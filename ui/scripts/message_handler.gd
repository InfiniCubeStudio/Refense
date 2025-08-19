extends Control

@onready var message_box = preload("res://ui/message_box.tscn")
@onready var action_box = preload("res://ui/action_box.tscn")

func show_messageBox(text,option,to_call = null):
	var new_message = message_box.instantiate()
	new_message.add_to_group("messages")
	new_message.get_node("text").text = text
	new_message.get_node("button").text = option
	new_message.to_call = to_call
	get_tree().root.add_child(new_message)
	return new_message

func show_actionBox(text):
	var new_message = action_box.instantiate()
	new_message.add_to_group("messages")
	new_message.get_node("text").text = text
	print(len(get_tree().get_nodes_in_group("messages")))
	get_tree().root.add_child(new_message)
	return new_message
