using UnityEngine;
using System.Collections.Generic;

public class PlayerKeyInventory : MonoBehaviour
{
    private HashSet<int> keys = new HashSet<int>();

    public void AddKey(int keyNumber)
    {
        keys.Add(keyNumber);
        Debug.Log($"[KeyInventory] Added Key {keyNumber}");
    }

    public bool HasKey(int keyNumber)
    {
        return keys.Contains(keyNumber);
    }

    public List<int> GetAllKeys()
    {
        return new List<int>(keys);
    }
}
