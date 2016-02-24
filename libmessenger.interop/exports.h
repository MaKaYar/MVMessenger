#pragma once
#include "stdafx.h"
#include "callback.interop.h"

extern "C" {

	void __declspec(dllexport) Init();

	void __declspec(dllexport) Disconnect();

	void __declspec(dllexport) Login(LoginCallback loginCallback, char* login, char* pass);

	void __declspec(dllexport) RequestActiveUsersWrapper(RequestUsersCallback requestUsersCalback);

	Message __declspec(dllexport)* SendMessageWrapper(const char* recepientId, MessageContent messageContent);

	void __declspec(dllexport) RegisterObserver(MessageReceived messageRecieved, MessageStatusChanged messageStatusChanged);
}

static	std::shared_ptr<messenger::IMessenger> g_messenger;
static messenger::SecurityPolicy sp;
static CallbackProxy g_loginCallbackProxy;
static RequestUsersCalbackProxy g_requestUsersCalbackProxy;
static ObserverProxy g_observerProxy;