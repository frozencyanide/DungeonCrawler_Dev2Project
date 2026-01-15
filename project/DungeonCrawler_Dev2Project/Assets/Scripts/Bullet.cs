using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damage = 10;

    private float timer;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}