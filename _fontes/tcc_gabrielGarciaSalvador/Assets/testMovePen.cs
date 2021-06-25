using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMovePen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.up * 0.0005f);
        }
        if (Input.GetKey("a"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.left * 0.0005f);
        }
        if (Input.GetKey("d"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.right * 0.0005f);
        }
        if (Input.GetKey("s"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.down * 0.0005f);
        }
        if (Input.GetKey("e"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.forward * 0.0005f);
        }
        if (Input.GetKey("q"))
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + (Vector3.back * 0.0005f);
        }
    }
}
