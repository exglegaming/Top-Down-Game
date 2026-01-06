using Godot;
using TopDownGame.scripts.autoloads;
using TopDownGame.scripts.resources.data.player;
using TopDownGame.scripts.resources.data.weapons;
using TopDownGame.scripts.ui.player_card;
using TopDownGame.scripts.ui.weapon_card;

namespace TopDownGame.scripts.ui.character_selection;

public partial class CharacterSelection : Control
{
    [ExportCategory("References")]
    [Export] private Texture2D _selectionCursor;
    [Export] private PlayerData[] _players;
    [Export] private WeaponData[] _weapons;

    private HBoxContainer _playerContainer;
    private HBoxContainer _weaponContainer;
    private PackedScene _playerCardScene;
    private PackedScene _weaponCardScene;

    public override void _Ready()
    {
        Cursor.Instance.Sprite2D.Texture = _selectionCursor;

        _playerContainer = GetNode<HBoxContainer>("PlayerContainer");
        _weaponContainer = GetNode<HBoxContainer>("WeaponContainer");

        _playerCardScene = GD.Load<PackedScene>("uid://bag7ifm3ms8g5");
        _weaponCardScene = GD.Load<PackedScene>("uid://b1hduu5435thl");

        LoadSelectionItems();
    }

    private void LoadSelectionItems()
    {
        foreach (var node in _playerContainer.GetChildren()) node.QueueFree();
        foreach (var node in _weaponContainer.GetChildren()) node.QueueFree();

        foreach (var data in _players)
        {
            var card = (PlayerCard)_playerCardScene.Instantiate();
            _playerContainer.AddChild(card);
            card.SetData(data);
        }

        foreach (var data in _weapons)
        {
            var card = (WeaponCard)_weaponCardScene.Instantiate();
            _weaponContainer.AddChild(card);
            card.SetData(data);
        }
    }
}
