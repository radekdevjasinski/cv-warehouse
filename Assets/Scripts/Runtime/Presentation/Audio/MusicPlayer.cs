using CvWarehouse.Core.Audio;
using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] tracks = System.Array.Empty<AudioClip>();

        private AudioSource audioSource;
        private AudioClip[] playlistTracks;
        private MusicPlaylist playlist;

        public AudioClip CurrentClip => audioSource.clip;

        public void PlayInOrder(AudioClip[] orderedTracks)
        {
            playlistTracks = orderedTracks;
            playlist = new MusicPlaylist(orderedTracks.Length);
            if (playlist.HasTracks)
                PlayCurrentTrack();
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        private void Start()
        {
            PlayInOrder(tracks);
        }

        private void Update()
        {
            if (playlist == null || !playlist.TryAdvance(audioSource.isPlaying))
                return;

            AudioClip finishedClip = audioSource.clip;
            PlayCurrentTrack();
            if (finishedClip != audioSource.clip)
                finishedClip.UnloadAudioData();
        }

        private void PlayCurrentTrack()
        {
            audioSource.clip = playlistTracks[playlist.CurrentTrack];
            audioSource.Play();
        }
    }
}
