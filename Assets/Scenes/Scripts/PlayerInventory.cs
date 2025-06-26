using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public static int TotalSpotLight;

    public static int TotalButtery;
    public int ItemCount = 0;

    private HashSet<string> keys = new HashSet<string>();

    private void Awake(){
        Instance = this;
        TotalSpotLight = 3;
    }

    public bool HasKey(string keyName){
        return keys.Contains(keyName);
    }

    public void AddKey(string keyName){
        keys.Add(keyName);
    }
}

