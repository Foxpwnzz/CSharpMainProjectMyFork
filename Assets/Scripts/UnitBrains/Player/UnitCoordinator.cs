using Model.Runtime.ReadOnly;
using Model;
using UnityEngine;
using Utilities;
using System.Linq;

public class UnitCoordinator
{
    private static UnitCoordinator _instance;
    private IReadOnlyRuntimeModel _runtimeModel;
    private TimeUtil _timeUtil;

    public static UnitCoordinator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UnitCoordinator();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private void Initialize()
    {
        _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        _timeUtil = ServiceLocator.Get<TimeUtil>();
    }

    public IReadOnlyUnit GetRecommendedTarget()
    {
        var playerBase = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
        var enemiesOnPlayerSide = _runtimeModel.RoBotUnits.Where(u => u.Pos.y < _runtimeModel.RoMap.Height / 2).ToList();

        if (enemiesOnPlayerSide.Any())
        {
            return enemiesOnPlayerSide.OrderBy(u => Vector2Int.Distance(u.Pos, playerBase)).First();
        }

        if (_runtimeModel.RoBotUnits.Any())
        {
            return _runtimeModel.RoBotUnits.OrderBy(u => u.Health).First();
        }
        else
        {
            // Обработайте случай, когда врагов нет
            return null;
        }
    }

    public Vector2Int GetRecommendedPoint()
    {
        var playerBase = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
        var enemiesOnPlayerSide = _runtimeModel.RoBotUnits.Where(u => u.Pos.y < _runtimeModel.RoMap.Height / 2).ToList();

        if (enemiesOnPlayerSide.Any())
        {
            // Точка перед базой
            return new Vector2Int(playerBase.x, playerBase.y + 2); // Смещаем перед базой
        }

        if (_runtimeModel.RoBotUnits.Any())
        {
            // Точка на расстоянии атаки от ближайшего врага
            var nearestEnemy = _runtimeModel.RoBotUnits.OrderBy(u => Vector2Int.Distance(u.Pos, playerBase)).First();
            return new Vector2Int(nearestEnemy.Pos.x, nearestEnemy.Pos.y + 3); // Смещение для расстояния выстрела
        }

        // Если врагов нет, возвращаем точку базы
        return playerBase;
    }
}