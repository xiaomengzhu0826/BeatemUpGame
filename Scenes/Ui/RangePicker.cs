using Godot;
using System;
using System.Collections.Generic;

public partial class RangePicker : ActivableControl
{
	private HBoxContainer _ticksContainer;

	private List<TextureRect> _ticks = new();
	private Texture2D TickOn = GD.Load<Texture2D>("res://Assets/art/ui/ui-tick-on.png");
	private Texture2D TickOff = GD.Load<Texture2D>("res://Assets/art/ui/ui-tick-off.png");

	public override void _Ready()
	{
		base._Ready();
		_ticksContainer = GetNode<HBoxContainer>("TicksContainer");

		foreach (TextureRect child in _ticksContainer.GetChildren())
		{
			_ticks.Add(child);
		}
		Refresh();
	}

	public override void _Process(double delda)
	{
		if (_isActive && Input.IsActionJustPressed("ui_left"))
		{
			SetValue(_currentValue - 1);
		}
				if (_isActive && Input.IsActionJustPressed("ui_right"))
		{
			SetValue(_currentValue + 1);
		}
		Refresh();
	}

	protected override void Refresh()
	{

		for (int i = 0; i < _ticks.Count; i++)
		{
			if (i < _currentValue)
			{
				_ticks[i].Texture = TickOn;
			}
			else
			{
				_ticks[i].Texture = TickOff;
			}
		}
	}
}
