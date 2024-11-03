using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UEProjectGenerator
{
    public class UnrealHeader
    {
        public static async Task FixMissingIncludes(List<string> UHTHeaderFiles, string moduleFolderPath)
        {
            Console.WriteLine("Fixing missing includes...");

            // Collect all module headers only once
            var moduleHeaders = Directory.EnumerateFiles(moduleFolderPath, "*.h", SearchOption.AllDirectories).ToList();

            // Create a dictionary to hold module header lines
            var moduleHeaderDataMap = new Dictionary<string, HashSet<string>>();

            foreach (var moduleHeader in moduleHeaders)
            {
                var moduleHeaderData = File.ReadAllLines(moduleHeader);
                moduleHeaderDataMap[moduleHeader] = new HashSet<string>(moduleHeaderData);
            }

            foreach (string header in UHTHeaderFiles)
            {
                var UHTHeaderData = File.ReadAllLines(header);
                bool needsUpdate = false;

                for (int x = 0; x < UHTHeaderData.Length; x++)
                {
                    string currentLine = UHTHeaderData[x];
                    if (currentLine.StartsWith("//CROSS-MODULE INCLUDE V2:"))
                    {
                        string moduleName = currentLine.Substring(currentLine.IndexOf("=") + 1).Trim().Split(' ')[0];
                        string fallbackName = currentLine.Substring(currentLine.LastIndexOf("=") + 1).Trim();

                        foreach (var moduleHeader in moduleHeaders)
                        {
                            if (moduleHeader.Contains(moduleName))
                            {
                                if (moduleHeaderDataMap[moduleHeader].Any(line =>
                                    line.Contains(fallbackName) &&
                                    (line.StartsWith("struct ") || line.StartsWith("enum ") ||
                                     line.StartsWith("namespace ") || line.Contains(fallbackName + " : public")) &&
                                    !line.EndsWith(";")))
                                {
                                    string includeLine = "#include \"" +
                                                         (moduleHeader.Contains(@"Classes\")
                                                          ? moduleHeader.Substring(moduleHeader.IndexOf("Classes\\") + "Classes\\".Length)
                                                          : moduleHeader.Substring(moduleHeader.IndexOf("Public\\") + "Public\\".Length)) + "\"";
                                    UHTHeaderData[x] = includeLine;
                                    needsUpdate = true;
                                    break; // Exit inner loop once an include is found
                                }
                            }
                        }
                    }
                }

                if (needsUpdate)
                {
                    File.WriteAllLines(header, UHTHeaderData);
                }
            }

            Console.WriteLine("Done!");
            Console.WriteLine("");
        }

        public static async Task FixDoubleIncludes(List<string> UHTHeadersPath)
        {
            Console.WriteLine("Fixing multiple includes...");

            foreach (string header in UHTHeadersPath)
            {
                string[] headerContent = File.ReadAllLines(header);
                for (int i = 0; i < headerContent.Count(); i++)
                {
                    for (int x = 0; x < headerContent.Count(); x++)
                    {
                        if (headerContent[i].StartsWith("#include"))
                        {
                            if (headerContent[i] == headerContent[x] && i != x)
                            {
                                headerContent[i] = string.Empty;
                            }
                        }
                    }
                }
                File.WriteAllLines(header, headerContent);
            }

            Console.WriteLine("Done!");
            Console.WriteLine("");
        }
    }
}
