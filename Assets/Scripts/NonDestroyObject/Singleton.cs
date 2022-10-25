using UnityEngine;

/* 게임 전체에서 한 개만 존재하는 Singleton 오브젝트에 부착
 * 씬이 넘어가도 파괴 안됨
 * 
 */
namespace Managers
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static bool _shutdown = false;
        private static object _lock = new object();
        private static T _instance;
        private void Awake() {
            lock (_lock) {
                if (_instance == null) {
                    T[] componentList = GetComponents<T>();
                    if (componentList.Length != 0 && componentList.Length > 1) {
                        Debug.Log("Error! " + typeof(T).ToString() + " have more than one Components");
                    }
                    _instance = componentList[0];
                    GameObject singletonObject = this.gameObject;

                    DontDestroyOnLoad(singletonObject);
                    //Debug.Log(typeof(T).ToString() + "Singleton well made.");
                }
            }
        }

        public static T Instance {
            get {
                if (_shutdown) {
                    Debug.Log("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                    return null;
                }
                return _instance;
            }
        }

        private void OnApplicationQuit() {
            _shutdown = true;
        }

        private void OnDestroy() {
            _shutdown = true;
        }
    }
}
