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



    // New fields for line rendering
    private LineRenderer pathLineRenderer;
    public Color lineColor = Color.white;
    public float lineWidth = 0.1f;

    // New fields to control line rendering position
    [Header("Line Rendering Settings")]
    public string lineSortingLayerName = "Player"; // Set this in inspector to match your sorting layer
    public int lineOrderInLayer = -1; // Negative to render behind player, positive to render in front
    public float lineHeightOffset = 0f; // Offset in Z-axis to control if line renders above/below ground

    // New field for the blue gradient
    [Header("Gradient Settings")]
    public Gradient blueGradient;
    private List<EdgeCollider2D> pathColliders = new List<EdgeCollider2D>();
    public LayerMask collisionLayer; // Set this to "Enemy" layer in Inspector

    private void Start()
    {
        LoadGameData();
        SetHp(Hp, MaxHp);

        Speed = OriginalSpeed;

        Time.timeScale = 1f;
        this.material.SetColor("_Tint", flashColour);
        
        

        //AudioManager.instance.Stop("BattleMusic");
        //AudioManager.instance.Stop("PlayerDeath");
        //AudioManager.instance.Play("FirstRoomMusic");

<<<<<<< Updated upstream


=======
        // Initialize LineRenderer with configurable rendering settings
        pathLineRenderer = gameObject.AddComponent<LineRenderer>();
        pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        // Remove startColor and endColor since we're using a gradient
        // pathLineRenderer.startColor = lineColor;
        // pathLineRenderer.endColor = lineColor;
        pathLineRenderer.startWidth = lineWidth;
        pathLineRenderer.endWidth = lineWidth;
        
        // Set the sorting layer and order
        pathLineRenderer.sortingLayerName = lineSortingLayerName;
        pathLineRenderer.sortingOrder = lineOrderInLayer;
        pathLineRenderer.useWorldSpace = true;

        // Assign the gradient to the LineRenderer
        pathLineRenderer.colorGradient = blueGradient;
    }
    
>>>>>>> Stashed changes
    private void Update()
    {
        playerSpriteRenderer.sortingOrder = (int)(-transform.position.y * 100);
        weaponSpriteRenderer.sortingOrder = (int)(-transform.position.y * 100 - 25);

        if (!playerIsDead)
        {
            // Movement
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Moving animation
            if (movement.sqrMagnitude == 0)
            {
                animator.SetBool("Moving", false);
            }
            else
            {
                animator.SetBool("Moving", true);
            }

            // Changing facing direction
            if (facingRight)
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        UpdatePathVisualization();
    }
<<<<<<< Updated upstream

=======

    private void UpdatePathVisualization()
    {
        if (dotList.Count == 0)
        {
            pathLineRenderer.positionCount = 0;
            ClearPathColliders();
            return;
        }

        // Set the number of points in the line renderer
        pathLineRenderer.positionCount = dotList.Count + 1;

        // Calculate the total number of segments for gradient
        int totalSegments = dotList.Count + 1;

        // Create or update list of points for visualization and collision
        List<Vector2> pathPoints = new List<Vector2>();

        // Add all dot positions
        for (int i = 0; i < dotList.Count; i++)
        {
            Vector3 dotPosition = dotList[i].transform.position;
            // Apply height offset for visual line only
            dotPosition.z = lineHeightOffset;
            pathLineRenderer.SetPosition(i, dotPosition);
            pathPoints.Add(dotList[i].transform.position);

            // Calculate gradient color
            float t = (float)i / (totalSegments - 1);
            Color dotColor = blueGradient.Evaluate(t);

            // Assign color to the dot's SpriteRenderer
            SpriteRenderer dotSprite = dotList[i].GetComponent<SpriteRenderer>();
            if (dotSprite != null)
            {
                dotSprite.color = dotColor;
            }
        }

        // Add player position as last point
        Vector3 playerPos = transform.position;
        playerPos.z = lineHeightOffset;
        pathLineRenderer.SetPosition(dotList.Count, playerPos);
        pathPoints.Add(transform.position);

        // Update colliders for path segments
        UpdatePathColliders(pathPoints);

        // Update the LineRenderer's color gradient
        pathLineRenderer.colorGradient = blueGradient;
    }

    // Add these new methods to handle the colliders:
    private void UpdatePathColliders(List<Vector2> pathPoints)
    {
        // Clear excess colliders if we have more than we need
        while (pathColliders.Count > pathPoints.Count - 1)
        {
            if (pathColliders[pathColliders.Count - 1] != null)
            {
                Destroy(pathColliders[pathColliders.Count - 1].gameObject);
            }
            pathColliders.RemoveAt(pathColliders.Count - 1);
        }

        // Create or update colliders for each segment
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            EdgeCollider2D collider;
            
            if (i >= pathColliders.Count)
            {
                // Create new GameObject for the collider
                GameObject colliderObj = new GameObject($"PathCollider_{i}");
                colliderObj.transform.position = Vector3.zero;
                collider = colliderObj.AddComponent<EdgeCollider2D>();
                
                // Put the collider in a specific layer (create this layer in Unity)
                colliderObj.layer = LayerMask.NameToLayer("LineColliders");
                
                // Set the tag
                colliderObj.tag = "LineColliders";
                
                // Configure the collision detection to only affect enemies
                Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), true); // Ignore player collision
                
                pathColliders.Add(collider);
            }
            else
            {
                collider = pathColliders[i];
            }

            // Update collider points using world space coordinates
            Vector2[] points = new Vector2[2];
            points[0] = pathPoints[i];
            points[1] = pathPoints[i + 1];
            collider.points = points;
        }
    }

    private void ClearPathColliders()
    {
        foreach (var collider in pathColliders)
        {
            if (collider != null)
            {
                Destroy(collider.gameObject);
            }
        }
        pathColliders.Clear();
    }
>>>>>>> Stashed changes


    private void FixedUpdate()
    {
        // Movement
        if (!playerIsDead & movement != Vector2.zero)
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

            if (Hp > 0) {
                StartCoroutine(TakeDamageAnimation());
            }
            else {
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
<<<<<<< Updated upstream
}
=======

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

        // Assign color based on the gradient
        int index = dotList.Count - 1;
        float t = (float)index / Mathf.Max(1, (dotList.Count + 1));
        Color dotColor = blueGradient.Evaluate(t);

        SpriteRenderer dotSprite = newDot.GetComponent<SpriteRenderer>();
        if (dotSprite != null)
        {
            dotSprite.color = dotColor;
        }
        else
        {
            Debug.LogWarning("Dot Prefab does not have a SpriteRenderer component.");
        }
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
        
        // Clear the line renderer
        pathLineRenderer.positionCount = 0;
        
        // Clear the colliders
        ClearPathColliders();
    }

    // Update OnDestroy to clean up colliders as well:
    private void OnDestroy()
    {
        // Clean up the line renderer material
        if (pathLineRenderer != null && pathLineRenderer.material != null)
        {
            Destroy(pathLineRenderer.material);
        }
        
        // Clean up colliders
        ClearPathColliders();
    }
}
>>>>>>> Stashed changes
