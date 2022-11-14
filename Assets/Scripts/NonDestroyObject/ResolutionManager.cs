using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class ResolutionManager : Singleton<ResolutionManager>
    {
        [Header(("Size"))]
        [SerializeField] private int FixedWidth;
        [Header("Components")]
        [SerializeField] private Camera MainCamera;
        [SerializeField] private Canvas MainCanvas;
        private void Start()
        {
            Debug.Log(MainCamera.aspect);
            MainCamera.orthographicSize = FixedWidth / ((float)Screen.width / Screen.height);
        }
    }
}
