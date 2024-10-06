using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Utils
{
    public class CoroutineUtils
    {
        public delegate void AfterWaitOperation();
        /// <summary>
        /// Wait for ${sec} and start operation
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static IEnumerator WaitAndOperationIEnum(float sec, AfterWaitOperation operation)
        {
            yield return new WaitForSeconds(sec);
            operation();
        }
        /// <summary>
        /// Move source transform to target transform for ${time}
        /// </summary>
        /// <param name="time"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerator TransformMove(float time, Transform source, Transform target, AfterWaitOperation afterEndOperation)
        {
            float elapsedTime = 0;
            Vector3 sourceInitPosition = source.localPosition;
            while(elapsedTime <= time)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
                source.localPosition = sourceInitPosition * (1 - (float)elapsedTime / (float)time) + target.localPosition * ((float)elapsedTime / (float)time);
            }
            afterEndOperation();
        }
    }
}