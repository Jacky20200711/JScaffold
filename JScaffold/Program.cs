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
            Console.WriteLine(@"輸入參數: 專案名稱, context名稱, 類別對應的表名, 類別路徑");
            Console.WriteLine(@"輸入範例: NekoFood, NekoFoodContext, Bentos, D:\Desktop\Project\NekoFood\NekoFood\Models\Bento.cs");
            Console.Write("> ");
            string input = Console.ReadLine();
            string[] names = input.Split(',');

            if (names.Length != 4)
            {
                Console.WriteLine("輸入錯誤，請檢查輸入的參數");
                return;
            }

            string projecName = names[0].Trim();
            string contextName = names[1].Trim();
            string tableName = names[2].Trim();
            string classPath = names[3].Replace('\\', '/').Trim();

            // 檢查類別路徑
            if (!File.Exists(classPath))
            {
                Console.WriteLine("類別檔案的路徑錯誤!");
                return;
            }

            // 通過路徑檢查後，取出類別名稱
            string className = classPath.Split('/')[^1].Replace(".cs", "");

            Dictionary<string, string> variables = ClassParser.GetVariable(classPath);

            // 產生程式碼
            ControllerGenerator controllerGenerator = new ControllerGenerator();
            ViewIndexGenerator viewIndexGenerator = new ViewIndexGenerator();
            ViewCreateGenerator viewCreateGenerator = new ViewCreateGenerator();
            ViewEditGenerator viewEditGenerator = new ViewEditGenerator();
            string text1 = controllerGenerator.GenerateCode(projecName, className, contextName, tableName, variables);
            string text2 = viewIndexGenerator.GenerateCode(className, variables, projecName);
            string text3 = viewCreateGenerator.GenerateCode(className, variables);
            string text4 = viewEditGenerator.GenerateCode(className, variables, projecName);

            // 設定輸出目錄
            string outputDir1 = $"D:/Desktop";
            string outputDir2 = $"{outputDir1}/{className}";
            if (!Directory.Exists(outputDir1))
            {
                Directory.CreateDirectory(outputDir1);
            }
            if (!Directory.Exists(outputDir2))
            {
                Directory.CreateDirectory(outputDir2);
            }

            // 寫入輸出目錄
            WriteToFile($"{outputDir1}/{className}Controller.cs", text1);
            WriteToFile($"{outputDir2}/Index.cshtml", text2);
            WriteToFile($"{outputDir2}/Create.cshtml", text3);
            WriteToFile($"{outputDir2}/Edit.cshtml", text4);
        }
    }
}