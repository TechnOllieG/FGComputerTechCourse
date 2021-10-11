#pragma once
#include <vector>
#include "Components/Component.h"

class System;

struct Entity
{
	std::vector<System*> systems;
	std::vector<Component> components;
	int id = -1;

	void AddComponent(System* system, Component component) { systems.push_back(system); components.push_back(component); }
};