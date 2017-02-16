using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControls : MonoBehaviour
{
    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;

    private Rigidbody m_Rigidbody;
    private float m_HorizontalInputValue;
    private float m_VerticalInputValue;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_HorizontalInputValue = 0f;
        m_VerticalInputValue = 0f;
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    void Start ()
    {
    }

    void Update ()
    {
        m_HorizontalInputValue = Input.GetAxis("Horizontal");
        m_VerticalInputValue = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        MoveAndTurn();
    }

    private void MoveAndTurn()
    {
        Vector3 movement = transform.forward * m_VerticalInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        float turn = m_HorizontalInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}
