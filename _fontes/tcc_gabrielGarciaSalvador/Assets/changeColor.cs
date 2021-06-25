using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine;

public class changeColor : MonoBehaviour
{
    public PickupHand rightHand;
    private bool changingColor = false;
    private Queue<Color> colorRotation = new Queue<Color> ( new[] { Color.black, Color.blue, Color.red, Color.green, Color.yellow, Color.magenta, Color.white } );

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rightHand.GetComponent<PickupHand>().controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger))
        {
            if (trigger && !changingColor)
            {
                StartCoroutine(SwapColor());
            }
        }
    }

    IEnumerator SwapColor()
    {
        if (rightHand.holdingTarget != null)
        {
            if (rightHand.holdingTarget.transform.root.name == transform.gameObject.name)
            {
                changingColor = true;
                Renderer[] childArray;
                Color colorToChange = colorRotation.Peek();
                colorRotation.Enqueue(colorRotation.Dequeue());
                childArray = transform.GetComponentsInChildren<Renderer>();
                Debug.Log(colorToChange);
                foreach (Renderer render in childArray)
                {
                    render.material.SetColor("_Color", colorToChange);

                }

            }
        }
        yield return new WaitForSeconds(0.5f);
        changingColor = false;
    }

}
