using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask TankLayerMask;
    public ParticleSystem ExplosionParticles;

    private float m_MaxDamage = 50f;
    private float m_Force = 1000f;
    private float m_Radius = 5f;
    private float m_Lifetime = 10f;

    private Rigidbody m_ShellRigidbody;

    private void Awake()
    {
        m_ShellRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, m_Lifetime);
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(m_ShellRigidbody.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_Radius, TankLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_Force, transform.position, m_Radius);

            TankControls tankControls = targetRigidbody.GetComponent<TankControls>();
            if (tankControls != null)
                tankControls.ApplyDamage(CalculateDamage(targetRigidbody.position));
        }
        
        ExplosionParticles.transform.parent = null;
        ExplosionParticles.Play();

        ParticleSystem.MainModule mainModule = ExplosionParticles.main;
        Destroy(ExplosionParticles.gameObject, mainModule.duration);

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float relativeDistance = (m_Radius - explosionToTarget.magnitude) / m_Radius;

        return Mathf.Max(0f, relativeDistance * m_MaxDamage);
    }
}