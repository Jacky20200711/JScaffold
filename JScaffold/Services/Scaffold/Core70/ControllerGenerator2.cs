using System.Collections.Generic;

namespace JScaffold.Services.Scaffold.Core70
{
    public class ControllerGenerator2
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
                if (item.Key == "modify_user") continue;
                if (item.Key == "modify_date") continue;
                if (item.Key == "ModifyUser") continue;
                if (item.Key == "ModifyDate") continue;

                // 若是常見的特定欄位則額外處理
                if (item.Key == "create_user" || item.Key == "CreateUser")
                {
                    paras.Add($"                data.{item.Key} = _loginService.GetUserName();");
                }
                else if (item.Key == "create_date" || item.Key == "CreateDate")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string")
                {
                    paras.Add($"                data.{item.Key} = data.{item.Key}?.Trim();");
                }
                // 若是一般的字串則去除首尾空白
                else if (item.Value == "string?")
                {
                    paras.Add($"                data.{item.Key} = data.{item.Key}?.Trim();");
                }
                // 若是日期則一律給 DateTime.Now
                else if (item.Value == "DateTime?" || item.Value == "DateTime")
                {
                    paras.Add($"                data.{item.Key} = DateTime.Now;");
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
                if (item.Key == "modify_user" || item.Key == "ModifyUser")
                {
                    paras.Add($"                data.{item.Key} = _loginService.GetUserName();");
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

        public async Task<IActionResult> Index()
        {{
            try
            {{
                TempData[""pageNum""] = 1;
                TempData[""pageMax""] = 1;
                var data = await _context.{tableName}.ToListAsync();
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