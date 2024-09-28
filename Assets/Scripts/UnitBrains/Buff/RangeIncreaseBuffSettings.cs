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

        // Метод для создания RangeIncreaseBuff с параметрами из инспектора
        public RangeIncreaseBuff CreateBuff()
        {
            return new RangeIncreaseBuff(rangeMultiplier, duration);
        }

        // Свойства для доступа к параметрам
        public float RangeMultiplier => rangeMultiplier;
        public float Duration => duration;
    }
}