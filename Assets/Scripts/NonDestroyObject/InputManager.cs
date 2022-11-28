using System.Collections;
using System.Collections.Concurrent;
using Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace NonDestroyObject
{
    public class InputManager : Singleton<InputManager>
    {
        public bool touched;
        public bool Blocked;
        
        // Update is called once per frame
        private void Update()
        {
            touched = false;
            if (Blocked) return;
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    PlayerCombatManager.Instance.Player.Action(ObjectStatus.Attack);
                    touched = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerCombatManager.Instance.Player.Action(ObjectStatus.Attack);
            }
        }
    }
}
