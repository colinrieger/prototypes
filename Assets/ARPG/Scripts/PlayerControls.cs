using UnityEngine;
using UnityEngine.AI;

namespace arpg
{
    public class PlayerControls : MonoBehaviour
    {
        private CharacterBehaviour m_CharacterBehaviour;
        private NavMeshAgent m_NavMeshAgent;

        private void Awake()
        {
            m_CharacterBehaviour = GetComponent<CharacterBehaviour>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
                SetTargetOrDestination();
        }

        private void SetTargetOrDestination()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                switch (hit.collider.tag)
                {
                    case "NPC":
                        m_CharacterBehaviour.AttackTarget = hit.collider.gameObject;
                        m_CharacterBehaviour.InteractTarget = null;
                        break;
                    case "Item":
                        m_CharacterBehaviour.InteractTarget = hit.collider.gameObject;
                        m_CharacterBehaviour.AttackTarget = null;
                        break;
                    default:
                        m_NavMeshAgent.destination = hit.point;
                        m_CharacterBehaviour.AttackTarget = null;
                        m_CharacterBehaviour.InteractTarget = null;
                        break;
                }
            }
        }

        // BroadcastMessage Receiver
        void Death()
        {
            enabled = false;
        }
    }
}