﻿namespace JScaffold.Services.Scaffold
{
    public class ViewIndexGenerator
    {
        public string GenerateCode(string controllerName)
        {
            return $@"@model List<{controllerName}>

<script>
    function DeleteData(idOfData) {{
        var result = confirm(""確定要刪除這筆資料?"");
        if (result) {{
            $.ajax({{
                type: 'POST',
                url: '@Url.Action(""Delete"", ""{controllerName}"")',
                data: {{ id: idOfData }}
            }})
            .done(function (message) {{
                alert(message);
                if (message == ""刪除成功"") {{
                    self.location.reload();
                }}
            }});
        }}
    }}
</script>

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
        alert(serverMessage);
    }}
</script>

<div id = ""page-wrapper"" >
     <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-lg-12"">
                <h1 class=""page-header"">資料列表</h1>
            </div>
            <!-- /.col-lg-12 -->
        </div>
        <!-- /.row -->
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
                                        <th style=""white-space: nowrap;"">分類</th>
                                        <th style=""white-space: nowrap;"">標題</th>
                                        <th style=""white-space: nowrap;"">說明</th>
                                        <th style=""white-space: nowrap;"">檔案位置</th>
                                        <th style=""white-space: nowrap;"">修改人員</th>
                                        <th style=""white-space: nowrap;"">修改日期</th>
                                        <th style=""white-space: nowrap;"">操作選項</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @{{
                                        if(Model != null)
                                        {{
                                            int sequence = 0;
                                            foreach (var data in Model)
                                            {{
                                                sequence++;
                                                <tr class=""gradeA"">
                                                    <td style=""display:none;"">@sequence</td>
                                                    @*
                                                    <td style=""white-space: nowrap;"">@data.MenuName</td>
                                                    <td style=""white-space: nowrap;"">@Convert.ToDateTime(data.ModifyDate).ToString(""yyyy-MM-dd HH:mm"")</td>
                                                    *@
                                                    <td style=""white-space: nowrap;"">
                                                        <button class=""btn btn-success"" onclick=""location.href='@Url.Action(""Edit"", ""{controllerName}"", new {{ id = data.Id }})'"">修改</button>
                                                        <button class=""btn btn-danger"" onclick=""DeleteData(@data.Id)"">刪除</button>
                                                    </td>
                                                </tr>
                                            }}
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
</div>";
        }
    }
}