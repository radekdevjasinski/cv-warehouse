using System.Collections.Generic;
using CvWarehouse.Core.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class TruckView : MonoBehaviour
    {
        private const int BodyLayer = 0;
        private const int DecalLayer = 1;
        private const int DoorOrder = 1;
        private const float WheelCentreAboveBottom = 5f / DepthSorting.PixelsPerUnit;
        private const float LogoCentreAboveBottom = 22f / DepthSorting.PixelsPerUnit;

        [SerializeField] private TruckSkin skin;

        private readonly List<SpriteRenderer> parts = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> wheels = new List<SpriteRenderer>();

        public IReadOnlyList<SpriteRenderer> Parts => parts;

        public IReadOnlyList<SpriteRenderer> Wheels => wheels;

        public SpriteRenderer Door { get; private set; }

        public SpriteRenderer Logo { get; private set; }

        public void Show(WarehouseLayout layout, Material material)
        {
            var plan = new TruckPlan(layout.TruckSize.Width);
            ShowDoor(layout, material);
            ShowParts(plan, layout.TruckOrigin, material);
            ShowWheels(plan, layout.TruckOrigin, material);
            if (plan.HasLogo)
                Logo = CreateDecal("Truck Logo", skin.Logo, material, layout.TruckOrigin, plan.LogoCentre, LogoCentreAboveBottom);
        }

        private void ShowParts(TruckPlan plan, GridPosition origin, Material material)
        {
            for (int column = 0; column < plan.Parts.Count; column++)
            {
                TruckPart part = plan.Parts[column];
                SpriteRenderer partRenderer = CreateRenderer("Truck " + part, skin.SpriteOf(part), material, DepthSorting.OrderAt(origin.Y, BodyLayer));
                partRenderer.transform.localPosition = new Vector3(origin.X + column + 0.5f, origin.Y);
                parts.Add(partRenderer);
            }
        }

        private void ShowWheels(TruckPlan plan, GridPosition origin, Material material)
        {
            foreach (float centre in plan.WheelCentres)
                wheels.Add(CreateDecal("Truck Wheel", skin.Wheel, material, origin, centre, WheelCentreAboveBottom));
        }

        private void ShowDoor(WarehouseLayout layout, Material material)
        {
            Door = CreateRenderer("Loading Door", skin.LoadingDoor, material, DoorOrder);
            Door.drawMode = SpriteDrawMode.Tiled;
            Door.size = new Vector2(layout.TruckSize.Width + skin.DoorExtraColumns, skin.LoadingDoor.bounds.size.y);
            Door.transform.localPosition = new Vector3(layout.TruckOrigin.X + layout.TruckSize.Width * 0.5f, layout.TruckOrigin.Y);
        }

        private SpriteRenderer CreateDecal(string name, Sprite sprite, Material material, GridPosition origin, float centreX, float centreAboveBottom)
        {
            SpriteRenderer decal = CreateRenderer(name, sprite, material, DepthSorting.OrderAt(origin.Y, DecalLayer));
            decal.transform.localPosition = new Vector3(origin.X + centreX, origin.Y + centreAboveBottom);
            return decal;
        }

        private SpriteRenderer CreateRenderer(string name, Sprite sprite, Material material, int sortingOrder)
        {
            var partObject = new GameObject(name);
            partObject.transform.SetParent(transform, false);
            SpriteRenderer spriteRenderer = partObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sharedMaterial = material;
            spriteRenderer.sortingOrder = sortingOrder;
            return spriteRenderer;
        }
    }
}
