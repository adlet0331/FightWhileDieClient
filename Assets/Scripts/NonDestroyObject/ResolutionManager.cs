using System;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class ResolutionManager : Singleton<ResolutionManager>
    {
        [Header(("Size"))]
        [SerializeField] private int FixedHeight;
        [Header("Components")]
        [SerializeField] private Camera MainCamera;
        [SerializeField] private Canvas MainCanvas;
        private void Start()
        {
#if UNITY_ANDROID
            MainCamera.orthographic = true;
            MainCamera.orthographicSize = 5;
# endif
            
        }
    }
}
