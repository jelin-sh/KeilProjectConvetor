using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeilPorjectConvertor
{
    class VSCodeWorkspace_Folder
    {
        public string name { get; set; }
        public string path { get; set; }        
    }

    class VSCodeWorkspace_Setter
    {
        
    }
    class VSCodeWorkspace
    {
        public List<VSCodeWorkspace_Folder> folders { get; set; }        
    }
    class VSCode
    {
        static VSCodeWorkspace workspace = new VSCodeWorkspace {
            folders = new List<VSCodeWorkspace_Folder>()
        };
        static string Include = "";
        static string Define = "";

        public static void AddDefine(string define)
        {
            Define += $"\"{define}\", ";
        }
        public static void AddPath(string name ,string path)
        {
            bool sign = false;
            if (!Directory.Exists(path))
            {
                return;
            }
            string p = Path.GetFullPath(path);
            workspace.folders.ForEach((t) =>
            {
                if(t.path == p)
                {
                    sign = true;
                }
            });
            if (sign)
            {
                return;
            }
            workspace.folders.Add(new VSCodeWorkspace_Folder { name = name,path = p });
        }

        public static void AddInclude(string path)
        {            
            string p = Path.GetFullPath(path);
            p = p.Replace("\\","/");
            Include += $"\"{p}\", ";
        }
        public static void Build(string name)
        {
            AddPath("Project",".");
            string json = JsonConvert.SerializeObject(workspace);
            json = json.Insert(json.LastIndexOf(']') + 1, ",\"settings\": { \"C_Cpp.default.includePath\":[" + Include + "]}");
            json = json.Insert(json.LastIndexOf(']') + 1, ",\"C_Cpp.default.defines\":[" + Define + "]");
            File.WriteAllText($"./{name}.code-workspace", json);
            workspace = new VSCodeWorkspace
            {
                folders = new List<VSCodeWorkspace_Folder>()
            };
            Include = "";
            Define = "";
        }
    }
}
