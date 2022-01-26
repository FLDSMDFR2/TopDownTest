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
        //TODO: CHANGES THIS BUT FOR NOW WE ONLY HAVE 1 PLAYER SO SET THE UI EVENTS TO LISTEND FOR THIS PLAYERS EVENTS
        UIEvents.SetPlayerId(player.ID);
        Players.Add(player);
    }

    public static void RemovePlayer(BasePlayer player)
    {
        Players.Remove(player);
    }
}
