using JScaffold.Services.Scaffold;
using JScaffold.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace JScaffold
{
    class Program
    {
        public static void WriteToFile(string path, string text)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(text);
            streamWriter.Close();
        }

        static void Main()
        {
            Console.WriteLine(@"輸入參數: 專案名稱, 類別名稱, context名稱, 在context的表名, 類別路徑");
            Console.WriteLine(@"輸入範例: NekoFood, Bento, NekoFoodContext, Bentos, D:\Desktop\Project\NekoFood\NekoFood\Models\Bento.cs");
            Console.Write("> ");
            string input = Console.ReadLine();
            string[] names = input.Split(',');

            if (names.Length != 5)
            {
                Console.WriteLine("輸入錯誤，請檢查輸入的參數數量");
                return;
            }

            string projecName = names[0].Trim();
            string controllerName = names[1].Trim();
            string contextName = names[2].Trim();
            string tableName = names[3].Trim();
            string classPath = names[4].Trim();

            // 檢查類別路徑
            if (!File.Exists(classPath))
            {
                Console.WriteLine("類別檔案的路徑錯誤!");
                return;
            }

            Dictionary<string, string> variables = ClassParser.GetVariable(classPath);

            // 產生程式碼
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            ViewIndexGenerator viewIndexGenerator = new ViewIndexGenerator();
            ViewCreateGenerator viewCreateGenerator = new ViewCreateGenerator();
            ViewEditGenerator viewEditGenerator = new ViewEditGenerator();
            string text1 = controllerGenerator.GenerateCode(projecName, controllerName, contextName, tableName, variables);
            string text2 = viewIndexGenerator.GenerateCode(controllerName, variables);
            string text3 = viewCreateGenerator.GenerateCode(controllerName, variables);
            string text4 = viewEditGenerator.GenerateCode(controllerName, variables);

            // 設定輸出目錄
            string outputDir1 = $"D:/Desktop";
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