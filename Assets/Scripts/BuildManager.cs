using System;
using UnityEngine;
using EventParameters;
using EventEnums;
using TileGridTypes;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour, PlayerControls.IBuildSystemActions
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject selectionTileBase;
    [SerializeField] private string buildLayerName;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private CameraController cameraController;

    private GameObject currentObject;
    private TileGrid currentGrid;

    private PlayerControls playerControls;
    private Action<EventParam> toggleBuildModeListener;
    private bool buildMode;
    private GameObject selectionTileInstance;
    private LayerMask targetLayer;
    private Vector3Int lastTile;

    private Vector2 mousePos;
    private bool buildKeyDown = false;
    private bool deleteKeyDown = false;

    private void Update()
    {
        if (buildMode)
        {
            Vector3Int? mouseTile = GetMouseTile();
            if (mouseTile.HasValue)
            {
                (TileGridType nextGridType, Vector2Int pos)? local = GlobalTileToLocalTile(mouseTile.Value);
                if (local.HasValue)
                {
                    //Debug.Log($"{local.Value.nextGridType} : {local.Value.pos}");
                    if (mouseTile.Value != lastTile)
                    {
                        lastTile = mouseTile.Value;
                        // create a graphic on the moused over tile. start by moving it to the cell pos, but slightly raised
                        selectionTileInstance.transform.position = mouseTile.Value;
                    }

                    // place  or delete selected item
                    // make sure we don't build while clicking the UI
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        //Vector2Int targetTile = new Vector2Int(mouseTile.Value.x, mouseTile.Value.z);
                        if (buildKeyDown && currentObject != null)
                        {
                            TileManager.AddToTile(local.Value.nextGridType, currentObject, local.Value.pos);
                        }
                        else if (deleteKeyDown)
                        {
                            TileManager.RemoveFromTile(local.Value.nextGridType, local.Value.pos);
                        }
                    }

                    if (!selectionTileInstance.activeSelf)
                    {
                        selectionTileInstance.SetActive(true);
                    }
                }
            }
            else if (selectionTileInstance.activeSelf)
            {
                selectionTileInstance.SetActive(false);
            }
        }
        else
        {
            //turn off the selection tile if there's no raycast hits
            if (selectionTileInstance.activeSelf)
            {
                selectionTileInstance.SetActive(false);
            }
        }
    }

    //gets the target tile the mouse is hovering
    private Vector3Int? GetMouseTile()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, 200f, targetLayer))
        {
            // check what cell the ray position is inside by casting to int(effectively truncate)
            return new Vector3Int(
                (int)(Mathf.Sign(hit.point.x) * Mathf.Round(Mathf.Abs(hit.point.x))),
                (int)(Mathf.Sign(hit.point.y) * Mathf.Round(Mathf.Abs(hit.point.y))),
                (int)(Mathf.Sign(hit.point.z) * Mathf.Round(Mathf.Abs(hit.point.z)))
            );

        }
        return null;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.BuildSystem.SetCallbacks(this);
    }
    private void Start()
    {
        // create selection tile instance and hide it
        selectionTileInstance = Instantiate(selectionTileBase, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        selectionTileInstance.transform.localScale = new Vector3(grid.cellSize.x/10, grid.cellSize.y/10, grid.cellSize.z/10);
        selectionTileInstance.SetActive(false);

        // get the target layer
        targetLayer = LayerMask.GetMask(buildLayerName);

        // set the last tile to something that won't be the next tile
        lastTile = new Vector3Int(999, 999, 999);

        //set the camera's starting target grid
        currentGrid = TileManager.instance.GetTileGrid(TileGridType.farmTileGrid);
        SwitchCameraGrid(TileGridType.farmTileGrid);
    }

    // turns a global tile position into a local tile on one of the two grids
    private (TileGridType, Vector2Int)? GlobalTileToLocalTile(Vector3Int pos)
    {
        Vector2Int newPos = new Vector2Int(pos.x, pos.z);
        TileGrid farmTileGrid = TileManager.instance.GetTileGrid(TileGridType.farmTileGrid);
        TileGrid processingTileGrid = TileManager.instance.GetTileGrid(TileGridType.processingTileGrid);

        if (withinLocalGrid(farmTileGrid, newPos) )
        {
            return (TileGridType.farmTileGrid, newPos - farmTileGrid.offset);
        } 
        else if (withinLocalGrid(processingTileGrid, newPos) )
        {
            return (TileGridType.processingTileGrid, newPos - processingTileGrid.offset);
        }
        else
        {
            return null;
        }
    }

    private bool withinLocalGrid(TileGrid nextGrid, Vector2Int pos)
    {
        if (pos.x >= nextGrid.offset.x && pos.x <= nextGrid.size.x + nextGrid.offset.x)
        {
            if (pos.y >= nextGrid.offset.y && pos.y <= nextGrid.size.y + nextGrid.offset.y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        toggleBuildModeListener = new Action<EventParam>(ToggleBuildMode);
        EventManager.StartListening(GameEventType.ToggleBuildMode, toggleBuildModeListener);
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnDestroy()
    {
        EventManager.StopListening(GameEventType.ToggleBuildMode, toggleBuildModeListener);
    }

    private void ToggleBuildMode(EventParam param)
    {
        buildMode = param.state;
    }

    public void SetCurrentObject(GameObject newCurrentObject)
    {
        currentObject = newCurrentObject;
        //Debug.Log(currentObject);
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (context.started) { buildKeyDown = true; }
        else if (context.canceled) { buildKeyDown = false; }
    }

    public void OnDelete(InputAction.CallbackContext context)
    {
        if (context.started) { deleteKeyDown = true; }
        else if (context.canceled) { deleteKeyDown = false; }
    }

    public void OnHotbar(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //get the number key pressed
            int numKey = int.Parse(context.control.name);
            hotbarManager.OnHotbar(numKey);
        }
    }

    public void OnSwitchGrid(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //get what the next grid is going to be
            TileGridType nextGridType = (currentGrid.tileGridType == TileGridType.farmTileGrid) ? TileGridType.processingTileGrid : TileGridType.farmTileGrid;
            SwitchCameraGrid(nextGridType);
        }
    }

    public void SwitchCameraGrid(TileGridType nextGridType)
    {
        TileGrid nextGrid = TileManager.instance.GetTileGrid(nextGridType);
        
        // get the new range of motion for the camera
        Vector2 min = new Vector2(nextGrid.offset.x - 0.5f, nextGrid.offset.y - 0.5f);
        Vector2 max = new Vector2(nextGrid.offset.x - 0.5f + nextGrid.size.x, nextGrid.offset.y - 0.5f + nextGrid.size.y);
        // finally get the relative camera position from the old nextGrid, and find the new position
        Vector3 relativePos = cameraController.GetTargetPos() - new Vector3(currentGrid.offset.x, 0f, currentGrid.offset.y);
        Vector3 newPos = new Vector3(nextGrid.offset.x, 0f, nextGrid.offset.y) + relativePos;

        cameraController.setMovementBounds(min, max);
        cameraController.SetTargetPos(newPos);

        currentGrid = nextGrid;
    }
}
