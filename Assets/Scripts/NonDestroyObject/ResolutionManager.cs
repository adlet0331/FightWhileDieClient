using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class ResolutionManager : Singleton<ResolutionManager>
    {
        [Header(("Size"))]
        [SerializeField] private float FixedWidth;
        [SerializeField] private float HeightTopLimit;
        [Header("Components")]
        [SerializeField] private Camera MainCamera;
        [SerializeField] private Canvas MainCanvas;
        private void Start()
        {
            Debug.Log(MainCamera.aspect);
            MainCamera.orthographicSize = FixedWidth / ((float)Screen.width / Screen.height) < 8 ? FixedWidth / ((float)Screen.width / Screen.height) : 8;
        }
    }
}
