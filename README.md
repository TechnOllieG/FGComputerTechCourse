# FGComputerTechCourse
A small minecraft clone made using Unity DOTS

# How to play
Run around with WASD and jump with space, press F3 to view coordiantes and chunk coordinates, press escape to change render distance, hold f3 and press n to eject the camera from the player and fly around, press f3+n again to snap back to the player.

# How I used DOD
A while back during the algorithms & data structures course I made my first Minecraft clone with procedural terrain generation that was made using perlin noise.
The problem with that implementation though was that it was object oriented and was super slow to generate, it also lagged a lot.

<strong>Performance</strong>

With the object oriented approach I got 6 fps at a render distance of 12 chunks (which is 480 000 cubes).
With the exact same amount of blocks but using this project I get around 20 fps.
If I decrease the render distance to 5 chunks (which is 92 928 cubes) my fps is locked at 60.
At 8 chunks I get around 55 fps with some lag spikes when generating new chunks.

<strong>Implementation</strong>

Each block is an entity that is converted from a prefab. It has a cube mesh (not unity's built in since I need a uv-map for the texture).
The material is using Unity's standard shader and the textures are the standard minecraft textures.

I have a system called TerrainGenerationSystem (inherits from SystemBase) that based on the players position and the render distance determines which chunks should be generated.
The system keeps track of all chunks that are generated and has a list of chunks to generate.
For each chunk to generate it spaWns all entities that will be used for that chunk (each chunk is 16x16 and each chunk has 1 layer of grass and 2 layers of dirt under that) (currently this happens in the main thread).
Then it runs a multi-threaded and burst compiled ForEach loop over the entities that were just spawned and sets their position using perlin noise.
When the player moves, the system generates new chunks and adds chunks that should be degenerated to a list and destroys all the entities in that chunk (also in main threaad currently).

<strong>Ideas for even more optimization</strong>

Today (the final day) I have worked on only rendering quads that actually are exposed instead of the full blocks which should in theory give much better performance because of the significant decrease in triangles.
I am planning on using DrawMeshInstanced and make a custom shader that takes in a bitmask to determine which part of the texture it should render on the quad.
I've been battling with the API to figure out when a multi-threaded job is done (to then run another job that determines which sides are exposed) but I haven't quite gotten there. I still feel motivated though so I'll keep working on it when I have time.

Additionally I think it's a good idea to spawn the entities using an entity command buffer also to make spawning multi-threaded also.
