using System.Collections.Generic;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    [System.Serializable]
    public class FruitData
    {
        [Header("Fruit Info")]
        public int fruitID;

        public GameObject fruitPrefab;
    }

    [Header("Fruit List")]
    [SerializeField] private List<FruitData> fruits = new List<FruitData>();

    private void OnValidate()
    {
        AssignFruitIDs();
    }

    /// <summary>
    /// Tự động gán ID theo thứ tự list
    /// </summary>
    private void AssignFruitIDs()
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            fruits[i].fruitID = i;
        }
    }

    /// <summary>
    /// Lấy prefab fruit theo ID
    /// </summary>
    public GameObject GetFruitPrefab(int id)
    {
        if (id < 0 || id >= fruits.Count)
            return null;

        return fruits[id].fruitPrefab;
    }

    /// <summary>
    /// Lấy ID cuối cùng (fruit lớn nhất)
    /// </summary>
    public int GetMaxFruitID()
    {
        return fruits.Count - 1;
    }

    /// <summary>
    /// Lấy prefab fruit kế tiếp
    /// </summary>
    public GameObject GetNextFruitPrefab(int currentID)
    {
        int nextID = currentID + 1;

        if (nextID >= fruits.Count)
            return null;

        return fruits[nextID].fruitPrefab;
    }
}