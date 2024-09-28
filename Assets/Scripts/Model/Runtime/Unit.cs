using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using Assets.Scripts.Utilities;
using Assets.Scripts.UnitBrains;
using Assets.Scripts.UnitBrains.Player;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit
    {
        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;

        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;

        public float Speed { get; private set; }
        public float AttackSpeed { get; private set; }
        public float AttackRange { get; private set; } // Добавлено поле для изменения радиуса атаки

        private bool _doubleShotEnabled = false;

        public Unit(UnitConfig config, Vector2Int startPos, UnitCoordinator coordinator)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            Speed = Config.MoveDelay;
            AttackSpeed = Config.AttackDelay;
            AttackRange = Config.AttackRange; // Инициализируем собственное поле для радиуса атаки
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _brain.SetCoordinator(coordinator);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        }

        public void Update(float deltaTime, float time)
        {
            if (IsDead)
                return;

            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            if (_nextMoveTime < time)
            {
                _nextMoveTime = time + Speed;
                Move();
            }

            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + AttackSpeed;
            }
        }

        private bool Attack()
        {
            var projectiles = _brain.GetProjectiles();
            if (projectiles == null || projectiles.Count == 0)
                return false;

            _pendingProjectiles.AddRange(projectiles);

            if (_doubleShotEnabled)
            {
                // Двойной выстрел, добавляем ещё один залп
                _pendingProjectiles.AddRange(_brain.GetProjectiles());
            }

            return true;
        }

        private void Move()
        {
            var targetPos = _brain.GetNextStep();
            var delta = targetPos - Pos;
            if (delta.sqrMagnitude > 2)
            {
                Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
                return;
            }

            if (_runtimeModel.RoMap[targetPos] ||
                _runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
            {
                return;
            }

            Pos = targetPos;
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }

        // Метод для изменения скорости передвижения
        public void ModifySpeed(float multiplier)
        {
            Speed *= multiplier;
        }

        // Метод для изменения скорости атаки
        public void ModifyAttackSpeed(float multiplier)
        {
            AttackSpeed *= multiplier;
        }

        // Метод для включения/выключения двойного выстрела
        public void SetDoubleShot(bool enable)
        {
            _doubleShotEnabled = enable;
        }

        // Метод для изменения радиуса атаки
        public void ModifyAttackRange(float multiplier)
        {
            AttackRange *= multiplier;
        }

        // Метод для восстановления радиуса атаки
        public void RestoreAttackRange()
        {
            AttackRange = Config.AttackRange; // Возвращаем значение к оригинальному из конфига
        }
    }
}