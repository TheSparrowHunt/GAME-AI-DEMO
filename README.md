# This is an example of procedural dungeon creation code

This example uses the Unity Standard Assets (which are included in this repo, this code was made by Unity Technologies). My code uses the FPSController prefab as a control method.[1]

The methodology of dungeon creation that this implements is based on the method used in the roguelike game Nuclear Throne[2] (formerly known as Wasteland Kings). My implementation only implements the first three steps as described in that write up. Its also worth noting that my implementation does not rely on using real-time steps as Nuclear Throne's does, but instead runs as fast as possible, generating a level in a much shorter amount of time. This methodology guarantees that any level generated will be completely connected under all circumstances.

Use: open Main.unity in the Unity editor, I used Unity 5.6, run the scene. The E key swaps between a first person camera and a third person, overhead camera (for viewing generated levels). The Q key generates a new level. Contained as a component on the GameObject named gameObject is a script called debug_fill.cs (this name remains from the original script I used to test programmatic object creation), this script has a number of user controllable parameters - map size, the minimum distance each carver will move before having a chance of destroying itself, the initial amount of carvers that are generated (here referred to as crawlers) and the minimum and maximum floor space (the space that the player can walk in).

##How it Works

####The generate Function

1. The values from the editor are evaluated against a list of limitations to protect against extremely unlikely or impossible maps, if this condition is met, parameters are changed to their nearest value that wouldn't result in one of these situations happening.
2. A new List of Lists of ints called map is created (this will be our complete map).
3. A new list of carvingAgents is created (this will be our list of carving agents)
4. Any remnants from the previous map are destroyed with the removeOld function.
5. generateNew is called

####The generateNew Function

1. createEmpty() clears the map and fills it with map_size number of map_size columns of 1. (map_size squared number of 1s (for the map, the number 1 represents a filled tile)).
2. carveOut() adds the number of inital carving agents decided in the editor to a list, in the middle of the map.
3. while there's still carvers in the list it runs cycles of carveCycle()
4. evaluateMap() is called, this method checks the map against the parameters wanted, if it meets those, it calls removeNonEdges() and flushValuesToEnvironment() otherwise it calls generateNew() again, making this a tail recursive function.


####The carveCycle Function

1. this runs each carver's run() member function and gets its returned value
2. case 0: this sets the position in the map this carver is at to 0
3. case -1: this removes the carver from the list (destroy)
4. case -2: this adds a new carver to the list at the same position the one that returned the run value had.
5. case -3: this places a room (a square of clear space)with the clearRoomArea() function

####The CarvingAgent Class

Member variables: an x and y position, maximum x and y positions (for protection against going out of the map limits), the minimum distance it can travel before being destroyed.
functions: randomiseDirection() - this chooses a direction to travel in, run() this behaviour defines how this carver moves around the map and what results it passes back to debug_fill's carveCycle function.

####The run Function

1. This checks if the current position has exceeded the bounds of the map, if so carveCycle destroys it.
2. it then generates a random number with Random.value and stores it.
3. (70% likelihood) the position of the carvingAgent is moved one further along on its direction vector.
4. (10% likelihood) the direction vector is randomised to (possibly) another direction.
5. (0.5% likelihood) a room is created at the current position of this carvingAgent.
6. (12.5% likelihood) if the distance travelled by this carver exceeds the minimum distance needed, this carver is destroyed, otherwise it just places an open space in the map.
7. (7% likelihood) if the minimum distance has been exceeded create a new carvingAgent at the position of this one
note: its important that the rate of creation for new carvers is less than the rate of destruction of current carvers as otherwise this would most often result in the cycle never ending and crashing by overflowing the stack.

####The removeNonEdges Function

This function was implemented to reduce the amount of objects placed and therefore significantly improve the performance of this code, especially with very large maps. it simply finds all instances in the map (List of List of ints) that are a filled tile but are also completely surrounded by filled tiles and replaces them with empty tiles. This results is the map being just the edges of the map and therefore not functionally different in gameplay but with a massively reduced number of GameObjects to place when calling flushValuesToEnvironment() and significantly less behaviour to run at runtime.

####The flushValuesToEnvironment Function

This function simply iterates through the map and places filled tile objects where there are 1s (scaled for the prefab), then sets the player's position to 0,0 (guaranteed to be free as its the start for all of the crawlers)

A Demonstration:
https://drive.google.com/file/d/1GCgnYgzzYIxOETk0NSnX21_R5ozTquF9/view?usp=sharing


1. Unity Technologies, Unity Standard Assets - https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-32351

2. Rami Ismail, Random level generation in Wasteland Kings http://rami-ismail.squarespace.com/blog/2013/04/02/random-level-generation-in-wasteland-kings
