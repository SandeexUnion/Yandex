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

    [Header("Quad Shot")]
    public bool isQuadShotActive = false;

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
    public bool isFireRateWasChange = false;

    // ПЕРЕМЕННЫЕ ДЛЯ ХРАНЕНИЯ ОРУЖИЯ
    public IWeapon currentWeapon;          // Интерфейс оружия (для логики стрельбы)
    private GameObject currentWeaponObject; // GameObject текущего оружия (ссылка)

    private SpriteRenderer spriteRenderer;  // Ссылка на SpriteRenderer
    public GameObject CurrentWeaponObject => currentWeaponObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        HandleSpriteFlipping();
    }

    private void HandleSpriteFlipping()
    {
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimations()
    {
        if (movement.magnitude > 0.1f)
        {
            lastDirection = movement.normalized;
        }

        animator.SetFloat(MoveX, lastDirection.x, animationSmoothing, Time.deltaTime);
        animator.SetFloat(MoveY, lastDirection.y, animationSmoothing, Time.deltaTime);
        animator.SetFloat(Speed, movement.magnitude);
    }

    public void PlayHurtAnimation()
    {
        animator.SetTrigger(Hurt);
    }

    public void SetInputEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
        SetInputEnabled(false);
        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;
    }

    // Установка нового оружия
    public void SetWeapon(GameObject weaponPrefab, float duration = 60f)
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeapon = null;
            currentWeaponObject = null;
        }

        currentWeaponObject = Instantiate(weaponPrefab);
        currentWeaponObject.transform.SetParent(transform);
        currentWeaponObject.transform.localPosition = Vector3.zero;

        if (isFireRateWasChange)
        {
            currentWeaponObject.GetComponent<Weapon>().fireRate /= 2f;
        }

        SpriteRenderer weaponSprite = currentWeaponObject.GetComponent<SpriteRenderer>();
        if (weaponSprite != null)
        {
            weaponSprite.enabled = false;
        }

        currentWeapon = currentWeaponObject.GetComponent<IWeapon>();

        if (currentWeapon == null)
        {
            Debug.LogError("Weapon prefab does not implement IWeapon interface!");
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            return;
        }

        Debug.Log($"Weapon changed to: {currentWeaponObject.name}");

        if (weaponSwitchCoroutine != null)
        {
            StopCoroutine(weaponSwitchCoroutine);
        }

        if (duration > 0)
        {
            weaponSwitchCoroutine = StartCoroutine(SwitchToDefaultWeaponAfterDelay(duration));
        }
    }

    private IEnumerator SwitchToDefaultWeaponAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentWeaponObject != null && currentWeaponObject.name != pistolPrefab.name + "(Clone)")
        {
            SetWeapon(pistolPrefab);
            Debug.Log("Returned to default pistol");
        }
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
            if (isQuadShotActive)
            {
                ShootInAllDirections();
            }
            else
            {
                Shoot(shootDirection);
            }
        }
    }

    private void ShootInAllDirections()
    {
        if (currentWeapon != null && Time.time >= nextFireTime)
        {
            // Для дробовика используем специальную логику
            if (currentWeapon is ShotgunWeapon shotgun)
            {
                // Дробовик стреляет веером в каждом направлении
                shotgun.FireMultiple(Vector2.up, firePointUp, shotgun.bulletCount, shotgun.spreadAngle);
                shotgun.FireMultiple(Vector2.down, firePointDown, shotgun.bulletCount, shotgun.spreadAngle);
                shotgun.FireMultiple(Vector2.left, firePointLeft, shotgun.bulletCount, shotgun.spreadAngle);
                shotgun.FireMultiple(Vector2.right, firePointRight, shotgun.bulletCount, shotgun.spreadAngle);

                // Muzzle flash для всех точек
                shotgun.DisplayMuzzleFlash(firePointUp);
                shotgun.DisplayMuzzleFlash(firePointDown);
                shotgun.DisplayMuzzleFlash(firePointLeft);
                shotgun.DisplayMuzzleFlash(firePointRight);
            }
            else
            {
                // Для обычного оружия просто создаем снаряды во всех направлениях
                CreateProjectileForDirection(Vector2.up, firePointUp);
                CreateProjectileForDirection(Vector2.down, firePointDown);
                CreateProjectileForDirection(Vector2.left, firePointLeft);
                CreateProjectileForDirection(Vector2.right, firePointRight);
            }

            nextFireTime = Time.time + currentWeapon.FireRate;
        }
    }

    private void CreateProjectileForDirection(Vector2 direction, Transform firePoint)
    {
        Weapon weapon = currentWeapon as Weapon;
        if (weapon != null)
        {
            GameObject projectile = Instantiate(weapon.projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * weapon.projectileSpeed;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Destroy(projectile, weapon.projectileLifetime);

            Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (projectileCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(projectileCollider, playerCollider);
            }

            if (weapon.muzzleFlashPrefab != null)
            {
                GameObject muzzleFlash = Instantiate(weapon.muzzleFlashPrefab, firePoint.position, firePoint.rotation);
                Destroy(muzzleFlash, 0.1f);
            }
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

    // Метод для активации QuadShot из PowerUpManager
    public void ActivateQuadShot(float duration)
    {
        if (!isQuadShotActive)
        {
            StartCoroutine(QuadShotCoroutine(duration));
        }
    }

    private IEnumerator QuadShotCoroutine(float duration)
    {
        isQuadShotActive = true;
        Debug.Log("QuadShot activated!");

        yield return new WaitForSeconds(duration);

        isQuadShotActive = false;
        Debug.Log("QuadShot deactivated!");
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
                SetWeapon(pickup.weaponPrefab, timeDuration);
                Destroy(collision.gameObject);
            }
        }
    }
}