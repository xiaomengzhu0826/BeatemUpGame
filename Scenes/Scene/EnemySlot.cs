using Godot;
using System;

public partial class EnemySlot : Node2D
{
	public BaseEnemy Occupant = null;

	public override void _Ready()
	{

	}

	public bool IsFree()
	{
		return Occupant == null;
	}

	public void FreeUp()
	{
		Occupant = null;
	}

	public void Occupy(BaseEnemy enemy)
	{
		Occupant = enemy;
	}




}
