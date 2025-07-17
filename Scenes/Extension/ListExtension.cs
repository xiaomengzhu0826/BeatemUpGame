using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random _rng = new Random();

    // 扩展方法：打乱 List<T> 中的元素顺序
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1); // 生成 0 到 n（包含）的随机数
            (list[k], list[n]) = (list[n], list[k]); // C# 7.0+ 支持元组交换
        }
    }

    public static T PopFront<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new InvalidOperationException("Cannot PopFront from an empty or null list.");

        T first = list[0];
        list.RemoveAt(0);
        return first;
    }
}