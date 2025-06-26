using AStarPathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    public int width;
    public int height;
    public bool openDrawGizmos;
    public LayerMask layerMask;
    public GameObject player;
    private List<Node> paths;
    public float moveSpeed;
    private int canMove;
    public bool CanAttack
    {
        get
        {
            return canMove == 1;
        }
    }
    void Start()
    {
        canMove = 1;
        player = GameObject.FindWithTag("Player");
        StartCoroutine(findPath());
    }
    private void Update()
    {
        if (player != null && paths != null && paths.Count > 0)
        {
            transform.Translate((new Vector3(paths[0].X + 0.5f, paths[0].Y + 0.5f, 0) - transform.position).normalized * moveSpeed * canMove * Time.deltaTime);
            if (Vector2.Distance(transform.position, new Vector2(paths[0].X + 0.5f, paths[0].Y + 0.5f)) < 0.1f)
            {
                paths.RemoveAt(0);
            }
        }
    }
    IEnumerator findPath()
    {
        while (true)
        {
            if(player == null)
            {
                player = GameObject.FindWithTag("Player");
            }
            if (player == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            AStar aStar = new AStar(width, height, layerMask);
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = new Vector2(player.transform.position.x, player.transform.position.y);
            paths = aStar.FindPath((int)pos.x, (int)pos.y, (int)targetPos.x, (int)targetPos.y);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Attack()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Light")
        {
            canMove = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Light")
        {
            canMove = 1;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0.92f,0.0016f,0.5f);
        Gizmos.DrawCube(new Vector2(width / 2, height / 2), new Vector2(width, height));
    }
}
