using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject nextLevelPos;
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            other.transform.position=nextLevelPos.transform.position;
        }
    }
}
