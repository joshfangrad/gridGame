using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, PlayerControls.ICameraActions
{
    [SerializeField] private float startingRotation = 45, startingAngle = 45, startingZoom = 15f;
    [SerializeField] private float minHeight = 5f, maxHeight = 89f;
    [SerializeField] private float rotateDampening = 10f, rotateSpeed = 4f;
    [SerializeField] private float minZoom = 2, maxZoom = 20;
    [SerializeField] private float zoomDampening = 10f, zoomSpeed = 10f;
    [SerializeField] private float panDampening = 10f, panSpeed = 0.03f, panSpeedMultAtFullZoom = 0.2f;

    private Vector2 movementLimitMin = new Vector2(-999f, 999f);
    private Vector2 movementLimitMax = new Vector2(-999f, 999f);

    private PlayerControls playerControls;

    private bool lookKeyHeld = false;
    private Vector3 panVelocity;
    private Vector2 moveInput;
    private Vector3 targetPos;
    private float targetCameraDistance;
    private Vector3 targetRotation;

    private new Camera camera;
    private float cameraDistance;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Camera.SetCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (lookKeyHeld)
        {
            Vector2 look = context.ReadValue<Vector2>();
            targetRotation.x += look.x * rotateSpeed * Time.deltaTime;
            targetRotation.y += -look.y * rotateSpeed * Time.deltaTime;
            //clamp y axis
            if (targetRotation.y < minHeight)
            {
                targetRotation.y = minHeight;
            }
            else if (targetRotation.y > maxHeight)
            {
                targetRotation.y = maxHeight;
            }
        }
    }

    public void OnLookKey(InputAction.CallbackContext context)
    {
        if (context.started) { lookKeyHeld = true; }
        else if (context.canceled) { lookKeyHeld = false; }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        
        Vector2 zoom = context.ReadValue<Vector2>();
        targetCameraDistance += -zoom.y * zoomSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    private void Start()
    {
        camera = this.GetComponentInChildren<Camera>();

        targetCameraDistance = startingZoom;
        targetRotation.x = startingRotation;
        targetRotation.y = startingAngle;
    }

    void Update()
    {
        //first we get a value scale between panSpeedMultAtFullZoom and 1, so we can scale how fast we pan based on zoom level 
        float panSpeedScale = (targetCameraDistance - minZoom) / (maxZoom - minZoom) * (1 - panSpeedMultAtFullZoom) + panSpeedMultAtFullZoom;

        //we also rotate the input relative to the current camera angle, so we always are panning relative to where we're looking
        float rotationAngle = transform.rotation.eulerAngles.y;
        Quaternion panAngleRotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
        Vector3 addedMovement = new Vector3(
            moveInput.x * Time.deltaTime * panSpeed * panSpeedScale,
            0f,
            moveInput.y * Time.deltaTime * panSpeed * panSpeedScale
        );
        panVelocity = panAngleRotation * addedMovement;

        
        targetPos += panVelocity;
        targetPos.x = Mathf.Clamp(targetPos.x, movementLimitMin.x, movementLimitMax.x);
        targetPos.z = Mathf.Clamp(targetPos.z, movementLimitMin.y, movementLimitMax.y);

        //apply panning movement
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * panDampening);

        //dampen the zoom and clamp the camera value
        cameraDistance = Mathf.Lerp(cameraDistance, targetCameraDistance, Time.deltaTime * zoomDampening);
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
        targetCameraDistance = Mathf.Clamp(targetCameraDistance, minZoom, maxZoom);

        //convert rotation to a quaternion and lerp it
        Quaternion rotationQuaternion = Quaternion.Euler(targetRotation.y, targetRotation.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationQuaternion, Time.deltaTime * rotateDampening);

        //set the camera's position and point it at the origin point
        camera.transform.position = transform.position + (transform.forward * -cameraDistance);
        camera.transform.rotation = transform.rotation;
    }

    public void setMovementBounds(Vector2 min, Vector2 max)
    {
        movementLimitMin = min;
        movementLimitMax = max;
    }
    
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }
    
    public void SetTargetPos(Vector3 newPos)
    {
        targetPos = newPos;
    }
}
