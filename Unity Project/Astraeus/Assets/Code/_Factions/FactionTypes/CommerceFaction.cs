﻿using System.Collections.Generic;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code._Galaxy.GalaxyComponents;

namespace Code._Factions.FactionTypes {
    public class CommerceFaction : Faction {
        public CommerceFaction(SolarSystem homeSystem) : base(homeSystem, FactionTypeEnum.Commerce) {
        }

        public static int BlackHoleDesire = -100;

        public static int GetCommerceFactionSystemDesire(SolarSystem system) {
            //assign higher values to larger planets - so the more large planets the better
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    desireValue += (int)body.Tier * (int)body.Tier;
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += CommerceFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }
    }
}