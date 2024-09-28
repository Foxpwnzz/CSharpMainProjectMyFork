using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Buff
{
    public class AttackSpeedBoostBuff : BaseBuff
    {
        private float attackSpeedMultiplier = 1.5f;

        public AttackSpeedBoostBuff(float duration)
        {
            Duration = duration;
        }

        public override void ApplyBuff(Unit unit)
        {
            // Используем метод для модификации скорости атаки
            unit.ModifyAttackSpeed(attackSpeedMultiplier);
        }

        public override void RemoveBuff(Unit unit)
        {
            // Используем метод для возврата к оригинальной скорости атаки
            unit.ModifyAttackSpeed(1 / attackSpeedMultiplier);
        }

        public override bool CanApplyTo(Unit unit)
        {
            return true; // Разрешаем применять бафф ко всем юнитам или уточните по типам
        }
    }
}