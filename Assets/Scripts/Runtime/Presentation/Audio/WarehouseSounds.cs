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
        [SerializeField] private BotVoicePlayer botVoices;

        private WarehouseSimulation simulation;
        private BotSelection selection;

        public void Listen(WarehouseSimulation listenedSimulation, BotSelection listenedSelection)
        {
            Unsubscribe();
            simulation = listenedSimulation;
            selection = listenedSelection;
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
            simulation.Score.Delivered += PlayDelivery;
            selection.BotsSelected += botVoices.Play;
        }

        private void Unsubscribe()
        {
            if (simulation == null)
                return;

            simulation.Extracted -= PlayExtract;
            simulation.Score.Delivered -= PlayDelivery;
            selection.BotsSelected -= botVoices.Play;
        }

        private void PlayExtract(Bot bot)
        {
            extractSound.Play();
        }

        private void PlayDelivery(CvBlock block, IReadOnlyList<int> letters)
        {
            deliverySound.Play();
        }
    }
}
