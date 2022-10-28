using Managers;

namespace NonDestroyObject
{
    public enum UIStatus
    {
        Idle = 0,
        Starting = 1,
        Fighting = 2,
        Shop = 3,
        Equip = 4,
        Enforcement = 5,
    }
    public class UIManager : Singleton<UIManager>
    {
        private UIStatus _currentStatus;
        
        public UIStatus CurrentStatus => _currentStatus;

        public void ChangeUIStatus(UIStatus status)
        {
            _currentStatus = status;
        }
        
    }
}