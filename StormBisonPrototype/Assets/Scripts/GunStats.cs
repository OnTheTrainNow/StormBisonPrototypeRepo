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

    public int maxAmmo;

    // Values related to the shotgun
    public float pelletDmg; // controls the damage each individual pellet does
    public int pellets; // this controls the number of pellets in each shot (the lower the amount the lower the damage, the higher the amount the higher the damage)
    public float pelletsSpreadAngle; // this sets the spread angle (smaller values = tighter spread, higher values = wider spread)

    public GameObject gunModel; //the gameObject holding the gun model for the gun
    public ParticleSystem hitEffect; //the impact effect for the gun
    public AudioClip shootSFX; //the sound effect that will play when the player shoots
    [Range(0, 1)] public float shootSoundVol; //the volume of the shoot sfx

    // bools related to weapon ui
    public bool isShotgunEquipped;
    public bool isPistolEquipped;
    public bool isRifleEquipped;
}
