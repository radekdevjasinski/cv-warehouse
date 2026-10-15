using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Audio;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class WarehouseDemo : MonoBehaviour
    {
        [SerializeField] private WarehouseSettings settings;
        [SerializeField] private GeneratorConfig generatorConfig;
        [SerializeField] private Material shapeMaterial;
        [SerializeField] private WarehouseCamera warehouseCamera;
        [SerializeField] private WarehouseGridView gridView;
        [SerializeField] private WarehouseBoxesView boxesView;
        [SerializeField] private WarehouseBotsView botsView;
        [SerializeField] private WarehouseHudView hudView;
        [SerializeField] private PointsView pointsView;
        [SerializeField] private ShiftStatusView shiftStatusView;
        [SerializeField] private CrewListView crewListView;
        [SerializeField] private SelectionClearArea crewPanelClearArea;
        [SerializeField] private BotRouteView routeView;
        [SerializeField] private BotPanelView botPanelView;
        [SerializeField] private BotGroupPanelView botGroupPanelView;
        [SerializeField] private BoxPanelView boxPanelView;
        [SerializeField] private BoxHighlightView boxHighlightView;
        [SerializeField] private SelectionBoxView selectionBoxView;
        [SerializeField] private WarehouseBotSelector botSelector;
        [SerializeField] private WarehousePauseControl pauseControl;
        [SerializeField] private WarehouseSounds sounds;
        [SerializeField] private int seed = 12345;

        private SquareShapeFactory shapes;

        public WarehouseSimulation Simulation { get; private set; }

        public BotShop Shop { get; private set; }

        public BotSelection Selection { get; private set; }

        public BoxSelection BoxSelection { get; private set; }

        public int Seed => seed;

        public bool TryBegin(IReadOnlyList<CvBlock> blocks)
        {
            var random = new System.Random(seed);
            if (!new WarehouseGenerator(generatorConfig.CreateGeneratorSettings(), random).TryGenerate(blocks, out WarehouseLayout layout))
                return false;

            Simulation = new WarehouseSimulation(layout, settings.CreateSimulationSettings(), random);
            for (int bot = 0; bot < settings.StartingBots; bot++)
                Simulation.TryAddBot();
            Shop = new BotShop(Simulation, settings.CreateShopSettings());
            Selection = new BotSelection();
            BoxSelection = new BoxSelection();

            shapes = new SquareShapeFactory(shapeMaterial);
            warehouseCamera.Frame(layout);
            ShowWarehouse(layout);
            ShowControls();
            sounds.Listen(Simulation, Selection);
            return true;
        }

        private void ShowWarehouse(WarehouseLayout layout)
        {
            gridView.Show(layout, shapeMaterial);
            boxesView.Show(layout, shapes);
            boxHighlightView.Show(BoxSelection, shapes);
            botsView.Show(Simulation, Selection, shapes);
            routeView.Show(Selection, shapeMaterial, Simulation.MaxBots);
            selectionBoxView.Show(shapes);
        }

        private void ShowControls()
        {
            hudView.Show(Simulation, Shop);
            pointsView.Show(Shop);
            shiftStatusView.Show(Simulation);
            crewListView.Show(Simulation, Selection, BoxSelection);
            crewPanelClearArea.Connect(Selection, BoxSelection);
            botPanelView.Show(Simulation, Shop, Selection);
            botGroupPanelView.Show(Simulation, Shop, Selection);
            boxPanelView.Show(BoxSelection);
            botSelector.Connect(Simulation, Selection, BoxSelection);
            pauseControl.Show(Simulation);
        }

        private void Update()
        {
            if (Simulation != null)
                Simulation.Advance(Time.deltaTime);
        }

        private void OnDestroy()
        {
            shapes?.Dispose();
        }
    }
}
