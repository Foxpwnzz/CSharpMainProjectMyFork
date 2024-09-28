using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class SlowMovementBuff : BaseBuff
    {
        private float slowMultiplier = 0.5f;

        public SlowMovementBuff(float duration)
        {
            Duration = duration;
        }

        public override void ApplyBuff(Unit unit)
        {
            unit.ModifySpeed(slowMultiplier);
        }

        public override void RemoveBuff(Unit unit)
        {
            unit.ModifySpeed(1 / slowMultiplier);
        }

        public override bool CanApplyTo(Unit unit)
        {
            return true;
        }
    }
}