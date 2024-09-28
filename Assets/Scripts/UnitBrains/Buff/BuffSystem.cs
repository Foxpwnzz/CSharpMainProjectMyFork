using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Runtime;
using Assets.Scripts.UnitBrains.Buff;

namespace Assets.Scripts.UnitBrains.Player
{
    public class BuffSystem : MonoBehaviour
    {
        public static BuffSystem Instance { get; private set; }

        private Dictionary<Unit, List<BaseBuff>> activeBuffs = new Dictionary<Unit, List<BaseBuff>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddBuff(Unit unit, BaseBuff buff)
        {
            if (buff.CanApplyTo(unit))
            {
                if (!activeBuffs.ContainsKey(unit))
                {
                    activeBuffs[unit] = new List<BaseBuff>();
                }
                activeBuffs[unit].Add(buff);
                buff.ApplyBuff(unit);
                StartCoroutine(HandleBuff(unit, buff));
            }
        }

        private IEnumerator HandleBuff(Unit unit, BaseBuff buff)
        {
            yield return new WaitForSeconds(buff.Duration);
            buff.RemoveBuff(unit);
            activeBuffs[unit].Remove(buff);
        }
    }
}