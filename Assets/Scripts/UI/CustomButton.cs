using NonDestroyObject;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Class that registers PlayClip to the Button component on the same GameObject
    /// </summary>
    public class CustomButton : MonoBehaviour
    {
        [SerializeField] private ClipName clipName = ClipName.ButtonClick;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(PlayButtonSound);
            }
            else
            {
                Debug.LogWarning($"[CustomButton] Button component not found on {gameObject.name}");
            }
        }

        private void PlayButtonSound()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClip(clipName);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(PlayButtonSound);
            }
        }
    }
}
