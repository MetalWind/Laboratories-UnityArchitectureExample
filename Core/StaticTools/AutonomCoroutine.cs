using System.Collections;
using UnityEngine;

namespace Laboratory.Core
{
    public class AutonomCoroutine : MonoBehaviour
    {
        private static AutonomCoroutine instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AutonomCoroutineRunner");
                    _instance = go.AddComponent<AutonomCoroutine>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private static AutonomCoroutine _instance;

        public static Coroutine StartRoutine(IEnumerator enumerator)
        {
            return instance.StartCoroutine(enumerator);
        }

        public static void StopRoutine(Coroutine coroutine)
        {
            instance.StopCoroutine(coroutine);
        }
    }
}