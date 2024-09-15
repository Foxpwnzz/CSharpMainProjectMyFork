using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;
using UnitBrains.Pathfinding;
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

        private AStarUnitPath _path;  // Добавляем поле для хранения пути A*

        private static int unitCounter = 0; // Статическое поле для присвоения номеров юнитов
        private int unitNumber; // Поле для номера юнита
        private const int MaxTargetsToConsider = 3; // Константа для максимального количества целей для рассмотрения

        public SecondUnitBrain()
        {
            unitNumber = ++unitCounter; // Присваиваем уникальный номер каждому юниту
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
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
        }

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / OverheatCooldown;
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }

            // Получаем цель и целевую точку
            var target = UnitCoordinator.Instance.GetRecommendedTarget();
            var recommendedPoint = UnitCoordinator.Instance.GetRecommendedPoint();

            // Если юнит может атаковать цель, атакуем
            if (target != null && Vector2Int.Distance(unit.Pos, target.Pos) <= unit.Config.AttackRange * 2)
            {
                GenerateProjectiles(target.Pos, new List<BaseProjectile>());
            }
            else
            {
                // Если путь еще не был рассчитан или нужен новый путь, создаем путь
                if (_path == null || _path.EndPoint != recommendedPoint)
                {
                    _path = new AStarUnitPath(runtimeModel, unit.Pos, recommendedPoint);
                }

                // Получаем следующий шаг по пути
                var nextStep = _path.GetNextStepFrom(unit.Pos);
                unit.MoveTo(nextStep);  // Перемещаем юнита на следующий шаг
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets();
            if (result.Count > 0)
            {
                SortByDistanceToOwnBase(result);

                if (result.Count > MaxTargetsToConsider)
                {
                    result = result.GetRange(0, MaxTargetsToConsider);
                }

                int targetIndex = (unitNumber - 1) % result.Count;
                Vector2Int target = result[targetIndex];

                result.Clear();
                result.Add(target);
            }
            return result;
        }

        private int GetTemperature()
        {
            return _overheated ? (int)OverheatTemperature : (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature)
            {
                _overheated = true;
            }
        }

        private void SortByDistanceToOwnBase(List<Vector2Int> targets)
        {
            targets.Sort((a, b) => DistanceToOwnBase(a).CompareTo(DistanceToOwnBase(b)));
        }
    }
}