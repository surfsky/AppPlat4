
-------------------------------------
���ͷ���
-------------------------------------
�¶�����ʾ���ͷ����������ӣ�
	˼·
		/�ͻ�����ajax ���޽����ķ�ʽ���ֳ�����
		������Event (Type, Value, Bin, Sended, CreateDt, SendDt)
		��Ҫ����ʱ����������ݣ��綩��������������������ȣ�
		�����첽ҳ�棬��ѯEvent��������Ƿ��͸���ǰ�ͻ��ˣ�ע��˼·���ˣ�����ʵ��ͬ��ҳ���߼���
		/�ͻ��˸��� Type+Value ����Ӧ�Ĳ���
	�ο� goEasy �� Channel �ķ�ʽʵ�֣�����Ӧ��ͬ�Ŀͻ��ˡ�����ͻ��ˡ���ͬsessionId�����
		Server
			goEasy.publish("my_channel", "Hello, GoEasy!");
		Client
			var goEasy = new GoEasy({
				appkey: "my_appkey"
				onConnected: function () {alert("�ɹ�����GoEasy��");},
				onDisconnected: function () {alert("��GoEasy���ӶϿ���");}��
				onConnectFailed: function (error) {alert("��GoEasy����ʧ�ܣ�������룺"+error.code+"������Ϣ��"+error.content);}
			});
			goEasy.publish({
				channel: "my_channel",
				message: "Hello, GoEasy!",
				onSuccess:function(){alert("��Ϣ�����ɹ���");},
				onFailed: function (error) {alert("��Ϣ����ʧ�ܣ�������룺"+error.code+" ������Ϣ��"+error.content);}
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
	���շ������ɹ���
		Server
			�� /Pages/Comet.ashx �����û�������������������һֱ����
			����������Ҫ����ʱ������ CometMgr.Send(...) ��������Ӧ�ͻ�������
		�ͻ���
			��ajax������ѯ /Pages/Comet.ashx?channel=xxx����������ʾ
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
					if (t == "Online")     lblInfo.html("���ߣ�" + v);
					else if (t == "Order") lblInfo.html("�¶�����" + v.ID);
					else if (t == "News")  lblInfo.html("����Ϣ��" + v.ID);
					else lblInfo.html(t + " " + v);
				})
				.always(function (data) {
					console.info(data.statusText);
					longConnect();
				});
			}
		����
			�򿪣�http://localhost:5625//Admins/Main.aspx   ��ҳ�����˳���������
			�򿪣�http://localhost:5625/HttpApi/Mall/InsuranceOrderCreate?assetId=1&productSpecId=5&ShopId=1  �����Ӵ�����һ������
			�鿴 Main.aspx ҳ��ײ����¶����Ż���ʾ


���·���
	�� SignalR ��������ʵ��
	�ÿ�ܻ��Զ�ʵ��Web�ͻ�����������˵����ӣ������ӡ�websocket�ȣ�
	˫����Ϣ���Կɿ�����Ĵ��ݸ��Է�
