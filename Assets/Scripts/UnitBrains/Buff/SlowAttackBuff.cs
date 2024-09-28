using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class SlowAttackBuff : BaseBuff
    {
        private float slowAttackMultiplier = 0.5f;

        public SlowAttackBuff(float duration)
        {
            Duration = duration;
        }

        public override void ApplyBuff(Unit unit)
        {
            unit.ModifyAttackSpeed(slowAttackMultiplier);
        }

        public override void RemoveBuff(Unit unit)
        {
            unit.ModifyAttackSpeed(1 / slowAttackMultiplier);
        }

        public override bool CanApplyTo(Unit unit)
        {
            return true;
        }
    }
}