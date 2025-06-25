using UnityEngine;

public class ChangeDirection : MonoBehaviour
{
    public float rayLength = 5f;

    // 指定要检测的 Layer（可在 Inspector 中设置）
    public LayerMask targetLayer;

    void Update(){
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2[] directions =
            {
                Vector2.up,
                Vector2.down,
                Vector2.left,
                Vector2.right
            };

            int hitCount = 0;

            foreach (Vector2 dir in directions) {
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, dir, rayLength, targetLayer); // 只检测指定 Layer
                Debug.DrawRay(mouseWorldPos, dir * rayLength, Color.yellow, 0.5f);

                if (hit.collider != null) {
                    hitCount++;
                    Debug.Log($"方向 {dir} 命中对象: {hit.collider.name}");
                }
            }

            switch (hitCount) {
                case 0:
                    Debug.Log("命中0个指定层对象，执行逻辑A");
                    break;
                case 1:
                    Debug.Log("命中1个指定层对象，执行逻辑B");
                    break;
                case 2:
                    Debug.Log("命中2个指定层对象，执行逻辑C");
                    break;
                default:
                    Debug.Log($"命中{hitCount}个对象，超过设定范围，忽略");
                    break;
            }
        }
    }
}