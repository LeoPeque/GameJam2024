using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wogol : Enemy
{
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject particleSystemPrefab;

    private Player player;
    private float stoppingRadius = 1f;
    private float triggerRadius = 2f;
    private float damageRadius = 10f;
    private bool exploding = false;

    private float timeSinceLastAttackAttempt = 0f;
    private float attackCooldown = 5f; // Time in seconds before switching targets
    private Transform currentTarget;
    private BoxCollider2D targetBoxCollider;
    private bool isExploding = false;


    protected override void Start()
    {
        base.Start();
        player = GameObject.Find("Player").GetComponent<Player>();
        currentTarget = player.transform;
        targetBoxCollider = currentTarget.GetComponent<BoxCollider2D>();
        Hp = 2;
        Damage = 20;
        Speed = 3;
    }

    protected override void Update()
    {
        base.Update();

        if (!isExploding)
        {
            // Update time since last attack attempt
            timeSinceLastAttackAttempt += Time.deltaTime;

            // Check if we need to switch targets
            if (timeSinceLastAttackAttempt >= attackCooldown)
            {
                // Find the nearest LineCollider
                GameObject[] lineColliders = GameObject.FindGameObjectsWithTag("LineColliders");
                if (lineColliders.Length > 0)
                {
                    currentTarget = FindNearestLineCollider(lineColliders);
                    targetBoxCollider = currentTarget.GetComponent<BoxCollider2D>();
                }
            }
            else
            {
                // Target the Player
                currentTarget = player.transform;
                targetBoxCollider = currentTarget.GetComponent<BoxCollider2D>();
            }
        }

        Attack(player); // Pass the player parameter
    }


    private Transform FindNearestLineCollider(GameObject[] lineColliders)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject lineCollider in lineColliders)
        {
            float distance = Vector3.Distance(lineCollider.transform.position, currentPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = lineCollider.transform;
            }
        }
        return nearest;
    }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    protected override void OnCollisionStay2D(Collision2D collision) { }

    public override void Move(Vector2 force)
    {
        if (Vector2.Distance(boxCollider.bounds.center, targetBoxCollider.bounds.center) > stoppingRadius)
        {
            rb.MovePosition(rb.position + force);
        }
    }

    protected override void Attack(Player player)
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetBoxCollider.bounds.center);

        if (distanceToTarget <= triggerRadius && !isExploding)
        {
            isExploding = true;
            StartCoroutine(Explosion());
        }
    }


    private IEnumerator Explosion()
    {
        for (int i = 0; i < 10; i++)
        {
            materialTintColour = new Color(218 / 255f, 78 / 255f, 55 / 255f, 1);
            this.enemySpriteRenderer.material.SetColor("_Tint", materialTintColour);

            yield return new WaitForSeconds(0.1f);

            materialTintColour = new Color(1, 1, 1, 0);
            this.enemySpriteRenderer.material.SetColor("_Tint", materialTintColour);

            yield return new WaitForSeconds(0.1f);
        }

        // Check if either the player or line collider is within damage radius
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < damageRadius)
        {
            player.TakeDamage(Damage, this.gameObject);
            timeSinceLastAttackAttempt = 0f; // Reset timer
        }

        // Explosion visual effects
        GameObject emptyGameObject = new GameObject();
        emptyGameObject.transform.position = this.transform.position;
        GameObject particleSystem = Instantiate(particleSystemPrefab, emptyGameObject.transform, false);
        particleSystem.GetComponent<ParticleSystem>().Play();
        Destroy(emptyGameObject, 0.75f);

        currentRoom.RemoveEnemy();

        Destroy(this.gameObject);
    }

    protected override void Die(Vector2 knockback, Player player)
    {
        base.Die(knockback, player);
        StopCoroutine("Explosion");
    }
}
