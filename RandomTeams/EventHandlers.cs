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

        private static List<ReferenceHub> scpPlayers = new List<ReferenceHub>();
        private static List<ReferenceHub> humanPlayers = new List<ReferenceHub>();

        private static bool isRoundStarted = false;
        private static bool endRound = false;

        internal static bool EnabledNextRound = false;
        public static bool Enabled = false;

        public void OnRaCommand(ref RACommandEvent ev)
        {
            try
            {
                if (ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT"))
                    return;

                string[] args = ev.Command.Split(' ');
                ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? PlayerManager.localPlayer.GetPlayer() : Player.GetPlayer(ev.Sender.SenderId);

                switch (args[0].ToLower())
                {
                    case "rteamenable":
                        {
                            if (!sender.CheckPermission("rt.enable"))
                            {
                                ev.Sender.RAMessage("Permission denied.");
                                return;
                            }

                            if (args[1].ToLower() == "true")
                            {
                                EnabledNextRound = true;
                                ev.Sender.RAMessage("Random Teams Enabled for next round!", false);
                                return;
                            } else if (args[1].ToLower() == "false")
                            {
                                EnabledNextRound = false;
                                ev.Sender.RAMessage("Random Teams Disabled for next round!", false);
                                return;
                            } else
                            {
                                ev.Sender.RAMessage("Argument not valid. True or False only!", false);
                            }
                            break;
                        }
                }
            }
            catch(Exception e)
            {
                Log.Error($"Handling command error: {e}");
            }
        }



        public void OnRoundStart() 
        {
            if (!Enabled) return;
            isRoundStarted = true;
            endRound = false;
        }

        public void OnPlayerLeave(PlayerLeaveEvent ev)
        {
            if (!Enabled) return;
            if (redTeamPlayers.Contains(ev.Player))
                redTeamPlayers.Remove(ev.Player);
            if (blueTeamPlayers.Contains(ev.Player))
                blueTeamPlayers.Remove(ev.Player);
            if (orangeTeamPlayers.Contains(ev.Player))
                orangeTeamPlayers.Remove(ev.Player);
            if (greenTeamPlayers.Contains(ev.Player))
                greenTeamPlayers.Remove(ev.Player);
            if (scpPlayers.Contains(ev.Player))
                scpPlayers.Remove(ev.Player);
            if (humanPlayers.Contains(ev.Player))
                humanPlayers.Remove(ev.Player);

            ev.Player.SetRank("", "default", false);
            ev.Player.SetRank(default);
        }

        public void OnRoundRestart()
        {
            redTeamPlayers.Clear();
            blueTeamPlayers.Clear();
            orangeTeamPlayers.Clear();
            redTeamPlayers.Clear();
            scpPlayers.Clear();
            humanPlayers.Clear();

            isRoundStarted = false;
            endRound = false;

            if (EnabledNextRound)
            {
                Enabled = true;
            }
            else Enabled = false;
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
            if (!Enabled) return;

            if (Plugin.TeamDamage) 
            {
                EnableFF(ev.Attacker);
                EnableFF(ev.Player);
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
            if (!Enabled) return;
            Timing.CallDelayed(0.1f, () => PlayerTeamAdd(ev.Player));
        }

        internal static void PlayerTeamAdd(ReferenceHub player)
        {
            if (player.GetTeam() != Team.RIP)
            {
                int teamChance = rand.Next(0, 3);
                if (teamChance == 0)
                {
                    greenTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Green Team!", false);
                    player.SetRank("Green Team", "blue_green", true);

                }
                else if (teamChance == 1)
                {
                    orangeTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Orange Team!", false);
                    player.SetRank("Orange Team", "orange", true);
                }
                else if (teamChance == 2)
                {
                    blueTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Blue Team!", false);
                    player.SetRank("Blue Team", "aqua", true);
                }
                else { 
                    redTeamPlayers.Add(player);
                    player.Broadcast(10, "You Are On Red Team!", false);
                    player.SetRank("Red Team", "red", true);
                }

                if (player.GetTeam() == Team.SCP)
                {
                    scpPlayers.Add(player);
                }
                else if (player.GetTeam() != Team.RIP)
                {
                    humanPlayers.Add(player);
                }
            }
        }

        public void OnPlayerDeath(ref PlayerDeathEvent ev)
        {
            if (!Enabled) return;
            if (redTeamPlayers.Contains(ev.Player))
                redTeamPlayers.Remove(ev.Player);
            if (blueTeamPlayers.Contains(ev.Player))
                blueTeamPlayers.Remove(ev.Player);
            if (orangeTeamPlayers.Contains(ev.Player))
                orangeTeamPlayers.Remove(ev.Player);
            if (greenTeamPlayers.Contains(ev.Player))
                greenTeamPlayers.Remove(ev.Player);
            if (scpPlayers.Contains(ev.Player))
                scpPlayers.Remove(ev.Player);
            if (humanPlayers.Contains(ev.Player))
                humanPlayers.Remove(ev.Player);

            ev.Player.SetRank("", "default", false);
            ev.Player.SetRank(default);

            DisableFF(ev.Killer);
            DisableFF(ev.Player);

            if (redTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !greenTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Green Team Has Won!", 20, false);
                endRound = true;
            }
            else if (redTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && greenTeamPlayers.IsEmpty() && !orangeTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Orange Team Has Won!", 20, false);
                endRound = true;
            }
            else if (redTeamPlayers.IsEmpty() && greenTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !blueTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Blue Team Has Won!", 20, false);
                endRound = true;
            }
            else if (greenTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && !redTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Red Team Has Won!", 20, false);
                endRound = true;
            }
            else if (greenTeamPlayers.IsEmpty() && redTeamPlayers.IsEmpty() && orangeTeamPlayers.IsEmpty() && blueTeamPlayers.IsEmpty())
            {
                Map.Broadcast("Tie!", 20, false);
                endRound = true;
            }
            else if (!scpPlayers.IsEmpty() && humanPlayers.IsEmpty())
            {
                Map.Broadcast("Stalemate!", 20, false);
                endRound = true;
            }
            else endRound = false;
        }

        public void OnRoundEnd()
        {
            foreach (ReferenceHub player in Player.GetHubs())
            {
                player.SetRank("", "default", false);
                player.SetRank(default);
            }

            redTeamPlayers.Clear();
            blueTeamPlayers.Clear();
            orangeTeamPlayers.Clear();
            greenTeamPlayers.Clear();
            scpPlayers.Clear();
            humanPlayers.Clear();
            isRoundStarted = false;
            endRound = false;
        }

        public void CheckRoundEnd(ref CheckRoundEndEvent ev)
        {
            if (!Enabled) return;
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
