using Godot;
using Godot.Collections;
using TopDownGame.scripts.resources.data.player;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.autoloads;

public partial class Global : Node
{
    public static Global Instance { get; private set; }

    private string _savePath = "user://save.json";

    public Dictionary<string, bool> Settings { get; private set; } = new Dictionary<string, bool>
    {
        { "music", true },
        { "sfx", true },
        { "fullscreen", false }
    };

    public Dictionary<string, PackedScene> AllPlayers { get; private set; } = new Dictionary<string, PackedScene>
    {
        { "Bunny", GD.Load<PackedScene>("uid://brqlu552oijhw") },
        { "Dog", GD.Load<PackedScene>("uid://c5oxdpdqidm62") },
        { "Mouse", GD.Load<PackedScene>("uid://dmur05tmvsy37") },
        { "Cat", GD.Load<PackedScene>("uid://btnvga7vtsw4b") }
    };

    public PlayerData SelectedPlayer;
    public WeaponData SelectedWeapon;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public PackedScene GetPlayer()
    {
        return AllPlayers[SelectedPlayer.ID];
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

        var loadedDict = data.AsGodotDictionary();
        Settings.Clear();
        foreach (var key in loadedDict)
        {
            Settings[key.Key.ToString()] = (bool)key.Value;
        }
    }
}
