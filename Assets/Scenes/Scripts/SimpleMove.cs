using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Vector2 moveInput;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (moveInput.sqrMagnitude > 0.01f) {
            // ✅ 方式一：让角色整体朝向移动方向旋转
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // ✅ 若你用方式二（横版翻转）可替换为：
            // Vector3 scale = transform.localScale;
            // scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            // transform.localScale = scale;
        }
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}