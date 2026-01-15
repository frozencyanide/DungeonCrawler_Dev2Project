using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{
   
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPosition;

    [SerializeField] int HP;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOriginal;
    float shootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime; 

        if(shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPosition.position, transform.rotation);
    }


public void takeDamage(int amount)
    {
        HP -= amount;

        if(HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

}
