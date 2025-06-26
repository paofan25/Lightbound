using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("要跟随的目标")] public Transform target;

    [Header("跟随平滑度")] public float smoothSpeed = 5f;

    [Header("相机偏移量")] public Vector3 offset = new Vector3(0f, 0f, -10f); // z要拉远，避免相机挡住画面

    void LateUpdate(){
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}