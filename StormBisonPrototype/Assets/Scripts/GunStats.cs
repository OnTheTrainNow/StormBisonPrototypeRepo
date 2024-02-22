using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public int shootDamage; //damage dont by a shot
    public int shootRange; //the range of the gun
    public float firingRate; //the firing rate of the gun

    public int currAmmo; //the player current ammo for the gun
    public int maxAmmo; //the players max ammo for the gun

    public GameObject gunModel; //the gameObject holding the gun model for the gun
    public ParticleSystem hitEffect; //the impact effect for the gun
    public AudioClip shootSFX; //the sound effect that will play when the player shoots
    [Range(0, 1)] public float shootSoundVol; //the volume of the shoot sfx
}
