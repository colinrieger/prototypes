using System.Collections.Generic;
using UnityEngine;

namespace arpg
{
    public enum ItemSlot
    {
        None = 0,
        Head,
        Sword,
        Shield
    }

    public class Item
    {
        public int ItemId { get; private set; }
        public ItemSlot ItemSlot { get; private set; }
        public string ItemPrefabPath { get; private set; }

        public Item(int itemId, ItemSlot itemSlot, string itemPrefabPath)
        {
            ItemId = itemId;
            ItemSlot = itemSlot;
            ItemPrefabPath = itemPrefabPath;
        }
    }

    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }

        private Dictionary<int, Item> m_Items = new Dictionary<int, Item>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            int itemId = 0;
            m_Items.Add(itemId, new Item(itemId++, ItemSlot.Head, "Items/helmet"));
            m_Items.Add(itemId, new Item(itemId++, ItemSlot.Sword, "Items/sword"));
            m_Items.Add(itemId, new Item(itemId++, ItemSlot.Shield, "Items/shield"));
        }

        private void Start()
        {
            Random.InitState(System.DateTime.Now.Millisecond);
        }

        public Item GetItem(int itemId)
        {
            return m_Items.ContainsKey(itemId) ? m_Items[itemId] : null;
        }

        public void DropItem(int itemId, Transform transform)
        {
            PlaceItem(itemId, transform.position + GetRandomPositionOffset());
        }

        public void PlaceItem(int itemId, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion())
        {
            Item item = Instance.GetItem(itemId);
            if (item != null)
            {
                GameObject itemObject = Instantiate(Resources.Load(item.ItemPrefabPath), position, rotation) as GameObject;
                itemObject.AddComponent<ItemPickup>().ItemId = itemId;
            }
        }

        private Vector3 GetRandomPositionOffset()
        {
            float[] offsets = new float[] { -1f, -0.5f, -0.75f, 0f, 0.5f, 0.75f, 1f };
            Vector3 positionOffset = new Vector3();
            positionOffset.y = 0.1f;
            positionOffset.x = offsets[Random.Range(0, offsets.Length)];
            positionOffset.z = offsets[Random.Range(0, offsets.Length)];

            return positionOffset;
        }
    }
}