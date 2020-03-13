using System;
using System.Collections.Generic;
using EXILED;
using Harmony;

namespace RandomTeams
{
    public class Plugin : EXILED.Plugin
    {
        public EventHandlers EventHandler;
        public static bool PluginEnabled;
        public static bool TeamDamage;
        public override string getName { get; } = "RandomTeams";

        public override void OnEnable()
        {
            LoadConfigs();

            if (!PluginEnabled) return;

            EventHandler = new EventHandlers(this);
            Events.RemoteAdminCommandEvent += EventHandler.OnRaCommand;
            Events.RoundStartEvent += EventHandler.OnRoundStart;
            Events.PlayerLeaveEvent += EventHandler.OnPlayerLeave;
            Events.PlayerDeathEvent += EventHandler.OnPlayerDeath;
            Events.CheckRoundEndEvent += EventHandler.CheckRoundEnd;
            Events.RoundEndEvent += EventHandler.OnRoundEnd;
            Events.PlayerHurtEvent += EventHandler.OnPlayerHurt;
            Events.PlayerSpawnEvent += EventHandler.OnPlayerSpawn;
            Events.RoundRestartEvent += EventHandler.OnRoundRestart;
        }

        public override void OnDisable()
        {
            Events.RoundRestartEvent -= EventHandler.OnRoundRestart;
            Events.PlayerSpawnEvent -= EventHandler.OnPlayerSpawn;
            Events.PlayerHurtEvent -= EventHandler.OnPlayerHurt;
            Events.RoundEndEvent -= EventHandler.OnRoundEnd;
            Events.CheckRoundEndEvent -= EventHandler.CheckRoundEnd;
            Events.PlayerDeathEvent -= EventHandler.OnPlayerDeath;
            Events.PlayerLeaveEvent -= EventHandler.OnPlayerLeave;
            Events.RoundStartEvent -= EventHandler.OnRoundStart;
            Events.RemoteAdminCommandEvent -= EventHandler.OnRaCommand;
            EventHandler = null;
        }

        private void LoadConfigs()
        {
            PluginEnabled = Config.GetBool("ranteam_enabled", true);
            TeamDamage = Config.GetBool("ranteam_teamkill", false);
        }


        public override void OnReload(){}
    }
}
