using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoor : MonoBehaviour
{
    public GameObject door;
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            door.SetActive(true);
        }
    }
}
