using System.Collections.Generic;

namespace JScaffold.Services
{
    public class ViewEditCodeGenService
    {
        public string GenerateCode(string className, Dictionary<string, string> variables, string projecName, string controllerName, string primaryKeyName)
        {
            List<string> paras = new List<string>();

            // 設定 PK 名稱
            if (variables.ContainsKey("ID")) primaryKeyName = "ID";
            if (variables.ContainsKey("Id")) primaryKeyName = "Id";

            #region 設定欄位內容
            foreach (var item in variables)
            {
                // 忽略不會顯示的欄位
                if (item.Key.ToLower() == primaryKeyName.ToLower()) continue;
                if (item.Key == "modify_user" || item.Key == "ModifyUser") continue;
                if (item.Key == "create_user" || item.Key == "CreateUser") continue;
                if (item.Key == "modify_date" || item.Key == "ModifyDate") continue;
                if (item.Key == "create_date" || item.Key == "CreateDate") continue;

                if (item.Key.ToLower().StartsWith("remark"))
                {
                    paras.Add($"        <tr>");
                    paras.Add($"            <td>{item.Key}</td>");
                    paras.Add($"            <td><textarea name=\"{item.Key}\" rows=\"4\" maxlength=\"200\" style=\"width:100%\"></textarea></td>");
                    paras.Add($"        </tr>");
                    continue;
                }

                // 判斷 input 的欄位類型
                string inputType = "text";
                string inputValue = $"@Model.{item.Key}";

                if (item.Value.ToLower().Contains("datetime"))
                {
                    inputType = "date";
                    inputValue = $"@(Model.{item.Key} != null ? Convert.ToDateTime(Model.{item.Key}).ToString(\"yyyy-MM-dd\") : \"\")";
                }

                paras.Add($"        <tr>");
                paras.Add($"            <td>{item.Key}</td>");
                paras.Add($"            <td><input type=\"{inputType}\" name=\"{item.Key}\" value=\"{inputValue}\" maxlength=\"100\"></td>");
                paras.Add($"        </tr>");

            }
            string paraInput = string.Join("\n", paras);
            #endregion

            return $@"@model {projecName}.Models.Entities.{className}
<link href=""~/css/myEdit.css"" rel=""stylesheet"" type=""text/css"">
<link href=""~/css/myBoxStyle.css"" rel=""stylesheet"" type=""text/css"">
<form role=""form"" asp-controller=""{controllerName}"" asp-action=""Edit"" method=""post"">
    @Html.AntiForgeryToken()
    <table>
{paraInput}
        <tr>
            <td>多選模板</td>
            <td><input type=""checkbox"" />&nbsp;加蛋 <input type=""checkbox"" style=""margin-left:10px;"" />&nbsp;加肉</td>
        </tr>
        <tr>
            <td>單選模板</td>
            <td><input type=""radio"" name=""radio1"" />&nbsp;是 <input type=""radio"" name=""radio1"" style=""margin-left:24px;"" />&nbsp;否</td>
        </tr>
        <tr>
            <td>日期模板</td>
            <td><input type=""date"" /></td>
        </tr>
        <tr>
            <td>下拉模板</td>
            <td>
                <select>
                    <option>請選擇</option>
                    <option>提交中</option>
                    <option>請補正</option>
                    <option>已審核</option>
                </select>
            </td>
        </tr>
        <tr>
            <td>上傳模板</td>
            <td><input type=""file"" id=""file1"" /></td>
        </tr>
        </table>
        <input type=""hidden"" name=""{primaryKeyName}"" value=""@Model.{primaryKeyName}"" />
        <div style=""margin-top:10px; text-align:center;"">
            <button type=""submit"" class=""btn btn-primary"">送出</button>
            <a class=""btn btn-danger"" href=""@Url.Action(""Index"", ""{controllerName}"")"">返回列表</a>
        </div>
</form>
";
        }
    }
}