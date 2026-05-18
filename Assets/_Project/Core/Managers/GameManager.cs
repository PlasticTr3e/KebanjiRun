using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

namespace KebanjiRun.Core.Managers
{
    public enum GameState
    {
        PreEvent,
        Event,
        PostEvent,
        GameOver,
        MissionComplete,
        Pause,
        MainMenu
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        public event Action<GameState> OnGameStateChanged;
        public GameState CurrentState {get; private set;}
        public event Action OnFloodWarningClosed;
        public float WarningTriggerTime {get; private set;}

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ChangeState(GameState.PreEvent);
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            OnGameStateChanged?.Invoke(CurrentState);

            HandleStateChange(newState);
                
        }

        public void TriggerFloodWarningClosed()
        {
            OnFloodWarningClosed?.Invoke();
        }

        public void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.PreEvent:
                    break;
                
                case GameState.Event:
                    break;

                case GameState.PostEvent:
                    break;
            }
        }
    }
}