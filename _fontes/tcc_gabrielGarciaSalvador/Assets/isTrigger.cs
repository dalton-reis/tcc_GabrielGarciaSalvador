using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isTrigger : MonoBehaviour
{

    public bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "DrawingGlass" || other.gameObject.name == "Body")
        {
            this.isTriggered = true;
            transform.root.Rotate(Vector3.zero);
            transform.root.GetComponent<Rigidbody>().freezeRotation = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.name == "DrawingGlass" || other.gameObject.name == "Body")
        {
            this.isTriggered = false;
            transform.root.GetComponent<Rigidbody>().freezeRotation = false;
        }

    }

}
