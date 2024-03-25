using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class buttonFunctions : MonoBehaviour
{
    //[Header("Volume Settings")]
    //[SerializeField] private TMP_Text volumeTextValue = null;
    //[SerializeField] private Slider volumeSlider = null;
    //[SerializeField] private float defaultVolume = 0.2f;

    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    //[Header("Graphics Settings")]
    //[SerializeField] private Slider brightnessSlider = null;
    //[SerializeField] private TMP_Text brightnessTextValue = null;
    //[SerializeField] private float defaultBrightness = 4;

    //[Space(10)]
    //[SerializeField] private TMP_Dropdown qualityDropdown;
    //[SerializeField] private Toggle fullscreenToggle;

    //private int qualityLevel;
    //private bool isFullscreen;
    //private float brightnessLevel;

    [Header("Levels To Load")]
    public string _newGameLevel;
    //private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public AudioSource sfxPreviewSource;
    public AudioClip sfxPreview;

    public AudioSource menuSFXSource;
    public AudioClip menuSFX;

    private void Start()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
        }

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameDialogYes()
    {
        DataManager.instance.forceNewGameFile();
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        /*if(PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }*/
        if (System.IO.File.Exists(DataManager.instance.getFullFilePath()))
        {
            SceneManager.LoadScene("MainLobby");
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void resume()
    {
        onClickSFX();
        gameManager.instance.stateUnpaused();

        PlayerPrefs.SetInt("Mouse Sensitivity X", (int)gameManager.instance.sensitivitySliderX.value);
        PlayerPrefs.SetInt("Mouse Sensitivity Y", (int)gameManager.instance.sensitivitySliderY.value);
    }

    public void restart()
    {
        onClickSFX();
        StartCoroutine(restartWithDelay());
    }

    IEnumerator restartWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void quit()
    {
        onClickSFX();
        Application.Quit();
    }

    public void respawn()
    {
        onClickSFX();
        if (gameManager.instance.playerScript.playerLives > 0)
        {
            gameManager.instance.stateUnpaused(); //unpause the game
            gameManager.instance.playerScript.respawn(); //call the players respawn function
        }
    }

    public void mouseSensitivity()
    {
        gameManager.instance.sensitivityTextX.text = gameManager.instance.sensitivitySliderX.value.ToString("F0");
        gameManager.instance.sensitivityTextY.text = gameManager.instance.sensitivitySliderY.value.ToString("F0");
    }

    /*
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }*/

    /*
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }
    */

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if(invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    //public void SetBrightness(float brightness)
    //{
    //    brightnessLevel = brightness;
    //    brightnessTextValue.text = brightness.ToString("0");
    //}

    //public void SetFullscreen(bool fullscreen)
    //{
    //    isFullscreen = fullscreen;
    //}

    //public void SetQuality(int qualityIndex)
    //{
    //    qualityLevel = qualityIndex;
    //}

    public void GraphicsApply()
    {
        //PlayerPrefs.SetFloat("MasterBrightness", brightnessLevel);
        //PlayerPrefs.SetInt("MasterQuality", qualityLevel);
        //QualitySettings.SetQualityLevel(qualityLevel);
        //PlayerPrefs.SetInt("MasterFullscreen", (isFullscreen ? 1 : 0));
        //Screen.fullScreen = isFullscreen;

        Resolution currentResolution = Screen.currentResolution;
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);

        StartCoroutine(ConfirmationBox());
    }

    public void resetButton(string MenuType)
    {
        
        /*if(MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }*/
        

        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
        }

        if(MenuType == "Graphics")
        {
            //brightnessSlider.value = defaultBrightness;
            //brightnessTextValue.text = defaultBrightness.ToString("0.0");

            //qualityDropdown.value = 1;
            //QualitySettings.SetQualityLevel(1);

            //fullscreenToggle.isOn = false;
            //Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    public void SFXPreview()
    {
        sfxPreviewSource.PlayOneShot(sfxPreview, 1);
    }

    public void menuSettings()
    {
        onClickSFX();
        gameManager.instance.pauseMenuSettings();
    }

    public void onClickSFX()
    {
        menuSFXSource.PlayOneShot(menuSFX, 1);
    }
}
