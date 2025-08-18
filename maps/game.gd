extends Node

@onready var player_scene = preload("res://player/player.tscn")
var players := {}

func _ready() -> void:
	TimeManager.sun = $Sun
	TimeManager.environment = $Environment

	# Spawn the local player
	_spawn_player(multiplayer.get_unique_id())

	# Listen for new peers
	NetworkManager.client_connected.connect(_on_peer_connected)

# Called on host when a new peer joins
@rpc("any_peer")
func _on_peer_connected(peer_id: int) -> void:
	# Spawn new player locally (host sees them)
	_spawn_player(peer_id)

	# Tell the new peer to spawn all existing players
	rpc("_spawn_all_existing_players", players.keys())

# Spawn a single player (local or remote)
func _spawn_player(peer_id: int) -> void:
	if players.has(peer_id):
		return

	var player = player_scene.instantiate()
	player.get_node("MultiplayerSynchronizer").set_multiplayer_authority(peer_id)
	players[peer_id] = player
	player.name = str(peer_id)

	# Determine if this is the actual local player
	player.is_local = peer_id == multiplayer.get_unique_id()

	if player.is_local:
		player.get_node("Camera").current = true
		player.get_node("hud").show()
	
	add_child(player)

# RPC for late joiners to spawn all existing players
@rpc("any_peer", "call_local")
func _spawn_all_existing_players(existing_peer_ids: Array) -> void:
	for peer_id in existing_peer_ids:
		_spawn_player(peer_id)
