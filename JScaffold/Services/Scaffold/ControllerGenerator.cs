using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ControllerGenerator
    {
        public string GenerateCode(string projectName, string controllerName, string contextName, string tableName, Dictionary<string, string> variables)
        {
            List<string> paras = new List<string>();

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
                    paras.Add($"                {item.Value} {item.Key} = int.TryParse(PostData[\"{item.Key}\"].ToString(), out int val) ? val : 0;");
                }
                else if (item.Value == "float" || item.Value == "float?")
                {
                    paras.Add($"                {item.Value} {item.Key} = float.Parse(PostData[\"{item.Key}\"].ToString());");
                }
                else if (item.Value == "double" || item.Value == "double?")
                {
                    paras.Add($"                {item.Value} {item.Key} = double.Parse(PostData[\"{item.Key}\"].ToString());");
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

                paras.Add($"                data.{item.Key} = {item.Key};");
            }
            string paraAssign_edit = string.Join("\n", paras);
            #endregion

            return $@"using {projectName}.Models;
using {projectName}.Models.Entities;
using {projectName}.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text;

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
                return Content(""<h2>資料庫異常，請聯絡相關人員!</h2>"", ""text/html"", Encoding.UTF8);
            }}
        }}

        public IActionResult Create()
        {{
            return View();
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection PostData)
        {{
            try
            {{
                // 提取參數
{paraFetch}
                
                // 創建資料
                {controllerName} newData = new {controllerName}()
                {{
{paraAssign_create}
                }};
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData[""message""] = ""新增成功"";
                return RedirectToAction(""Index"");
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""新增 {controllerName} 失敗 -> {{ex}}"");
                return Content(""<h2>資料庫異常，請聯絡相關人員!</h2>"", ""text/html"", Encoding.UTF8);
            }}
        }}

        [HttpPost]
        public async Task<string> Delete(int? id)
        {{
            try
            {{
                #region 檢查此筆資料是否存在

                if (id == null)
                {{
                    return ""資料不存在!"";
                }}

                var data = await _context.{tableName}.FindAsync(id);

                if (data == null)
                {{
                    return ""資料不存在!"";
                }}

                #endregion

                // 更新DB
                _context.Remove(data);
                await _context.SaveChangesAsync();

                return ""刪除成功"";
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""刪除 {controllerName} 失敗 -> {{ex}}"");
                return ""資料庫異常，請聯絡相關人員!"";
            }}
        }}

        public async Task<IActionResult> Edit(int? id)
        {{
            try
            {{
                #region 檢查此筆資料是否存在

                if (id == null)
                {{
                    return Content(""<h2>資料id錯誤!</h2>"", ""text/html"", Encoding.UTF8);
                }}

                var data = await _context.{tableName}.FindAsync(id);

                if (data == null)
                {{
                    return Content(""<h2>資料不存在!</h2>"", ""text/html"", Encoding.UTF8);
                }}

                #endregion

                return View(data);
            }}
            catch (Exception)
            {{
                return Content(""<h2>資料庫異常，請聯絡相關人員!</h2>"", ""text/html"", Encoding.UTF8);
            }}
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormCollection PostData)
        {{
            try
            {{
                // 提取參數
                int id = int.Parse(PostData[""id""].ToString());
{paraFetch}

                // 撈取目標
                var data = await _context.{tableName}.FindAsync(id);
                if (data == null)
                {{
                    TempData[""message""] = ""修改失敗，此筆資料不存在"";
                    return RedirectToAction(""Index"");
                }}

                // 修改資料並更新DB
{paraAssign_edit}
                await _context.SaveChangesAsync();

                TempData[""message""] = ""修改成功"";
                return RedirectToAction(""Index"");
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""修改 {controllerName} 失敗 -> {{ex}}"");
                return Content(""<h2>資料庫異常，請聯絡相關人員!</h2>"", ""text/html"", Encoding.UTF8);
            }}
        }}
    }}
}}";
        }
    }
}