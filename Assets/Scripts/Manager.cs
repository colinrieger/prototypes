using UnityEngine;
using UnityEngine.AI;

public class Manager : MonoBehaviour
{
    public GameObject m_TankPrefab;
    public Transform m_PlayerSpawn;
    public Transform m_AISpawn;
    public GameObject m_PauseMenu;

    private GameObject m_PlayerTank;
    private GameObject m_AITank;

    private bool m_IsPaused = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SpawnPlayerTank();
        SpawnAITank();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_IsPaused = !m_IsPaused;
            Time.timeScale = m_IsPaused ? 0 : 1;
            m_PauseMenu.SetActive(m_IsPaused);
            Cursor.visible = m_IsPaused;
            Cursor.lockState = m_IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private void SpawnPlayerTank()
    {
        m_PlayerTank = Instantiate(m_TankPrefab, m_PlayerSpawn.position, m_PlayerSpawn.rotation) as GameObject;
        Camera.main.GetComponent<CameraControls>().m_Target = m_PlayerTank.transform.Find("Renderers/Turret/CameraTargetTransform").gameObject;
        m_PlayerTank.AddComponent<PlayerControls>();
    }

    private void SpawnAITank()
    {
        m_AITank = Instantiate(m_TankPrefab, m_AISpawn.position, m_AISpawn.rotation) as GameObject;
        m_AITank.AddComponent<AIControls>().m_TargetTank = m_PlayerTank;
        m_AITank.AddComponent<NavMeshAgent>();
    }
}