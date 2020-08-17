<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestChunkUp.aspx.cs"  Inherits="App.Tests.TestChunkUp" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script>
        function setVideoUrl() {
            var v = document.getElementById("video");
            v.src = "/Files/Videos/About.mp4?t=" + Math.random();
        }
    </script>
</head>
<body onload="setVideoUrl()">
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Form2" runat="server" />
        <f:Form ID="Form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" ID="toolbar" >
                    <Items>
                        <f:Button runat="server" ID="btnUpload" OnClick="btnUpload_Click" Icon="FolderUp" Text="上传" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:ContentPanel runat="server" ShowHeader="false" ShowBorder="false">
                    <video controls="controls" muted="muted" id="video"  >
                    您的浏览器不支持 video 标签。
                    </video>
                </f:ContentPanel>
            </Items>
        </f:Form>
        <f:Window runat="server" ID="win" EnableIFrame="true" Width="500" Height="200" Title="上传" Hidden="true" CloseAction="HideRefresh"   />
    </form>
    </body>
</html>
