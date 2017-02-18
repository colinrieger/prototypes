using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankLayerMask;
    public float m_MaxDamage = 100f;
    public float m_Force = 1000f;
    public float m_Radius = 5f;
    public float m_ShellLifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, m_ShellLifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_Radius, m_TankLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_Force, transform.position, m_Radius);

            /*TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);*/
        }

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_Radius - explosionDistance) / m_Radius;

        return Mathf.Max(0f, relativeDistance * m_MaxDamage);
    }
}