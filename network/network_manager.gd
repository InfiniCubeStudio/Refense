extends Node

signal server_ready
signal client_connected(peer_id: int)

var multiplayer_peer: ENetMultiplayerPeer
var currentMessage;

func host_game(port: int = 4242):
	multiplayer_peer = ENetMultiplayerPeer.new()
	var err = multiplayer_peer.create_server(port, 16)
	print(err)
	if err == OK:
		multiplayer.multiplayer_peer = multiplayer_peer
		multiplayer.peer_connected.connect(Callable(self, "_on_peer_connected"))
		print("Server hosted on port ", port)
		get_tree().change_scene_to_file("res://maps/level.tscn")
		emit_signal("server_ready")
	return err

func join_game(ip: String = "127.0.0.1", port: int = 4242):
	multiplayer_peer = ENetMultiplayerPeer.new()
	var err = multiplayer_peer.create_client(ip, port)
	print(err)
	if err == OK:
		multiplayer.multiplayer_peer = multiplayer_peer
		multiplayer.connection_failed.connect(Callable(self, "_on_connection_failed"))
		multiplayer.connected_to_server.connect(Callable(self, "_on_connection_succeeded"))
		multiplayer.server_disconnected.connect(Callable(self, "_on_server_disconnect"))
		currentMessage = MessageHandler.display_message("Connecting to server...","Cancel",Callable(self,"disconnect_from_server"))
	return err

func disconnect_from_server():
	get_tree().change_scene_to_file("res://ui/main.tscn")
	multiplayer.multiplayer_peer = null;
	MessageHandler.display_message("Disconnected from server.","Ok")

func _on_peer_connected(id: int) -> void:
	print("Peer connected: ", id)
	print(id)
	if multiplayer.is_server():
		rpc_id(id, "receive_server_data", get_server_data())
		print("Sending serverData")
	emit_signal("client_connected", id)

func _on_connection_succeeded() -> void:
	currentMessage.queue_free()
	print("Connected to server")
	emit_signal("client_connected", multiplayer.get_unique_id())
	get_tree().change_scene_to_file("res://maps/level.tscn")
	
func _on_connection_failed() -> void:
	currentMessage.queue_free()
	MessageHandler.display_message("Failed to connect to server.","Ok")
	

func get_server_data():
	var serverData = {}
	serverData.time = [TimeManager.in_game_time,TimeManager.day]
	print("Compiled serverData: "+str(serverData))
	return serverData

@rpc("reliable")
func receive_server_data(serverData) -> void:
	print("Recieved serverData: "+str(serverData))
	# Sync Times
	TimeManager.in_game_time = serverData.time[0]
	TimeManager.day = serverData.time[1]

func _on_server_disconnect():
	get_tree().change_scene_to_file("res://ui/main.tscn")
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	MessageHandler.display_message("Lost connection to server.","Ok")
