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

                paras.Add($"                    {item.Key} = {item.Key},");
            }
            string paraAssign_create = string.Join("\n", paras);
            #endregion

            #region 取得修改資料的細節
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

            return $@"using {projectName}.Models;
using {projectName}.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace {projectName}.Controllers
{{
    //[AuthorizeManager]
    public class {controllerName}Controller : Controller
    {{
        private readonly ILogger<{controllerName}Controller> _logger;
        private readonly {contextName} _context;

        public {controllerName}Controller({contextName} context, ILogger<{controllerName}Controller> logger)
        {{
            _context = context;
            _logger = logger;
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
                _logger.LogError($""取得 {controllerName} 失敗 -> {{ex}}"");
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
                _context.Add(data);
                await _context.SaveChangesAsync();
                TempData[""message""] = ""新增成功"";
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""新增 {controllerName} 失敗 -> {{ex}}"");
                TempData[""message""] = ""操作失敗"";
            }}
            return RedirectToAction(""Index"");
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> Delete(int id)
        {{
            try
            {{
                {controllerName} data = new {controllerName}() {{ {idName} = id }};
                _context.Entry(data).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return ""刪除成功"";
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""刪除 {controllerName} 失敗 -> {{ex}}"");
                return ""操作失敗"";
            }}
        }}

        public async Task<IActionResult> Edit(int? id)
        {{
            try
            {{
                var data = await _context.{tableName}.FindAsync(id);
                return View(data);
            }}
            catch (Exception)
            {{
                TempData[""message""] = ""操作失敗"";
                return RedirectToAction(""Index"");
            }}
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit({controllerName} data)
        {{
            try
            {{
                _context.Update(data);
                await _context.SaveChangesAsync();
                TempData[""message""] = ""修改成功"";
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""修改 {controllerName} 失敗 -> {{ex}}"");
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