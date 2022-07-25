using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ViewCreateGenerator
    {
        public string GenerateCode(string controllerName, Dictionary<string, string> variables)
        {
            List<string> paras = new List<string>();

            #region 設定欄位內容
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;
                if(item.Key == "modify_user" || item.Key == "ModifyUser") continue;
                if (item.Key == "modify_date" || item.Key == "ModifyDate") continue;

                paras.Add($"                                    <div class=\"form-group\">");
                paras.Add($"                                        <label>{item.Key}</label>");
                paras.Add($"                                        <input class=\"form-control\" name=\"{item.Key}\" maxlength=\"100\">");
                paras.Add($"                                    </div>");

            }
            string paraInput = string.Join("\n", paras);
            #endregion

            return $@"<div id=""page-wrapper"">
    <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h1 class=""page-header"">新增資料</h1>
            </div>
        </div>
        <div class=""row"">
            <div class=""col-lg-12"">
                <div class=""panel panel-default"">
                    <div class=""panel-heading"">
                        請填寫下列欄位
                    </div>
                    <div class=""panel-body"">
                        <div class=""row"">
                            <div class=""col-lg-6"">
                                <form role=""form"" asp-controller=""{controllerName}"" asp-action=""Create"">
{paraInput}
                                    <div class=""form-group"">
                                        <label>選擇器模板，若不需要則自行移除</label>
                                        <select class=""form-control"" name=""fieldName"">
                                            <option>A</option>
                                            <option>B</option>
                                        </select>
                                    </div>
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