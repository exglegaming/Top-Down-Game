using Godot;
using Godot.Collections;
using TopDownGame.scripts.resources.data.player;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.autoloads;

public partial class Global : Node
{
    private static readonly string Bunny = "uid://brqlu552oijhw";
    private static readonly string Dog = "uid://c5oxdpdqidm62";
    private static readonly string Mouse = "uid://dmur05tmvsy37";
    private static readonly string Cat = "uid://btnvga7vtsw4b";
    private static readonly string AK47 = "uid://hjuhq6jbth5j";
    private static readonly string Mac10 = "uid://btlepcaimi46b";
    private static readonly string MP5 = "uid://bcsqadn3w18im";
    private static readonly string Pistol = "uid://b5pray2nwvym5";
    private static readonly string Shotgun = "uid://cakp2tlpbqsq8";
    private static readonly string Sniper = "uid://cll86688cg3ve";
    private static readonly string Uzi = "uid://c64bomahf66fn";

    public static Global Instance { get; set; }

    private string _savePath = "user://save.json";

    public PlayerData SelectedPlayer;
    public WeaponData SelectedWeapon;
    public Dictionary<string, bool> Settings { get; set; } = new Dictionary<string, bool>
    {
        { "music", true },
        { "sfx", true },
        { "fullscreen", false }
    };

    public Dictionary<string, PackedScene> AllPlayers { get; set; } = new Dictionary<string, PackedScene>
    {
        { "Bunny", GD.Load<PackedScene>(Bunny) },
        { "Dog", GD.Load<PackedScene>(Dog) },
        { "Mouse", GD.Load<PackedScene>(Mouse) },
        { "Cat", GD.Load<PackedScene>(Cat) }
    };

    public Dictionary<string, PackedScene> AllWeapons { get; set; } = new Dictionary<string, PackedScene>
    {
        { "AK47", GD.Load<PackedScene>(AK47) },
        { "Mac 10", GD.Load<PackedScene>(Mac10) },
        { "MP5", GD.Load<PackedScene>(MP5) },
        { "Pistol", GD.Load<PackedScene>(Pistol) },
        { "Shotgun", GD.Load<PackedScene>(Shotgun) },
        { "Sniper", GD.Load<PackedScene>(Sniper) },
        { "Uzi", GD.Load<PackedScene>(Uzi) }
    };

    public override void _EnterTree()
    {
        Instance = this;
    }

    public PackedScene GetPlayer()
    {
        return AllPlayers[SelectedPlayer.ID];
    }
    
    public PackedScene GetWeapon()
    {
        return AllWeapons[SelectedWeapon.WeaponName];
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
