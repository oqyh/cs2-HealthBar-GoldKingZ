using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace HealthBar_HitMark_GoldKingZ.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }
        public int Default { get; }
        public string Message { get; }

        public RangeAttribute(int min, int max, int defaultValue, string message)
        {
            Min = min;
            Max = max;
            Default = defaultValue;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BreakLineAttribute : Attribute
    {
        public string BreakLine { get; }

        public BreakLineAttribute(string breakLine)
        {
            BreakLine = breakLine;
        }
    }
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static IStringLocalizer? StringLocalizer { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string jsonFilePath2 = "ServerPrecacheResources.txt";
        private static string? _jsonFilePath2;
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }
            _jsonFilePath2 = Path.Combine(configFileDirectory, jsonFilePath2);
            Helper.CreateResource(_jsonFilePath2);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
                _configData!.Validate();
            }
            else
            {
                _configData = new ConfigData();
                _configData.Validate();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
{
    if (_configFilePath is null)
        throw new Exception("Config not yet loaded.");

    string json = JsonSerializer.Serialize(configData, SerializationOptions);
    json = Regex.Unescape(json);

    var lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    var newLines = new List<string>();

    foreach (var line in lines)
    {
        var match = Regex.Match(line, @"^\s*""(\w+)""\s*:.*");
        bool isPropertyLine = false;
        PropertyInfo? propInfo = null;

        if (match.Success)
        {
            string propName = match.Groups[1].Value;
            propInfo = typeof(ConfigData).GetProperty(propName);

            var breakLineAttr = propInfo?.GetCustomAttribute<BreakLineAttribute>();
            if (breakLineAttr != null)
            {
                string breakLine = breakLineAttr.BreakLine;

                if (breakLine.Contains("{space}"))
                {
                    breakLine = breakLine.Replace("{space}", "").Trim();

                    if (breakLineAttr.BreakLine.StartsWith("{space}"))
                    {
                        newLines.Add("");
                    }

                    newLines.Add("// " + breakLine);
                    newLines.Add("");
                }
                else
                {
                    newLines.Add("// " + breakLine);
                }
            }

            var commentAttr = propInfo?.GetCustomAttribute<CommentAttribute>();
            if (commentAttr != null)
            {
                var commentLines = commentAttr.Comment.Split('\n');
                foreach (var commentLine in commentLines)
                {
                    newLines.Add("// " + commentLine.Trim());
                }
            }

            isPropertyLine = true;
        }

        newLines.Add(line);

        // Add empty line after the property line
        if (isPropertyLine && propInfo?.GetCustomAttribute<CommentAttribute>() != null)
        {
            newLines.Add("");
        }
    }

    // Second pass: Add empty line after closing ] of arrays
    var adjustedLines = new List<string>();
    foreach (var line in newLines)
    {
        adjustedLines.Add(line);
        if (Regex.IsMatch(line, @"^\s*\],?\s*$"))
        {
            adjustedLines.Add("");
        }
    }

    File.WriteAllText(_configFilePath, string.Join(Environment.NewLine, adjustedLines), Encoding.UTF8);
}

        public class ConfigData
        {
            private string? _Version;
            private string? _Link;
            private string? _Our_WorkShop_ID;
            [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{space}")]
            public string Version
            {
                get => _Version!;
                set
                {
                    _Version = value;
                    if (_Version != HealthBarHitMarkGoldKingZ.Instance.ModuleVersion)
                    {
                        Version = HealthBarHitMarkGoldKingZ.Instance.ModuleVersion;
                    }
                }
            }

            public string Link
            {
                get => _Link!;
                set
                {
                    _Link = value;
                    if (_Link != "https://github.com/oqyh/cs2-HealthBar-HitMark-GoldKingZ")
                    {
                        Link = "https://github.com/oqyh/cs2-HealthBar-HitMark-GoldKingZ";
                    }
                }
            }
            public string Our_WorkShop_ID
            {
                get => _Our_WorkShop_ID!;
                set
                {
                    _Our_WorkShop_ID = value;
                    if (_Our_WorkShop_ID != "3446191649")
                    {
                        Our_WorkShop_ID = "3446191649";
                    }
                }
            }

            [BreakLine("{space}----------------------------[ ↓ CS2 HealthBar Config ↓ ]----------------------------{space}")]

            [Comment("Enable CS2 HealthBar?\ntrue = Yes\nfalse = No")]
            public bool CS2_HealthBar { get; set; }

            [Comment("Disable CS2 HealthBar On WarmUp?\ntrue = Yes\nfalse = No")]
            public bool CS2_DisableOnWarmUp { get; set; }

            [Comment("Required [CS2_HealthBar = true]\nHealthBar Style:\n0 = Green With Red\n1 = Orange With Grey\n2 = Green With Light Green\nTo Preview Check This link (https://github.com/oqyh/cs2-HealthBar-HitMark-GoldKingZ/blob/main/Resources/style.gif)")]
            [Range(0, 2, 0, "[HealthBar] CS_DisplayHealthBarStyle: is invalid, setting to default value (0) Please Choose From 0 To 2.\n[HealthBar] HealthBar Style:\n[HealthBar] 0 = Green With Red\n[HealthBar] 1 = Orange With Grey\n[HealthBar] 2 = Green With Light Green\n[HealthBar] To Preview Check This link (https://github.com/oqyh/cs2-HealthBar-HitMark-GoldKingZ/blob/main/Resources/style.gif)")]
            public int CS_DisplayHealthBarStyle { get; set; }

            [Comment("Required [CS2_HealthBar = true]\nShow HealthBar To:\n1 = All\n2 = Attacker Only\n3 = Victim Team\n4 = Attacker Team")]
            [Range(1, 4, 2, "[HealthBar] CS2_ShowHealthBarTo: is invalid, setting to default value (1) Please Choose From 1 To 4.\n[HealthBar] 1 = All\n[HealthBar] 2 = Attacker Only\n[HealthBar] 3 = Victim Team\n[HealthBar] 4 = Attacker Team")]
            public int CS2_ShowHealthBarTo { get; set; }

            [BreakLine("{space}----------------------------[ ↓ HitMark Config ↓ ]----------------------------{space}")]
            [Comment("Enable HitMark?\ntrue = Yes\nfalse = No")]
            public bool HM_HealthBar { get; set; }

            [Comment("Disable HitMark On WarmUp?\ntrue = Yes\nfalse = No")]
            public bool HM_DisableOnWarmUp { get; set; }

            [Comment("Mute Default HeadShot And BodyShot Only On If There Is Custom Sounds?\ntrue = Yes\nfalse = No")]
            public bool HM_MuteDefaultHeadShotBodyShot { get; set; }

            [Comment("SetUp HeadShot HitMark Per Players\n<PlayerFlag or PlayerGroup or PlayerSteam or ANY>|<Path>|<Optional SoundPath>\nANY = AnyOne\nFlags = @css/vips\nGroups = #css/vips\nSteamID = !STEAM_0:1:122910632 or !U:1:245821265 or !245821265 or !76561198206086993\nSoundPath = sounds/weapons/awp/awp_cliphit.vsnd")]
            public List<string> HM_HeadShot { get; set; }
            [Comment("SetUp BodyShot HitMark Per Players\n<PlayerFlag or PlayerGroup or PlayerSteam or ANY>|<Path>|<Optional SoundPath>\nANY = AnyOne\nFlags = @css/vips\nGroups = #css/vips\nSteamID = !STEAM_0:1:122910632 or !U:1:245821265 or !245821265 or !76561198206086993\nSoundPath = sounds/weapons/awp/awp_cliphit.vsnd")]
            public List<string> HM_BodyShot { get; set; }

            [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?\ntrue = Yes\nfalse = No")]
            [BreakLine("{space}----------------------------[ ↓ Utilities  ↓ ]----------------------------{space}")]
            public bool EnableDebug { get; set; }
            
            public ConfigData()
            {
                Version = HealthBarHitMarkGoldKingZ.Instance.ModuleVersion;
                Link = "https://github.com/oqyh/cs2-HealthBar-HitMark-GoldKingZ";
                Our_WorkShop_ID = "3446191649";
                CS2_HealthBar = true;
                CS2_DisableOnWarmUp = false;
                CS_DisplayHealthBarStyle = 0;
                CS2_ShowHealthBarTo = 2;
                HM_HealthBar = true;
                HM_DisableOnWarmUp = false;
                HM_MuteDefaultHeadShotBodyShot = false;
                HM_HeadShot = new List<string>
                {
                    "ANY | particles/goldkingz/hitmark/hitmark_head.vpcf | sounds/goldkingz/hitmark/headshot.vsnd",
                    "@css/vips,#css/vips | particles/goldkingz/hitmark/hitmark_head_2.vpcf | sounds/goldkingz/hitmark/headshot_2.vsnd",
                    "!STEAM_0:1:122910632,!76561198974936845 | particles/goldkingz/hitmark/owner_hitmark_head.vpcf"
                };
                HM_BodyShot = new List<string>
                {
                    "ANY | particles/goldkingz/hitmark/hitmark_body.vpcf | sounds/goldkingz/hitmark/bodyhit.vsnd",
                    "@css/vips,#css/vips | particles/goldkingz/hitmark/hitmark_body_2.vpcf | sounds/goldkingz/hitmark/bodyhit_2.vsnd",
                    "!STEAM_0:1:122910632,!76561198974936845 | particles/goldkingz/hitmark/owner_hitmark_body.vpcf"
                };
                EnableDebug = false;
            }

            public void Validate()
            {
                foreach (var prop in GetType().GetProperties())
                {
                    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null && prop.PropertyType == typeof(int))
                    {
                        int value = (int)prop.GetValue(this)!;
                        if (value < rangeAttr.Min || value > rangeAttr.Max)
                        {
                            prop.SetValue(this, rangeAttr.Default);
                            Helper.DebugMessage(rangeAttr.Message,false);
                        }
                    }
                }
            }
        }
    }
}