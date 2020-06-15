using UnityEngine;

namespace tanks
{
    public class Pickup : MonoBehaviour
    {
        public Modifier Modifier { get; set; }
        public float RespawnTime { get; set; }

        private const float c_RotationSpeed = 45f;

        void Update()
        {
            transform.Rotate(Vector3.up, c_RotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            TankControls tankControls = other.gameObject.GetComponent<TankControls>();
            if (tankControls != null)
            {
                tankControls.AddModifier(Modifier);

                gameObject.SetActive(false);

                Invoke("Respawn", RespawnTime);
            }
        }

        void Respawn()
        {
            gameObject.SetActive(true);
        }
    }
}