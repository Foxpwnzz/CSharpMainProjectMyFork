using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class SpeedBoostBuff : BaseBuff
    {
        private float speedMultiplier = 1.5f;

        public SpeedBoostBuff(float duration)
        {
            Duration = duration;
        }

        public override void ApplyBuff(Unit unit)
        {
            unit.ModifySpeed(speedMultiplier);
        }

        public override void RemoveBuff(Unit unit)
        {
            unit.ModifySpeed(1 / speedMultiplier);
        }

        public override bool CanApplyTo(Unit unit)
        {
            return true;
        }
    }
}