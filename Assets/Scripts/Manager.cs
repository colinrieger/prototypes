using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public CameraControls m_CameraControls;
    public GameObject m_TankPrefab;
	
    private void Start()
    {
        SpawnPlayerTank();
        SpawnAITank();
    }
    
    private void SpawnPlayerTank()
    {
        GameObject tank = Instantiate(m_TankPrefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
        m_CameraControls.m_Target = tank.transform.Find("Renderers/Turret").gameObject;
    }

    private void SpawnAITank()
    {
        for (int i = 0; i < 1; i++)
        {
            GameObject tank = Instantiate(m_TankPrefab, new Vector3(0f, 0f, 10f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
            tank.GetComponent<TankControls>().m_IsAI = true;
        }
    }
}