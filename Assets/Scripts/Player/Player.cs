using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hola

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public Material material;
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer weaponSpriteRenderer;
    public GameObject aim;
    public Sprite deathSprite;
    public BoxCollider2D boxCollider;
    [SerializeField] private Weapon weapon;

    public bool attacking = false;
    public bool facingRight = true;
    public bool playerIsDead = false;

    // Properties
    public float OriginalSpeed { get; } = 5f;
    public float Speed { get; set; }
    public int MaxHp { get; private set; } = 100;
    private int _hp;
    public int Hp
    {
        get => _hp;
        private set
        {
            if (value < 0)
            {
                _hp = 0;
            }
            else if (value > MaxHp)
            {
                _hp = MaxHp;
            }
            else
            {
                _hp = value;
            }
        }
    }
    private float _lifeStealChance;
    public float LifeStealChance
    {
        get => _lifeStealChance;
        set
        {
            if (value > 0.5f)
            {
                _lifeStealChance = 0.5f;
            }
            else
            {
                _lifeStealChance = value;
            }
        }
    }

    public HealthBar healthBar;

    private Vector2 movement;

    private bool invincible = false;
    private Color flashColour = new Color(255, 255, 255, 0);
    private Color normalColour = new Color(255, 255, 255, 255);
    private bool firstRoom = true;

    // Modified dot variables
    public GameObject dotPrefab;
    public float dotDistance = 2f;
    public float dotOffsetDistance = 1f;
    private Vector2 lastDotPosition;
    private List<GameObject> dotList = new List<GameObject>();
    private Vector2 lastMovementDirection;
    private bool isBacktracking = false;  // New flag to track backtracking state
    // New variables for smooth backtracking
    public float backtrackSpeed = 10f;  // How fast to move when backtracking
    private bool isMovingToDot = false;  // Are we currently moving to a dot?
    private GameObject targetDot = null;  // The dot we're currently moving towards

    private void Start()
    {
        LoadGameData();
        SetHp(Hp, MaxHp);

        Speed = OriginalSpeed;

        Time.timeScale = 1f;
        this.material.SetColor("_Tint", flashColour);

        // Initialize lastDotPosition to the starting position
        lastDotPosition = rb.position;
        CreateDot(rb.position);

        //AudioManager.instance.Stop("BattleMusic");
        //AudioManager.instance.Stop("PlayerDeath");
        //AudioManager.instance.Play("FirstRoomMusic");
    }

    private void Update()
    {
        playerSpriteRenderer.sortingOrder = (int)(-transform.position.y * 100);
        weaponSpriteRenderer.sortingOrder = (int)(-transform.position.y * 100 - 25);

        if (!playerIsDead)
        {
            // Only process normal movement input if we're not moving to a dot
            if (!isMovingToDot)
            {
                // Movement
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                // If player starts moving in any direction while backtracking
                if (movement.sqrMagnitude > 0)
                {
                    if (dotList.Count > 0)
                    {
                        Vector2 directionToLastDot = ((Vector2)dotList[dotList.Count - 1].transform.position - (Vector2)transform.position).normalized;
                        float dotProduct = Vector2.Dot(movement.normalized, directionToLastDot);
                        
                        if (dotProduct < 0)
                        {
                            isBacktracking = false;
                        }
                    }
                    else
                    {
                        isBacktracking = false;
                    }
                }

                // Only update movement direction if not backtracking
                if (!isBacktracking && movement != Vector2.zero)
                {
                    lastMovementDirection = movement.normalized;
                }

                // Moving animation
                animator.SetBool("Moving", movement.sqrMagnitude != 0);

                // Changing facing direction
                transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);

                // Handle dot creation only when moving forward AND not backtracking
                if (movement.sqrMagnitude > 0 && !isBacktracking)
                {
                    HandleDotCreation();
                }
            }
            else
            {
                // If we're moving to a dot, handle that movement
                HandleBacktrackMovement();
            }
        }
    }
    private void FixedUpdate()
    {
        // Only process normal movement if we're not moving to a dot
        if (!playerIsDead && !isMovingToDot && movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * Speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!playerIsDead)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                Teleport(collision);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Collectables"))
            {
                collision.gameObject.GetComponent<Collectables>().Consume(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerIsDead && collision.CompareTag("Dot"))
        {
            GameObject collidedDot = collision.gameObject;
            int dotIndex = dotList.IndexOf(collidedDot);

            // Check if this is the last dot or if we're already backtracking
            if (dotIndex == dotList.Count - 1 || isBacktracking)
            {
                // If this is a valid dot to backtrack to
                if (dotIndex >= 0 && dotIndex == dotList.Count - 1)
                {
                    // Simply remove the dot - no repositioning
                    dotList.Remove(collidedDot);
                    Destroy(collidedDot);
                    
                    // Update tracking variables
                    if (dotList.Count > 0)
                    {
                        lastDotPosition = dotList[dotList.Count - 1].transform.position;
                    }
                    else
                    {
                        lastDotPosition = rb.position;
                        isBacktracking = false;
                    }
                    
                    isBacktracking = true;
                }
            }
        }
    }

    private void StartBacktrackToTarget(GameObject dot)
    {
        targetDot = dot;
        isMovingToDot = true;
        // Make sure the animator shows movement
        animator.SetBool("Moving", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Optional: If you want to ensure clean transitions between dots
        if (collision.CompareTag("Dot"))
        {
            // You could add additional logic here if needed
        }
    }

    private void Teleport(Collision2D collision)
    {
        Vector2 collisionCoordinates = collision.collider.ClosestPoint(transform.position);
        Vector2 currentRoomCoordinates = RoomController.instance.currentRoomCoordinates;
        int width = RoomController.instance.width;
        int height = RoomController.instance.height;

        Vector3 newPlayerPosition;
        Debug.Log(collisionCoordinates - currentRoomCoordinates * new Vector2(width, height) * 2);

        //if (firstRoom)
        //{
        //    AudioManager.instance.Stop("FirstRoomMusic");
        //    AudioManager.instance.Play("BattleMusic");
        //    firstRoom = false;
        //}

        //  Right door
        if (collisionCoordinates.x - currentRoomCoordinates.x * width * 2 > 3)
        {
            newPlayerPosition = new Vector3(rb.position.x + width + 1, rb.position.y, 0);
            RoomController.instance.currentRoomCoordinates.x++;
        }
        //  Left door
        else if (currentRoomCoordinates.x * width * 2 - collisionCoordinates.x > 3)
        {
            newPlayerPosition = new Vector3(rb.position.x - width - 1, rb.position.y, 0);
            RoomController.instance.currentRoomCoordinates.x--;
        }
        //  Top door
        else if (collisionCoordinates.y - currentRoomCoordinates.y * height * 2 > 3)
        {
            newPlayerPosition = new Vector3(rb.position.x, rb.position.y + height + 3, 0);
            RoomController.instance.currentRoomCoordinates.y++;
        }
        //  Bottom door
        else
        {
            newPlayerPosition = new Vector3(rb.position.x, rb.position.y - height - 3, 0);
            RoomController.instance.currentRoomCoordinates.y--;
        }

        transform.position = newPlayerPosition;

        // Optionally, clear existing dots when teleporting
        ClearAllDots();
    }

    public void SetHp(int newHP, int newMaxHP)
    {
        Hp = newHP;
        MaxHp = newMaxHP;
        healthBar.SetHP(newHP, newMaxHP);
    }

    public void TakeDamage(int enemyDamage, GameObject collisionGameObject)
    {
        if (!invincible && !playerIsDead)
        {
            SetHp(Hp - enemyDamage, MaxHp);

            if (Hp > 0)
            {
                StartCoroutine(TakeDamageAnimation());
            }
            else
            {
                Die(collisionGameObject);
            }
        }
    }

    private IEnumerator TakeDamageAnimation()
    {
        invincible = true;

        animator.SetTrigger("Hit");

        material.SetColor("_Tint", normalColour);
        yield return new WaitForSeconds(0.05f);
        material.SetColor("_Tint", flashColour);

        for (int i = 0; i < 5; i++)
        {
            playerSpriteRenderer.color = flashColour;
            yield return new WaitForSeconds(0.1f);

            playerSpriteRenderer.color = normalColour;
            yield return new WaitForSeconds(0.1f);
        }

        invincible = false;
    }

    //IEnumerator PlayDead()
    //{
    //    AudioManager.instance.Stop("BattleMusic");
    //    yield return new WaitForSeconds(0.1f);
    //    AudioManager.instance.Play("PlayerDeath");
    //}

    private void Die(GameObject collisionGameObject)
    {
        playerIsDead = true;
        animator.enabled = false;
        aim.SetActive(false);
        //StartCoroutine(PlayDead());

        if (facingRight)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 90);
        }
        else
        {
            transform.rotation = Quaternion.Euler(Vector3.back * 90);
        }

        playerSpriteRenderer.color = new Color(0.25f, 0.25f, 0.25f, 1);

        Vector2 knockback = boxCollider.bounds.center - collisionGameObject.GetComponent<BoxCollider2D>().bounds.center;
        knockback = knockback.normalized * 10;
        rb.AddForce(knockback, ForceMode2D.Impulse);
        StartCoroutine(GameOverMenu.instance.GameOver());
    }

    private void LoadGameData()
    {
        GameData data = SaveSystem.LoadGameData();

        if (data == null)
        {
            Hp = MaxHp;
        }
        else
        {
            Hp = data.hp;
            MaxHp = data.maxHp;
        }
    }

    /// <summary>
    /// Handles the creation of dots behind the player at set distance intervals.
    /// </summary>
    private void HandleDotCreation()
    {
        float distance = Vector2.Distance(rb.position, lastDotPosition);
        if (distance >= dotDistance)
        {
            // Add a slight additional offset when moving upwards to prevent immediate collision
            float verticalOffset = movement.y > 0 ? dotOffsetDistance * 1.5f : dotOffsetDistance;
            
            // Calculate position behind the player based on movement direction
            Vector2 dotPosition = (Vector2)rb.position - (lastMovementDirection * verticalOffset);
            
            // Ensure the dot isn't too close to the player's collider
            if (Vector2.Distance(dotPosition, rb.position) > boxCollider.size.y)
            {
                CreateDot(dotPosition);
                lastDotPosition = rb.position;
            }
        }
    }

    private void HandleBacktrackMovement()
    {
        if (targetDot == null)
        {
            isMovingToDot = false;
            return;
        }

        // Calculate direction to the target dot
        Vector2 directionToDot = (targetDot.transform.position - transform.position);
        float distanceToDot = directionToDot.magnitude;

        // If we're close enough to the dot
        if (distanceToDot < 0.1f)
        {
            // Snap to exact position and complete the backtrack
            rb.position = targetDot.transform.position;
            CompleteBacktrack(targetDot);
        }
        else
        {
            // Move towards the dot
            Vector2 normalizedDirection = directionToDot.normalized;
            rb.MovePosition(rb.position + normalizedDirection * backtrackSpeed * Time.fixedDeltaTime);

            // Update facing direction based on movement
            facingRight = normalizedDirection.x > 0;
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }

    private void CompleteBacktrack(GameObject dot)
    {
        // Remove the dot we just reached
        dotList.Remove(dot);
        Destroy(dot);

        // Update the last dot position
        if (dotList.Count > 0)
        {
            lastDotPosition = dotList[dotList.Count - 1].transform.position;
        }
        else
        {
            lastDotPosition = rb.position;
            isBacktracking = false;
        }

        // Reset movement state
        isMovingToDot = false;
        targetDot = null;
        
        // Update animation state
        animator.SetBool("Moving", false);
    }

    /// <summary>
    /// Instantiates a dot at the specified position and adds it to the dot list.
    /// </summary>
    /// <param name="position">The position to place the dot.</param>
    private void CreateDot(Vector2 position)
    {
        if (dotPrefab == null)
        {
            Debug.LogWarning("Dot Prefab is not assigned in the Inspector.");
            return;
        }

        GameObject newDot = Instantiate(dotPrefab, position, Quaternion.identity);
        dotList.Add(newDot);
    }

    /// <summary>
    /// Handles the backtracking functionality by moving the player to the last dot's position.
    /// Triggered when the player collides with a dot.
    /// </summary>
    /// <param name="dot">The dot GameObject collided with.</param>
    private void Backtrack(GameObject dot)
    {
        // Move the player to the dot's position
        rb.position = dot.transform.position;

        // Remove only this dot
        dotList.Remove(dot);
        Destroy(dot);

        // Update the last dot position
        if (dotList.Count > 0)
        {
            lastDotPosition = dotList[dotList.Count - 1].transform.position;
        }
        else
        {
            lastDotPosition = rb.position;
            isBacktracking = false; // Stop backtracking when we run out of dots
        }
    }

    /// <summary>
    /// Clears all existing dots from the scene and the list.
    /// Useful when teleporting or resetting the level.
    /// </summary>
    private void ClearAllDots()
    {
        foreach (GameObject dot in dotList)
        {
            Destroy(dot);
        }
        dotList.Clear();
        lastDotPosition = rb.position;
        isBacktracking = false;
    }
}