using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ControllerGenerator
    {
        public string GenerateCode(string projectName, string controllerName, string contextName, string tableName, Dictionary<string, string> variables)
        {
            List<string> paras = new List<string>();
            string idName = "id";
            if (variables.ContainsKey("ID")) idName = "ID";
            if (variables.ContainsKey("Id")) idName = "Id";

            #region 取得提取參數的細節
            foreach (var item in variables)
            {
                // 排除 Primary key
                if (item.Key.ToLower() == "id")
                {
                    continue;
                }

                if (item.Value == "int" || item.Value == "int?")
                {
                    paras.Add($"                {item.Value} {item.Key} = int.Parse(PostData[\"{item.Key}\"]);");
                }
                else if (item.Value == "float" || item.Value == "float?")
                {
                    paras.Add($"                {item.Value} {item.Key} = float.Parse(PostData[\"{item.Key}\"]);");
                }
                else if (item.Value == "double" || item.Value == "double?")
                {
                    paras.Add($"                {item.Value} {item.Key} = double.Parse(PostData[\"{item.Key}\"]);");
                }
                else if (item.Value == "DateTime" || item.Value == "DateTime?")
                {
                    paras.Add($"                {item.Value} {item.Key} = DateTime.Now;");
                }
                else
                {
                    paras.Add($"                {item.Value} {item.Key} = PostData[\"{item.Key}\"].ToString().Trim();");
                }
            }
            string paraFetch = string.Join("\n", paras);
            #endregion

            #region 取得新增資料的細節
            paras.Clear();
            foreach (var item in variables)
            {
                // 排除 Primary key
                if (item.Key.ToLower() == "id")
                {
                    continue;
                }

                // 若是常見的特定欄位則額外處理
                if (item.Key == "modify_user" || item.Key == "ModifyUser")
                {
                    paras.Add($"                data.{item.Key} = Utility.GetLoginName(HttpContext);");
                }
                else if (item.Key == "modify_date" || item.Key == "ModifyDate")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string")
                {
                    paras.Add($"                data.{item.Key} = data.{item.Key}.Trim();");
                }
            }
            string paraAssign_create = string.Join("\n", paras);
            #endregion

            #region 取得修改資料的細節
            paras.Clear();
            foreach (var item in variables)
            {
                // 若是常見的特定欄位則優先處理
                if (item.Key == "modify_user" || item.Key == "ModifyUser")
                {
                    paras.Add($"                    {item.Key} = Utility.GetLoginName(HttpContext),");
                }
                else if (item.Key == "modify_date" || item.Key == "ModifyDate")
                {
                    paras.Add($"                    {item.Key} = DateTime.Now,");
                }
                // 若是一班的欄位則取出並轉型
                else if (item.Value == "int" || item.Value == "int?")
                {
                    paras.Add($"                    {item.Key} = int.Parse(PostData[\"{item.Key}\"]),");
                }
                else if (item.Value == "float" || item.Value == "float?")
                {
                    paras.Add($"                    {item.Key} = float.Parse(PostData[\"{item.Key}\"]),");
                }
                else if (item.Value == "double" || item.Value == "double?")
                {
                    paras.Add($"                    {item.Key} = double.Parse(PostData[\"{item.Key}\"]),");
                }
                else
                {
                    paras.Add($"                    {item.Key} = PostData[\"{item.Key}\"].ToString().Trim(),");
                }
            }
            string paraSetting = string.Join("\n", paras);

            paras.Clear();
            foreach (var item in variables)
            {
                // 排除 Primary key
                if (item.Key.ToLower() == "id")
                {
                    continue;
                }

                paras.Add($"                _context.Entry(editData).Property(p => p.{item.Key}).IsModified = true;");
            }
            string paraAssign_edit = string.Join("\n", paras);
            #endregion

            return $@"using {projectName}.Models.Entities;
using {projectName}.Services;
using {projectName}.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace {projectName}.Controllers
{{
    //[AuthorizeManager]
    public class {controllerName}Controller : Controller
    {{
        private readonly {contextName} _context;

        public {controllerName}Controller({contextName} context)
        {{
            _context = context;
        }}

        public async Task<IActionResult> Index()
        {{
            try
            {{
                var data = await _context.{tableName}.ToListAsync();
                return View(data);
            }}
            catch (Exception ex)
            {{
                Console.WriteLine($""取得 {controllerName} 失敗 -> {{ex}}"");
                TempData[""message""] = ""操作失敗"";
                return RedirectToRoute(new {{ controller = ""Home"", action = ""Index"" }});
            }}
        }}

        public IActionResult Create()
        {{
            return View();
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create({controllerName} data)
        {{
            try
            {{
{paraAssign_create}
                _context.Add(data);
                await _context.SaveChangesAsync();
                TempData[""message""] = ""新增成功"";
            }}
            catch (Exception ex)
            {{
                Console.WriteLine($""新增 {controllerName} 失敗 -> {{ex}}"");
                TempData[""message""] = ""操作失敗"";
            }}
            return RedirectToAction(""Index"");
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Result> Delete(int id)
        {{
            Result result = new Result
            {{
                Code = 0,
                Message = ""fail""
            }};

            try
            {{
                {controllerName} data = new {controllerName}() {{ {idName} = id }};
                _context.Entry(data).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                TempData[""message""] = ""刪除成功"";
                result.Code = 1;
                result.Message = ""success"";
                return result;
            }}
            catch (Exception ex)
            {{
                result.Message = ex.ToString();
            }}
            return result;
        }}

        public async Task<IActionResult> Edit(int? id)
        {{
            try
            {{
                var data = await _context.{tableName}.FindAsync(id);
                return View(data);
            }}
            catch (Exception ex)
            {{
                Console.WriteLine($""準備修改 {controllerName} 時發生錯誤 -> {{ex}}"");
                TempData[""message""] = ""操作失敗"";
                return RedirectToAction(""Index"");
            }}
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormCollection PostData)
        {{
            try
            {{
                // 創建資料
                {controllerName} editData = new {controllerName}()
                {{
{paraSetting}
                }};
                
                // 標記要修改的欄位
                _context.Attach(editData);
{paraAssign_edit}

                // 更新DB
                await _context.SaveChangesAsync();
                TempData[""message""] = ""修改成功"";
            }}
            catch (Exception ex)
            {{
                Console.WriteLine($""修改 {controllerName} 失敗 -> {{ex}}"");
                TempData[""message""] = ""操作失敗"";
            }}
            return RedirectToAction(""Index"");
        }}
    }}
}}
";
        }
    }
}