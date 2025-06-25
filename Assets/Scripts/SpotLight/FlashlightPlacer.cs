using UnityEngine;
using System.Collections.Generic;

public class FlashlightPlacer : MonoBehaviour
{
    public GameObject flashlightPrefab;
    public LayerMask obstacleLayer;
    public float rayLength = 3f;

    private GameObject currentFlashlight;   // 当前正在放置的手电筒（可多个）
    private bool isPlaced = false;

    private List<Vector2> availableDirections = new List<Vector2>();
    private int currentDirectionIndex = 0;

    public float moveThreshold = 0.05f;
    void Update(){
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (!isPlaced) {
            if (currentFlashlight == null) {
                currentFlashlight = Instantiate(flashlightPrefab, mouseWorldPos, Quaternion.identity);
                Debug.Log("手电筒生成");
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
                availableDirections.Clear();
                currentDirectionIndex = 0;
                Debug.Log("鼠标移动超过阈值，已重置可用方向列表");
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
                isPlaced = true;
                Debug.Log("手电筒已放置");

                currentFlashlight = null;
                availableDirections.Clear();
                currentDirectionIndex = 0;
            }
        }

        // R键：放弃当前手电筒，重新生成一个新的并拖动
        if (Input.GetKeyDown(KeyCode.R)) {
            if (currentFlashlight != null) {
                Destroy(currentFlashlight); // 销毁旧的
                Debug.Log("当前手电筒已销毁");
            }

            isPlaced = false; // 进入拖动状态
            availableDirections.Clear(); // 清除方向
            currentDirectionIndex = 0;

            // 会在 Update() 的 isPlaced == false 中重新生成新的手电筒
            Debug.Log("准备生成新的手电筒");
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


    /// <summary>
    /// 将手电筒旋转至当前方向，并输出方向名称（上/下/左/右）
    /// </summary>
    void SwitchToNextDirection(){
        if (availableDirections.Count == 0) return;

        // 获取当前方向
        Vector2 nextDir = availableDirections[currentDirectionIndex];

        // 将方向向量转为角度，控制手电筒朝向
        float angle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg-90f;
        currentFlashlight.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 获取方向名称（上下左右）
        string dirName = GetDirectionName(nextDir);

        // 输出调试信息
        Debug.Log($"🔄 切换手电筒方向至：{dirName}（{nextDir}）");

        // 指向下一个可用方向（循环）
        currentDirectionIndex = (currentDirectionIndex + 1) % availableDirections.Count;
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
