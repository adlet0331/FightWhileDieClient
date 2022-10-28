using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class InputManager : Singleton<InputManager>
    {
        private void Start()
        {
            _waitAfterAttack = null;
        }

        private IEnumerator _waitAfterAttack;
        IEnumerator WaitAndReturnToIdle(float second)
        {
            yield return new WaitForSeconds(second);
            PlayerManager.Instance.SetAnimation(PlayerStatus.Idle);
            _waitAfterAttack = null;
        }

        public void WaitAndReturnToIdleAttack()
        {
            if (_waitAfterAttack != null)
            {
                StopCoroutine(_waitAfterAttack);
            }
            _waitAfterAttack = WaitAndReturnToIdle(0.25f);
            StartCoroutine(_waitAfterAttack);
        }

        // Update is called once per frame
        void Update()
        {
            if (_waitAfterAttack != null) return;
            
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    AdsManager.Instance.RequestRewardAds();
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerManager.Instance.SetAnimation(PlayerStatus.Attack);
                WaitAndReturnToIdleAttack();
            }
        }
    }
}
