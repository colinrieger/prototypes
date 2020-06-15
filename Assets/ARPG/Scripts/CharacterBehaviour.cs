using UnityEngine;
using UnityEngine.AI;

namespace arpg
{
    public class CharacterBehaviour : MonoBehaviour
    {
        public float StartingHealth;
        public float MaxAttackRange;
        public float MaxInteractRange;
        public float AttackCooldown;
        public float Damage;

        public GameObject AttackTarget { get; set; }
        public GameObject InteractTarget { get; set; }

        private float m_CurrentHealth;
        private float m_CurrentAttackCooldown;

        private Animator m_Animator;
        private NavMeshAgent m_NavMeshAgent;

        private const float c_RotationMultiplier = 20f;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            m_CurrentHealth = StartingHealth;

            m_NavMeshAgent.updatePosition = false;
            m_NavMeshAgent.updateRotation = false;
        }

        void Update()
        {
            Move();
            Attack();
        }

        private void Attack()
        {
            if (m_CurrentAttackCooldown > 0f)
            {
                m_CurrentAttackCooldown -= Time.deltaTime;

                AttackTarget = null; // we're not ready to attack, ignore the mouse hold or npc targeting from this frame
            }
            else if (AttackTargetIsInAttackRange())
            {
                CharacterBehaviour characterBehaviour = AttackTarget.GetComponent<CharacterBehaviour>();
                if (characterBehaviour != null && !characterBehaviour.Dead())
                {
                    m_CurrentAttackCooldown = AttackCooldown;
                    m_Animator.ResetTrigger("Hit");
                    m_Animator.SetTrigger("Attack");
                    characterBehaviour.ApplyDamage(Damage);
                }

                AttackTarget = null; // we've executed the attack, require a new mouse hold/click or npc targeting
            }
            else if (InteractTargetIsInInteractRange())
            {
                ItemPickup itemPickup = InteractTarget.GetComponent<ItemPickup>();
                if (itemPickup != null)
                {
                    GetComponent<CharacterInventory>().AddItem(itemPickup.ItemId);
                    Destroy(InteractTarget);
                }

                InteractTarget = null;
            }
        }

        public void ApplyDamage(float damage)
        {
            m_CurrentHealth -= damage;

            if (Dead())
                BroadcastMessage("Death");
            else if (!Attacking())
                m_Animator.SetTrigger("Hit");
        }

        public bool Attacking()
        {
            return m_Animator.GetBool("Attack") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("attack");
        }

        public bool Dead()
        {
            return m_CurrentHealth <= 0f;
        }

        public bool AttackTargetIsAlive()
        {
            return (AttackTarget != null) && !AttackTarget.GetComponent<CharacterBehaviour>().Dead();
        }

        private bool AttackTargetIsInAttackRange()
        {
            return (AttackTarget != null) && ((AttackTarget.transform.position - transform.position).magnitude <= MaxAttackRange);
        }

        private bool InteractTargetIsInInteractRange()
        {
            return (InteractTarget != null) && ((InteractTarget.transform.position - transform.position).magnitude <= MaxInteractRange);
        }

        private void Move()
        {
            if (AttackTarget != null)
                m_NavMeshAgent.destination = AttackTarget.transform.position;
            else if (InteractTarget != null)
                m_NavMeshAgent.destination = InteractTarget.transform.position;

            if ((m_NavMeshAgent.destination - transform.position).magnitude >= 0.1f)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation((m_NavMeshAgent.destination - transform.position).normalized),
                                                      Time.deltaTime * c_RotationMultiplier);

            if (AttackTargetIsInAttackRange())
                m_NavMeshAgent.destination = transform.position;

            m_Animator.SetFloat("Speed", m_NavMeshAgent.desiredVelocity.magnitude);
        }

        void OnAnimatorMove()
        {
            transform.position = m_NavMeshAgent.nextPosition;
        }

        // BroadcastMessage Receiver
        void Death()
        {
            m_Animator.SetTrigger("Death");
            m_NavMeshAgent.enabled = false;
            enabled = false;
        }
    }
}