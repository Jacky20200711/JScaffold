using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JScaffold.Services;

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

            // 解析該類別內部的變數名稱與類型
            ClassParseService classParseService = new ClassParseService();
            Dictionary<string, string> variables = classParseService.Parse(classPath);

            // 若沒有指定 controllerName 則將 className 指派給他
            if (string.IsNullOrEmpty(controllerName))
            {
                controllerName = className;
            }

            // 創建程式碼產生服務
            Core31ControllerCodeGenService core31ControllerCodeGenService = new Core31ControllerCodeGenService();
            Core70ControllerCodeGenService core70ControllerCodeGenService = new Core70ControllerCodeGenService();
            ViewIndexCodeGenService viewIndexCodeGenService = new ViewIndexCodeGenService();
            ViewCreateCodeGenService viewCreateCodeGenService = new ViewCreateCodeGenService();
            ViewEditCodeGenService viewEditCodeGenService = new ViewEditCodeGenService();
            
            // 產生 Controller 的程式碼(有 Core 3.1 與 Core 7.0 兩種版本)
            string codeOfCore31Controller = core31ControllerCodeGenService.GenerateCode(projecName, className, contextName, tableName, variables, controllerName, primaryKeyName);
            string codeOfCore70Controller = core70ControllerCodeGenService.GenerateCode(projecName, className, contextName, tableName, variables, controllerName, primaryKeyName);

            // 產生 View 的程式碼(這些程式碼與後端框架無關，只要產生一組即可)
            string codeOfViewIndex = viewIndexCodeGenService.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);
            string codeOfViewCreate = viewCreateCodeGenService.GenerateCode(controllerName, variables, primaryKeyName);
            string codeOfViewEdit = viewEditCodeGenService.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);

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

            // 寫入 Core 3.1 的模板輸出路徑
            WriteToFile($"{core31SubDir1}/Index.cshtml", codeOfViewIndex);
            WriteToFile($"{core31SubDir1}/Create.cshtml", codeOfViewCreate);
            WriteToFile($"{core31SubDir1}/Edit.cshtml", codeOfViewEdit);
            WriteToFile($"{core31SubDir2}/{controllerName}Controller.cs", codeOfCore31Controller);

            // 寫入 Core 7.0 的模板輸出路徑
            WriteToFile($"{core70SubDir1}/Index.cshtml", codeOfViewIndex);
            WriteToFile($"{core70SubDir1}/Create.cshtml", codeOfViewCreate);
            WriteToFile($"{core70SubDir1}/Edit.cshtml", codeOfViewEdit);
            WriteToFile($"{core70SubDir2}/{controllerName}Controller.cs", codeOfCore70Controller);
        }
    }
}