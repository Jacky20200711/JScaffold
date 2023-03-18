using System.Collections.Generic;

namespace JScaffold.Services.Scaffold
{
    public class ControllerGenerator
    {
        public string GenerateCode(string projectName, string className, string contextName, string tableName, Dictionary<string, string> variables, string controllerName)
        {
            List<string> paras = new List<string>();

            string idName = "id";
            if (variables.ContainsKey("ID")) idName = "ID";
            if (variables.ContainsKey("Id")) idName = "Id";

            #region 設定新增資料的欄位
            paras.Clear();
            foreach (var item in variables)
            {
                // 忽略在新增時不會去異動的欄位
                if (item.Key.ToLower() == "id") continue;
                if (item.Key == "modify_user") continue;
                if (item.Key == "modify_date") continue;
                if (item.Key == "ModifyUser") continue;
                if (item.Key == "ModifyDate") continue;

                // 若是常見的特定欄位則額外處理
                if (item.Key == "create_user" || item.Key == "CreateUser")
                {
                    paras.Add($"                data.{item.Key} = _utility.GetLoginAccount(HttpContext);");
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
            }
            string paraAssign_create = string.Join("\n", paras);
            #endregion

            #region 設定修改資料的欄位
            paras.Clear();
            foreach (var item in variables)
            {
                // 忽略在修改時不會去異動的欄位
                if (item.Key.ToLower() == "id") continue;
                if (item.Key == "create_user") continue;
                if (item.Key == "create_date") continue;
                if (item.Key == "CreateUser") continue;
                if (item.Key == "CreateDate") continue;

                // 若是常見的特定欄位則優先處理
                if (item.Key == "modify_user" || item.Key == "ModifyUser")
                {
                    paras.Add($"                data.{item.Key} = _utility.GetLoginAccount(HttpContext);");
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
                if (item.Key.ToLower() == "id") continue;
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
using {projectName}.Models.ActionFilter;
using {projectName}.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using NLog;

namespace {projectName}.Controllers
{{
    [TypeFilter(typeof(LoginValidationFilter))]
    public class {controllerName}Controller : Controller
    {{
        private readonly {contextName} _context;
        private readonly Utility _utility;

        public {controllerName}Controller({contextName} context, Utility utility)
        {{
            _context = context;
            _utility = utility;
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
                LogManager.GetLogger(""{controllerName}"").Error($""{{ex.Message}}\n{{ex.StackTrace}}"");
                TempData[""message""] = ""Error"";
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
                LogManager.GetLogger(""{controllerName}"").Error($""{{ex.Message}}\n{{ex.StackTrace}}"");
                TempData[""message""] = ""Error"";
            }}
            return RedirectToAction(""Index"");
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Result> Delete(int {idName})
        {{
            Result result = new Result
            {{
                Code = 0,
            }};

            try
            {{
                {className} data = new {className}() {{ {idName} = {idName} }};
                _context.Entry(data).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                TempData[""message""] = ""刪除成功"";
                result.Code = 1;
            }}
            catch (Exception ex)
            {{
                LogManager.GetLogger(""{controllerName}"").Error($""{{ex.Message}}\n{{ex.StackTrace}}"");
            }}
            return result;
        }}

        public async Task<IActionResult> Edit(int? {idName})
        {{
            try
            {{
                if ({idName} == null)
                {{
                    return NotFound();
                }}

                var data = await _context.{tableName}.FindAsync({idName});

                if(data == null)
                {{
                    return NotFound();
                }}

                return View(data);
            }}
            catch (Exception ex)
            {{
                LogManager.GetLogger(""{controllerName}"").Error($""{{ex.Message}}\n{{ex.StackTrace}}"");
                TempData[""message""] = ""Error"";
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
                LogManager.GetLogger(""{controllerName}"").Error($""{{ex.Message}}\n{{ex.StackTrace}}"");
                TempData[""message""] = ""Error"";
            }}
            return RedirectToAction(""Index"");
        }}
    }}
}}
";
        }
    }
}