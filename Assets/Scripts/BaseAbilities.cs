using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class BaseAbilities : MonoBehaviour
{
    /* Holding base information for all abilities
     * EnergyGain can either be a positive value and gain energy, or be a negative value and loose energy
     * 
     */ 
    public Image icon;
    public Animation attackAnim;
    public string name;
    public float damage;
    public float energyGain;
}
