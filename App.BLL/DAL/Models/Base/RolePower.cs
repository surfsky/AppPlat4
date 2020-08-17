using App.Utils;
using App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL
{
    /// <summary>
    /// 角色拥有的权限
    /// </summary>
    [UI("系统", "角色拥有的权限")]
    public class RolePower : EntityBase<RolePower>
    {
        public long RoleID { get; set; }
        public Powers PowerID { get; set; }
    }
}
