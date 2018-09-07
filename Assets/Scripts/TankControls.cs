using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankControls : MonoBehaviour
{
    public Rigidbody ShellRigidBody;
    public GameObject ExplosionPrefab;

    public float CurrentHealth
    {
        get { return m_CurrentHealth; }
        set
        {
            m_CurrentHealth = Math.Min(c_StartingHealth, Math.Max(0f, value));

            UpdateHealthSlider();
            
            if (m_CurrentHealth <= 0f)
                Death();
        }
    }

    public float CurrentFireCooldown
    {
        get { return m_CurrentFireCooldown; }
        set
        {
            m_CurrentFireCooldown = Math.Min(FireCooldown, Math.Max(0f, value));

            UpdateCooldownSlider();
        }
    }

    public float Speed { get { return c_Speed + SpeedModifier; } }
    public float TankRotationSpeed { get { return c_TankRotationSpeed + TankRotationSpeedModifier; } }
    public float ShellVelocity { get { return c_ShellVelocity + ShellVelocityModifier; } }
    public float FireCooldown { get { return c_FireCooldown - FireCooldownModifier; } }

    public float SpeedModifier { get; set; }
    public float TankRotationSpeedModifier { get; set; }
    public float ShellVelocityModifier { get; set; }
    public float FireCooldownModifier { get; set; }

    private float m_CurrentHealth;
    private float m_CurrentFireCooldown;
    
    private const float c_StartingHealth = 100f;
    private const float c_FireCooldown = 2f; // seconds
    private const float c_Speed = 20f;
    private const float c_TankRotationSpeed = 90f;
    private const float c_TurretRotationSpeed = 180f;
    private const float c_ShellVelocity = 60f;
    private const float c_BarrelRotationSpeed = 30f;
    private const float c_BarrelMinXRotation = -15f;
    private const float c_BarrelMaxXRotation = 0f;

    private readonly Vector3 c_DefaultTurretRotation = new Vector3(0f, 0f, 0f);

    private Rigidbody m_TankRigidbody;
    private GameObject m_TankTurret;
    private GameObject m_TankBarrel;
    private Slider m_HealthSlider;
    private Slider m_CooldownSlider;
    private Transform m_ShellOriginTransform;
    private ParticleSystem m_ExplosionParticles;

    private List<Modifier> m_Modifiers = new List<Modifier>();

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
        ResetModifiers();

        CurrentHealth = c_StartingHealth;
        CurrentFireCooldown = c_FireCooldown;
        
        m_TankTurret.transform.localEulerAngles = c_DefaultTurretRotation;

        if (m_TankRigidbody != null)
        {
            m_TankRigidbody.velocity = Vector3.zero;
            m_TankRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        UpdateModifiers();

        CurrentFireCooldown += Time.deltaTime;

        if (TankIsUpsideDown() || TankIsBelowLevel())
            Death();
    }

    public Quaternion RotateTank(float yRotationValue)
    {
        Quaternion tankRotation = Quaternion.Euler(0f, yRotationValue * TankRotationSpeed * Time.deltaTime, 0f);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * tankRotation);
        return tankRotation;
    }

    public void RotateTankTowards(Quaternion rotationToTarget)
    {
        Quaternion tankRotation = Quaternion.RotateTowards(transform.rotation, rotationToTarget, Time.deltaTime * TankRotationSpeed);
        m_TankRigidbody.MoveRotation(tankRotation);
    }

    public void RotateTurret(float yRotationValue, Quaternion inverseRotation)
    {
        m_TankTurret.transform.localRotation = m_TankTurret.transform.localRotation *
                                               Quaternion.Euler(0f, yRotationValue * c_TurretRotationSpeed * Time.deltaTime, 0f) *
                                               Quaternion.Inverse(inverseRotation);
    }

    public void RotateTurretTowards(Quaternion rotationToTarget)
    {
        Quaternion turretRotation = Quaternion.RotateTowards(m_TankTurret.transform.rotation, rotationToTarget, Time.deltaTime * c_TurretRotationSpeed);
        m_TankTurret.transform.rotation = turretRotation;
    }

    public void RotateBarrel(float xRotationValue)
    {
        Quaternion barrelRotation = m_TankBarrel.transform.localRotation *
                                    Quaternion.Euler(-xRotationValue * c_BarrelRotationSpeed * Time.deltaTime, 0f, 0f);
        m_TankBarrel.transform.localRotation = Quaternion.Euler(ClampAngle(barrelRotation.eulerAngles.x,
                                                                           c_BarrelMinXRotation,
                                                                           c_BarrelMaxXRotation),
                                                                m_TankBarrel.transform.localEulerAngles.y,
                                                                m_TankBarrel.transform.localEulerAngles.z);
    }

    public void RotateBarrelTowards(Quaternion rotationToTarget)
    {
        Quaternion barrelRotation = Quaternion.RotateTowards(m_TankBarrel.transform.localRotation, rotationToTarget, Time.deltaTime * c_BarrelRotationSpeed);
        m_TankBarrel.transform.localRotation = barrelRotation;
    }

    public void MoveTank(float forwardMoveValue = 1f)
    {
        Vector3 movement = transform.forward * forwardMoveValue * Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);
    }

    public void Fire()
    {
        if (CurrentFireCooldown < FireCooldown)
            return;

        Rigidbody shellInstance = Instantiate(ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = ShellVelocity * m_ShellOriginTransform.forward;

        CurrentFireCooldown = 0f;
    }

    public void AddModifier(Modifier modifier)
    {
        m_Modifiers.Add(modifier);
    }

    private void UpdateModifiers()
    {
        ResetModifiers();

        if (m_Modifiers.Count == 0)
            return;

        for (int i = 0; i < m_Modifiers.Count; i++)
            m_Modifiers[i].Apply(this);

        m_Modifiers.RemoveAll(m => m.Duration <= 0f);
    }

    private void ResetModifiers()
    {
        SpeedModifier = 0f;
        TankRotationSpeedModifier = 0f;
        ShellVelocityModifier = 0f;
        FireCooldownModifier = 0f;
    }

    private bool TankIsUpsideDown()
    {
        return Vector3.Dot(transform.up, Vector3.down) >= 0.99f;
    }

    private bool TankIsBelowLevel()
    {
        return transform.position.y < -1f;
    }

    private void UpdateHealthSlider()
    {
        m_HealthSlider.value = (m_CurrentHealth / c_StartingHealth) * 100f;
    }

    private void UpdateCooldownSlider()
    {
        m_CooldownSlider.value = (CurrentFireCooldown / FireCooldown) * 100f;
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
