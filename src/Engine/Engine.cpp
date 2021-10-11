#include "Engine.h"
#include "Systems/System.h"
#include "Systems/TransformSystem.h"
#include "Components/Component.h"
#include "Entity.h"
#include <iostream>

Engine* Engine::instance = nullptr;

Engine::Engine(SDL_Renderer* renderer)
{
	if (instance != nullptr)
	{
		std::cout << "Tried to create the engine even though there already exists one" << std::endl;
		return;
	}
	instance = this;
}

void Engine::Run()
{
	this->renderer = renderer;

	std::vector<System> systems;

	RegisterSystems(systems);

	bool applicationRunning = true;
	float oldTime = SDL_GetTicks();

	while (applicationRunning)
	{
		float time = SDL_GetTicks();
		float deltaTime = time - oldTime;
		oldTime = time;

		SDL_Event event;
		while (SDL_PollEvent(&event))
		{
			if (event.type == SDL_EventType::SDL_KEYDOWN && event.key.keysym.sym == SDLK_ESCAPE || event.type == SDL_QUIT)
			{
				applicationRunning = false;
				break;
			}
		}

		for (System& system : systems)
		{
			system.Update(deltaTime);
		}

		RemoveEntites(systems);
		AddNewEntities(systems);

		SDL_RenderPresent(renderer);
	}
}

void Engine::RegisterSystems(std::vector<System>& systems)
{
	systems.push_back(TransformSystem());
}

int Engine::CreateEntity(Entity entity)
{
	if (returnedIds.size() > 0)
	{
		entity.id = returnedIds.back();
		returnedIds.pop_back();
	}
	else
	{
		entity.id = currentFreeId++;
	}

	newEntities.push_back(entity);
	return entity.id;
}

bool Engine::DestroyEntity(int id)
{
	if (currentEntities.count(id) < 1)
		return false;
	
	entitiesToRemove.push_back(id);
	return true;
}

void Engine::AddNewEntities(std::vector<System>& systems)
{
	for (Entity& entity : newEntities)
	{
		int systemSize = entity.systems.size();
		if (systemSize != entity.components.size())
		{
			std::cout << "Size of system array does not match size of component array in new entity" << std::endl;
			continue;
		}

		for (int i = 0; i < systemSize; i++)
		{
			entity.systems[i]->components.emplace(entity.id, entity.components[i]);
		}

		currentEntities.emplace(entity.id, entity);
	}

	newEntities.clear();
}

void Engine::RemoveEntites(std::vector<System>& systems)
{
	for (int id : entitiesToRemove)
	{
		for (int i = 0; i < currentEntities[id].systems.size(); i++)
		{
			currentEntities[id].systems[i]->components.erase(id);
		}
		currentEntities.erase(id);
		returnedIds.push_back(id);
	}

	entitiesToRemove.clear();
}
