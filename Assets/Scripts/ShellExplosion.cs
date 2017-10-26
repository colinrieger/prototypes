using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankLayerMask;
    public float m_MaxDamage = 50f;
    public float m_Force = 1000f;
    public float m_Radius = 5f;
    public float m_Lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, m_Lifetime);
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

            TankControls tankControls = targetRigidbody.GetComponent<TankControls>();
            if (tankControls != null)
                tankControls.ApplyDamage(CalculateDamage(targetRigidbody.position));
        }

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float relativeDistance = (m_Radius - explosionToTarget.magnitude) / m_Radius;

        return Mathf.Max(0f, relativeDistance * m_MaxDamage);
    }
}