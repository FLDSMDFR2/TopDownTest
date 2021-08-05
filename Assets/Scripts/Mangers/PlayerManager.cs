using System.Collections.Generic;

public class PlayerManager
{
    protected static List<BasePlayer> Players = new List<BasePlayer>();


    public static List<BasePlayer> GetPlayers()
    {
        return Players;
    }

    public static void AddPlayer(BasePlayer player)
    {
        Players.Add(player);
    }

    public static void RemovePlayer(BasePlayer player)
    {
        Players.Remove(player);
    }
}
