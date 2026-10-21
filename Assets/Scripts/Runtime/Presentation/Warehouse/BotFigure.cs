using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Rendering;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotFigure
    {
        private const int BodyLayer = 0;
        private const int FaceLayer = 1;
        private const int CargoLayer = 2;
        private const float FeetBelowCentre = 6f / DepthSorting.PixelsPerUnit;
        private const float CargoLeft = -4f / DepthSorting.PixelsPerUnit;
        private const float HeadTop = 5f / DepthSorting.PixelsPerUnit;
        private const float StepBob = 1f / DepthSorting.PixelsPerUnit;
        private const float DepthTieBreakStep = 0.01f;

        private readonly BotSkin skin;
        private readonly GameObject root;
        private readonly SortingGroup sortingGroup;
        private readonly SpriteRenderer body;
        private readonly SpriteRenderer face;
        private readonly SpriteRenderer cargo;
        private readonly BotProgressBar progressBar;

        public BotFigure(string name, Transform parent, BotSkin skin, SquareShapeFactory shapes)
        {
            this.skin = skin;
            body = shapes.CreateSquare(name, parent, BodyLayer);
            face = shapes.CreateSquare("Face", body.transform, FaceLayer);
            cargo = shapes.CreateSquare("Cargo", body.transform, CargoLayer);
            progressBar = new BotProgressBar(parent, skin, shapes);
            root = body.gameObject;
            sortingGroup = root.AddComponent<SortingGroup>();
            root.SetActive(false);
        }

        public bool IsVisible => root.activeSelf;

        public Color BodyColor => body.color;

        public Sprite BodySprite => body.sprite;

        public Sprite FaceSprite => face.sprite;

        public bool IsCarrying => cargo.enabled;

        public Sprite CargoSprite => cargo.sprite;

        public int SortingOrder => sortingGroup.sortingOrder;

        public BotProgressBar ProgressBar => progressBar;

        public float Scale => root.transform.localScale.x;

        public void Hide()
        {
            if (root.activeSelf)
                root.SetActive(false);
            progressBar.Show(0f);
        }

        public void Show(Bot bot, BotPose pose, Color color)
        {
            if (!root.activeSelf)
                root.SetActive(true);

            var centre = new Vector3(bot.CentreX, bot.CentreY);
            root.transform.localPosition = new Vector3(centre.x, centre.y, -(bot.CrewSlot + 1) * DepthTieBreakStep);
            body.sprite = skin.Body(pose);
            body.color = color;
            face.sprite = skin.Face(pose);
            ShowCargo(bot, pose);
            progressBar.MoveTo(centre);
            progressBar.Show(bot.ExtractProgress);
            sortingGroup.sortingOrder = DepthSorting.OrderAt(bot.CentreY - FeetBelowCentre, BodyLayer);
        }

        public void SetScale(float scale)
        {
            root.transform.localScale = new Vector3(scale, scale, 1f);
        }

        private void ShowCargo(Bot bot, BotPose pose)
        {
            cargo.enabled = bot.CargoPieces > 0;
            if (!cargo.enabled)
                return;

            cargo.sprite = skin.Cargo(bot.CargoWords);
            Vector3 halfSize = cargo.sprite.bounds.extents;
            float headTop = pose.IsStepping ? HeadTop + StepBob : HeadTop;
            cargo.transform.localPosition = new Vector3(CargoLeft + halfSize.x, headTop + halfSize.y);
        }
    }
}
