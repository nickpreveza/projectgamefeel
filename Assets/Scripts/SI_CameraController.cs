using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
public class SI_CameraController : MonoBehaviour
{
    public static SI_CameraController Instance;
    public bool gameReady;
    public bool controlsLocked;

    [SerializeField] Camera mainCamera;

    [SerializeField] CinemachineVirtualCamera worldViewVirtualCamera;
    [SerializeField] CinemachineVirtualCamera cityViewTransitionVirtualCamera;
    [SerializeField] CinemachineVirtualCamera cityViewVirtualCamera;

    [SerializeField] bool outOfBounds;

    public WorldTile selectedTile;

    [SerializeField] float internalTouchTimer;
    [SerializeField] float timeToRegisterTap;
    [SerializeField] float timeToRegisterHold;

    bool tapValid;

    [SerializeField] float internalMapHeight;
    [SerializeField] float internalMapWidth;

    Vector3 oldPosition;


    [SerializeField] float moveSpeed;
    Vector3 lastMousePosition;

    Vector3 diff;

    public bool keyboardControls;
    public bool touchControls;
    public bool zoomEnabled;
    public bool dragEnabled;

    Vector3 cameraTargetOffset;

    [SerializeField] LayerMask interactableMask;

    delegate void UpdateFunction();
    UpdateFunction Update_CurrentFunction;// = Update_DetectMode;

    //Drag
    int mouseDragThreshold = 1;
    Vector3 dragOrigin;

    //Zoom
    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;

    public float mapMinX, mapMaxX, mapMinY, mapMaxY;

    //camera clamp
    float camHeight, camWidth;
    float minX, maxX, minY, maxY;
    float newX, newY;
    Vector3 clampedPosition;

    [SerializeField]
    float transitionZoomOffsetX, transitionZoomOffsetY;

    //hex panning
    [SerializeField] Vector3 cameraOffsetFromPanTarget;
    Vector3 prevCameraPosition;
    Vector3 targetCameraPosition;
    WorldTile tile;
    bool autoMove;
    Vector3 currentVelocity;
    float smoothTime = 0.5f;

    public bool repeatSelection;

    Vector3 dragTemp;
    Vector3 panTemp;
    Vector3 dir;
    Vector3 lastCameraPosition;

    public LayerMask menuLayerMask;
    public LayerMask gameLayerMask;
    public LayerMask cityLayerMask;
    public bool animationsRunning;

