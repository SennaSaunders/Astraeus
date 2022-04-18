using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using UnityEngine;
using Random = System.Random;

namespace Code._Ships.Controllers {
    public class NPCItineraryController {
        private NPCShipController _npcShipController;
        private SolarSystem _solarSystem;
        private List<Vector2> _schedule;
        private List<Body> _bodySchedule; 
        private Body _frequentStop;
        private float _frequentStopChance = 0.8f;
        private int _baseScheduleLength = 10;
        private float _scheduleModifier = .5f;
        public int _maxStopsBeforeFrequent = 3;
        

        private void SetDestination() {
            //check if there are hostiles nearby - if so pause itinerary
            //if ship is a military ship/pirate it should focus on combat
            //traders should run
            if (!HostilesCheck()) {
                if (!HuntCheck()) {
                    if (_schedule.Count == 0) {
                        FlyAway();
                    }
                    else {
                        if (ReachedDestinationCheck()) {
                            ReachedDestination();
                        }
                        
                    }
                }
            }
        }

        private bool ReachedDestinationCheck() {
            return false;
        }

        private void ReachedDestination() {
            _schedule.RemoveAt(0);
            if (_schedule.Count > 0) {
                
            }
        }

        private bool HostilesCheck() {
            return false;
        }

        //pirates/military should seek out targets
        //any target for pirates
        //hostiles against other ships for military priority on helping highest faction relation
        private bool HuntCheck() {
            return false;
        }
        
        private void FlyAway() {
            
        }

        private void SetSchedule() {
            Random r = new Random();
            int maxRoll = _solarSystem.Bodies.Count;
            if (maxRoll > 2) {
                bool hasFrequentStop = r.NextDouble() < _frequentStopChance;
                if (hasFrequentStop) {
                    _frequentStop = _solarSystem.Bodies[r.Next(maxRoll)];
                    int scheduleLength = (int)(r.NextDouble() * _scheduleModifier * _baseScheduleLength + _baseScheduleLength);
                    int stopsTillFrequent = 0;
                    while (_bodySchedule.Count < scheduleLength) {
                        if (stopsTillFrequent == 0) {
                            _bodySchedule.Add(_frequentStop);
                            stopsTillFrequent = r.Next(_maxStopsBeforeFrequent) + 1;
                        }
                        else {
                            Body nextStop = _solarSystem.Bodies[r.Next(maxRoll)];
                            if (nextStop != _frequentStop) {
                                _bodySchedule.Add(nextStop);
                                stopsTillFrequent--;
                            }
                        }
                    }

                    if (_bodySchedule[_bodySchedule.Count - 1] != _frequentStop) {
                        _bodySchedule.Add(_frequentStop);
                    }
                }
            }else if (maxRoll == 2) {
                //short schedule go between the points and then leave
                //if one of the points is a space station then visit and leave
                //if only stars then just fly around randomly
                int first = r.Next(2);
                int second = first == 1 ? 0 : 1;
                _bodySchedule.Add(_solarSystem.Bodies[first]);
                _bodySchedule.Add(_solarSystem.Bodies[second]);
            }
            else {
                _bodySchedule.Add(_solarSystem.Bodies[0]);
            }
        }

        
    }
}