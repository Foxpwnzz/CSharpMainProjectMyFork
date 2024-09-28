using Assets.Scripts.UnitBrains.Buff;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.BuffP
{

    public class DoubleShotBuffSettings : MonoBehaviour
    {
        [SerializeField]
        private float duration = 10f;

        // ����� ��� �������� DoubleShotBuff � ����������� �� ����������
        public DoubleShotBuff CreateBuff()
        {
            return new DoubleShotBuff(duration);
        }

        // �������� ��� ������� � ����������
        public float Duration => duration;
    }
}