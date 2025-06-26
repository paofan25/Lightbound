using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlightt : MonoBehaviour
{
    private float remainingTime; // 剩余时间

    private float countdownStartTime; // 倒计时开始的游戏时间

    public float lifetime = 15f; // 点亮时间（秒）

    private bool isLit = false;

    private Light2D lightComponent; // 你可以用 SpriteRenderer 模拟也行

    bool isPlayerInside = false;

    Collider2D playerCollider;

    

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")) {
            isPlayerInside = true;
            playerCollider = other;
        }
    }

    void Update(){
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("🎒 手电筒被玩家捡起");

            float elapsed = Time.time - countdownStartTime;
            remainingTime -= elapsed;
            CancelInvoke("TurnOff");
            isLit = false;

            var placer = FindObjectOfType<FlashlightPlacer>();
            placer.ReceiveFlashlightBack(this.gameObject, remainingTime); // ✅ 新接口
            gameObject.SetActive(false);
        }
    }

    public float GetRemainingTime(){
        if (!isLit) return remainingTime;
        return remainingTime - (Time.time - countdownStartTime);
    }
    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player")) {
            isPlayerInside = false;
            playerCollider = null;
        }
    }
    // void OnTriggerStay2D(Collider2D other){
    //     Debug.Log("我被碰到了");
    //     if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) {
    //         Debug.Log("🎒 手电筒被玩家捡起");
    //
    //         // 通知管理器准备重新拖动放置
    //         FindObjectOfType<FlashlightPlacer>().PickUpFlashlight(this.gameObject);
    //
    //         // 暂时隐藏手电筒（或设置未放置状态）
    //         gameObject.SetActive(false);
    //     }
    // }
    void Start(){
        lightComponent = GetComponentInChildren<Light2D>();
        remainingTime = lifetime; // 初始为满时间
    }

    // private void OnEnable(){
    //     StartLifetimeCountdown();
    // }

    // public void StartLifetimeCountdown(){
    //     if (!isLit) {
    //         isLit = true;
    //         Invoke("TurnOff", lifetime);
    //     }
    // }
    public void StartLifetimeCountdown(){
        if (!isLit) {
            isLit = true;
            countdownStartTime = Time.time; // 记录开始时间
            remainingTime = lifetime;
            Invoke("TurnOff", remainingTime);
        }
    }

    void TurnOff(){
        Debug.Log("⏳ 手电筒已熄灭");
        if (lightComponent != null)
            // lightComponent.enabled = false;
            Destroy(gameObject);
        // 也可以：gameObject.SetActive(false);
    }

    public void Reactivate(){
        CancelInvoke("TurnOff");

        if (lightComponent != null)
            lightComponent.enabled = true;

        isLit = true;
        countdownStartTime = Time.time;
        Invoke("TurnOff", remainingTime); // 使用剩余时间
        Debug.Log($"🔁 手电筒再次激活，剩余时间：{remainingTime:F2} 秒");
    }
}