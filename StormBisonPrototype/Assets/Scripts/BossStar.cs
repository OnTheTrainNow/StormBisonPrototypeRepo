using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStar : MonoBehaviour
{
    [SerializeField] GameObject bossStar;

    public void spawnStar()
    {
        Instantiate(bossStar, transform.position, transform.rotation);
    }
}
