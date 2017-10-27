using UnityEngine;
using UnityEngine.UI;

public class TankControls : MonoBehaviour
{
    public float m_StartingHealth = 100f;
    public float m_Speed = 16f;
    public float m_TankRotationSpeed = 90f;
    public float m_TurretRotationSpeed = 90f;
    public float m_ShellForce = 100f;
    public float m_FireCooldown = 2f; // seconds
    public Rigidbody m_ShellRigidBody;
    public GameObject m_ExplosionPrefab;
    public Transform m_ShellOriginTransform;
    public Transform m_TurretTranform;
    public Slider m_HealthSlider;
    public Slider m_CooldownSlider;

    private float m_CurrentHealth;
    private float m_CurrentFireCooldown;
    private ParticleSystem m_ExplosionParticles;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_CurrentFireCooldown = 0f;

        UpdateHealthSlider();
        UpdateCooldownSlider();
    }

    void Update()
    {
        if (m_CurrentFireCooldown > 0f)
        {
            m_CurrentFireCooldown -= Time.deltaTime;
            UpdateCooldownSlider();
        }
    }

    public void Fire()
    {
        if (m_CurrentFireCooldown > 0f)
            return;
        Rigidbody shellInstance = Instantiate(m_ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellForce * m_ShellOriginTransform.forward;
        m_CurrentFireCooldown = m_FireCooldown;
        UpdateCooldownSlider();
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
        m_HealthSlider.value = (m_CurrentHealth / m_StartingHealth) * 100f;
    }

    private void UpdateCooldownSlider()
    {
        m_CooldownSlider.value = 100f - ((m_CurrentFireCooldown / m_FireCooldown) * 100f);
    }

    private void Death()
    {
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();

        gameObject.SetActive(false);
    }
}
