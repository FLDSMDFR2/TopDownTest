using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RandomGenerator : MonoBehaviour
{
    public string Seed;
    protected static System.Random SeedRandom;

    void Awake()
    {
        SeedRandom = new System.Random(GetSeed());
    }

    public static int SeededRange(int Min, int Max)
    {
        return SeedRandom.Next(Min, Max);
    }

    #region GenerationSeed
    protected virtual int GetSeed()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(Seed);
        int retval = 0;
        foreach (var b in bytes)
        {
            retval += Convert.ToInt32(b);
        }
        return retval;
    }
    #endregion

}
