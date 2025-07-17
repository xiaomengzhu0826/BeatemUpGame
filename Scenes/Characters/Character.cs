using Godot;
using System;
using System.Collections.Generic;

public partial class Character : CharacterBody2D
{
	protected const float GRAVITY = 600.0f;

	// [ExportCategory("Category")]
	// [ExportGroup("group")]
	// [ExportSubgroup("subgroup")]
	[Export] protected bool _canRespawn;
	[Export] protected int _damage;
	[Export] protected int _maxHealth;
	[Export] public Type _currentType;

	[ExportGroup("Movement")]
	[Export] protected float _flightSpeed;
	[Export] protected float _jumpIntensity;
	[Export] protected float _knockbackIntensity;
	[Export] protected float _knockdownIntensity;
	[Export] protected float _speed;

	[ExportGroup("Weapons")]
	[Export] private bool _autoDestroyOnDrop;
	[Export] protected int _maxAmmoPerGun;
	[Export] protected bool _canRespawnKnife;
	[Export] protected int _damageGunshot;
	[Export] protected int _damagePower;
	[Export] protected float _durationOnGround;
	[Export] protected float _durationBetweenKnifeRespawn;
	[Export] protected bool _hasKnife;
	[Export] protected bool _hasGun;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _characterSprite;
	private Area2D _damageEmitter;
	private Area2D _collateralDamageEmitter;
	private DamageReceiver _damageReceiver;
	private CollisionShape2D _collisionShape2D;
	private Sprite2D _knifeSprite;
	private Sprite2D _gunSprite;
	protected RayCast2D _projectileAim;
	private Area2D _collectibleSensor;
	private Node2D _weaponPosition;

	protected int _currentHealth = 0;
	protected Vector2 _heading = Vector2.Right;
	public float _height = 0;
	protected float _heightSpeed = 0;
	public State _currentState;
	protected Dictionary<State, string> _animationMap;
	protected float _timeSinceGrounded = Time.GetTicksMsec();
	protected float _timeSinceKnifeDismiss = Time.GetTicksMsec();
	protected List<string> _animationAttacks = new List<string>();
	protected int _attackComboIndex = 0;
	protected bool _isLastHitSuccessful = false;
	protected int _ammoLeft = 0;


	public enum State
	{
		IDLE,
		WALK,
		ATTACK,
		TAKEOFF,
		JUMP,
		LAND,
		JUMPKICK,
		HURT,
		FALL,
		GROUNDED,
		DEATH,
		FLY,
		PREP_ATTACK,
		THROW,
		PICKUP,
		SHOOT,
		PREP_SHOOT,
		RECOVERY,
		DROP,
		WAIT,
		APPEARING
	}

	public enum Type
	{

		PUNK,
		GOON,
		THUG,
		BOUNCER,
		PLAYER
	}

	public override void _Ready()
	{
		_animationMap = new Dictionary<State, string>()
		{
			{State.IDLE,"idle"},
			{State.WALK,"walk"},
			{State.TAKEOFF,"takeoff"},
			{State.JUMP,"jump"},
			{State.LAND,"land"},
			{State.JUMPKICK,"jumpkick"},
			{State.HURT,"hurt"},
			{State.FALL,"fall"},
			{State.GROUNDED,"grounded"},
			{State.DEATH,"grounded"},
			{State.FLY,"fly"},
			{State.PREP_ATTACK,"idle"},
			{State.THROW,"throw"},
			{State.PICKUP,"pickup"},
			{State.SHOOT,"shoot"},
			{State.PREP_SHOOT,"idle"},
			{State.RECOVERY,"recovery"},
			{State.DROP,"idle"},
			{State.WAIT,"idle"},
			{State.APPEARING,"idle"},
		};

		SetHealth(_maxHealth);

		NodeInitiation();

		_currentState = State.IDLE;

		_damageEmitter.AreaEntered += OnDamageEmit;
		_damageReceiver.OnDamageReceived += OnDamageReceived;
		_collateralDamageEmitter.AreaEntered += OnCollateralDamage;
		_collateralDamageEmitter.BodyEntered += OnWallHit;

		SetSpriteHeightPosition();
	}



