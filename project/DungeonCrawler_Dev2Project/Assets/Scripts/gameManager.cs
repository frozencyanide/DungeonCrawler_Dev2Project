using System.Xml.Serialization;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [SerializeField] GameObject ActiveMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject LossScreen;
    [SerializeField] GameObject VictoryScreen;

    public bool isPaused;
    public GameObject player;
    public playerController pController;

    float timeScaleOriginal;

    int goalCount;

    public static gameManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOriginal = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        pController = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {

            if (ActiveMenu == null)
            {
                PauseGame();
                ActiveMenu = PauseMenu;
                ActiveMenu.SetActive(true);
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
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        ActiveMenu.SetActive(false);
        ActiveMenu = null;
    }

    public void LostGame()
    {
        PauseGame();
        ActiveMenu = LossScreen;
        ActiveMenu.SetActive(true);
    }

   public void updateGameGoal(int amount)
    {
        goalCount += amount;
        if(goalCount <=0)
        {
            //you Win!
            PauseGame();
            ActiveMenu = VictoryScreen;
            ActiveMenu.SetActive(true);
        }
    }

}
