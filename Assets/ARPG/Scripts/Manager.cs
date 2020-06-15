using UnityEngine;

namespace arpg
{
    public class Manager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject ZombiePrefab;

        private void Start()
        {
            SpawnPlayer();
            SpawnZombie();

            ItemManager.Instance.PlaceItem(1);
        }

        private void SpawnPlayer()
        {
            GameObject player = Instantiate(PlayerPrefab, new Vector3(-4f, 0f, -4f), Quaternion.Euler(new Vector3(0f, 45f, 0f))) as GameObject;
            CharacterInventory characterInventory = player.GetComponent<CharacterInventory>();

            characterInventory.AddInventoryItemSlot(ItemSlot.Head, "Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End/Helmet_joint");
            characterInventory.AddInventoryItemSlot(ItemSlot.Sword, "Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/Sword_joint");
            characterInventory.AddInventoryItemSlot(ItemSlot.Shield, "Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/Shield_joint");
        }

        private void SpawnZombie()
        {
            GameObject zombie = Instantiate(ZombiePrefab, new Vector3(4f, 0f, 4f), Quaternion.Euler(new Vector3(0f, -135f, 0f))) as GameObject;
            CharacterInventory characterInventory = zombie.GetComponent<CharacterInventory>();

            characterInventory.AddInventoryItemSlot(ItemSlot.Head, "Zombie:Hips/Zombie:Spine/Zombie:Spine1/Zombie:Spine2/Zombie:Neck/Zombie:Neck1/Zombie:Head/Zombie:HeadTop_End/Helmet_joint");

            characterInventory.AddItem(0);
            characterInventory.AddItem(2);
        }
    }
}