using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class PickupHand : MonoBehaviour
{
    public float distToPickup = 0.3f;
    public bool isHandClosed = false;

    public XRController controller = null;
    public LayerMask pickupableLayer;
    public Rigidbody holdingTarget;
    private GameObject hand;
    private isTrigger isTrigger;
    private GameObject holdingObjectHand;
    private GameObject holdingObject;
    private GameObject objectToHide;
    private GameObject objectHandToHide;
    private GameObject auxObjectToDestroy;
    public GrabbedObjectsManager grabbedObjectsManager;
    public PickupHand otherHand;
    private bool firstMove = true;

    public WhichHand whichHand = WhichHand.RIGHT;

    public enum WhichHand { LEFT, RIGHT };

    // Start is called before the first frame update
    void Awake()
    {
        hand = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripped))
        {
            this.isHandClosed = gripped;
        }

        GrabAndMoveObject();
    }

    private void GrabAndMoveObject()
    {
        if (!isHandClosed)
        {
            if (auxObjectToDestroy != null)
            {
                Destroy(this.auxObjectToDestroy);
                this.auxObjectToDestroy = null;
            }
            FindObjectToGrab();
        }
        else
        {
            //Se a mão está fechada e possui um objeto selecionado
            if (holdingTarget && holdingTarget != otherHand.holdingTarget)
            {
                if (firstMove)
                {
                    Renderer[] childArray = holdingTarget.GetComponentsInChildren<Renderer>();
                    foreach (Renderer render in childArray)
                    {
                        render.material.SetFloat("_OutlineWidth", 0.000f);
                    }
                    this.holdingTarget.position = transform.position;
                    this.holdingTarget.rotation = transform.rotation;
                    firstMove = false;
                }
                //Se a mão está visível, esconde ela.
                if (hand.active)
                    {
                        StopCoroutine(showHand());
                        hand.SetActive(false);
                        showHoldingObjectHand();
                    }
                    bool isObjectAgainstWall = isHoldingTargetAgainstWall();

                    MoveAndRotateHoldingObject(isObjectAgainstWall);
            }
        }
    }

    private void MoveAndRotateHoldingObject(bool isObjectAgainstWall)
    {
        if (holdingTarget.name.Contains("ShapeParent") && this.auxObjectToDestroy == null){
            this.auxObjectToDestroy = GameObject.Instantiate(holdingTarget.gameObject) as GameObject;
            Destroy(auxObjectToDestroy.transform.root.GetComponent<changeColor>());
            this.auxObjectToDestroy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.auxObjectToDestroy.GetComponent<Rigidbody>().useGravity = true;
            holdingTarget = this.auxObjectToDestroy.GetComponent<Rigidbody>();
        }
        //Move a posição do objeto na mão
        this.holdingTarget.velocity = (transform.position - holdingTarget.transform.position) / Time.fixedDeltaTime;
        if (!isObjectAgainstWall)
        {
            //Rotaciona o objeto na mão
            this.holdingTarget.maxAngularVelocity = 20;
            Quaternion deltaRot = transform.rotation * Quaternion.Inverse(this.holdingTarget.transform.rotation);
            Vector3 eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y),
                Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
            eulerRot *= 0.95f;
            eulerRot *= Mathf.Deg2Rad;
            this.holdingTarget.angularVelocity = eulerRot / Time.fixedDeltaTime;
        }
    }

    private bool isHoldingTargetAgainstWall()
    {
        //Procura se o objeto possui um script isTrigger para o caso do canetão/apagador
        this.isTrigger = this.holdingTarget.GetComponentInChildren<isTrigger>();
        var isObjectTriggered = false;

        if (this.isTrigger != null)
        {
            isObjectTriggered = this.isTrigger.isTriggered;
        }

        return isObjectTriggered;
    }

    private void FindObjectToGrab()
    {
        //Mostra a mão quando aberta
        if (!hand.active)
            StartCoroutine(showHand());
        if (this.holdingObjectHand != null && this.holdingObjectHand.active)
        {
            this.holdingObjectHand.SetActive(false);
        }
            //Verifica em um raio próximo a mão todos os objetos com colliders
            Collider[] colliders = Physics.OverlapSphere(transform.position, distToPickup, pickupableLayer);
        if (colliders.Length > 0)
        {
            //Se existe objeto dentro do raio, pegue o mais próximo da mão
            float lastDistance = float.MaxValue;
            Collider closestCollider = new Collider();
            foreach(Collider collider in colliders)
            {
                float distance = Vector3.Distance(collider.transform.position, transform.position);
                if(distance < lastDistance)
                {
                    lastDistance = distance;
                    closestCollider = collider;
                }
            }
            if (closestCollider != null)
            {
                Rigidbody aux = closestCollider.transform.root.GetComponent<Rigidbody>();
                if (otherHand.holdingTarget != aux && aux != this.holdingTarget)
                {
                    Renderer[] childArray;
                    if (holdingTarget != null)
                    {
                        childArray = holdingTarget.GetComponentsInChildren<Renderer>();
                        foreach (Renderer render in childArray)
                        {
                            render.material.SetFloat("_OutlineWidth", 0.000f);
                        }
                    }
                    holdingTarget = aux;
                    childArray = holdingTarget.GetComponentsInChildren<Renderer>();
                    Debug.Log(childArray.Length);
                    foreach (Renderer render in childArray)
                    {
                        render.material.SetColor("_OutlineColor", new Color(0, 255, 244, 1));
                        render.material.SetFloat("_OutlineWidth", 0.005f);
                    }
                    firstMove = true;
                }
            }
        }
        else
        {
            if(holdingTarget != null)
            {
                Renderer[] childArray = holdingTarget.GetComponentsInChildren<Renderer>();
                foreach (Renderer render in childArray)
                {
                    render.material.SetFloat("_OutlineWidth", 0.000f);
                }
            }
            holdingTarget = null;
        }
    }

    IEnumerator showHand()
    {
        yield return new WaitForSeconds(0.9f);
        this.hand.SetActive(true);
    }

    void showHoldingObjectHand()
    {
        if (this.holdingTarget != null)
        {
            if (this.holdingTarget.GetComponent<grabbableHands>() != null)
            {
                if (this.whichHand == WhichHand.LEFT)
                {
                    this.holdingObjectHand = this.holdingTarget.GetComponent<grabbableHands>().leftHandObject;
                    this.holdingObject = this.holdingTarget.GetComponent<grabbableHands>().leftObject;
                    this.objectToHide = this.holdingTarget.GetComponent<grabbableHands>().rightObject;
                    this.objectHandToHide = this.holdingTarget.GetComponent<grabbableHands>().rightHandObject;
                }
                else if (this.whichHand == WhichHand.RIGHT)
                {
                    this.holdingObjectHand = this.holdingTarget.GetComponent<grabbableHands>().rightHandObject;
                    this.holdingObject = this.holdingTarget.GetComponent<grabbableHands>().rightObject;
                    this.objectToHide = this.holdingTarget.GetComponent<grabbableHands>().leftObject;
                    this.objectHandToHide = this.holdingTarget.GetComponent<grabbableHands>().leftHandObject;
                }

                this.objectHandToHide.SetActive(false);
                this.objectToHide.SetActive(false);
                this.holdingObject.SetActive(true);
                this.holdingObjectHand.SetActive(true);

            }
        }
    }

}
