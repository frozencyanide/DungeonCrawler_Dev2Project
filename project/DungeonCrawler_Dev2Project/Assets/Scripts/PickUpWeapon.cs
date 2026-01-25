using UnityEngine;

using System.Collections;
public class PickUpWeapon : MonoBehaviour
{
    [Header("Weapon")] 
    [SerializeField] weaponStats Weapon;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();
        GameManager.instance.AmmoTextBox.SetActive(true);
        if (pick != null)
        {
            Weapon.currentAmmo = Weapon.MaxAmmo;
            pick.getWeaponStats(Weapon);
            Destroy(gameObject);
            StartCoroutine(PickPop());
        }
    }

    IEnumerator PickPop()
    {
        GameManager.instance.ItemPopUp.SetActive(true);
        yield return new WaitForSeconds(1);
        GameManager.instance.ItemPopUp.SetActive(false);
    }
}
