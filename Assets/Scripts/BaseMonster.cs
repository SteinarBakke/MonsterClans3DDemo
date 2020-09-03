using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseMonster : MonoBehaviour
{


    /*
    * You want this class to hold all variables shared amongst all Monster
    */
    [Header("Base Stats")]
    public string name;
    public int baseLVL; 
    public float baseHP;
    public float baseEnergy;
    public float baseStamina;
    public float baseStrength;
    public float baseSpeed;
    public float basecritRate;
    public float basecritDamage;
    public List<GameObject> abilities = new List<GameObject>();


    //public Image Icon;
    //Public List Spells/Abilities
        // In my Abilities I want
            // 1. Dmg
            // 2. Energy Regain/Cooldown
            // 3. Use Cooldown/energy
            // 4. Icon(?) / name
    //Public Animation SpellAnimations
    //Public Animation DeathAnimation
    //Public Animation Victory
    // ?? Public List Animations

    
    /*
     * When setting base stats for a monster, set them in a random range between x to z.
     * 
     */ 

    public enum elementType
    {
        FIRE,
        WATER,
        GRASS,
        EARTH,
        AIR,
        LIGHTNING,
        SHADOW
    };
    public elementType element;

    [Header("Current Status")]
    public int currLVL;
    public float currHP;
    public float currEnergy;
    public float currStamina;
    public float currStrength;
    public float currSpeed;
    public float speedMultiplier;
    public float currCritRate;
    public float currCritDamage;

    [Header("DO I HAVE A OWNER")]
    public bool wild;

    void Start()
    {
        currLVL = baseLVL;
        /*
         *  //Temp add a randomizer increase to the level
         */
        currLVL += Random.Range(0, 5);
        baseHP += currLVL *2;
        baseStrength += currLVL * 2;
        baseSpeed += currLVL * 2;

        //Setting current stats = base stats
        currHP = baseHP;
        currStrength = baseStrength;
        currSpeed = baseSpeed;
        currStamina = baseStamina;
        currEnergy = baseEnergy;
        currCritRate = basecritRate;
        currCritDamage = basecritDamage;
    }
}

