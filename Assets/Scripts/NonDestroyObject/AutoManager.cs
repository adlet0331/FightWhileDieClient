using UnityEngine;

namespace NonDestroyObject
{
    public delegate void AutoChanged();
    public class AutoManager : Singleton<AutoManager>
    {
        [Header("Status")]
        [SerializeField] private bool isAuto;

        public bool IsAuto
        {
            get => isAuto;
            set
            {
                isAuto = value;
                AutoChanged?.Invoke();
            }
        }
        public event AutoChanged AutoChanged;
        
    }
}