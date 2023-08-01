using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BetterRandom
{
    public static float Range(float a, float b)
    {
        return UnityEngine.Random.Range(a, b);
    }

    public static int Range(int a, int b)
    {
        return UnityEngine.Random.Range(a, b + 1);
    }

    public static bool RandomBool
    {
        get
        {
            return Range(0, 1) == 1;
        }
    }

    public static bool GetBool(int chance)
    {
        if (chance == 0)
        {
            return true;
        }
        return Range(0, chance) == chance;
    }

    public static T RandomElement<T>(T[] array)
    {
        return array[Range(0, array.Length - 1)];
    }

    public static T RandomElement<T>(List<T> list)
    {
        return list[Range(0, list.Count - 1)];
    }

    public static T RandomElement<T>(System.Collections.Generic.IEnumerable<T> enumerable)
    {
        T[] array = enumerable.ToArray();
        return array[Range(0, array.Length - 1)];
    }
}
