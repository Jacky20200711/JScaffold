using JScaffold.Services.Scaffold;
using JScaffold.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            Console.WriteLine(@"輸入範例: MVCTest, DBContext, AdminUsers, D:\Desktop\Project\ProjectTest\MVCTest\Models\Entities\AdminUser.cs");
            Console.WriteLine(@"輸入範例: MVCTestAdmin, DBContext, AdminUsers, D:\Desktop\Project\ProjectTest\MVCTest\Models\Entities\AdminUser.cs");
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
            string text1 = controllerGenerator.GenerateCode(projecName, className, contextName, tableName, variables, controllerName, primaryKeyName);
            string text2 = viewIndexGenerator.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);
            string text3 = viewCreateGenerator.GenerateCode(controllerName, variables, primaryKeyName);
            string text4 = viewEditGenerator.GenerateCode(className, variables, projecName, controllerName, primaryKeyName);

            // 設定輸出目錄
            string outputDir1 = Directory.Exists("D:") ? "D:/Desktop" : "C:/Users/ycgis/Desktop";
            string outputDir2 = $"{outputDir1}/{controllerName}";

            if (!Directory.Exists(outputDir1))
            {
                Directory.CreateDirectory(outputDir1);
            }
            if (!Directory.Exists(outputDir2))
            {
                Directory.CreateDirectory(outputDir2);
            }

            // 寫入輸出目錄
            WriteToFile($"{outputDir1}/{controllerName}Controller.cs", text1);
            WriteToFile($"{outputDir2}/Index.cshtml", text2);
            WriteToFile($"{outputDir2}/Create.cshtml", text3);
            WriteToFile($"{outputDir2}/Edit.cshtml", text4);
        }
    }
}