using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Загружаем спрайт из префаба оружия (если у оружия есть спрайт)
        if (weaponPrefab != null)
        {
            IWeapon weapon = weaponPrefab.GetComponent<IWeapon>();
            if (weapon != null)
            {
                SpriteRenderer weaponSpriteRenderer = weaponPrefab.GetComponent<SpriteRenderer>();
                if (weaponSpriteRenderer != null)
                {
                    spriteRenderer.sprite = weaponSpriteRenderer.sprite;
                }
            }
            else
            {
                Debug.LogError("Weapon prefab does not implement IWeapon interface!");
            }
        }
        else
        {
            Debug.LogError("Weapon prefab is not assigned!");
        }
    }
}
