using UnityEngine;

namespace arpg
{
    public class ItemPickup : MonoBehaviour
    {
        public int ItemId { get; set; }

        private void Awake()
        {
            gameObject.tag = "Item";
            gameObject.AddComponent<SphereCollider>().radius = 0.3f;
        }
    }
}