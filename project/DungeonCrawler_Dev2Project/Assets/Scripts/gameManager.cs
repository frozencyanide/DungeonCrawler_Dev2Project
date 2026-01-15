using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menus")]
    [SerializeField] private GameObject ActiveMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject LossScreen;
    [SerializeField] private GameObject VictoryScreen;

    [Header("Player Reference")]
    public GameObject player;
    public PlayerController pController;

    [Header("UI")]
    public Image playerHPBar;               // Drag the fill Image here in Inspector

    public bool isPaused { get; private set; }

    private float timeScaleOriginal;

    [Header("Win Condition")]
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int initialEnemyCount;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        timeScaleOriginal = Time.timeScale;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                pController = player.GetComponent<PlayerController>();
            }
        }
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
            initialEnemyCount = activeEnemies.Count;
        }
    }

    public void EnemyDied(Enemy enemy)
    {
        if (enemy == null) return;

        activeEnemies.Remove(enemy);

        if (activeEnemies.Count <= 0 && initialEnemyCount > 0)
        {
            VictoryGame();
        }
    }

    public void VictoryGame()
    {
        PauseGame();
        ActiveMenu = VictoryScreen;
        if (ActiveMenu != null) ActiveMenu.SetActive(true);
    }
}