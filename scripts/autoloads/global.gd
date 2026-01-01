extends Node

var save_path: String = "user://save.json"
var settings: Dictionary = {
    "music": true,
    "sfx": true,
    "fullscreen": false
}


func save_data() -> void:
    var save: Dictionary = settings.duplicate()

    var file: FileAccess = FileAccess.open(save_path, FileAccess.WRITE)
    var json_string: String = JSON.stringify(save)
    file.store_string(json_string)
    file.close()


func load_data() -> void:
    if !FileAccess.file_exists(save_path):
        return
    
    var file: FileAccess = FileAccess.open(save_path, FileAccess.READ)
    var json: String = file.get_as_text()
    var data: Variant = JSON.parse_string(json)
    file.close()

    settings = data
