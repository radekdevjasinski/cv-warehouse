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
        private const string ScrollActionName = "UI/ScrollWheel";
        private const float PressedThreshold = 0.5f;

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private float startSize = 12f;
        [SerializeField] private float closestSize = 5f;
        [SerializeField] private float margin = 2f;
        [SerializeField] private float keyPanViewsPerSecond = 1.2f;
        [SerializeField] private float zoomStep = 0.12f;

        private Camera warehouseCamera;
        private InputAction moveAction;
        private InputAction pointAction;
        private InputAction pressAction;
        private InputAction scrollAction;
        private CameraFraming framing;
        private Vector2 lastPointer;
        private bool wasPressed;
        private bool isDragging;

        public void Frame(WarehouseLayout layout)
        {
            framing = new CameraFraming(new Vector2(layout.Grid.Width, layout.Grid.Height), closestSize, margin);
            framing.SetAspect(warehouseCamera.aspect);
            float truckCentreX = layout.TruckOrigin.X + layout.TruckSize.Width * 0.5f;
            framing.LookAt(new Vector2(truckCentreX, layout.Grid.Height), startSize);
            Apply();
        }

        private void Awake()
        {
            warehouseCamera = GetComponent<Camera>();
            moveAction = inputActions.FindAction(MoveActionName, true);
            pointAction = inputActions.FindAction(PointActionName, true);
            pressAction = inputActions.FindAction(PressActionName, true);
            scrollAction = inputActions.FindAction(ScrollActionName, true);
        }

        private void OnEnable()
        {
            moveAction.Enable();
            pointAction.Enable();
            pressAction.Enable();
            scrollAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            pointAction.Disable();
            pressAction.Disable();
            scrollAction.Disable();
        }

        private void Update()
        {
            if (framing == null)
                return;

            framing.SetAspect(warehouseCamera.aspect);
            PanWithKeys();
            PanWithDrag();
            ZoomWithScroll();
            Apply();
        }

        private void PanWithKeys()
        {
            Vector2 move = moveAction.ReadValue<Vector2>();
            if (move != Vector2.zero)
                framing.Pan(move * (framing.Size * 2f * keyPanViewsPerSecond * Time.unscaledDeltaTime));
        }

        private void PanWithDrag()
        {
            Vector2 pointer = pointAction.ReadValue<Vector2>();
            bool isPressed = pressAction.ReadValue<float>() > PressedThreshold;
            if (isPressed && !wasPressed)
                isDragging = !eventSystem.IsPointerOverGameObject();
            else if (isPressed && isDragging)
                framing.Pan((lastPointer - pointer) * (framing.Size * 2f / Screen.height));

            wasPressed = isPressed;
            lastPointer = pointer;
        }

        private void ZoomWithScroll()
        {
            float scroll = scrollAction.ReadValue<Vector2>().y;
            if (scroll != 0f && !eventSystem.IsPointerOverGameObject())
                framing.Zoom(1f - Mathf.Sign(scroll) * zoomStep);
        }

        private void Apply()
        {
            transform.position = new Vector3(framing.Centre.x, framing.Centre.y, transform.position.z);
            warehouseCamera.orthographicSize = framing.Size;
        }
    }
}
