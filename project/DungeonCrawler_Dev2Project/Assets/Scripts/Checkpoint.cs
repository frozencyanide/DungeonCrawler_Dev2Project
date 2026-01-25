using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.SpawnPoint.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            StartCoroutine(Pop());
        }
    }

    IEnumerator Pop()
    {
        GameManager.instance.CheckpointPopUp.SetActive(true);
        yield return new WaitForSeconds(1);
        GameManager.instance.CheckpointPopUp.SetActive(false);
    }
}
