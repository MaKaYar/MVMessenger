// dllmain.cpp: определяет точку входа для приложения DLL.
#include "stdafx.h"
struct User
{
	char* identifier;
};

struct UserList
{
	
};

struct BinaryData
{
	unsigned char* Data;
	int DataLength;
};

struct MessageContent
{
	messenger::message_content_type::Type Type;
	bool encrypted;
	BinaryData Data;
};

struct Message
{
	User Recepient;
	MessageContent Content;
};

volatile struct RequestActiveUsersResult
{
	messenger::operation_result::Type RequestResult;
	User* UserList;
	int Length;
};
static	std::shared_ptr<messenger::IMessenger> g_messenger;
static messenger::SecurityPolicy sp;
typedef void(*LoginCallback)(messenger::operation_result::Type);
typedef void(*MessageStatusChanged)(/*const messenger::MessageId& msgId, messenger::message_status::Type status*/);
typedef void(*MessageReceived)(const char* sender,Message& msg);
typedef void(*RequestUsersCallback)(User* users, int length, messenger::operation_result::Type result);



//temp struct
class CallbackProxy : public messenger::ILoginCallback
{
public:
	LoginCallback loginCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result) override {
		if (loginCallback != NULL) loginCallback(result);
		loginCallback = NULL;
	}
};

class RequestUsersCalbackProxy : public messenger::IRequestUsersCallback
{
public:
	RequestUsersCallback requestUsersCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result, const messenger::UserList& users) override {
		if (requestUsersCallback != NULL)
		{
			//User *temp = new User[users.size()];
			/*RequestActiveUsersResult temp;
			temp.Length = users.size();
			temp.RequestResult = result;*/
			User* UserList = new User[users.size()];
			for (int i = 0; i < users.size(); i++)
			{
				std::string str = users[i].identifier;
				char * writable = new char[str.size() + 1];
				std::copy(str.begin(), str.end(), writable);
				writable[str.size()] = '\0';
				UserList[i].identifier = writable;
			}
			requestUsersCallback(UserList,users.size(),result);
		}
		requestUsersCallback = NULL;
	}
};
//class RequestUsersCalbackProxy : public messenger::IRequestUsersCallback
//{
//public:
//	RequestUsersCallback requestUsersCallback = NULL;
//private:
//	void OnOperationResult(messenger::operation_result::Type result, const messenger::UserList& users) override {
//		if (requestUsersCallback != NULL)
//		{
//			User *temp = new User[users.size()];
//			for (int i = 0; i < users.size(); i++)
//			{
//				std::string str = users[i].identifier;
//				char * writable = new char[str.size() + 1];
//				std::copy(str.begin(), str.end(), writable);
//				writable[str.size()] = '\0';
//				temp[i].identifier = writable;
//			}
//			requestUsersCallback(result, users, users.size());
//		}
//		requestUsersCallback = NULL;
//	}
//};



class ObserverProxy : public messenger::IMessagesObserver
{
public:
	MessageReceived onMessageRecieved = NULL;
	MessageStatusChanged onMessageStatusChanged = NULL;
private:
	void OnMessageStatusChanged(const messenger::MessageId& msgId, messenger::message_status::Type status) override {
		if (onMessageStatusChanged != NULL) onMessageStatusChanged(/*msgId, status*/);
		onMessageStatusChanged = NULL;
	}
	void OnMessageReceived(const messenger::UserId& senderId, const messenger::Message& msg) override {
		if (onMessageRecieved != NULL)
		{
			Message recievedMessage;
			recievedMessage.Content.encrypted = msg.content.encrypted;
			recievedMessage.Content.Type = msg.content.type;
			recievedMessage.Content.Data.Data = new unsigned char[msg.content.data.size()];
			std::copy(msg.content.data.begin(), msg.content.data.end(), recievedMessage.Content.Data.Data);
			
			recievedMessage.Content.Data.DataLength = msg.content.data.size();
			
			std::string str = msg.identifier;
			char * writable = new char[str.size() + 1];
			std::copy(str.begin(), str.end(), writable);
			writable[str.size()] = '\0';
			recievedMessage.Recepient.identifier = writable;
			onMessageRecieved(senderId.c_str(), recievedMessage);
		};
		onMessageRecieved = NULL;
	}
};

static CallbackProxy g_callbackProxy;
static RequestUsersCalbackProxy g_requestUsersCalbackProxy;
static ObserverProxy g_observerProxy;

extern "C" {
	

	void __declspec(dllexport) Init()
	{
		messenger::MessengerSettings settings;
		settings.serverPort = 5222;
		//settings.serverUrl = "137.116.213.109";
		settings.serverUrl = "137.116.213.109";
		g_messenger = messenger::GetMessengerInstance(settings);
	}

	void __declspec(dllexport) Disconnect()
	{
		g_messenger->Disconnect();
	}

	void __declspec(dllexport) Login(LoginCallback loginCallback, char* login, char* pass) {
		std::string loginStr(login);
		std::string passwordStr(pass);
		g_callbackProxy.loginCallback = loginCallback;
		g_messenger->Login(loginStr, passwordStr,sp,&g_callbackProxy);
	}

	void __declspec(dllexport) RequestActiveUsersWrapper(RequestUsersCallback requestUsersCalback)
	{
		g_requestUsersCalbackProxy.requestUsersCallback = requestUsersCalback;
		g_messenger->RequestActiveUsers(&g_requestUsersCalbackProxy);

	}

	void __declspec(dllexport) SendMessageWrapper(const char* recepientId, BinaryData* message)
	{
		std::string recID = recepientId;
		messenger::MessageContent messageContent;
		messageContent.encrypted = false;
		messageContent.type = messenger::message_content_type::Text;
		std::vector<unsigned char> msg(message->DataLength);
		for (int i = 0; i < message->DataLength; i++)
		{
			msg[i] = message->Data[i];
		}
		messageContent.data = msg;
		messenger::Message temp = g_messenger->SendMessage(recID, messageContent);
	}

	void __declspec(dllexport) RegisterObserver(MessageReceived messageRecieved, MessageStatusChanged messageStatusChanged)
	{
		g_observerProxy.onMessageRecieved = messageRecieved;
		g_observerProxy.onMessageStatusChanged = messageStatusChanged;
		g_messenger->RegisterObserver(&g_observerProxy);
	}


} 
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