	private void NodeInitiation()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_characterSprite = GetNode<Sprite2D>("CharacterSprite");
		_damageEmitter = GetNode<Area2D>("DamageEmitter");
		_damageReceiver = GetNode<DamageReceiver>("DamageReceiver");
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_collateralDamageEmitter = GetNode<Area2D>("CollateralDamageEmitter");
		_knifeSprite = GetNode<Sprite2D>("KnifeSprite2D");
		_gunSprite = GetNode<Sprite2D>("GunSprite");
		_projectileAim = GetNode<RayCast2D>("ProjectileAim");
		_collectibleSensor = GetNode<Area2D>("CollectibleSensor");
		_weaponPosition = GetNode<Node2D>("%WeaponPosition");
	}

	public override void _Process(double delta)
	{
		HandleInput();
		HandleMovement();
		HandleAnimations();
		HandleAirTime(delta);
		HandlePrepAttack();
		HandlePrepShoot();
		HandleGrounded();
		HandleKnifeRespawns();
		HandleDeath(delta);
		SetHeading();
		FlipSprite();
		SetSpriteVisibility();
		SetSpriteHeightPosition();
		SetCollisions();
		MoveAndSlide();
	}

	private void SetSpriteVisibility()
	{
		_knifeSprite.Visible = _hasKnife;
		_gunSprite.Visible = _hasGun;
	}

	private void SetSpriteHeightPosition()
	{
		_characterSprite.Position = Vector2.Up * _height;
		_knifeSprite.Position = Vector2.Up * _height;
		_gunSprite.Position = Vector2.Up * _height;
	}

	private void SetCollisions()
	{
		_collisionShape2D.Disabled = IsCollisionDisabled();
		//_damageEmitter.SetDeferred("monitoring", IsAttacking());
		_damageEmitter.Monitoring = IsAttacking();
		//_damageReceiver.SetDeferred("monitorable", CanGetHurt());
		_damageReceiver.Monitorable = CanGetHurt();
		_collateralDamageEmitter.Monitoring = _currentState == State.FLY;
	}

	private void HandleMovement()
	{
		if (CanMove())
		{
			if (Velocity.Length() == 0)
			{
				_currentState = State.IDLE;
			}
			else
			{
				_currentState = State.WALK;
			}
		}
	}

	protected virtual void HandleInput()
	{

	}

	private void HandleAnimations()
	{
		if (_currentState == State.ATTACK)
		{
			_animationPlayer.Play(_animationAttacks[_attackComboIndex]);
		}
		else if (_animationPlayer.HasAnimation(_animationMap[_currentState]))
		{
			_animationPlayer.Play(_animationMap[_currentState]);
		}
	}

	protected virtual void HandleAirTime(double delta)
	{
		var airborneStates = new List<State> { State.JUMP, State.JUMPKICK, State.FALL, State.DROP };
		if (airborneStates.Contains(_currentState))
		{
			_height += _heightSpeed * (float)delta;
			if (_height < 0)
			{
				_height = 0;
				if (_currentState == State.FALL)
				{
					_currentState = State.GROUNDED;
					_timeSinceGrounded = Time.GetTicksMsec();
				}
				else
				{
					_currentState = State.LAND;
				}
				Velocity = Vector2.Zero;
			}
			else
			{
				_heightSpeed -= GRAVITY * (float)delta;
			}
		}
	}

	protected virtual void HandlePrepAttack()
	{

	}

	protected virtual void HandlePrepShoot()
	{

	}

	protected virtual void HandleGrounded()
	{
		if (_currentState == State.GROUNDED && (Time.GetTicksMsec() - _timeSinceGrounded) > _durationOnGround)
		{
			if (_currentHealth == 0)
			{
				_currentState = State.DEATH;
			}
			else
			{
				_currentState = State.LAND;
			}
		}
	}

	private void HandleKnifeRespawns()
	{
		if (_canRespawnKnife && !_hasKnife && (Time.GetTicksMsec() - _timeSinceKnifeDismiss) > _durationBetweenKnifeRespawn)
		{
			_hasKnife = true;
		}
	}

	protected virtual void HandleDeath(double delta)
	{
		if (_currentState == State.DEATH && !_canRespawn)
		{
			Modulate -= new Color(Modulate.R, Modulate.G, Modulate.B, (float)(delta / 2.0));
			if (Modulate.A <= 0)
			{
				QueueFree();
			}
		}
	}

	protected virtual void SetHeading()
	{

	}

	private void FlipSprite()
	{
		if (_heading == Vector2.Right)
		{
			_characterSprite.FlipH = false;
			_gunSprite.Scale = new Vector2(1, 1);
			_knifeSprite.Scale = new Vector2(1, 1);
			_projectileAim.Scale = new Vector2(1, 1);
			_damageEmitter.Scale = new Vector2(1, 1);
		}
		else
		{
			_characterSprite.FlipH = true;
			_gunSprite.Scale = new Vector2(-1, 1);
			_knifeSprite.Scale = new Vector2(-1, 1);
			_projectileAim.Scale = new Vector2(-1, 1);
			_damageEmitter.Scale = new Vector2(-1, 1);
		}
	}
	#region  Bool Check
	protected virtual bool CanAttack()
	{
		return _currentState == State.IDLE || _currentState == State.WALK;
	}

	protected bool CanMove()
	{
		return _currentState == State.WALK || _currentState == State.IDLE;
	}

	protected bool CanJump()
	{
		return _currentState == State.IDLE || _currentState == State.WALK;
	}

	protected bool CanJumpKick()
	{
		return _currentState == State.JUMP;
	}

	protected virtual bool CanGetHurt()
	{
		var state = new List<State> { State.IDLE, State.WALK, State.TAKEOFF, State.LAND, State.PREP_ATTACK };
		return state.Contains(_currentState);
	}

	protected bool CanPickupCollectible()
	{
		if (_canRespawnKnife)
		{
			return false;
		}
		if (Time.GetTicksMsec() - _timeSinceKnifeDismiss < _durationBetweenKnifeRespawn)
		{
			return false;
		}
		var collectibleAreas = _collectibleSensor.GetOverlappingAreas();
		if (collectibleAreas.Count == 0)
		{
			return false;
		}
		Collectible collectible = (Collectible)collectibleAreas[0];
		if (collectible._currentType == Collectible.Type.KNIFE && !IsCarryingWeapon())
		{
			return true;
		}
		if (collectible._currentType == Collectible.Type.GUN && !IsCarryingWeapon())
		{
			return true;
		}
		if (collectible._currentType == Collectible.Type.FOOD)
		{
			return true;
		}
		return false;
	}

	protected virtual bool IsAttacking()
	{
		var state = new List<State> { State.ATTACK, State.JUMPKICK };
		return state.Contains(_currentState);
	}

	protected bool IsCarryingWeapon()
	{
		return _hasKnife || _hasGun;
	}

	protected void PickupCollectible()
	{

		if (CanPickupCollectible())
		{
			var collectibleAreas = _collectibleSensor.GetOverlappingAreas();
			Collectible collectible = (Collectible)collectibleAreas[0];
			if (collectible._currentType == Collectible.Type.KNIFE && !_hasKnife)
			{
				_hasKnife = true;
			}
			if (collectible._currentType == Collectible.Type.GUN && !_hasGun)
			{
				_hasGun = true;
				_ammoLeft = _maxAmmoPerGun;
			}
			if (collectible._currentType == Collectible.Type.FOOD)
			{
				//GD.Print(_currentHealth);
				SetHealth(_maxHealth);
				//GD.Print(_currentHealth);
			}
			collectible.QueueFree();
		}
	}

	protected void ShootGun()
	{
		_currentState = State.SHOOT;
		Velocity = Vector2.Zero;
		var targetPoint = _heading * (GlobalPosition.X + GetViewportRect().Size.X);
		var target = _projectileAim.GetCollider();
		if (target is Character character)
		{
			if (character != null)
			{
				targetPoint = _projectileAim.GetCollisionPoint();
				character._damageReceiver.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, _damageGunshot, _heading, (int)DamageReceiver.HitType.KNOCKDOWN);
			}
		}
		if (target is Barrel barrel)
		{
			if (barrel != null)
			{
				targetPoint = _projectileAim.GetCollisionPoint();
				barrel._damageReceiver.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, _damageGunshot, _heading, (int)DamageReceiver.HitType.KNOCKDOWN);
			}
		}

		var weaponRootPosition = new Vector2(_weaponPosition.GlobalPosition.X, Position.Y);
		var weaponHeight = -_weaponPosition.Position.Y;
		var distance = targetPoint.X - _weaponPosition.GlobalPosition.X;
		SignalManager.EmitOnSpawnShot(weaponRootPosition, distance, weaponHeight);
	}

	private bool IsCollisionDisabled()
	{
		var state = new List<State> { State.GROUNDED, State.DEATH, State.FLY };
		return state.Contains(_currentState);
	}
	#endregion

	#region AnimationPlay Callback
	public virtual void OnActionCompelete()
	{
		_currentState = State.IDLE;
	}

	public void OnThrowCompelete()
	{
		_currentState = State.IDLE;
		var collectibleType = Collectible.Type.KNIFE;
		if (_hasGun)
		{
			collectibleType = Collectible.Type.GUN;
			_hasGun = false;
		}
		else
		{
			_hasKnife = false;
		}

		var collectibleGlobalPosition = new Vector2(_weaponPosition.GlobalPosition.X, GlobalPosition.Y);
		var collectibleHeight = -_weaponPosition.Position.Y;
		SignalManager.EmitOnSpawnCollectible
		(
			(int)collectibleType,
			(int)Collectible.State.FLY,
			collectibleGlobalPosition,
			_heading,
			collectibleHeight,
			false
		);
	}

	protected void OnPickupCompelete()
	{
		_currentState = State.IDLE;
		PickupCollectible();
	}

	public void OnTakeoffCompelete()
	{
		_currentState = State.JUMP;
		_heightSpeed = _jumpIntensity;
	}

	public void OnLandCompelete()
	{
		_currentState = State.IDLE;
	}
	#endregion

	protected virtual void OnDamageEmit(Area2D area)
	{
		var hitType = DamageReceiver.HitType.NORMAL;
		var damageReciver = (DamageReceiver)area;
		Vector2 direction = damageReciver.GlobalPosition.X > GlobalPosition.X ? Vector2.Right : Vector2.Left;
		var currentDamage = _damage;
		if (_currentState == State.JUMPKICK)
		{
			hitType = DamageReceiver.HitType.KNOCKDOWN;
		}
		if (_attackComboIndex == _animationAttacks.Count - 1)
		{
			hitType = DamageReceiver.HitType.POWER;
			currentDamage = _damagePower;
		}
		//damageReciver.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, _damage, direction, Variant.From(hitType));
		damageReciver.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, currentDamage, direction, (int)hitType);
		_isLastHitSuccessful = true;
	}

	protected virtual void OnDamageReceived(int damage, Vector2 direction, DamageReceiver.HitType hitType)
	{
		if (CanGetHurt())
		{
			_attackComboIndex = 0;
			_canRespawnKnife = false;
			if (_hasKnife)
			{
				_hasKnife = false;
				SignalManager.EmitOnSpawnCollectible((int)Collectible.Type.KNIFE, (int)Collectible.State.FALL, GlobalPosition, Vector2.Zero, 0.0f, _autoDestroyOnDrop);
				_timeSinceKnifeDismiss = Time.GetTicksMsec();
			}
			if (_hasGun)
			{
				_hasGun = false;
				SignalManager.EmitOnSpawnCollectible((int)Collectible.Type.GUN, (int)Collectible.State.FALL, GlobalPosition, Vector2.Zero, 0.0f, _autoDestroyOnDrop);
			}
			SetHealth(_currentHealth - damage);
			if (_currentHealth == 0 || hitType == DamageReceiver.HitType.KNOCKDOWN)
			{
				_currentState = State.FALL;
				_heightSpeed = _knockdownIntensity;
				Velocity = direction * _knockbackIntensity;
			}
			else if (hitType == DamageReceiver.HitType.POWER)
			{
				_currentState = State.FLY;
				Velocity = direction * _flightSpeed;
			}
			else
			{
				_currentState = State.HURT;
				Velocity = direction * _knockbackIntensity;
			}
		}
	}

	private void OnWallHit(Node2D body)
	{
		_currentState = State.FALL;
		_heightSpeed = _knockdownIntensity;
		Velocity = -Velocity / 2;
	}


	private void OnCollateralDamage(Area2D area)
	{
		var collateralDamage = area as DamageReceiver;
		if (collateralDamage != _damageReceiver)
		{
			Vector2 direction = collateralDamage.GlobalPosition.X > GlobalPosition.X ? Vector2.Right : Vector2.Left;
			collateralDamage.EmitSignal(DamageReceiver.SignalName.OnDamageReceived, 0, direction, (int)DamageReceiver.HitType.KNOCKDOWN);
		}
	}

	private void SetHealth(int health)
	{
		_currentHealth = Mathf.Clamp(health, 0, _maxHealth);
		SignalManager.EmitOnHealthChange((int)_currentType, _currentHealth,_maxHealth);
	}

}
