using System;
using System.Web.UI;
using FineUIPro;
using App.Controls;
using System.Text.RegularExpressions;
using App.Utils;
using App.DAL;
using App.Components;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestEncode : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.tbText.Text = @"
{
  ""ToUserName"": ""<html><body>content&nbsp;</body></html>"",
  ""FromUserName"": ""\r\n\t\v"",
  ""CreateTime"": ""1556607041"",
  ""EventKey"": """",
  ""MsgType"": ""Event"",
  ""Event"": ""Subscribe"",
  ""CreateDt"": ""2019-04-30 14:50:41""
}
            ";
            }
        }



        //---------------------------------------------
        // 网页编码
        //---------------------------------------------
        protected void btnHtmlEncode_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            this.tbResult.Text = text.HtmlEncode();
        }

        protected void btnHtmlDecode_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            this.tbResult.Text = text.HtmlDecode();
        }

        protected void btnUrlEncode_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            this.tbResult.Text = text.UrlEncode();
        }

        protected void btnUrlDecode_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            this.tbResult.Text = text.UrlDecode();
        }

        //---------------------------------------------
        // 引号
        //---------------------------------------------
        protected void btnAddQuote_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            text = string.Format("\"{0}\"", text.Replace("\"", "\\\""));
            this.tbResult.Text = text;
        }

        protected void btnRemoveQuote_Click(object sender, EventArgs e)
        {
            var text = UI.GetText(tbText);
            text = text.TrimStart('"').TrimEnd('"').Replace("\\\"", "\"");
            this.tbResult.Text = text;
        }

        //---------------------------------------------
        // Json & Xml
        //---------------------------------------------
        protected void btnJson_Click(object sender, EventArgs e)
        {
            try
            {
                var text = UI.GetText(tbText);
                if (text.StartsWith("<"))
                    this.tbResult.Text = text.ParseXmlToJson();         // 识别xml并转化为json
                else
                    this.tbResult.Text = text.ParseJObject().ToJson();  // 识别json并美化
            }
            catch (Exception ex)
            {
                UI.ShowAlert(ex.Message);
            }
        }

        protected void btnXml_Click(object sender, EventArgs e)
        {
            try
            {
                var text = UI.GetText(tbText);
                if (text.StartsWith("<"))
                    this.tbResult.Text = text.ParseXml().ToString();    // 识别Xml并美化
                //else
                //    this.tbResult.Text = text.ParseJObject().ToXml();   // 识别Json并转化为xml（未完成）
            }
            catch (Exception ex)
            {
                UI.ShowAlert(ex.Message);
            }
        }

        //---------------------------------------------
        // 文本处理
        //---------------------------------------------
        protected void btnRemoveBlank_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).RemoveBlank();
        }

        protected void btnRemoveBlankTranslator_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).RemoveBlankTranslator();
        }

        protected void btnSlim_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).Slim();
        }

        protected void btnSummary_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).Summary(20);
        }

        protected void btnRemoveTag_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).RemoveTag();
        }

        protected void btnRemoveHtml_Click(object sender, EventArgs e)
        {
            this.tbResult.Text = UI.GetText(tbText).RemoveHtml();
        }
    }
}
