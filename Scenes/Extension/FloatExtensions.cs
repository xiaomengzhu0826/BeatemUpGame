using Godot;
using System;

public static class FloatExtensions
{
    public static float ToDb(this float linear)
    {
        if (linear <= 0.0001f)
            return -80f;
        return 20.0f * (float)Math.Log10(linear); // 用 System.Math
    }

    public static float ToLinear(this float db)
    {
        return (float)Math.Pow(10.0f, db / 20.0f);
    }
}
