using System;
using System.Collections;
using System.Collections.Generic;
using abc;
using LDtkUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class ReadExample : MonoBehaviour
{
    [FormerlySerializedAs("LDtkFields")] public LDtkFields lDtkFields;
    public int life;
    public bool isAwaken;

    public ItemType weapon;

    public ItemType[] bag;
    // Start is called before the first frame update
    void Start(){
        lDtkFields = GetComponent<LDtkFields>();
        life=lDtkFields.GetInt("life");
        isAwaken = lDtkFields.GetBool("isAwaken");
        weapon = lDtkFields.GetEnum<ItemType>("weapon");
        bag = lDtkFields.GetEnumArray<ItemType>("bag");
    }

    // Update is called once per frame
    void Update(){
        
    }

    // public void OnLDtkImportFields(LDtkFields fields){
    //     life = fields.GetInt("life");
    //     isAwaken = fields.GetBool("isAwaken");
    //     weapon = fields.GetEnum<ItemType>("weapon");
    //     bag = fields.GetEnumArray<ItemType>("PossibleDrops");
    // }
}
