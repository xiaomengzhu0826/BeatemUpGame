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

    public enum EffectType
    {
        SPARK,
    }

    public static readonly Dictionary<Character.Type, PackedScene> ENEMY_PREFAB = new Dictionary<Character.Type, PackedScene>()
    {
        { Character.Type.PUNK, GD.Load<PackedScene>("res://Scenes/Characters/BaseEnemy.tscn") },
        { Character.Type.GOON, GD.Load<PackedScene>("res://Scenes/Characters/GoonEnemy.tscn") },
        { Character.Type.THUG, GD.Load<PackedScene>("res://Scenes/Characters/ThugEnemy.tscn") },
        { Character.Type.BOUNCER, GD.Load<PackedScene>("res://Scenes/Characters/IgorBoss.tscn") },
    };

    public static readonly Dictionary<CollectibleKeys, PackedScene> COLLECTIBLE_PREFAB = new Dictionary<CollectibleKeys, PackedScene>()
    {
        { CollectibleKeys.KNIFE, GD.Load<PackedScene>("res://Scenes/Props/Knife.tscn") },
        { CollectibleKeys.SHOT, GD.Load<PackedScene>("res://Scenes/Props/Shot.tscn") },
        { CollectibleKeys.GUN, GD.Load<PackedScene>("res://Scenes/Props/Gun.tscn") },
        { CollectibleKeys.FOOD, GD.Load<PackedScene>("res://Scenes/Props/Food.tscn") },
    };

    public static readonly Dictionary<Character.Type, Texture2D> AVATAR_PREFAB = new Dictionary<Character.Type, Texture2D>()
    {
        { Character.Type.PUNK, GD.Load<Texture2D>("res://Assets/art/ui/avatars/avatar-punk.png") },
        { Character.Type.GOON, GD.Load<Texture2D>("res://Assets/art/ui/avatars/avatar-goon.png") },
        { Character.Type.THUG, GD.Load<Texture2D>("res://Assets/art/ui/avatars/avatar-thug.png") },
        { Character.Type.BOUNCER, GD.Load<Texture2D>("res://Assets/art/ui/avatars/avatar-boss.png") },
    };

    public static readonly Dictionary<EffectType, PackedScene> EFFECT_PREFAB = new Dictionary<EffectType, PackedScene>()
    {
        { EffectType.SPARK, GD.Load<PackedScene>("res://Scenes/Props/Spark.tscn") },
    };
}
