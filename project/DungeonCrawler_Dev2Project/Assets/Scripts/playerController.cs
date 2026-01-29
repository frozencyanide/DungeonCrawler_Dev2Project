using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;

    [Header("----- Physics -----")]
    [SerializeField] GameObject PlayerSpawn;
    [Range(15, 40)][SerializeField] int gravity;

    [Header("----- Weapons -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] ParticleSystem BulletSpark;
    [SerializeField] GameObject powerStoneModel;
    public List<weaponStats> weaponList = new List<weaponStats>();

    [Header("-----Audio-----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] AudJump;
    [Range(0,1)][SerializeField] float audJumpVol;

    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    int jumpCount;
    int HPOriginal;
    float shootTimer;
    float reloadTimer;
    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isPlayingSteps;

    int baseSpeed;
    int weaponListPOS;
    int goldCount;

    void Start()
    {
        baseSpeed = speed;
        HPOriginal = HP;
        RespawnPlayer();
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;
        
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
            playerVel.y = 0;
            if(moveDir.normalized.magnitude > 0.3f && !isPlayingSteps)
            {
                StartCoroutine(playSteps());
            }
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && weaponList.Count != 0 && weaponList[weaponListPOS].currentAmmo > 0 && shootTimer >= shootRate)
        {
            shoot();
        }else if(Input.GetButton("Fire1") && weaponList.Count != 0 && weaponList[weaponListPOS].currentAmmo == 0 && shootTimer >= shootRate)
        {
            reload();
        }else if (Input.GetButtonDown("Reload"))
        {
            reload();
        }

        selectWeapon();
        UpdatePlayerUI();
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
            aud.PlayOneShot(AudJump[Random.Range(0, AudJump.Length)], audJumpVol);
        }
    }

    void sprint()
    {
        speed = Input.GetButton("Sprint") ? baseSpeed * sprintMod : baseSpeed;
        
            isSprinting = true;
        
        
            //isSprinting = false;
    }

    IEnumerator playSteps()
    {
      isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if(isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingSteps = false;
    }

    void shoot()
    {
        shootTimer = 0f;
        RaycastHit hit;

        weaponList[weaponListPOS].currentAmmo--;

        if (Physics.Raycast(Camera.main.transform.position,
                            Camera.main.transform.forward,
                            out hit,
                            shootDist, ~ignoreLayer, QueryTriggerInteraction.Ignore))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            Debug.Log(hit.collider.name);

            Instantiate(BulletSpark, hit.point, Quaternion.identity);

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

        if(GameManager.instance != null && weaponList.Count >= 1)
        {
            GameManager.instance.maxAmmoText.text = weaponList[weaponListPOS].MaxAmmo.ToString();
            GameManager.instance.currentAmmoText.text = weaponList[weaponListPOS].currentAmmo.ToString();
        }
    }

    public void RespawnPlayer()
    {
        HPOriginal = HP;
        controller.transform.position = GameManager.instance.SpawnPoint.transform.position;
        UpdatePlayerUI();
        Physics.SyncTransforms();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        

        if (HP <= 0)
        {
            HP = 0;
            if (GameManager.instance != null)
            {
                GameManager.instance.LostGame();
            }
        }
        else
        {
            StartCoroutine(flashDamage());
        }
    }
    IEnumerator flashDamage()
    {
        GameManager.instance.DamageFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.DamageFlash.SetActive(false);
        
    }

    public void getWeaponStats(weaponStats weapon)
    {
        if (!weaponList.Contains(weapon))
        {
            weaponList.Add(weapon);
            weaponListPOS = weaponList.Count - 1;
        }
        else
        {
            weaponList.Remove(weapon);
            weaponList.Add(weapon);
            weaponListPOS = weaponList.Count - 1;
        }
        changeWeapon();
        UpdatePlayerUI();
    }

    void changeWeapon()
    {
        shootDamage = weaponList[weaponListPOS].staffDamage;
        shootDist = weaponList[weaponListPOS].staffDistance;
        shootRate = weaponList[weaponListPOS].staffFireRate;
        BulletSpark = weaponList[weaponListPOS].hitEffect;

        powerStoneModel.GetComponent<MeshFilter>().sharedMesh = weaponList[weaponListPOS].powerStoneModel.GetComponent<MeshFilter>().sharedMesh;
        powerStoneModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[weaponListPOS].powerStoneModel.GetComponent<MeshRenderer>().sharedMaterial;

       
    }

    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponListPOS < weaponList.Count - 1)
        {
            weaponListPOS++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponListPOS > 0)
        {
            weaponListPOS--;
            changeWeapon();
        }
        else if (Input.GetButtonDown("Weapon1") && weaponList.Count >= 1)
        {
            weaponListPOS = 0;
            changeWeapon();
        }
        else if (Input.GetButtonDown("Weapon2") && weaponList.Count >= 2)
        {
            weaponListPOS = 1;
            changeWeapon();
        }
        else if (Input.GetButtonDown("Weapon3") && weaponList.Count > 3)
        {
            weaponListPOS = 2;
            changeWeapon();
        }
    }

    void reload()
    {
        if (weaponList.Count > 0)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0){
                weaponList[weaponListPOS].currentAmmo = weaponList[weaponListPOS].MaxAmmo;
            }
        }
    }
}