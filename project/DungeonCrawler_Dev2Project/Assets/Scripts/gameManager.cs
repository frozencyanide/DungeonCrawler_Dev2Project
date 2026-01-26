using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject ActiveMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject LossScreen;
    [SerializeField] GameObject VictoryScreen;
    public GameObject DamageFlash;

    [Header("Player Reference")]
    public GameObject player;
    public PlayerController pController;
    public GameObject SpawnPoint;

    [Header("UI")]
    public Image playerHPBar;               // Drag the fill Image here in Inspector
    public bool isPaused { get; private set; }
    float timeScaleOriginal;
    public GameObject CheckpointPopUp;
    public GameObject AmmoTextBox;
    public TMP_Text currentAmmoText;
    public TMP_Text maxAmmoText;
    public GameObject ItemPopUp;

    [Header("Win Condition")]
    public List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] TMP_Text GoalMissionText;
    [SerializeField] TMP_Text GoalCountText;
    public GameObject WinArea;
    public GameObject BossDoor;
    public GameObject VictoryDoor;

    void Awake()
    {
     
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (WinArea == null)
        {
            WinArea = GameObject.FindWithTag("WinArea");
        }
        if (BossDoor == null)
        {
            BossDoor = GameObject.FindWithTag("Door1");
        }
        if (VictoryDoor == null)
        {
            VictoryDoor = GameObject.FindWithTag("Door2");
        }
        if(SpawnPoint == null)
        {
            SpawnPoint = GameObject.FindWithTag("SpawnPoint");
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                pController = player.GetComponent<PlayerController>();
            }
        }

        
    }

    private void Start()
    {
        pController.RespawnPlayer();
        Time.timeScale = 1f;
        timeScaleOriginal = Time.timeScale;
    }

    void Update()
    {
     
        if (Input.GetButtonDown("Cancel"))
        {
            if (ActiveMenu == null)
            {
                PauseGame();
                ActiveMenu = PauseMenu;
                if (ActiveMenu != null) ActiveMenu.SetActive(true);
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (ActiveMenu != null)
        {
            ActiveMenu.SetActive(false);
            ActiveMenu = null;
        }
    }

    public void LostGame()
    {
        PauseGame();
        ActiveMenu = LossScreen;
        if (ActiveMenu != null) ActiveMenu.SetActive(true);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
        UpdateGoalCount();
    }

    public void EnemyDied(Enemy enemy)
    {
        if (enemy == null) return;

        activeEnemies.Remove(enemy);
        UpdateGoalCount();
        
    }

    public void VictoryGame()
    {
        PauseGame();
        ActiveMenu = VictoryScreen;
        if (ActiveMenu != null) { ActiveMenu.SetActive(true); }
    }


   public void UpdateGoalCount()
    {
        GoalCountText.text = activeEnemies.Count.ToString();
       
        if (GoalCountText.text == "1")
        {
            GoalMissionText.text = "Defeat the boss!";
            
            BossDoor.SetActive(false);
        } else
        if (GoalCountText.text == "0")
        {
            GoalMissionText.text = "Get to the treasure room!";
            GoalCountText.text = "";
            VictoryDoor.SetActive(false);
        }
        else
        {
            GoalMissionText.text = "Enemies Remaining: ";
        }
    }
}