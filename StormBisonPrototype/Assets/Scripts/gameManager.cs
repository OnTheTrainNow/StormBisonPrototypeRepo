using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject player;

    public bool isPaused;
    int enemyCount;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // esc key will bring up Pause Menu, requires button functionality script
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    // Pauses the game for the Pause Menu
    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0; // Pauses the game
        Cursor.visible = true; // re-enables cursor
        Cursor.lockState = CursorLockMode.Confined; // confines the cursor to the window
    }

    // Unpauses the game after Resume Gets pressed, requires button functionality
    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1; // UnPauses the game
        Cursor.visible = false; // Hides the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lockes the cursor
        menuActive.SetActive(false); // Sets Menu's active state to false
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        // Player Win condition is met when all enemies are dead
        if (enemyCount <= 0)
        {
            menuActive = menuWin;
            menuActive.SetActive(true);
            statePaused();
        }
    }

    // Player Lose function Will be called
    public void youLose()
    {
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
