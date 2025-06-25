using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace AStarPathfinding
{
    public class Node
    {
        public int X { get; }
        public int Y { get; }
        public int GCost { get; set; }  // 从起点到当前节点的代价
        public int HCost { get; set; }  // 启发式代价，当前节点到终点的估计值
        public int FCost => GCost + HCost;  // 总代价
        public Node Parent { get; set; }  // 路径中的上一个节点
        public bool IsWalkable { get; set; } = true;  // 是否可通过

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class AStar
    {
        private Node[,] grid;
        private int width;
        private int height;
        public LayerMask LayerMask { get; set; } 

        public AStar(bool[,] map, LayerMask layerMask)
        {
            width = map.GetLength(0);
            height = map.GetLength(1);
            this.LayerMask = layerMask;
            grid = new Node[width, height];

            // 初始化网格节点
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new Node(x, y);
                    grid[x, y].IsWalkable = map[x, y];
                }
            }
        }

        public AStar(int width,int height,LayerMask layerMask)
        {
            this.width = width;
            this.height = height;
            this.LayerMask = layerMask;
            grid = new Node[width, height];
            bool[,] map = new bool[width, height];

            // 初始化网格节点 根据collider创建map数据
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = new Node(x, y);
                    grid[x, y].IsWalkable = Physics2D.OverlapBox(new Vector2(x + 0.5f, y + 0.5f),new Vector2(0.9f,0.9f), 0 ,LayerMask) == null;
                    //if (grid[x, y].IsWalkable)
                    //{
                    //    GameObject.Instantiate(Resources.Load("Point"), new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                    //}
                    //else
                    //{
                    //    GameObject.Instantiate(Resources.Load("Point2"), new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                    //}
                }
            }
        }

        public List<Node> FindPath(int startX, int startY, int endX, int endY)
        {
            // 获取起点和终点节点
            Node startNode = grid[startX, startY];
            Node endNode = grid[endX, endY];

            // 初始化开放集和关闭集
            HashSet<Node> openSet = new HashSet<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // 找到开放集中FCost最小的节点
                Node currentNode = openSet.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();

                // 找到路径，回溯构建路径
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // 遍历邻居节点
                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    // 跳过不可通过或已在关闭集中的节点
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                        continue;

                    // 计算到达邻居的新代价
                    int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                    // 如果新路径代价更低或邻居不在开放集中
                    if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, endNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // 路径不存在
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        private IEnumerable<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            // 上下左右四个方向
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int newX = node.X + dx[i];
                int newY = node.Y + dy[i];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    neighbors.Add(grid[newX, newY]);
                }
            }

            return neighbors;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            // 曼哈顿距离（适用于4方向）
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);
            return (dstX + dstY) * 10;
        }
    }
}