#include "TransformSystem.h"
#include <iostream>

TransformSystem* TransformSystem::instance = nullptr;

TransformSystem::TransformSystem()
{
	if (instance != nullptr)
	{
		std::cout << "Tried to create a transform system even though there already exists one" << std::endl;
		return;
	}
	instance = this;
}

void TransformSystem::Update(float deltaTime)
{

}
