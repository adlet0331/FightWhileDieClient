using System.Collections;
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
            for (int i = 0; i < (int)(sec / Time.fixedDeltaTime); i++)
            {
                yield return new WaitForFixedUpdate();
            }
            operation();
        }
        /// <summary>
        /// Move source transform to target transform for ${time}
        /// </summary>
        /// <param name="time"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerator TransformMove(float time, Transform source, Transform target)
        {
            int count = (int)(time / (Time.deltaTime * 2));
            Vector3 interval = (target.position - source.position) / count;
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(Time.deltaTime * 2);
                source.position += interval;
                
            }
        }
    }
}