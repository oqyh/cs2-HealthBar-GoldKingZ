using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Attributes;
using HealthBar_HitMark_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;

namespace HealthBar_HitMark_GoldKingZ;


public class HealthBarHitMarkGoldKingZ : BasePlugin
{
    public override string ModuleName => "Show HealthBar , Custom HitMarks , Custom Sounds";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	public static HealthBarHitMarkGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;
    
        RegisterEventHandler<EventPlayerHurt>(OnEventPlayerHurt);
        RegisterEventHandler<EventRoundStart>(OnEventRoundStart);
        RegisterListener<Listeners.OnServerPrecacheResources>(OnServerPrecacheResources);
        RegisterListener<Listeners.CheckTransmit>(CheckTransmit);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);


        if(Configs.GetConfigData().HM_MuteDefaultHeadShotBodyShot)
        {
            HookUserMessage(208, um =>
            {
                var soundevent = um.ReadUInt("soundevent_hash");
                uint HeadShotHit_ClientSide = 2831007164;
                uint Player_Got_Damage_ClientSide = 708038349;

                bool HH = g_Main.Player_Data.Any(playerdata => !string.IsNullOrEmpty(playerdata.Value.Sound_HeadShot) && soundevent == HeadShotHit_ClientSide) ? true : false;
                bool BH = g_Main.Player_Data.Any(playerdata => !string.IsNullOrEmpty(playerdata.Value.Sound_BodyShot) && soundevent == Player_Got_Damage_ClientSide) ? true : false;
                if (HH || BH)
                {
                    return HookResult.Stop;
                }
                return HookResult.Continue;

            }, HookMode.Pre);
        }
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        try
        {
            string filePath = $"{ModuleDirectory}/../../plugins/HealthBar-HitMark-GoldKingZ/config/ServerPrecacheResources.txt";

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("//"))continue;
                manifest.AddResource(line);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage(ex.Message);
        }
    }

    public void CheckTransmit(CCheckTransmitInfoList infoList)
    {
        foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
        {
            if (player == null || !player.IsValid)continue;
            if (!g_Main.Player_Data.TryGetValue(player, out var playerData))continue;

            foreach (var particle_hs in g_Main.Particles_HS)
            {
                if (particle_hs == null || !particle_hs.IsValid)continue;

                if (playerData.HeadShot != null && playerData.HeadShot.IsValid && particle_hs != playerData.HeadShot)
                {
                    info.TransmitEntities.Remove(particle_hs);
                }
            }

            foreach (var particle_bs in g_Main.Particles_BS)
            {
                if (particle_bs == null || !particle_bs.IsValid)continue;

                if (playerData.BodyShot != null && playerData.BodyShot.IsValid && particle_bs != playerData.BodyShot)
                {
                    info.TransmitEntities.Remove(particle_bs);
                }
            }
        }
    }

    public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        Helper.ClearVariables();

        foreach(var players in Helper.GetPlayersController())
        {
            if(players == null || !players.IsValid)continue;

            Helper.SpawnHitMarks(players);
        }
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var victim = @event.Userid;
        var dmgHealth = @event.DmgHealth;
        var health = @event.Health;
        var Hitgroup = @event.Hitgroup;

        if (victim == null || !victim.IsValid || victim.PlayerPawn == null || !victim.PlayerPawn.IsValid
        || victim.PlayerPawn.Value == null || !victim.PlayerPawn.Value.IsValid) return HookResult.Continue;
        var victimHealth = victim.PlayerPawn.Value.MaxHealth;

        var attacker = @event.Attacker;
        if (attacker == null || !attacker.IsValid) return HookResult.Continue;

        bool Check_teammates_are_enemies = ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == false && attacker.TeamNum != victim.TeamNum || ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() == true;
        if (!Check_teammates_are_enemies) return HookResult.Continue;

        float oldHealth = health + dmgHealth;
        if (oldHealth == health) return HookResult.Continue;

        float oldHealthRatio = oldHealth / victimHealth;
        float newHealthRatio = (float)health / victimHealth;
        
        if(Configs.GetConfigData().CS2_HealthBar)
        {
            if(!Configs.GetConfigData().CS2_DisableOnWarmUp || Configs.GetConfigData().CS2_DisableOnWarmUp && !Helper.IsWarmup())
            {
                var message = UserMessage.FromPartialName("UpdateScreenHealthBar");
                message.SetInt("entidx", (int)victim.PlayerPawn.Index);
                message.SetFloat("healthratio_old", oldHealthRatio);
                message.SetFloat("healthratio_new", newHealthRatio);
                message.SetInt("style", Configs.GetConfigData().CS_DisplayHealthBarStyle);
                if(Configs.GetConfigData().CS2_ShowHealthBarTo == 1)
                {
                    message.Recipients.AddAllPlayers();
                    message.Send();
                }else if(Configs.GetConfigData().CS2_ShowHealthBarTo == 2)
                {
                    message.Send(attacker);
                }else
                {
                    foreach(var players in Helper.GetPlayersController(false,false))
                    {
                        if(players == null || !players.IsValid)continue;
                        if(Configs.GetConfigData().CS2_ShowHealthBarTo == 3 && victim.TeamNum != players.TeamNum)continue;
                        if(Configs.GetConfigData().CS2_ShowHealthBarTo == 4 && attacker.TeamNum != players.TeamNum)continue;

                        message.Send(players);
                    }
                }
            }
        }

        if(Configs.GetConfigData().HM_HealthBar)
        {
            if(!Configs.GetConfigData().HM_DisableOnWarmUp || Configs.GetConfigData().HM_DisableOnWarmUp && !Helper.IsWarmup())
            {
                if(Hitgroup == 1)
                {
                    Helper.StartHitMark(attacker, true);
                    if (g_Main.Player_Data.TryGetValue(attacker, out var playerData))
                    {
                        if(!string.IsNullOrEmpty(playerData.Sound_HeadShot))
                        {
                            attacker.ExecuteClientCommand("play " + playerData.Sound_HeadShot);
                        }
                    }
                }else
                {
                    Helper.StartHitMark(attacker, false);
                    
                }
            }
        }
        
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.Player_Data.ContainsKey(player))g_Main.Player_Data.Remove(player);

        return HookResult.Continue;
    }

    private void OnMapEnd()
    {
        Helper.ClearVariables();
    }

    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }


    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null || !player.IsValid)return;

        
    } */
}