public enum GameMode { SinglePlayer, Multiplayer }

[System.Serializable]
public class GameConfig
{
    public GameMode mode;
    public int playerCount;
    public int player1Money;
    public int player2Money;
}