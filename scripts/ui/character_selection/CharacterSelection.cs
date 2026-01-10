using System.Linq;
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
    [Export] private HBoxContainer _playerContainer;
    [Export] private HBoxContainer _weaponContainer;
    [Export] private TextureButton _playButton;
    [Export] private TextureButton _backButton;
    [Export] private AudioStreamPlayer _uiSound;
    [Export] private AudioStreamPlayer _hoverSound;

    private PackedScene _playerCardScene;
    private PackedScene _weaponCardScene;

    public override void _Ready()
    {
        Cursor.Instance.Sprite2D.Texture = _selectionCursor;

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
            card.Pressed += () => OnPlayerCardPressed(data, card);
            _playerContainer.AddChild(card);
            card.SetData(data);
        }

        foreach (var data in _weapons)
        {
            var card = (WeaponCard)_weaponCardScene.Instantiate();
            card.Pressed += () => OnWeaponCardPressed(data, card);
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

    private void OnPlayerCardPressed(PlayerData data, PlayerCard selectedCard)
    {
        _uiSound.Play();
        Global.Instance.SelectedPlayer = data;
        foreach (var card in _playerContainer.GetChildren().Cast<PlayerCard>()) card.Selector.Hide();
        selectedCard.Selector.Show();
    }

    private void OnWeaponCardPressed(WeaponData data, WeaponCard selectedCard)
    {
        _uiSound.Play();
        Global.Instance.SelectedWeapon = data;
        foreach (var card in _weaponContainer.GetChildren().Cast<WeaponCard>()) card.Selector.Hide();
        selectedCard.Selector.Show();
    }

    private void OnButtonMouseEntered()
    {
        _hoverSound.Play();
    }
}
