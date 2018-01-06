using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_fill : MonoBehaviour {
	
	public float y_height = 0;

	public List<List<int>> map;

	private List<carvingAgent> carvers;

	public GameObject object_empty;
	public GameObject object_wall;
	public GameObject player;

	[Range(10, 1000)]
	public int map_size = 0;
	[Range(1, 100)]
	public int minimumDistanceForEachCarver;
	[Range(1, 50)]
	public int initalCrawlerCount;
	[Range(1, 1000)] 
	public int minimumFloorSpace;
	[Range(50, 10000)]
	public int maximumFloorSpace;

	// Use this for initialization
	void Start () {
		generate();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("q")) {
			generate();
		}
	}

	void generate(){
		if (checkValues ()) {
			map = new List<List<int>> ();
			carvers = new List<carvingAgent> ();
			removeOld ();
			generateNew ();
		}
	}

	void removeOld(){
		if (GameObject.FindGameObjectsWithTag("Tile") != null) {
			GameObject[] tiles = GameObject.FindGameObjectsWithTag ("Tile");
			foreach (GameObject tile in tiles) {
				Destroy(tile);
			}
		}
	}

	bool checkValues(){

		if (minimumFloorSpace > maximumFloorSpace){
			minimumFloorSpace = maximumFloorSpace;
			Debug.Log("minimum floor space larger than maximum floor space");
		}

		if (minimumFloorSpace == maximumFloorSpace) {
			minimumFloorSpace = maximumFloorSpace - (int)(((float)maximumFloorSpace)*0.1f);
		}


		if (minimumFloorSpace > ((map_size-1) * (map_size-1))/2){
			minimumFloorSpace = ((map_size - 1) * (map_size - 1)) / 2;
			Debug.Log("minimum bigger than half map");
		}

		if (minimumDistanceForEachCarver > map_size/2) {
			minimumDistanceForEachCarver = map_size / 2;
			Debug.Log("minimum distance bigger than half map");
		}

		if (maximumFloorSpace < (minimumDistanceForEachCarver * initalCrawlerCount)) {
			maximumFloorSpace = (minimumDistanceForEachCarver * initalCrawlerCount);
			Debug.Log("maximum floor bigger than minimum distance and initial count");
		}

		if (minimumFloorSpace > (minimumDistanceForEachCarver * initalCrawlerCount)*3) {
			minimumFloorSpace = (minimumDistanceForEachCarver * initalCrawlerCount)*3;
			Debug.Log("minimum floor bigger than 1/3 minimum distance and initial count");
		}

		if ((float)(initalCrawlerCount * minimumDistanceForEachCarver) < ((float)minimumFloorSpace * 0.1f)){
			minimumFloorSpace = (int)((float)initalCrawlerCount * (float)minimumDistanceForEachCarver * 0.1f);
			Debug.Log("minimum floor lower than initial count * minimum distance");
		}

		if ((float)maximumFloorSpace < ((float)initalCrawlerCount * (float)minimumDistanceForEachCarver * 0.7f)) {
			maximumFloorSpace = (int)((float)initalCrawlerCount * (float)minimumDistanceForEachCarver * 0.7f);
			Debug.Log("maximum floor space lower than 1/10 inital count * minimum distance");
		}


		return true;
	}

	void generateNew(){
		createEmpty();
		carveOut();
		if (evaluateMap()) {
			removeNonEdges ();
			flushValuesToEnvironment();
		} 
		else{
			generateNew();
		}

	}

	void createEmpty(){
		map.Clear();
		for(int x = 0; x < map_size; x++){
			map.Add(new List<int>());
			for(int y = 0; y < map_size; y++){
				//if we're on the first/last one
				map[x].Add(1);
			}
		}

	}

	void carveOut(){
		for (int i = 0; i < initalCrawlerCount; i++) {
			carvers.Add(new carvingAgent(map_size/2,map_size/2, map_size, map_size, minimumDistanceForEachCarver));
		}
		//add a few new carving agents in the middle of the map.
		while(carvers.Count > 0){
			carveCycle();
		}
	}

	void carveCycle(){
		for(int i = 0; i < carvers.Count; i++){
			switch (carvers[i].run()) {
			case 0:
				map[carvers[i].x][carvers[i].y]	= 0;
				break;
			case -1:
				carvers.Remove(carvers[i]);
				break;
			case -2:
				carvers.Add (new carvingAgent (carvers[i].x, carvers[i].y, map_size, map_size, minimumDistanceForEachCarver));
				break;

			case -3:
				clearRoomArea (carvers [i].x, carvers [i].y, roomSize());
				break;

			case -4:
				break;
			}

		}
			
	}

	//a very simple evaluation function based on the amount of usable floor space
	bool evaluateMap(){
		int floorSpace = 0;
		for(int i = 0; i < map.Count; i++){
			for(int j = 0; j < map[i].Count; j++){
				if (map [i] [j] == 0) {
					floorSpace++;
				}
			}
		}

		return ((floorSpace >= minimumFloorSpace) && (floorSpace <= maximumFloorSpace));
	}

	int roomSize(){
		return (Random.Range (5, 14));
	}

	void clearRoomArea(int x, int y, int range){
		for (int i = -range / 2; i < range / 2; i++) {
			for (int j = -range / 2; j < range / 2; j++) {
				//limit protection
				if (Mathf.Abs (x + i) >= map_size-1 || Mathf.Abs (y + j) >= map_size-1 || (x + i) <= 1 || (y + j) <= 1) {
					break;
				} 
				else{
					//clear the area
					map [x + i] [y + j] = 0;
				}

			}
		}
	}

	//this was implemented to improve performance - not placing 10,000 tiles for a 100*100 grid
	void removeNonEdges(){
		//make a new map
		List<List<int>> newMap = new List<List<int>>();
		//copy the info from the old map
		for (int i = 0; i < map.Count; i++){

			List<int> line = new List<int>();
			for (int j = 0; j < map [i].Count; j++) {
				line.Add (map [i] [j]);
			}
			newMap.Add(line);
		}
			
		//iterate through all except the edges of the original map
		for (int i = 1; i < map.Count-1; i++) {
			for (int j = 1; j < map [i].Count-1; j++) {
				int surrounding = 0;
				for (int k = -1; k < 2; k++) {
					for (int l = -1; l < 2; l++) {
						if (map [i + k] [j + l] == 1) {
							surrounding++;
						}
					}
				}
				//if this tile is completely surrounded, it doesn't need to be placed, so can be removed. 
				if (surrounding == 9) {
					newMap [i] [j] = 0;
				}
			}
		}

		//replace the map with the new map that's removed the non-edges
		map = newMap;
	}

	//actually place tiles in the environmen
	void flushValuesToEnvironment(){
		int lines = 0;
		int units = 0;
		float widthOfTile = object_wall.transform.localScale.x;
		//lowest position something can be on the flat plane
		float mininumPosition = -((widthOfTile * map_size) / 2.0f);

		Vector3 position = new Vector3(0.0f, y_height, 0.0f);

		Debug.Log("flushing");

		for(int i = 0; i < map.Count; i++){
			units = 0;
			position.x = (mininumPosition + (lines*widthOfTile));

			for(int j = 0; j < map[i].Count; j++) {
				position.z = (mininumPosition + (units * widthOfTile));
				int value = map [i] [j];
				switch(value){
				case 0:
					/*position.y = object_empty.transform.position.y;
					Instantiate(object_empty, position, object_empty.transform.rotation);*/
					break;
				case 1:
					position.y = object_wall.transform.position.y;
					Instantiate(object_wall, position, Quaternion.identity);
					break;
				default:

					break;
				}
				units++;
			}
			lines++;
		}

		//place the player at the origin (guarenteed to be free)
		player.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
	}

}
