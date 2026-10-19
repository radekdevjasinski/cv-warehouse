using CvWarehouse.Core.Warehouse;
using UnityEngine;

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

        private readonly BotSkin skin;
        private readonly GameObject root;
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
            progressBar = new BotProgressBar(body.transform, skin, shapes);
            root = body.gameObject;
            root.SetActive(false);
        }

        public bool IsVisible => root.activeSelf;

        public Color BodyColor => body.color;

        public Sprite BodySprite => body.sprite;

        public Sprite FaceSprite => face.sprite;

        public bool IsCarrying => cargo.enabled;

        public Sprite CargoSprite => cargo.sprite;

        public int SortingOrder => body.sortingOrder;

        public BotProgressBar ProgressBar => progressBar;

        public float Scale => root.transform.localScale.x;

        public void Hide()
        {
            if (root.activeSelf)
                root.SetActive(false);
        }

        public void Show(Bot bot, BotPose pose, Color color)
        {
            if (!root.activeSelf)
                root.SetActive(true);

            root.transform.localPosition = new Vector3(bot.CentreX, bot.CentreY);
            body.sprite = skin.Body(pose);
            body.color = color;
            face.sprite = skin.Face(pose);
            ShowCargo(bot, pose);
            progressBar.Show(bot.ExtractProgress);
            Sort(bot.CentreY - FeetBelowCentre);
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

        private void Sort(float feetY)
        {
            body.sortingOrder = DepthSorting.OrderAt(feetY, BodyLayer);
            face.sortingOrder = DepthSorting.OrderAt(feetY, FaceLayer);
            cargo.sortingOrder = DepthSorting.OrderAt(feetY, CargoLayer);
        }
    }
}
