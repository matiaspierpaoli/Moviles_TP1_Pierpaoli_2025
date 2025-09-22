using System;
public enum GameState { Calibrating, Playing, Paused, Finished }
public enum PlayerSide { Left, Right, Middle, Default}

public static class GameSignals
{
    public static event Action SingleplayerActive;
    public static event Action MultiplayerActive;
    public static event Action<GameState> GameStateChanged;
    public static event Action<GameDifficulty> GameDiffucltyChanged;
    public static event Action<int> PlayerSelected;
    public static event Action CalibrationStarted;
    public static event Action MatchStarted;
    public static event Action MatchEnded;
    public static event Action<int> PlayerFinished;
    public static event Action<float> CountdownTick;

    public static event Action<int, PlayerSide> PlayerSideAssigned;

    public static event Action<int> CalibrationDone; // playerId
    public static event Action MatchCountdownStarted;

    public static void RaiseSingleplayerActive() => SingleplayerActive?.Invoke();
    public static void RaiseMultiplayerActive() => MultiplayerActive?.Invoke();
    public static void RaiseGameState(GameState s) => GameStateChanged?.Invoke(s);
    public static void RaiseGameDifficulty(GameDifficulty d) => GameDiffucltyChanged?.Invoke(d);
    public static void RaisePlayerSelected(int id) => PlayerSelected?.Invoke(id);
    public static void RaiseCalibrationStarted() => CalibrationStarted?.Invoke();
    public static void RaiseCalibrationDone(int id) => CalibrationDone?.Invoke(id);
    public static void RaiseMatchCountdownStarted() => MatchCountdownStarted?.Invoke();
    public static void RaiseMatchStarted() => MatchStarted?.Invoke();
    public static void RaiseMatchEnded() => MatchEnded?.Invoke();
    public static void RaisePlayerFinished(int id) => PlayerFinished?.Invoke(id);
    public static void RaiseCountdown(float t) => CountdownTick?.Invoke(t);
    public static void RaisePlayerSideAssigned(int id, PlayerSide side) => PlayerSideAssigned?.Invoke(id, side);

}
