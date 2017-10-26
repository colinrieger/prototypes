using UnityEngine;
using UnityEngine.AI;

public class Manager : MonoBehaviour
{
    public GameObject m_TankPrefab;
    public Transform m_PlayerSpawn;
    public Transform m_AISpawn;

    private GameObject m_PlayerTank;
    private GameObject m_AITank;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SpawnPlayerTank();
        SpawnAITank();
    }

    private void SpawnPlayerTank()
    {
        m_PlayerTank = Instantiate(m_TankPrefab, m_PlayerSpawn.position, m_PlayerSpawn.rotation) as GameObject;
        Camera.main.GetComponent<CameraControls>().m_Target = m_PlayerTank.transform.Find("Renderers/Turret").gameObject;
        m_PlayerTank.AddComponent<PlayerControls>();
    }

    private void SpawnAITank()
    {
        m_AITank = Instantiate(m_TankPrefab, m_AISpawn.position, m_AISpawn.rotation) as GameObject;
        m_AITank.AddComponent<AIControls>().m_TargetTank = m_PlayerTank;
        m_AITank.AddComponent<NavMeshAgent>();
    }
}