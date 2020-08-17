using System;
using System.Web.UI;
using FineUIPro;
using App.Controls;
using System.Text.RegularExpressions;
using App.DAL;
using App.Components;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestRegex : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.tbText.Text = @"03/01/2019";
                this.tbRegex.Text = @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b";
                this.tbReplace.Text = @"${year}-${month}-${day}";
            }
        }

        // 匹配
        protected void btnMatch_Click(object sender, EventArgs e)
        {
            tbMatchResult.Text = "";
            var raw = UI.GetText(tbText);
            var matchRegex = UI.GetText(tbRegex);
            var regex = new Regex(matchRegex, RegexOptions.IgnoreCase);
            foreach (Match m in regex.Matches(raw))
            {
                //var value = m.Result("$1");
                foreach (Group g in m.Groups)
                {
                    var value = g.Value;
                    tbMatchResult.Text +=  value + "\r\n";
                }
            }
        }

        // 替换
        protected void btnReplace_Click(object sender, EventArgs e)
        {
            var raw = UI.GetText(tbText);
            var matchRegex = UI.GetText(tbRegex);
            var replaceRegex = UI.GetText(tbReplace);
            this.tbReplaceResult.Text = Regex.Replace(raw, matchRegex, replaceRegex);
        }
    }
}
