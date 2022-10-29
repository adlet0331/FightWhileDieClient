using System.Collections;
using Combat;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class InputManager : Singleton<InputManager>
    {
        // Update is called once per frame
        private void Update()
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    AdsManager.Instance.RequestRewardAds();
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerManager.Instance.Player.Action(ObjectStatus.Attack);
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerManager.Instance.Player.Action(ObjectStatus.Dead);
            }
        }
    }
}
