using Microsoft.AspNetCore.Http.Json;
using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

public partial class Program
{
    private static WebApplicationBuilder builder;
    private static WebApplication app;
    public static List<Game> sessions = new List<Game>();
    public static void Main(string[] args)
    {
        builder = WebApplication.CreateBuilder(args);
        app = builder.Build();

        SetRoutes();

        app.UseStaticFiles();
        app.Run();
    }

    public static Game GetSessionByID(string sessionID)
    {
        foreach(Game g in sessions)
        {
            if(g.sessionID.Equals(sessionID))
                return g;
        }
        return null;
    }

    public static Game GetSessionByName(string sessionName)
    {
        foreach (Game g in sessions)
        {
            if (g.sessionName.Equals(sessionName))
                return g;
        }
        return null;
    }

    private static void SetRoutes()
    {
        app.MapGet("/", async (context) => {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/pages/main.html");
        });

        app.MapGet("/game_p", async (context) => {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/pages/game_p.html");
        });

        app.MapGet("/game_d", async (context) => {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/pages/game_d.html");
        });

        // Get players via sessionID
        app.MapPost("/get_players", async (context) =>
        {
            string sessionID = await new StreamReader(context.Request.Body).ReadToEndAsync();
            Game session = Utils.GetClassByPropertyValue<Game>(sessions.ToArray(), "sessionID", sessionID);

            string sessionPlayersJson = JsonConvert.SerializeObject(session.playersInGame);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(sessionPlayersJson);
        });

        // Get sessionID via name
        app.MapPost("/get_session_id", async (context) =>
        {
            if(sessions.Count > 0)
            {
                string sessionName = await new StreamReader(context.Request.Body).ReadToEndAsync();
                string sessionID = Utils.GetClassByPropertyValue(sessions.ToArray(), "sessionName", sessionName).sessionID;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(sessionID);
            }
            else { await context.Response.WriteAsync(JsonConvert.SerializeObject("NO_SESSIONS_AVAILABLE")); }
        });

        app.MapPost("/login", async (context) => {
            string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            string[] loginInfo = Utils.SplitLoginInfo(body);

            if (loginInfo[1].Equals("DEALER"))
            {
                if(Utils.GetClassByPropertyValue(sessions.ToArray(), "sessionName", loginInfo[0]) != null)
                {
                    await context.Response.WriteAsync("SESSION_DUPLICATE"); // Dealer already present, cant connect
                }
                else
                {
                    // Start new game
                    Game newSession = new Game(loginInfo[0]);

                    Player player = new Player(loginInfo[0], Player.ROLE.DEALER);
                    newSession.JoinSession(player);

                    await context.Response.WriteAsync("SESSION_OPEN");
                }
            }
            else if (loginInfo[1].Equals("PLAYER"))
            {
                Game sessionQuery = Utils.GetClassByPropertyValue(sessions.ToArray(), "sessionName", loginInfo[0]);
                if (sessionQuery != null) // Session exists
                {
                    // Join game
                    Game activeSession = sessionQuery;
                    Player player = new Player(loginInfo[0], Player.ROLE.PLAYER);
                    activeSession.JoinSession(player);

                    await context.Response.WriteAsync("SESSION_JOINED");
                }
                else
                {
                    // No session found, create game
                    // Use username since it was originally sent as a player
                    Game newSession = new Game(loginInfo[0] + "'s game");
                    Player player = new Player(loginInfo[0], Player.ROLE.DEALER); // Auto assign dealer role.
                    newSession.JoinSession(player);

                    await context.Response.WriteAsync("SESSION_OPEN");
                }
            }
        });
    }
}
