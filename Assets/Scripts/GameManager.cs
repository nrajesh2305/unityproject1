using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGameState
{
    void OnEnter(GameManager manager);
    void OnExit(GameManager manager);
}

public enum GameStateType
{
    InMainMenu,
    PreparingLevel,
    PlayingLevel,
    LevelPaused,
    PlayerDied,
    LevelCompleted,
    PlayingCredits
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameStateType CurrentState { get; private set; }
    private IGameState _currentStateHandler;
    private int _currentSceneIndex = 0;
    private readonly int _totalNumberOfScenes = SceneManager.sceneCountInBuildSettings;

    public event Action<GameStateType> OnGameStateChange;

    private readonly Dictionary<GameStateType, IGameState> _gameStateHandlers = new Dictionary<GameStateType, IGameState>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        QualitySettings.vSyncCount = 0;
        InitializeGameStates();
    }

    private void InitializeGameStates()
    {
        _gameStateHandlers.Add(GameStateType.InMainMenu, new InMainMenuState());
        _gameStateHandlers.Add(GameStateType.PreparingLevel, new InPreparingLevelState());
        _gameStateHandlers.Add(GameStateType.PlayingLevel, new InPlayingLevelState());
        _gameStateHandlers.Add(GameStateType.LevelPaused, new InLevelPausedState());
        _gameStateHandlers.Add(GameStateType.PlayerDied, new InPlayerDiedState());
        _gameStateHandlers.Add(GameStateType.LevelCompleted, new InLevelCompletedState());
        _gameStateHandlers.Add(GameStateType.PlayingCredits, new InPlayingCreditsState());
    }

    public void UpdateGameState(GameStateType newState)
    {
        if (CurrentState == newState) return;

        _currentStateHandler?.OnExit(this);
        OnGameStateChange?.Invoke(newState);

        CurrentState = newState;
        _currentStateHandler = _gameStateHandlers[newState];
        _currentStateHandler?.OnEnter(this);
    }

    public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void SwitchToScene(string sceneName, bool updateCurrentSceneIndex = true)
    {
        SceneManager.LoadScene(sceneName);
        if (updateCurrentSceneIndex) UpdateCurrentSceneIndex(sceneName);
    }

    public void SwitchToScene(int sceneIndex, bool updateCurrentSceneIndex = true)
    {
        SceneManager.LoadScene(sceneIndex);
        if (updateCurrentSceneIndex) _currentSceneIndex = sceneIndex;
    }

    private void UpdateCurrentSceneIndex(string sceneName)
    {
        _currentSceneIndex = SceneManager.GetSceneByName(sceneName).buildIndex;
    }
}
