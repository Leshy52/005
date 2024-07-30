using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("SteamCheck", "YourName", "1.0.0")]
    class SteamCheck : RustPlugin
    {
        private string discordWebhookUrl = "YOUR_DISCORD_WEBHOOK_URL";
        private string steamApiKey = "YOUR_STEAM_API_KEY";

        void Init()
        {
            permission.RegisterPermission("steamcheck.admin", this);
            AddCovalenceCommand("checkplayers", "CheckPlayersCommand", "steamcheck.admin");
        }

        private void CheckPlayersCommand(IPlayer player, string command, string[] args)
        {
            foreach (BasePlayer onlinePlayer in BasePlayer.activePlayerList)
            {
                ulong steamId = onlinePlayer.userID;
                string playerName = onlinePlayer.displayName;
                string ip = onlinePlayer.net.connection.ipaddress;
                CheckSteamAccount(steamId, playerName, ip);
            }
        }

        void CheckSteamAccount(ulong steamId, string playerName, string ip)
        {
            string url = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + steamApiKey + "&steamids=" + steamId;
            webrequest.Enqueue(url, null, (code, response) =>
            {
                if (code != 200 || response == null)
                {
                    Puts("Failed to check Steam account for player with SteamID: " + steamId);
                    return;
                }

                var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                var players = ((Dictionary<string, object>)jsonData["response"])["players"] as List<object>;

                if (players.Count > 0)
                {
                    var playerData = players[0] as Dictionary<string, object>;
                    string accountType = playerData["personastate"].ToString();

                    if (accountType == "1")
                    {
                        Puts("Player with SteamID " + steamId + " is using an official Steam account. Name: " + playerName + ", IP: " + ip);
                        SendDiscordMessage("Player with SteamID " + steamId + " is using an official Steam account. Name: " + playerName + ", IP: " + ip);
                    }
                    else
                    {
                        Puts("Player with SteamID " + steamId + " is using a pirate account. Name: " + playerName + ", IP: " + ip);
                        SendDiscordMessage("Player with SteamID " + steamId + " is using a pirate account. Name: " + playerName + ", IP: " + ip);
                    }
                }
            }, this);
        }

        void SendDiscordMessage(string message)
        {
            Dictionary<string, string> jsonData = new Dictionary<string, string>
            {
                { "content", message }
            };

            webrequest.Enqueue(discordWebhookUrl, JsonConvert.SerializeObject(jsonData), (code, response) =>
            {
                if (code != 200)
                {
                    Puts("Failed to send Discord message: " + response);
                }
            }, this);
        }
    }
}