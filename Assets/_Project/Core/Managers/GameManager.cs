using System;
using UnityEngine;
using UnityEngine.XR;

namespace Kebanjirun.Core.Managers
{
    public enum GameState
    {
        PreEvent,
        Event,
        PostEvent,
        GameOver,
        MissionComplete
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        public event Action<GameState> OnGameStateChanged;
        public GameState CurrentState {get; private set;}

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

                case GameState.GameOver:
                    break;
                
                case GameState.MissionComplete:
                    break;
            }
        }
    }
}