using UnityEngine;
using UnityEngine.AI;

public class AIControls : MonoBehaviour
{
    public GameObject TargetTank
    {
        get { return m_TargetTank; }
        set
        {
            m_TargetTank = value;
            m_TargetTankTurret = TargetTank.transform.Find("Renderers/Turret").gameObject;
        }
    }

    private GameObject m_TargetTank;
    private GameObject m_TargetTankTurret;
    
    private TankControls m_TankControls;
    private GameObject m_TankTurret;
    private GameObject m_TankBarrel;
    private NavMeshAgent m_NavMeshAgent;
    private Quaternion m_TankRotationToNavDestination;
    private Quaternion m_TurretRotationToTargetTank;
    private Quaternion m_BarrelRotationToTargetTank;

    private const float c_MaxAngleToTargetToMove = 10f;
    private const float c_MaxAngleToTargetToFire = 1f;
    private const float c_DistanceToTargetOffset = 20f;
    private const float c_ShellRadius = 0.2f;

    private readonly Vector3 c_DefaultBarrelRotation = new Vector3(-2f, 0f, 0f);

    private void Awake()
    {
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
        m_TankBarrel = transform.Find("Renderers/Turret/Barrel").gameObject;
    }

    private void Start()
    {
        m_TankBarrel.transform.localEulerAngles = c_DefaultBarrelRotation;

        m_NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        m_NavMeshAgent.autoBraking = false;
        m_NavMeshAgent.updatePosition = false;
        m_NavMeshAgent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        if (!HaveTargetTank())
            return;

        m_NavMeshAgent.speed = m_TankControls.Speed;
        m_NavMeshAgent.angularSpeed = m_TankControls.TankRotationSpeed;

        m_NavMeshAgent.destination = TargetTank.transform.position;

        UpdateRotationTargets();
        m_TankControls.RotateTankTowards(m_TankRotationToNavDestination);
        m_TankControls.RotateTurretTowards(m_TurretRotationToTargetTank);
        m_TankControls.RotateBarrelTowards(m_BarrelRotationToTargetTank);

        if (TankShouldMove())
            m_TankControls.MoveTank();

        if (TankShouldFire())
            m_TankControls.Fire();
    }

    private void UpdateRotationTargets()
    {
        Vector3 targetNavDirection = m_NavMeshAgent.nextPosition - transform.position;
        m_TankRotationToNavDestination = (targetNavDirection != Vector3.zero) ? Quaternion.LookRotation(targetNavDirection) : new Quaternion();
        m_TankRotationToNavDestination.x = 0;
        m_TankRotationToNavDestination.z = 0;

        Vector3 targetTankDirection = (m_TargetTankTurret.transform.position - m_TankTurret.transform.position);
        m_TurretRotationToTargetTank = (targetTankDirection != Vector3.zero) ? Quaternion.LookRotation(targetTankDirection) : new Quaternion();
        m_TurretRotationToTargetTank.x = 0;
        m_TurretRotationToTargetTank.z = 0;
        
        float distance = targetTankDirection.magnitude;
        float v = m_TankControls.ShellVelocity;
        float angle = 0.5f * (Mathf.Asin((-Physics.gravity.y * distance) / (v * v)));
        m_BarrelRotationToTargetTank = Quaternion.Euler(new Vector3(-angle * Mathf.Rad2Deg, 0f, 0f));
    }

    private bool TankShouldMove()
    {
        float distanceToTarget = Vector3.Distance(transform.position, TargetTank.transform.position);
        float tankToTargetAngle = Quaternion.Angle(transform.rotation, m_TankRotationToNavDestination);

        return (distanceToTarget > c_DistanceToTargetOffset) && (tankToTargetAngle < c_MaxAngleToTargetToMove);
    }

    private bool TankShouldFire()
    {
        if (Quaternion.Angle(m_TankTurret.transform.rotation, m_TurretRotationToTargetTank) > c_MaxAngleToTargetToFire)
            return false;

        RaycastHit hit;
        Vector3 targetDirection = (m_TargetTankTurret.transform.position - m_TankTurret.transform.position);
        return Physics.SphereCast(m_TankTurret.transform.position, c_ShellRadius, targetDirection, out hit) && hit.transform == TargetTank.transform;
    }

    private bool HaveTargetTank()
    {
        return (TargetTank != null) && TargetTank.activeSelf;
    }
}
