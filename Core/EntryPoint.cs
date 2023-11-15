using UnityEngine;
using Zenject;

namespace Laboratory.Core
{
    public class EntryPoint : MonoBehaviour
    {
        [Inject] private GameStateMachineBase _gsm;

        void Awake()
        {
            _gsm.StartGame();
        }
    }
}
