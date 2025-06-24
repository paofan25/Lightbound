using System;
using UnityEngine;
using LDtkUnity;

public class MyEventTrigger : MonoBehaviour
{
    private void Start(){
        LDtkFields field = GetComponent<LDtkFields>();
        if (field == null) {
            Debug.LogWarning("未找到 LDtkComponentEntity");
            return;
        }

        
        // var targets = field.;

        
    }
}