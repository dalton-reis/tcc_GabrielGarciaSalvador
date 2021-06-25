using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPosition : MonoBehaviour
{

    public OVRInput.Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
        transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
    }
}
