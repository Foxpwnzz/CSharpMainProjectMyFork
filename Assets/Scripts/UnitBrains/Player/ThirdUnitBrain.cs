using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class ThirdUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Ironclad Behemoth";

        private enum UnitState
        {
            Moving,
            Attacking,
            Switching
        }

        private UnitState currentState = UnitState.Moving;
        private float switchCooldown = 1.0f;
        private float switchTimer = 0.0f;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            if (currentState == UnitState.Switching)
            {
                switchTimer -= deltaTime;
                if (switchTimer <= 0)
                {
                    currentState = currentState == UnitState.Attacking ? UnitState.Moving : UnitState.Attacking;
                }
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (currentState == UnitState.Switching || currentState == UnitState.Moving)
            {
                return new List<Vector2Int>(); // Не выбираем цели для атаки
            }

            return base.SelectTargets();
        }

        public override Vector2Int GetNextStep()
        {
            if (currentState == UnitState.Switching || currentState == UnitState.Attacking)
            {
                return unit.Pos; // Не двигаемся
            }

            return base.GetNextStep();
        }

        public void SwitchState()
        {
            if (currentState != UnitState.Switching)
            {
                currentState = UnitState.Switching;
                switchTimer = switchCooldown;
            }
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            if (currentState == UnitState.Attacking)
            {
                base.GenerateProjectiles(forTarget, intoList);
            }
        }
    }
}
