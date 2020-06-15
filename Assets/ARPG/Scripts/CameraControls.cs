using UnityEngine;

namespace arpg
{
    public class CameraControls : MonoBehaviour
    {
        private GameObject m_Player;
        private readonly Vector3 c_PositionOffset = new Vector3(0f, 10f, -4f);

        private void Start()
        {
            m_Player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            if (m_Player != null)
                transform.position = m_Player.transform.position + c_PositionOffset;
        }
    }
}
