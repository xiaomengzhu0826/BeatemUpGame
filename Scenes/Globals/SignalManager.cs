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

    [Signal] public delegate void OnCheckPointCompeleteEventHandler(CheckPoint checkPoint);

    public static void EmitOnCheckPointCompelete(CheckPoint checkPoint)
    {
        Instance.EmitSignal(SignalName.OnCheckPointCompelete, checkPoint);
    }

    [Signal] public delegate void OnOrphanActorEventHandler(Node2D orphan);

    public static void EmitOnOrphanActor(Node2D orphan)
    {
        Instance.EmitSignal(SignalName.OnOrphanActor, orphan);
    }

    [Signal] public delegate void OnHealthChangeEventHandler(Character.Type characterType, int currentHealth, int maxHealth);

    public static void EmitOnHealthChange(int characterType, int currentHealth, int maxHealth)
    {
        Instance.EmitSignal(SignalName.OnHealthChange, characterType, currentHealth, maxHealth);
    }

    [Signal] public delegate void OnRegisterHitEventHandler();

    public static void EmitOnRegisterHit()
    {
        Instance.EmitSignal(SignalName.OnRegisterHit);
    }

    [Signal] public delegate void OnSpawnSparkEventHandler(Vector2 sparkPosition);

    public static void EmitOnSpawnSpark(Vector2 sparkPosition)
    {
        Instance.EmitSignal(SignalName.OnSpawnSpark, sparkPosition);
    }

    [Signal] public delegate void OnHeavyBlowReceivedEventHandler();

    public static void EmitOnHeavyBlowReceived()
    {
        Instance.EmitSignal(SignalName.OnHeavyBlowReceived);
    }

    [Signal] public delegate void OnPlayerReviveEventHandler();

    public static void EmitOnPlayerRevive()
    {
        Instance.EmitSignal(SignalName.OnPlayerRevive);
    }

    [Signal] public delegate void OnStageCompeleteEventHandler();

    public static void EmitOnStageCompelete()
    {
        Instance.EmitSignal(SignalName.OnStageCompelete);
    }
    
    [Signal] public delegate void OnStageInterimEventHandler();
    
    public static void EmitOnStageInterim()
    {
        Instance.EmitSignal(SignalName.OnStageInterim);
    }
    

}
