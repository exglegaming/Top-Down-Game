using System;
using Godot;
using Godot.Collections;
using TopDownGame.scripts.extra;
using TopDownGame.scripts.player;
using TopDownGame.scripts.resources.data.player;
using TopDownGame.scripts.resources.data.weapons;

namespace TopDownGame.scripts.autoloads;

public partial class Global : Node
{
    public static readonly PackedScene SpawnMarkerScene = GD.Load<PackedScene>("uid://bx7ulfs4833r");
    private static readonly Shader HitShader = GD.Load<Shader>("uid://bvtcpygj3gpx4");
    public static readonly ShaderMaterial HitMaterial = new() { Shader = HitShader };
    public static readonly PackedScene ChestScene = GD.Load<PackedScene>("uid://bpy8vjvvh0lf7");
    public static readonly PackedScene StoreItemScene = GD.Load<PackedScene>("uid://c4qfdnuhn8wf0");
    
    private const string Bunny = "uid://brqlu552oijhw";
    private const string Dog = "uid://c5oxdpdqidm62";
    private const string Mouse = "uid://dmur05tmvsy37";
    private const string Cat = "uid://btnvga7vtsw4b";
    private const string Ak47 = "uid://hjuhq6jbth5j";
    private const string Mac10 = "uid://btlepcaimi46b";
    private const string Mp5 = "uid://bcsqadn3w18im";
    private const string Pistol = "uid://b5pray2nwvym5";
    private const string Shotgun = "uid://cakp2tlpbqsq8";
    private const string Sniper = "uid://cll86688cg3ve";
    private const string Uzi = "uid://c64bomahf66fn";
    private const string Sword = "uid://0o45tdqijyqc";
    private const string Axe = "uid://cdf8e1ug5o42i";
    private static readonly PackedScene ExplosionEffectScene = GD.Load<PackedScene>("uid://gusc66iqufsn");
    private static readonly PackedScene DamageTextScene = GD.Load<PackedScene>("uid://bbodibp7vmq8c");
    private static readonly PackedScene DeadParticleScene = GD.Load<PackedScene>("uid://cco8b418ugmeg");
    private static readonly PackedScene BloodEffectScene = GD.Load<PackedScene>("uid://cf310gv286jmd");

    private string _savePath = "user://save.json";

    private Dictionary<string, PackedScene> AllPlayers { get; set; } = new()
    {
        { "Bunny", GD.Load<PackedScene>(Bunny) },
        { "Dog", GD.Load<PackedScene>(Dog) },
        { "Mouse", GD.Load<PackedScene>(Mouse) },
        { "Cat", GD.Load<PackedScene>(Cat) }
    };

    public Dictionary<string, PackedScene> AllWeapons { get; set; } = new()
    {
        { "AK47", GD.Load<PackedScene>(Ak47) },
        { "Mac 10", GD.Load<PackedScene>(Mac10) },
        { "MP5", GD.Load<PackedScene>(Mp5) },
        { "Pistol", GD.Load<PackedScene>(Pistol) },
        { "Shotgun", GD.Load<PackedScene>(Shotgun) },
        { "Sniper", GD.Load<PackedScene>(Sniper) },
        { "Uzi", GD.Load<PackedScene>(Uzi) },
        { "Sword", GD.Load<PackedScene>(Sword) },
        { "Axe", GD.Load<PackedScene>(Axe) }
    };
    
    public static Global Instance { get; private set; }

    public Player PlayerRef;
    public PlayerData SelectedPlayer;
    public WeaponData SelectedWeapon;
    public float Coins;
    public Dictionary<string, bool> Settings { get; set; } = new()
    {
        { "music", true },
        { "sfx", true },
        { "fullscreen", false }
    };

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        Instance.LoadData();
    }

    public PackedScene GetPlayer()
    {
        return AllPlayers[SelectedPlayer.Id];
    }

    public void CreateExplosion(Vector2 position)
    {
        var explosion = (Node2D)ExplosionEffectScene.Instantiate();
        explosion.GlobalPosition = position;
        GetTree().Root.AddChild(explosion);
    }

    public void CreateDamageText(float value, Vector2 position)
    {
        var damage = (DamageText)DamageTextScene.Instantiate();
        GetParent().AddChild(damage);
        var randomPosition = (float)GD.RandRange(0, Math.Tau);
        damage.GlobalPosition = position + Vector2.Right.Rotated(randomPosition) * 20;
        damage.Setup(value);
        
        var blood = (Node2D)BloodEffectScene.Instantiate();
        GetParent().AddChild(blood);
        blood.GlobalPosition = position;
    }

    public void CreateDeadParticle(Texture2D texture, Vector2 position)
    {
        var particle = (GpuParticles2D)DeadParticleScene.Instantiate();
        GetTree().Root.AddChild(particle);
        particle.GlobalPosition = position;
        particle.Texture = texture;
        particle.Emitting = true;
    }
    
    public void SaveData()
    {
        var save = Settings.Duplicate();

        using var file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        var jsonString = Json.Stringify(save);
        file.StoreString(jsonString);
    }
    
    private PackedScene GetWeapon()
    {
        return AllWeapons[SelectedWeapon.WeaponName];
    }

    private void LoadData()
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
