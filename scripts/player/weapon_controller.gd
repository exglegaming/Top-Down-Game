class_name WeaponController
extends Node2D


var current_weapon: Weapon
var target_position: Vector2

@onready var weapon_range_pistol: WeaponRange = $WeaponRangePistol


func _ready() -> void:
    current_weapon = weapon_range_pistol


func _process(_delta: float) -> void:
    target_position = get_global_mouse_position()
    rotate_weapon()


func rotate_weapon() -> void:
    if !current_weapon: return

    current_weapon.pivot.look_at(target_position)
