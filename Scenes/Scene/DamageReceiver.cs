using Godot;
using System;

public partial class DamageReceiver : Area2D
{
	[Signal] public delegate void OnDamageReceivedEventHandler(int damage, Vector2 direction,HitType hitType);
	
	public enum HitType
    {
        NORMAL,
        KNOCKDOWN,
        POWER
    }
}
