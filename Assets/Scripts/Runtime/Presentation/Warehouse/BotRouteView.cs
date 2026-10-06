using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Rendering;

namespace CvWarehouse.Presentation.Warehouse
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class BotRouteView : MonoBehaviour
    {
        private const int RouteSortingOrder = 1;
        private const int RoundingVertices = 6;
        private const int FewestLinePoints = 2;
        private const float CellCentre = 0.5f;

        [SerializeField] private Color routeColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        [SerializeField] private float routeWidth = 0.18f;

        private LineRenderer line;
        private BotSelection selection;

        public int VisiblePointCount => line.positionCount;

        public void Show(BotSelection botSelection, Material material)
        {
            selection = botSelection;
            line.sharedMaterial = material;
            line.useWorldSpace = false;
            line.sortingOrder = RouteSortingOrder;
            line.numCornerVertices = RoundingVertices;
            line.numCapVertices = RoundingVertices;
            line.widthMultiplier = routeWidth;
            line.startColor = routeColor;
            line.endColor = routeColor;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.positionCount = 0;
        }

        private static Vector3 CentreOf(GridPosition cell)
        {
            return new Vector3(cell.X + CellCentre, cell.Y + CellCentre);
        }

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        private void LateUpdate()
        {
            if (selection == null)
                return;

            if (selection.HasSingleBot)
                DrawRoute(selection.SingleBot);
            else
                SetPointCount(0);
        }

        private void DrawRoute(Bot bot)
        {
            int stepPoints = bot.IsMoving ? 1 : 0;
            int pointCount = 1 + stepPoints + bot.RouteLength;
            if (pointCount < FewestLinePoints)
            {
                SetPointCount(0);
                return;
            }

            SetPointCount(pointCount);
            line.SetPosition(0, new Vector3(bot.CentreX, bot.CentreY));
            if (bot.IsMoving)
                line.SetPosition(1, CentreOf(bot.NextCell));
            for (int index = 0; index < bot.RouteLength; index++)
                line.SetPosition(1 + stepPoints + index, CentreOf(bot.RouteCell(index)));
        }

        private void SetPointCount(int pointCount)
        {
            if (line.positionCount != pointCount)
                line.positionCount = pointCount;
        }
    }
}
