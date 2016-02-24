
#include "stdafx.h"

// ------------------------------
// CallbackProxy methods defenition
// ------------------------------------
void CallbackProxy::OnOperationResult(messenger::operation_result::Type result)
{
	if (loginCallback != NULL) loginCallback(result);
	loginCallback = NULL;
}
// ------------------------------------
// RequestUsersCalbackProxy methods defenition
// ------------------------------------
void RequestUsersCalbackProxy::OnOperationResult(messenger::operation_result::Type result, const messenger::UserList& users)
{
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
		requestUsersCallback(UserList, users.size(), result);
	}
	requestUsersCallback = NULL;
}

// ------------------------------------
// CallbackProxy methods defenition
// ------------------------------------
void ObserverProxy::OnMessageStatusChanged(const messenger::MessageId& msgId, messenger::message_status::Type status)
{
	if (onMessageStatusChanged != NULL) onMessageStatusChanged(/*msgId, status*/);
	onMessageStatusChanged = NULL;
}

void ObserverProxy::OnMessageReceived(const messenger::UserId& senderId, const messenger::Message& msg)
{
	if (onMessageRecieved != NULL)
	{
		Message recievedMessage;
		recievedMessage.Content.encrypted = msg.content.encrypted;
		recievedMessage.Content.Type = msg.content.type;
		recievedMessage.Time = msg.time;
		recievedMessage.Content.Data.Data = new unsigned char[msg.content.data.size()];
		std::copy(msg.content.data.begin(), msg.content.data.end(), recievedMessage.Content.Data.Data);

		recievedMessage.Content.Data.DataLength = msg.content.data.size();

		std::string str = msg.identifier;
		char * writable = new char[str.size() + 1];
		std::copy(str.begin(), str.end(), writable);
		writable[str.size()] = '\0';
		recievedMessage.MessageId = writable;
		onMessageRecieved(senderId.c_str(), recievedMessage);
	};
	//onMessageRecieved = NULL;
}

