

using System.Collections;
using UnityEngine;

public enum Painter_BrushMode { PAINT, DECAL };
public class TexturePainter : MonoBehaviour
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

    Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
    float brushSize = 0.040f; //The size of our brush
    Color brushColor; //The selected color
    int brushCounter = 0, MAX_BRUSH_COUNT = 1000; //To avoid having millions of brushes
    bool saving = false; //Flag to check if we are saving the texture
    bool erasing = false;
    bool hasChanges = false;

    void Start()
    {
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        baseMaterial.mainTexture = tex;
        RenderTexture.active = null;
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
            if (rightHandPickedObject != null && rightHandPickedObject.GetComponentInChildren<isTrigger>() != null)
            {
                pen = rightHandPickedObject.transform.GetComponent<PenRef>().rightHandPen;
            }
            else if (leftHandPickedObject != null && rightHandPickedObject.GetComponentInChildren<isTrigger>() != null)
            {
                pen = leftHandPickedObject.transform.GetComponent<PenRef>().leftHandPen;
            }
            if (pen.GetComponent<isTrigger>().isTriggered)
            {
                DoAction();
            } else
            {
                auxUvWorldPosition = Vector3.zero;
            }
        }
        //UpdateBrushCursor();
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction()
    {
        if (saving || erasing)
            return;
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            if(auxUvWorldPosition == Vector3.zero)
            {
                auxUvWorldPosition = uvWorldPosition;
            }
            GameObject brushObj;
            Debug.Log(pen.name);
            switch (pen.name.Split('_')[0])
            {
                case "RED":
                    brushColor = Color.red;
                    brushColor.a = 1.0f;
                    break;
                case "BLUE":
                    brushColor = Color.blue;
                    break;
                case "GREEN":
                    brushColor = Color.green;
                    break;
                case "WHITE":
                    brushColor = Color.white;
                    break;
            }
            if ("ERASER" != pen.name.Split('_')[0])
            {
                hasChanges = true;
                float distanceBetweenPoints = Vector3.Distance(uvWorldPosition, auxUvWorldPosition);
                if (distanceBetweenPoints > 0.001)
                {
                    Debug.Log("estou above 0.001");
                    for (float i = 0; i <= 1; i += 0.1f)
                    {
                        brushObj = (GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); //Paint a brush
                        brushObj.GetComponent<SpriteRenderer>().color = brushColor; //Set the brush color
                        brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
                        brushObj.transform.localPosition = Vector3.Lerp(auxUvWorldPosition, uvWorldPosition, i); ; //The position of the brush (in the UVMap)
                        brushObj.transform.localScale = new Vector3(0.5f, 1, 1) * brushSize;
                    }
                }
                brushObj = (GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); //Paint a brush
                brushObj.GetComponent<SpriteRenderer>().color = brushColor; //Set the brush color
                //brushColor.a = brushSize * 2.0f; // Brushes have alpha to have a merging effect when painted over.
                brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
                brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
               //brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush
                brushObj.transform.localScale = new Vector3(0.5f, 1, 1) * brushSize;
            }
            else
            {

                //if (!erasing)
                //{
                //    erasing = true;
                //    StartCoroutine("EraseTexture");
                //}
                RaycastHit hit;
                if (pen.GetComponent<isTrigger>().isTriggered && Physics.Raycast(pen.transform.GetChild(0).position, pen.transform.GetChild(0).forward, out hit, 2, 3 << LayerMask.NameToLayer("DrawingGlass")))
                {
                    erasing = true;
                    Debug.Log("to apagano");
                    RenderTexture.active = canvasTexture;
                    Texture2D myTexture2D = new Texture2D(canvasTexture.width, canvasTexture.height);
                    myTexture2D.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
                    myTexture2D.Apply();
                    Renderer renderer = hit.collider.GetComponent<MeshRenderer>();
                    Vector2 pCoord = hit.textureCoord;
                    pCoord.x *= myTexture2D.width;
                    pCoord.y *= myTexture2D.height;
                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    Vector2 tiling = renderer.material.mainTextureScale;
                    Color[] testColor = new Color[50 * 300];
                    for (int i = 0; i < testColor.Length; i++)
                    {
                        testColor[i] = Color.black;
                        testColor[i].a = 0.1f;
                    }
                    //myTexture2D.SetPixel(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y), Color.clear);
                    myTexture2D.SetPixels(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y), 50, 300, testColor);
                    myTexture2D.Apply();
                    baseMaterial.mainTexture = myTexture2D;
                    erasing = false;
                }
            }

            auxUvWorldPosition = uvWorldPosition;
        }
        brushCounter++; //Add to the max brushes
        if (brushCounter >= MAX_BRUSH_COUNT)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
          //brushCursor.SetActive (false);
            saving = true;
            //Invoke("SaveTexture",0.1f);
            StartCoroutine("SaveTexture");

        }
    }
    //To update at realtime the painting cursor on the mesh
    //void UpdateBrushCursor(){
    //	Vector3 uvWorldPosition=Vector3.zero;
    //	if (HitTestUVPosition (ref uvWorldPosition) && !saving) {
    //		brushCursor.SetActive(true);
    //		brushCursor.transform.position =uvWorldPosition+brushContainer.transform.position;									
    //	} else {
    //		brushCursor.SetActive(false);
    //	}		
    //}
    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        //Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        //Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        if (pen.GetComponent<isTrigger>().isTriggered && Physics.Raycast(pen.transform.GetChild(0).position, pen.transform.GetChild(0).forward, out hit, 10f, 3 << LayerMask.NameToLayer("DrawingGlass")))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }
    }


    //Sets the base material with a our canvas texture, then removes all our brushes
    void SaveTexture()
    {
        brushCounter = 0;
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex;  //Put the painted texture as the base

        foreach (Transform child in brushContainer.transform)
        {//Clear brushes
            Destroy(child.gameObject);
        }
        saving = false;
        hasChanges = false;
        //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
        //Invoke ("ShowCursor", 0.1f);
    }

    void EraseTexture()
    {
        Debug.Log("Entrei erase");
        if (hasChanges)
        {
            this.SaveTexture();
        }
        RaycastHit hit;
        if (pen.GetComponent<isTrigger>().isTriggered && Physics.Raycast(pen.transform.GetChild(0).position, pen.transform.GetChild(0).forward, out hit, 2, 3 << LayerMask.NameToLayer("DrawingGlass")))
        {
            Debug.Log("to apagano");
            RenderTexture.active = canvasTexture;
            Texture2D myTexture2D = new Texture2D(canvasTexture.width, canvasTexture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
            myTexture2D.Apply();
            Renderer renderer = hit.collider.GetComponent<MeshRenderer>();
            Vector2 pCoord = hit.textureCoord;
            pCoord.x *= myTexture2D.width;
            pCoord.y *= myTexture2D.height;
            MeshCollider meshCollider = hit.collider as MeshCollider;
            Vector2 tiling = renderer.material.mainTextureScale;
            Color[] testColor = new Color[50 * 300];
            for (int i = 0; i < testColor.Length; i++)
            {
                testColor[i] = Color.black;
                testColor[i].a = 0.1f;
            }
            //myTexture2D.SetPixel(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y), Color.clear);
            myTexture2D.SetPixels(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y), 50, 300, testColor);
            myTexture2D.Apply();
            baseMaterial.mainTexture = myTexture2D;
            erasing = false;
        }

        //Debug.Log(uvWorldPosition.x + " " + uvWorldPosition.y);
        //RenderTexture.active = canvasTexture;
        //Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.ARGB32, false);
        //tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        //Color[] testColor = new Color[10 * 10];
        //for (int i = 0; i < testColor.Length; i++) {
        //	testColor[i].r = 255f;
        //	testColor[i].b = 255f;
        //}
        //Debug.Log(tex.width + " " + tex.height);
        //var pixelsSelected = tex.GetPixels( (int) uvWorldPosition.x, (int) uvWorldPosition.y, 10, 10);
        //      for (int x = 0; x < tex.width; x++)
        //      {
        //          for (int y = 0; y < tex.height; y++)
        //          {
        //		Debug.Log(x + " " + y);
        //		if (x == uvWorldPosition.x && y == uvWorldPosition.y) {
        //			tex.SetPixel(x, y, Color.black);
        //		}

        //          }
        //      }
        //for (int i = 0; i < pixelsSelected.Length; i++)
        //{
        //	pixelsSelected[i].r = 100f;
        //	pixelsSelected[i].g = 100f;
        //	pixelsSelected[i].a = 1f;
        //}
        //tex.SetPixels((int) uvWorldPosition.x, (int) uvWorldPosition.y, 10, 10, pixelsSelected);
        //      tex.Apply();
        //RenderTexture.active = null;
        //baseMaterial.mainTexture = tex; //Put the painted texture as the base
        //foreach (Transform child in brushContainer.transform)
        //{//Clear brushes
        //	Destroy(child.gameObject);
        //}

    }
    //Show again the user cursor (To avoid saving it to the texture)
    void ShowCursor()
    {
        saving = false;
    }

    ////////////////// PUBLIC METHODS //////////////////

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

    ////////////////// OPTIONAL METHODS //////////////////

#if !UNITY_WEBPLAYER
    IEnumerator SaveTextureToFile(Texture2D savedTexture)
    {
        brushCounter = 0;
        string fullPath = System.IO.Directory.GetCurrentDirectory() + "\\UserCanvas\\";
        System.DateTime date = System.DateTime.Now;
        string fileName = "CanvasTexture.png";
        if (!System.IO.Directory.Exists(fullPath))
            System.IO.Directory.CreateDirectory(fullPath);
        var bytes = savedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath + fileName, bytes);
        Debug.Log("<color=orange>Saved Successfully!</color>" + fullPath + fileName);
        yield return null;
    }
#endif
}
