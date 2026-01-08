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
    private TextureButton _playButton;
    private TextureButton _backButton;
    private AudioStreamPlayer _uiSound;
    private AudioStreamPlayer _hoverSound;

    public override void _Ready()
    {
        Cursor.Instance.Sprite2D.Texture = _selectionCursor;

        _playerContainer = GetNode<HBoxContainer>("PlayerContainer");
        _weaponContainer = GetNode<HBoxContainer>("WeaponContainer");
        _playButton = GetNode<TextureButton>("%PlayButton");
        _backButton = GetNode<TextureButton>("%BackButton");
        _uiSound = GetNode<AudioStreamPlayer>("UISound");
        _hoverSound = GetNode<AudioStreamPlayer>("HoverSound");

        _playerCardScene = GD.Load<PackedScene>("uid://bag7ifm3ms8g5");
        _weaponCardScene = GD.Load<PackedScene>("uid://b1hduu5435thl");

        LoadSelectionItems();

        _playButton.Pressed += OnPlayButtonPressed;
        _backButton.Pressed += OnBackButtonPressed;
        _playButton.MouseEntered += OnButtonMouseEntered;
        _backButton.MouseEntered += OnButtonMouseEntered;
    }

    private void LoadSelectionItems()
    {
        foreach (var node in _playerContainer.GetChildren()) node.QueueFree();
        foreach (var node in _weaponContainer.GetChildren()) node.QueueFree();

        foreach (var data in _players)
        {
            var card = (PlayerCard)_playerCardScene.Instantiate();
            card.Pressed += () => OnPlayerCardPressed(data);
            _playerContainer.AddChild(card);
            card.SetData(data);
        }

        foreach (var data in _weapons)
        {
            var card = (WeaponCard)_weaponCardScene.Instantiate();
            card.Pressed += () => OnWeaponCardPressed(data);
            _weaponContainer.AddChild(card);
            card.SetData(data);
        }
    }

    private void OnPlayButtonPressed()
    {
        if (Global.Instance.SelectedPlayer == null && Global.Instance.SelectedWeapon == null)
        {
            GD.Print("No player and weapon selected");
            return;
        }

        if (Global.Instance.SelectedPlayer == null)
        {
            GD.Print("No player selected");
            return;
        }

        if (Global.Instance.SelectedWeapon == null)
        {
            GD.Print("No weapon selected");
            return;
        }

        _uiSound.Play();
        Transition.Instance.TransitionTo("uid://dheb1iulvcciu");
    }

    private void OnBackButtonPressed()
    {
        _uiSound.Play();
        Transition.Instance.TransitionTo("uid://bdmo5icd2xpue");
    }

    private void OnPlayerCardPressed(PlayerData data)
    {
        _uiSound.Play();
        Global.Instance.SelectedPlayer = data;
    }

    private void OnWeaponCardPressed(WeaponData data)
    {
        _uiSound.Play();
        Global.Instance.SelectedWeapon = data;
    }

    private void OnButtonMouseEntered()
    {
        _hoverSound.Play();
    }
}
