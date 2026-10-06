using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseBotSelector : MonoBehaviour
    {
        private const string PointActionName = "UI/Point";
        private const string PressActionName = "UI/Click";
        private const float PressedThreshold = 0.5f;

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private SelectionBoxView boxView;
        [SerializeField] private float pickRadius = 0.6f;
        [SerializeField, Range(0f, 0.1f)] private float tapTravelScreenFraction = 0.015f;

        private readonly TapDetector tapDetector = new TapDetector();
        private readonly List<Bot> pickedBots = new List<Bot>();
        private IReadOnlyList<Bot> bots;
        private BotSelection selection;
        private InputAction pointAction;
        private InputAction pressAction;
        private Vector2 pressWorldPoint;
        private Vector2 lastPointer;
        private bool wasPressed;
        private bool isTracking;
        private bool isMousePress;

        public void Connect(WarehouseSimulation simulation, BotSelection botSelection)
        {
            bots = simulation.Bots;
            selection = botSelection;
        }

        public void SelectAt(Vector2 worldPoint)
        {
            if (selection == null)
                return;

            if (BotPicker.TryPick(bots, worldPoint.x, worldPoint.y, pickRadius, out Bot picked))
                selection.Select(picked);
            else
                selection.Clear();
        }

        public void SelectInside(SelectionArea area)
        {
            if (selection == null)
                return;

            BotPicker.PickInside(bots, area, pickedBots);
            selection.SelectAll(pickedBots);
        }

        private void Awake()
        {
            pointAction = inputActions.FindAction(PointActionName, true);
            pressAction = inputActions.FindAction(PressActionName, true);
        }

        private void OnEnable()
        {
            pointAction.Enable();
            pressAction.Enable();
        }

        private void Update()
        {
            if (selection == null)
                return;

            Vector2 pointer = pointAction.ReadValue<Vector2>();
            bool isPressed = pressAction.ReadValue<float>() > PressedThreshold;
            if (isPressed && !wasPressed)
                BeginPress(pointer);
            else if (isPressed && isTracking)
                Drag(pointer);
            else if (!isPressed && wasPressed && isTracking)
                EndPress();

            wasPressed = isPressed;
            lastPointer = pointer;
        }

        private void BeginPress(Vector2 pointer)
        {
            isTracking = !eventSystem.IsPointerOverGameObject();
            isMousePress = pressAction.activeControl != null && pressAction.activeControl.device is Mouse;
            pressWorldPoint = viewCamera.ScreenToWorldPoint(pointer);
            tapDetector.Press(pointer);
        }

        private void Drag(Vector2 pointer)
        {
            tapDetector.Move(pointer);
            if (isMousePress && !IsTap())
                boxView.Draw(AreaTo(pointer));
        }

        private void EndPress()
        {
            isTracking = false;
            boxView.Hide();
            if (IsTap())
                SelectAt(viewCamera.ScreenToWorldPoint(lastPointer));
            else if (isMousePress)
                SelectInside(AreaTo(lastPointer));
        }

        private bool IsTap()
        {
            return tapDetector.IsTap(Screen.height * tapTravelScreenFraction);
        }

        private SelectionArea AreaTo(Vector2 pointer)
        {
            Vector2 worldPoint = viewCamera.ScreenToWorldPoint(pointer);
            return new SelectionArea(pressWorldPoint.x, pressWorldPoint.y, worldPoint.x, worldPoint.y);
        }
    }
}
