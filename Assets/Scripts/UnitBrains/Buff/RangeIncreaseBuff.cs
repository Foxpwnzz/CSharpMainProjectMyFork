using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class RangeIncreaseBuff : BaseBuff
    {
        private float _rangeMultiplier;
        private float _duration;

        public RangeIncreaseBuff(float rangeMultiplier, float duration)
        {
            _rangeMultiplier = rangeMultiplier;
            _duration = duration;
            Duration = duration;
        }

        public override bool CanApplyTo(Unit unit)
        {
            // Проверяем, что имя юнита соответствует тому, что использует ThirdUnitBrain
            return unit.Config.Name == "Ironclad Behemoth";
        }

        public override void ApplyBuff(Unit unit)
        {
            unit.ModifyAttackRange(_rangeMultiplier);
        }

        public override void RemoveBuff(Unit unit)
        {
            unit.ModifyAttackRange(1 / _rangeMultiplier);
        }
    }
}