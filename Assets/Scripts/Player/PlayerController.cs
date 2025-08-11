using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Default Weapon - Pistol")]
    public GameObject pistolPrefab;

    [Header("Fire Points")]
    public List<Transform> firePoints = new List<Transform>();
    public Transform firePointUp;
    public Transform firePointDown;
    public Transform firePointLeft;
    public Transform firePointRight;
    public float timeDuration = 60;

    [Header("Animation")]
    public Animator animator;
    public float animationSmoothing = 0.1f;

    private IWeapon currentWeapon;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float nextFireTime;
    private Transform currentFirePoint;
    private Coroutine weaponSwitchCoroutine;
    private Vector2 lastDirection = Vector2.right;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int Die = Animator.StringToHash("Die");

    private SpriteRenderer spriteRenderer;  // Ссылка на SpriteRenderer

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        // Получаем SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on PlayerController!");
        }

        // Инициализируем список firePoints
        firePoints = new List<Transform> { firePointRight, firePointUp, firePointLeft, firePointDown };
        currentFirePoint = firePointRight;

        // Устанавливаем пистолет как оружие по умолчанию
        SetWeapon(pistolPrefab);
    }

    private void Update()
    {
        HandleMovementInput();
        HandleShootingInput();
        UpdateAnimations();
        HandleSpriteFlipping(); // Добавляем функцию для поворота спрайта
    }

    private void HandleSpriteFlipping()
    {
        // Отражаем спрайт по горизонтали в зависимости от направления движения
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Смотрим вправо
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Смотрим влево
        }
        // Если movement.x == 0, не меняем flipX, чтобы персонаж смотрел в последнюю сторону
    }

    private void UpdateAnimations()
    {
        if (movement.magnitude > 0.1f)
        {
            lastDirection = movement.normalized;
        }

        // Плавное изменение параметров анимации
        animator.SetFloat(MoveX, lastDirection.x, animationSmoothing, Time.deltaTime);
        animator.SetFloat(MoveY, lastDirection.y, animationSmoothing, Time.deltaTime);
        animator.SetFloat(Speed, movement.magnitude);
    }

    public void PlayHurtAnimation()
    {
        animator.SetTrigger(Hurt);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    // Установка нового оружия
    public void SetWeapon(GameObject weaponPrefab, float duration = 60f)
    {
        // Удаляем текущее оружие, если оно есть
        if (currentWeapon != null)
        {
            Destroy((currentWeapon as MonoBehaviour)); // Уничтожаем GameObject, на котором висит скрипт оружия
        }

        // Добавляем новое оружие
        GameObject weaponObject = Instantiate(weaponPrefab);
        weaponObject.transform.SetParent(transform); // Важно: делаем оружие дочерним объектом игрока, чтобы оно двигалось вместе с ним
        weaponObject.gameObject.GetComponent<SpriteRenderer>().enabled = false;
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
                SetWeapon(pickup.weaponPrefab, timeDuration);  // Передаем префаб и длительность
                Destroy(collision.gameObject);
            }
        }
    }
}
