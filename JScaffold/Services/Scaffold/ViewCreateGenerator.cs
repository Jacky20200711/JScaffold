﻿using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ViewCreateGenerator
    {
        public string GenerateCode(string controllerName, Dictionary<string, string> variables, string primaryKeyName)
        {
            List<string> paras = new List<string>();

            // 設定 PK 名稱
            string idName = primaryKeyName;
            if (variables.ContainsKey("ID")) idName = "ID";
            if (variables.ContainsKey("Id")) idName = "Id";

            #region 設定欄位內容
            foreach (var item in variables)
            {
                // 忽略不會顯示的欄位
                if (item.Key.ToLower() == idName.ToLower()) continue;
                if (item.Key == "modify_user" || item.Key == "ModifyUser") continue;
                if (item.Key == "create_user" || item.Key == "CreateUser") continue;
                if (item.Key == "modify_date" || item.Key == "ModifyDate") continue;
                if (item.Key == "create_date" || item.Key == "CreateDate") continue;

                if (item.Key.ToLower().StartsWith("remark"))
                {
                    paras.Add($"                                    <div class=\"form-group\">");
                    paras.Add($"                                        <label>{item.Key}</label>");
                    paras.Add($"                                        <textarea class=\"form-control\" name=\"{item.Key}\" rows=\"4\" maxlength=\"200\"></textarea>");
                    paras.Add($"                                    </div>");
                    continue;
                }

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
                                    <div class=""form-group"" style=""margin-top:25px;"">
                                        @{{
                                            string cBoxStyle = ""width:18px; height:18px; cursor:pointer; margin-top:3px;"";
                                            string cBoxLabelStyle = ""margin-left:4px; font-size:14px; margin-top:3px;"";
                                        }}    
                                        <label>Inline Checkboxes測試</label>&nbsp;&nbsp;&nbsp;&nbsp;
                                        <label class=""checkbox-inline"">
                                            <input type=""checkbox"" name=""cbox1"" style=""@cBoxStyle"" checked>
                                            <label style=""@cBoxLabelStyle"">測試1</label>
                                        </label>
                                        <label class=""checkbox-inline"">
                                            <input type=""checkbox"" name=""cbox2"" style=""@cBoxStyle"">
                                            <label style=""@cBoxLabelStyle"">測試2</label>
                                        </label>
                                    </div>
                                    <div class=""form-group"" style=""margin-top:25px;"">
                                        @{{
                                            string radioStyle = ""width:18px; height:18px; cursor:pointer; margin-top:3px;"";
                                            string radioLabelStyle = ""margin-left:4px; font-size:14px; margin-top:3px;"";
                                        }}
                                        <label>Inline CheckRadio測試</label>&nbsp;&nbsp;&nbsp;&nbsp;
                                        <label class=""radio-inline"">
                                            <input type=""radio"" style=""@radioStyle"" name=""rbox"" value=""option1"" checked>
                                            <label style=""@radioLabelStyle"">測試1</label>
                                        </label>
                                        <label class=""radio-inline"">
                                            <input type=""radio"" style=""@radioStyle"" name=""rbox"" value=""option2"">
                                            <label style=""@radioLabelStyle"">測試2</label>
                                        </label>
                                    </div>
                                    <button type=""submit"" class=""btn btn-primary"">送出</button>
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