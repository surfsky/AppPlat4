﻿using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Components
{
    /// <summary>访问鉴权</summary>
    /// <example>
    /// [Auth(Power.UserView, Power.UserEdit, Power.UserNew, Power.UserDelete)]
    /// [Auth(AuthLogin=true, AuthSign=true)]
    /// public class UserPage : Page {...}
    /// </example>
    public class AuthAttribute : Attribute
    {
        /// <summary>查看权限</summary>
        public Powers? ViewPower { get; set; }
        /// <summary>新建权限</summary>
        public Powers? NewPower { get; set; }
        /// <summary>编辑权限</summary>
        public Powers? EditPower { get; set; }
        /// <summary>删除权限</summary>
        public Powers? DeletePower { get; set; }

        /// <summary>校验登陆</summary>
        public bool AuthLogin { get; set; } = false;
        /// <summary>校验URL签名</summary>
        public bool AuthSign { get; set; } = false;

        /// <summary>是否忽略安全检测</summary>
        public bool Ignore { get; set; } = false;

        /// <summary>是否安全（有查看、登录、签名鉴权；或忽略）</summary>
        public bool IsSafe { get; set; }

        public bool CheckSafe()
        {
            this.IsSafe = ViewPower != null || AuthLogin == true || AuthSign == true || Ignore == true;
            return IsSafe;
        }

        // 构造方法（注：Attribute 构造函数不支持可空类型，只能多写几个构造方法了）
        public AuthAttribute() { }
        public AuthAttribute(bool isSafe) { this.IsSafe = isSafe; }
        public AuthAttribute(Powers viewPower)
        {
            this.ViewPower = viewPower;
            this.NewPower = viewPower;
            this.EditPower = viewPower;
            this.DeletePower = viewPower;
        }
        public AuthAttribute(Powers viewPower, Powers newPower, Powers editPower, Powers deletePower)
        {
            this.ViewPower = viewPower;
            this.NewPower = newPower;
            this.EditPower = editPower;
            this.DeletePower = deletePower;
        }
    }


}
