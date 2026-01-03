class_name WeaponRange
extends Weapon


var direction: Vector2

@onready var sprite_2d: Sprite2D = %Sprite2D


func _process(_delta: float) -> void:
    rotate_weapon()


func use_weapon() -> void:
    pass


func rotate_weapon() -> void:
    direction = get_global_mouse_position() - global_position
    sprite_2d.flip_v = direction.x < 0
