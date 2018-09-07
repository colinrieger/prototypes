using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask TankLayerMask;
    public ParticleSystem ExplosionParticles;

    private Rigidbody m_ShellRigidbody;

    private const float c_MaxDamage = 50f;
    private const float c_Force = 1000f;
    private const float c_Radius = 5f;
    private const float c_Lifetime = 10f;

    private void Awake()
    {
        m_ShellRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, c_Lifetime);
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(m_ShellRigidbody.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, c_Radius, TankLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(c_Force, transform.position, c_Radius);

            TankControls tankControls = targetRigidbody.GetComponent<TankControls>();
            if (tankControls != null)
                tankControls.CurrentHealth -= CalculateDamage(targetRigidbody.position);
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
        float relativeDistance = (c_Radius - explosionToTarget.magnitude) / c_Radius;

        return Mathf.Max(0f, relativeDistance * c_MaxDamage);
    }
}