using Assets.Scripts.UnitBrains.Buff;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.BuffP
{

    public class DoubleShotBuffSettings : MonoBehaviour
    {
        [SerializeField]
        private float duration = 10f;

        // Метод для создания DoubleShotBuff с параметрами из инспектора
        public DoubleShotBuff CreateBuff()
        {
            return new DoubleShotBuff(duration);
        }

        // Свойство для доступа к параметрам
        public float Duration => duration;
    }
}