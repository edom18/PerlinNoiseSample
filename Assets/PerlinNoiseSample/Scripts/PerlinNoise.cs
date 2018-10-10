using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xorshift
{
    private uint[] _vec = new uint[4];

    public Xorshift(uint seed = 100)
    {
        for (uint i = 1; i <= _vec.Length; i++)
        {
            seed = 1812433253 * (seed ^ (seed >> 30)) + i;
            _vec[i - 1] = seed;
        }
    }

    public float Random()
    {
        uint t = _vec[0];
        uint w = _vec[3];

        _vec[0] = _vec[1];
        _vec[1] = _vec[2];
        _vec[2] = w;

        t ^= t << 11;
        t ^= t >> 8;
        w ^= w >> 19;
        w ^= t;

        _vec[3] = w;

        return w * 2.3283064365386963e-10f;
    }
}

public class PerlinNoise
{
    private Xorshift _xorshit;
    private int[] _p;

    /// <summary>
    /// Constructor
    /// </summary>
    public PerlinNoise(uint seed)
    {
        _xorshit = new Xorshift(seed);

        int[] p = new int[256];
        for (int i = 0; i < p.Length; i++)
        {
            // 0 - 255の間のランダムな値を生成する
            p[i] = (int)Mathf.Floor(_xorshit.Random() * 256);
        }

        // pの倍の数の配列を生成する
        int[] p2 = new int[p.Length * 2];
        for (int i = 0; i < p2.Length; i++)
        {
            p2[i] = p[i & 255];
        }

        _p = p2;
    }

    private float Fade(float t)
    {
        // 6t^5 - 5t^5 + 10t^3
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    /// <summary>
    /// Linear interpoloation
    /// </summary>
    private float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    /// <summary>
    /// Calculate gradient vector.
    /// </summary>
    private float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;
        float u = (h < 8) ? x : y;
        float v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    /// <summary>
    /// To simplify above function to below.
    /// </summary>
    // private float Grad(int hash, float x, float y, float z)
    // {
    //     switch(hash & 0xF)
    //     {
    //         case 0x0: return  x + y;
    //         case 0x1: return -x + y;
    //         case 0x2: return  x - y;
    //         case 0x3: return -x - y;
    //         case 0x4: return  x + z;
    //         case 0x5: return -x + z;
    //         case 0x6: return  x - z;
    //         case 0x7: return -x - z;
    //         case 0x8: return  y + z;
    //         case 0x9: return -y + z;
    //         case 0xA: return  y - z;
    //         case 0xB: return -y - z;
    //         case 0xC: return  y + x;
    //         case 0xD: return -y + z;
    //         case 0xE: return  y - x;
    //         case 0xF: return -y - z;
    //         default: return 0; // never happens
    //     }
    // }

    private float Noise(float x, float y = 0, float z = 0)
    {
        // Repeat while 0 - 255
        int X = (int)Mathf.Floor(x) & 255;
        int Y = (int)Mathf.Floor(y) & 255;
        int Z = (int)Mathf.Floor(z) & 255;

        // trim integer
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        z -= Mathf.Floor(z);

        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        int[] p = _p;

        #region ### calulate hashes from array of p ###
        int A, AA, AB, B, BA, BB;

        A = p[X + 0] + Y; AA = p[A] + Z; AB = p[A + 1] + Z;
        B = p[X + 1] + Y; BA = p[B] + Z; BB = p[B + 1] + Z;
        #endregion ### calulate hashes from array of p ###

        float a = Grad(p[AA + 0], x + 0, y + 0, z + 0);
        float b = Grad(p[BA + 0], x - 1, y + 0, z + 0);
        float c = Grad(p[AB + 0], x + 0, y - 1, z + 0);
        float d = Grad(p[BB + 0], x - 1, y - 1, z + 0);
        float e = Grad(p[AA + 1], x + 0, y + 0, z - 1);
        float f = Grad(p[BA + 1], x - 1, y + 0, z - 1);
        float g = Grad(p[AB + 1], x + 0, y - 1, z - 1);
        float h = Grad(p[BB + 1], x - 1, y - 1, z - 1);

        return Lerp(w, Lerp(v, Lerp(u, a, b),
                               Lerp(u, c, d)),
                       Lerp(v, Lerp(u, e, f),
                               Lerp(u, g, h)));
    }

    public float OctaveNoise(float x, int octaves)
    {
        float result = 0;
        float amp = 1.0f;

        for (int i = 0; i < octaves; i++)
        {
            result += Noise(x) * amp;
            x *= 2.0f;
            amp *= 0.5f;
        }

        return result;
    }

    public float OctaveNoise(float x, float y, int octaves)
    {
        float result = 0;
        float amp = 1.0f;

        for (int i = 0; i < octaves; i++)
        {
            result += Noise(x, y) * amp;
            x *= 2.0f;
            y *= 2.0f;
            amp *= 0.5f;
        }

        return result;
    }

    public float OctaveNoise(float x, float y, float z, int octaves)
    {
        float result = 0;
        float amp = 1.0f;

        for (int i = 0; i < octaves; i++)
        {
            result += Noise(x, y, z) * amp;
            x *= 2.0f;
            y *= 2.0f;
            z *= 2.0f;
            amp *= 0.5f;
        }

        return result;
    }
}
