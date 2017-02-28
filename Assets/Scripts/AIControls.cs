using UnityEngine;
using UnityEngine.UI;

public class AIControls : MonoBehaviour
{
    public GameObject m_TargetTank;

    private Rigidbody m_TankRigidbody;
    private TankControls m_TankControls;
    private GameObject m_TankTurret;

    private const float c_MaxAngleToTargetToMove = 10f;
    private const float c_DistanceToTargetOffset = 20f;
    private const float c_MaxAngleToTargetToFire = 1f;

    private void Start()
    {
        m_TankRigidbody = GetComponent<Rigidbody>();
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
    }

    private void FixedUpdate()
    {
        Vector3 targetDirection = (m_TargetTank.transform.position - transform.position).normalized;
        Quaternion rotationToTarget = Quaternion.LookRotation(targetDirection);

        Vector3 movement = transform.forward * m_TankControls.m_Speed * Time.deltaTime;
        Quaternion tankRotation = Quaternion.RotateTowards(transform.rotation, rotationToTarget, Time.deltaTime * m_TankControls.m_TankRotationSpeed);
        Quaternion turretRotation = Quaternion.RotateTowards(m_TankTurret.transform.rotation, rotationToTarget, Time.deltaTime * m_TankControls.m_TurretRotationSpeed);

        float distanceToTarget = Vector3.Distance(transform.position, m_TargetTank.transform.position);
        float tankToTargetAngle = Quaternion.Angle(rotationToTarget, tankRotation);
        float turretToTargetAngle = Quaternion.Angle(rotationToTarget, turretRotation);
        
        if (tankToTargetAngle < c_MaxAngleToTargetToMove && distanceToTarget > c_DistanceToTargetOffset)
            m_TankRigidbody.MovePosition(m_TankRigidbody.position + movement);

        m_TankRigidbody.MoveRotation(tankRotation);
        m_TankTurret.transform.rotation = turretRotation;

        if (turretToTargetAngle < c_MaxAngleToTargetToFire && m_TargetTank.activeSelf)
            m_TankControls.Fire();
    }
}
