using UnityEngine;

namespace NonDestroyObject
{
    public delegate void AutoChanged();
    public class AutoManager : Singleton<AutoManager>
    {
        [Header("Status")]
        [SerializeField] private bool isAuto;
        [SerializeField] private bool isFirst;

        public bool IsAuto
        {
            get => isAuto;
            set
            {
                if (!isAuto && value) isFirst = true;
                isAuto = value;
                AutoChanged?.Invoke();
            }
        }

        public bool IsFirst
        {
            get
            {
                if (isFirst)
                {
                    isFirst = false;
                    return true;
                }
                return isFirst;
            }
        }
        
        public event AutoChanged AutoChanged;
        
    }
}