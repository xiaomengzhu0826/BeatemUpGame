using Godot;
using System;

public partial class Stage : Node2D
{
    private Node2D _containers;
    private Node2D _doors;
    private Node2D _checkPoints;

    public override void _Ready()
    {

        _containers = GetNode<Node2D>("Container");
        _doors = GetNode<Node2D>("Doors");
        _checkPoints = GetNode<Node2D>("CheckPoints");

        foreach (Node2D container in _containers.GetChildren())
        {
            SignalManager.EmitOnOrphanActor(container);
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
    }

    private void DeferredEmit(Door door)
    {
        SignalManager.EmitOnOrphanActor(door);
    }
}
