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
            if (Input.touchCount == 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    
                }
            }
        }
    }
}
