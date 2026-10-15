using CvWarehouse.Core.Audio;
using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    [CreateAssetMenu(menuName = "CV Warehouse/Bot Voice Config", fileName = "BotVoiceConfig")]
    public sealed class BotVoiceConfig : ScriptableObject
    {
        [Tooltip("One voice per crew slot, in the order bots join.")]
        [SerializeField] private AudioClip[] voices = System.Array.Empty<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(0.5f, 2f)] private float pitch = 1f;
        [SerializeField, Range(0f, 0.4f)] private float pitchVariation = 0.03f;

        [Header("Selecting several bots")]
        [SerializeField, Min(1)] private int maxVoicesInCascade = 4;
        [SerializeField, Range(0f, 0.3f)] private float secondsBetweenVoices = 0.05f;
        [SerializeField, Range(0f, 1f)] private float groupVolume = 0.8f;

        public float Volume => volume;

        public int MaxVoicesInCascade => maxVoicesInCascade;

        public AudioClip Voice(int index)
        {
            return voices[index];
        }

        public void CopyTo(BotVoiceSettings settings)
        {
            settings.VoiceCount = voices.Length;
            settings.MaxVoicesInCascade = maxVoicesInCascade;
            settings.SecondsBetweenVoices = secondsBetweenVoices;
            settings.GroupVolume = groupVolume;
            settings.Pitch = pitch;
            settings.PitchVariation = pitchVariation;
        }
    }
}
