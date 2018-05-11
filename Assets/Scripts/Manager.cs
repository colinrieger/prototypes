using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public GameObject TankPrefab;
    public GameObject PauseMenu;
    public Text GameText;

    private GameObject m_PlayerTank;
    private List<GameObject> m_Tanks = new List<GameObject>();

    private int m_RoundNumber = 0;
    private int m_PlayerWins = 0;
    private int m_AIWins = 0;

    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;

    private const float c_StartDelay = 3f;
    private const float c_EndDelay = 3f;
    private const float c_RoundsToWin = 3f;

    struct StartingTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private List<StartingTransform> m_StartingTransforms = new List<StartingTransform>()
    {
        new StartingTransform() { position = new Vector3(0f, 0.1f, 0f), rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f)) },
        new StartingTransform() { position = new Vector3(40f, 0.1f, 40f), rotation = Quaternion.Euler(new Vector3(0f, 225f, 0f)) },
        new StartingTransform() { position = new Vector3(40f, 0.1f, -40f), rotation = Quaternion.Euler(new Vector3(0f, 315f, 0f)) },
        new StartingTransform() { position = new Vector3(-40f, 0.1f, 40f), rotation = Quaternion.Euler(new Vector3(0f, 135f, 0f)) },
        new StartingTransform() { position = new Vector3(-40f, 0.1f, -40f), rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f)) }
    };

    private List<int> m_StartingTransformIndexes;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        m_StartWait = new WaitForSeconds(c_StartDelay);
        m_EndWait = new WaitForSeconds(c_EndDelay);

        SpawnPlayerTank();
        SpawnAITank();
        
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(StartRound());

        yield return StartCoroutine(PlayRound());

        yield return StartCoroutine(EndRound());

        if (GameOver())
            SceneManager.LoadScene("Main");
        else
            StartCoroutine(GameLoop());
    }

    private IEnumerator StartRound()
    {
        SetControlsEnabled(false);
        ResetTanks();

        m_RoundNumber++;
        GameText.text = string.Format("Round {0}", m_RoundNumber);

        yield return m_StartWait;
    }

    private IEnumerator PlayRound()
    {
        SetControlsEnabled(true);

        GameText.text = string.Empty;

        while (!RoundComplete())
            yield return null;
    }

    private IEnumerator EndRound()
    {
        SetControlsEnabled(false);

        if (m_PlayerTank.activeSelf)
            m_PlayerWins++;
        else
            m_AIWins++;

        GameText.text = string.Format("Player: {0}    AI: {1}", m_PlayerWins, m_AIWins);
        if (GameOver())
            GameText.text += string.Format("\n\n{0} Wins", PlayerWon() ? "Player" : "AI");

        yield return m_EndWait;
    }

    private void TogglePause()
    {
        bool pause = Time.timeScale != 0;

        PauseMenu.SetActive(pause);
        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = pause ? 0 : 1;
    }

    private void GenerateStartingTransformIndexes()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        m_StartingTransformIndexes = new List<int>();
        for (int i = 0; i < m_StartingTransforms.Count; i++)
            m_StartingTransformIndexes.Insert(Random.Range(0, m_StartingTransformIndexes.Count + 1), i++);
    }

    private int GetStartingTransformIndex()
    {
        int startingTransformindex = m_StartingTransformIndexes.Count > 0 ? m_StartingTransformIndexes[0] : 0;
        m_StartingTransformIndexes.RemoveAt(0);
        return startingTransformindex;
    }

    private void SpawnPlayerTank()
    {
        m_PlayerTank = Instantiate(TankPrefab) as GameObject;
        Camera.main.GetComponent<CameraControls>().Target = m_PlayerTank.transform.Find("Renderers/Turret/CameraTargetTransform").gameObject;
        m_PlayerTank.AddComponent<PlayerControls>();
        m_Tanks.Add(m_PlayerTank);
    }

    private void SpawnAITank()
    {
        GameObject aiTank = Instantiate(TankPrefab) as GameObject;
        aiTank.AddComponent<AIControls>().TargetTank = m_PlayerTank;
        aiTank.GetComponent<TankControls>().ShellVelocity = 100f;
        m_Tanks.Add(aiTank);
    }

    private void RandomlyPlaceTank(GameObject tank)
    {
        int startingIndex = GetStartingTransformIndex();
        tank.transform.position = m_StartingTransforms[startingIndex].position;
        tank.transform.rotation = m_StartingTransforms[startingIndex].rotation;
        tank.transform.Find("Renderers/Turret").transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    private void ResetTanks()
    {
        GenerateStartingTransformIndexes();

        foreach (GameObject tank in m_Tanks)
        {
            tank.SetActive(false);
            tank.SetActive(true);
            RandomlyPlaceTank(tank);
        }
    }

    private bool RoundComplete()
    {
        int tanksActive = 0;

        foreach (GameObject tank in m_Tanks)
            if (tank.activeSelf)
                tanksActive++;
        
        return tanksActive <= 1;
    }

    private bool GameOver()
    {
        return PlayerWon() || AIWon();
    }

    private bool PlayerWon()
    {
        return m_PlayerWins == c_RoundsToWin;
    }

    private bool AIWon()
    {
        return m_AIWins == c_RoundsToWin;
    }

    private void SetControlsEnabled(bool enabled)
    {
        foreach (GameObject tank in m_Tanks)
        {
            if (tank.GetComponent<PlayerControls>() != null)
                tank.GetComponent<PlayerControls>().enabled = enabled;
            else if (tank.GetComponent<AIControls>() != null)
                tank.GetComponent<AIControls>().enabled = enabled;
        }
    }
}