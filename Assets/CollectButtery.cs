using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectButtery : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            
            FindObjectOfType<FlashlightPlacer>().AddBattery(1); // 每个电池+1
            Destroy(gameObject);
            PlayerInventory.TotalButtery += 1;
            
        }
    }
}
