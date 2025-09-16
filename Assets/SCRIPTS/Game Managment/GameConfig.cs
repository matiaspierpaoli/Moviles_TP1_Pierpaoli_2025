public enum GameMode { SinglePlayer, Multiplayer }

[System.Serializable]
public struct GameConfig
{
    public GameMode mode;
    public int playerCount;
}