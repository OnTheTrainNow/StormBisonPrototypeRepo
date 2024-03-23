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

    private GameObject confirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    public AudioSource sfxPreviewSource;
    public AudioClip sfxPreview;

    public AudioSource menuSFXSource;
    public AudioClip menuSFX;

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if(PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
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

    public void resetButton(string MenuType)
    {
        /*
        if(MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
        */

        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
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
