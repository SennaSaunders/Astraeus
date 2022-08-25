using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._GameControllers;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Utility;
using UnityEngine;
using Random = System.Random;

namespace Code._Ships.Controllers {
    public class NPCItineraryController : MonoBehaviour {
        private NPCShipController _npcShipController;
        private SolarSystem _solarSystem;
        private List<Vector2> _schedule = new List<Vector2>();
        private Body _frequentStop;
        private float _frequentStopChance = 0.8f;
        private int _baseScheduleLength = 10;
        private float _scheduleModifier = .5f;
        public int _maxStopsBeforeFrequent = 3;
        private GameController _gameController;

        private void Awake() {
            _gameController = GameObjectHelper.GetGameController();
            _solarSystem = _gameController.CurrentSolarSystem;
            SetSchedule();
            _npcShipController = gameObject.GetComponent<NPCShipController>();
            _npcShipController.destination = _schedule[0];
        }

        private void Update() {
            if (!_gameController.IsPaused) {
                SetScheduleDestination();
            }
        }

        private void SetScheduleDestination() {
            //check if there are hostiles nearby - if so pause itinerary
            //if ship is a military ship/pirate it should focus on combat
            //traders should run
            if (!SetHostilesDestination()) {
                if (!SetHuntDestination()) {
                    if (_schedule.Count == 0) {
                        DespawnFlyAway();
                    }
                    else {
                        ReachedDestination();
                    }
                }
            }
        }

        private void ReachedDestination() {
            float closeEnough = 30;
            if (((Vector2)_npcShipController.transform.position - _npcShipController.destination).magnitude < closeEnough) {
                _schedule.RemoveAt(0);
                if (_schedule.Count > 0) {
                    _npcShipController.destination = _schedule[0];
                }
            }
        }

        private bool SetHostilesDestination() {
            //check if hostiles still exist - remove if not
            for (int i = _npcShipController.hostiles.Count - 1; i >= 0; i--) {
                if (_npcShipController.hostiles[i].ShipObject == null) {
                    _npcShipController.hostiles.Remove(_npcShipController.hostiles[i]);
                }
            }

            //check hostiles for which is closest
            if (_npcShipController.hostiles.Count > 0) {
                _npcShipController.SpeedLimit = ThrusterController.MaxSpeed;//let npcs move full speed if hostiles are present

                Ship closestHostile = _npcShipController.hostiles[0];
                for (int i = 1; i < _npcShipController.hostiles.Count; i++) {
                    var currentClosestDistance = (closestHostile.ShipObject.transform.position - transform.position).magnitude;
                    var currentDistance = (_npcShipController.hostiles[i].ShipObject.transform.position - transform.position).magnitude;
                    if (currentDistance < currentClosestDistance) {
                        closestHostile = _npcShipController.hostiles[i];
                    }
                }


                //should change to be slightly away from the 
                _npcShipController.destination = closestHostile.ShipObject.transform.position;
                _npcShipController.currentTarget = closestHostile.ShipObject.GetComponent<ShipController>();
            }
            else {
                _npcShipController.SpeedLimit =NPCShipController.CruisingSpeed;//if no hostiles cruise at a slower speed
            }

            return false;
        }

        //pirates/military should seek out targets
        //any target for pirates
        //hostiles against other ships for military priority on helping highest faction relation
        private bool SetHuntDestination() {
            //get all ships that are of a hostile status to this ship's faction
            return false;
        }

        private void DespawnFlyAway() {
            if (!gameObject.GetComponentInChildren<Renderer>().isVisible) {
                _npcShipController.RemoveHostility();
                Destroy(gameObject);
            }

            Vector2 playerPos = _gameController.CurrentShip.ShipObject.transform.position;
            Vector2 delta = playerPos - (Vector2)gameObject.transform.position;
            _npcShipController.destination = Quaternion.Euler(0, 0, 180) * delta;
        }

        private void SetSchedule() {
            Random r = new Random();
            int maxRoll = _solarSystem.Bodies.Count;

            List<Body> bodySchedule = new List<Body>();
            if (maxRoll > 2) {
                int scheduleLength = (int)(r.NextDouble() * _scheduleModifier * _baseScheduleLength + _baseScheduleLength);
                bool hasFrequentStop = r.NextDouble() < _frequentStopChance;
                if (hasFrequentStop) {
                    _frequentStop = _solarSystem.Bodies[r.Next(maxRoll)];

                    int stopsTillFrequent = 0;
                    while (bodySchedule.Count < scheduleLength) {
                        if (stopsTillFrequent == 0) {
                            bodySchedule.Add(_frequentStop);
                            stopsTillFrequent = r.Next(_maxStopsBeforeFrequent) + 1;
                        }
                        else {
                            Body nextStop = _solarSystem.Bodies[r.Next(maxRoll)];
                            if (nextStop != _frequentStop && nextStop != bodySchedule[bodySchedule.Count - 1]) {
                                bodySchedule.Add(nextStop);
                                stopsTillFrequent--;
                            }
                        }
                    }

                    if (bodySchedule[bodySchedule.Count - 1] != _frequentStop) {
                        bodySchedule.Add(_frequentStop);
                    }
                }
                else {
                    while (bodySchedule.Count < scheduleLength) {
                        Body nextStop = _solarSystem.Bodies[r.Next(maxRoll)];
                        if (bodySchedule.Count > 0) {
                            if (bodySchedule[bodySchedule.Count - 1] != nextStop) {
                                bodySchedule.Add(nextStop);
                            }
                        }
                        else {
                            bodySchedule.Add(nextStop);
                        }
                    }
                }
            }
            else if (maxRoll == 2) {
                //short schedule go between the points and then leave
                //if one of the points is a space station then visit and leave
                //if only stars then just fly around randomly
                int first = r.Next(2);
                int second = first == 1 ? 0 : 1;
                bodySchedule.Add(_solarSystem.Bodies[first]);
                bodySchedule.Add(_solarSystem.Bodies[second]);
            }
            else {
                bodySchedule.Add(_solarSystem.Bodies[0]);
            }

            foreach (Body body in bodySchedule) {
                _schedule.Add(_gameController.GalaxyController.activeSystemController.GetBodyGameObject(body).transform.position);
            }
        }
    }
}