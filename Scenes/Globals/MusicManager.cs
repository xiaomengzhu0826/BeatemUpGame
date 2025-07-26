using Godot;
using System;
using System.Collections.Generic;

public partial class MusicManager : Node
{
    public static MusicManager Instance { get; private set; }

    private AudioStreamPlayer _audioStreamPlayer;

    private AudioStream _autoplayedMusic = null;

    public override void _Ready()
    {
        Instance = this;
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("MusicStreamPlayer");

        if (_autoplayedMusic != null)
        {
            _audioStreamPlayer.Stream = _autoplayedMusic;
            _audioStreamPlayer.Play();
        }
    }

    

    public enum MusicType
    {
        INTRO,
        MENU,
        STAGE1,
        STAGE2
    }

    public static readonly Dictionary<MusicType, AudioStream> MUSIC_PREFAB = new Dictionary<MusicType, AudioStream>()
    {
        { MusicType.INTRO, GD.Load<AudioStream>("res://Assets/music/intro.mp3") },
        { MusicType.MENU, GD.Load<AudioStream>("res://Assets/music/menu.mp3") },
        { MusicType.STAGE1, GD.Load<AudioStream>("res://Assets/music/stage-01.mp3") },
        { MusicType.STAGE2, GD.Load<AudioStream>("res://Assets/music/stage-02.mp3") },
    };

    public void Play(MusicType music)
    {
        if (_audioStreamPlayer.IsNodeReady())
        {
            _audioStreamPlayer.Stream = MUSIC_PREFAB[music];
            _audioStreamPlayer.Play();
        }
        else
        {
            _autoplayedMusic=MUSIC_PREFAB[music];
        }

    }


}
