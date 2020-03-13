using System;
using System.Collections.Generic;
using EXILED;
using UnityEngine;
using EXILED.Extensions;
using MEC;

namespace RandomTeams
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        private static System.Random rand = new System.Random();

        private static List<ReferenceHub> redTeamPlayers = new List<ReferenceHub>();
        private static List<ReferenceHub> orangeTeamPlayers = new List<ReferenceHub>();
        private static List<ReferenceHub> blueTeamPlayers = new List<ReferenceHub>();
        private static List<ReferenceHub> greenTeamPlayers = new List<ReferenceHub>();


        private static bool isRoundStarted = false;
        private static bool endRound = false;


        public void OnRoundStart() 
        {
            isRoundStarted = true;
            endRound = false;
        }

        public void OnPlayerLeave(PlayerLeaveEvent ev)
        {
            if (redTeamPlayers.Contains(ev.Player))
                redTeamPlayers.Remove(ev.Player);
            if (blueTeamPlayers.Contains(ev.Player))
                blueTeamPlayers.Remove(ev.Player);
            if (orangeTeamPlayers.Contains(ev.Player))
                orangeTeamPlayers.Remove(ev.Player);
            if (greenTeamPlayers.Contains(ev.Player))
                greenTeamPlayers.Remove(ev.Player);
        }

        public void OnRoundRestart()
        {
            redTeamPlayers.Clear();
            blueTeamPlayers.Clear();
            orangeTeamPlayers.Clear();
            redTeamPlayers.Clear();

            isRoundStarted = false;
            endRound = false;
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
            if (Plugin.TeamDamage) 
            {
                EnableFF(ev.Attacker);
                return;
            }

            if (redTeamPlayers.Contains(ev.Attacker) && redTeamPlayers.Contains(ev.Player)
                || blueTeamPlayers.Contains(ev.Attacker) && blueTeamPlayers.Contains(ev.Player)
                || greenTeamPlayers.Contains(ev.Attacker) && greenTeamPlayers.Contains(ev.Player)
                || orangeTeamPlayers.Contains(ev.Attacker) && orangeTeamPlayers.Contains(ev.Player))
            {
                DisableFF(ev.Attacker);
                DisableFF(ev.Player);
            }
            else EnableFF(ev.Attacker);
        }

        internal void EnableFF(ReferenceHub player)
        {
            player.weaponManager.NetworkfriendlyFire = true;
        }
        internal void DisableFF(ReferenceHub player)
        {
            player.weaponManager.NetworkfriendlyFire = false;
        }

        public void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            Timing.CallDelayed(0.1f, () => PlayerTeamAdd(ev.Player));
        }

        internal static void PlayerTeamAdd(ReferenceHub player)
        {
            if (player.GetTeam() != Team.SCP || player.GetTeam() != Team.RIP)
            {
                int teamChance = rand.Next(0, 2);
                if (teamChance == 0)
                {
                    greenTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Green Team!", false);

                }
                else if (teamChance == 1)
                {
                    orangeTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Orange Team!", false);
                }
                else if (teamChance == 2)
                {
                    blueTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Blue Team!", false);
                }
                else { 
                    redTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Red Team!", false);
                }
            }
            else if (player.GetTeam() == Team.SCP)
            {
                redTeamPlayers.Add(player);
                player.Broadcast(10, "You Are On Red Team!", false);
            }
        }

        public void OnPlayerDeath(ref PlayerDeathEvent ev)
        {
            if (redTeamPlayers.Contains(ev.Player))
                redTeamPlayers.Remove(ev.Player);
            if (blueTeamPlayers.Contains(ev.Player))
                blueTeamPlayers.Remove(ev.Player);
            if (orangeTeamPlayers.Contains(ev.Player))
                orangeTeamPlayers.Remove(ev.Player);
            if (greenTeamPlayers.Contains(ev.Player))
                greenTeamPlayers.Remove(ev.Player);

            DisableFF(ev.Killer);
            DisableFF(ev.Player);

            if (redTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !greenTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Green Team Has Won!", 20, false);
                endRound = true;
            }
            if (redTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && greenTeamPlayers.IsEmpty() && !orangeTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Orange Team Has Won!", 20, false);
                endRound = true;
            }
            if (redTeamPlayers.IsEmpty() && greenTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !blueTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Blue Team Has Won!", 20, false);
                endRound = true;
            }
            if (greenTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !redTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Red Team Has Won!", 20, false);
                endRound = true;
            }
            if (greenTeamPlayers.IsEmpty() && redTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Tie!", 20, false);
                endRound = true;
            }
            else endRound = false;
        }

        public void OnRoundEnd()
        {
            foreach (ReferenceHub player in Player.GetHubs())
            {
                if (redTeamPlayers.Contains(player))
                    redTeamPlayers.Remove(player);
                if (blueTeamPlayers.Contains(player))
                    blueTeamPlayers.Remove(player);
                if (orangeTeamPlayers.Contains(player))
                    orangeTeamPlayers.Remove(player);
                if (greenTeamPlayers.Contains(player))
                    greenTeamPlayers.Remove(player);
            }
            isRoundStarted = false;
            endRound = false;
        }

        public void CheckRoundEnd(ref CheckRoundEndEvent ev)
        {
            if (endRound)
            {
                ev.LeadingTeam = RoundSummary.LeadingTeam.Draw;
                ev.Allow = true;
                ev.ForceEnd = true;
                endRound = false;
            } else
            {
                ev.Allow = false;
            }
        }
    }
}
