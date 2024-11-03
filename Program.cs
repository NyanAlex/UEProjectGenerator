using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace UEProjectGenerator
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("UE Project Generator v0.1a by NyanAlex1");
            Console.WriteLine("");

            if (args.Length > 0 && args[0].EndsWith(".upg")) 
            {
                SaveManager.SaveStruct saveStruct = JsonConvert.DeserializeObject<SaveManager.SaveStruct>(File.ReadAllText(args[0]));

                SaveManager.PrintPresetInfo(saveStruct);

                Utils.GenerateProject(saveStruct);
            }
            else
            {
                SaveManager.SaveStruct saveStruct = new SaveManager.SaveStruct();

                OpenFileDialog uprojectFile = new OpenFileDialog();
                uprojectFile.Filter = "Game Unreal Project|*.uproject";
                uprojectFile.Title = "Unreal Project Selector";
                uprojectFile.CheckFileExists = true;
                uprojectFile.FileName = "";
                uprojectFile.Multiselect = false;
                if(uprojectFile.ShowDialog() == DialogResult.OK) saveStruct.UnrealProjectFilePath = uprojectFile.FileName;

                FolderBrowserDialog gamePlugins = new FolderBrowserDialog();
                gamePlugins.Description = "Select exported Plugins folder from game";
                gamePlugins.ShowNewFolderButton = false;
                if (gamePlugins.ShowDialog() == DialogResult.OK) saveStruct.ExportedPluginsFolderPath = gamePlugins.SelectedPath;

                FolderBrowserDialog UHTDumpFolder = new FolderBrowserDialog();
                UHTDumpFolder.Description = "Select UHT Dump generated with UE4SS";
                UHTDumpFolder.ShowNewFolderButton = false;
                if (UHTDumpFolder.ShowDialog() == DialogResult.OK) saveStruct.UnrealHeadersFolderPath = UHTDumpFolder.SelectedPath;

                FolderBrowserDialog UEFolder = new FolderBrowserDialog();
                UEFolder.Description = "Select Unreal Engine folder (ex: D:\\UE_X.XX\\Engine)";
                UEFolder.ShowNewFolderButton = false;
                if (UEFolder.ShowDialog() == DialogResult.OK) saveStruct.UnrealEngineFolderPath = UEFolder.SelectedPath;

                FolderBrowserDialog DestFolder = new FolderBrowserDialog();
                DestFolder.Description = "Select folder where you want to see generated project (program will make a folder with project name in it)";
                DestFolder.ShowNewFolderButton = true;
                if (DestFolder.ShowDialog() == DialogResult.OK) saveStruct.DestinationFolderPath = DestFolder.SelectedPath;

                SaveManager.PrintPresetInfo(saveStruct);

                SaveManager.SavePreset(saveStruct);

                Utils.GenerateProject(saveStruct);
            }
        }
    }
}
