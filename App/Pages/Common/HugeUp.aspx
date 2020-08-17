<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HugeUp.aspx.cs" Inherits="App.HugeUp" %>
<%@ Import Namespace="App.Utils" %>
<%@ Import Namespace="App.Components" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script src="../../Res/js/jquery-3.3.1.min.js" ></script>
    <title></title>
    <style>
        .title  {font-size:12pt; display:inline-block; width: 100px;}
        .inline {display: inline;}
        .hide   {visibility:hidden;}
    </style>
</head>
<body>
    <div>
        <label id="info" for="file" style="border-radius:5px; background: #0a7e3b; color:white; padding:3px 15px 3px 15px">选择文件</label>
        <input type="file" id="file" style="visibility:hidden; width:0;" onchange="selectFile()"  accept="<%=Asp.GetQueryString("filter") %>" />
        <button id="btnUpload" onclick="uploadFile()" disabled="disabled" class="hide" >上传</button>
        <div id="result" style="display:block; margin-top:10px">
            <div class="title">状态</div><div id="status" class="inline"></div><br />
            <div class="title">进度</div><div id="progress" class="inline"></div><br />
            <div class="title">URL </div><div id="url" class="inline"></div><br />
            <div class="title">大小</div><div id="size" class="inline"></div><br />
        </div>
    </div>
    <form id="form1" runat="server">
    </form>
</body>

    <script type="text/javascript">
        const CHUNKBYTES = 1024 * 1024 * 5; // 切片大小(MB)
        var requestUrl = '<%="/HttpApi/App.HugeUp/DoRequest".ToSignUrl() %>';
        var chunkUrl = '<%="/HttpApi/App.HugeUp/DoChunk".ToSignUrl() %>';
        var mergeUrl = '<%="/HttpApi/App.HugeUp/DoMerge".ToSignUrl() %>';
        var saveUrl = '<%="/HttpApi/App.HugeUp/DoSave".ToSignUrl()  %>';
        var id = '<%=App.Utils.SnowflakeID.Instance.NewID().ToString() %>';
        var folder = '<%=Asp.GetQueryString("folder") %>';
        var title = '<%=Asp.GetQueryString("title") %>';
        var key = '<%=Asp.GetQueryString("key") %>';
        var callback = '<%=Asp.GetQueryString("callback") %>';
        var sendChunks;  // 已发送切片数

        // 选择文件
        function selectFile() {
            var ele = document.getElementById("file");
            var file = ele.files[0];
            var fileName = file.name;
            var fileSize = file.size;
            $("#info").html(fileName);
            if (title == '')
                title = fileName;
            // 去服务器端验证
            $.ajax({
                type: "POST",
                url: requestUrl,
                data: { fileName: fileName, fileSize: fileSize }
            }).always(function (json) {
                if (json.Result == true) {
                    //$("#btnUpload").attr("disabled", false);
                    id = json.Info;
                    showStatus("Ready");
                    uploadFile();
                }
                else {
                    showStatus(json.Info);
                    //$("#btnUpload").attr("disabled", true);
                }
            });
        }

        // 上传文件
        function uploadFile() {
            var ele = document.getElementById("file");
            if (ele.files.lengh == 0)
                return;

            // 循环发送切片
            var file = ele.files[0];
            var totalChunks = Math.ceil(file.size / CHUNKBYTES);  // 切片总数

            // 如果只有一个切块，直接上传
            if (totalChunks == 1) {
                showStatus("Sending");
                saveFile(file);
            }
            else {
                // 循环发送切片
                var start = 0;
                var seq = 0;
                sendChunks = 0;
                showStatus("Loop sending");
                while (start < file.size) {
                    var end = start + CHUNKBYTES;
                    if (end > file.size)
                        end = file.size;
                    uploadFileChunk(file, seq, start, end, totalChunks);
                    start = end;
                    seq++;
                }
            }
        }

        //--------------------------------------------
        // 上传文件
        //--------------------------------------------
        // 上传文件切块
        function uploadFileChunk(file, seq, start, end, totalChunks) {
            // 构造form数据
            var chunk = file.slice(start, end);//切割文件
            var fd = new FormData();
            fd.append("file", chunk);
            fd.append("id", id);
            fd.append("seq", seq);

            // 上传切块
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    sendChunks++;
                    if (xhr.responseText) {
                        showProgress(sendChunks, totalChunks);
                    }

                    // 如果所有文件切片都成功发送，发送文件合并请求。
                    if (sendChunks == totalChunks)
                        mergeFile(totalChunks);
                }
            };
            xhr.open("POST", chunkUrl, true);
            xhr.send(fd);
        }

        // 通知服务器合并文件
        function mergeFile(totalChunks) {
            showStatus("Processing");
            var fd = new FormData();
            fd.append("id", id);
            fd.append("total", totalChunks);
            fd.append("folder", folder);
            fd.append("key", key);
            fd.append("title", title);
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.responseText) {
                        showResponse(xhr.responseText);
                        showStatus("OK");

                        // 上传成功后，跳到回调地址
                        if (callback != '')
                            location.href = callback + "?status=success";
                    }
                }
            };
            xhr.open("POST", mergeUrl, true);
            xhr.send(fd);
        }

        // 直接上传文件
        function saveFile(file) {
            var fd = new FormData();
            fd.append("file", file);
            fd.append("id", id);
            fd.append("folder", folder);
            fd.append("key", key);
            fd.append("title", title);
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.responseText) {
                        showResponse(xhr.responseText);
                        showStatus("OK");
                        showProgress(1, 1);
                    }
                    // 上传成功后，跳到回调地址
                    if (callback != '')
                        location.href = callback + "?status=success";
                }
            };
            xhr.open("POST", saveUrl, true);
            xhr.send(fd);
        }



        // 显示状态
        function showStatus(status) {
            $("#status").html(status);
        }
        // 显示进度
        function showProgress(sendChunks, totalChunks) {
            if (sendChunks != null && totalChunks != null) {
                var percent = sendChunks * 1.0 / totalChunks;
                $("#progress").html(Math.floor(percent * 100).toString() + "%");
            }
        }
        // 显示服务器端响应
        function showResponse(info) {
            var o = $.parseJSON(info);
            $("#status").html(o.Info);
            $("#url").html(o.Data.Url);
            $("#size").html(o.Data.Size);
        }

    </script>
</html>
