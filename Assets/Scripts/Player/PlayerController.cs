using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Default Weapon - Pistol")]
    public GameObject pistolPrefab; // Ссылка на префаб пистолета

    private IWeapon currentWeapon; // Текущее оружие

    [Header("Fire Points")]
    public Transform firePointUp;
    public Transform firePointDown;
    public Transform firePointLeft;
    public Transform firePointRight;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float nextFireTime;
    private Transform currentFirePoint;
    private Coroutine weaponSwitchCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        currentFirePoint = firePointRight;

        // Устанавливаем пистолет как оружие по умолчанию
        SetWeapon(pistolPrefab);
    }

    private void Update()
    {
        HandleMovementInput();
        HandleShootingInput();
    }

    // Установка нового оружия
    public void SetWeapon(GameObject weaponPrefab, float duration = 0f)
    {
        // Удаляем текущее оружие, если оно есть
        if (currentWeapon != null)
        {
            Destroy((currentWeapon as MonoBehaviour)); // Уничтожаем GameObject, на котором висит скрипт оружия
        }

        // Добавляем новое оружие
        GameObject weaponObject = Instantiate(weaponPrefab);
        weaponObject.transform.SetParent(transform); // Важно: делаем оружие дочерним объектом игрока, чтобы оно двигалось вместе с ним
        weaponObject.transform.localPosition = Vector3.zero; //  Устанавливаем позицию относительно игрока

        currentWeapon = weaponObject.GetComponent<IWeapon>();

        if (currentWeapon == null)
        {
            Debug.LogError("Weapon prefab does not implement IWeapon interface!");
            return;
        }

        // Останавливаем корутину переключения оружия
        if (weaponSwitchCoroutine != null)
        {
            StopCoroutine(weaponSwitchCoroutine);
        }
        if (duration > 0)
        {
            weaponSwitchCoroutine = StartCoroutine(SwitchToDefaultWeaponAfterDelay(duration));
        }
    }

    // Возврат к пистолету через время
    private IEnumerator SwitchToDefaultWeaponAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Предполагаем, что у вас есть префаб пистолета, который можно добавить
        SetWeapon(pistolPrefab); // Вместо параметров - префаб
    }

    private void HandleMovementInput()
    {
        movement = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) movement.y = 1;
        if (Input.GetKey(KeyCode.S)) movement.y = -1;
        if (Input.GetKey(KeyCode.A)) movement.x = -1;
        if (Input.GetKey(KeyCode.D)) movement.x = 1;
        movement = movement.normalized;
    }

    private void HandleShootingInput()
    {
        Vector2 shootDirection = Vector2.zero;
        bool isShooting = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            shootDirection = Vector2.up;
            currentFirePoint = firePointUp;
            isShooting = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            shootDirection = Vector2.down;
            currentFirePoint = firePointDown;
            isShooting = true;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            shootDirection = Vector2.left;
            currentFirePoint = firePointLeft;
            isShooting = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            shootDirection = Vector2.right;
            currentFirePoint = firePointRight;
            isShooting = true;
        }

        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot(shootDirection);
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (currentWeapon != null && Time.time >= nextFireTime)
        {
            currentWeapon.Fire(direction, currentFirePoint);
            nextFireTime = Time.time + currentWeapon.FireRate;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WeaponPickup"))
        {
            WeaponPickup pickup = collision.GetComponent<WeaponPickup>();
            if (pickup != null)
            {
                SetWeapon(pickup.weaponPrefab, 60f);  // Передаем префаб и длительность
                Destroy(collision.gameObject);
            }
        }
    }
}
