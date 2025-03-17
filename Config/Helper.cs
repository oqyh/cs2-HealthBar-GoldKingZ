using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.RegularExpressions;
using System.Text.Json;
using HealthBar_HitMark_GoldKingZ.Config;
using System.Text.Encodings.Web;
using CounterStrikeSharp.API.Core.Translations;

namespace HealthBar_HitMark_GoldKingZ;

public class Helper
{
    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToChat(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups))
        {
            return false;
        }
        var Groups = groups.Split(',');
        foreach (var group in Groups)
        {
            if (string.IsNullOrEmpty(group))
            {
                continue;
            }
            string groupId = group[0] == '!' ? group.Substring(1) : group;
            if (group[0] == '#' && AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
            else if (group[0] == '@' && AdminManager.PlayerHasPermissions(player, group))
            {
                return true;
            }
            else if (group[0] == '!' && player.AuthorizedSteamID != null && (groupId == player.AuthorizedSteamID.SteamId2.ToString() || groupId == player.AuthorizedSteamID.SteamId3.ToString().Trim('[', ']') ||
            groupId == player.AuthorizedSteamID.SteamId32.ToString() || groupId == player.AuthorizedSteamID.SteamId64.ToString()))
            {
                return true;
            }
            else if (AdminManager.PlayerInGroup(player, group))
            {
                return true;
            }
        }
        return false;
    }
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid && 
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
                        p.Connected == PlayerConnectedState.PlayerConnected && 
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p => 
            p != null && 
            p.IsValid && 
            p.Connected == PlayerConnectedState.PlayerConnected && 
            (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }
    
    public static void ClearVariables()
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;

        g_Main.Player_Data.Clear();
        g_Main.Particles_HS.Clear();
        g_Main.Particles_BS.Clear();
    }
    
    public static void CreateResource(string jsonFilePath)
    {
        string headerLine = "////// vvvvvv Add Paths For Precache Resources Down vvvvvvvvvv //////";
        string headerLine2 = "particles/goldkingz/hitmark/hitmark_head.vpcf";
        string headerLine3 = "particles/goldkingz/hitmark/hitmark_body.vpcf";
        string headerLine4 = "particles/goldkingz/hitmark/hitmark_head_2.vpcf";
        string headerLine5 = "particles/goldkingz/hitmark/hitmark_body_2.vpcf";
        if (!File.Exists(jsonFilePath))
        {
            using (StreamWriter sw = File.CreateText(jsonFilePath))
            {
                sw.WriteLine(headerLine);
                sw.WriteLine(headerLine2);
                sw.WriteLine(headerLine3);
                sw.WriteLine(headerLine4);
                sw.WriteLine(headerLine5);
            }
        }
        else
        {
            string[] lines = File.ReadAllLines(jsonFilePath);
            if (lines.Length == 0 || lines[0] != headerLine)
            {
                using (StreamWriter sw = new StreamWriter(jsonFilePath))
                {
                    sw.WriteLine(headerLine);
                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }
    }
    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string Prefix = $"[HealthBar]: ";
        Console.WriteLine(prefix?Prefix:"" + message);
        
        Console.ResetColor();
    }
    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void SpawnHitMarks(CCSPlayerController player)
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;
        if (player == null || !player.IsValid) return;

        string hs_DefaultParticlePath = null!;
        string hs_DefaultSoundPath = null!;
        string hs_ParticlePath = null!;
        string hs_SoundPath = null!;
        foreach (var flag in Configs.GetConfigData().HM_HeadShot)
        {
            var parts = flag.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (parts.Length < 2) continue;

            var Flag = parts[0];
            var Path = parts[1];
            var SoundPath = parts.Length >= 3 ? parts[2] : null;

            if (Flag.Equals("ANY", StringComparison.OrdinalIgnoreCase))
            {
                if (hs_DefaultParticlePath == null)
                {
                    hs_DefaultParticlePath = Path;
                }

                if (SoundPath != null && hs_DefaultSoundPath == null)
                {
                    hs_DefaultSoundPath = SoundPath;
                }
                continue;
            }

            if (IsPlayerInGroupPermission(player, Flag))
            {
                hs_ParticlePath = Path;

                if (SoundPath != null)
                {
                    hs_SoundPath = SoundPath;
                }
                break;
            }
        }

        if (hs_ParticlePath == null)
        {
            hs_ParticlePath = hs_DefaultParticlePath!;
            hs_SoundPath = hs_DefaultSoundPath!;
        }


        string bs_DefaultParticlePath = null!;
        string bs_DefaultSoundPath = null!;
        string bs_ParticlePath = null!;
        string bs_SoundPath = null!;
        foreach (var flag in Configs.GetConfigData().HM_BodyShot)
        {
            var parts = flag.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (parts.Length < 2) continue;

            var Flag = parts[0];
            var Path = parts[1];
            var SoundPath = parts.Length >= 3 ? parts[2] : null;

            if (Flag.Equals("ANY", StringComparison.OrdinalIgnoreCase))
            {
                if (bs_DefaultParticlePath == null)
                {
                    bs_DefaultParticlePath = Path;
                }

                if (SoundPath != null && bs_DefaultSoundPath == null)
                {
                    bs_DefaultSoundPath = SoundPath;
                }
                continue;
            }

            if (IsPlayerInGroupPermission(player, Flag))
            {
                bs_ParticlePath = Path;

                if (SoundPath != null)
                {
                    bs_SoundPath = SoundPath;
                }
                break;
            }
        }

        if (bs_ParticlePath == null)
        {
            bs_ParticlePath = bs_DefaultParticlePath!;
            bs_SoundPath = bs_DefaultSoundPath!;
        }


        CParticleSystem headParticle = null!;
        if (hs_ParticlePath != null)
        {
            headParticle = CreateHitMark(hs_ParticlePath);
        }

        CParticleSystem bodyParticle = null!;
        if (bs_ParticlePath != null)
        {
            bodyParticle = CreateHitMark(bs_ParticlePath);
        }

        if (headParticle != null || bodyParticle != null)
        {
            g_Main.Player_Data.Add(player, new Globals.PlayerDataClass(
                player,
                headParticle!,
                hs_SoundPath!,
                bodyParticle,
                bs_SoundPath
            ));

            if (headParticle != null)
            {
                g_Main.Particles_HS.Add(headParticle);
            }

            if (bodyParticle != null)
            {
                g_Main.Particles_BS.Add(bodyParticle);
            }
        }
    }
    public static void StartHitMark(CCSPlayerController player, bool HeadShot)
    {
        var g_Main = HealthBarHitMarkGoldKingZ.Instance.g_Main;
        if(player == null || !player.IsValid || !g_Main.Player_Data.ContainsKey(player))return;

        var playerData = g_Main.Player_Data[player];

        if(HeadShot)
        {
            if(playerData.HeadShot != null && playerData.HeadShot.IsValid)
            {
                playerData.HeadShot.AcceptInput("Stop");
                playerData.HeadShot.AddEntityIOEvent("Start", null!, null!, "", 0.1f);
                if(!string.IsNullOrEmpty(playerData.Sound_HeadShot))
                {
                    player.ExecuteClientCommand("play " + playerData.Sound_HeadShot);
                }
            }
        }else
        {
            if(playerData.BodyShot != null && playerData.BodyShot.IsValid)
            {
                playerData.BodyShot.AcceptInput("Stop");
                playerData.BodyShot.AddEntityIOEvent("Start", null!, null!, "", 0.1f);
                if(!string.IsNullOrEmpty(playerData.Sound_BodyShot))
                {
                    player.ExecuteClientCommand("play " + playerData.Sound_BodyShot);
                }
            }
        }
        
    }

    public static CParticleSystem CreateHitMark(string path)
    {
        var entity = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");
        if (entity == null) return null!;
        
        entity.EffectName = path;
        entity.DispatchSpawn();
        return entity;
    }
}