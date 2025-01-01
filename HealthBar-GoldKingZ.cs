using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Attributes;
using HealthBar_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Entities;

namespace HealthBar_GoldKingZ;


public class HealthBarGoldKingZ : BasePlugin
{
    public override string ModuleName => "Show HealthBar To Attacker Or All Players";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	public static HealthBarGoldKingZ Instance { get; set; } = new();

    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;

        RegisterEventHandler<EventPlayerHurt>(OnEventPlayerHurt);
    }

    public HookResult OnEventPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event == null || Configs.GetConfigData().DisableOnWarmUp && Helper.IsWarmup()) return HookResult.Continue;

        var victim = @event.Userid;
        var dmgHealth = @event.DmgHealth;
        var health = @event.Health;

        if (victim == null || !victim.IsValid) return HookResult.Continue;
        var victimHealth = victim.PlayerPawn.Value!.MaxHealth;

        var attacker = @event.Attacker;
        if (attacker == null || !attacker.IsValid) return HookResult.Continue;
        
        float oldHealth = health + dmgHealth;
        if (oldHealth == health) return HookResult.Continue;

        float oldHealthRatio = oldHealth / victimHealth;
        float newHealthRatio = (float)health / victimHealth;
        
        var message = UserMessage.FromPartialName("UpdateScreenHealthBar");
        message.SetInt("entidx", (int)victim.PlayerPawn.Index);
        message.SetFloat("healthratio_old", oldHealthRatio);
        message.SetFloat("healthratio_new", newHealthRatio);
        message.SetInt("style", Configs.GetConfigData().DisplayHealthBarStyle);
        if(Configs.GetConfigData().ShowHealthBarToAll)
        {
            message.Recipients.AddAllPlayers();
            message.Send();
        }else
        {
            message.Send(attacker);
        }
        
        return HookResult.Continue;
    }
}