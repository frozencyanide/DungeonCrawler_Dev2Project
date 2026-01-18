using System.Threading;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1, 10)][SerializeField] int HP = 5;
    [Range(1, 10)][SerializeField] int speed = 5;
    [Range(2, 5)][SerializeField] int sprintMod = 2;
    [Range(8, 20)][SerializeField] int jumpSpeed = 8;
    [Range(1, 3)][SerializeField] int jumpMax = 1;

    [Header("----- Physics -----")]
    [Range(15, 40)][SerializeField] int gravity = 20;

    [Header("----- Guns -----")]
    [SerializeField] int shootDamage = 1;
    [SerializeField] int shootDist = 50;
    [SerializeField] float shootRate = 0.25f;

    int jumpCount;
    int HPOriginal;
    float shootTimer;
    Vector3 moveDir;
    Vector3 playerVel;

    int baseSpeed;

    void Start()
    {
        baseSpeed = speed;
        HPOriginal = HP;
        UpdatePlayerUI();           // Initialize UI
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;
        Debug.Log($"timer: {shootTimer}  rate: {shootRate}");
        movement();
        sprint();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                    (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);
        controller.Move(playerVel * Time.deltaTime);
        jump();

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y += gravity * Time.deltaTime * -1f;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void sprint()
    {
        speed = Input.GetButton("Sprint") ? baseSpeed * sprintMod : baseSpeed;
    }

    void shoot()
    {
        shootTimer = 0f;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,
                            Camera.main.transform.forward,
                            out hit,
                            shootDist, ~ignoreLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);

            IDamage dmg = hit.collider.GetComponentInParent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void UpdatePlayerUI()
    {
        if (GameManager.instance != null && GameManager.instance.playerHPBar != null)
        {
            GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            HP = 0;
            if (GameManager.instance != null)
            {
                GameManager.instance.LostGame();
            }
        }
    }
    IEnumerator flashDamage()
    {
        GameManager.instance.DamageFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.DamageFlash.SetActive(false);
    }
}