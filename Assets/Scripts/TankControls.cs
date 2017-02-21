using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankControls : MonoBehaviour
{
    public float m_StartingHealth = 100f;
    public float m_Speed = 16f;
    public float m_TankTurnSpeed = 180f;
    public float m_TurretTurnSpeed = 180f;
    public float m_ShellForce = 100f;
    public Rigidbody m_ShellRigidBody;
    public Transform m_ShellOriginTransform;
    public Transform m_TurretTranform;
    public Slider m_HealthSlider;

    private float m_CurrentHealth;
    private Rigidbody m_TankRigidbody;
    private float m_HorizontalInputValue;
    private float m_VerticalInputValue;
    private float m_MouseXInputValue;

    private void Awake()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_TankRigidbody.isKinematic = false;
        m_HorizontalInputValue = 0f;
        m_VerticalInputValue = 0f;
        m_MouseXInputValue = 0f;

        UpdateHealthSlider();
    }

    private void OnDisable()
    {
        m_TankRigidbody.isKinematic = true;
    }

    void Start ()
    {
    }

    void Update ()
    {
        m_HorizontalInputValue = Input.GetAxis("Horizontal");
        m_VerticalInputValue = Input.GetAxis("Vertical");
        m_MouseXInputValue = Input.GetAxis("Mouse X");

        if (Input.GetButtonDown("Fire1"))
            Fire();
    }

    private void FixedUpdate()
    {
        MoveAndTurn();
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

    private void MoveAndTurn()
    {
        Vector3 movement = transform.forward * m_VerticalInputValue * m_Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);

        float tankTurn = m_HorizontalInputValue * m_TankTurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, tankTurn, 0f);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * turnRotation);

        float turretTurn = m_MouseXInputValue * m_TurretTurnSpeed * Time.deltaTime;
        m_TurretTranform.Rotate(0, turretTurn - tankTurn, 0);
    }

    private void Fire()
    {
        Rigidbody shellInstance = Instantiate(m_ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellForce * m_ShellOriginTransform.forward;
    }

    private void Death()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
