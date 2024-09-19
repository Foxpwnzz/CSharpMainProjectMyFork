using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class SmartNode
    {
        public Vector2Int Position;// ������� �� X/Y
        public int Cost = 10;// ��������� ��������
        public int Estimate;// ������ ���������� �� ����
        public int Value;// �������� �������� ������������� ������� (�������� ��������� ��������)
        public SmartNode Parent;// ������ �� ����, "���������"

        public SmartNode(Vector2Int position)
        {
            Position = position;
        }

        public void CalculateEstimate(Vector2Int targetPosition)// ������ ���������� �� ����
        {
            Estimate = Math.Abs(Position.x - targetPosition.x) + Math.Abs(Position.x - targetPosition.x);// ������� Math.Abs ���� ������ ������ �����, ������ ���� -
        }

        public void CalculateValue()// ������ ������������� �������, ������ �� ��������� � ���������� �� ����)
        {
            Value = Cost + Estimate;
        }

        public override bool Equals(object? obj)// �������� ��� ��������� ���������� � �����
        {
            if (obj is not SmartNode node)// ���� ���- ���������� false
                return false;

            return Position.Equals(node.Position);// ����� - ���������� ��������� �������� ���������
        }
    }
}