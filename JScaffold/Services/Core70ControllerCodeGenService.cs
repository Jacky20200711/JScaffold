﻿using System.Collections.Generic;

namespace JScaffold.Services
{
    public class Core70ControllerCodeGenService
    {
        public string GenerateCode(string projectName, string className, string contextName, string tableName, Dictionary<string, string> variables, string controllerName, string primaryKeyName)
        {
            List<string> paras = new List<string>();

            // 設定 PK 名稱
            if (variables.ContainsKey("ID")) primaryKeyName = "ID";
            if (variables.ContainsKey("Id")) primaryKeyName = "Id";

            #region 設定新增資料的欄位
            paras.Clear();
            foreach (var item in variables)
            {
                // 忽略在新增時不會去異動的欄位
                if (item.Key == primaryKeyName) continue;

                // 若是常見的特定欄位則額外處理
                if (item.Key == "create_user" || item.Key == "CreateUser" || item.Key == "ModifyUser" || item.Key == "modify_user")
                {
                    paras.Add($"                data.{item.Key} = _loginService.GetUserName();");
                }
                else if (item.Key == "create_date" || item.Key == "CreateDate" || item.Key == "modify_date" || item.Key == "ModifyDate")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string")
                {
                    // 由於 Core 7.0 的 string 不可為 null，所以 Trim 之前不用添加問號
                    paras.Add($"                data.{item.Key} = data.{item.Key}.Trim();");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string?")
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
                // 忽略在修改時不會去異動的欄位
                if (item.Key == primaryKeyName) continue;
                if (item.Key == "create_user") continue;
                if (item.Key == "create_date") continue;
                if (item.Key == "CreateUser") continue;
                if (item.Key == "CreateDate") continue;

                // 若是常見的特定欄位則優先處理
                if (item.Key == "ModifyUser" || item.Key == "modify_user")
                {
                    paras.Add($"                data.{item.Key} = _loginService.GetUserName();");
                }
                else if (item.Key == "modify_date" || item.Key == "ModifyDate")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string")
                {
                    // 由於 Core 7.0 的 string 不可為 null，所以 Trim 之前不用添加問號
                    paras.Add($"                data.{item.Key} = data.{item.Key}.Trim();");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string?")
                {
                    paras.Add($"                data.{item.Key} = data.{item.Key}?.Trim();");
                }
            }

            string paraSetting = string.Join("\n", paras);
            paras.Clear();

            foreach (var item in variables)
            {
                // 忽略在修改時不會去異動的欄位
                if (item.Key == primaryKeyName) continue;
                if (item.Key == "create_user") continue;
                if (item.Key == "create_date") continue;
                if (item.Key == "CreateUser") continue;
                if (item.Key == "CreateDate") continue;

                paras.Add($"                _context.Entry(data).Property(p => p.{item.Key}).IsModified = true;");
            }
            string paraAssign_edit = string.Join("\n", paras);
            #endregion

            return $@"using {projectName}.Models.Entities;
using {projectName}.Models;
using {projectName}.Models.ActionFilters;
using {projectName}.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Threading.Tasks;
using System;
using MVCTestAdmin.Models.SearchParamModel;

namespace {projectName}.Controllers
{{
    [TypeFilter(typeof(LoginValidationFilter))]
    public class {controllerName}Controller : Controller
    {{
        private readonly {contextName} _context;
        private readonly LoginService _loginService;

        public {controllerName}Controller({contextName} context, LoginService loginService)
        {{
            _context = context;
            _loginService = loginService;
        }}

        public async Task<IActionResult> Index(SearchParamBase searchParam)
        {{
            try
            {{
                // 撈取資料
                var data = await _context.{tableName}.ToListAsync();

                // 確保頁數 >= 1 & 定義每個分頁的資料數量
                int pageNum = searchParam.PageNum < 1 ? 1 : searchParam.PageNum;
                int dataNumOfEachPage = 10;

                // 根據撈取的資料數量，來計算最大頁數，並限制分頁數量最多為9999
                int dataAmount = data.Count;
                int pageMax = (dataAmount % dataNumOfEachPage == 0) ? (dataAmount / dataNumOfEachPage) : (dataAmount / dataNumOfEachPage) + 1;
                pageMax = pageMax > 9999 ? 9999 : pageMax;

                // 設置前端分頁按鈕群會用到的參數
                if (pageNum > pageMax) pageNum = pageMax;
                TempData[""pageNum""] = pageNum;
                TempData[""pageMax""] = pageMax;

                // 擷取分頁所需的資料
                data = data.Skip((pageNum - 1) * dataNumOfEachPage).Take(dataNumOfEachPage).ToList();
                return View(data);
            }}
            catch (Exception ex)
            {{
                LogManager.GetLogger(""{controllerName}"").Error($""搜尋失敗，{{ex}}"");
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
        public async Task<IActionResult> Create({className} data)
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
                LogManager.GetLogger(""{controllerName}"").Error($""新增失敗，{{ex}}"");
                TempData[""message""] = ""操作失敗"";
            }}

            return RedirectToAction(""Index"");
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ApiReturn> Delete(int {primaryKeyName})
        {{
            ApiReturn apiReturn = new ApiReturn {{ Code = 0 }};

            try
            {{
                {className} data = new {className}() {{ {primaryKeyName} = {primaryKeyName} }};
                _context.Entry(data).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                //TempData[""message""] = ""刪除成功"";
                apiReturn.Code = 1;
            }}
            catch (Exception ex)
            {{
                LogManager.GetLogger(""{controllerName}"").Error($""刪除失敗，{{ex}}"");
            }}
            return apiReturn;
        }}

        public async Task<IActionResult> Edit(int? {primaryKeyName})
        {{
            try
            {{
                if ({primaryKeyName} == null)
                {{
                    return NotFound();
                }}

                var data = await _context.{tableName}.FindAsync({primaryKeyName});

                if(data == null)
                {{
                    return NotFound();
                }}

                return View(data);
            }}
            catch (Exception ex)
            {{
                LogManager.GetLogger(""{controllerName}"").Error($""點選修改出錯，{{ex}}"");
                TempData[""message""] = ""操作失敗"";
                return RedirectToAction(""Index"");
            }}
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit({className} data)
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
                LogManager.GetLogger(""{controllerName}"").Error($""送出修改失敗，{{ex}}"");
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