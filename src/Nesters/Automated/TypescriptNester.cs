using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class TypescriptNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".js", new [] {".ts"}},
            {".js.map", new [] {".ts"}}
        };

        public NestingResult Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string realFileName = Path.GetFileName(fileName);
            int index = realFileName.IndexOf(".");

            if (index >= 0)
            {
                extension = realFileName.Substring(index);
            }

            if (!_mapping.ContainsKey(extension))
                return NestingResult.Continue;

            foreach (string ext in _mapping[extension])
            {
                string parent = Path.ChangeExtension(fileName, ext);

                if (index >= 0)
                {
                    parent = fileName.ToLower().Replace(extension, ext);
                }

                ProjectItem item = VSPackage.DTE.Solution.FindProjectItem(parent);

                if (item != null)
                {
                    item.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }

        public bool IsEnabled()
        {
            return VSPackage.Options.EnableTypescriptRule;
        }
    }
}
