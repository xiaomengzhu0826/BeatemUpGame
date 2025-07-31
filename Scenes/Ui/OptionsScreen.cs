using Godot;
using System;
using System.Collections.Generic;

public partial class OptionsScreen : Control
{
	[Signal] public delegate void OnExitEventHandler();

	private RangePicker _musicControl;
	private RangePicker _soundControl;
	private TogglePicker _shakeControl;
	private LabelPicker _returnControl;

	private int _currentSelectionIndex = 0;
	private List<ActivableControl> _activables;

	public override void _Ready()
	{
		_musicControl = GetNode<RangePicker>("%MusicVolumeControl");
		_soundControl = GetNode<RangePicker>("%SoundVolumeControl");
		_shakeControl = GetNode<TogglePicker>("%ShakeToggle");
		_returnControl = GetNode<LabelPicker>("%ReturnButton");
		_activables = new List<ActivableControl>() { _musicControl, _soundControl, _shakeControl, _returnControl };
		_musicControl.SetValue(OptionsManager.Instance._musicVolume);
		_soundControl.SetValue(OptionsManager.Instance._sfxVolume);
		_shakeControl.SetValue(OptionsManager.Instance._isScreenShakeEnabled ? 1 : 0);

		_musicControl.OnValueChanged += OnMusicValueChanged;
		_soundControl.OnValueChanged += OnSFXValueChanged;
		_shakeControl.OnValueChanged += OnShakeValueChanged;
		_returnControl.OnPress += OnReturnPressed;
		Refresh();
	}



	public override void _ExitTree()
	{
		_musicControl.OnValueChanged -= OnMusicValueChanged;
		_soundControl.OnValueChanged -= OnSFXValueChanged;
		_shakeControl.OnValueChanged -= OnShakeValueChanged;
	}

	public override void _Process(double delda)
	{
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("ui_down"))
		{
			_currentSelectionIndex = (int)Mathf.Clamp(_currentSelectionIndex + 1, 0, _activables.Count);
			Refresh();
			SoundManager.Instance.Play(SoundManager.SoundType.CLICK);
		}
		if (Input.IsActionJustPressed("ui_up"))
		{
			_currentSelectionIndex = (int)Mathf.Clamp(_currentSelectionIndex - 1, 0, _activables.Count);
			Refresh();
			SoundManager.Instance.Play(SoundManager.SoundType.CLICK);
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < _activables.Count; i++)
		{
			_activables[i].SetActive(_currentSelectionIndex == i);
		}
	}

	private void OnMusicValueChanged(int value)
	{
		OptionsManager.Instance.SetMusicVolume(value);
		SoundManager.Instance.Play(SoundManager.SoundType.HIT1);
	}

	private void OnSFXValueChanged(int value)
	{
		OptionsManager.Instance.SetSoundVolume(value);
		SoundManager.Instance.Play(SoundManager.SoundType.HIT1);
	}

	private void OnShakeValueChanged(int value)
	{
		OptionsManager.Instance.SetScreenShake(value == 1);
		SoundManager.Instance.Play(SoundManager.SoundType.HIT1);
	}

	private void OnReturnPressed()
	{
		EmitSignal(SignalName.OnExit);
		SoundManager.Instance.Play(SoundManager.SoundType.HIT1);
    }
}
