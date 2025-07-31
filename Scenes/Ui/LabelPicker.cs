using Godot;
using System;

public partial class LabelPicker : ActivableControl
{
	[Signal] public delegate void OnPressEventHandler();

	public override void _Process(double delta)
	{
		if (_isActive && (Input.IsActionJustPressed("attack") || Input.IsActionJustPressed("jump")))
		{
			EmitSignal(SignalName.OnPress);
		}
	}
}
