using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoinManager : MonoBehaviour
{
    public static RedCoinManager instance;

    [SerializeField] GameObject Star;
    [SerializeField] int positionIndex;
    [SerializeField] int StarID;
    [SerializeField] GameObject SpawnPositon;

    bool activated;
    public int redCoinCount;

    MeshRenderer render;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        if (render != null) //turn the renderer off for the manager
        {
            render.enabled = false;
        }
    }

    public void UpdateRedCoins(int amount)
    {
        redCoinCount += amount;

        if (redCoinCount <= 0 && !activated)
        {
            activated = true;
            GameObject spawned = Instantiate(Star, SpawnPositon.transform.position, SpawnPositon.transform.rotation);
            spawned.GetComponent<Star>().positionIndex = positionIndex;
            spawned.GetComponent<Star>().starArrayID = StarID;
        }
    }
}
