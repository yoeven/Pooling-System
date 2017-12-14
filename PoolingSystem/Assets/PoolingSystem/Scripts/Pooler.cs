using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pooler : MonoBehaviour
{
    public static Pooler instance;
    public ItemToPool[] PrefabsToPool;

    GameObject PoolItemsObject;
    List<PoolItem> PoolItems;
    Dictionary<GameObject, Coroutine> ItemsToBeDisabled;


    private void Awake()
    {
        if (instance == null) instance = this; else if (instance != this) Destroy(gameObject);
        PoolItemsObject = new GameObject();
        PoolItemsObject.name = "PoolItemsObject";
        PoolItemsObject.transform.SetParent(transform);
        PoolItems = new List<PoolItem>();
        ItemsToBeDisabled = new Dictionary<GameObject, Coroutine>();

        for (int i = 0; i < PrefabsToPool.Length; i++)
        {
            if (PrefabsToPool[i].PrefabToPool == null)
            {
                Debug.Log("Prefab at index " + i + " has no prefab");
                continue;
            }

            string Name = PrefabsToPool[i].Name == "" || PrefabsToPool[i].Name == null ? PrefabsToPool[i].PrefabToPool.name : PrefabsToPool[i].Name;
            int Amount = PrefabsToPool[i].Amount == 0 ? 100 : PrefabsToPool[i].Amount;
            CreatePool(Name, PrefabsToPool[i].PrefabToPool, Amount);
        }
    }

    public void CreatePool(string ItemPoolName, GameObject Item, int amount)
    {
        PoolItem p = PoolItemsObject.AddComponent<PoolItem>() as PoolItem;
        p.Pool(ItemPoolName, Item, amount);
        PoolItems.Add(p);
    }

    public GameObject GetPoolItem(string ItemName, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < PoolItems.Count; i++)
        {
            if (PoolItems[i].PoolName == ItemName)
            {
                GameObject item = PoolItems[i].GetPooledItem();
                if (item == null)
                {
                    Debug.Log("No Pool Item Left");
                    return null;
                }
                item.transform.SetParent(null);
                item.transform.position = position;
                item.transform.rotation = rotation;
                item.SendMessage("Init", SendMessageOptions.DontRequireReceiver);
                return item;
            }
        }
        Debug.Log("No Pool Item Left");
        return null;
    }

    public void DisableAllOfType(string ItemName)
    {
        for (int i = 0; i < PoolItems.Count; i++)
        {
            if (PoolItems[i].PoolName == ItemName)
            {
                for (int y = 0; y < PoolItems[i].PoolObjects.Length; y++)
                {
                    DisablePoolItem(PoolItems[i].PoolObjects[y]);
                }
                break;
            }
        }
    }

    public void DisablePoolItem(GameObject item)
    {
        item.transform.SetParent(null);
        item.SetActive(false);
    }

    public void DisablePoolItem(GameObject item, float Time)
    {
        if (ItemsToBeDisabled.ContainsKey(item))
        {
            Debug.LogError(item + "Object already has invoked a disable, cancle old disable before invoking a new one.");
            return;
        }
        Coroutine disableStore = StartCoroutine(DiablePoolItemByTime(item, Time));
        ItemsToBeDisabled.Add(item, disableStore);
    }

    public void DisablePoolItem(GameObject[] item)
    {
        for (int i = 0; i < item.Length; i++)
        {
            DisablePoolItem(item[i]);
        }
    }

    public void DisablePoolItem(GameObject[] item, float Time)
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (ItemsToBeDisabled.ContainsKey(item[i]))
            {
                Debug.LogError(item[i] + " Object already has invoked a disable, cancle old disable before invoking a new one.");
                return;
            }
            Coroutine disableStore = StartCoroutine(DiablePoolItemByTime(item[i], Time));
            ItemsToBeDisabled.Add(item[i], disableStore);
        }
    }

    public void CancelItemDisable(GameObject item)
    {
        if (ItemsToBeDisabled.ContainsKey(item))
        {
            StopCoroutine(ItemsToBeDisabled[item]);
            ItemsToBeDisabled.Remove(item);
        }
        else
        {
            Debug.Log("Object has no pending disable stored.");
        }
    }

    IEnumerator DiablePoolItemByTime(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ItemsToBeDisabled.Remove(item);
        DisablePoolItem(item);
    }
}

[Serializable]
public class ItemToPool
{
    [Tooltip("Leave Name blank to use prefab name for Name field")]
    public string Name;
    public GameObject PrefabToPool;
    [Tooltip("Leave Amount to 0 to use a default of 100")]
    public int Amount;
}
