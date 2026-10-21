using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    [DefaultExecutionOrder(-100)]
    public sealed class AudioSessionStarter : MonoBehaviour
    {
        [SerializeField] private AudioSessionChannel channel;
        [SerializeField] private AudioSession sessionPrefab;

        private void Awake()
        {
            if (channel.HasSession)
                return;

            AudioSession session = Instantiate(sessionPrefab);
            DontDestroyOnLoad(session.gameObject);
        }
    }
}
