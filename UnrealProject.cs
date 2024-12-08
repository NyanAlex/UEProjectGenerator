using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UEProjectGenerator
{
	public class UEProjectModule
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string LoadingPhase { get; set; }
	}
	public class UEProjectPlugin
	{
		public string Name { get; set; }
		public bool Enabled { get; set; }
	}
	public class UEProjectFile
	{
		public int FileVersion { get; set; }
		public string EngineAssociation { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public List<UEProjectModule> Modules { get; set; }
		public List<UEProjectPlugin> Plugins { get; set; }
		public string[] TargetPlatforms { get; set; }
	}

	public class UnrealProject
	{
		public static async Task CreateProject(string projectFilePath, string exportedPluginsPath, string UHTDumpPath, string UnrealEnginePath, string destinationPath)
		{
			Console.WriteLine("Generating Project Template...");

			string newProjectPath = destinationPath + "\\" + Path.GetFileNameWithoutExtension(projectFilePath);
			Directory.CreateDirectory(newProjectPath);
			UEProjectFile project = JsonConvert.DeserializeObject<UEProjectFile>(File.ReadAllText(projectFilePath));
			string uePluginsPath = UnrealEnginePath + "\\Plugins\\";
			string ueModulesPath = UnrealEnginePath + "\\Source\\Runtime\\";
			string newProjectPluginsPath = newProjectPath + "\\Plugins\\";
			string newProjectSourcePath = newProjectPath + "\\Source\\";
			Directory.CreateDirectory(newProjectPluginsPath);
			Directory.CreateDirectory(newProjectSourcePath);

			List<string> gamePluginsPath = Directory.EnumerateFiles(exportedPluginsPath, "*.uplugin", SearchOption.AllDirectories).ToList();
			List<string> gameModulesPath = Directory.EnumerateDirectories(UHTDumpPath, "*", SearchOption.TopDirectoryOnly).ToList();
			List<string> ueModulesCSPath = Directory.EnumerateFiles(ueModulesPath, "*.Build.cs", SearchOption.AllDirectories).ToList();
			List<string> uePluginsPluginsPath = Directory.EnumerateFiles(uePluginsPath, "*.uplugin", SearchOption.AllDirectories).ToList();

            List<string> uePluginsName = new List<string>();
            List<string> uePluginsModuleName = new List<string>();

            foreach (string uePluginPath in uePluginsPluginsPath)
            {
                UnrealPlugin plugin = JsonConvert.DeserializeObject<UnrealPlugin>(File.ReadAllText(uePluginPath));

                if (plugin != null && plugin.Modules != null)
                {
                    foreach (var pluginModule in plugin.Modules)
                    {
                        uePluginsModuleName.Add(pluginModule.Name);
                    }
                }

                string uePluginName = Path.GetFileNameWithoutExtension(uePluginPath);
                uePluginsName.Add(uePluginName);
            }

            foreach (string gamePluginPath in gamePluginsPath)
			{
				UnrealPlugin plugin = JsonConvert.DeserializeObject<UnrealPlugin>(File.ReadAllText(gamePluginPath));
				string pluginName = Path.GetFileNameWithoutExtension(gamePluginPath);

				string currentPluginPath = newProjectPluginsPath + pluginName + "\\";
				string currentPluginSourcePath = newProjectPluginsPath + pluginName + "\\Source\\";

				Directory.CreateDirectory(currentPluginPath);
				File.Copy(gamePluginPath, currentPluginPath + Path.GetFileName(gamePluginPath), true);

				List<string> moduleNames = new List<string>();

				for (int x = 0; x < gameModulesPath.Count(); x++)
				{
					string moduleName = gameModulesPath[x].Substring(gameModulesPath[x].LastIndexOf("\\") + 1);
                    moduleNames.Add(moduleName);
                }

				for (int x = 0; x < plugin.Modules.Count(); x++) 
				{
                    if (!moduleNames.Contains(plugin.Modules[x].Name))
                    {
						plugin.Modules.RemoveAt(x);
                    }
                }

                for (int x = 0; x < moduleNames.Count(); x++)
                {
                    foreach (var pluginModule in plugin.Modules)
                    {
                        if (moduleNames[x] == pluginModule.Name)
                        {
                            Utils.CopyDirectory(gameModulesPath[x], currentPluginSourcePath + moduleNames[x]);
                            gameModulesPath.RemoveAt(x);
                        }
                    }
                }

                if (!Directory.Exists(currentPluginSourcePath))
				{
					plugin.Modules.Clear();
				}

				File.WriteAllText(currentPluginPath + Path.GetFileName(gamePluginPath), JsonConvert.SerializeObject(plugin, Formatting.Indented));
			}

			foreach (string gamePluginPath in gamePluginsPath)
			{
				string currentPluginPath = newProjectPluginsPath + Path.GetFileNameWithoutExtension(gamePluginPath) + "\\" + Path.GetFileName(gamePluginPath);

				UnrealPlugin unrealPlugin = JsonConvert.DeserializeObject<UnrealPlugin>(File.ReadAllText(currentPluginPath));

				if (unrealPlugin.Plugins != null)
				{
					for (int x = 0; x < unrealPlugin.Plugins.Count(); x++)
					{
						if (!uePluginsName.Contains(unrealPlugin.Plugins[x].Name))
						{
							unrealPlugin.Plugins.RemoveAt(x);
						}
					}

					File.WriteAllText(currentPluginPath, JsonConvert.SerializeObject(unrealPlugin, Formatting.Indented));
				}
			}

                List<string> ueModulesName = new List<string>();

			foreach (string ueModulesCSFile in ueModulesCSPath)
			{
				string ueModuleName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(ueModulesCSFile));
				ueModulesName.Add(ueModuleName);
			}

			List<string> gameModulesName = new List<string>();

			foreach (string gameModulePath in gameModulesPath)
			{
				string gameModuleName = gameModulePath.Substring(gameModulePath.LastIndexOf("\\") + 1);

				gameModulesName.Add(gameModuleName);

				if (!ueModulesName.Contains(gameModuleName) && !uePluginsModuleName.Contains(gameModuleName))
				{
					Utils.CopyDirectory(gameModulePath, newProjectSourcePath + gameModuleName);
				}
			}

			for (int y = 0; y < project.Modules.Count(); y++)
			{
				UEProjectModule projectModule = project.Modules[y];
				if (!gameModulesName.Contains(projectModule.Name))
				{
					project.Modules.RemoveAt(y); // Removing missing modules from project
				}
			}

			List<string> gamePluginsName = new List<string>();

			foreach (string gamePluginPath in gamePluginsPath)
			{
				string gamePluginName = Path.GetFileNameWithoutExtension(gamePluginPath);
				gamePluginsName.Add(gamePluginName);
			}

			for (int x = 0; x < project.Plugins.Count(); x++)
			{
				UEProjectPlugin projectPlugin = project.Plugins[x];
				if (!gamePluginsName.Contains(projectPlugin.Name) && !uePluginsName.Contains(projectPlugin.Name))
				{
					project.Plugins.RemoveAt(x); // Removing missing plugins from project
				}

				List<string> alreadyUsedUEPlugins = new List<string>();

                foreach (var ueplugin in uePluginsPluginsPath)
                {
                    UnrealPlugin unrealPlugin = JsonConvert.DeserializeObject<UnrealPlugin>(File.ReadAllText(ueplugin));
                    if (unrealPlugin.FriendlyName == projectPlugin.Name && !unrealPlugin.EnabledByDefault)
                    {
                        UEProjectPlugin gamePlugin = new UEProjectPlugin();
                        gamePlugin.Name = unrealPlugin.FriendlyName;
                        gamePlugin.Enabled = true;

                        if (!alreadyUsedUEPlugins.Contains(gamePlugin.Name))
                        {
							alreadyUsedUEPlugins.Add(gamePlugin.Name);
                            project.Plugins.Add(gamePlugin);
                        }
                    }
                }
            }

            List<string> modulesForTargetCS = Directory.EnumerateDirectories(newProjectSourcePath, "*", SearchOption.TopDirectoryOnly).ToList();

			string projectTargetCS = "using UnrealBuildTool;\n\npublic class " + Path.GetFileNameWithoutExtension(projectFilePath) + "Target : TargetRules {\n\tpublic " + Path.GetFileNameWithoutExtension(projectFilePath) + "Target(TargetInfo Target) : base(Target) {\n\t\tType = TargetType.Game;" /* + "\n\t\tDefaultBuildSettings = BuildSettingsVersion.V2;" */ + "\n\t\tExtraModuleNames.AddRange(new string[] {";
			string projectEditorTargetCS = "using UnrealBuildTool;\n\npublic class " + Path.GetFileNameWithoutExtension(projectFilePath) + "Editor" + "Target : TargetRules {\n\tpublic " + Path.GetFileNameWithoutExtension(projectFilePath) + "Editor" + "Target(TargetInfo Target) : base(Target) {\n\t\tType = TargetType.Editor;" /* + "\n\t\tDefaultBuildSettings = BuildSettingsVersion.V2;" */ + "\n\t\tExtraModuleNames.AddRange(new string[] {";

			foreach (string moduleForTargetCS in modulesForTargetCS) 
			{
				string moduleName = moduleForTargetCS.Substring(moduleForTargetCS.LastIndexOf("\\") + 1);
				projectEditorTargetCS += "\n\t\t\t\"" + moduleName + "\",";
				projectTargetCS += "\n\t\t\t\"" + moduleName + "\",";
			}

			projectEditorTargetCS += "\n\t\t});\n\t}\n}";
			projectTargetCS += "\n\t\t});\n\t}\n}";

			File.WriteAllText(newProjectSourcePath + Path.GetFileNameWithoutExtension(projectFilePath) + "Editor.Target.cs", projectEditorTargetCS);
			File.WriteAllText(newProjectSourcePath + Path.GetFileNameWithoutExtension(projectFilePath) + ".Target.cs", projectTargetCS);

			File.WriteAllText(newProjectPath + "\\" + Path.GetFileName(projectFilePath), JsonConvert.SerializeObject(project, Formatting.Indented));

			Console.WriteLine("Done!");
			Console.WriteLine("");
		}
	}
}