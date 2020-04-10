using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    private static bool HasValue(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    private static string GetStringValue(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    private static void SetStringValue(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static bool[][] GetUnlockedItems(string itemsType, int categoriesTotal)
    {
        bool[][] result = new bool[categoriesTotal][];

        for (int i = 0; i < categoriesTotal; i++)
        {
            string dataKey = itemsType + i;

            if (HasValue(dataKey))
            {
                string dataValue = GetStringValue(dataKey);
                result[i] = new bool[dataValue.Length];

                for (int j = 0; j < dataValue.Length; j++)
                {
                    result[i][j] = dataValue[j] != '0';
                }
            }            
        }

        return result;
    }

    public static void UnlockItem(string itemsType, int categoryIndex, int elementInCategory)
    {
        string dataKey = itemsType + categoryIndex;
        string dataValue = HasValue(dataKey) ? GetStringValue(dataKey) : string.Empty;

        while (dataValue.Length <= elementInCategory)
        {
            dataValue += '0';
        }
        dataValue = dataValue.Remove(elementInCategory, 1);
        dataValue = dataValue.Insert(elementInCategory, Random.Range(1, 10).ToString());

        SetStringValue(dataKey, dataValue);
    }

    public static int GetIntDecoded(System.Type type, int index)
    {
        string key = IntCoder.GetDataKey(type, index);
        if (HasValue(key))
        {
            return IntCoder.Decode(type, index, GetStringValue(key));
        }
        else
        {
            return 0;
        }
    }

    public static void SetIntEncoded(System.Type type, int index, int value)
    {
        string key = IntCoder.GetDataKey(type, index);

        SetStringValue(key, IntCoder.Encode(type, index, value));
    }
}
