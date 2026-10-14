using CvWarehouse.Core.Warehouse;
using UnityEngine;
using UnityEngine.Serialization;

namespace CvWarehouse.Presentation.Warehouse
{
    [CreateAssetMenu(menuName = "CV Warehouse/Warehouse Settings", fileName = "WarehouseSettings")]
    public sealed class WarehouseSettings : ScriptableObject
    {
        [SerializeField] private int ticksPerSecond = 60;
        [SerializeField] private int maxTicksPerFrame = 5;
        [SerializeField] private int ticksPerCell = 8;
        [SerializeField] private int speedStepPercent = 25;
        [SerializeField, FormerlySerializedAs("lightExtractTicks")] private int normalExtractTicks = 20;
        [SerializeField, FormerlySerializedAs("mediumExtractTicks")] private int importantExtractTicks = 30;
        [SerializeField, FormerlySerializedAs("heavyExtractTicks")] private int keyExtractTicks = 45;
        [SerializeField] private int unloadTicks = 20;
        [SerializeField, FormerlySerializedAs("lightPiecePoints")] private int normalPiecePoints = 1;
        [SerializeField, FormerlySerializedAs("mediumPiecePoints")] private int importantPiecePoints = 2;
        [SerializeField, FormerlySerializedAs("heavyPiecePoints")] private int keyPiecePoints = 3;
        [SerializeField] private int startingBots = 3;

        [Header("Shop prices, in thousandths of all the points in the warehouse")]
        [SerializeField] private int firstBotPricePermille = 15;
        [SerializeField] private int botPriceGrowthPercent = 125;
        [SerializeField] private int maxUpgradeLevel = 5;
        [SerializeField] private int carryUpgradePricePermille = 10;
        [SerializeField] private int speedUpgradePricePermille = 10;
        [SerializeField] private int upgradePriceGrowthPercent = 150;

        [Header("Crew, in the order bots join")]
        [SerializeField] private CrewMember[] crew =
        {
            new CrewMember("PIP", new Color32(0x7b, 0xd3, 0x89, 0xff)),
            new CrewMember("BLIP", new Color32(0xa5, 0x93, 0xe0, 0xff)),
            new CrewMember("NIB", new Color32(0x6c, 0xb6, 0xe0, 0xff)),
            new CrewMember("MOX", new Color32(0xe5, 0x8f, 0xb5, 0xff)),
            new CrewMember("ZEE", new Color32(0xc8, 0xdd, 0x6a, 0xff)),
            new CrewMember("TUK", new Color32(0x6f, 0xe0, 0xd0, 0xff)),
            new CrewMember("FIG", new Color32(0xe0, 0xd2, 0x6c, 0xff)),
            new CrewMember("LUM", new Color32(0xe0, 0x70, 0x70, 0xff))
        };

        public int StartingBots => startingBots;

        public Color CrewColor(int crewSlot)
        {
            return crew.Length == 0 ? Color.white : crew[crewSlot % crew.Length].Color;
        }

        public ShopSettings CreateShopSettings()
        {
            return new ShopSettings
            {
                FreeBots = startingBots,
                FirstBotPricePermille = firstBotPricePermille,
                BotPriceGrowthPercent = botPriceGrowthPercent,
                MaxUpgradeLevel = maxUpgradeLevel,
                CarryUpgradePricePermille = carryUpgradePricePermille,
                SpeedUpgradePricePermille = speedUpgradePricePermille,
                UpgradePriceGrowthPercent = upgradePriceGrowthPercent
            };
        }

        public SimulationSettings CreateSimulationSettings()
        {
            return new SimulationSettings
            {
                TickSeconds = 1f / ticksPerSecond,
                MaxTicksPerAdvance = maxTicksPerFrame,
                TicksPerCell = ticksPerCell,
                SpeedStepPercent = speedStepPercent,
                NormalExtractTicks = normalExtractTicks,
                ImportantExtractTicks = importantExtractTicks,
                KeyExtractTicks = keyExtractTicks,
                UnloadTicks = unloadTicks,
                NormalPiecePoints = normalPiecePoints,
                ImportantPiecePoints = importantPiecePoints,
                KeyPiecePoints = keyPiecePoints,
                CrewNames = CrewNames()
            };
        }

        private string[] CrewNames()
        {
            var names = new string[crew.Length];
            for (int index = 0; index < crew.Length; index++)
                names[index] = crew[index].Name;
            return names;
        }
    }
}
