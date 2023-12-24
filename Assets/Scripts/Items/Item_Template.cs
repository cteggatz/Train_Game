using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item_Template : ScriptableObject
{
    // ---- Graphics Settings ----
    [Header("Graphics")]
    public Sprite sprite;
    public Vector3 sprite_Size;

    // ---- General setting ----
    [Header("General")]
    public new string name;
    public string description;

    // ---- Gun Settings ----
    [Header("Item Usage")]
    [Min(0)] public int numberOfUses;
    [Min(0)] public int useIncrement;
    [Min(0f)] public float rearmTime;
    [Min(0f)] public float useCooldown;

    public abstract void Use(PlayerItemController itemController, float angle, Vector3 position, float spawnDistance, int layer);
}
