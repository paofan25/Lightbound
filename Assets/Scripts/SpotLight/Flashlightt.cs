using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlightt : MonoBehaviour
{
    public float lifetime = 15f; // 点亮时间（秒）

    private bool isLit = false;

    private Light2D lightComponent; // 你可以用 SpriteRenderer 模拟也行

    void OnTriggerStay2D(Collider2D other){
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("🎒 手电筒被玩家捡起");

            // 通知管理器准备重新拖动放置
            FindObjectOfType<FlashlightPlacer>().PickUpFlashlight(this.gameObject);

            // 暂时隐藏手电筒（或设置未放置状态）
            gameObject.SetActive(false);
        }
    }
    void Start(){
        lightComponent = GetComponentInChildren<Light2D>(); // 或者其他灯光方式
    }

    private void OnEnable(){
        StartLifetimeCountdown();
    }

    public void StartLifetimeCountdown(){
        if (!isLit) {
            isLit = true;
            Invoke("TurnOff", lifetime);
        }
    }

    void TurnOff(){
        Debug.Log("⏳ 手电筒已熄灭");
        if (lightComponent != null)
            lightComponent.enabled = false;
        // 也可以：gameObject.SetActive(false);
    }

    public void Reactivate(){
        CancelInvoke("TurnOff");
        if (lightComponent != null)
            lightComponent.enabled = true;
        isLit = true;
        Invoke("TurnOff", lifetime);
        Debug.Log("🔁 手电筒再次激活");
    }
}