    bool IsMouseOverGameWindow { get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } }

    public float cameraToCityViewDelay = 1f;
    public float cameraReturnDelay = 0.1f;

    public bool tileSelectMode;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        oldPosition = this.transform.position;
       
    }

    public void GameStarted()
    {
        mainCamera.cullingMask = gameLayerMask;
        worldViewVirtualCamera.gameObject.SetActive(true);
        cityViewTransitionVirtualCamera.gameObject.SetActive(false);
        cityViewVirtualCamera.gameObject.SetActive(false);
        Update_CurrentFunction = Update_DetectModeStart;
    }

    public void GameEnded()
    {
//mainCamera.cullingMask = menuLayerMask;
        //cityCamera.gameObject.SetActive(false);
    }

    public void CenterCamera(WorldTile tile)
    {
        Vector3 prevPos = worldViewVirtualCamera.gameObject.transform.position;
        prevPos.x = tile.posX;
        prevPos.y = tile.posY;

        worldViewVirtualCamera.gameObject.transform.position = prevPos;
    }

    public void ShowCity(WorldCity city)
    {
        controlsLocked = true;
        Vector3 prevPos = cityViewTransitionVirtualCamera.gameObject.transform.position;
        prevPos.x = city.parentTile.posX + transitionZoomOffsetX;
        prevPos.y = city.parentTile.posY + transitionZoomOffsetY;
        cityViewTransitionVirtualCamera.gameObject.transform.position = prevPos;
        worldViewVirtualCamera.gameObject.SetActive(false);
        cityViewVirtualCamera.gameObject.SetActive(false);

        cityViewTransitionVirtualCamera.gameObject.SetActive(true);
        Invoke("EnableRealCityCamera", cameraToCityViewDelay);
    }

    void EnableRealCityCamera()
    {
        cityViewTransitionVirtualCamera.gameObject.SetActive(false);
        cityViewVirtualCamera.gameObject.SetActive(true);
        mainCamera.cullingMask = cityLayerMask;
    }

    public void HideCity()
    {
        cityViewVirtualCamera.gameObject.SetActive(false);
        cityViewTransitionVirtualCamera.gameObject.SetActive(true);
       
        mainCamera.cullingMask = gameLayerMask;
        Invoke("EnableWorldCamera", cameraReturnDelay);
    }

    void EnableWorldCamera()
    {
        controlsLocked = false;
        cityViewTransitionVirtualCamera.gameObject.SetActive(false);
        cityViewVirtualCamera.gameObject.SetActive(false);

        worldViewVirtualCamera.gameObject.SetActive(true);
        
    }
  
    private void Update()
    {
        if (!gameReady)
        {
            return;
        }

        if (controlsLocked)
        {
            animationsRunning = true;
        }
        else
        {
            animationsRunning = false;
        }

        if (controlsLocked)
        {
            return;
        }

        if (autoMove)
        {
            AutoHexPan();
            CheckifCameraMoved();
            return;
        }

        if (outOfBounds)
        {
          // MoveBack();
           // return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || !Application.isFocused)
        {
            CancelUpdateFunction();
            return;
        }

        Update_CurrentFunction();

        if (!IsPointerOverUIObject() )
        {
            

            if (zoomEnabled)
            {
                Update_ScrollZoom();
            }

        }

        if (keyboardControls && !animationsRunning)
        {
            KeyboardInput();
        }

        if (touchControls && !animationsRunning)
        {
           // TouchInput();
        }

        lastMousePosition = Input.mousePosition;
        //lastClickedMousePosition = Input.mousePosition;
        CheckifCameraMoved();
    }

  
    void CancelUpdateFunction()
    {
        Update_CurrentFunction = Update_DetectModeStart;
        //clean up any UI associated with the mode
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

    }

    void Update_DetectModeStart()
    {
        if (animationsRunning)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
           // Update_CurrentFunction;
            //left mouse button just went down

        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Vector3.Distance(Input.mousePosition, lastMousePosition) > 1)
            {
                Debug.Log("Canceled late click by dragging");
                return;
            }

            Update_Tap(); 
        }
        else if (Input.GetMouseButton(0) && (Vector3.Distance(Input.mousePosition, lastMousePosition) > mouseDragThreshold))
        {
            Update_CurrentFunction = Update_CameraDrag;

            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);  
            Update_CurrentFunction();
        }
        else if (Input.GetMouseButton(1))
        {

        }
    }

    public void PanToTile(WorldTile tile)
    {
        //autoPanHexIdentifier = tile.hexIdentifier;
        //prevCameraPosition = this.transform.position;
        //targetCameraPosition = hex.hexData.PositionFromCamera() + cameraOffsetFromPanTarget * (this.transform.position.y / 60);
        //targetCameraPosition.y = this.transform.position.y;

        //autoMove = true;
    }

    void AutoHexPan()
    {

        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetCameraPosition, ref currentVelocity, smoothTime);

        if (Vector3.Distance(this.transform.position, targetCameraPosition) < 0.1)
        {
            autoMove = false;
            currentVelocity = Vector3.zero;
            //SI_EventManager.Instance.OnAutoPanCompleted(autoPanHexIdentifier);
        }

       
    }


    void KeyboardInput()
    {
        Vector3 translate = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        this.transform.Translate(translate * moveSpeed * Time.deltaTime, Space.World);

        ClampCamera();
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        camHeight = mainCamera.orthographicSize;
        camWidth = mainCamera.orthographicSize + mainCamera.aspect;

        minX = mapMinX + camWidth;
        maxX = mapMaxX - camWidth;

        minY = mapMinY + camHeight;
        maxY = mapMaxY - camHeight;

        newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        clampedPosition.x = newX;
        clampedPosition.y = newY;
        clampedPosition.z = targetPosition.z;

        return clampedPosition;
    }

    void ClampCamera()
    {
        //return;
        this.transform.position = ClampCamera(this.transform.position);
    }

    void Update_Tap()
    {
        if (IsPointerOverUIObject())
        {
            Debug.Log("Mouse is over GUI");
            return;
        }

        if(animationsRunning)
        {
            return;
        }

        WorldTile tile = MapGenerator.Instance.GetTile(GetMouseWorldPosition());

        if (tile != null)
        {
            Debug.Log("Mouse is over Tile: " + tile.posX + "," + tile.posY);
            //UIManager.Instance.HideResearchPanel();
            //UIManager.Instance.HideOverviewPanel();
            //UIManager.Instance.HideSettingsPanel();
            SelectTile(tile);
        }
        else
        {
            Debug.Log("Mouse did not hit");
            selectedTile = null;
        }
        /*
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.transform.position, GetMouseWorldPosition(), interactableMask);

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Tile"))
            {
                
                WorldTile newTile = hit.transform.parent.GetComponent<WorldTile>();
                Debug.Log("Mouse is over Tile: " + newTile.posX + "," + newTile.posY);
                //UIManager.Instance.HideResearchPanel();
                //UIManager.Instance.HideOverviewPanel();
                //UIManager.Instance.HideSettingsPanel();
                SelectTile(newTile);
                //PanToHex(newTile);
            }
            else
            {
                Debug.Log("Mouse is over " + hit.transform.tag);
                selectedTile = null;
            }
        }
        else
        {
            Debug.Log("Mouse did not hit");
            selectedTile = null;
        }*/
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    void Update_CameraDrag()
    {
        if (Input.GetMouseButtonUp(0) ||  IsPointerOverUIObject())
        {
            CancelUpdateFunction();
            return;
        }

        diff = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);

        this.transform.Translate(diff, Space.World);

        dragTemp = this.transform.position;

        ClampCamera();
    }

 
    void Update_ScrollZoom()
    {
        if (!IsMouseOverGameWindow)
        {
            return;
        }
       
        float axisValue = Input.GetAxis("Mouse ScrollWheel");
        if (axisValue != 0)
        {
            float scrollAmount = -axisValue * zoomStep;
            float newSize = mainCamera.orthographicSize + scrollAmount;

            worldViewVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
            //mainCamera.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
            //cityCamera.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

            ClampCamera();
        }
       

    }

    void CheckifCameraMoved()
    {
        if (oldPosition != this.transform.position)
        {
            //Something moved the camera
            oldPosition = this.transform.position;

            SI_EventManager.Instance.OnCameraMoved();
        }
    }

    public void DeselectSelection()
    {
        if (selectedTile != null)
        {
            selectedTile.Deselect();
        }

        selectedTile = null;
    }

    public void SelectTile(WorldTile newTile) //this shouldnt be public. Used for cityView workaround
    {
        if (selectedTile != null)
        {
            selectedTile.Deselect();
        }

        if (newTile == selectedTile)
        {
            if (repeatSelection)
            {
                repeatSelection = false;
                selectedTile.Deselect();
                selectedTile = null;
                return;
            }
            repeatSelection = true;
            selectedTile.Select(repeatSelection);
            internalTouchTimer = 0;
            tapValid = false;

        }
        else
        {
            repeatSelection = false;
            selectedTile = newTile;
            selectedTile.Select(repeatSelection);
            internalTouchTimer = 0;
            tapValid = false;
        }
    }
}
