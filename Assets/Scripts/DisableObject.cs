using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableObject : MonoBehaviour
{
    public GameObject Obj;
    public float activeTime;

    void Update()
    {
        if (Obj.active == true)
        {
            StartCoroutine(DisableObj());
        }
    }

    IEnumerator DisableObj()
    {
        yield return new WaitForSeconds(activeTime);
        Obj.SetActive(false);
    }
}
