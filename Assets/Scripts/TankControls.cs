using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ModifierType
{
    None = 0,
    Health,
    Speed,
    Fire
}

class Modifier
{
    public ModifierType modifierType;
    public float duration;
}

public class TankControls : MonoBehaviour
{
    public Rigidbody ShellRigidBody;
    public GameObject ExplosionPrefab;

    public float Speed { get { return m_Speed + m_SpeedModifier; } }
    public float TankRotationSpeed { get { return m_TankRotationSpeed + m_TankRotationSpeedModifier; } }
    public float ShellVelocity { get { return m_ShellVelocity + m_ShellVelocityModifier; } }
    public float FireCooldown { get { return m_FireCooldown - m_FireCooldownModifier; } }

    public float CurrentHealth
    {
        get { return m_CurrentHealth; }
        set
        {
            m_CurrentHealth = Math.Min(m_StartingHealth, Math.Max(0f, value));

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

    private float m_StartingHealth = 100f;
    private float m_CurrentHealth;

    private float m_Speed = 20f;
    private float m_TankRotationSpeed = 90f;
    private float m_TurretRotationSpeed = 180f;

    private float m_BarrelRotationSpeed = 30f;
    private float m_BarrelMinXRotation = -15f;
    private float m_BarrelMaxXRotation = 0f;

    private float m_ShellVelocity = 60f;
    private float m_FireCooldown = 2f; // seconds
    private float m_CurrentFireCooldown;
    
    private float m_SpeedModifier = 0f;
    private float m_TankRotationSpeedModifier = 0f;
    private float m_ShellVelocityModifier = 0f;
    private float m_FireCooldownModifier = 0f;

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
        CurrentHealth = m_StartingHealth;
        CurrentFireCooldown = m_FireCooldown;

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

        if (TankIsUpsideDown())
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

    public void AddModifier(ModifierType modifierType, float modifierDuration)
    {
        m_Modifiers.Add(new Modifier() { modifierType = modifierType, duration = modifierDuration });
    }

    private void UpdateModifiers()
    {
        ResetModifiers();

        if (m_Modifiers.Count == 0)
            return;

        for (int i = 0; i < m_Modifiers.Count; i++)
        {
            Modifier modifier = m_Modifiers[i];

            switch (modifier.modifierType)
            {
                case ModifierType.Health:
                    CurrentHealth += 20f;
                    break;
                case ModifierType.Speed:
                    m_SpeedModifier = 10f;
                    m_TankRotationSpeedModifier = 30f;
                    break;
                case ModifierType.Fire:
                    m_ShellVelocityModifier = 40f;
                    m_FireCooldownModifier = 1f;
                    break;
            }
            modifier.duration -= Time.deltaTime;
        }

        m_Modifiers.RemoveAll(m => m.duration <= 0f);
    }

    private void ResetModifiers()
    {
        m_SpeedModifier = 0f;
        m_TankRotationSpeedModifier = 0f;
        m_ShellVelocityModifier = 0f;
        m_FireCooldownModifier = 0f;
    }

    private bool TankIsUpsideDown()
    {
        return Vector3.Dot(transform.up, Vector3.down) >= 0.99f;
    }

    private void UpdateHealthSlider()
    {
        m_HealthSlider.value = (m_CurrentHealth / m_StartingHealth) * 100f;
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
