using UnityEngine;

public class WinArea : MonoBehaviour
{
    public bool playerInTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && GameManager.instance.activeEnemies.Count.ToString() == "0") 
        {
            GameManager.instance.VictoryGame();
        }
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
}
