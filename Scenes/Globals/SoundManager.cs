using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    public static SoundManager Instance { get; private set; }

    private List<AudioStreamPlayer> _soundList = new List<AudioStreamPlayer>();

    public enum SoundType
    {
        CLICK,
        FOOD,
        GOGOGO,
        GRUNT,
        GUNSHOOT,
        HIT1,
        HIT2,
        KNIFE,
        SWOOSH
    }

    public override void _Ready()
    {
        Instance = this;

        for (int i = 0; i < GetChildren().Count; i++)
        {
            AudioStreamPlayer child = (AudioStreamPlayer)GetChild(i);
            _soundList.Add(child);
        }
    }

    public void Play(SoundType sfx, bool tweakPitch=false)
    {
        var addedPitch = 0f;
        if (tweakPitch)
        {
            addedPitch = (float)GD.RandRange(-0.3f, 0.3f);
        }
        _soundList[(int)sfx].PitchScale = 1 + addedPitch;
        _soundList[(int)sfx].Play();
    }

    
}
