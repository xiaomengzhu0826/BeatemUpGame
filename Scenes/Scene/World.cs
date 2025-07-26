using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node
{
    private CharacterBody2D _player;
    private Camera2D _mainCamera;
    private AnimatableBody2D _leftWall;
    private AnimatableBody2D _rightWall;
    private Node2D _actorsContainer;
    private bool _isCameraLocked = false;
    private List<Door> _doors = new List<Door>();


    public override void _Ready()
    {

        NodeInitiation();

        SignalManager.Instance.OnSpawnCollectible += OnSpawnCollectible;
        SignalManager.Instance.OnSpawnShot += OnSpawnShot;
        SignalManager.Instance.OnSpawnEnemy += OnSpawnEnemy;
        SignalManager.Instance.OnOrphanActor += OnOrphanActor;
        SignalManager.Instance.OnCheckPointStart += OnCheckPointStart;
        SignalManager.Instance.OnCheckPointCompelete += OnCheckPointCompelete;
        SignalManager.Instance.OnSpawnSpark += OnSpawnSpark;
    }

    private void OnCheckPointCompelete()
    {
        _isCameraLocked = false;
    }


    private void OnCheckPointStart()
    {
        _isCameraLocked = true;
    }


    private void NodeInitiation()
    {
        _player = GetNode<CharacterBody2D>("%Player");
        _mainCamera = GetNode<Camera2D>("MainCamera");
        _leftWall = GetNode<AnimatableBody2D>("%LeftWall");
        _rightWall = GetNode<AnimatableBody2D>("%RightWall");
        _actorsContainer = GetNode<Node2D>("ActorsContainer");
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
        BaseEnemy enemy = (BaseEnemy)PrefabManager.ENEMY_PREFAB[enemyData._type].Instantiate();
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
}
