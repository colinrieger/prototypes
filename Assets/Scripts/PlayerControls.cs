using System;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private float m_VerticalInputValue;
    private float m_HorizontalInputValue;
    private float m_MouseYInputValue;
    private float m_MouseXInputValue;

    private Rigidbody m_TankRigidbody;
    private TankControls m_TankControls;
    private GameObject m_TankTurret;
    private GameObject m_TankBarrel;

    private void Start()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
        m_TankBarrel = transform.Find("Renderers/Turret/Barrel").gameObject;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        m_VerticalInputValue = Input.GetAxis("Vertical");
        m_HorizontalInputValue = Input.GetAxis("Horizontal");
        m_MouseYInputValue = Input.GetAxis("Mouse Y");
        m_MouseXInputValue = Input.GetAxis("Mouse X");

        if (Input.GetButtonDown("Fire1"))
            m_TankControls.Fire();
    }

    private void FixedUpdate()
    {
        Quaternion tankRotation = Quaternion.Euler(0f, m_HorizontalInputValue * m_TankControls.m_TankRotationSpeed * Time.deltaTime, 0f);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * tankRotation);
        
        m_TankTurret.transform.rotation = m_TankTurret.transform.rotation *
                                          Quaternion.Euler(0f, m_MouseXInputValue * m_TankControls.m_TurretRotationSpeed * Time.deltaTime, 0f) *
                                          Quaternion.Inverse(tankRotation);

        Quaternion barrelRotation = m_TankBarrel.transform.rotation *
                                    Quaternion.Euler(-m_MouseYInputValue * m_TankControls.m_BarrelRotationSpeed * Time.deltaTime,0f, 0f);
        m_TankBarrel.transform.rotation = Quaternion.Euler(ClampAngle(barrelRotation.eulerAngles.x,
                                                                      m_TankControls.m_BarrelMinXRotation,
                                                                      m_TankControls.m_BarrelMaxXRotation),
                                                           barrelRotation.eulerAngles.y,
                                                           barrelRotation.eulerAngles.z);

        Vector3 movement = transform.forward * m_VerticalInputValue * m_TankControls.m_Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180)
            return Math.Max(angle - 360, min) + 360;
        return Math.Min(angle, max);
    }
}
