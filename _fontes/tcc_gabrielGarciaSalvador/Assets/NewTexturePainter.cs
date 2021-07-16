using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Collections;
using UnityEngine;

public class NewTexturePainter : MonoBehaviour
{
    public GameObject brushCursor, brushContainer; //The cursor that overlaps the model and our container for the brushes painted
    public Camera sceneCamera, canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
    public Sprite cursorPaint, cursorDecal; // Cursor for the differen functions 
    public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
    public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)
    public GameObject leftHand;
    public GameObject rightHand;
    private GameObject pen;
    private Vector3 auxUvWorldPosition;
    private GameObject currentPaintingBrush;
    private GameObject placingShape;
    public Material trailMaterial;
    public GameObject drawingBoard;
    private Material shapeMaterial;
    public GameObject wipeBoard;
    public bool resettingQuadro = false;

    Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
    float brushSize = 0.040f; //The size of our brush
    Color brushColor; //The selected color
    int brushCounter = 0, MAX_BRUSH_COUNT = 1000; //To avoid having millions of brushes
    bool saving = false; //Flag to check if we are saving the texture
    bool erasing = false;
    bool hasChanges = false;
    bool isPlacingShape = false;

    void Start()
    {
        shapeMaterial = Resources.Load("Materials/ShapeMaterial", typeof(Material)) as Material;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        baseMaterial.mainTexture = tex;
        RenderTexture.active = null;
    }

    public IEnumerator resetQuadro()
    {
        resettingQuadro = true;
        wipeBoard.SetActive(true);
        yield return new WaitForSeconds(3);
        wipeBoard.SetActive(false);
        resettingQuadro = false;
    }

    void FixedUpdate()
    {
        var rightHandPickedObject = rightHand.GetComponent<PickupHand>().holdingTarget;
        var leftHandPickedObject = leftHand.GetComponent<PickupHand>().holdingTarget;
        var leftHandClosed = leftHand.GetComponent<PickupHand>().isHandClosed;
        var rightHandClosed = rightHand.GetComponent<PickupHand>().isHandClosed;
        if (rightHandPickedObject != null || leftHandPickedObject != null)
        {
            pen = null;
            if (rightHandPickedObject != null && rightHandPickedObject.transform.childCount > 0 && rightHandPickedObject.GetComponentInChildren<isTrigger>() != null)
            {
                if (rightHandPickedObject.name.Contains("Shape"))
                {
                    pen = rightHandPickedObject.transform.GetChild(0).gameObject;
                }
                else
                {
                    pen = rightHandPickedObject.transform.GetComponent<PenRef>().rightHandPen;
                }
            }
            else if (leftHandPickedObject != null && leftHand.transform.childCount > 0 && leftHand.GetComponentInChildren<isTrigger>() != null)
            {
                if (leftHandPickedObject.name.Contains("Shape"))
                {
                    pen = leftHandPickedObject.transform.GetChild(0).gameObject;
                }
                else
                {
                    pen = leftHandPickedObject.transform.GetComponent<PenRef>().leftHandPen;
                }
            }
            if (pen != null && pen.GetComponent<isTrigger>() != null && pen.GetComponent<isTrigger>().isTriggered)
            {
                if (pen.name.Contains("Shape"))
                {
                    PlaceOverDrawingBoard();

                }
                else
                {
                    DoAction();
                }
            }
            else
            {
                if (currentPaintingBrush)
                {
                    Debug.Log("destructed");
                    Destroy(currentPaintingBrush);
                }
                currentPaintingBrush = null;
                auxUvWorldPosition = Vector3.zero;
            }
        }
        else
        {
            Destroy(placingShape);
            placingShape = null;
        }
        //UpdateBrushCursor();
    }

    private void PlaceOverDrawingBoard()
    {
        if (placingShape == null)
        {
            if (pen.name.Contains("Sphere"))
            {
                placingShape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                placingShape.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                Destroy(placingShape.GetComponent<Collider>());
            }
            else if (pen.name.Contains("Cube"))
            {
                placingShape = GameObject.CreatePrimitive(PrimitiveType.Cube);
                placingShape.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                Destroy(placingShape.GetComponent<Collider>());
            }
            else if (pen.name.Contains("Checkerboard"))
            {
                placingShape = Instantiate(Resources.Load("CheckerboardShape")) as GameObject;
                placingShape.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                Destroy(placingShape.GetComponent<Collider>());
            }
            else if (pen.name.Contains("GraphShape"))
            {
                placingShape = Instantiate(Resources.Load("GraphShape")) as GameObject;
                placingShape.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
                Destroy(placingShape.GetComponent<Collider>());
            }

            shapeMaterial.SetColor("_Color", pen.GetComponent<Renderer>().material.GetColor("_Color"));
            shapeMaterial.SetColor("_EmissionColor", pen.GetComponent<Renderer>().material.GetColor("_Color"));
            placingShape.AddComponent<MeshRenderer>();
            placingShape.AddComponent<MeshFilter>();
            MeshRenderer[] meshRenderers;
            meshRenderers = placingShape.transform.root.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = shapeMaterial;
            }
            placingShape.GetComponent<MeshRenderer>().material = shapeMaterial;
            placingShape.transform.position = drawingBoard.transform.position + new Vector3(0, 0, -0.01f);
        }

        if (rightHand.GetComponent<PickupHand>().controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            if (axis.y > 0.5)
            {
                if (placingShape.transform.localScale.x > 0 && placingShape.transform.localScale.y > 0)
                {
                    placingShape.transform.localScale += new Vector3(0.25f, 0.25f, 0f) * Time.deltaTime;
                }
            }
            else if (axis.y < 0)
            {
                if (placingShape.transform.localScale.x > 0 && placingShape.transform.localScale.y > 0)
                {
                    placingShape.transform.localScale -= new Vector3(0.25f, 0.25f, 0f) * Time.deltaTime;
                }
            }
        }
        if (rightHand.GetComponent<PickupHand>().controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger))
        {
            if (trigger && !isPlacingShape)
            {
                isPlacingShape = true;
                StartCoroutine(placeShapeInDrawingBoard());
            }
        }

        placingShape.transform.position = new Vector3(pen.transform.position.x, pen.transform.position.y, placingShape.transform.position.z);

    }

    private IEnumerator placeShapeInDrawingBoard()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {

            currentPaintingBrush = Instantiate(placingShape) as GameObject;

            Vector3 paintedObjectScaleAdjustments = new Vector3(currentPaintingBrush.transform.localScale.x * 0.15f, currentPaintingBrush.transform.localScale.y * 0.15f, currentPaintingBrush.transform.localScale.z);
            paintedObjectScaleAdjustments *= -1;
            currentPaintingBrush.transform.localScale = paintedObjectScaleAdjustments;

            currentPaintingBrush.transform.parent = brushContainer.transform;
            currentPaintingBrush.transform.localPosition = uvWorldPosition; 
                                                                            
            yield return new WaitForSeconds(1);
            Destroy(currentPaintingBrush);
            currentPaintingBrush = null;
        }
        isPlacingShape = false;
    }

    void DoAction()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (currentPaintingBrush == null)
        {
            if (pen.name.Contains("ERASER"))
            {
                currentPaintingBrush = Instantiate(Resources.Load("EraserCube")) as GameObject; //Paint a brus
                currentPaintingBrush.transform.localScale = new Vector3(1, 2, 0.01f) * 0.01f;
            }
            else
            {
                currentPaintingBrush = (GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); 
                currentPaintingBrush.AddComponent<TrailRenderer>();
                if (pen.name.Contains("RED"))
                {

                    currentPaintingBrush.GetComponent<SpriteRenderer>().color = Color.red;
                    currentPaintingBrush.transform.localScale = new Vector3(1, 1, 1) * 0.0008f;
                    trailMaterial.SetColor("_EmissionColor", Color.red);
                    currentPaintingBrush.GetComponent<TrailRenderer>().material = trailMaterial;
                    currentPaintingBrush.GetComponent<TrailRenderer>().time = 0.25f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().minVertexDistance = 0.005f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startColor = Color.red;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endColor = Color.red;
                    currentPaintingBrush.GetComponent<TrailRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    currentPaintingBrush.GetComponent<TrailRenderer>().textureMode = LineTextureMode.DistributePerSegment;
                    brushColor = Color.red;
                    brushColor.a = 1.0f;
                }
                else if (pen.name.Contains("GREEN"))
                {
                    currentPaintingBrush.GetComponent<SpriteRenderer>().color = Color.green; //Set the brush color
                    currentPaintingBrush.transform.localScale = new Vector3(1, 1, 1) * 0.0008f;
                    trailMaterial.SetColor("_EmissionColor", Color.green);
                    currentPaintingBrush.GetComponent<TrailRenderer>().material = trailMaterial;
                    currentPaintingBrush.GetComponent<TrailRenderer>().time = 0.25f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().minVertexDistance = 0.005f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startColor = Color.green;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endColor = Color.green;
                    currentPaintingBrush.GetComponent<TrailRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    currentPaintingBrush.GetComponent<TrailRenderer>().textureMode = LineTextureMode.DistributePerSegment;
                    brushColor = Color.green;
                    brushColor.a = 1.0f;
                }
                else if (pen.name.Contains("BLUE"))
                {
                    currentPaintingBrush.GetComponent<SpriteRenderer>().color = Color.blue; //Set the brush color
                    currentPaintingBrush.transform.localScale = new Vector3(1, 1, 1) * 0.0008f;
                    trailMaterial.SetColor("_EmissionColor", Color.blue);
                    currentPaintingBrush.GetComponent<TrailRenderer>().material = trailMaterial;
                    currentPaintingBrush.GetComponent<TrailRenderer>().time = 0.25f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endWidth = 1 * 0.002f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().minVertexDistance = 0.005f;
                    currentPaintingBrush.GetComponent<TrailRenderer>().startColor = Color.blue;
                    currentPaintingBrush.GetComponent<TrailRenderer>().endColor = Color.blue;
                    currentPaintingBrush.GetComponent<TrailRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    currentPaintingBrush.GetComponent<TrailRenderer>().textureMode = LineTextureMode.DistributePerSegment;
                    brushColor = Color.blue;
                    brushColor.a = 1.0f;
                }
            }

        }
        if (HitTestUVPosition(ref uvWorldPosition) && currentPaintingBrush != null)
        {
            if (pen.name.Contains("ERASER"))
            {
                Quaternion newRot = pen.transform.GetChild(0).rotation;
                newRot.x = 0;
                newRot.y = 0;
                currentPaintingBrush.transform.rotation = newRot;
            }
            currentPaintingBrush.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
            currentPaintingBrush.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
        }
    }

    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        if (pen.GetComponent<isTrigger>().isTriggered && Physics.Raycast(
            pen.transform.GetChild(0).position, pen.transform.GetChild(0).forward, out hit, 10f, 3 << LayerMask.NameToLayer("DrawingGlass")))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
            uvWorldPosition.y = (pixelUV.y - canvasCam.orthographicSize) / 5;//To center the UV on Y
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }
   
    public void SetBrushMode(Painter_BrushMode brushMode)
    { //Sets if we are painting or placing decals
        mode = brushMode;
        //brushCursor.GetComponent<SpriteRenderer> ().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
    }
    public void SetBrushSize(float newBrushSize)
    { //Sets the size of the cursor brush or decal
        brushSize = newBrushSize;
        //brushCursor.transform.localScale = Vector3.one * brushSize;
    }

}
