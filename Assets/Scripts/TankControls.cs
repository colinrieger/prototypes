using UnityEngine;
using UnityEngine.UI;

public class TankControls : MonoBehaviour
{
    public float m_StartingHealth = 100f;
    public float m_Speed = 16f;
    public float m_TankRotationSpeed = 90f;
    public float m_TurretRotationSpeed = 180f;
    public float m_ShellForce = 100f;
    public Rigidbody m_ShellRigidBody;
    public Transform m_ShellOriginTransform;
    public Transform m_TurretTranform;
    public Slider m_HealthSlider;

    private float m_CurrentHealth;
    private float m_CurrentFireCooldown;

    private const float c_FireCooldown = 6f; // seconds

    private void OnEnable()
    {
        m_CurrentFireCooldown = 0f;
        m_CurrentHealth = m_StartingHealth;

        UpdateHealthSlider();
    }

    void Update()
    {
        if (m_CurrentFireCooldown > 0f)
            m_CurrentFireCooldown -= Time.deltaTime;
    }

    public void Fire()
    {
        if (m_CurrentFireCooldown > 0f)
            return;
        Rigidbody shellInstance = Instantiate(m_ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellForce * m_ShellOriginTransform.forward;
        m_CurrentFireCooldown = c_FireCooldown;
    }

    public void ApplyDamage(float damage)
    {
        m_CurrentHealth -= damage;

        UpdateHealthSlider();

        if (m_CurrentHealth <= 0f)
            Death();
    }

    private void UpdateHealthSlider()
    {
        m_HealthSlider.value = m_CurrentHealth;
    }

    private void Death()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
