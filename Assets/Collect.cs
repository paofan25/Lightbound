using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            if (GetComponent<SpriteRenderer>() != null) {
                this.GetComponent<SpriteRenderer>().enabled = false;
                PlayerInventory.TotalButtery += 1;
            }
        }
    }
}
