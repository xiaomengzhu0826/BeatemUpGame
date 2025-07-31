using Godot;
using System;

public partial class TogglePicker : ActivableControl
{
	private Label _valueLabel;

	public override void _Ready()
	{
		base._Ready();
		_valueLabel = GetNode<Label>("ValueLabel");

		Refresh();
	}

	public override void _Process(double delda)
	{
		if (_isActive && HasInputToggle())
		{
			if (_currentValue == 1)
			{
				SetValue(0);
				_currentValue = 0;
			}
			else
			{
				SetValue(1);
				_currentValue = 1;
			}
		}
		Refresh();
	}

	private bool HasInputToggle()
	{
		string[] actions = new string[] { "ui_left", "ui_right", "attack", "jump" };
		foreach (var item in actions)
		{
			if (Input.IsActionJustPressed(item))
			{
				return true;
			}
		}
		return false;
	}

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if (_isActive && @event is InputEventAction actionEvent && actionEvent.Pressed)
	// 	{
	// 		if (actionEvent.Action == "ui_left" || actionEvent.Action == "ui_right")
	// 		{
	// 			if (_currentValue == 1)
	// 			{
	// 				SetValue(0);
	// 			}
	// 			else
	// 			{
	// 				SetValue(1);
	// 			}
	// 		}
	// 	}
	// }

	protected override void Refresh()
	{
		if (_currentValue == 1)
		{
			_valueLabel.Text = "ON";
		}
		else
		{
			_valueLabel.Text = "OFF";
		}
	}
}
