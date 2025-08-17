extends Node

@onready var player_scene = preload("res://player/player.tscn")

# Keep track of players by peer ID
var players := {}

func _ready() -> void:
	var my_id = multiplayer.get_unique_id()
	_spawn_player(my_id, true)  # Spawn local/host player first

	# Listen for new peers connecting
	NetworkManager.client_connected.connect(_on_peer_connected)

# Called when a new peer joins
func _on_peer_connected(peer_id: int) -> void:
	if peer_id == multiplayer.get_unique_id():
		return

	# Spawn the new client locally (host sees new player)
	_spawn_player(peer_id, false)

	# Tell the new client to spawn the host player
	var host_id = multiplayer.get_unique_id()
	rpc_id(peer_id, "_spawn_remote_player", host_id)

# Spawn a player locally
func _spawn_player(peer_id: int, local: bool) -> void:
	if players.has(peer_id):
		return  # Already spawned

	var player = player_scene.instantiate()
	player.name = str(peer_id)
	add_child(player)
	player.is_local = local

	if local:
		player.get_node("Camera").current = true
		player.get_node("hud").show()

	players[peer_id] = player

	# Notify other peers if this is the host
	if local and multiplayer.is_server():
		for other_peer in multiplayer.get_peers():
			if other_peer != multiplayer.get_unique_id():
				rpc_id(other_peer, "_spawn_remote_player", peer_id)

# RPC called on all clients to spawn remote players
@rpc("any_peer", "call_local")
func _spawn_remote_player(peer_id: int) -> void:
	if peer_id == multiplayer.get_unique_id() or players.has(peer_id):
		return

	var player = player_scene.instantiate()
	player.name = str(peer_id)
	add_child(player)
	player.is_local = false
	players[peer_id] = player
