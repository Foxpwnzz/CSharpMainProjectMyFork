using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnitBrains.Pathfinding;
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
        private float switchCooldown = 0.0f;
        private float switchTimer = 1.0f;

        private AStarUnitPath _path;  // Поле для хранения пути

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            // Получаем цель и целевую точку
            var target = UnitCoordinator.Instance.GetRecommendedTarget();
            var recommendedPoint = UnitCoordinator.Instance.GetRecommendedPoint();

            // Проверка состояния переключения
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
                    // Если цель в зоне атаки, переключаемся на атаку
                    if (target != null && Vector2Int.Distance(unit.Pos, target.Pos) <= unit.Config.AttackRange * 2)
                    {
                        SwitchToState(UnitState.SwitchingToAttack);
                    }
                    else
                    {
                        if (_path == null || _path.EndPoint != recommendedPoint)
                        {
                            _path = new AStarUnitPath(runtimeModel, unit.Pos, recommendedPoint);
                        }

                        // Получаем следующий шаг
                        var nextStep = _path.GetNextStepFrom(unit.Pos);

                        // Перемещаем юнит на следующий шаг
                        unit.UpdateMove(nextStep);
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