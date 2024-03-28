using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creditsScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(autoScroll());
    }

    IEnumerator autoScroll()
    {
        while (scrollRect.normalizedPosition.y > 0)
        {
            float newY = Time.deltaTime * scrollSpeed;
            scrollRect.normalizedPosition = new Vector2(0, Mathf.Max(scrollRect.normalizedPosition.y - newY, 0));
            yield return null;
        }

        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
