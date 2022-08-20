﻿using System.Collections.Generic;

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

            #region 設定新增資料的欄位
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
                    paras.Add($"                data.{item.Key} = data.{item.Key}?.Trim();");
                }
            }
            string paraAssign_create = string.Join("\n", paras);
            #endregion

            #region 設定修改資料的欄位
            paras.Clear();
            foreach (var item in variables)
            {
                // 若是常見的特定欄位則優先處理
                if (item.Key == "modify_user" || item.Key == "ModifyUser")
                {
                    paras.Add($"                data.{item.Key} = Utility.GetLoginName(HttpContext);");
                }
                else if (item.Key == "modify_date" || item.Key == "ModifyDate")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
                }
                else if (item.Value.StartsWith("string"))
                {
                    paras.Add($"                data.{item.Key} = data.{item.Key}?.Trim();");
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

                paras.Add($"                _context.Entry(data).Property(p => p.{item.Key}).IsModified = true;");
            }
            string paraAssign_edit = string.Join("\n", paras);
            #endregion

            return $@"using {projectName}.Models.Entities;
using {projectName}.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace {projectName}.Controllers
{{
    //[AuthorizeManager]
    public class {controllerName}Controller : Controller
    {{
        private readonly {contextName} _context;
        private readonly ILogger<{controllerName}> _logger;

        public {controllerName}Controller({contextName} context, ILogger<{controllerName}> logger)
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
                _logger.LogError($""撈取 {controllerName} 失敗 -> {{ex}}"");
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
                _logger.LogError($""新增 {controllerName} 失敗 -> {{ex}}"");
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
                Code = 0
            }};

            try
            {{
                {controllerName} data = new {controllerName}() {{ {idName} = id }};
                _context.Entry(data).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                TempData[""message""] = ""刪除成功"";
                result.Code = 1;
                return result;
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""刪除 {controllerName} 失敗 -> {{ex}}"");
            }}
            return result;
        }}

        public async Task<IActionResult> Edit(int? id)
        {{
            try
            {{
                if (id == null)
                {{
                    return NotFound();
                }}

                var data = await _context.{tableName}.FindAsync(id);

                if(data == null)
                {{
                    return NotFound();
                }}

                return View(data);
            }}
            catch (Exception ex)
            {{
                _logger.LogError($""準備修改 {controllerName} 時發生錯誤 -> {{ex}}"");
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
                // 調整欄位
{paraSetting}
                
                // 標記要修改的欄位
                _context.Attach(data);
{paraAssign_edit}

                // 更新DB
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