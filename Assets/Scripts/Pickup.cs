using UnityEngine;

public class Pickup : MonoBehaviour
{
    public ModifierType ModifierType;
    public float ModifierDuration;
    public float RespawnTime;
    
    private float m_RotationSpeed = 45f;
    
    void Update()
    {
        transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankControls tankControls = other.gameObject.GetComponent<TankControls>();
        if (tankControls != null)
        {
            tankControls.AddModifier(ModifierType, ModifierDuration);

            gameObject.SetActive(false);

            Invoke("Respawn", RespawnTime);
        }
    }

    void Respawn()
    {
        gameObject.SetActive(true);
    }
}
