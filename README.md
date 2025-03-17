## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] HealthBar-HitMark-GoldKingZ (1.0.1)

Show HealthBar , Custom HitMarks , Custom Sounds

![healthbar](https://github.com/user-attachments/assets/25e88501-00c8-4fea-b7fd-7324d79048db)
![hitmark](https://github.com/user-attachments/assets/0a7a6e2b-79f8-4102-92ef-67473db1138d)

---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2.x-2d2d2d?logo=sourceengine)](https://www.sourcemm.net/downloads.php?branch=dev)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

---

## 📥 Installation

1. Download latest release
2. Extract to `csgo` directory
3. Configure `HealthBar-HitMark-GoldKingZ\config\config.json`
4. Restart server

---

## ⚙️ Configuration

> [!NOTE]
> Located In ..\HealthBar-HitMark-GoldKingZ\config\config.json                                           
>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|  
| `CS2_HealthBar` | Enable CS2 HealthBar | `true`/`false` | - |  
| `CS2_DisableOnWarmUp` | Disable CS2 HealthBar On WarmUp | `true`/`false` | - |  
| `CS_DisplayHealthBarStyle` | HealthBar Style | `0`-Green With Red<br>`1`-Orange With Grey<br>`2`-Green With Light Green | `CS2_HealthBar=true` |  
| `CS2_ShowHealthBarTo` | Show HealthBar To | `1`-All<br>`2`-Attacker Only<br>`3`-Victim Team<br>`4`-Attacker Team | `CS2_HealthBar=true` |  
| `HM_HealthBar` | Enable HitMark | `true`/`false` | - |  
| `HM_DisableOnWarmUp` | Disable HitMark On WarmUp | `true`/`false` | - |  
| `HM_MuteDefaultHeadShotBodyShot` | Mute Default HeadShot And BodyShot Only On If There Is Custom Sounds | `true`/`false` | `CS2_HealthBar=true` |  
| `HM_HeadShot` | SetUp HeadShot | `<PlayerFlag or PlayerGroup or PlayerSteam or ANY>`|`<Path>`|`<Optional SoundPath>` | `CS2_HealthBar=true` |  
| `HM_BodyShot` | SetUp BodyShot | `<PlayerFlag or PlayerGroup or PlayerSteam or ANY>`|`<Path>`|`<Optional SoundPath>` | `CS2_HealthBar=true` |  
| `EnableDebug` | Debug mode | `true`/`false` | - |  


---

---

## 📜 Changelog

<details>
<summary>📋 View Version History (Click to expand 🔽)</summary>

### [1.0.1]
- Added CS2_ShowHealthBarTo 3 = Victim Team 4 = Attacker Team
- Added HM_HealthBar
- Added HM_DisableOnWarmUp
- Added HM_MuteDefaultHeadShotBodyShot
- Added HM_HeadShot
- Added HM_BodyShot
- Added EnableDebug
- Added In config.json info on each what it do

### [1.0.0]
- Initial Release

</details>

---
