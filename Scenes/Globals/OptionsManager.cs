using Godot;
using System;

public partial class OptionsManager : Node
{
    public static OptionsManager Instance { get; private set; }

    public bool _isScreenShakeEnabled = true;
    public int _musicVolume = 3;
    public int _sfxVolume = 5;

    public override void _Ready()
    {
        Instance = this;
    }

    public void SetMusicVolume(int value)
    {
        _musicVolume = value;
        AudioServer.SetBusVolumeDb(1, ((float)value / 10).ToDb());
    }

    public void SetSoundVolume(int value)
    {
        _sfxVolume = value;
        AudioServer.SetBusVolumeDb(2, ((float)value / 10).ToDb());
    }

    public void SetScreenShake(bool value)
    {
        _isScreenShakeEnabled = value;
    }

}
