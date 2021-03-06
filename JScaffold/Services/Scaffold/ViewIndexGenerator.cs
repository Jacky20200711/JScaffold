using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ViewIndexGenerator
    {
        public string GenerateCode(string controllerName, Dictionary<string, string> variables, string projecName)
        {
            List<string> paras = new List<string>();
            string idName = "id";
            if (variables.ContainsKey("ID")) idName = "ID";
            if (variables.ContainsKey("Id")) idName = "Id";

            #region 設定標題列
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;

                paras.Add($"                                        <th style=\"white-space: nowrap;\">{item.Key}</th>");
            }
            paras.Add($"                                        <th style=\"white-space: nowrap;\">操作選項</th>");
            string paraTitle = string.Join("\n", paras);
            #endregion

            #region 設定欄位內容
            paras.Clear();
            foreach (var item in variables)
            {
                if (item.Key.ToLower() == "id") continue;

                paras.Add($"                                                    <td style=\"white-space: nowrap;\">@data.{item.Key}</td>");
            }
            string paraContent = string.Join("\n", paras);
            #endregion

            return $@"@model List<{projecName}.Models.Entities.{controllerName}>

<script>
    // 這個函數用來將 Entity Code 轉回中文
    function decodeEntities(encodedString) {{
        var textArea = document.createElement('textarea');
        textArea.innerHTML = encodedString;
        return textArea.value;
    }}
    var serverMessage = decodeEntities('@TempData[""message""]');
    if (serverMessage.length > 0)
    {{
        Swal.fire(serverMessage);
    }}
</script>

<script>
    function DeleteData(idOfData) {{
        var result = confirm(""確定要刪除這筆資料?"");
        if (result) {{
            $.ajax({{
                beforeSend: function (xhr) {{
                    xhr.setRequestHeader(""requestverificationtoken"",
                        $('input:hidden[name=""__RequestVerificationToken""]').val());
                }},
                type: 'POST',
                url: '@Url.Action(""Delete"", ""{controllerName}"")',
                data: {{ id: idOfData }}
            }})
            .done(function (result) {{
                if (result[""code""] == 1) {{
                    self.location.reload();
                }}
                else
                {{
                    Swal.fire(""操作失敗"");
                }}
            }});
        }}
    }}
</script>

@Html.AntiForgeryToken()
<div id = ""page-wrapper"" >
     <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h1 class=""page-header"">資料列表</h1>
            </div>
        </div>
        <div class=""row"">
            <div class=""col-lg-12"">
                <div class=""panel panel-default"">
                    <div class=""panel-heading"">
                        <button class=""btn btn-primary"" onclick=""location.href='@Url.Action(""Create"",""{controllerName}"")'"">新增</button>
                    </div>
                    <!-- /.panel-heading -->
                    <div class=""panel-body"">
                        <div class=""table-responsive"">
                            <table class=""table table-striped table-bordered table-hover"" id=""dataTables-example"">
                                <thead>
                                    <tr>
                                        <th style=""display:none;"">排版用(不顯示)</th>
{paraTitle}
                                    </tr>
                                </thead>
                                <tbody>
                                    @{{
                                        int sequence = 0;
                                        foreach (var data in Model)
                                        {{
                                            sequence++;
                                            <tr class=""gradeA"">
                                                <td style=""display:none;"">@sequence</td>
{paraContent}   
                                                <td style=""white-space: nowrap;"">
                                                    <button class=""btn btn-success"" onclick=""location.href='@Url.Action(""Edit"", ""{controllerName}"", new {{ id = data.{idName} }})'"">修改</button>
                                                    <button class=""btn btn-danger"" onclick=""DeleteData(@data.{idName})"">刪除</button>
                                                </td>
                                            </tr>
                                        }}
                                    }}
                                </tbody>
                            </table>
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