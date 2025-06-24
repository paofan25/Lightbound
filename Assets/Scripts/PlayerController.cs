using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")] [SerializeField] float moveSpeed = 5f;

    [Header("跳跃参数")] [SerializeField] float jumpForce = 6f;

    Rigidbody2D rb;

    bool isGrounded;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() // 处理输入
    {
        // ① 水平移动：使用 Unity 内置输入轴，可在 Input Manager 里映射 A/D、←/→、手柄左摇杆
        float move = Input.GetAxisRaw("Horizontal"); // 取值 -1,0,1
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // ② 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded) // 默认把 Space 映射到 Jump
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // 直到再次碰撞地面
        }
    }

    // ③ 落地判定：只要任何接触点的法线朝上即可
    void OnCollisionEnter2D(Collision2D collision){
        foreach (var c in collision.contacts) {
            if (c.normal.y > 0.5f) {
                // 角度阈值≈60°
                isGrounded = true;
                break;
            }
        }
    }
}