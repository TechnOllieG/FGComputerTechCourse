#pragma once
#include "../Components/Component.h"
#include "../Engine.h"
#include <map>

class System
{
public:
	std::map<int, Component> components;

	virtual void Update(float deltaTime) {}
};