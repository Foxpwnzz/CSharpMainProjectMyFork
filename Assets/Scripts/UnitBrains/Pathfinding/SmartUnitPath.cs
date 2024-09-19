using Model;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class SmartUnitPath : BaseUnitPath
    {
        private Vector2Int _startPosition;
        private Vector2Int _targetPosition;
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };

        private bool _isTarget;
        private bool _isEnemyUnitClose;
        private SmartNode _nextToEnemyUnit;

        public SmartUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPosition, Vector2Int targetPosition) :
            base(runtimeModel, startPosition, targetPosition)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
        }

        protected override void Calculate()
        {
            
            SmartNode startNode = new SmartNode(_startPosition);// ����� ��������� ����������
            SmartNode targetNode = new SmartNode(_targetPosition);// ����� ���������� ����
            List<SmartNode> openList = new List<SmartNode> { startNode };// � ������ �������� ������� � ������� ����� �����
            List<SmartNode> closedList = new List<SmartNode>();// � ������ �������� ���������� �������, ������� �� ��������� � �����������

            int counter = 0;
            int maxCount = runtimeModel.RoMap.Width * runtimeModel.RoMap.Height;// ������������ �������� ����� ��������� �����

            while (openList.Count > 0 && counter++ < maxCount)// ���� ����������� ���� � openList ��� ���� ����
            {
                SmartNode currentNode = openList[0];// ���������� ������ ���� �� ������ (���������� ���������� � 0)

                foreach (var node in openList)
                {
                    if (node.Value < currentNode.Value)// ���������� ���� � ������ � ���� � ���������� ��������� ������������� �������
                        currentNode = node;// ������ ����� ���� �������
                }

                openList.Remove(currentNode);//��� ��� ���� ��������, �� ��� ����������� �� ��������� ������
                closedList.Add(currentNode);// � ����������� � ��������

                if (_isTarget)
                {
                    path = FindPath(currentNode);
                    return;
                }

                for (int i = 0; i < dx.Length; i++)
                {
                    // ���������� ���������� ������� ���� � � �������� �� ���, � ������ ����� ���������� �� � � Y ��������������
                    int newX = currentNode.Position.x + dx[i];
                    int newY = currentNode.Position.y + dy[i];
                    var newPosition = new Vector2Int(newX, newY);//����� ������� 

                    if (newPosition == targetNode.Position)// ���������, ����� �� ��������� ������� �������
                        _isTarget = true;

                    if (IsValid(newPosition) || _isTarget)// ���� ������ �������� ��� ���� � ��� �������� �������
                    {
                        SmartNode neighbor = new SmartNode(newPosition);// ��� �� �������� ����

                        if (closedList.Contains(neighbor))// ���������, ��� ���� ���� ��� � �������� ������
                            continue;

                        neighbor.Parent = currentNode;// ��������� � ����������� ������� ����
                        neighbor.CalculateEstimate(targetNode.Position);// ������������ ����������
                        neighbor.CalculateValue();// � ��������� ������������� �������

                        openList.Add(neighbor);// ��������� ���� � �������� ������
                    }
                    if (CheckCollisionWithEnemy(newPosition) && !_isEnemyUnitClose)
                    {
                        _isEnemyUnitClose = true;
                        _nextToEnemyUnit = currentNode;
                    }
                }
            }
            if (_isEnemyUnitClose)
            {
                path = FindPath(_nextToEnemyUnit);
                return;
            }

            path = new Vector2Int[] { startNode.Position };
        }

        private Vector2Int[] FindPath(SmartNode currentNode)
        {
            List<Vector2Int> path = new();

            while (currentNode != null)// ���� �������� � �������� �������, ���� currentNode ����� ��������
            {
                path.Add(currentNode.Position);// �������� ������� ���� � ������ "����"
                currentNode = currentNode.Parent;// ����������� ��� ������� ���� ��������� �� Parent
            }

            path.Reverse();// ������������� ������ � �������� (����������) �������
            return path.ToArray();// ���������� ������ ���� ������� � ���������� �����
        }

        private bool IsValid(Vector2Int point)//��������� ������������ ������ �� �����
        {
            return runtimeModel.IsTileWalkable(point);
        }

        private bool CheckCollisionWithEnemy(Vector2Int newPos)//��������� ������������ � ������
        {
            var botUnitPositions = runtimeModel.RoBotUnits.Select(u => u.Pos).Where(u => u == newPos);

            return botUnitPositions.Any();
        }
    }
}