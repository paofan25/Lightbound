using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType
{
    None,

    RequireItemCount,

    RequireKey,

    RequireEnemyCleared
}

public class OpenDoor : MonoBehaviour
{
    public GameObject door;

    public ConditionType condition = ConditionType.None;

    public int requiredItemCount = 0;

    public string requiredKeyName = "";

    [Header("可选：触发时添加钥匙")] public bool addKeyOnOpen = false;

    public string keyToAdd = "";

    private void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player") && CheckCondition()) {
            door.SetActive(false);

            // 添加钥匙（如果启用）
            if (addKeyOnOpen && !string.IsNullOrEmpty(keyToAdd)) {
                PlayerInventory.Instance.AddKey(keyToAdd);
                Debug.Log("已添加钥匙：" + keyToAdd);
            }
        }
    }

    bool CheckCondition(){
        switch (condition) {
            case ConditionType.None:
                return true;

            case ConditionType.RequireItemCount:
                return PlayerInventory.Instance.ItemCount >= requiredItemCount;

            case ConditionType.RequireKey:
                return PlayerInventory.Instance.HasKey(requiredKeyName);

            case ConditionType.RequireEnemyCleared:
                return EnemyManager.Instance.AllEnemiesDefeated();

            default:
                return false;
        }
    }
}