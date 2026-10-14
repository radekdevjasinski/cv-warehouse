using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CvWarehouse.Presentation.Warehouse
{
    [RequireComponent(typeof(Camera))]
    public sealed class WarehouseCamera : MonoBehaviour
    {
        private const string MoveActionName = "Player/Move";
        private const string PointActionName = "UI/Point";
        private const string PressActionName = "UI/Click";
        private const string RightPressActionName = "UI/RightClick";
        private const string MiddlePressActionName = "UI/MiddleClick";
        private const string ScrollActionName = "UI/ScrollWheel";
        private const float PressedThreshold = 0.5f;

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private int spritePixelsPerUnit = 16;
        [SerializeField] private float closestViewCells = 6f;
        [SerializeField] private float farthestViewCells = 24f;
        [SerializeField] private float zoomCooldownSeconds = 0.15f;
        [SerializeField] private float keyPanViewsPerSecond = 1.2f;

        private Camera warehouseCamera;
        private InputAction moveAction;
        private InputAction pointAction;
        private InputAction pressAction;
        private InputAction rightPressAction;
        private InputAction middlePressAction;
        private InputAction scrollAction;
        private CameraFraming framing;
        private Vector2 lastPointer;
        private bool wasPressed;
        private bool isDragging;
        private bool isPointerAMouse;
        private float nextZoomTime;

        public void Frame(WarehouseLayout layout)
        {
            var limits = new CameraZoomLimits(spritePixelsPerUnit, closestViewCells, farthestViewCells);
            framing = new CameraFraming(new Vector2(layout.Grid.Width, layout.Grid.Height), limits);
            framing.SetViewPixels(ViewPixels);
            framing.ZoomOutFully();
            float truckCentreX = layout.TruckOrigin.X + layout.TruckSize.Width * 0.5f;
            framing.LookAt(new Vector2(truckCentreX, layout.Grid.Height));
            Apply();
        }

        private void Awake()
        {
            warehouseCamera = GetComponent<Camera>();
            moveAction = inputActions.FindAction(MoveActionName, true);
            pointAction = inputActions.FindAction(PointActionName, true);
            pressAction = inputActions.FindAction(PressActionName, true);
            rightPressAction = inputActions.FindAction(RightPressActionName, true);
            middlePressAction = inputActions.FindAction(MiddlePressActionName, true);
            scrollAction = inputActions.FindAction(ScrollActionName, true);
        }

        private void OnEnable()
        {
            moveAction.Enable();
            pointAction.Enable();
            pressAction.Enable();
            rightPressAction.Enable();
            middlePressAction.Enable();
            scrollAction.Enable();
            pointAction.performed += RememberPointerDevice;
        }

        private void OnDisable()
        {
            pointAction.performed -= RememberPointerDevice;
            moveAction.Disable();
            pointAction.Disable();
            pressAction.Disable();
            rightPressAction.Disable();
            middlePressAction.Disable();
            scrollAction.Disable();
        }

        private void Update()
        {
            if (framing == null)
                return;

            framing.SetViewPixels(ViewPixels);
            PanWithKeys();
            PanWithDrag();
            ZoomWithScroll();
            Apply();
        }

        private Vector2Int ViewPixels => new Vector2Int(warehouseCamera.pixelWidth, warehouseCamera.pixelHeight);

        private void PanWithKeys()
        {
            Vector2 move = moveAction.ReadValue<Vector2>();
            if (move != Vector2.zero)
                framing.Pan(move * (framing.Size * 2f * keyPanViewsPerSecond * Time.unscaledDeltaTime));
        }

        private static bool IsPressed(InputAction action)
        {
            return action.ReadValue<float>() > PressedThreshold;
        }

        private void PanWithDrag()
        {
            Vector2 pointer = pointAction.ReadValue<Vector2>();
            bool isPressed = IsPanPressed();
            if (isPressed && !wasPressed)
                isDragging = !eventSystem.IsPointerOverGameObject();
            else if (isPressed && isDragging)
                framing.Pan((lastPointer - pointer) * framing.UnitsPerScreenPixel);

            wasPressed = isPressed;
            lastPointer = pointer;
        }

        private bool IsPanPressed()
        {
            bool isTouchPressed = !isPointerAMouse && IsPressed(pressAction);
            return isTouchPressed || IsPressed(rightPressAction) || IsPressed(middlePressAction);
        }

        private void RememberPointerDevice(InputAction.CallbackContext context)
        {
            isPointerAMouse = context.control.device is Mouse;
        }

        private void ZoomWithScroll()
        {
            float scroll = scrollAction.ReadValue<Vector2>().y;
            if (scroll == 0f || Time.unscaledTime < nextZoomTime || eventSystem.IsPointerOverGameObject())
                return;

            nextZoomTime = Time.unscaledTime + zoomCooldownSeconds;
            if (scroll > 0f)
                framing.ZoomIn();
            else
                framing.ZoomOut();
        }

        private void Apply()
        {
            transform.position = new Vector3(framing.Centre.x, framing.Centre.y, transform.position.z);
            warehouseCamera.orthographicSize = framing.Size;
        }
    }
}
