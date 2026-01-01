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
@onready var ui_sound: AudioStreamPlayer = $UISound
@onready var music_label: Label = %MusicLabel
@onready var sfx_label: Label = %SFXLabel
@onready var window_label: Label = %WindowLabel


func _ready() -> void:
    play_button.pressed.connect(_on_play_button_pressed)
    settings_button.pressed.connect(_on_settings_button_pressed)
    quit_button.pressed.connect(_on_quit_button_pressed)
    music_button.pressed.connect(_on_music_button_pressed)
    sfx_button.pressed.connect(_on_sfx_button_pressed)
    window_button.pressed.connect(_on_window_button_pressed)
    back_button.pressed.connect(_on_back_button_pressed)

    update_audio_bus("Music", music_label, Global.settings.music)
    update_audio_bus("SFX", sfx_label, Global.settings.sfx)
    update_fullscreen(Global.settings.fullscreen)


func update_audio_bus(bus_name: String, label: Label, is_on: bool) -> void:
    AudioServer.set_bus_mute(AudioServer.get_bus_index(bus_name), !is_on)
    label.text = "%s: %s" % [bus_name.to_upper(), "ON" if is_on else "OFF"]


func update_fullscreen(is_on: bool) -> void:
    var mode: DisplayServer.WindowMode = DisplayServer.WINDOW_MODE_FULLSCREEN if is_on else DisplayServer.WINDOW_MODE_WINDOWED
    DisplayServer.window_set_mode(mode)
    window_label.text = "FULSCREEN" if is_on else "WINDOWED"


func _on_play_button_pressed() -> void:
    ui_sound.play()
    Transition.transition_to("uid://cinbkpaw7sfcs")


func _on_settings_button_pressed() -> void:
    ui_sound.play()
    var tween: Tween = create_tween()
    tween.tween_property(main_buttons, "global_position:y", 350, 0.2)
    tween.tween_interval(0.1)
    tween.tween_property(settings_buttons, "global_position:x", 145, 0.3)


func _on_quit_button_pressed() -> void:
    ui_sound.play()
    get_tree().quit()


func _on_music_button_pressed() -> void:
    ui_sound.play()
    update_audio_bus("Music", music_label, Global.settings.music)


func _on_sfx_button_pressed() -> void:
    ui_sound.play()
    update_audio_bus("SFX", sfx_label, Global.settings.sfx)


func _on_window_button_pressed() -> void:
    ui_sound.play()
    update_fullscreen(Global.settings.fullscreen)


func _on_back_button_pressed() -> void:
    ui_sound.play()
    var tween: Tween = create_tween()
    tween.tween_property(settings_buttons, "global_position:x", 558, 0.3)
    tween.tween_interval(0.1)
    tween.tween_property(main_buttons, "global_position:y", 115, 0.2)
