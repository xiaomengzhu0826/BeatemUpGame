using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node
{
    private Player _player;
    private Camera2D _mainCamera;
    private AnimatableBody2D _leftWall;
    private AnimatableBody2D _rightWall;
    private Node2D _actorsContainer;
    private Node2D _stageContainer;
    private StageTransition _stageTransition;

    private bool _isCameraLocked = false;
    private List<Door> _doors = new List<Door>();
    private List<PackedScene> _stagePrefabs = new() { GD.Load<PackedScene>("res://Scenes/Scene/Stage01.tscn"), GD.Load<PackedScene>("res://Scenes/Scene/Stage02.tscn") };
    private int _currentStageIndex = -1;
    private PackedScene _playerPrefab = GD.Load<PackedScene>("res://Scenes/Characters/Player.tscn");
    private Vector2 _cameraInitialPosition = Vector2.Zero;

    public override void _Ready()
    {

        NodeInitiation();
        _cameraInitialPosition = _mainCamera.Position;
        SignalManager.Instance.OnSpawnCollectible += OnSpawnCollectible;
        SignalManager.Instance.OnSpawnShot += OnSpawnShot;
        SignalManager.Instance.OnSpawnEnemy += OnSpawnEnemy;
        SignalManager.Instance.OnOrphanActor += OnOrphanActor;
        SignalManager.Instance.OnCheckPointStart += OnCheckPointStart;
        SignalManager.Instance.OnCheckPointCompelete += OnCheckPointCompelete;
        SignalManager.Instance.OnSpawnSpark += OnSpawnSpark;
        SignalManager.Instance.OnPlayerRevive += OnPlayerRevive;
        SignalManager.Instance.OnStageInterim += DefferdLoadNextStage;

        LoadNextStage();
    }

    private void DefferdLoadNextStage()
    {
        CallDeferred(MethodName.LoadNextStage);
    }


    private void OnCheckPointCompelete(CheckPoint checkPoint)
    {
        _isCameraLocked = false;
    }


    private void OnCheckPointStart()
    {
        _isCameraLocked = true;
    }


    private void NodeInitiation()
    {
        //_player = GetNode<Player>("%Player");
        _mainCamera = GetNode<Camera2D>("MainCamera");
        _leftWall = GetNode<AnimatableBody2D>("%LeftWall");
        _rightWall = GetNode<AnimatableBody2D>("%RightWall");
        _actorsContainer = GetNode<Node2D>("ActorsContainer");
        _stageContainer = GetNode<Node2D>("StageContainer");
        _stageTransition=GetNode<StageTransition>("Ui/UiContainer/StageTransition");
    }

    public override void _Process(double delta)
    {
        if (!_isCameraLocked && _player.GlobalPosition.X > _mainCamera.GlobalPosition.X)
        {
            _mainCamera.GlobalPosition = new Vector2(_player.GlobalPosition.X, _mainCamera.GlobalPosition.Y);
            // _rightWall.GlobalPosition = new Vector2(_player.GlobalPosition.X + 50, _mainCamera.GlobalPosition.Y);
            // _leftWall.GlobalPosition = new Vector2(_player.GlobalPosition.X-50, _mainCamera.GlobalPosition.Y);
        }
    }
    


    private void LoadNextStage()
    {
        _currentStageIndex += 1;
        if (_currentStageIndex < _stagePrefabs.Count)
        {
            foreach (var item in _actorsContainer.GetChildren())
            {
                item.QueueFree();
            }
            Stage stage = (Stage)_stagePrefabs[_currentStageIndex].Instantiate();
            foreach (var existingStage in _stageContainer.GetChildren())
            {
                existingStage.QueueFree();
            }
            _stageContainer.AddChild(stage);
            _player = (Player)_playerPrefab.Instantiate();
            _actorsContainer.AddChild(_player);
            _player.Position = stage.GetPlayerSpawnLocation();
            _mainCamera.Position = _cameraInitialPosition;
            _mainCamera.ResetSmoothing();
            _stageTransition.EndTransition();
        }
    }

    private void OnSpawnCollectible(Collectible.Type type, Collectible.State initialState, Vector2 collectibleGlobalPosition, Vector2 collectibleDirection, float initialHeight, bool autoDestroy)
    {
        var collectiblePrefab = PrefabManager.CollectibleKeys.KNIFE;
        if (type == Collectible.Type.GUN)
        {
            collectiblePrefab = PrefabManager.CollectibleKeys.GUN;
        }
        if (type == Collectible.Type.FOOD)
        {
            collectiblePrefab = PrefabManager.CollectibleKeys.FOOD;
        }
        var collectible = (Collectible)PrefabManager.COLLECTIBLE_PREFAB[collectiblePrefab].Instantiate();
        collectible._currentType = type;
        collectible._currentState = initialState;
        collectible.GlobalPosition = collectibleGlobalPosition;
        collectible._direction = collectibleDirection;
        collectible._height = initialHeight;
        collectible._autoDestroy = autoDestroy;
        _actorsContainer.CallDeferred("add_child", collectible);
        //_actorsContainer.AddChild(collectible);
    }

    private void OnSpawnShot(Vector2 gunRootPosition, float distanceTravel, float height)
    {
        var shot = (Shot)PrefabManager.COLLECTIBLE_PREFAB[PrefabManager.CollectibleKeys.SHOT].Instantiate();
        _actorsContainer.AddChild(shot);
        //_actorsContainer.CallDeferred("add_child", shot);
        shot.Position = gunRootPosition;
        shot.Initialize(distanceTravel, height);
    }

    private void OnSpawnEnemy(EnemyData enemyData)
    {
        Character character = (Character)PrefabManager.ENEMY_PREFAB[enemyData._type].Instantiate();
        if (character is BaseEnemy enemy)
        {
            enemy.GlobalPosition = enemyData._globalPosition;
            enemy.Player = (Player)_player;
            _actorsContainer.AddChild(enemy);
            if (enemy.GlobalPosition.Y < 0)
            {
                enemy._height = 50;
                enemy.GlobalPosition = enemyData._globalPosition + Vector2.Down * enemy._height;
                enemy._currentState = Character.State.DROP;
            }
            else
            {
                enemy.GlobalPosition = enemyData._globalPosition;
                enemy._currentState = Character.State.IDLE;
            }
            if (enemyData._doorIndex > -1)
            {
                enemy.AssignDoor(_doors[enemyData._doorIndex]);
            }
        }
        if (character is IgorBoss boss)
        {
            boss.GlobalPosition = enemyData._globalPosition;
            boss._player = (Player)_player;
            _actorsContainer.AddChild(boss);
            if (boss.GlobalPosition.Y < 0)
            {
                boss._height = 50;
                boss.GlobalPosition = enemyData._globalPosition + Vector2.Down * boss._height;
                boss._currentState = Character.State.DROP;
            }
            else
            {
                boss.GlobalPosition = enemyData._globalPosition;
                boss._currentState = Character.State.IDLE;
            }
        }


    }

    private void OnOrphanActor(Node2D orphan)
    {
        if (orphan is Door door)
        {
            _doors.Add(door);
        }
        orphan.Reparent(_actorsContainer);
    }

    private void OnSpawnSpark(Vector2 sparkPosition)
    {
        var sparkInstance = PrefabManager.EFFECT_PREFAB[PrefabManager.EffectType.SPARK].Instantiate() as Node2D;
        sparkInstance.Position = sparkPosition;
        _actorsContainer.AddChild(sparkInstance);
    }

    private void OnPlayerRevive()
    {
        foreach (var child in _actorsContainer.GetChildren())
        {
            if (child is Character character)
            {
                if (character._currentType != Character.Type.PLAYER)
                {
                    character.OnDamageReceived(0, Vector2.Zero, DamageReceiver.HitType.KNOCKDOWN);
                }
            }
        }
    }
}
