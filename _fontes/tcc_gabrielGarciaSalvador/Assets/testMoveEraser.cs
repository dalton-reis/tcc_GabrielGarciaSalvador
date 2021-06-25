using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMoveEraser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.up * 0.0005f);
        }
        if (Input.GetKey("left"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.left * 0.0005f);
        }
        if (Input.GetKey("right"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.right * 0.0005f);
        }
        if (Input.GetKey("down"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.down * 0.0005f);
        }
        if (Input.GetKey("i"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.forward * 0.0005f);
        }
        if (Input.GetKey("k"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.back * 0.0005f);
        }
    }
}
