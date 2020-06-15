using UnityEngine;

namespace arpg
{
    public class NPCControls : MonoBehaviour
    {
        public LayerMask PlayerLayerMask;

        private CharacterBehaviour m_CharacterBehaviour;

        private const float c_AgroRadius = 5f;

        private void Awake()
        {
            m_CharacterBehaviour = GetComponent<CharacterBehaviour>();
        }

        private void FixedUpdate()
        {
            if (!m_CharacterBehaviour.AttackTargetIsAlive())
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, c_AgroRadius, PlayerLayerMask);
                if (colliders.Length > 0)
                    m_CharacterBehaviour.AttackTarget = colliders[0].gameObject;
            }
        }

        void AfterDeath()
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }

        // BroadcastMessage Receiver
        void Death()
        {
            Invoke("AfterDeath", 2);
            enabled = false;
        }
    }
}