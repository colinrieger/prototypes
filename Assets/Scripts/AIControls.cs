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
    private Quaternion m_RotationToNavDestination;
    private Quaternion m_RotationToTargetTank;

    private const float c_MaxAngleToTargetToMove = 10f;
    private const float c_MaxAngleToTargetToFire = 1f;
    private const float c_DistanceToTargetOffset = 20f;
    private const float c_ShellRadius = 0.2f;

    private void Awake()
    {
        m_TankControls = GetComponent<TankControls>();
        m_TankTurret = transform.Find("Renderers/Turret").gameObject;
        m_TankBarrel = transform.Find("Renderers/Turret/Barrel").gameObject;
    }

    private void Start()
    {
        m_TankBarrel.transform.localEulerAngles = new Vector3(-2f, 0f, 0f);

        m_NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        m_NavMeshAgent.speed = m_TankControls.Speed;
        m_NavMeshAgent.angularSpeed = m_TankControls.TankRotationSpeed;
        m_NavMeshAgent.autoBraking = false;
        m_NavMeshAgent.updatePosition = false;
        m_NavMeshAgent.updateRotation = false;
    }

    private void FixedUpdate()
    {
        if (!HaveTargetTank())
            return;
        
        m_NavMeshAgent.destination = TargetTank.transform.position;
        UpdateRotationTargets();

        m_TankControls.RotateTankTowards(m_RotationToNavDestination);

        if (TankShouldMove())
            m_TankControls.MoveTank();

        m_TankControls.RotateTurretTowards(m_RotationToTargetTank);

        if (TankShouldFire())
            m_TankControls.Fire();
    }

    private void UpdateRotationTargets()
    {
        Vector3 targetNavDirection = m_NavMeshAgent.nextPosition - transform.position;
        m_RotationToNavDestination = (targetNavDirection != Vector3.zero) ? Quaternion.LookRotation(targetNavDirection) : new Quaternion();
        m_RotationToNavDestination.x = 0;
        m_RotationToNavDestination.z = 0;

        Vector3 targetTankDirection = (m_TargetTankTurret.transform.position - m_TankTurret.transform.position);
        m_RotationToTargetTank = (targetTankDirection != Vector3.zero) ? Quaternion.LookRotation(targetTankDirection) : new Quaternion();
        m_RotationToTargetTank.x = 0;
        m_RotationToTargetTank.z = 0;
    }

    private bool TankShouldMove()
    {
        float distanceToTarget = Vector3.Distance(transform.position, TargetTank.transform.position);
        float tankToTargetAngle = Quaternion.Angle(transform.rotation, m_RotationToNavDestination);

        return (distanceToTarget > c_DistanceToTargetOffset) && (tankToTargetAngle < c_MaxAngleToTargetToMove);
    }

    private bool TankShouldFire()
    {
        if (Quaternion.Angle(m_TankTurret.transform.rotation, m_RotationToTargetTank) > c_MaxAngleToTargetToFire)
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
