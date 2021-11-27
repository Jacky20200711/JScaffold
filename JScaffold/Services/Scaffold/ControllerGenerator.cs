namespace JScaffold.Services.Scaffold
{
    public class ControllerGenerator
    {
        public string GenerateCode(string projectName, string controllerName, string contextName, string tableName)
        {
            return $@"using {projectName}.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace {projectName}.Controllers
{{
    public class {controllerName}Controller : Controller
    {{
        private readonly ILogger<{controllerName}Controller> _logger;
        private readonly {contextName} _context;
        private readonly HttpContext? _httpContext;

        public {controllerName}Controller({contextName} context,
            ILogger<{controllerName}Controller> logger,
            IHttpContextAccessor httpContextAccessor)
        {{
            _context = context;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }}

        public async Task<IActionResult> Index()
        {{
            try
            {{
                var data = await _context.{tableName}.ToListAsync();
                return View(data);
            }}
            catch (Exception)
            {{
                _logger.LogError($""取得 {controllerName} 失敗"");
                return View(""~/Views/Shared/ErrorPage.cshtml"");
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
                // 提取前端傳來的參數
                string field1 = PostData[""field1""].ToString().Trim();
                string field2 = PostData[""field2""].ToString().Trim();
                
                // 創建一筆資料
                {controllerName} newData = new()
                {{
                }};
                
                // 更新DB
                _context.Add(newData);
                await _context.SaveChangesAsync();
                
                TempData[""message""] = ""新增成功"";
                return RedirectToAction(""Index"");
            }}
            catch (Exception)
            {{
                _logger.LogError($""新增 {controllerName} 失敗"");
                return View(""~/Views/Shared/ErrorPage.cshtml"");
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
                    return ""刪除失敗，查無這筆資料!"";
                }}

                var data = _context.{tableName}.FirstOrDefault(u => u.Id == id);

                if (data == null)
                {{
                    return ""刪除失敗，查無這筆資料!"";
                }}

                #endregion

                // 更新DB
                _context.Remove(data);
                await _context.SaveChangesAsync();

                return ""刪除成功"";
            }}
            catch (Exception)
            {{
                _logger.LogError($""刪除 {controllerName} 失敗"");
                return ""刪除失敗，系統忙碌中"";
            }}
        }}

        public async Task<IActionResult> Edit(int? id)
        {{
            #region 檢查資料庫是否有這筆資料

            if (id == null)
            {{
                return NotFound();
            }}

            var data = await _context.{tableName}.FindAsync(id);

            if (data == null)
            {{
                return NotFound();
            }}

            #endregion

            return View(data);
        }}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormCollection PostData)
        {{
            try
            {{
                // 提取前端傳來的參數
                string field1 = PostData[""field1""].ToString().Trim();
                string field2 = PostData[""field2""].ToString().Trim();
                int id = Convert.ToInt32(PostData[""id""].ToString());

                // 撈取目標資料
                var data = await _context.{tableName}.FindAsync(id);
                if (data == null)
                {{
                    TempData[""message""] = ""修改失敗，此筆資料不存在"";
                    return RedirectToAction(""Index"");
                }}

                // 修改目標資料並更新DB
                //data.Field1 = field1;
                //data.Field2 = field2;
                await _context.SaveChangesAsync();

                TempData[""message""] = ""修改成功"";
                return RedirectToAction(""Index"");
            }}
            catch (Exception)
            {{
                _logger.LogError($""修改 {controllerName} 失敗"");
                return View(""~/Views/Shared/ErrorPage.cshtml"");
            }}
        }}
    }}
}}";
        }
    }
}