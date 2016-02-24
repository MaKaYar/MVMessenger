#pragma once
#include "stdafx.h"

class CallbackProxy : public messenger::ILoginCallback
{
public:
	LoginCallback loginCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result) override;
};

class RequestUsersCalbackProxy : public messenger::IRequestUsersCallback
{
public:
	RequestUsersCallback requestUsersCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result, const messenger::UserList& users) override;
};

class ObserverProxy : public messenger::IMessagesObserver
{
public:
	MessageReceived onMessageRecieved = NULL;
	MessageStatusChanged onMessageStatusChanged = NULL;
private:
	void OnMessageStatusChanged(const messenger::MessageId& msgId, messenger::message_status::Type status) override;

	void OnMessageReceived(const messenger::UserId& senderId, const messenger::Message& msg) override;
};