using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Material enemyMaterial;


    [Header("----- Stats -----")]
    [SerializeField] private float shootRate = 2f;
    [SerializeField] private int faceTargetSpeed = 5;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float sightDistance = 45f;

    [Header("----- Vision -----")]
    [SerializeField] private Transform headPos;           // Assign head position in Inspector
    [SerializeField] private float fov = 45f;             // Field of view angle

    private float shootTimer;
    private Vector3 playerDirection;
    private float angleToPlayer;
    private bool playerInTrigger;

    void Start()
    {
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

        shootTimer += Time.deltaTime;

        if (playerInTrigger && CanSeePlayer())
        {

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
        maxHealth -= amount;
        if (maxHealth <= 0)
        {
            if (GameManager.instance != null)
                GameManager.instance.EnemyDied(this);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
        }


    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPos.position, playerDirection * sightDistance, Color.white);


        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {



            if (angleToPlayer <= fov && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }


                if (shootTimer >= shootRate)
                {
                    shootTimer = 0f;
                    if (shootPos != null && bulletPrefab != null)
                    {
                        Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
                    }
                    
                }
                return true;
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

    //private void OnTriggerStay(Collider other)
    //{
        //if (other.CompareTag("Player") && playerInTrigger && CanSeePlayer())
        //{
        //    // Enemy sees player → can shoot aggressively
        //    // Add attack logic here if desired
        //} commented out to improve enemyAI
    //}

    IEnumerator FlashRed()
    {
        enemyMaterial.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyMaterial.color = Color.yellow;
    }
}