using UnityEngine;

namespace CvWarehouse.Presentation.Audio
{
    [CreateAssetMenu(menuName = "CV Warehouse/Audio Session Channel", fileName = "AudioSessionChannel")]
    public sealed class AudioSessionChannel : ScriptableObject
    {
        public AudioSession Session { get; private set; }

        public bool HasSession => Session != null;

        public void Register(AudioSession session)
        {
            Session = session;
        }

        public void Unregister(AudioSession session)
        {
            if (Session == session)
                Session = null;
        }
    }
}
