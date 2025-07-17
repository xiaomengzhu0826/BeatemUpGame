using Godot;
using System;

public partial class SignalManager : Node
{
    public static SignalManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    [Signal] public delegate void OnSpawnCollectibleEventHandler(Collectible.Type type, Collectible.State initialState, Vector2 collectibleGlobalPosition, Vector2 collectibleDirection, float initialHeight, bool autoDestroy);

    public static void EmitOnSpawnCollectible(int type, int initialState, Vector2 collectibleGlobalPosition, Vector2 collectibleDirection, float initialHeight, bool autoDestroy)
    {
        Instance.EmitSignal(SignalName.OnSpawnCollectible, type, initialState, collectibleGlobalPosition, collectibleDirection, initialHeight, autoDestroy);
    }

    [Signal] public delegate void OnSpawnShotEventHandler(Vector2 gunRootPosition, float distanceTravel, float height);

    public static void EmitOnSpawnShot(Vector2 gunRootPosition, float distanceTravel, float height)
    {
        Instance.EmitSignal(SignalName.OnSpawnShot, gunRootPosition, distanceTravel, height);
    }

    [Signal] public delegate void OnSpawnEnemyEventHandler(EnemyData enemyData);

    public static void EmitOnSpawnEnemy(EnemyData enemyData)
    {
        Instance.EmitSignal(SignalName.OnSpawnEnemy, enemyData);
    }

    [Signal] public delegate void OnDeathEnemyEventHandler(Character enemy);

    public static void EmitOnDeathEnemy(Character enemy)
    {
        Instance.EmitSignal(SignalName.OnDeathEnemy, enemy);
    }

    [Signal] public delegate void OnCheckPointStartEventHandler();

    public static void EmitOnCheckPointStart()
    {
        Instance.EmitSignal(SignalName.OnCheckPointStart);
    }

    [Signal] public delegate void OnCheckPointCompeleteEventHandler();

    public static void EmitOnCheckPointCompelete()
    {
        Instance.EmitSignal(SignalName.OnCheckPointCompelete);
    }

    [Signal] public delegate void OnOrphanActorEventHandler(Node2D orphan);

    public static void EmitOnOrphanActor(Node2D orphan)
    {
        Instance.EmitSignal(SignalName.OnOrphanActor, orphan);
    }
    
    [Signal] public delegate void OnHealthChangeEventHandler(Character.Type characterType,int currentHealth,int maxHealth);
    
    public static void EmitOnHealthChange(int characterType,int currentHealth,int maxHealth)
    {
        Instance.EmitSignal(SignalName.OnHealthChange,characterType,currentHealth,maxHealth);
    }
}
