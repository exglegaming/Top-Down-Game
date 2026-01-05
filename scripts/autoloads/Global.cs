using Godot;
using Godot.Collections;

namespace TopDownGame.scripts.autoloads;

public partial class Global : Node
{
    public static Global Instance { get; private set; }

    private string _savePath = "user://save.json";

    public Dictionary Settings { get; private set; } = new Dictionary
    {
        { "music", true },
        { "sfx", true },
        { "fullscreen", false }
    };

    public override void _EnterTree()
    {
        Instance = this;
    }
    
    public void SaveData()
    {
        var save = Settings.Duplicate();

        using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(save);
        file.StoreString(jsonString);
    }

    public void LoadData()
    {
        if (!FileAccess.FileExists(_savePath))
            return;

        using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
        var json = file.GetAsText();
        var data = Json.ParseString(json);

        Settings = data.AsGodotDictionary();
    }
}
