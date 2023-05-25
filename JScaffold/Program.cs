using JScaffold.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JScaffold.Services.Scaffold.Core31;
using JScaffold.Services.Scaffold.Core70;

namespace JScaffold
{
    class Program
    {
        public static void WriteToFile(string path, string text)
        {
            StreamWriter streamWriter = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8);
            streamWriter.Write(text);
            streamWriter.Close();
        }

        static void Main()
        {
            Console.WriteLine(@"輸入參數: 專案名稱, context名稱, 類別對應的表名, 類別路徑, controller名稱(非必填)");
            Console.WriteLine(@"輸入範例: MVCTestAdmin,DBContext,AdminUsers,D:\Desktop\Project\ProjectTest\MVCTestAdmin\Models\Entities\AdminUser.cs");
            Console.Write("> ");
            string input = Console.ReadLine();
            string[] names = input.Split(',');

            if (names.Length < 4 || names.Length > 5)
            {
                Console.WriteLine("參數數量錯誤!");
                return;
            }

            // 調整各項參數 & 去除空白
            string projecName = names[0].Trim();
            string contextName = names[1].Trim();
            string tableName = names[2].Trim();
            string classPath = names[3].Replace('\\', '/').Trim();
            string controllerName = names.Length == 5 ? names[4].Trim() : "";

            Console.Write(@"Name of Primary Key : ");
            string primaryKeyName = Console.ReadLine().Trim();
            if(primaryKeyName.Length == 0)
            {
                primaryKeyName = "id";
            }

            // 檢查類別路徑
            if (!File.Exists(classPath))
            {
                Console.WriteLine("類別路徑錯誤!");
                return;
            }

            // 通過路徑檢查後，取出類別名稱
            string className = classPath.Split('/')[^1].Replace(".cs", "");

            // 解析該類別內部的各項變數(名稱與類型)
            Dictionary<string, string> variables = ClassParser.GetVariable(classPath);

            // 若沒有指定 controllerName 則將 className 指派給他
            if (string.IsNullOrEmpty(controllerName))
            {
                controllerName = className;
            }

            // 產生程式碼
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            ViewIndexGenerator viewIndexGenerator = new ViewIndexGenerator();
            ViewCreateGenerator viewCreateGenerator = new ViewCreateGenerator();
            ViewEditGenerator viewEditGenerator = new ViewEditGenerator();
            string controllerCore31 = controllerGenerator.GenerateCode(projecName, className, contextName, tableName, variables, controllerName, primaryKeyName);
            string viewIndexCore31 = viewIndexGenerator.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);
            string viewCreateCore31 = viewCreateGenerator.GenerateCode(controllerName, variables, primaryKeyName);
            string viewEditCore31 = viewEditGenerator.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);

            ControllerGenerator2 controllerGenerator2 = new ControllerGenerator2();
            ViewIndexGenerator2 viewIndexGenerator2 = new ViewIndexGenerator2();
            ViewCreateGenerator2 viewCreateGenerator2 = new ViewCreateGenerator2();
            ViewEditGenerator2 viewEditGenerator2 = new ViewEditGenerator2();
            string controllerCore70 = controllerGenerator2.GenerateCode(projecName, className, contextName, tableName, variables, controllerName, primaryKeyName);
            string viewIndexCore70 = viewIndexGenerator2.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);
            string viewCreateCore70 = viewCreateGenerator2.GenerateCode(controllerName, variables, primaryKeyName);
            string viewEditCore70 = viewEditGenerator2.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);

            // 設定輸出目錄
            string rootDir = Directory.Exists("D:") ? "D:/Desktop" : "C:/Users/ycgis/Desktop";
            string core31OutputDir = $"{rootDir}/Core31";
            string core70OutputDir = $"{rootDir}/Core70";
            string core31SubDir1 = $"{rootDir}/Core31/Views/{controllerName}";
            string core70SubDir1 = $"{rootDir}/Core70/Views/{controllerName}";
            string core31SubDir2 = $"{rootDir}/Core31/Controllers";
            string core70SubDir2 = $"{rootDir}/Core70/Controllers";

            // 若輸出目錄不存在則創建
            List<string> outputDirList = new List<string>
            {
                core31OutputDir, core70OutputDir,
                core31SubDir1, core70SubDir1,
                core31SubDir2, core70SubDir2
            };

            foreach(var dName in outputDirList)
            {
                if (!Directory.Exists(dName))
                {
                    Directory.CreateDirectory(dName);
                }
            }

            // 寫入輸出目錄
            WriteToFile($"{core31SubDir1}/Index.cshtml", viewIndexCore31);
            WriteToFile($"{core31SubDir1}/Create.cshtml", viewCreateCore31);
            WriteToFile($"{core31SubDir1}/Edit.cshtml", viewEditCore31);
            WriteToFile($"{core31SubDir2}/{controllerName}Controller.cs", controllerCore31);

            WriteToFile($"{core70SubDir1}/Index.cshtml", viewIndexCore70);
            WriteToFile($"{core70SubDir1}/Create.cshtml", viewCreateCore70);
            WriteToFile($"{core70SubDir1}/Edit.cshtml", viewEditCore70);
            WriteToFile($"{core70SubDir2}/{controllerName}Controller.cs", controllerCore70);
        }
    }
}