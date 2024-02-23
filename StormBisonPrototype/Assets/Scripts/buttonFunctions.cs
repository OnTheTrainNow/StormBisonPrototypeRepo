using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpaused();

        //if(PlayerPrefs.HasKey("Mouse Sensitivity X"))
        {
            PlayerPrefs.SetInt("Mouse Sensitivity X", (int)gameManager.instance.horizontalSensitivity);
        }
        //if (PlayerPrefs.HasKey("Mouse Sensitivity Y"))
        {
            PlayerPrefs.SetInt("Mouse Sensitivity Y", (int)gameManager.instance.verticalSensitivity);
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void quit()
    {
        Application.Quit();
    }

    public void respawn()
    {
        gameManager.instance.stateUnpaused(); //unpause the game
        gameManager.instance.playerScript.respawn(); //call the players respawn function
    }

    public void mouseSensitivity()
    {
        gameManager.instance.horizontalSensitivity = (int)gameManager.instance.sensitivitySliderX.value;
        gameManager.instance.verticalSensitivity = (int)gameManager.instance.sensitivitySliderY.value;

        gameManager.instance.sensitivityTextX.text = gameManager.instance.sensitivitySliderX.value.ToString("F0");
        gameManager.instance.sensitivityTextY.text = gameManager.instance.sensitivitySliderY.value.ToString("F0");
    }
}
