public enum GameMode { SinglePlayer, Multiplayer }
public enum GameDifficulty { Easy, Medium, Hard }

[System.Serializable]
public class GameConfig
{
    public GameMode mode;
    public GameDifficulty difficulty;
    public int playerCount;
    public int player1Money;
    public int player2Money;
}