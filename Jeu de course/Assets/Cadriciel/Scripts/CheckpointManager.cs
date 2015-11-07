﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CheckpointManager : MonoBehaviour 
{

	[SerializeField]
	private GameObject _carContainer;

	[SerializeField]
	private int _checkPointCount;
	[SerializeField]
	private int _totalLaps;

	private int _resetTimer;

	private bool _finished = false;
	
	private Dictionary<CarController,PositionData> _carPositions = new Dictionary<CarController, PositionData>();

	private class PositionData
	{
		public int lap;
		public int checkPoint;
		public Transform position;
	}

	// Use this for initialization
	void Awake () 
	{
		foreach (CarController car in _carContainer.GetComponentsInChildren<CarController>(true))
		{
			_carPositions[car] = new PositionData();
		}
		_resetTimer = GameObject.FindWithTag("GameController").GetComponent<RaceManager>().TimeToStart;
	}
	
	public void CheckpointTriggered(CarController car, int checkPointIndex, Transform trans)
	{

		PositionData carData = _carPositions[car];

		if (!_finished)
		{
			if (checkPointIndex == 0)
			{
				if (carData.checkPoint == _checkPointCount-1)
				{
					carData.checkPoint = checkPointIndex;
					carData.position = trans;
					carData.lap += 1;
					Debug.Log(car.name + " lap " + carData.lap);
					if (IsPlayer(car))
					{
						GetComponent<RaceManager>().Announce("Tour " + (carData.lap+1).ToString());
					}

					if (carData.lap >= _totalLaps)
					{
						_finished = true;
						GetComponent<RaceManager>().EndRace(car.name.ToLower());
					}
				}
			}
			else if (carData.checkPoint == checkPointIndex-1) //Checkpoints must be hit in order
			{
				carData.checkPoint = checkPointIndex;
			}
			carData.position = trans;
		}


	}

	bool IsPlayer(CarController car)
	{
		return car.GetComponent<CarUserControlMP>() != null;
	}

	private int _index = -3;

	public IEnumerator ResetCar(CarController car)
	{
		if (_carPositions [car].position != null) 
		{
			int count = (int)((float)_resetTimer / 0.16f + 0.5f);
			if(_index == 3)
				_index = -3;
			int index = _index++;
			do
			{
				car.transform.position = car.GetComponent<WaypointProgressTracker>().LastWaypoint.position;
				car.transform.position = new Vector3(car.transform.position.x + (index%2)*0.0f, car.transform.position.y, car.transform.position.z + index*7.0f);
				car.transform.rotation = car.GetComponent<WaypointProgressTracker>().LastWaypoint.rotation;
				car.rigidbody.velocity = Vector3.zero;

				yield return new WaitForSeconds (0.16f);
				count--;
			} while (count > 0);
		}
		car.Reset();
	}
}
