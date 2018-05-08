using System;
using UnityEngine;
using UnityEngine.UI;

public class TankControls : MonoBehaviour
{
    public Rigidbody ShellRigidBody;
    public GameObject ExplosionPrefab;

    public float Speed { get { return m_Speed; } set { m_Speed = value; } }
    public float TankRotationSpeed { get { return m_TankRotationSpeed; } set { m_TankRotationSpeed = value; } }
    public float ShellVelocity { get { return m_ShellVelocity; } set { m_ShellVelocity = value; } }

    private float m_StartingHealth = 100f;
    private float m_Speed = 20f;
    private float m_TankRotationSpeed = 90f;
    private float m_TurretRotationSpeed = 180f;
    private float m_BarrelRotationSpeed = 30f;
    private float m_BarrelMinXRotation = -15f;
    private float m_BarrelMaxXRotation = 0f;
    private float m_ShellVelocity = 60f;
    private float m_FireCooldown = 2f; // seconds
    
    private Rigidbody m_TankRigidbody;
    private GameObject m_TankTurret;
    private GameObject m_TankBarrel;
    private Slider m_HealthSlider;
    private Slider m_CooldownSlider;
    private Transform m_ShellOriginTransform;
    private float m_CurrentHealth;
    private float m_CurrentFireCooldown;
    private ParticleSystem m_ExplosionParticles;

    private void Awake()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
        m_TankBarrel = transform.Find("Renderers/Turret/Barrel").gameObject;
        m_HealthSlider = transform.Find("Renderers/Turret/Canvas/HealthSlider").GetComponent<Slider>();
        m_CooldownSlider = transform.Find("Renderers/Turret/Canvas/CooldownSlider").GetComponent<Slider>();
        m_ShellOriginTransform = transform.Find("Renderers/Turret/Barrel/ShellOriginTransform").transform;
        m_ExplosionParticles = Instantiate(ExplosionPrefab).GetComponent<ParticleSystem>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_CurrentFireCooldown = 0f;

        if (m_TankRigidbody != null)
        {
            m_TankRigidbody.velocity = Vector3.zero;
            m_TankRigidbody.angularVelocity = Vector3.zero;
        }

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

        if (Vector3.Dot(transform.up, Vector3.down) >= 0.99f)
            Death();
    }

    public Quaternion RotateTank(float yRotationValue)
    {
        Quaternion tankRotation = Quaternion.Euler(0f, yRotationValue * m_TankRotationSpeed * Time.deltaTime, 0f);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * tankRotation);
        return tankRotation;
    }

    public void RotateTankTowards(Quaternion rotationToTarget)
    {
        Quaternion tankRotation = Quaternion.RotateTowards(transform.rotation, rotationToTarget, Time.deltaTime * m_TankRotationSpeed);
        m_TankRigidbody.MoveRotation(tankRotation);
    }

    public void RotateTurret(float yRotationValue, Quaternion inverseRotation)
    {
        m_TankTurret.transform.localRotation = m_TankTurret.transform.localRotation *
                                               Quaternion.Euler(0f, yRotationValue * m_TurretRotationSpeed * Time.deltaTime, 0f) *
                                               Quaternion.Inverse(inverseRotation);
    }

    public void RotateTurretTowards(Quaternion rotationToTarget)
    {
        Quaternion turretRotation = Quaternion.RotateTowards(m_TankTurret.transform.rotation, rotationToTarget, Time.deltaTime * m_TurretRotationSpeed);
        m_TankTurret.transform.rotation = turretRotation;
    }

    public void RotateBarrel(float xRotationValue)
    {
        Quaternion barrelRotation = m_TankBarrel.transform.localRotation *
                                    Quaternion.Euler(-xRotationValue * m_BarrelRotationSpeed * Time.deltaTime, 0f, 0f);
        m_TankBarrel.transform.localRotation = Quaternion.Euler(ClampAngle(barrelRotation.eulerAngles.x,
                                                                           m_BarrelMinXRotation,
                                                                           m_BarrelMaxXRotation),
                                                                m_TankBarrel.transform.localEulerAngles.y,
                                                                m_TankBarrel.transform.localEulerAngles.z);
    }

    public void MoveTank(float forwardMoveValue = 1f)
    {
        Vector3 movement = transform.forward * forwardMoveValue * m_Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);
    }

    public void Fire()
    {
        if (m_CurrentFireCooldown > 0f)
            return;
        Rigidbody shellInstance = Instantiate(ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellVelocity * m_ShellOriginTransform.forward;
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

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180)
            return Math.Max(angle - 360, min) + 360;
        return Math.Min(angle, max);
    }
}
