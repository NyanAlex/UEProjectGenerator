using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UEProjectGenerator
{
    public class Utils
	{
		static public void CopyDirectory(string directory, string dest)
		{
			List<string> allFiles = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories).ToList();

			string folder = dest + "\\";

			Directory.CreateDirectory(folder);


			foreach (string file in allFiles)
			{
				string newdirectory = file.Replace(directory, string.Empty).Replace(Path.GetFileName(file), string.Empty);

				Directory.CreateDirectory(folder + newdirectory);
				File.Copy(file, folder + newdirectory + Path.GetFileName(file), true);
			}
		}
		static public async void GenerateProject(SaveManager.SaveStruct saveStruct) 
		{
			await UnrealProject.CreateProject(saveStruct.UnrealProjectFilePath, saveStruct.ExportedPluginsFolderPath, saveStruct.UnrealHeadersFolderPath, saveStruct.UnrealEngineFolderPath, saveStruct.DestinationFolderPath);

			List<string> allHeaders = Directory.EnumerateFiles(saveStruct.DestinationFolderPath, "*.h", SearchOption.AllDirectories).ToList();

			await UnrealHeader.FixMissingIncludes(allHeaders, saveStruct.UnrealEngineFolderPath + "\\Source\\Runtime\\");
			Console.WriteLine("Checking plugins...");
            await UnrealHeader.FixMissingIncludes(allHeaders, saveStruct.UnrealEngineFolderPath + "\\Plugins\\");
            await UnrealHeader.FixDoubleIncludes(allHeaders);

			Console.WriteLine("Enjoy!");
			Console.ReadKey();
		}
	}
}
