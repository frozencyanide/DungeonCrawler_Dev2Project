using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifetime;
    [Range(1, 10)][SerializeField] int damage;

    float timer;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}