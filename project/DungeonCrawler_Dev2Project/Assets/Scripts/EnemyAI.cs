using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] Renderer model;
    [SerializeField] GameObject dropItem;

    //adding stats to determine how far enemy roams and how often they change positions
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTime;


    [Header("----- Stats -----")]
    [SerializeField] private float shootRate = 2f;
    [SerializeField] private int faceTargetSpeed = 5;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float sightDistance = 45f;

    [Header("----- Vision -----")]
    [SerializeField] private Transform headPos;           // Assign head position in Inspector
    [SerializeField] private float fov = 45f;             // Field of view angle

    private float shootTimer;
    private float angleToPlayer;
    private float roamTimer;
    private float originalStoppingDistance;
    private float distanceToPlayer;
   
    private Vector3 playerDirection;
    private Vector3 enemyStartPos;

    private bool playerInTrigger;

    Color OriginalColor;
    //starting position for enemy AI before they roam
    



    void Start()
    {
    
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterEnemy(this);
        }
        originalStoppingDistance = agent.stoppingDistance;
        OriginalColor = model.material.color;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInTrigger && !CanSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInTrigger)
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
        }
        
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;
        Vector3 randPos = Random.insideUnitSphere * roamDistance;
        randPos += enemyStartPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);
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
            {
                GameManager.instance.EnemyDied(this);
              
               
                if(dropItem != null)
                {
                    Instantiate(dropItem, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
                }

                Destroy(gameObject);
            }
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

        if (Physics.Raycast(headPos.position, playerDirection, out hit, sightDistance))
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
                    //shootTimer = 0f;
                    if (shootPos != null && bulletPrefab != null)
                    {
                        //  Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
                        shoot();
                    }
                    
                }
                agent.stoppingDistance = originalStoppingDistance;
                return true;
            }
        
        }
        agent.stoppingDistance = 0;
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
        agent.stoppingDistance = 0;    
    }

    //private void OnTriggerStay(Collider other)
    //{
        //if (other.CompareTag("Player") && playerInTrigger && CanSeePlayer())
        //{
        //    // Enemy sees player → can shoot aggressively
        //    // Add attack logic here if desired
        //} commented out to improve enemyAI
    //}
    void shoot()
    {
        shootTimer = 0;
        Instantiate(bulletPrefab, shootPos.position, transform.rotation);
    }
    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = OriginalColor;
    }
}