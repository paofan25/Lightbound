using UnityEngine;
using System.Collections.Generic;

public class FlashlightPlacer : MonoBehaviour
{
    public GameObject flashlightPrefab;
    public LayerMask obstacleLayer;
    public float rayLength = 3f;

    private List<GameObject> placedFlashlights = new List<GameObject>();

    private GameObject currentFlashlight;   // 当前正在放置的手电筒（可多个）
    private bool isPlaced = false;

    public int maxPlaceCount = 3; // 最大允许放置次数（由电池决定）
    private int currentPlaceCount = 0; // 当前已放次数

    public TMPro.TextMeshProUGUI shortestFlashlightText; // 或 UnityEngine.UI.Text

    private List<Vector2> availableDirections = new List<Vector2>();
    private int currentDirectionIndex = 0;
    public bool canPlace = true;
    public float moveThreshold = 0.05f;

    private Stack<GameObject> recycledFlashlights = new Stack<GameObject>(); // 回收的手电筒

    public LayerMask playerMask;
    void Update(){
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D a = Physics2D.OverlapPoint(mouseWorldPos, playerMask);
        if (a != null && a.CompareTag("Player")) {
            Debug.Log("碰到了");
        }
        if (!isPlaced&& canPlace) {
            if (currentFlashlight == null && recycledFlashlights.Count == 0) {
                currentFlashlight = Instantiate(flashlightPrefab, mouseWorldPos, Quaternion.identity);
                Debug.Log("手电筒生成");
                AutoDetectAndSetDirection();
            }

            // ✅ 判断是否移动了位置
            Vector2 oldPos = currentFlashlight.transform.position;
            currentFlashlight.transform.position = mouseWorldPos;

            // if ((Vector2)oldPos != mouseWorldPos) {
            //     // 如果发生位置变化，重置F键方向列表
            //     availableDirections.Clear();
            //     currentDirectionIndex = 0;
            //     Debug.Log("鼠标移动，已重置可用方向列表");
            // }
            if (Vector2.Distance(oldPos, mouseWorldPos) > moveThreshold) {
                // availableDirections.Clear();
                // currentDirectionIndex = 0;
                // Debug.Log("鼠标移动超过阈值，已重置可用方向列表");
                AutoDetectAndSetDirection();
            }
            // ScanAvailableDirections();
            if (Input.GetKeyDown(KeyCode.F)) {
                if (availableDirections.Count == 0) {
                    Debug.Log("首次按下F，扫描可用方向");
                    ScanAvailableDirections();
                }

                if (availableDirections.Count > 0) {
                    SwitchToNextDirection();
                }
                else {
                    Debug.Log("无可用方向");
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                if (currentPlaceCount >= maxPlaceCount) {
                    Debug.Log("⚠️ 放置次数已用完，无法放置更多手电筒");
                    return;
                }
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, playerMask);
                if (hit != null && hit.CompareTag("Player")) {
                    Debug.Log("👆 点击了人物");

                    if (currentFlashlight != null) {
                        GameObject player = hit.gameObject;
                        Transform existingFlashlight = player.transform.Find("Flashlight");
                        if (existingFlashlight != null) {
                            Debug.Log("❌ 人物已经有手电筒，不能重复添加");
                            return;
                        }

                        if (currentFlashlight != null) {
                            // ✅ 给手电筒重命名，便于下次判断
                            currentFlashlight.name = "Flashlight";

                            currentFlashlight.transform.SetParent(player.transform);
                            currentFlashlight.transform.localPosition = Vector3.zero;
                            currentFlashlight.transform.rotation = player.transform.rotation * Quaternion.Euler(0, 0, -90);
                            currentFlashlight.GetComponent<Flashlightt>().StartLifetimeCountdown();
                            Debug.Log("🔦 手电筒已附加到人物身上");
                            currentPlaceCount++;
                            Debug.Log($"✅ 手电筒已放置（{currentPlaceCount}/{maxPlaceCount}）");
                            isPlaced = true;
                            placedFlashlights.Add(currentFlashlight);
                            currentFlashlight = null;
                            availableDirections.Clear();
                            currentDirectionIndex = 0;
                        }
                    }
                }
                else {
                    // 正常放置在地面上
                    isPlaced = true;
                    currentPlaceCount++;
                    currentFlashlight.GetComponent<Flashlightt>().StartLifetimeCountdown();
                    Debug.Log($"✅ 手电筒已放置（{currentPlaceCount}/{maxPlaceCount}）");
                
                    // currentFlashlight.GetComponent<Flashlight>().StartLifetimeCountdown();
                    placedFlashlights.Add(currentFlashlight);
                    currentFlashlight = null;
                    availableDirections.Clear();
                    currentDirectionIndex = 0;
                }
            }
        }

        // R键：放弃当前手电筒，重新生成一个新的并拖动
        // if (Input.GetKeyDown(KeyCode.R)) {
        //     if (currentPlaceCount >= maxPlaceCount) {
        //         Debug.Log("⚠️ 已达到放置上限，无法重新放置手电筒");
        //         return;
        //     }
        //     if (currentFlashlight != null) {
        //         Destroy(currentFlashlight); // 销毁旧的
        //         Debug.Log("当前手电筒已销毁");
        //     }
        //
        //     isPlaced = false; // 进入拖动状态
        //     availableDirections.Clear(); // 清除方向
        //     currentDirectionIndex = 0;
        //
        //     // 会在 Update() 的 isPlaced == false 中重新生成新的手电筒
        //     Debug.Log("准备生成新的手电筒");
        // }

        // if (Input.GetKeyDown(KeyCode.R)) {
        //     if (placedFlashlights.Count > 0) {
        //         GameObject last = placedFlashlights[placedFlashlights.Count - 1];
        //         placedFlashlights.RemoveAt(placedFlashlights.Count - 1);
        //
        //         PickUpFlashlight(last); // 启用旧手电筒
        //         currentPlaceCount--; // 回收次数
        //
        //         Debug.Log("🔁 已回收上一个手电筒，进入拖动状态");
        //     }
        //     else {
        //         Debug.Log("⚠️ 没有可回收的手电筒");
        //     }
        // }
        if (Input.GetKeyDown(KeyCode.R)) {
            if (currentFlashlight != null) {
                Debug.Log("当前已有拖动手电筒，不能再次取出");
                return;
            }

            if (recycledFlashlights.Count > 0) {
                currentFlashlight = recycledFlashlights.Pop();
                currentFlashlight.SetActive(true);
                isPlaced = false;
                availableDirections.Clear();
                currentDirectionIndex = 0;
                Debug.Log("🔁 从回收堆中取出手电筒重新放置");
            }
            else if (currentPlaceCount < maxPlaceCount) {
                // ✅ 没有回收的，允许新建
                currentFlashlight = Instantiate(flashlightPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                currentFlashlight.transform.position = new Vector3(currentFlashlight.transform.position.x, currentFlashlight.transform.position.y, 0f); // Z归零
                isPlaced = false;
                AutoDetectAndSetDirection();
                Debug.Log("🆕 没有回收手电筒，新建一个手电筒进行放置");
            }
            else {
                Debug.Log("⚠️ 已达到最大放置次数，且没有回收手电筒可用");
            }
        }
        UpdateShortestFlashlightUI();
    }

    void UpdateShortestFlashlightUI(){
        Flashlightt shortest = null;
        float minTime = float.MaxValue;

        foreach (var light in FindObjectsOfType<Flashlightt>()) {
            float remaining = light.GetRemainingTime();
            if (remaining < minTime && remaining > 0f) {
                minTime = remaining;
                shortest = light;
            }
        }

        if (shortest != null) {
            shortestFlashlightText.text = $"最短寿命：{minTime:F1} 秒";
        }
        else {
            shortestFlashlightText.text = "当前无激活手电筒";
        }
    }
    public void ReceiveFlashlightBack(GameObject flashlight, float remaining){
        recycledFlashlights.Push(flashlight); // 压入回收栈
        flashlight.GetComponent<Flashlightt>().Reactivate(); // 恢复光照
        maxPlaceCount++; // 允许重新放置
        Debug.Log($"♻️ 回收了一个手电筒，当前最大放置数：{maxPlaceCount}");
    }
    void AutoDetectAndSetDirection(){
        availableDirections.Clear();
        currentDirectionIndex = 0;

        if (currentFlashlight == null) return;

        Vector2 pos = currentFlashlight.transform.position;

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions) {
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, rayLength, obstacleLayer);
            Debug.DrawRay(pos, dir * rayLength, Color.yellow, 0.3f);

            if (hit.collider == null) {
                availableDirections.Add(dir);
            }
        }

