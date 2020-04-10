using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCollection : ScriptableObject
{
    [System.Serializable]
    public class Group
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private GameObject[] prefabs;

        public string Name => name;
        public GameObject[] Prefabs => prefabs;
    }

    [SerializeField]
    protected Group[] groups;

    public int GetGroupsCount()
    {
        return groups.Length;
    }

    public int GetPrefabsCount(int groupIndex)
    {
        var prefabs = GetGroup(groupIndex);
        if (prefabs == null)
        {
            return 0;
        }

        return prefabs.Length;
    }

    public GameObject[] GetGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= groups.Length)
        {
            Debug.LogError(string.Format("{0}: Cannot get a prefab from group {1} - valid groups range is [0, {2}).", name, groupIndex, groups.Length));
            return null;
        }

        return groups[groupIndex].Prefabs;
    }

    public GameObject GetPrefab(int groupIndex, int prefabIndex)
    {
        var prefabs = GetGroup(groupIndex);
        if (prefabs == null)
        {
            return null;
        }

        if (prefabIndex < 0 || prefabIndex >= prefabs.Length)
        {
            Debug.LogError(string.Format("{0}: Cannot get a prefab from group {1} at index {2} - valid range is [0, {3}).", name, groupIndex, prefabIndex, prefabs.Length));
            return null;
        }

        return prefabs[prefabIndex];
    }
}
