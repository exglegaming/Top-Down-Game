class_name MainMenu
extends Control


@onready var main_buttons: Control = $MainButtons
@onready var settings_buttons: Control = $SettingsButtons
@onready var play_button: TextureButton = %PlayButton
@onready var settings_button: TextureButton = %SettingsButton
@onready var quit_button: TextureButton = %QuitButton
@onready var music_button: TextureButton = %MusicButton
@onready var sfx_button: TextureButton = %SFXButton
@onready var window_button: TextureButton = %WindowButton
@onready var back_button: TextureButton = %BackButton


func _ready() -> void:
    play_button.pressed.connect(_on_play_button_pressed)
    settings_button.pressed.connect(_on_settings_button_pressed)
    quit_button.pressed.connect(_on_quit_button_pressed)
    music_button.pressed.connect(_on_music_button_pressed)
    sfx_button.pressed.connect(_on_sfx_button_pressed)
    window_button.pressed.connect(_on_window_button_pressed)
    back_button.pressed.connect(_on_back_button_pressed)


func _on_play_button_pressed() -> void:
    pass


func _on_settings_button_pressed() -> void:
    var tween: Tween = create_tween()
    tween.tween_property(main_buttons, "global_position:y", 350, 0.2)
    tween.tween_interval(0.1)
    tween.tween_property(settings_buttons, "global_position:x", 145, 0.3)


func _on_quit_button_pressed() -> void:
    pass


func _on_music_button_pressed() -> void:
    pass


func _on_sfx_button_pressed() -> void:
    pass


func _on_window_button_pressed() -> void:
    pass


func _on_back_button_pressed() -> void:
    var tween: Tween = create_tween()
    tween.tween_property(settings_buttons, "global_position:x", 558, 0.3)
    tween.tween_interval(0.1)
    tween.tween_property(main_buttons, "global_position:y", 115, 0.2)
