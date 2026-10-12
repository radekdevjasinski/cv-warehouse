using CvWarehouse.Core.Warehouse;
using TMPro;
using UnityEngine;

namespace CvWarehouse.Presentation.Warehouse
{
    public sealed class BotStatusText : MonoBehaviour
    {
        private const string WalkingLabel = "Walking to a box";
        private const string PickingUpLabel = "Picking up";
        private const string HaulingOneWordLabel = "Hauling 1 word";
        private const string HaulingWordsFormat = "Hauling {0} words";
        private const string UnloadingLabel = "Loading truck";
        private const string ParkingLabel = "Heading to parking";
        private const string IdleLabel = "Idle";

        [SerializeField] private TMP_Text text;

        private Bot shownBot;
        private BotState shownState;
        private int shownWords;

        public void Show(Bot bot)
        {
            if (shownBot == bot && shownState == bot.State && shownWords == bot.CargoWords)
                return;

            shownBot = bot;
            shownState = bot.State;
            shownWords = bot.CargoWords;
            if (bot.State == BotState.GoingToDrop && bot.CargoWords > 1)
                text.SetText(HaulingWordsFormat, bot.CargoWords);
            else
                text.SetText(LabelOf(bot.State));
        }

        private static string LabelOf(BotState state)
        {
            switch (state)
            {
                case BotState.GoingToBox:
                    return WalkingLabel;
                case BotState.Extracting:
                    return PickingUpLabel;
                case BotState.GoingToDrop:
                    return HaulingOneWordLabel;
                case BotState.Unloading:
                    return UnloadingLabel;
                case BotState.GoingToParking:
                    return ParkingLabel;
                default:
                    return IdleLabel;
            }
        }
    }
}
