using UnityEngine;
using Model.Runtime;

public abstract class BaseBuff : MonoBehaviour
{
    public float Duration { get; protected set; }
    public abstract void ApplyBuff(Unit unit);
    public abstract void RemoveBuff(Unit unit);
}
