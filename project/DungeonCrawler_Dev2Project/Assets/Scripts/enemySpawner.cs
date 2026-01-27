using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [Range(1, 10)][SerializeField] int spawnTotalAmount;
    [Range(1, 3)][SerializeField] int perSpawnAmount;
    [Range(0.1f, 20)][SerializeField] float spawnRate;

    int spawnCount;
    
    float spawnTimer;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.UpdateGoalCount(spawnTotalAmount * perSpawnAmount);
       
    }

    // Update is called once per frame
    void Update()
    {
        if(startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if(spawnCount < spawnTotalAmount && spawnTimer >= spawnRate)
            {
                spawn();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;   
        }
    }

    void spawn()
    {
        spawnTimer = 0;
        spawnCount++;
        for (int i = 0; i < perSpawnAmount; i++) {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }

}
