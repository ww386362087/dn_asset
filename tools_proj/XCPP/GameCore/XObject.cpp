#include "XObject.h"


XObject::XObject()
{

}

XObject::~XObject()
{

}

bool XObject::Initilize()
{
	return true;
}

void XObject::Uninitilize()
{

}

void XObject::OnCreated()
{

}


void XObject::OnEnterScene()
{

}


void XObject::OnSceneReady()
{

}

void XObject::OnLeaveScene()
{

}


void XObject::Unload()
{

	delete this;
}

void XObject::EventSubscribe()
{

}

void XObject::RegisterEvent(XEventDefine eventID, XEventHandler handler)
{

}

bool XObject::DispatchEvent(XEventArgs* e)
{
	return true;
}