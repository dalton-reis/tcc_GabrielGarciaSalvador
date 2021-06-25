using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundCar : MonoBehaviour
{
    public GameObject endpoint;
    public GameObject startpoint;
    private int speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        this.speed = Random.Range(5, 11);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == endpoint.transform.position)
        {
            transform.position = startpoint.transform.position;
            this.speed = Random.Range(5, 9);
            transform.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());
        }
        transform.position = Vector3.MoveTowards(transform.position, endpoint.transform.position, this.speed * Time.deltaTime);
    }
}
