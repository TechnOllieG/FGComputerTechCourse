#pragma once
#include "SDL.h"
#include "Entity.h"
#include <vector>
#include <map>

class System;

class Engine
{
public:
	Engine(SDL_Renderer* renderer);
	void Run();
	void RegisterSystems(std::vector<System>& systems);
	int CreateEntity(Entity entity);
	bool DestroyEntity(int id);

	static Engine* instance;

private:
	void AddNewEntities(std::vector<System>& systems);
	void RemoveEntites(std::vector<System>& systems);
	std::map<int, Entity> currentEntities;

	std::vector<Entity> newEntities;
	std::vector<int> entitiesToRemove;
	std::vector<int> returnedIds;

	int currentFreeId = 0;
	SDL_Renderer* renderer = nullptr;
};