using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRatingDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text starsCollected;
    [SerializeField] Image ratingForeground;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            starsCollected.text = starManager.instance.starCount.ToString() + " / " + starManager.instance.starTracker.Length.ToString();
            ratingForeground.fillAmount = (float)starManager.instance.starCount / (float)starManager.instance.starTracker.Length;
        }
    }
}
