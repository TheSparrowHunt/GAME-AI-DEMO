using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carvingAgent{
	public int x, y;
	public int maxX, maxY;
	public int minimumDistance;
	int distanceTravelled;


	int x_direction, y_direction;

	public carvingAgent(int _x, int _y, int _maxX, int _maxY, int _minimumDistance){
		x = _x;
		y = _y;
		maxX = _maxX-2;
		maxY = _maxY-2;
		minimumDistance = _minimumDistance;
		distanceTravelled = 0;
		randomiseDirection();
	}

	void randomiseDirection(){
		switch (Random.Range (0, 4)) {
		case 0:
			x_direction = 0;
			y_direction = 1;
			break;
		case 1:
			x_direction = 1;
			y_direction = 0;
			break;
		case 2:
			x_direction = 0;
			y_direction = -1;
			break;
		case 3:
			x_direction = -1;
			y_direction = 0;
			break;
		}

	}
		
	public int run(){
		//remove this it has hit or exceeds the map limits
		if (x >= maxX || y >= maxY || x <= 1 || y <= 1) {
			return -1;
		}
		float random = Random.value;

		//carve a space out and move forward on its directiuon vector
		if (random >= 0.0f && random < 0.7f) {
			x += x_direction;
			y += y_direction;
			distanceTravelled++;
			return 0;
		}

		//carve a space out and (potentially) change direction
		if (random >= 0.7f && random < 0.8f) {
			randomiseDirection();
			return 0;
		}

		if (random >= 0.8 && random < 0.805f) {
			distanceTravelled++;
			return -3;
		}

		//cut out and kill this carver
		if (random >= 0.805f && random < 0.93f) {
			if (distanceTravelled > minimumDistance) {
				//(choose whether to place items etc here)
				return -1;
			} 
			else {
				return 0;
			}
		}

		//add a new carver to the list, then run the code again
		if (random >= 0.93f && random <= 1.0f) {
			if (distanceTravelled > minimumDistance) {
				return -2;
			} 
			else {
				return 0;
			}
		}

		return 0;
	}
}
