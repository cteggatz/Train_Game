using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Blank_Item : ScriptableObject
{

//---- GRAPICS ----
    [Header("Graphics")]
    public Sprite sprite;
    public Vector3 sprite_Size;
    public bool collidable;

// ---- General Description ----
    [Header("Description")]
    public new string name;
    public string description;

}

[Serializable]
public abstract class Usable_Item : Blank_Item{
    // ---- USAGE ----
    [Header("Item Usability")]    
    public int maxUseQuantity;
    public int useIncrement;
    public float useCooldown;
    public abstract void use(Transform player, Vector3 pos, float angle, int layer);
}
