using Godot;
using System;
using System.Collections.Generic;

public partial class CheckPoint : Node2D
{
	[Export] private int _nbSimultaneousEnemies;

	private Node2D _enemies;
	private Area2D _playerDetectionArea;

	private int _activeEnemyCounter = 0;
	private List<EnemyData> _enemyData = new List<EnemyData>();
	private bool _isActived = false;

	public override void _Ready()
	{
		_enemies = GetNode<Node2D>("Enemies");
		_playerDetectionArea = GetNode<Area2D>("PlayerDetectionArea");

		_playerDetectionArea.BodyEntered += OnPlayerEnter;
		SignalManager.Instance.OnDeathEnemy += OnEnemyDeath;
		//CreateEnemyData();
	}

	public override void _Process(double delta)
	{
		if (_isActived && CanSpawnEnemies())
		{
			var enemy = _enemyData.PopFront();
			SignalManager.EmitOnSpawnEnemy(enemy);
			_activeEnemyCounter += 1;
		}
	}

	public void CreateEnemyData()
	{
		foreach (BaseEnemy enemy in _enemies.GetChildren())
		{
			_enemyData.Add(new EnemyData(enemy.GlobalPosition, enemy._assignedDoorIndex,enemy._currentType));
			enemy.QueueFree();
		}
	}

	private bool CanSpawnEnemies()
	{
		return _enemyData.Count > 0 && _activeEnemyCounter < _nbSimultaneousEnemies;
	}

	private void OnPlayerEnter(Node2D body)
	{
		var player = (Player)body;
		if (!_isActived)
		{
			SignalManager.EmitOnCheckPointStart();
			_activeEnemyCounter =0;
			_isActived = true;
		}
	}

	private void OnEnemyDeath(Character enemy)
	{
		_activeEnemyCounter -= 1;
		if (_activeEnemyCounter == 0 && _enemyData.Count == 0)
		{
			SignalManager.EmitOnCheckPointCompelete();
			QueueFree();
		}
    }

}
