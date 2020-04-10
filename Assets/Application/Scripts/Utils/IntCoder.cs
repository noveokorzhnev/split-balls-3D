using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeCoderInfo = System.ValueTuple<string, System.Func<int, string>, System.Func<string, int>>;

public static class IntCoder
{
    private static System.Func<int, string> SimpleEncoder(int term)
    {
        return value => (value + term).ToString();
    }

    private static System.Func<string, int> SimpleDecoder(int term)
    {
        return value => int.Parse(value) - term;
    }

    private static readonly Dictionary<System.Type, TypeCoderInfo[]> typeInfos = new Dictionary<System.Type, TypeCoderInfo[]>()
    {
        {
            typeof(GameCurrencySystem),
            new TypeCoderInfo[]
            {
                ("c_0", SimpleEncoder(9001), SimpleDecoder(9001))
            }
        },
        {
            typeof(GameCurrencySystem.CurrencyValue),
            new TypeCoderInfo[]
            {
                ("c_0_v", SimpleEncoder(1337), SimpleDecoder(1337))
            }
        },
        {
            typeof(StageSystem),
            new TypeCoderInfo[]
            {
                ("s_s", SimpleEncoder(159246), SimpleDecoder(159246))
            }
        },
        {
            typeof(BallSystem),
            new TypeCoderInfo[]
            {
                ("b_s_c", SimpleEncoder(412), SimpleDecoder(412)),
                ("b_s_i", SimpleEncoder(7643), SimpleDecoder(7643))
            }
        },
        {
            typeof(KeysSystem),
            new TypeCoderInfo[]
            {
                ("k_t", SimpleEncoder(198), SimpleDecoder(198))
            }
        },
    };

    public static string GetDataKey(System.Type type, int index)
    {
        return typeInfos[type][index].Item1;
    }

    public static string Encode(System.Type type, int index, int value)
    {
        return typeInfos[type][index].Item2(value);
    }

    public static int Decode(System.Type type, int index, string value)
    {
        return typeInfos[type][index].Item3(value);
    }
}
