using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

public class Player
{
    public enum ROLE
    {
        PLAYER, DEALER
    }

    public string playerId;
    public string playerName;
    public string sessionID;
    public float playerCredit = 100;
    public ROLE role;

    public Player(string playerName, ROLE role)
    {
        this.playerName = playerName;
        this.playerId = Utils.GenerateUID(Game.UID_TYPE.PLAYER);
    }
}
