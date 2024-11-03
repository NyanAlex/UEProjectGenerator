using System.Collections.Generic;

namespace UEProjectGenerator
{
    public class LocalizationTarget
    {
        public string Name { get; set; }
        public string LoadingPolicy { get; set; }
    }

    public class Module
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string LoadingPhase { get; set; }
        public List<string> WhitelistPlatforms { get; set; }
    }

    public class Plugin
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

    public class UnrealPlugin
    {
        public int FileVersion { get; set; }
        public string FriendlyName { get; set; }
        public double Version { get; set; }
        public string VersionName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByURL { get; set; }
        public bool EnabledByDefault { get; set; }
        public List<Module> Modules { get; set; }
        public List<LocalizationTarget> LocalizationTargets { get; set; }
        public List<Plugin> Plugins { get; set; }
    }
}
