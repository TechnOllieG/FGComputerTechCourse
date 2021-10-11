#pragma once
#include "System.h"

class TransformSystem : public System
{
public:
	TransformSystem();
	static TransformSystem* instance;
	virtual void Update(float deltaTime) override;
};