using Godot;
using TopDownGame.scripts.autoloads;

namespace TopDownGame.scripts.ui;

public partial class MainMenu : Control
{
    [ExportCategory("References")]
    [Export] private Texture2D _menuCursor;
    [Export] private Control _mainButtons;
    [Export] private Control _settingsButtons;
    [Export] private TextureButton _playButton;
    [Export] private TextureButton _settingsButton;
    [Export] private TextureButton _quitButton;
    [Export] private TextureButton _musicButton;
    [Export] private TextureButton _sfxButton;
    [Export] private TextureButton _windowButton;
    [Export] private TextureButton _backButton;
    [Export] private AudioStreamPlayer _uiSound;
    [Export] private AudioStreamPlayer _hoverSound;
    [Export] private Label _musicLabel;
    [Export] private Label _sfxLabel;
    [Export] private Label _windowLabel;

    public override void _Ready()
    {
        Cursor.Instance.Sprite2D.Texture = _menuCursor;
        
        UpdateAudioBus("Music", _musicLabel, Global.Instance.Settings["music"]);
        UpdateAudioBus("SFX", _sfxLabel, Global.Instance.Settings["sfx"]);
        UpdateFullscreen(Global.Instance.Settings["fullscreen"]);
        
        _playButton.Pressed += OnPlayButtonPressed;
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;
        _musicButton.Pressed += OnMusicButtonPressed;
        _sfxButton.Pressed += OnSfxButtonPressed;
        _windowButton.Pressed += OnWindowButtonPressed;
        _backButton.Pressed += OnBackButtonPressed;

        _playButton.MouseEntered += OnButtonMouseEntered;
        _settingsButton.MouseEntered += OnButtonMouseEntered;
        _quitButton.MouseEntered += OnButtonMouseEntered;
        _musicButton.MouseEntered += OnButtonMouseEntered;
        _sfxButton.MouseEntered += OnButtonMouseEntered;
        _windowButton.MouseEntered += OnButtonMouseEntered;
        _backButton.MouseEntered += OnButtonMouseEntered;
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
        Transition.Instance.TransitionTo("uid://cinbkpaw7sfcs");
    }

    private void OnSettingsButtonPressed()
    {
        _uiSound.Play();
        var tween = CreateTween();
        tween.TweenProperty(_mainButtons, "global_position:y", 350, 0.2);
        tween.TweenInterval(0.1);
        tween.TweenProperty(_settingsButtons, "global_position:x", 145, 0.3);
    }

    private void OnQuitButtonPressed()
    {
        _uiSound.Play();
        Global.Instance.SaveData();
        GetTree().Quit();
    }

    private void OnMusicButtonPressed()
    {
        _uiSound.Play();
        var currentMusic = Global.Instance.Settings["music"];
        Global.Instance.Settings["music"] = !currentMusic;
        UpdateAudioBus("Music", _musicLabel, Global.Instance.Settings["music"]);
    }

    private void OnSfxButtonPressed()
    {
        _uiSound.Play();
        var currentSfx = Global.Instance.Settings["sfx"];
        Global.Instance.Settings["sfx"] = !currentSfx;
        UpdateAudioBus("SFX", _sfxLabel, Global.Instance.Settings["sfx"]);
    }

     private void OnWindowButtonPressed()
    {
        _uiSound.Play();
        var currentFullscreen = Global.Instance.Settings["fullscreen"];
        Global.Instance.Settings["fullscreen"] = !currentFullscreen;
        UpdateFullscreen(Global.Instance.Settings["fullscreen"]);
    }

    private void OnBackButtonPressed()
    {
        _uiSound.Play();
        var tween = CreateTween();
        tween.TweenProperty(_settingsButtons, "global_position:x", 558, 0.3);
        tween.TweenInterval(0.1);
        tween.TweenProperty(_mainButtons, "global_position:y", 115, 0.2);
    }

    private void OnButtonMouseEntered()
    {
        _hoverSound.Play();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest) Global.Instance.SaveData();
    }
}
