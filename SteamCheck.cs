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
            AddCovalenceCommand("steamcheck.checkplayers", "CheckPlayersConsoleCommand", "steamcheck.admin");
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

        private void CheckPlayersConsoleCommand(IPlayer player, string command, string[] args)
        {
            if (player.IsServer)
            {
                foreach (BasePlayer onlinePlayer in BasePlayer.activePlayerList)
                {
                    ulong steamId = onlinePlayer.userID;
                    string playerName = onlinePlayer.displayName;
                    string ip = onlinePlayer.net.connection.ipaddress;
                    CheckSteamAccount(steamId, playerName, ip);
                }
            }
            else
            {
                player.Reply("This command can only be used from the server console.");
            }
        }

        void CheckSteamAccount(ulong steamId, string playerName, string ip)
        {
            // Оставляем вашу реализацию проверки Steam аккаунта здесь
        }

        void SendDiscordMessage(string message)
        {
            // Оставляем вашу реализацию отправки сообщения в Discord здесь
        }
    }
}