using System.Collections.Generic;
using UnityEngine;
using UnitBrains.Pathfinding;
using Model;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private int maxRetryAttempts = 3;  // �������� 3 ������� ��������� ����
        private int currentRetryAttempts = 0;

        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
            : base(runtimeModel, startPoint, endPoint)
        {
            Calculate();  // ������������ ���� ��� ��������
        }

        protected override void Calculate()
        {
            if (currentRetryAttempts >= maxRetryAttempts)
            {
                path = new Vector2Int[0];  // ������������� �����, ���� ���� �� ����� ���� ������
                return;
            }

            Vector2Int nearestWalkableTile = FindNearestWalkableTile(EndPoint);  // ���������� EndPoint
            if (nearestWalkableTile == StartPoint)  // ���������� StartPoint
            {
                currentRetryAttempts++;
                path = new Vector2Int[0];  // ���� �����������
                return;
            }

            // A* ��������
            List<Vector2Int> openSet = new List<Vector2Int> { StartPoint };  // ���������� StartPoint
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float> { [StartPoint] = 0 };
            Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float> { [StartPoint] = Heuristic(StartPoint, nearestWalkableTile) };

            while (openSet.Count > 0)
            {
                Vector2Int current = GetLowestScore(openSet, fScore);

                if (current == nearestWalkableTile)
                {
                    ReconstructPath(cameFrom, current);
                    currentRetryAttempts = 0;  // ���������� ������� ������� ��� �������� ���������� ����
                    return;
                }

                openSet.Remove(current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!runtimeModel.IsTileWalkable(neighbor))
                        continue;

                    float tentative_gScore = gScore[current] + Vector2Int.Distance(current, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, nearestWalkableTile);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            currentRetryAttempts++;  // ����������� ������� �������
            path = new Vector2Int[0];  // ���� �����������, ���� �����
        }

        public Vector2Int GetNextStepFrom(Vector2Int unitPos)
        {
            if (path == null || path.Length == 0)
            {
                return unitPos;  // ���� ���� �� ������, ���� ������� �� �����
            }

            var found = false;
            foreach (var cell in path)
            {
                if (found)
                    return cell;

                found = cell == unitPos;
            }

            // ���� ���� �� �� ����, ������� ��������� �� ������
            Debug.LogError($"Unit {unitPos} is not on the path");

            // ������������� ���� ��� �����
            Calculate();

            // ���� ���� �� ��� �� ������, ���������� ������� ���������
            if (path == null || path.Length == 0)
            {
                return unitPos;
            }

            // ���������� ��������� ��� ����� ���������
            return path[0];
        }

        private void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector2Int> totalPath = new List<Vector2Int> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            path = totalPath.ToArray();
        }

        private Vector2Int FindNearestWalkableTile(Vector2Int target)
        {
            foreach (var neighbor in GetNeighbors(target))
            {
                if (runtimeModel.IsTileWalkable(neighbor))
                {
                    return neighbor;
                }
            }

            int maxRadius = 5;  // ����������� ������ �� 5 ������
            for (int radius = 2; radius <= maxRadius; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        Vector2Int expandedNeighbor = new Vector2Int(target.x + dx, target.y + dy);
                        if (runtimeModel.IsTileWalkable(expandedNeighbor))
                        {
                            return expandedNeighbor;
                        }
                    }
                }
            }

            return StartPoint;  // ���� �� ������� ���������� ������, ���� ������� �� ������
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }

        private Vector2Int GetLowestScore(List<Vector2Int> openSet, Dictionary<Vector2Int, float> fScore)
        {
            float lowestScore = float.MaxValue;
            Vector2Int bestNode = openSet[0];

            foreach (var node in openSet)
            {
                if (fScore.TryGetValue(node, out float score) && score < lowestScore)
                {
                    lowestScore = score;
                    bestNode = node;
                }
            }

            return bestNode;
        }

        private IEnumerable<Vector2Int> GetNeighbors(Vector2Int node)
        {
            yield return new Vector2Int(node.x + 1, node.y);
            yield return new Vector2Int(node.x - 1, node.y);
            yield return new Vector2Int(node.x, node.y + 1);
            yield return new Vector2Int(node.x, node.y - 1);
        }
    }
}