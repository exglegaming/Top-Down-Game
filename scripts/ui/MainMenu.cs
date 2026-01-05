using Godot;
using TopDownGame.Scritps.Autoloads;

namespace TopDownGame.Scritps.UI;

public partial class MainMenu : Control
{
    [ExportCategory("References")]
    [Export] private Texture2D menuCursor;

    private Control _mainButtons;
    private Control _settingsButtons;
    private TextureButton _playButton;
    private TextureButton _settingsButton;
    private TextureButton _quitButton;
    private TextureButton _musicButton;
    private TextureButton _sfxButton;
    private TextureButton _windowButton;
    private TextureButton _backButton;
    private AudioStreamPlayer _uiSound;
    private Label _musicLabel;
    private Label _sfxLabel;
    private Label _windowLabel;

    public override void _Ready()
    {
        _mainButtons = GetNode<Control>("MainButtons");
        _settingsButtons = GetNode<Control>("SettingsButtons");
        _playButton = GetNode<TextureButton>("%PlayButton");
        _settingsButton = GetNode<TextureButton>("%SettingsButton");
        _quitButton = GetNode<TextureButton>("%QuitButton");
        _musicButton = GetNode<TextureButton>("%MusicButton");
        _sfxButton = GetNode<TextureButton>("%SFXButton");
        _windowButton = GetNode<TextureButton>("%WindowButton");
        _backButton = GetNode<TextureButton>("%BackButton");
        _uiSound = GetNode<AudioStreamPlayer>("UISound");
        _musicLabel = GetNode<Label>("%MusicLabel");
        _sfxLabel = GetNode<Label>("%SFXLabel");
        _windowLabel = GetNode<Label>("%WindowLabel");

        // Global.LoadData();
        Cursor.Instance._sprite2D.Texture = menuCursor;
        
        _playButton.Pressed += OnPlayButtonPressed;
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _musicButton.Pressed += OnMusicButtonPressed;
        _sfxButton.Pressed += OnSfxButtonPressed;
        _windowButton.Pressed += OnWindowButtonPressed;
        _backButton.Pressed += OnBackButtonPressed;
    }

    private void UpdateAudioBus(string busName, Label label, bool isOn)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex(busName), !isOn);
        label.Text = $"{busName.ToUpper()}: {(isOn ? "ON" : "OFF")}";
    }

    private void UpdateFullscreen(bool isOn)
    {
        var mode = isOn ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
        DisplayServer.WindowSetMode(mode);
        _windowLabel.Text = $"{(isOn ? "FULLSCREEN" : "WINDOWED")}";
    }

    private void OnPlayButtonPressed()
    {
        _uiSound.Play();
        // Transistion.TransitionTo("uid://dheb1iulvcciu");
    }

    private void OnSettingsButtonPressed()
    {
        _uiSound.Play();
        var tween = CreateTween();
        tween.TweenProperty(_mainButtons, "global_position:y", 350, 0.2);
        tween.TweenInterval(0.1);
        tween.TweenProperty(_settingsButton, "global_position:x", 145, 0.3);
    }

    private void OnQuitButtonPressed()
    {
        _uiSound.Play();
        // Global.SaveData();
        GetTree().Quit();
    }

    private void OnMusicButtonPressed()
    {
        _uiSound.Play();
        // Global.settings.music = !Global.settings.music
        // UpdateAudioBus("Music", _musicLabel, Global.settings.music);
    }

    private void OnSfxButtonPressed()
    {
        _uiSound.Play();
        // Global.settings.sfx = !Global.settings.sfx
        // UpdateAudioBus("SFX", _sfxLabel, Global.settings.sfx);
    }

     private void OnWindowButtonPressed()
    {
        _uiSound.Play();
        // Global.settings.fullscreen = !Global.settings.fullscreen
        // UpdateFullscreen(Global.settings.fullscreen);
    }

    private void OnBackButtonPressed()
    {
        _uiSound.Play();
        var tween = CreateTween();
        tween.TweenProperty(_settingsButton, "global_position:x", 558, 0.3);
        tween.TweenInterval(0.1);
        tween.TweenProperty(_mainButtons, "global_position:y", 115, 0.2);
    }

    // public override void _Notification(int what)
    // {
    //     if (what == NotificationWMCloseRequest) Global.SaveData();
    // }

}
