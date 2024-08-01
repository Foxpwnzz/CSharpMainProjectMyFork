using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

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

        private static int unitCounter = 0; // Статическое поле для присвоения номеров юнитов
        private int unitNumber; // Поле для номера юнита
        private const int MaxTargetsToConsider = 3; // Константа для максимального количества целей для рассмотрения

        public SecondUnitBrain()
        {
            unitNumber = ++unitCounter; // Присваиваем уникальный номер каждому юниту
        }

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
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = GetReachableTargets();
            if (result.Count > 0)
            {
                // Сортируем цели по дистанции до базы
                SortByDistanceToOwnBase(result);

                // Оставляем в списке ближайшие MaxTargetsToConsider цели
                if (result.Count > MaxTargetsToConsider)
                {
                    result = result.GetRange(0, MaxTargetsToConsider);
                }

                // Рассчитываем номер цели для атаки
                int targetIndex = (unitNumber - 1) % result.Count;
                Vector2Int target = result[targetIndex];

                // Очищаем список и добавляем в него только выбранную цель
                result.Clear();
                result.Add(target);
            }
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
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
