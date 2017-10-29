using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour {

    public static Pooler instance;
    public GameObject PoolItemsObject;

    List<PoolItem> PoolItems;
    Dictionary<GameObject,Coroutine> ItemsToBeDisabled;


    private void Awake()
    {
        instance = this;
        PoolItems = new List<PoolItem>();
        ItemsToBeDisabled = new Dictionary<GameObject, Coroutine>();
    }

    public void CreatePool(string ItemPoolName,GameObject Item,int amount)
    {
        PoolItem p = PoolItemsObject.AddComponent<PoolItem>() as PoolItem;
        p.Pool(ItemPoolName, Item, amount);
        PoolItems.Add(p);
    }

    public GameObject GetPoolItem(string ItemName, Vector3 position, Quaternion rotation)
    {
        for(int i =0;i<PoolItems.Count;i++)
        {
            if (PoolItems[i].PoolName == ItemName)
            {
                GameObject item = PoolItems[i].GetPooledItem();
                if (item == null)
                {
                    Debug.Log("No Pool Item Left");
                    return null;
                }
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
        item.SetActive(false);
    }

    public void DisablePoolItem(GameObject item, float Time)
    {
        Coroutine disableStore =  StartCoroutine(DiablePoolItemByTime(item, Time));
        ItemsToBeDisabled.Add(item, disableStore);
    }

    public void DisablePoolItem(GameObject[] item)
    {
        for (int i = 0; i < item.Length; i++)
        {
            item[i].SetActive(false);
        }
    }

    public void DisablePoolItem(GameObject[] item, float Time)
    {
        for (int i = 0; i < item.Length; i++)
        {
            Coroutine disableStore = StartCoroutine(DiablePoolItemByTime(item[i], Time));
            ItemsToBeDisabled.Add(item[i], disableStore);
        }
    }

    public void CancelItemDisable(GameObject item)
    {
        if(ItemsToBeDisabled.ContainsKey(item))
        {
            StopCoroutine(ItemsToBeDisabled[item]);
            ItemsToBeDisabled.Remove(item);
        }
    }

    IEnumerator DiablePoolItemByTime(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ItemsToBeDisabled.Remove(item);
        item.SetActive(false);
    }
}
