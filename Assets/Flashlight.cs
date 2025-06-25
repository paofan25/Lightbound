using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Flashlight : MonoBehaviour
{
    public float maxDistance = 10f;

    public LayerMask wallLayer;

    private Light2D light2D;

    private void Awake(){
        light2D = GetComponent<Light2D>();
    }

    public void ShootLight(Vector2 direction){
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, wallLayer);
        float finalDistance = hit.collider ? hit.distance : maxDistance;

        light2D.pointLightOuterRadius = finalDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}