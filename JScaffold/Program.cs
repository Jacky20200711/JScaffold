using JScaffold.Services.Scaffold;
using System;
using System.IO;

namespace JScaffold
{
    class Program
    {
        public static void GenerateCode()
        {
            Console.WriteLine("請輸入各項參數(專案,類別,DB實體,DB實體中的表名)");
            Console.WriteLine("輸入範例: CSKDAdmin, UserPermission, CSKDContext, UserPermissions");
            Console.Write(": ");
            string input = Console.ReadLine();
            string[] names = input.Split(',');

            if (names.Length != 4)
            {
                Console.WriteLine("輸入錯誤，請檢查輸入的參數數量");
                return;
            }

            string projecName = names[0].Trim();
            string controllerName = names[1].Trim();
            string contextName = names[2].Trim();
            string tableName = names[3].Trim();

            // 產生程式碼
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            ViewIndexGenerator viewIndexGenerator = new ViewIndexGenerator();
            ViewCreateGenerator viewCreateGenerator = new ViewCreateGenerator();
            ViewEditGenerator viewEditGenerator = new ViewEditGenerator();
            string text1 = controllerGenerator.GenerateCode(projecName, controllerName, contextName, tableName);
            string text2 = viewIndexGenerator.GenerateCode(controllerName);
            string text3 = viewCreateGenerator.GenerateCode(controllerName);
            string text4 = viewEditGenerator.GenerateCode(controllerName);

            // 建立 Views 的資料夾
            string path = $"D:/Desktop/{controllerName}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 寫入到檔案
            WriteToFile($"D:/Desktop/{controllerName}Controller.cs", text1);
            WriteToFile($"D:/Desktop/{controllerName}/Index.cshtml", text2);
            WriteToFile($"D:/Desktop/{controllerName}/Create.cshtml", text3);
            WriteToFile($"D:/Desktop/{controllerName}/Edit.cshtml", text4);
        }

        public static void WriteToFile(string path, string text)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(text);
            streamWriter.Close();
        }

        static void Main()
        {
            GenerateCode();
        }
    }
}
