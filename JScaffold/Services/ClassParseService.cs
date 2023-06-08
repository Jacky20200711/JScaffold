using System.Collections.Generic;
using System.IO;

namespace JScaffold.Services
{
    public class ClassParseService
    {
        public static HashSet<string> dType = new HashSet<string>
        {
            "DateTime", "DateTime?", "int", "int?", "float", "float?",
            "double", "double?", "bool", "string", "string?"
        };

        /// <summary>
        /// 分析類別檔案，取出變數類型與名稱
        /// </summary>
        /// <returns>所有變數的名稱與類型(Key=變數名稱，Val=變數類型)</returns>
        public Dictionary<string, string> Parse(string fileName)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            HashSet<string> dType = new HashSet<string>
            {
                "DateTime", "DateTime?", "int", "int?", "float", "float?",
                "double", "double?", "bool", "string", "string?"
            };

            StreamReader streamReader = new StreamReader(fileName);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                // 將每一行以空白做切割
                string[] sSplit = line.Split(' ');

                // 檢查切割結果
                for (int i = 0; i < sSplit.Length; i++)
                {
                    // 若偵測到合法的變數類型，則視為找到一組變數名稱與類型
                    if (dType.Contains(sSplit[i]))
                    {
                        variables[sSplit[i + 1]] = sSplit[i];
                    }
                }
            }

            streamReader.Close();
            return variables;
        }
    }
}
