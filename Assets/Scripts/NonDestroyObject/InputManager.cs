using System.Collections;
using System.Collections.Concurrent;
using Combat;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class InputManager : Singleton<InputManager>
    {
        public bool Blocked
        {
            get => blocked;
            set => blocked = value;
        }
        [SerializeField] private bool blocked;
        
        // Update is called once per frame
        private void Update()
        {
            if (blocked) return;
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    PlayerCombatManager.Instance.Player.Action(ObjectStatus.Attack);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerCombatManager.Instance.Player.Action(ObjectStatus.Attack);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerCombatManager.Instance.Player.Action(ObjectStatus.Dead);
            }
        }
    }
}
