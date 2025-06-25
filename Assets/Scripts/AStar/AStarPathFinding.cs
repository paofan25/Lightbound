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
        public int GCost { get; set; }  // ����㵽��ǰ�ڵ�Ĵ���
        public int HCost { get; set; }  // ����ʽ���ۣ���ǰ�ڵ㵽�յ�Ĺ���ֵ
        public int FCost => GCost + HCost;  // �ܴ���
        public Node Parent { get; set; }  // ·���е���һ���ڵ�
        public bool IsWalkable { get; set; } = true;  // �Ƿ��ͨ��

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

            // ��ʼ������ڵ�
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

            // ��ʼ������ڵ� ����collider����map����
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
            // ��ȡ�����յ�ڵ�
            Node startNode = grid[startX, startY];
            Node endNode = grid[endX, endY];

            // ��ʼ�����ż��͹رռ�
            HashSet<Node> openSet = new HashSet<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // �ҵ����ż���FCost��С�Ľڵ�
                Node currentNode = openSet.OrderBy(n => n.FCost).ThenBy(n => n.HCost).First();

                // �ҵ�·�������ݹ���·��
                if (currentNode == endNode)
                {
                    return RetracePath(startNode, endNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // �����ھӽڵ�
                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    // ��������ͨ�������ڹرռ��еĽڵ�
                    if (!neighbor.IsWalkable || closedSet.Contains(neighbor))
                        continue;

                    // ���㵽���ھӵ��´���
                    int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);

                    // �����·�����۸��ͻ��ھӲ��ڿ��ż���
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

            return null; // ·��������
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

            // ���������ĸ�����
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
            // �����پ��루������4����
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);
            return (dstX + dstY) * 10;
        }
    }
}