using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KeilPorjectConvertor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            List<XmlDocument> keilProject = new List<XmlDocument>();
            foreach (var item in files)
            {
                if (item.EndsWith(".uvprojx"))
                {
                    XmlDocument xml = new XmlDocument();
                    try
                    {
                        xml.Load(item);
                    }
                    catch
                    {                        
                        return;
                    }


                    var list = xml.SelectNodes("/Project/Targets/Target");

                    foreach (XmlNode target in list)
                    {                        
                        Console.WriteLine($"TargetName {target["TargetName"].InnerText}");
                        XmlNode VariousControls = target.SelectSingleNode("TargetOption/TargetArmAds/Cads/VariousControls");
                        /*添加宏定义*/
                        string define = VariousControls["Define"].InnerText;
                        var defines = define.Split(',');
                        foreach (var defineItem in defines)
                        {
                            Console.WriteLine($"Define {defineItem}");
                            VSCode.AddDefine(defineItem);
                        }
                        /*添加头文件目录*/
                        string incPath = VariousControls["IncludePath"].InnerText;
                        var includeLists = incPath.Split(';');
                        foreach (var includeNode in includeLists)
                        {
                            Console.WriteLine($"IncludePah {includeNode}");
                            VSCode.AddPath(GetPath(includeNode), includeNode);
                            VSCode.AddInclude(includeNode);
                        }
                        /*添加源文件目录*/
                        var groupList = target.SelectNodes("Groups/Group");
                        foreach (XmlNode groupNode in groupList)
                        {
                            Console.WriteLine($"GroupName {groupNode["GroupName"].InnerText}");
                            var fileList = groupNode.SelectNodes("Files/File");
                            foreach (XmlNode fileNode in fileList)
                            {
                                Console.WriteLine($"FileName {fileNode["FileName"].InnerText}");
                                VSCode.AddPath(GetPath(Path.GetDirectoryName(fileNode["FilePath"].InnerText))
                                    ,Path.GetDirectoryName(fileNode["FilePath"].InnerText));
                            }
                        }
                        VSCode.Build(target["TargetName"].InnerText);
                    }

                    keilProject.Add(xml);
                }
            }            
        }


        static string GetPath(string path)
        {
            return path.Replace("..\\","");
        }
    }
}
