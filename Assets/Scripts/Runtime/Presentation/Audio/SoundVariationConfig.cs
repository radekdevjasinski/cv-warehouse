using CvWarehouse.Core.Audio;
using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    [CreateAssetMenu(menuName = "CV Warehouse/Sound Variation Config", fileName = "SoundVariationConfig")]
    public sealed class SoundVariationConfig : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips = System.Array.Empty<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(0.5f, 2f)] private float pitch = 1f;
        [SerializeField, Range(0f, 0.4f)] private float pitchVariation = 0.08f;
        [SerializeField, Min(0f)] private float minSecondsBetweenPlays = 0.05f;
        [SerializeField, Min(1)] private int voices = 4;

        public float Volume => volume;

        public int Voices => voices;

        public AudioClip Clip(int index)
        {
            return clips[index];
        }

        public void CopyTo(SoundVariationSettings settings)
        {
            settings.ClipCount = clips.Length;
            settings.Pitch = pitch;
            settings.PitchVariation = pitchVariation;
            settings.MinSecondsBetweenPlays = minSecondsBetweenPlays;
        }
    }
}
