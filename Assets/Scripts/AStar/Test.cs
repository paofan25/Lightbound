using AStarPathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int width;
    public int height;
    public bool openDrawGizmos;
    public LayerMask layerMask;
    public GameObject monster;
    public GameObject player;
    public List<Node> paths;
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(findPath());
    }
    private void Update()
    {
        if(paths != null && paths.Count > 0)
        {
            monster.transform.Translate((new Vector3(paths[0].X + 0.5f, paths[0].Y + 0.5f, 0) - monster.transform.position).normalized * moveSpeed * Time.deltaTime);
            if (Vector2.Distance(monster.transform.position, new Vector2(paths[0].X + 0.5f, paths[0].Y + 0.5f)) < 0.1f)
            {
                //Debug.Log("到达下一个点 + " + Vector2.Distance(monster.transform.position, new Vector2(paths[0].X, paths[0].Y)));
                paths.RemoveAt(0);
            }
        }
    }
    IEnumerator findPath()
    {
        while (true)
        {
            AStar aStar = new AStar(width, height, layerMask);
            Vector2 pos = new Vector2(monster.transform.position.x, monster.transform.position.y);
            Vector2 targetPos = new Vector2(player.transform.position.x, player.transform.position.y);
            paths = aStar.FindPath((int)pos.x, (int)pos.y, (int)targetPos.x, (int)targetPos.y);
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 50), "寻路"))
        {
            AStar aStar = new AStar(width, height, layerMask);

            //AStar aStar = new AStar(map);
            var path = aStar.FindPath(0, 0, width -1, height -1);

            if (path != null)
            {
                string str = "Path found:";
                foreach (Node node in path)
                {
                    str += $"=>({node.X}, {node.Y})";
                }
                Debug.Log($"{str}");
            }
            else
            {
                Debug.Log("Path not found!");
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (openDrawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(new Vector2(width / 2, height / 2), new Vector2(width, height));
        }
    }
}
