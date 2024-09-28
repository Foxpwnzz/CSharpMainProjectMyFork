using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class RangeIncreaseBuffSettings : MonoBehaviour
    {
        [SerializeField]
        private float rangeMultiplier = 1.5f;

        [SerializeField]
        private float duration = 8f;

        // ����� ��� �������� RangeIncreaseBuff � ����������� �� ����������
        public RangeIncreaseBuff CreateBuff()
        {
            return new RangeIncreaseBuff(rangeMultiplier, duration);
        }

        // �������� ��� ������� � ����������
        public float RangeMultiplier => rangeMultiplier;
        public float Duration => duration;
    }
}