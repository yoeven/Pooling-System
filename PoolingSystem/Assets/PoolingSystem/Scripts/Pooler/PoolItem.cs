using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItem : MonoBehaviour {

    public string PoolName;
    public GameObject[] PoolObjects;

    public void Pool(string Name, GameObject Item, int amount)
    {
        PoolName = Name;
        PoolObjects = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            GameObject g = Instantiate(Item) as GameObject;
            g.SetActive(false);
            PoolObjects[i] = g;
        }
    }

    public GameObject GetPooledItem()
    {
        for (int i = 0; i < PoolObjects.Length; i++)
        {
            if(!PoolObjects[i].activeSelf)
            {
                PoolObjects[i].SetActive(true);
                return PoolObjects[i];
            }
        }

        return null;
    }

}
