using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using App.Components;
using App.Controls;
using App.Utils;
using App.DAL;
using FineUIPro;

namespace App.Pages
{
    /// <summary>
    /// 通用数据管理页面
    ///     [x] 显示
    ///     [x] 排序
    ///     [x] 分页
    ///     [x] URL安全控制
    ///     [x] 操作权限控制
    ///     [x] 导出
    ///     [x] 查找
    /// 请用 Common.GetDatasUrl() 获取调用 url
    /// </summary>
    [UI("通用数据管理页面")]
    [Auth(AuthLogin=true)]
    [Param("tp", "实体类名")]
    //[Param("q", "查询字典（如a=x&b=x）")]
    [Param("pv", "访问权限")]
    [Param("pn", "新建权限")]
    [Param("pe", "编辑权限")]
    [Param("pd", "删除权限")]
    public partial class Datas : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            // 解析类型名
            var typeName = Asp.GetQueryString("tp", false);
            var type = Reflector.GetType(typeName);
            if (type == null)
            {
                Asp.Fail("未找到指定的类");
                return;
            }

            //
            Grid1.SetUI(type).SetAutoUrls().Build();
            if (!IsPostBack)
            {
                Grid1.SetSortPage(SiteConfig.Instance.PageSize);
                Grid1.BindGrid();
            }
        }
    }
}