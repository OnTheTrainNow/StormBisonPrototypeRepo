using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour, IPersistence
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuDied;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuSettingsActive;
    [SerializeField] GameObject menuSettings;

    [SerializeField] TMP_Text weaponEquipped; //weapon equipped text component
    [SerializeField] TMP_Text enemyCountText; //enemy count text component
    [SerializeField] TMP_Text starCountText; //star count text component
    [SerializeField] TMP_Text coinCountText; //coin count text component
    [SerializeField] TMP_Text livesCountText; //lives count text component

    [SerializeField] AudioSource musicMenuAudioSource;

    public TMP_Text waterCountText; //enemy count text component

    public TMP_Text sensitivityTextX; //the text for the horizontal sensitivity
    public Slider sensitivitySliderX; //the slider for the horzontal sensitivity
    public TMP_Text sensitivityTextY; //the text for the vertical sensitivity
    public Slider sensitivitySliderY; //the slider for the vertical sensitivity

    public Image playerHPCircle; //player HP circle image
    public Image playerHPCircleBackground; //player HP circle image background

    public Image playerWaterBar; //player HP circle image

    public GameObject playerDamageFlash; //the damage effect panel
    public GameObject playerSpawnPosition; //the player spawn position

    public GameObject player;
    public PlayerController playerScript; //the player script component

    public bool isPaused;
    public bool gotFinalKey;
    int enemyCount;
    public int coinCount = 0;
    public bool extraLifeEE = false;
    public bool boughtMaxHPUpgrade;
    public bool boughtWaterCapUpgrade;
    public bool boughtWaterRefillEff;
    public bool boughtExtraLife;

    void Awake()
    {
        instance = this;
        loadSettings(); 
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>(); //get the player controller script
        playerSpawnPosition = GameObject.FindWithTag("PlayerSpawnPos"); //get the player spawn position
    }

    public void loadSettings()
    {
        if (PlayerPrefs.HasKey("Mouse Sensitivity X") && PlayerPrefs.HasKey("Mouse Sensitivity Y"))
        {
            sensitivitySliderX.value = PlayerPrefs.GetInt("Mouse Sensitivity X");
            sensitivitySliderY.value = PlayerPrefs.GetInt("Mouse Sensitivity Y");
            sensitivityTextX.text = sensitivitySliderX.value.ToString("F0");
            sensitivityTextY.text = sensitivitySliderY.value.ToString("F0");
        }
    }

    void Update()
    {
        coinCountText.text = coinCount.ToString();
        livesCountText.text = gameManager.instance.playerScript.playerLives.ToString();
        // esc key will bring up Pause Menu, requires button functionality script
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }

        if (extraLifeEE == false && coinCount == 69)
        {
            extraLifeEE = true;
            gameManager.instance.playerScript.playerLives += 1;
        }
    }

    // Pauses the game for the Pause Menu
    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0; // Pauses the game
        Cursor.visible = true; // re-enables cursor
        Cursor.lockState = CursorLockMode.Confined; // confines the cursor to the window
        if (musicMenuAudioSource != null)
        {
            musicMenuAudioSource.Play();
            musicMenuAudioSource.UnPause();
        }
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
        if (menuSettingsActive != null)
        {
            menuSettingsActive.SetActive(false);
            menuSettingsActive = null;
        }

        if (musicMenuAudioSource != null)
        {
            musicMenuAudioSource.Pause();
        }
    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0"); //set the enemy count text

        // Player Win condition is met when all enemies are dead
        if (enemyCount <= 0)
        {
            if (starManager.instance != null)
            {
                starManager.instance.spawnEnemyStar();
            }
        }
    }

    public void youWin()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
        statePaused();
        DataManager.instance.savePlayerData();
    }

    // Player Lose function Will be called
    public void youLose()
    {
        updateCoinUI(-50);
        
        statePaused();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youDied()
    {
        updateCoinUI(-100);
        
        statePaused();
        menuActive = menuDied;
        menuActive.SetActive(true);
        DataManager.instance.savePlayerData();
    }

    public void updateWeaponEquipped()
    {
        if (playerScript.isShotgunEquipped == true)
        {
            weaponEquipped.text = "Shotgun";
        }
        else if (playerScript.isPistolEquipped == true)
        {
            weaponEquipped.text = "Pistol";
        }
        else if (playerScript.isRifleEquipped == true)
        {
            weaponEquipped.text = "Rifle";
        }
    }

    public void updateStarUI()
    {
        starCountText.text = starManager.instance.starCount.ToString();
    }

    public void updateCoinUI(int amount)
    {
        if (coinCount < 50 && amount == -50)
        {
            coinCount = 0;
            coinCountText.text = coinCount.ToString("F0");
        }
        else if (coinCount < 100 && amount == -100)
        {
            coinCount = 0;
            coinCountText.text = coinCount.ToString("F0");
        }
        else
        {
            coinCount += amount;
            coinCountText.text = coinCount.ToString("F0");
        }
    }

    public void loadLobby()
    {
        StartCoroutine(loadLobbyWithDelay());
    }

    IEnumerator loadLobbyWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene("MainLobby");
        gameManager.instance.stateUnpaused();
    }

    public void ShopUI()
    {
        statePaused();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void pauseMenuSettings() 
    {
        menuSettingsActive = menuSettings;
        menuSettings.SetActive(true);
    }

    public void SavePersistentData(ref PlayerSaveData saveData)
    {
        saveData.gotFinalKey = this.gotFinalKey;
        saveData.boughtMaxHPUpgrade = this.boughtMaxHPUpgrade;
        saveData.boughtWaterCapUpgrade = this.boughtWaterCapUpgrade;
        saveData.boughtWaterRefillEff = this.boughtWaterRefillEff;
        saveData.boughtExtraLife = this.boughtExtraLife;
    }

    public void LoadPersistentData(PlayerSaveData saveData)
    {
        this.gotFinalKey = saveData.gotFinalKey;
        this.boughtMaxHPUpgrade = saveData.boughtMaxHPUpgrade;
        this.boughtWaterCapUpgrade = saveData.boughtWaterCapUpgrade;
        this.boughtWaterRefillEff = saveData.boughtWaterRefillEff;
        this.boughtExtraLife = saveData.boughtExtraLife;
    }
}
