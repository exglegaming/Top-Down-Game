extends CanvasLayer


@onready var sprite_2d: Sprite2D = $Sprite2D


func _ready() -> void:
    Input.set_mouse_mode(Input.MOUSE_MODE_HIDDEN)


func _process(_delta: float) -> void:
    sprite_2d.position = get_viewport().get_mouse_position()
