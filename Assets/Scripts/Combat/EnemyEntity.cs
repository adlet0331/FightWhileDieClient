using UnityEngine;

namespace Combat
{
    public class EnemyEntity : CombatEntityParent
    {
        public float CurrentHpRatio => (float)currentHp / (float)maxHp;
    }
}