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
        [SerializeField] private int seed = 12345;

        private SquareShapeFactory shapes;

        public WarehouseSimulation Simulation { get; private set; }

        public BotShop Shop { get; private set; }

        public int Seed => seed;

        private void Start()
        {
            var random = new System.Random(seed);
            WarehouseLayout layout = new WarehouseGenerator(generatorConfig.CreateGeneratorSettings(), random).Generate();
            Simulation = new WarehouseSimulation(layout, settings.CreateSimulationSettings());
            for (int bot = 0; bot < settings.StartingBots; bot++)
                Simulation.TryAddBot();
            Shop = new BotShop(Simulation, settings.CreateShopSettings());

            shapes = new SquareShapeFactory(shapeMaterial);
            warehouseCamera.Frame(layout);
            gridView.Show(layout, shapes);
            boxesView.Show(layout, shapes);
            botsView.Show(Simulation, shapes);
            hudView.Show(Simulation, Shop);
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
