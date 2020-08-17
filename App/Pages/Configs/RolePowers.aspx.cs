using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;
using AspNet = System.Web.UI.WebControls;
using App.DAL;
using App.Utils;
using System.Collections;
//using EntityFramework.Extensions;
using App.Components;
using App.Controls;

namespace App.Admins
{
    /// <summary>
    /// 角色权限管理。
    /// - 无授权再管理功能（如：A授权B，B授权C，不予实现）。
    /// - 全选功能在服务器端无法实现，已改为在客户端实现（2017-08 SURFSKY）
    /// - 角色清单和权限清单来自枚举，不再从数据库中取（2017-11 SURFSKY)
    /// </summary>
    [UI("角色权限管理")]
    [Auth(Powers.RolePowerEdit)]
    public partial class RolePowers : PageBase
    {
        private Dictionary<Powers, bool> _powers = new Dictionary<Powers, bool>();


        //---------------------------------------------------
        // Init
        //---------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                this.btnRole.OnClientClick += Window1.GetShowReference("Roles.aspx", "角色管理", 800, 600);
            }
        }

        private void LoadData()
        {
            Grid1.PageSize = SiteConfig.Instance.PageSize;
            BindGrid1();
            Grid1.SelectedRowIndex = 0;
            Grid2.PageSize = SiteConfig.Instance.PageSize;
            BindGrid2();
        }

        private void BindGrid1()
        {
            Grid1.DataSource = GetAllowedRoles();
            Grid1.DataBind();
        }

        /// <summary>获取自己授权控制的角色</summary>
        static List<Role> GetAllowedRoles()
        {
            if (Common.LoginUser.Name == "admin")
                return Role.All;
            var roles = Common.LoginUser.RoleIds;
            return Role.All.Search(t => roles.Contains(t.ID));
        }


        private void BindGrid2()
        {
            var roleId = Grid1.GetSelectedId();
            if (roleId == -1)
            {
                Grid2.DataSource = null;
                Grid2.DataBind();
            }
            else
            {
                // 当前角色拥有的权限
                long role = (long)(roleId);
                _powers.Clear();
                foreach (var item in RolePower.Set.Where(t => t.RoleID == role))
                {
                    if (!_powers.ContainsKey(item.PowerID))
                        _powers.Add(item.PowerID, true);
                }

                // 权限分组展示
                Grid2.DataSource = typeof(Powers).GetEnumGroups().Select(t => new { Group = t }).ToList();
                Grid2.DataBind();
            }
        }


        //---------------------------------------------------
        // Grid1（角色表格）
        //---------------------------------------------------
        protected void Grid1_RowClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            BindGrid2();
        }

        protected void btnEditRole_Click(object sender, EventArgs e)
        {
            this.Window1.Title = "编辑角色列表";
            this.Window1.IFrameUrl = "Roles.aspx";
            this.Window1.Hidden = false;
        }

        protected void Window1_Close(object sender, EventArgs e)
        {
            LoadData();
        }


        //---------------------------------------------------
        // Grid2（权限表格）
        //---------------------------------------------------
        // 编辑权限列表
        protected void btnEditPower_Click(object sender, EventArgs e)
        {
            this.Window1.Title = "编辑权限列表";
            this.Window1.IFrameUrl = "Powers.aspx";
            this.Window1.Hidden = false;
        }

        // 用CheckBoxList展现权限列表
        protected void Grid2_RowDataBound(object sender, FineUIPro.GridRowEventArgs e)
        {
            AspNet.CheckBoxList cbl = (AspNet.CheckBoxList)Grid2.Rows[e.RowIndex].FindControl("ddlPowers");
            string group = e.DataItem.GetValue("Group").ToString();
            var items = typeof(Powers).GetEnumInfos(group);
            foreach (var power in items)
            {
                AspNet.ListItem item = new AspNet.ListItem();
                item.Value = power.ID.ToString();
                item.Text = power.Title;
                item.Attributes["data-qtip"] = power.Title;
                item.Selected = _powers.ContainsKey((Powers)(power.Value));
                cbl.Items.Add(item);
            }
        }

        // 保存权限
        protected void btnGroupUpdate_Click(object sender, EventArgs e)
        {
            // 角色
            var roleId = Grid1.GetSelectedId();
            if (roleId == -1)
                return;

            // 新的权限列表
            var role = (long)roleId;
            var powers = new List<Powers>();
            for (int i = 0; i < Grid2.Rows.Count; i++)
            {
                AspNet.CheckBoxList ddlPowers = (AspNet.CheckBoxList)Grid2.Rows[i].FindControl("ddlPowers");
                foreach (AspNet.ListItem item in ddlPowers.Items)
                    if (item.Selected)
                    {
                        Powers power = (Powers)(Convert.ToInt32(item.Value));
                        if (!powers.Contains(power))
                            powers.Add(power);
                    }
            }

            // 更新权限信息
            DAL.User.SetRolePowers(role, powers);
            this.lblInfo.Text = string.Format("已保存 {0:HH:mm:ss}", DateTime.Now);
        }


    }
}
