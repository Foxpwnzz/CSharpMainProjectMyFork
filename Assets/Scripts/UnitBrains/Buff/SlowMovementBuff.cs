using UnityEngine;
using Model.Runtime;

public class SlowMovementBuff : BaseBuff
{
    public float SlowMultiplier = 0.5f;

    public SlowMovementBuff(float duration)
    {
        Duration = duration;
    }

    public override void ApplyBuff(Unit unit)
    {
        unit.Speed *= SlowMultiplier; // Замедляем скорость
    }

    public override void RemoveBuff(Unit unit)
    {
        unit.Speed /= SlowMultiplier; // Восстанавливаем исходную скорость
    }
}
