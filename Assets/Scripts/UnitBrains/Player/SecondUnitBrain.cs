﻿using System.Collections.Generic;
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
                // 1. Определяем ближайшую к базе цель
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
                // 2. Очищаем список и добавляем в него ближайшую цель
                result.Clear();
                result.Add(closestTarget);
            }
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
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
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}