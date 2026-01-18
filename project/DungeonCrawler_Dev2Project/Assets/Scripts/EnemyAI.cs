using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletPrefab;

    [Header("----- Stats -----")]
    [SerializeField] private float shootRate = 2f;
    [SerializeField] private int faceTargetSpeed = 5;
    [SerializeField] private int maxHealth = 10;

    [Header("----- Vision -----")]
    [SerializeField] private Transform headPos;           // Assign head position in Inspector
    [SerializeField] private float fov = 45f;             // Field of view angle

    private float shootTimer;
    private Vector3 playerDirection;
    private int currentHealth;
    private bool playerInTrigger;

    void Start()
    {
        currentHealth = maxHealth;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterEnemy(this);
        }
    }

    void Update()
    {
        if (GameManager.instance.player == null) return;

        playerDirection = GameManager.instance.player.transform.position - transform.position;

        agent.SetDestination(GameManager.instance.player.transform.position);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shootTimer = 0f;
            if (shootPos != null && bulletPrefab != null)
            {
                Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
            }
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
        }
    }

    private void FaceTarget()
    {
        Vector3 flatDir = new Vector3(playerDirection.x, 0f, playerDirection.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * faceTargetSpeed);
        }
    }

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.EnemyDied(this);
            Destroy(gameObject);
        }
    }

    private bool CanSeePlayer()
    {
        if (headPos == null) return false;

        Vector3 direction = GameManager.instance.player.transform.position - headPos.position;
        Vector3 normDir = direction.normalized;

        Debug.DrawRay(headPos.position, normDir * 20f, Color.white);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, normDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                float angle = Vector3.Angle(transform.forward, normDir);
                if (angle < fov / 2f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInTrigger && CanSeePlayer())
        {
            // Enemy sees player → can shoot aggressively
            // Add attack logic here if desired
        }
    }
}