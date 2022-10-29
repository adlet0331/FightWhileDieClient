using System;
using Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace NonDestroyObject
{
    public class PlayerManager : Managers.Singleton<PlayerManager>
    {
        public CombatObject Player;

        private void Start()
        {
            if (Player.IsUnityNull())
            {
                Player = GetComponent<CombatObject>();
            }
        }
    }
}