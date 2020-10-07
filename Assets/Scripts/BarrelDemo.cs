using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelDemo : MonoBehaviour
{
    public Transform barrel;
    public Transform target;

    void Start()
    {
        var barrelInstance = Instantiate(barrel);
        barrelInstance.SetParent(target);
        barrelInstance.transform.localPosition = Vector3.zero;
        barrelInstance.transform.localRotation = Quaternion.identity;
    }
}
