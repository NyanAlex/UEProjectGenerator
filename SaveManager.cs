using Newtonsoft.Json;
using System;
using System.IO;

namespace UEProjectGenerator
{
    public class SaveManager
    {
        public class SaveStruct
        {
            public string UnrealProjectFilePath;
            public string ExportedPluginsFolderPath;
            public string UnrealHeadersFolderPath;
            public string UnrealEngineFolderPath;
            public string DestinationFolderPath;
        }

        public static void PrintPresetInfo(SaveStruct saveStruct) 
        {
            Console.WriteLine("Selected Uproject file: " + saveStruct.UnrealProjectFilePath);
            Console.WriteLine("Selected Plugins folder: " + saveStruct.ExportedPluginsFolderPath);
            Console.WriteLine("Selected UHT Dump folder: " + saveStruct.UnrealHeadersFolderPath);
            Console.WriteLine("Selected Unreal Engine folder: " + saveStruct.UnrealEngineFolderPath);
            Console.WriteLine("Selected Destination folder: " + saveStruct.DestinationFolderPath);
            Console.WriteLine("");
        }

        public static void SavePreset(SaveStruct saveStruct) 
        {
            Console.WriteLine("Saving UnrealProjectGen preset...");
            File.WriteAllText(Path.GetFileNameWithoutExtension(saveStruct.UnrealProjectFilePath) + ".upg", JsonConvert.SerializeObject(saveStruct, Formatting.Indented));
            Console.WriteLine("Saved!");
            Console.WriteLine("");
        }
    }
}
