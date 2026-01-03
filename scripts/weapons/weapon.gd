class_name Weapon
extends Node2D


@export_category("References")
@export var data: WeaponData

@onready var pivot: Node2D = $Pivot


func use_weapon() -> void:
    pass
