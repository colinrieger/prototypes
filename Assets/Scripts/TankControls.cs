using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControls : MonoBehaviour
{
    public float m_Speed = 16f;
    public float m_TurnSpeed = 180f;
    public float m_ShellForce = 100f;
    public Rigidbody m_ShellRigidBody;
    public Transform m_ShellOriginTransform;

    private Rigidbody m_TankRigidbody;
    private float m_HorizontalInputValue;
    private float m_VerticalInputValue;

    private void Awake()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_TankRigidbody.isKinematic = false;
        m_HorizontalInputValue = 0f;
        m_VerticalInputValue = 0f;
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

        if (Input.GetButtonDown("Fire1"))
            Fire();
    }

    private void FixedUpdate()
    {
        MoveAndTurn();
    }

    private void MoveAndTurn()
    {
        Vector3 movement = transform.forward * m_VerticalInputValue * m_Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);

        float turn = m_HorizontalInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * turnRotation);
    }

    private void Fire()
    {
        Rigidbody shellInstance = Instantiate(m_ShellRigidBody, m_ShellOriginTransform.position, m_ShellOriginTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_ShellForce * m_ShellOriginTransform.forward;
    }
}
