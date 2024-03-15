using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class shopItemSO : ScriptableObject
{
    public string itemName;
    public string description;
    public int price;
}
