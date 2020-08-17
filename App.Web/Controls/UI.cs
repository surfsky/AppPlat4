using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using App.Components;
//using App.DAL;  // Power
using App.Utils;
using FineUIPro;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// UI 操作 (FineUI) 辅助类
    /// </summary>
    public static partial class UI
    {
        //-----------------------------------------
        // Render
        //-----------------------------------------
        /// <summary>生成 Juery 渲染脚本到客户端</summary>
        public static void Render(string clientId, string html, bool useFineUI = true)
        {
            var script = string.Format("$(function(){{ $('#{0}').html({1}); }});", clientId, html.Quote());
            if (useFineUI)
                PageContext.RegisterStartupScript(script);
            else
                (HttpContext.Current.Handler as Page).ClientScript.RegisterStartupScript(null, "News", script, true);
        }

        //-----------------------------------------
        // 控件可操作性控制
        //-----------------------------------------
        /// <summary>控件可编辑性控制</summary>
        public static void SetEditable(Field control, Powers power)
        {
            control.Readonly = !Common.LoginUser.HasPower(power);
        }

        /// <summary>控件可编辑性控制</summary>
        public static void SetEditable(bool editable, params Field[] controls)
        {
            foreach (var control in controls)
                control.Readonly = !editable;
        }

        /// <summary>控件可视性控制</summary>
        public static void SetVisible(bool visible, params ControlBase[] controls)
        {
            foreach (var control in controls)
                control.Hidden = !visible;
        }

        
        /// <summary>控件可视性控制</summary>
        public static void SetVisible(ControlBase control, Powers power)
        {
            control.Hidden = !Common.LoginUser.HasPower(power);
        }

        /// <summary>控件有用性控制</summary>
        public static void SetEnable(bool enable, params ControlBase[] controls)
        {
            foreach (var control in controls)
                control.Enabled = enable;
        }


        /// <summary>控件有用性控制</summary>
        public static void SetEnable(ControlBase control, Powers power)
        {
            control.Enabled = Common.LoginUser.HasPower(power);
        }


        /// <summary>根据QueryString键的值来设置控件的显示隐藏状态（默认显示，除非显式指定为false）</summary>
        /// <param name="queryKey">查询键名</param>
        /// <param name="controls">需要控制显隐的控件（如果本来状态就是hidden则不处理）</param>
        public static void SetVisibleByQuery(string queryKey, params ControlBase[] controls)
        {
            var visible = (Asp.GetQueryBool(queryKey) != false);
            foreach (var control in controls)
                if (!control.Hidden)
                    control.Hidden = !visible;
        }


        /// <summary>提示输入控件输入有误</summary>
        public static void SetInvalid(Field control, string msg)
        {
            control.MarkInvalid(msg);
        }

        //-----------------------------------------
        // 窗口控制
        //-----------------------------------------
        /// <summary>关闭窗口</summary>
        public static void HideWindow(CloseAction closeAction)
        {
            var script = ActiveWindow.GetHideReference();
            if (closeAction == CloseAction.HidePostBack)
                script = ActiveWindow.GetHidePostBackReference();
            if (closeAction == CloseAction.HideRefresh)
                script = ActiveWindow.GetHideRefreshReference();
            PageContext.RegisterStartupScript(script);
        }

        /// <summary>关闭窗口并触发脚本</summary>
        public static void HideWindow(string script)
        {
            var hideRunScript = ActiveWindow.GetHideExecuteScriptReference(script);
            PageContext.RegisterStartupScript(hideRunScript);
        }

        /// <summary>显示窗口</summary>
        public static void ShowWindow(Window win, 
            string url, string title, 
            int? width = null, int? height = null, 
            CloseAction? closeAction = null,
            Target? target = null
            )
        {
            if (width != null)       win.Width = width.Value;
            if (height != null)      win.Height = height.Value;
            if (closeAction != null) win.CloseAction = closeAction.Value;
            if (target != null)      win.Target = target.Value;
            var script = win.GetShowReference(url, title, win.Width, win.Height);
            PageContext.RegisterStartupScript(script);
        }

        /// <summary>尝试从面板中获取或创建工具栏控件</summary>
        public static Toolbar GetToolbar(PanelBase toolbarPanel)
        {
            if (toolbarPanel.Toolbars.Count > 0)
                return toolbarPanel.Toolbars[0];
            else
                return CreateToolbar(toolbarPanel, false);
        }

        /// <summary>创建工具栏</summary>
        public static Toolbar CreateToolbar(PanelBase toolbarPanel, bool insertOrAppend)
        {
            var toolbar = new Toolbar();
            if (insertOrAppend)
                toolbarPanel.Toolbars.Insert(0, toolbar);
            else
                toolbarPanel.Toolbars.Add(toolbar);
            return toolbar;
        }


        /// <summary>显隐控件</summary>
        public static void SetVisible(ControlBase ctrl, bool visible)
        {
            ctrl.Hidden = !visible;
        }

        //------------------------------------------------------
        // 对话框
        //------------------------------------------------------
        public static void ShowHud(string info, MessageBoxIcon icon=MessageBoxIcon.Information)
        {
            Notify notify = new Notify();
            notify.Message = info;
            notify.MessageBoxIcon = icon;
            notify.Target = Target.Self;
            notify.ShowHeader = false;
            notify.DisplayMilliseconds = 1000;
            notify.PositionX = Position.Center;
            notify.PositionY = Position.Top;
            notify.IsModal = false;
            notify.MessageAlign = TextAlign.Center;
            notify.Show();
        }

        /// <summary>显示警告框</summary>
        public static void ShowAlert(string info, MessageBoxIcon icon=MessageBoxIcon.Information)
        {
            PageContext.RegisterStartupScript(Alert.GetShowInTopReference(info, icon));
        }

        /// <summary>显示警告框</summary>
        public static void ShowAlert(string info, FineUIPro.Icon icon, string title="警告", Target target = Target.Top)
        {
            Alert alert = new Alert();
            alert.Message = info;
            alert.Icon = icon;
            alert.Target = Target.Top;
            alert.Title = title;
            alert.Show();
        }

        /// <summary>显示确认对话框（并指定处理控件）</summary>
        public static void ShowConfirm(string msg, string title, ControlBase responseCtrl)
        {
            var script = Confirm.GetShowReference(
                msg,
                title,
                MessageBoxIcon.Question,
                responseCtrl.GetPostBackEventReference(),
                String.Empty
                );
            PageContext.RegisterStartupScript(script);
        }

        /// <summary>显示确认对话框（并指定处理控件）</summary>
        public static void ShowConfirm(string msg, string title, string okScript)
        {
            Confirm.Show(msg, title, MessageBoxIcon.Question, okScript, "", Target.Top);
        }

        /// <summary>给页面管理器发送消息(PageManager1_CustomEvent)</summary>
        public static void PostToPageManager(string eventName)
        {
            var script = PageManager.Instance.GetCustomEventReference(eventName);
            PageContext.RegisterStartupScript(script);
        }

        /// <summary>获取回发脚本</summary>
        public static string GetPostReference(ControlBase ctrl, string eventArg="")
        {
            return ctrl.GetPostBackEventReference(eventArg);
        }

        /// <summary>显示权限错误警告框</summary>
        public static void ShowPowerFailAlert()
        {
            ShowAlert(Common.CHECK_POWER_FAIL_ACTION_MESSAGE);
        }

        /// <summary>给父页面postback刷新消息</summary>
        public static void PostParentToRefresh(string argument = "Refresh")
        {
            string script = string.Format("parent.__doPostBack('', '{0}');", argument);
            PageContext.RegisterStartupScript(script);
        }

        /// <summary>是否是PostBack刷新命令</summary>
        public static bool IsPostBackRefresh(Page page, string argument = "Refresh")
        {
            return page.IsPostBack && Asp.Request.Params["__EVENTARGUMENT"] == argument;
        }

        /// <summary>获取在标签页打开的脚本（未测试）</summary>
        public static string GetTabUrl(string tabId, string tabTitle, string url)
        {
            JsObjectBuilder joBuilder = new JsObjectBuilder();
            joBuilder.AddProperty("id", "grid_newtab_edit_" + tabId);
            joBuilder.AddProperty("url", url);
            joBuilder.AddProperty("text", tabTitle);
            joBuilder.AddProperty("icon", "page");
            joBuilder.AddProperty("refreshWhenExist", true);

            return String.Format("window.top.addMainTab({0});", joBuilder);
        }

        /// <summary>创建窗口</summary>
        public static Window CreateWindow(string title, int width, int height)
        {
            return new Window() 
            { 
                Title = title, Hidden = true, 
                EnableIFrame = true, EnableMaximize = true, EnableResize = true, 
                Target = Target.Top, IsModal = true, 
                Width = width, Height = height 
            };
        }


        //------------------------------------------------------
        // 图片上传相关
        //------------------------------------------------------
        /// <summary>显示图片推荐尺寸</summary>
        public static void SetText(FineUIPro.FileUpload uploader, Size? size)
        {
            uploader.ButtonText = string.Format("上传图片(建议尺寸{0}x{1})", size?.Width, size?.Height);
        }


        /// <summary>上传图片到指定目录</summary>
        /// <param name="fileUpload">文件上传控件</param>
        /// <param name="folderName">上传目录名，如Users</param>
        /// <returns>图片的虚拟路径</returns>
        public static string UploadFile(FineUIPro.FileUpload fileUpload, string folderName, Size? size = null)
        {
            if (!fileUpload.HasFile)
                return "";

            // 保存文件
            string fileName = fileUpload.ShortFileName;
            string virtualPath = Uploader.GetUploadPath(folderName, fileName);
            string physicalPath = SaveUploadFile(fileUpload, virtualPath);

            // 如果是图片的话调整尺寸
            if (IO.IsImageFile(fileName))
            {
                if (size != null && size.Value.Width !=0)
                    Painter.Thumbnail(physicalPath, physicalPath, size.Value.Width);
            }
            return virtualPath;
        }


        /// <summary>保存上传文件</summary>
        private static string SaveUploadFile(FileUpload fileUpload, string virtualPath)
        {
            string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);
            IO.PrepareDirectory(physicalPath);
            fileUpload.SaveAs(physicalPath);
            return physicalPath;
        }



        //------------------------------------------------------
        // 权限相关
        //------------------------------------------------------
        // 根据权限设置按钮状态
        public static void SetButtonByPower(FineUIPro.Button btn, Powers power)
        {
            if (!Common.CheckPower(power))
            {
                btn.Hidden = true;
            }
        }

        /// <summary>
        /// 在主窗口增加标签页。(未成功，需再调试）
        /// 注：addMainTab在Main.aspx中注册了
        /// </summary>
        /// <param name="id">标签页的ID，若再次调用，会跳到已经打开的标签页。</param>
        /// <param name="url">标签页URL</param>
        /// <param name="title">标题</param>
        /// <param name="icon">图标</param>
        /// <param name="refreshWhenExists">再次调用时是否刷新</param>
        public static void AddMainTab(string id, string url, string title, FineUIPro.Icon icon, bool refreshWhenExists = false)
        {
            string iconPath = GetIconUrl(icon);
            string script = string.Format("window.top.addMainTab('{0}', '{1}', '{2}', '{3}', {4})", id, url, title, iconPath, refreshWhenExists);
            FineUIPro.PageContext.RegisterStartupScript(script);
        }

    }
}