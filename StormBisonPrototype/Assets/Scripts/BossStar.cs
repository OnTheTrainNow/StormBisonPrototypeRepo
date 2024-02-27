using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStar : MonoBehaviour
{
    [SerializeField] enemyAI enemy;
    [SerializeField] Star bossStar;

    // Update is called once per frame
    void Update()
    {
        if (enemy != null)
        {
            if (enemy.IsDead())
            {
                Instantiate(bossStar);
            }
        }
    }
}
