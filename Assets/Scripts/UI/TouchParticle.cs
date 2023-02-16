using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class TouchParticle : MonoBehaviour
    {
        private void Start()
        {
            DestroyAfter1Second().Forget();
        }

        private async UniTaskVoid DestroyAfter1Second()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

            await UniTask.SwitchToMainThread();
            Destroy(gameObject);
        }
    }
}