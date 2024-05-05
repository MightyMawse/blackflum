using System;
using System.Collections;
using System.Net;

public class Game
{
    public enum ROLE
    {
        PLAYER, DEALER
    }

    public enum UID_TYPE
    {
        SESSION, PLAYER
    }

    const uint maxPlayers = 5;
    public string sessionID { get; set; }
    public string sessionName { get; set; }
    public Dictionary<string, Player> nameToPlayer = new Dictionary<string, Player>();
    public List<Player> playersInGame = new List<Player>();

    public Game(string sessionName)
    {
        this.sessionName = sessionName;
        Task.Run(() =>
        {
            bool idMatch = false; // Make sure new ID doesnt already exist
            while (!idMatch)
            {
                Game[] arr   = Program.sessions.ToArray();
                string newID = Utils.GenerateUID(UID_TYPE.SESSION);
                if(Utils.GetClassByPropertyValue(arr, "sessionID", newID) == null)
                {
                    this.sessionID = newID; // Assign new id
                    idMatch = true;
                }
            }
        });
        Program.sessions.Add(this); // Add to open sessions
    }


    public Player GetPlayerByID(string id)
    {
        foreach(Player p in playersInGame)
        {
            if(p.playerId.Equals(id))
                return p;
        }
        return null;
    }

    public void JoinSession(Player player)
    {
        playersInGame.Add(player);
    }
}
