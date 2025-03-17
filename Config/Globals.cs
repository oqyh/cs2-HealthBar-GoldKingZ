using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Diagnostics;

namespace HealthBar_HitMark_GoldKingZ;

public class Globals
{
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public CParticleSystem HeadShot { get; set; }   
        public string Sound_HeadShot { get; set; }       
        public CParticleSystem BodyShot { get; set; }
        public string Sound_BodyShot { get; set; }       
    
        public PlayerDataClass(CCSPlayerController player, CParticleSystem headShot, string sound_HeadShot, CParticleSystem bodyShot, string sound_BodyShot)
        {
            Player = player;
            HeadShot = headShot;
            Sound_HeadShot = sound_HeadShot;
            BodyShot = bodyShot;
            Sound_BodyShot = sound_BodyShot;
        }
    }
    public Dictionary<CCSPlayerController, PlayerDataClass> Player_Data = new Dictionary<CCSPlayerController, PlayerDataClass>();

    public List<CParticleSystem> Particles_HS = new List<CParticleSystem>();
    public List<CParticleSystem> Particles_BS = new List<CParticleSystem>();

}