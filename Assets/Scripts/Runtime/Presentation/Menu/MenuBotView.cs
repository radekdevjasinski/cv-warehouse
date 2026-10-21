using CvWarehouse.Core.Menu;
using CvWarehouse.Presentation.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Menu
{
    public sealed class MenuBotView : MonoBehaviour
    {
        private const int BodyLayer = 0;
        private const int FaceLayer = 1;
        private const int CargoLayer = 2;
        private const int CrewSlot = 0;
        private const float FeetBelowCentre = 6f / DepthSorting.PixelsPerUnit;
        private const float CargoLeft = -4f / DepthSorting.PixelsPerUnit;
        private const float HeadTop = 5f / DepthSorting.PixelsPerUnit;
        private const float StepBob = 1f / DepthSorting.PixelsPerUnit;

        [SerializeField] private MenuBackdropConfig config;
        [SerializeField] private Transform sourceStand;
        [SerializeField] private Transform targetStand;
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpriteRenderer face;
        [SerializeField] private SpriteRenderer cargo;

        private float elapsedSeconds;

        public MenuBotLoop Loop { get; private set; }

        private void Awake()
        {
            Loop = new MenuBotLoop(config.CreateLoopSettings());
            body.color = config.BotColor;
            Show();
        }

        private void Update()
        {
            elapsedSeconds += Time.deltaTime;
            Loop.Advance(Time.deltaTime);
            Show();
        }

        private void Show()
        {
            BotPose pose = CurrentPose();
            Vector3 centre = Vector3.Lerp(sourceStand.localPosition, targetStand.localPosition, Loop.Position);
            body.transform.localPosition = SnapToArtPixel(centre);
            body.sprite = config.Skin.Body(pose);
            face.sprite = config.Skin.Face(pose);
            ShowCargo(pose);
            Sort(body.transform.localPosition.y - FeetBelowCentre);
        }

        private BotPose CurrentPose()
        {
            int tickCount = (int)(elapsedSeconds * config.AnimationTicksPerSecond);
            bool isStepping = config.BotAnimation.IsStepping(Loop.IsMoving, tickCount);
            return new BotPose(isStepping, config.BotAnimation.IsBlinking(CrewSlot, elapsedSeconds));
        }

        private void ShowCargo(BotPose pose)
        {
            cargo.enabled = Loop.CargoWords > 0;
            if (!cargo.enabled)
                return;

            cargo.sprite = config.Skin.Cargo(Loop.CargoWords);
            Vector3 halfSize = cargo.sprite.bounds.extents;
            float headTop = pose.IsStepping ? HeadTop + StepBob : HeadTop;
            cargo.transform.localPosition = new Vector3(CargoLeft + halfSize.x, headTop + halfSize.y);
        }

        private void Sort(float feetY)
        {
            body.sortingOrder = DepthSorting.OrderAt(feetY, BodyLayer);
            face.sortingOrder = DepthSorting.OrderAt(feetY, FaceLayer);
            cargo.sortingOrder = DepthSorting.OrderAt(feetY, CargoLayer);
        }

        private static Vector3 SnapToArtPixel(Vector3 position)
        {
            return new Vector3(
                Mathf.Round(position.x * DepthSorting.PixelsPerUnit) / DepthSorting.PixelsPerUnit,
                Mathf.Round(position.y * DepthSorting.PixelsPerUnit) / DepthSorting.PixelsPerUnit,
                position.z);
        }
    }
}
