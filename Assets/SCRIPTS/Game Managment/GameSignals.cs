using System;
public enum GameState { Boot, Calibrating, Playing, Paused, Finished }
public enum PlayerSide { Left, Right, Default}

public static class GameSignals
{
    public static event Action<GameState> GameStateChanged;
    public static event Action CalibrationStarted;
    public static event Action MatchStarted;
    public static event Action MatchEnded;
    public static event Action<float> CountdownTick;

    public static event Action<int, bool> TogglePlayerUI; // (playerId, visible)
    public static event Action<int, PlayerSide> PlayerSideAssigned;

    public static event Action<int> CalibrationDone; // playerId

    public static void RaiseGameState(GameState s) => GameStateChanged?.Invoke(s);
    public static void RaiseCalibrationStarted() => CalibrationStarted?.Invoke();
    public static void RaiseCalibrationDone(int id) => CalibrationDone?.Invoke(id);
    public static void RaiseMatchStarted() => MatchStarted?.Invoke();
    public static void RaiseMatchEnded() => MatchEnded?.Invoke();
    public static void RaiseCountdown(float t) => CountdownTick?.Invoke(t);
    public static void RaiseTogglePlayerUI(int id, bool v) => TogglePlayerUI?.Invoke(id, v);
    public static void RaisePlayerSideAssigned(int id, PlayerSide side) => PlayerSideAssigned?.Invoke(id, side);

}
