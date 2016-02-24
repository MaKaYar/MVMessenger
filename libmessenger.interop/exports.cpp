// libmessenger.interop.cpp: определяет экспортированные функции для приложения DLL.
//

#include "stdafx.h"
#include "exports.h"

void __declspec(dllexport) Init(unsigned short port, const char* url)
{
	messenger::MessengerSettings settings;
	//settings.serverPort = 5222;
	settings.serverPort = port;
	//settings.serverUrl = "137.116.213.109";
	settings.serverUrl = url;
	g_messenger = messenger::GetMessengerInstance(settings);
}

void __declspec(dllexport) Disconnect()
{
	g_messenger->Disconnect();
}

void __declspec(dllexport) Login(LoginCallback loginCallback, char* login, char* pass) {
	std::string loginStr(login);
	std::string passwordStr(pass);
	g_loginCallbackProxy.loginCallback = loginCallback;
	g_messenger->Login(loginStr, passwordStr, sp, &g_loginCallbackProxy);
}

void __declspec(dllexport) RequestActiveUsersWrapper(RequestUsersCallback requestUsersCalback)
{
	g_requestUsersCalbackProxy.requestUsersCallback = requestUsersCalback;
	g_messenger->RequestActiveUsers(&g_requestUsersCalbackProxy);

}

Message __declspec(dllexport)* SendMessageWrapper(const char* recepientId, MessageContent messageContent)
{
	std::string recID = recepientId;
	messenger::MessageContent libMessageContent;
	libMessageContent.encrypted = messageContent.encrypted;
	libMessageContent.type = messageContent.Type;
	std::vector<unsigned char> msg(messageContent.Data.DataLength);
	for (int i = 0; i < messageContent.Data.DataLength; i++)
	{
		msg[i] = messageContent.Data.Data[i];
	}
	libMessageContent.data = msg;
	messenger::Message sentMessage = g_messenger->SendMessage(recID, libMessageContent);
	Message temp;
	temp.Time = sentMessage.time;
	std::string str = sentMessage.identifier;
	char * writable = new char[str.size() + 1];
	std::copy(str.begin(), str.end(), writable);
	writable[str.size()] = '\0';
	temp.MessageId = writable;
	temp.Content.encrypted = sentMessage.content.encrypted;
	temp.Content.Type = sentMessage.content.type;
	// I know, that the code below is not truly orthodox
	temp.Content = messageContent;
	/*
	MessageMetaData messageMetaData;
	messageMetaData.Time = sentMessage.time;
	std::string str = sentMessage.identifier;
	char * writable = new char[str.size() + 1];
	std::copy(str.begin(), str.end(), writable);
	writable[str.size()] = '\0';
	messageMetaData.MessageId = writable;*/
	return new Message(temp);
}

void __declspec(dllexport) RegisterObserver(MessageReceived messageRecieved, MessageStatusChanged messageStatusChanged)
{
	g_observerProxy.onMessageRecieved = messageRecieved;
	g_observerProxy.onMessageStatusChanged = messageStatusChanged;
	g_messenger->RegisterObserver(&g_observerProxy);
}