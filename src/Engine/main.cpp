#include <iostream>
#include <vector>
#include "SDL.h"
#include "Systems/TransformSystem.h"
#include "Engine.h"
#undef main

int main()
{
	if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
	{
		std::cout << "Failed to initialize SDL, error is: " << SDL_GetError() << std::endl;
		return -1;
	}

	SDL_Window* window = SDL_CreateWindow("TechnOllieGEngine", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, SDL_WINDOW_VULKAN);
	SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, 0);

	if (window == nullptr || renderer == nullptr)
	{
		std::cout << "Failed to create window/renderer, error is: " << SDL_GetError() << std::endl;
		return -1;
	}

	SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
	SDL_RenderClear(renderer);

	Engine engine(renderer);
	engine.Run();

	SDL_DestroyRenderer(renderer);
	SDL_DestroyWindow(window);
	SDL_Quit();

	return 0;
}