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

    private void AssignFruitIDs()
    {
        for (int i = 0; i < fruits.Count; i++)
        {
            fruits[i].fruitID = i;
        }
    }

    public GameObject GetFruitPrefab(int id)
    {
        if (id < 0 || id >= fruits.Count) return null;

        return fruits[id].fruitPrefab;
    }
    public int GetMaxFruitID()
    {
        return fruits.Count - 1;
    }

    public GameObject GetNextFruitPrefab(int currentID)
    {
        int nextID = currentID + 1;

        if (nextID >= fruits.Count) return null;

        return fruits[nextID].fruitPrefab;
    }
}