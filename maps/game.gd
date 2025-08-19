extends Node

@onready var player_scene = preload("res://player/player.tscn")
var players := {}

func _ready() -> void:
	TimeManager.sun = $Sun
	TimeManager.environment = $Environment

	# Spawn the local player
	_spawn_player(multiplayer.get_unique_id(), NetworkManager.username)

	# Listen for new peers
	NetworkManager.client_connected.connect(_on_peer_connected)

@rpc("any_peer")
func _on_peer_connected(peer_id: int, username: String) -> void:
	# Spawn new player on the host side
	_spawn_player(peer_id, username)

	rpc("_spawn_all_existing_players", players)


# Spawn a single player (local or remote)
func _spawn_player(peer_id: int, username: String) -> void:
	if players.has(peer_id):
		return

	var player = player_scene.instantiate()
	player.get_node("MultiplayerSynchronizer").set_multiplayer_authority(peer_id)
	player.name = str(peer_id)
	player.username = username
	player.is_local = peer_id == multiplayer.get_unique_id()

	if player.is_local:
		player.get_node("Camera").current = true
		player.get_node("hud").show()
	else:
		MessageHandler.show_actionBox(username+" joined the game!")

	add_child(player)

	# store player and username together
	players[peer_id] = {"node": player, "username": username}

@rpc("any_peer", "call_local")
func _despawn_player(peer_id: int) -> void:
	print("Despawning player...")
	if not players.has(peer_id):
		return
	var player = players[peer_id]
	MessageHandler.show_actionBox(player.username+" left the game!")
	if typeof(player) == TYPE_DICTIONARY and player.has("node"):
		player = player["node"]
	if player and player.is_inside_tree():
		player.queue_free()
	players.erase(peer_id)


# RPC for late joiners to spawn all existing players
@rpc("any_peer", "call_local")
func _spawn_all_existing_players(existing_players: Dictionary) -> void:
	for peer_id in existing_players.keys():
		_spawn_player(peer_id, existing_players[peer_id].username)
