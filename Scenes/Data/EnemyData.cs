using Godot;
using System;


public partial class EnemyData : Resource
{

    [Export] public Character.Type _type;
    [Export] public Vector2 _globalPosition;
    [Export] public int _doorIndex;

    public EnemyData(Vector2 position, int assignedDoorIndex, Character.Type type = Character.Type.PUNK)
    {
        this._type = type;
        this._globalPosition = position;
        this._doorIndex = assignedDoorIndex;
    }
}
