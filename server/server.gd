extends Node

func _ready():
	print("Dedicated server starting...")
	var nm = load("res://network/NetworkManager.gd").new()
	add_child(nm)
	nm.host_game()
	# Optionally load Game.tscn here if the server should simulate the world
