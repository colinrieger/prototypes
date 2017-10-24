using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject m_TankPrefab;

    private GameObject m_PlayerTank;
	
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SpawnPlayerTank();
        SpawnAITank();
    }

    private void SpawnPlayerTank()
    {
        m_PlayerTank = Instantiate(m_TankPrefab, new Vector3(-40f, 0f, -40f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
        Camera.main.GetComponent<CameraControls>().m_Target = m_PlayerTank.transform.Find("Renderers/Turret").gameObject;
        m_PlayerTank.AddComponent<PlayerControls>();
    }

    private void SpawnAITank()
    {
        GameObject tank = Instantiate(m_TankPrefab, new Vector3(40f, 0f, 40f), new Quaternion(0f, 180f, 0f, 0f)) as GameObject;
        tank.AddComponent<AIControls>().m_TargetTank = m_PlayerTank;
    }
}