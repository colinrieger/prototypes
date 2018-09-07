using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ModifierType
{
    None = 0,
    Health,
    Speed,
    Fire
}

public class Modifier
{
    public ModifierType ModifierType { get; private set; }
    public float Duration { get; private set; }

    public Modifier(ModifierType modifierType, float duration)
    {
        ModifierType = modifierType;
        Duration = duration;
    }

    public Modifier Clone()
    {
        return MemberwiseClone() as Modifier;
    }

    public void Apply(TankControls tankControls)
    {
        switch (ModifierType)
        {
            case ModifierType.Health:
                tankControls.CurrentHealth += 20f;
                break;
            case ModifierType.Speed:
                tankControls.SpeedModifier = 10f;
                tankControls.TankRotationSpeedModifier = 30f;
                break;
            case ModifierType.Fire:
                tankControls.ShellVelocityModifier = 40f;
                tankControls.FireCooldownModifier = 1f;
                break;
        }
        Duration -= Time.deltaTime;
    }
}

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance { get; private set; }

    private struct PickupData
    {
        public Modifier Modifier;
        public string PrefabPath;
        public float RespawnTime;
    }
    private Dictionary<ModifierType, PickupData> m_Pickups = new Dictionary<ModifierType, PickupData>();

    private List<Vector3> m_PickupStartingPositions = new List<Vector3>()
    {
        new Vector3(20f, 2f, 20f),
        new Vector3(20f, 2f, -20f),
        new Vector3(-20f, 2f, 20f),
        new Vector3(-20f, 2f, -20f)
    };
    private List<GameObject> m_CurrentPickups = new List<GameObject>();

    private const float c_NoDuration = 0f;
    private const float c_FireDuration = 8f;
    private const float c_FireRespawnTime = 10f;
    private const float c_HealthDuration = c_NoDuration;
    private const float c_HealthRespawnTime = 10f;
    private const float c_SpeedDuration = 8f;
    private const float c_SpeedRespawnTime = 10f;

    private void Awake()
    {
        Instance = this;

        LoadPickupData();
    }

    private void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    private void LoadPickupData()
    {
        AddPickupData(ModifierType.Fire, c_FireDuration, "Pickups/Fire", c_FireRespawnTime);
        AddPickupData(ModifierType.Health, c_HealthDuration, "Pickups/Health", c_HealthRespawnTime);
        AddPickupData(ModifierType.Speed, c_SpeedDuration, "Pickups/Speed", c_SpeedRespawnTime);
    }

    private void AddPickupData(ModifierType modifierType, float duration, string pickupPrefabPath, float respawnTime)
    {
        PickupData pickupData = new PickupData()
        {
            Modifier = new Modifier(modifierType, duration),
            PrefabPath = pickupPrefabPath,
            RespawnTime = respawnTime
        };
        m_Pickups.Add(modifierType, pickupData);
    }

    public void RandomizePickups()
    {
        foreach (GameObject pickup in m_CurrentPickups.ToList())
        {
            m_CurrentPickups.Remove(pickup);
            Destroy(pickup);
        }

        foreach (Vector3 pickupStartingPosition in m_PickupStartingPositions)
        {
            PickupData pickupData = GetRandomPickupData();
            GameObject pickupObject = Instantiate(Resources.Load(pickupData.PrefabPath), pickupStartingPosition, new Quaternion()) as GameObject;

            Pickup pickup = pickupObject.GetComponent<Pickup>();
            pickup.Modifier = pickupData.Modifier.Clone();
            pickup.RespawnTime = pickupData.RespawnTime;

            m_CurrentPickups.Add(pickupObject);
        }
    }

    private PickupData GetRandomPickupData()
    {
        int minModifierType = System.Enum.GetValues(typeof(ModifierType)).Cast<int>().Min() + 1; // skip None
        int maxModifierType = System.Enum.GetValues(typeof(ModifierType)).Cast<int>().Max();
        ModifierType modifierType = (ModifierType)Random.Range(minModifierType, maxModifierType + 1);

        return m_Pickups[modifierType];
    }
}