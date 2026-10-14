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
        private const int NoStyle = -1;
        private const int StylesPerCrewSlot = 2;
        private const float CellCentre = 0.5f;

        [SerializeField] private WarehouseSettings settings;
        [SerializeField] private float routeWidth = 0.18f;
        [SerializeField] private float groupRouteWidth = 0.1f;
        [SerializeField, Range(0f, 1f)] private float routeOpacity = 0.55f;

        private LineRenderer[] lines;
        private int[] shownStyles;
        private BotSelection selection;

        public int VisiblePointCount => lines == null ? 0 : lines[0].positionCount;

        public int VisibleRouteCount { get; private set; }

        public LineRenderer Route(int index)
        {
            return lines[index];
        }

        public void Show(BotSelection botSelection, Material material, int mostBots)
        {
            selection = botSelection;
            lines = new LineRenderer[mostBots];
            shownStyles = new int[mostBots];
            lines[0] = GetComponent<LineRenderer>();
            for (int index = 1; index < lines.Length; index++)
                lines[index] = CreateLine(index);

            for (int index = 0; index < lines.Length; index++)
            {
                Prepare(lines[index], material);
                shownStyles[index] = NoStyle;
            }
        }

        private static Vector3 CentreOf(GridPosition cell)
        {
            return new Vector3(cell.X + CellCentre, cell.Y + CellCentre);
        }

        private static void Prepare(LineRenderer line, Material material)
        {
            line.sharedMaterial = material;
            line.useWorldSpace = false;
            line.sortingOrder = RouteSortingOrder;
            line.numCornerVertices = RoundingVertices;
            line.numCapVertices = RoundingVertices;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.positionCount = 0;
        }

        private static void SetPointCount(LineRenderer line, int pointCount)
        {
            if (line.positionCount != pointCount)
                line.positionCount = pointCount;
        }

        private static bool DrawRoute(LineRenderer line, Bot bot)
        {
            int stepPoints = bot.IsMoving ? 1 : 0;
            int pointCount = 1 + stepPoints + bot.RouteLength;
            if (pointCount < FewestLinePoints)
            {
                SetPointCount(line, 0);
                return false;
            }

            SetPointCount(line, pointCount);
            line.SetPosition(0, new Vector3(bot.CentreX, bot.CentreY));
            if (bot.IsMoving)
                line.SetPosition(1, CentreOf(bot.NextCell));
            for (int index = 0; index < bot.RouteLength; index++)
                line.SetPosition(1 + stepPoints + index, CentreOf(bot.RouteCell(index)));
            return true;
        }

        private LineRenderer CreateLine(int index)
        {
            var lineObject = new GameObject("Route " + index);
            lineObject.transform.SetParent(transform, false);
            return lineObject.AddComponent<LineRenderer>();
        }

        private void LateUpdate()
        {
            if (selection == null)
                return;

            int routeCount = 0;
            for (int index = 0; index < lines.Length; index++)
            {
                if (index >= selection.Count)
                {
                    SetPointCount(lines[index], 0);
                    continue;
                }

                Bot bot = selection.Bots[index];
                Style(index, bot.CrewSlot, selection.HasManyBots);
                if (DrawRoute(lines[index], bot))
                    routeCount++;
            }

            VisibleRouteCount = routeCount;
        }

        private void Style(int index, int crewSlot, bool isOneOfMany)
        {
            int style = crewSlot * StylesPerCrewSlot + (isOneOfMany ? 1 : 0);
            if (shownStyles[index] == style)
                return;

            shownStyles[index] = style;
            Color color = settings.CrewColor(crewSlot);
            color.a = routeOpacity;
            lines[index].widthMultiplier = isOneOfMany ? groupRouteWidth : routeWidth;
            lines[index].startColor = color;
            lines[index].endColor = color;
        }
    }
}
