#pragma once
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
	char* MessageId;
	MessageContent Content;
	std::time_t Time;
};

struct MessageMetaData
{
	char* MessageId;
	std::time_t Time;
};

struct RequestActiveUsersResult
{
	messenger::operation_result::Type RequestResult;
	User* UserList;
	int Length;
};


typedef void(*LoginCallback)(messenger::operation_result::Type);

typedef void(*MessageStatusChanged)(/*const messenger::MessageId& msgId, messenger::message_status::Type status*/);

typedef void(*MessageReceived)(const char* sender, Message msg);

typedef void(*RequestUsersCallback)(User* users, int length, messenger::operation_result::Type result);
