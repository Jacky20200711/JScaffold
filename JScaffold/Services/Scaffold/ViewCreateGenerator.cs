﻿namespace JScaffold.Services.Scaffold
{
    public class ViewCreateGenerator
    {
        public string GenerateCode(string controllerName)
        {
            return $@"<script>
    // 這個函數用來將 Entity Code 轉回中文
    function decodeEntities(encodedString) {{
        var textArea = document.createElement('textarea');
        textArea.innerHTML = encodedString;
        return textArea.value;
    }}
    var serverMessage = decodeEntities('@TempData[""message""]');
    if (serverMessage.length > 0) {{
        alert(serverMessage);
    }}
</script>

<div id=""page-wrapper"">
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
                                    <div class=""form-group"">
                                        <label>分類</label>
                                        <select class=""form-control"" name=""field1"">
                                            <option>A</option>
                                            <option>B</option>
                                        </select>
                                    </div>
                                    <div class=""form-group"">
                                        <label>標題</label>
                                        <input class=""form-control"" name=""field2"" maxlength=""50"" required>
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