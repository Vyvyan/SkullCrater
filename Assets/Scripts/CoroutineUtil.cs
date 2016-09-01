using UnityEngine;
using System.Collections;

public static class CoroutineUtil
{
    public static IEnumerator WaitForRealTime(float delay)
    {
        while (true)
        {
            float pauseEndTime = Time.unscaledTime + delay;
            while (Time.unscaledTime < pauseEndTime)
            {
                yield return 0;
            }
            break;
        }
    }
}
