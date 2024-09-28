using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class DoubleShotBuff : BaseBuff
    {
        private float _duration;

        public DoubleShotBuff(float duration)
        {
            _duration = duration;
            Duration = duration;
        }

        public override bool CanApplyTo(Unit unit)
        {
            // Проверяем, что имя юнита соответствует тому, что использует SecondUnitBrain
            return unit.Config.Name == "Cobra Commando";
        }

        public override void ApplyBuff(Unit unit)
        {
            unit.SetDoubleShot(true);
        }

        public override void RemoveBuff(Unit unit)
        {
            unit.SetDoubleShot(false);
        }
    }
}