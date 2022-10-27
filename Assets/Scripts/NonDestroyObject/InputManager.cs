using System;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class InputManager : Singleton<InputManager>
    {
        public readonly EventHandler<SceneStatus> SceneSwitched = (sender, eventArgs) =>
        {
            
        };
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
        }

        // Update is called once per frame
        void Update()
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
                Debug.Log("ASDF");
                AdsManager.Instance.RequestRewardAds();
            }
        }
    }
}
