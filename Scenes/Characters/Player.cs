using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Player : Character
{
	private const int REVIVE_HEIGHT = 80;

	[Export] private int _maxDurationBetweenSuccessfulHits;

	private List<EnemySlot> _enemySlots = new();
	private Node2D _enemySlotsParent;
	private float _timeSinceLastSuccessfulAttack = Time.GetTicksMsec();

	public override void _Ready()
	{
		base._Ready();
		_animationAttacks.AddRange(new string[] { "punch", "punch_alt", "kick", "roundkick" });
		_enemySlotsParent = GetNode<Node2D>("EnemySlots");
		_enemySlots = _enemySlotsParent.GetChildren().Cast<EnemySlot>().ToList();

		SignalManager.Instance.OnPlayerRevive += OnPlayerRevive;
	}



	public override void _Process(double delta)
	{
		base._Process(delta);
		ProcessTimeBetweenCombos();
	}

	private void ProcessTimeBetweenCombos()
	{
		if (Time.GetTicksMsec() - _timeSinceLastSuccessfulAttack > _maxDurationBetweenSuccessfulHits)
		{
			_attackComboIndex = 0;
		}
	}

	protected override void HandleInput()
	{
		if (CanMove())
		{
			var direction = Input.GetVector("left", "right", "up", "down");
			Velocity = direction * _speed;
		}

		if (CanAttack() && Input.IsActionPressed("attack"))
		{
			Velocity = Vector2.Zero;
			if (_hasKnife)
			{
				_currentState = State.THROW;
			}
			else if (_hasGun)
			{
				if (_ammoLeft > 0)
				{
					ShootGun();
					_ammoLeft -= 1;
				}
				else
				{
					_currentState = State.THROW;
				}
			}
			else
			{
				if (CanPickupCollectible())
				{
					_currentState = State.PICKUP;
				}
				else
				{
					_currentState = State.ATTACK;
					SoundManager.Instance.Play(SoundManager.SoundType.SWOOSH);
					if (_isLastHitSuccessful)
					{
						_timeSinceLastSuccessfulAttack = Time.GetTicksMsec();
						_attackComboIndex = (_attackComboIndex + 1) % _animationAttacks.Count;
						_isLastHitSuccessful = false;
					}
					else
					{
						_attackComboIndex = 0;
					}
				}

			}

		}
		if (CanJump() && Input.IsActionPressed("jump"))
		{
			_currentState = State.TAKEOFF;
		}
		if (CanJumpKick() && Input.IsActionPressed("attack"))
		{
			_currentState = State.JUMPKICK;
			SoundManager.Instance.Play(SoundManager.SoundType.SWOOSH);
		}
	}

	protected override void SetHeading()
	{
		if (CanMove())
		{
			if (Velocity.X > 0)
			{
				_heading = Vector2.Right;
			}
			else if (Velocity.X < 0)
			{
				_heading = Vector2.Left;
			}
		}

	}

	protected override void HandleDeath(double delta)
	{
		if (_currentState == State.DEATH)
		{

		}
	}

	public EnemySlot ReserveSlot(BaseEnemy enemy)
	{
		float shortestDistance = float.MaxValue;
		EnemySlot occupiedSlot = null;
		foreach (EnemySlot slot in _enemySlots)
		{
			if (slot.IsFree() && shortestDistance > (enemy.GlobalPosition - slot.GlobalPosition).Length())
			{
				shortestDistance = (enemy.GlobalPosition - slot.GlobalPosition).Length();
				occupiedSlot = slot;
			}
		}
		if (occupiedSlot != null)
		{
			occupiedSlot.Occupy(enemy);
		}
		return occupiedSlot;
	}

	public void FreeSlot(BaseEnemy enemy)
	{
		foreach (EnemySlot slot in _enemySlots)
		{
			if (slot.Occupant == enemy)
			{
				slot.FreeUp();
			}
		}
	}

	private void GamePaused()
	{

		// if (_currentState == State.DEATH)
		// {
		// 	GD.Print("player die");
		// 	GetTree().Paused = true;
		// }

	}

	private void OnPlayerRevive()
	{
		_currentHealth = _maxHealth;
		_currentState = State.JUMP;
		_height = REVIVE_HEIGHT;
		SignalManager.EmitOnHealthChange((int)Character.Type.PLAYER, _currentHealth, _maxHealth);
    }
}
