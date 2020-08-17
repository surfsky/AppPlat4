using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Components;
using App.Entities;

namespace App.DAL
{
    [UI("基础", "组织")]
    [Auth(Powers.OrgView, Powers.OrgNew, Powers.OrgEdit, Powers.OrgDelete)]
    public class Org : EntityBase<Org>, IDeleteLogic
    {
        [UI("名称")]               public string Name { get; set; }
        [UI("城市")]               public string City { get; set; }
        [UI("地址")]               public string Addr { get; set; }
        [UI("组织机构代码证编号")] public string CertNo { get; set; }
        [UI("组织机构代码证图片")] public string CertPic { get; set; }
        [UI("营业范围")]           public string BizScope { get; set; }
        [UI("法人")]               public string LegalPerson { get; set; }
        [UI("法人电话")]           public string LegalPersonTel { get; set; }
        [UI("法人身份证编号")]     public string LegalPersonIDCardNo { get; set; }
        [UI("法人身份证图片")]     public string LegalPersonIDCardPic { get; set; }
        [UI("在用")]               public bool? InUsed { get;set;} = true;
        [UI("是否审核通过")]       public bool? Approved { get; set; }

        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                ID,
                Name,
                City,
                Addr,
                CertNo,
                CertPic,
                BizScope,
                LegalPerson,
                LegalPersonTel,
                LegalPersonIDCardNo,
                LegalPersonIDCardPic,
                Approved
            };
        }


        [Param("name", "组织名称")]
        [Param("certNo", "组织机构代码证号码")]
        public static IQueryable<Org> Search(string name, string certNo)
        {
            IQueryable<Org> q = ValidSet;
            if (name.IsNotEmpty())     q = q.Where(t => t.Name.Contains(name));
            if (certNo.IsNotEmpty())   q = q.Where(t => t.CertNo.Contains(certNo));
            return q;
        }

    }
}
