using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _outOfReachTargets = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            int currentTemperature = GetTemperature();
            if (currentTemperature >= (int)OverheatTemperature)
            {
                return;
            }
            IncreaseTemperature();
            int projectileCount = currentTemperature + 1;
            for (int i = 0; i < projectileCount; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (_outOfReachTargets.Count == 0 || IsInAttackRange(_outOfReachTargets[0]))
            {
                return unit.Pos;
            }
            return unit.Pos.CalcNextStepTowards(_outOfReachTargets[0]);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = new List<Vector2Int>(GetAllTargets());
            _outOfReachTargets.Clear();

            if (result.Count > 0)
            {
                Vector2Int closestTarget = result[0];
                float closestDistance = DistanceToOwnBase(closestTarget);

                for (int i = 1; i < result.Count; i++)
                {
                    float distance = DistanceToOwnBase(result[i]);
                    if (distance < closestDistance)
                    {
                        closestTarget = result[i];
                        closestDistance = distance;
                    }
                }

                if (IsReachable(closestTarget))
                {
                    result.Clear();
                    result.Add(closestTarget);
                }
                else
                {
                    _outOfReachTargets.Add(closestTarget);
                }
            }
            else
            {
                var enemyBase = GetEnemyBasePosition();
                _outOfReachTargets.Add(enemyBase);
            }

            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }

        private bool IsReachable(Vector2Int target)
        {

            return true; // Заглушка
        }

        private bool IsInAttackRange(Vector2Int target)
        {
            return true; // Заглушка
        }

        private Vector2Int GetEnemyBasePosition()
        {
            if (IsPlayerUnitBrain)
            {
                return runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            }
            else
            {
                return runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
            }
        }
    }
}
