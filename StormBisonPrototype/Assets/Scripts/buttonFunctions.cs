using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpaused();

        PlayerPrefs.SetInt("Mouse Sensitivity X", (int)gameManager.instance.sensitivitySliderX.value);
        PlayerPrefs.SetInt("Mouse Sensitivity Y", (int)gameManager.instance.sensitivitySliderY.value);
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
}
