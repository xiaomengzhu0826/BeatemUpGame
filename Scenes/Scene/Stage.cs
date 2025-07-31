using Godot;
using System;

public partial class Stage : Node2D
{
    [Export] private MusicManager.MusicType _music;

    private Node2D _containers;
    private Node2D _doors;
    private Node2D _checkPoints;
    private Node2D _playerSpawnLocation;

    public override void _Ready()
    {

        _containers = GetNode<Node2D>("Container");
        _doors = GetNode<Node2D>("Doors");
        _checkPoints = GetNode<Node2D>("CheckPoints");
        _playerSpawnLocation = GetNode<Node2D>("PlayerSpawnLocation");

        SignalManager.Instance.OnCheckPointCompelete += OnCheckPointCompelete;

        foreach (Node2D container in _containers.GetChildren())
        {
            CallDeferred(nameof(DeferredEmit), container);
            //SignalManager.EmitOnOrphanActor(container);
        }

        for (int i = 0; i < _doors.GetChildCount(); i++)
        {
            Door door = (Door)_doors.GetChild(i);
            foreach (BaseEnemy enemy in door._enemies)
            {
                enemy._assignedDoorIndex = i;
                //GD.Print(enemy._assignedDoorIndex);
            }
        }

        foreach (Node2D door in _doors.GetChildren())
        {
            CallDeferred(nameof(DeferredEmit), door);
            //SignalManager.EmitOnOrphanActor(door);
        }

        foreach (CheckPoint checkPoint in _checkPoints.GetChildren())
        {
            checkPoint.CreateEnemyData();
        }

        MusicManager.Instance.Play(_music);
    }

    private void OnCheckPointCompelete(CheckPoint checkPoint)
    {
        if (_checkPoints.GetChild(-1) == checkPoint)
        {
            SignalManager.EmitOnStageCompelete();
        }
    }


    private void DeferredEmit(Node2D node2D)
    {
        SignalManager.EmitOnOrphanActor(node2D);
    }

    public Vector2 GetPlayerSpawnLocation()
    {
        return _playerSpawnLocation.Position;
    }
}
