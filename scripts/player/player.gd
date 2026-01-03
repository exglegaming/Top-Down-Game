class_name Player
extends CharacterBody2D

@export_category("References")
@export var data: PlayerData

var can_move: bool = true
var movement: Vector2
var direction: Vector2

@onready var visuals: Node2D = $Visuals
@onready var animated_sprite_2d: AnimatedSprite2D = %AnimatedSprite2D
@onready var health_component: HealthComponent = $HealthComponent


func _ready() -> void:
    health_component.on_unit_damaged.connect(_on_health_component_on_unit_damaged)
    health_component.on_unit_dead.connect(_on_health_component_on_unit_dead)
    health_component.on_unit_healed.connect(_on_health_component_on_unit_healed)

    health_component.init_health(data.max_hp)


func _physics_process(_delta: float) -> void:
    if !can_move:
        return
    
    direction = Input.get_vector("move_left", "move_right", "move_up", "move_down")
    if direction != Vector2.ZERO:
        movement = direction * data.move_speed
        animated_sprite_2d.play("move")
    else:
        movement = Vector2.ZERO
        animated_sprite_2d.play("idle")

    velocity = movement
    rotate_player()
    move_and_slide()


func _input(event: InputEvent) -> void:
    if event.is_action_pressed("ui_accept"):
        health_component.take_damage(1)


func rotate_player() -> void:
    if direction != Vector2.ZERO && direction.x >= 0.1:
        visuals.scale = Vector2(1.25, 1.25)
    elif direction != Vector2.ZERO && direction.x <= -0.1:
        visuals.scale = Vector2(-1.25, 1.25)


func _on_health_component_on_unit_damaged(_amount: float) -> void:
    EventBus.on_player_health_updated.emit(health_component.current_health, data.max_hp)


func _on_health_component_on_unit_dead() -> void:
    queue_free()


func _on_health_component_on_unit_healed(_amount: float) -> void:
    pass
