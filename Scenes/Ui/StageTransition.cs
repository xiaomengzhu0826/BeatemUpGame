using Godot;
using System;

public partial class StageTransition : Control
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void StartTransition()
	{
		_animationPlayer.Play("start_transition");
	}

	public void EndTransition()
	{
		_animationPlayer.Play("end_transition");
	}

	public void OnCompeleteStartTransition()
	{
		_animationPlayer.Play("interim");
		SignalManager.EmitOnStageInterim();
	}

	public void OnCompeleteEndTransition()
	{
		_animationPlayer.Play("idle");
	}


}
