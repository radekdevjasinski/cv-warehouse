using CvWarehouse.Core.Warehouse;
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
        [SerializeField] private BotRouteView routeView;
        [SerializeField] private BotPanelView botPanelView;
        [SerializeField] private BotGroupPanelView botGroupPanelView;
        [SerializeField] private SelectionBoxView selectionBoxView;
        [SerializeField] private WarehouseBotSelector botSelector;
        [SerializeField] private WarehousePauseControl pauseControl;
        [SerializeField] private int seed = 12345;

        private SquareShapeFactory shapes;

        public WarehouseSimulation Simulation { get; private set; }

        public BotShop Shop { get; private set; }

        public BotSelection Selection { get; private set; }

        public int Seed => seed;

        private void Start()
        {
            var random = new System.Random(seed);
            WarehouseLayout layout = new WarehouseGenerator(generatorConfig.CreateGeneratorSettings(), random).Generate();
            Simulation = new WarehouseSimulation(layout, settings.CreateSimulationSettings());
            for (int bot = 0; bot < settings.StartingBots; bot++)
                Simulation.TryAddBot();
            Shop = new BotShop(Simulation, settings.CreateShopSettings());
            Selection = new BotSelection();

            shapes = new SquareShapeFactory(shapeMaterial);
            warehouseCamera.Frame(layout);
            ShowWarehouse(layout);
            ShowControls();
        }

        private void ShowWarehouse(WarehouseLayout layout)
        {
            gridView.Show(layout, shapes);
            boxesView.Show(layout, shapes);
            botsView.Show(Simulation, Selection, shapes);
            routeView.Show(Selection, shapeMaterial);
            selectionBoxView.Show(shapes);
        }

        private void ShowControls()
        {
            hudView.Show(Simulation, Shop);
            botPanelView.Show(Simulation, Shop, Selection);
            botGroupPanelView.Show(Simulation, Selection);
            botSelector.Connect(Simulation, Selection);
            pauseControl.Show(Simulation);
        }

        private void Update()
        {
            Simulation.Advance(Time.deltaTime);
        }

        private void OnDestroy()
        {
            shapes?.Dispose();
        }
    }
}
