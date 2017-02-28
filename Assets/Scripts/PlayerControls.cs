using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private float m_VerticalInputValue;
    private float m_HorizontalInputValue;
    private float m_MouseXInputValue;

    private Rigidbody m_TankRigidbody;
    private TankControls m_TankControls;
    private GameObject m_TankTurret;

    private void Start()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
    }

    void Update()
    {
        m_VerticalInputValue = Input.GetAxis("Vertical");
        m_HorizontalInputValue = Input.GetAxis("Horizontal");
        m_MouseXInputValue = Input.GetAxis("Mouse X");

        if (Input.GetButtonDown("Fire1"))
            m_TankControls.Fire();
    }

    private void FixedUpdate()
    {
        Vector3 movement = transform.forward * m_VerticalInputValue * m_TankControls.m_Speed * Time.deltaTime;
        Quaternion tankRotation = Quaternion.Euler(0f, m_HorizontalInputValue * m_TankControls.m_TankRotationSpeed * Time.deltaTime, 0f);
        Quaternion turretRotation = Quaternion.Euler(0f, m_MouseXInputValue * m_TankControls.m_TurretRotationSpeed * Time.deltaTime, 0f);

        m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);
        m_TankRigidbody.MoveRotation(m_TankRigidbody.rotation * tankRotation);
        m_TankTurret.transform.rotation = m_TankTurret.transform.rotation * turretRotation;
    }
}