        if (availableDirections.Count > 0) {
            // 设置朝向第一个空方向
            Vector2 firstDir = availableDirections[0];
            float angle = Mathf.Atan2(firstDir.y, firstDir.x) * Mathf.Rad2Deg - 90f;
            currentFlashlight.transform.rotation = Quaternion.Euler(0, 0, angle);

            Debug.Log($"🧭 自动朝向空方向：{firstDir}");
        }
        else {
            Debug.Log("🧱 没有任何空方向，保持默认旋转");
        }
    }

    /// <summary>
    /// 扫描四个方向，查找没有障碍的方向
    /// </summary>
    // void ScanAvailableDirections()
    // {
    //     availableDirections.Clear();
    //     currentDirectionIndex = 0;
    //
    //     Vector2 pos = currentFlashlight.transform.position;
    //     Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    //
    //     foreach (Vector2 dir in directions)
    //     {
    //         RaycastHit2D hit = Physics2D.Raycast(pos, dir, rayLength, obstacleLayer);
    //         Debug.DrawRay(pos, dir * rayLength, Color.green, 0.5f);
    //
    //         if (hit.collider == null)
    //         {
    //             availableDirections.Add(dir);
    //             Debug.Log($"空方向：{dir}");
    //         }
    //         else
    //         {
    //             // Debug.Log($"方向 {dir} 被 {hit.collider.name} 阻挡");
    //         }
    //     }
    // }
    /// <summary>
    /// 扫描四个方向，查找没有障碍的方向，并打印方向名称
    /// </summary>
    void ScanAvailableDirections(){
        availableDirections.Clear();
        currentDirectionIndex = 0;

        Vector2 pos = currentFlashlight.transform.position;

        // 定义四个方向及其名称
        Dictionary<Vector2, string> directionNames = new Dictionary<Vector2, string>
        {
            { Vector2.up, "↑ 上" },
            { Vector2.down, "↓ 下" },
            { Vector2.left, "← 左" },
            { Vector2.right, "→ 右" }
        };

        foreach (Vector2 dir in directionNames.Keys) {
            RaycastHit2D hit = Physics2D.Raycast(pos, dir, rayLength, obstacleLayer);
            Debug.DrawRay(pos, dir * rayLength, Color.green, 0.5f);

            string dirName = directionNames[dir];

            if (hit.collider == null) {
                availableDirections.Add(dir);
                Debug.Log($"✅ 空方向：{dirName}（{dir}）");
            }
            else {
                Debug.Log($"❌ 被阻挡：{dirName}（{dir}） --> {hit.collider.name}");
            }
        }

        if (availableDirections.Count == 0) {
            Debug.Log("⚠️ 没有任何空方向");
        }
    }

    public void AddBattery(int count){
        maxPlaceCount += count;
        Debug.Log($"🔋 电池增加，当前最多可放置次数：{maxPlaceCount}");
    }
    public void PickUpFlashlight(GameObject picked){
        currentFlashlight = picked;
        currentFlashlight.SetActive(true);
        isPlaced = false;
        availableDirections.Clear();
        currentDirectionIndex = 0;
        // currentPlaceCount--;
        // var flash = currentFlashlight.GetComponent<Flashlightt>();
        // if (flash != null) {
        //     flash.Reactivate(); // ✅ 恢复灯光 & 剩余时间倒计时
        // }

        Debug.Log("✅ 手电筒已进入重新放置状态");
    }

    /// <summary>
    /// 将手电筒旋转至当前方向，并输出方向名称（上/下/左/右）
    /// </summary>
    // void SwitchToNextDirection(){
    //     if (availableDirections.Count == 0) return;
    //
    //     // 获取当前方向
    //     Vector2 nextDir = availableDirections[currentDirectionIndex];
    //
    //     // 将方向向量转为角度，控制手电筒朝向
    //     float angle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg-90f;
    //     currentFlashlight.transform.rotation = Quaternion.Euler(0, 0, angle);
    //
    //     // 获取方向名称（上下左右）
    //     string dirName = GetDirectionName(nextDir);
    //
    //     // 输出调试信息
    //     Debug.Log($"🔄 切换手电筒方向至：{dirName}（{nextDir}）");
    //
    //     // 指向下一个可用方向（循环）
    //     currentDirectionIndex = (currentDirectionIndex + 1) % availableDirections.Count;
    // }
    void SwitchToNextDirection(){
        if (availableDirections.Count == 0 || currentFlashlight == null) return;

        // 当前手电筒角度（Z 轴）用于比较
        float currentAngle = currentFlashlight.transform.eulerAngles.z;

        int originalIndex = currentDirectionIndex;
        int attempts = 0;

        do {
            Vector2 nextDir = availableDirections[currentDirectionIndex];
            float newAngle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg - 90f;

            // 如果角度不同于当前角度，则设置并跳出
            if (!Mathf.Approximately(NormalizeAngle(newAngle), NormalizeAngle(currentAngle))) {
                currentFlashlight.transform.rotation = Quaternion.Euler(0, 0, newAngle);

                string dirName = GetDirectionName(nextDir);
                Debug.Log($"🔄 切换手电筒方向至：{dirName}（{nextDir}）");

                currentDirectionIndex = (currentDirectionIndex + 1) % availableDirections.Count;
                return;
            }

            // 向后循环尝试
            currentDirectionIndex = (currentDirectionIndex + 1) % availableDirections.Count;
            attempts++;

        } while (attempts < availableDirections.Count); // 最多尝试一次完整轮换
    }

    float NormalizeAngle(float angle){
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
    /// <summary>
    /// 根据方向向量返回文字名称（上/下/左/右）
    /// </summary>
    string GetDirectionName(Vector2 dir){
        if (dir == Vector2.up) return "↑ 上";
        if (dir == Vector2.down) return "↓ 下";
        if (dir == Vector2.left) return "← 左";
        if (dir == Vector2.right) return "→ 右";
        return "未知方向";
    }
}
