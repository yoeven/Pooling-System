using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour {

    public GameObject Prefab;
    GameObject g;


    void Start () {
        Pooler.instance.CreatePool(Prefab.name, Prefab, 100);
        g = Pooler.instance.GetPoolItem(Prefab.name, transform.position, Quaternion.identity);
        Pooler.instance.DisablePoolItem(g,5);
        Invoke("StopDisable", 4);
        GameObject t =  Pooler.instance.GetPoolItem("Sphere", Vector3.one, Quaternion.identity);
    }

    void StopDisable()
    {
        Pooler.instance.CancelItemDisable(g);
    }
}
