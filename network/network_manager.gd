extends Node

signal server_ready
signal client_connected(peer_id: int)

var multiplayer_peer: ENetMultiplayerPeer

func host_game(port: int = 4242):
	multiplayer_peer = ENetMultiplayerPeer.new()
	var err = multiplayer_peer.create_server(port, 16)
	if err != OK:
		push_error("Failed to host server: %s" % err)
		return
	multiplayer.multiplayer_peer = multiplayer_peer
	print("Server hosted on port ", port)
	emit_signal("server_ready")

func join_game(ip: String = "127.0.0.1", port: int = 4242):
	multiplayer_peer = ENetMultiplayerPeer.new()
	var err = multiplayer_peer.create_client(ip, port)
	if err != OK:
		push_error("Failed to connect: %s" % err)
		return
	multiplayer.multiplayer_peer = multiplayer_peer
	multiplayer.peer_connected.connect(Callable(self, "_on_peer_connected"))
	multiplayer.connected_to_server.connect(Callable(self, "_on_connection_succeeded"))
	print("Joining ", ip, ":", port)

func _on_peer_connected(id: int) -> void:
	print("Peer connected: ", id)
	emit_signal("client_connected", id)

func _on_connection_succeeded() -> void:
	print("Connected to server")
	emit_signal("client_connected", multiplayer.get_unique_id())
