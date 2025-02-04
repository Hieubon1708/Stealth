using UnityEngine;

namespace Hunter
{
    public class WeaponEquip : MonoBehaviour
    {
        public GameObject[] preWeapons;

        public void Equip(Player player, GameController.WeaponType weaponType)
        {
            if (player.weapon != null)
            {
                Destroy(player.weapon.gameObject);
            }
            GameObject w = GetPreWeaponByIndex((int)weaponType - 1);
            if (w)
            {
                GameObject weapon = Instantiate(w, player.hand);
                weapon.transform.localRotation = w.transform.localRotation;
                weapon.transform.localPosition = w.transform.localPosition;
                player.Init(weapon.GetComponent<Weapon>());
            }
            else
            {
                player.attackRangeCollider.radius = 0;
            }
        }

        GameObject GetPreWeaponByIndex(int index)
        {
            for (int i = 0; i < preWeapons.Length; i++)
            {
                if (i == index) return preWeapons[i];
            }
            return null;
        }
    }
}