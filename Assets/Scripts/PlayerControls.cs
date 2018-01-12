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
    private LineRenderer m_FiringArc;

    private const int c_FiringArcPositions = 1000;

    private void Start()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
        m_TankBarrel = m_TankTurret.transform.Find("Barrel").gameObject;
        m_FiringArc = m_TankBarrel.transform.Find("ShellOriginTransform/FiringArc").GetComponent<LineRenderer>();
        
        m_FiringArc.positionCount = c_FiringArcPositions;
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
                                    Quaternion.Euler(-m_MouseYInputValue * m_TankControls.m_BarrelRotationSpeed * Time.deltaTime, 0f, 0f);
        m_TankBarrel.transform.rotation = Quaternion.Euler(ClampAngle(barrelRotation.eulerAngles.x,
                                                                      m_TankControls.m_BarrelMinXRotation,
                                                                      m_TankControls.m_BarrelMaxXRotation),
                                                           barrelRotation.eulerAngles.y,
                                                           barrelRotation.eulerAngles.z);

        Vector3 movement = transform.forward * m_VerticalInputValue * m_TankControls.m_Speed * Time.deltaTime;
        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);

        UpdateFiringArc();
    }

    private void UpdateFiringArc()
    {
        Vector3[] arcArray = new Vector3[c_FiringArcPositions + 1];
        
        float fireAngle = m_TankBarrel.transform.rotation.eulerAngles.x;
        if (fireAngle > 180)
            fireAngle -= 360;
        float radianAngle = Mathf.Deg2Rad * -fireAngle;
        float v = m_TankControls.m_ShellVelocity;

        for (int i = 0; i <= c_FiringArcPositions; i++)
        {
            float t = i * Time.fixedDeltaTime;
            float z = v * t * Mathf.Cos(radianAngle);
            float y = v * t * Mathf.Sin(radianAngle) - ((-Physics.gravity.y * t * t) / 2);
            arcArray[i] = new Vector3(0f, y, z);
        }

        m_FiringArc.SetPositions(arcArray);
        // arc positions are calculated locally, so counter the x barrel rotation on the firing arc
        m_FiringArc.transform.localRotation = Quaternion.Euler(-m_TankBarrel.transform.localEulerAngles.x, m_TankBarrel.transform.localEulerAngles.y, m_TankBarrel.transform.localEulerAngles.z);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180)
            return Math.Max(angle - 360, min) + 360;
        return Math.Min(angle, max);
    }
}
