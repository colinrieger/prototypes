using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace arpg
{
    public class InventoryItemSlot
    {
        public Item Item { get; private set; }

        private GameObject m_ItemGameObject;
        private GameObject m_Joint;

        public InventoryItemSlot(GameObject joint)
        {
            m_Joint = joint;
        }

        public void EquipItem(Item item)
        {
            Item = item;
            m_ItemGameObject = Object.Instantiate(Resources.Load(Item.ItemPrefabPath), m_Joint.transform) as GameObject;
        }

        public void UnequipItem()
        {
            Item = null;
            if (m_ItemGameObject != null)
                Object.Destroy(m_ItemGameObject);
        }
    }

    public class CharacterInventory : MonoBehaviour
    {
        private Dictionary<ItemSlot, InventoryItemSlot> m_InventoryItemSlots = new Dictionary<ItemSlot, InventoryItemSlot>();
        private List<Item> m_BagItems = new List<Item>();

        public void AddInventoryItemSlot(ItemSlot itemSlot, string itemJointPath)
        {
            if (itemSlot == ItemSlot.None)
                return;

            Transform itemJointTransform = transform.Find(itemJointPath);
            if (itemJointTransform == null)
                return;

            m_InventoryItemSlots[itemSlot] = new InventoryItemSlot(itemJointTransform.gameObject);
        }

        public bool EquipItem(Item item)
        {
            if (item == null ||
                item.ItemSlot == ItemSlot.None ||
                !m_InventoryItemSlots.ContainsKey(item.ItemSlot) ||
                m_InventoryItemSlots[item.ItemSlot].Item != null)
                return false;

            m_InventoryItemSlots[item.ItemSlot].EquipItem(item);

            return true;
        }

        public bool UnequipItem(ItemSlot itemSlot)
        {
            if (itemSlot == ItemSlot.None || !m_InventoryItemSlots.ContainsKey(itemSlot))
                return false;

            m_InventoryItemSlots[itemSlot].UnequipItem();

            return true;
        }

        public bool AddItemToBag(Item item)
        {
            if (item == null)
                return false;

            m_BagItems.Add(item);

            return true;
        }

        public bool RemoveItemFromBag(Item item)
        {
            if (item == null)
                return false;

            m_BagItems.Remove(item);

            return true;
        }

        public bool AddItem(int itemId)
        {
            Item item = ItemManager.Instance.GetItem(itemId);
            if (item == null)
                return false;

            return EquipItem(item) || AddItemToBag(item);
        }

        // BroadcastMessage Receiver
        void Death()
        {
            foreach (InventoryItemSlot inventoryItemSlot in m_InventoryItemSlots.Values)
            {
                if (inventoryItemSlot.Item != null)
                {
                    ItemManager.Instance.DropItem(inventoryItemSlot.Item.ItemId, transform);
                    inventoryItemSlot.UnequipItem();
                }
            }
            foreach (Item item in m_BagItems.ToList())
            {
                ItemManager.Instance.DropItem(item.ItemId, transform);
                m_BagItems.Remove(item);
            }
        }
    }
}