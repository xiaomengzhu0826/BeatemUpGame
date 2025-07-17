using Godot;
using System;
using System.Collections.Generic;

public partial class PrefabManager : Node
{
    public static PrefabManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public enum CollectibleKeys
    {
        KNIFE,
        GUN,
        SHOT,
        FOOD
    }

    public enum EnemyKeys
    {
        PUNK,
        GOON,
        THUG,
        BOUNCER
    }

    public static readonly Dictionary<EnemyKeys, PackedScene> ENEMY_PREFAB = new Dictionary<EnemyKeys, PackedScene>()
    {
        { EnemyKeys.PUNK, GD.Load<PackedScene>("res://Scenes/Characters/BaseEnemy.tscn") },
        { EnemyKeys.GOON, GD.Load<PackedScene>("res://Scenes/Characters/GoonEnemy.tscn") },
        { EnemyKeys.THUG, GD.Load<PackedScene>("res://Scenes/Characters/ThugEnemy.tscn") },
        { EnemyKeys.BOUNCER, GD.Load<PackedScene>("res://Scenes/Characters/IgorBoss.tscn") },
    };



    public static readonly Dictionary<CollectibleKeys, PackedScene> COLLECTIBLE_PREFAB = new Dictionary<CollectibleKeys, PackedScene>()
    {
        { CollectibleKeys.KNIFE, GD.Load<PackedScene>("res://Scenes/Props/Knife.tscn") },
        { CollectibleKeys.SHOT, GD.Load<PackedScene>("res://Scenes/Props/Shot.tscn") },
        { CollectibleKeys.GUN, GD.Load<PackedScene>("res://Scenes/Props/Gun.tscn") },
        { CollectibleKeys.FOOD, GD.Load<PackedScene>("res://Scenes/Props/Food.tscn") },
    };
}
