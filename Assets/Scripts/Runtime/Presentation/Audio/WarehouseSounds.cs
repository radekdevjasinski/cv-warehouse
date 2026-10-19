using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using CvWarehouse.Core.Warehouse;
using CvWarehouse.Presentation.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    public sealed class WarehouseSounds : MonoBehaviour
    {
        [SerializeField] private VariedSoundPlayer extractSound;
        [SerializeField] private VariedSoundPlayer deliverySound;
        [SerializeField] private VariedSoundPlayer carryUpgradeSound;
        [SerializeField] private VariedSoundPlayer speedUpgradeSound;
        [SerializeField] private BotVoicePlayer botVoices;

        private WarehouseSimulation simulation;
        private BotSelection selection;
        private BotShop shop;

        public void Listen(WarehouseSimulation listenedSimulation, BotSelection listenedSelection, BotShop listenedShop)
        {
            Unsubscribe();
            simulation = listenedSimulation;
            selection = listenedSelection;
            shop = listenedShop;
            if (isActiveAndEnabled)
                Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (simulation == null)
                return;

            simulation.Extracted += PlayExtract;
            simulation.BotAdded += botVoices.Play;
            simulation.Score.Delivered += PlayDelivery;
            selection.BotsSelected += botVoices.Play;
            shop.Upgraded += PlayUpgrade;
        }

        private void Unsubscribe()
        {
            if (simulation == null)
                return;

            simulation.Extracted -= PlayExtract;
            simulation.BotAdded -= botVoices.Play;
            simulation.Score.Delivered -= PlayDelivery;
            selection.BotsSelected -= botVoices.Play;
            shop.Upgraded -= PlayUpgrade;
        }

        private void PlayExtract(Bot bot)
        {
            extractSound.Play();
        }

        private void PlayUpgrade(BotUpgrade upgrade)
        {
            if (upgrade == BotUpgrade.Speed)
                speedUpgradeSound.Play();
            else
                carryUpgradeSound.Play();
        }

        private void PlayDelivery(CvBlock block, IReadOnlyList<int> letters)
        {
            deliverySound.Play();
        }
    }
}
