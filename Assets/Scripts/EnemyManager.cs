using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public List<GameObject> enemies;

    private void Awake(){
        Instance = this;
    }

    public bool AllEnemiesDefeated(){
        return enemies.TrueForAll(enemy => enemy == null || !enemy.activeInHierarchy);
    }
}
