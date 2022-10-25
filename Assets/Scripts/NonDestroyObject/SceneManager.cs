using System;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public enum SceneStatus
    {
        GaneScene = 0
    }

    public class SceneManager : Singleton<SceneManager>
    {
        public event EventHandler<SceneStatus> SwitchSceneNotify;
        
        public void SwitchScene(SceneStatus sceneStatus)
        {
            //Scene Switch Process
            
            // Notice Scene is Switched
            SwitchSceneNotify?.Invoke(this, sceneStatus);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
            this.SwitchSceneNotify += InputManager.Instance.SceneSwitched;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
