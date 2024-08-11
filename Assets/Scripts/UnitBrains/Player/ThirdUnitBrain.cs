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
            SwitchingToAttack,
            SwitchingToMove
        }

        private UnitState currentState = UnitState.Moving;
        private float switchCooldown = 1.0f;
        private float switchTimer = 0.0f;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            switch (currentState)
            {
                case UnitState.SwitchingToAttack:
                case UnitState.SwitchingToMove:
                    switchTimer -= deltaTime;
                    if (switchTimer <= 0)
                    {
                        if (currentState == UnitState.SwitchingToAttack)
                        {
                            currentState = UnitState.Attacking;
                        }
                        else if (currentState == UnitState.SwitchingToMove)
                        {
                            currentState = UnitState.Moving;
                        }
                    }
                    break;

                case UnitState.Moving:
                    if (HasTargets())
                    {
                        SwitchToState(UnitState.SwitchingToAttack);
                    }
                    break;

                case UnitState.Attacking:
                    if (!HasTargets())
                    {
                        SwitchToState(UnitState.SwitchingToMove);
                    }
                    break;
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (currentState == UnitState.SwitchingToAttack || currentState == UnitState.Moving)
            {
                return new List<Vector2Int>();
            }

            return base.SelectTargets();
        }

        public override Vector2Int GetNextStep()
        {
            if (currentState == UnitState.SwitchingToAttack || currentState == UnitState.Attacking)
            {
                return unit.Pos;
            }

            return base.GetNextStep();
        }

        private void SwitchToState(UnitState newState)
        {
            currentState = newState;
            switchTimer = switchCooldown;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            if (currentState == UnitState.Attacking)
            {
                base.GenerateProjectiles(forTarget, intoList);
            }
        }

        private bool HasTargets()
        {
            var targets = base.SelectTargets();
            return targets != null && targets.Count > 0;
        }
    }
}
