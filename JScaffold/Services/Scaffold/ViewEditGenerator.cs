using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ViewEditGenerator
    {
        public string GenerateCode(string controllerName, Dictionary<string, string> variables, string projecName)
        {
            List<string> paras = new List<string>();
            string idName = "id";
            if (variables.ContainsKey("ID")) idName = "ID";
            if (variables.ContainsKey("Id")) idName = "Id";

            #region 設定欄位內容
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;
                if (item.Key == "modify_user" || item.Key == "ModifyUser") continue;
                if (item.Key == "modify_date" || item.Key == "ModifyDate") continue;

                paras.Add($"                                        <div class=\"form-group\">");
                paras.Add($"                                            <label>{item.Key}</label>");
                paras.Add($"                                            <input class=\"form-control\" name=\"{item.Key}\" maxlength=\"100\" value=\"@Model.{item.Key}\">");
                paras.Add($"                                        </div>");

            }
            string paraInput = string.Join("\n", paras);
            #endregion

            return $@"@model {projecName}.Models.Entities.{controllerName}

<div id=""page-wrapper"">
    <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h1 class=""page-header"">修改資料</h1>
            </div>
            <!-- /.col-lg-12 -->
        </div>
        <!-- /.row -->
        <div class=""row"">
            <div class=""col-lg-12"">
                <div class=""panel panel-default"">
                    <div class=""panel-heading"">
                        修改完畢後，點選下方的送出按鈕
                    </div>
                    <div class=""panel-body"">
                        <div class=""row"">
                            <div class=""col-lg-6"">
                                <form role=""form"" asp-controller=""{controllerName}"" asp-action=""Edit"">
                                    <div class=""form-group"">
{paraInput}
                                        <label>選擇器模板，若不需要則自行移除</label>
                                        <select class=""form-control"" name=""fieldName"">
                                            <option>A</option>
                                            <option>B</option>
                                        </select>
                                    </div>
                                    <input type=""hidden"" name=""id"" value=@Model.{idName} />
                                    <button type=""submit"" class=""btn btn-primary"">送出</button>
                                    <button type=""reset"" class=""btn btn-success"">重設</button>
                                    <a class=""btn btn-danger"" href=""@Url.Action(""Index"", ""{controllerName}"")"">返回列表</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
";
        }
    }
}