
-------------------------------------
推送方案
-------------------------------------
新订单提示推送方案（长连接）
	思路
		/客户端用ajax 无限接续的方式保持长连接
		建立表：Event (Type, Value, Bin, Sended, CreateDt, SendDt)
		需要发送时往里面插数据（如订单创建、在线人数变更等）
		创建异步页面，查询Event表，看情况是否发送给当前客户端（注：思路错了，这其实是同步页面逻辑）
		/客户端根据 Type+Value 做对应的操作
	参考 goEasy 用 Channel 的方式实现，以适应不同的客户端、多个客户端、不同sessionId的情况
		Server
			goEasy.publish("my_channel", "Hello, GoEasy!");
		Client
			var goEasy = new GoEasy({
				appkey: "my_appkey"
				onConnected: function () {alert("成功连接GoEasy。");},
				onDisconnected: function () {alert("与GoEasy连接断开。");}，
				onConnectFailed: function (error) {alert("与GoEasy连接失败，错误编码："+error.code+"错误信息："+error.content);}
			});
			goEasy.publish({
				channel: "my_channel",
				message: "Hello, GoEasy!",
				onSuccess:function(){alert("消息发布成功。");},
				onFailed: function (error) {alert("消息发送失败，错误编码："+error.code+" 错误信息："+error.content);}
			});
			goEasy.subscribe({
				channel: "my_channel",
				onMessage: function (message) {
					alert("Channel:" + message.channel + " content:" + message.content);
				}
			});
			goEasy. unsubscribe ({
				channel: "my_channel"
			});
	最终方案（成功）
		Server
			用 /Pages/Comet.ashx 接受用户长连接请求，这个请求会一直挂起
			有新数据需要发送时，调用 CometMgr.Send(...) 方法，响应客户端请求
		客户端
			用ajax无限轮询 /Pages/Comet.ashx?channel=xxx，并解析显示
			function longConnect() {
				var lblInfo = $('#<%=lblInfo.ClientID%>');
				$.ajax({
					url: "/Pages/Comets.ashx",
					timeout: 1000*60*5
				})
				.done(function (data) {
					console.info(data);
					var t = data.Type;
					var v = data.Value;
					if (t == "Online")     lblInfo.html("在线：" + v);
					else if (t == "Order") lblInfo.html("新订单：" + v.ID);
					else if (t == "News")  lblInfo.html("新消息：" + v.ID);
					else lblInfo.html(t + " " + v);
				})
				.always(function (data) {
					console.info(data.statusText);
					longConnect();
				});
			}
		测试
			打开：http://localhost:5625//Admins/Main.aspx   该页面做了长连接请求
			打开：http://localhost:5625/HttpApi/Mall/InsuranceOrderCreate?assetId=1&productSpecId=5&ShopId=1  该连接创建了一条订单
			查看 Main.aspx 页面底部，新订单号会显示


最新方案
	用 SignalR 可以轻松实现
	该框架会自动实现Web客户端与服务器端的连接（长连接、websocket等）
	双方消息可以可靠无误的传递给对方
