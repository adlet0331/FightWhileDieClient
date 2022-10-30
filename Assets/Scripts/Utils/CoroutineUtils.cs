using System.Collections;
using UnityEngine;

namespace Utils
{
    public class CoroutineUtils
    {
        public delegate void AfterWaitOperation();
        // Wait For Sec and start operation.
        public static IEnumerator WaitAndOperationIEnum(float sec, AfterWaitOperation operation)
        {
            yield return new WaitForSeconds(sec);
            operation();
        }
    }
}