using System.Collections.Generic;

namespace JScaffold.Services.Scaffold.Core70
{
    public class ViewIndexGenerator2
    {
        public string GenerateCode(string className, Dictionary<string, string> variables, string projecName, string controllerName, string primaryKeyName)
        {
            List<string> paras = new List<string>();

            // 設定 PK 名稱
            if (variables.ContainsKey("ID")) primaryKeyName = "ID";
            if (variables.ContainsKey("Id")) primaryKeyName = "Id";

            #region 設定標題列
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;

                paras.Add($"                                <th style=\"white-space: nowrap;\">{item.Key}</th>");
            }
            paras.Add($"                                <th style=\"white-space: nowrap;\">Operation</th>");
            string paraTitle = string.Join("\n", paras);
            #endregion

            #region 設定欄位內容
            paras.Clear();
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;

                // 優先處理常見的欄位
                if(item.Value.ToLower().Contains("datetime"))
                {
                    paras.Add($"                                    <td style=\"white-space: nowrap;\">@(data.{item.Key} != null ? Convert.ToDateTime(data.{item.Key}).ToString(\"yyyy-MM-dd HH:mm\") : \"\")</td>");
                }
                else
                {
                    paras.Add($"                                    <td style=\"white-space: nowrap;\">@data.{item.Key}</td>");
                }
            }
            string paraContent = string.Join("\n", paras);
            #endregion

            return $@"@model List<{projecName}.Models.Entities.{className}>
@Html.AntiForgeryToken()
<h2>資料列表</h2>
<div class=""row"" style=""margin-top:20px;"">
    <div class=""col-lg-12"">
        <div class=""panel panel-default"">
            <div class=""panel-heading"">
                <div style=""display: flex;"">
                    <div style=""width:30%"">
                        <a class=""btn btn-primary"" href=""@Url.Action(""Create"",""{controllerName}"")"">新增資料</a>
                    </div>
                    <div style=""width:70%"">
                        @*<form>
                            <div style=""float:right;"">
                                <input type=""hidden"" name=""searchType"" value=""blur"">
                                <input type=""search"" id=""search"" name=""search"" style=""height:30px; position:relative; top:1px;"" maxlength=""50"" required />
                                <button class=""btn btn-primary"" type=""submit"" style=""height:33px;"">
                                    <i class=""fa fa-search""></i>
                                </button>
                                <a class=""btn btn-primary"" href=""@Url.Action(""Index"", ""PagePermission"")"">
                                    列出全部
                                </a>
                            </div>
                        </form>*@
                    </div>
                </div>
            </div>
            <div class=""panel-body"">
                <div class=""table-responsive"">
                    <table class=""table table-striped table-bordered table-hover"">
                        <thead>
                            <tr>
{paraTitle}
                            </tr>
                        </thead>
                        <tbody>
                            @foreach (var data in Model)
                            {{
                                <tr style=""background-color:white;"">
{paraContent}
                                    <td style=""white-space: nowrap;"">
                                        <button class=""btn btn-success"" onclick=""location.href='@Url.Action(""Edit"",""{controllerName}"", new {{ {primaryKeyName} = data.{primaryKeyName} }})'"">修改</button>
                                        <button class=""btn btn-danger"" onclick=""DeleteData(@data.{primaryKeyName})"">刪除</button>
                                    </td>
                                </tr>
                            }}    
                        </tbody>
                    </table>
                    @*下方是分頁按鈕群*@
                    <div style=""margin-top:15px; text-align:center;"">
                        <div style=""width:75%; margin-left:15%;"">
                            @*最多需要顯示 14 個按鈕，例如 << < [1]~[10] > >> *@
                            <button class=""btn_prev"" id=""btn_prev_prev"" onclick=""GetPageData('btn_prev_prev', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))"">&lt;&lt;</button>
                            <button class=""btn_prev"" style=""margin-right:4px;"" id=""btn_prev"" onclick=""GetPageData('btn_prev', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))"">&lt;</button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_1"" onclick=""GetPageData('btn_page_1', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_2"" onclick=""GetPageData('btn_page_2', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_3"" onclick=""GetPageData('btn_page_3', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_4"" onclick=""GetPageData('btn_page_4', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_5"" onclick=""GetPageData('btn_page_5', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_6"" onclick=""GetPageData('btn_page_6', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_7"" onclick=""GetPageData('btn_page_7', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_8"" onclick=""GetPageData('btn_page_8', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_9"" onclick=""GetPageData('btn_page_9', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_page"" style=""display:none;"" id=""btn_page_10"" onclick=""GetPageData('btn_page_10', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))""></button>
                            <button class=""btn_next"" style=""margin-left:1px;"" id=""btn_next"" onclick=""GetPageData('btn_next', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))"">&gt;</button>
                            <button class=""btn_next"" style=""margin-left:1px;"" id=""btn_next_next"" onclick=""GetPageData('btn_next_next', @((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))"">&gt;&gt;</button>
                            <span style=""margin-left:10px;"">共 @TempData[""pageMax""] 頁</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<script>
    function DeleteData(idOfData) {{
        Swal.fire({{
            title: ""確定要刪除?"",
            showCancelButton: true,
            customClass: {{
                title: 'swal-label-font-size',
                confirmButton: 'swal-button-font-size',
                cancelButton: 'swal-button-font-size',
            }},
        }}).then(function(result) {{
            if (result.value) {{
                $.ajax({{
                    beforeSend: function (xhr) {{
                        xhr.setRequestHeader(""requestverificationtoken"",
                            $('input:hidden[name=""__RequestVerificationToken""]').val());
                    }},
                    type: 'POST',
                    url: '@Url.Action(""Delete"", ""{controllerName}"")',
                    data: {{ {primaryKeyName}: idOfData }}
                }})
                .done(function (result) {{
                    if (result[""code""] == 1) {{
                        self.location.reload();
                    }}
                    else
                    {{
                        Swal.fire({{
                            title: '操作失敗',
                            customClass: {{
                                title: 'swal-label-font-size',
                                confirmButton: 'swal-button-font-size',
                                cancelButton: 'swal-button-font-size',
                            }}
                        }});
                    }}
                }});
            }}
        }});
    }}
</script>
<script src=""~/js/myPagination.js""></script>
<script>SetPaginationButton(@((int)TempData[""pageNum""]!), @((int)TempData[""pageMax""]!))</script>
";
        }
    }
}