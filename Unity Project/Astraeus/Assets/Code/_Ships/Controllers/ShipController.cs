using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._GameControllers;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.Controllers {
    public abstract class ShipController : MonoBehaviour {
        private Ship _ship;
        public ThrusterController ThrusterController;
        protected List<WeaponController> WeaponControllers;
        private PowerPlantController _powerPlantController;
        private ShieldController _shieldController;
        public CargoController CargoController;
        private List<Ship> hostiles;

        public void Setup(Ship ship) {
            _ship = ship;
            List<PowerPlant> powerPlants = new List<PowerPlant>();
            List<CargoBay> cargoBays = new List<CargoBay>();
            List<Shield> shields = new List<Shield>();
            foreach (var internalSlot in _ship.ShipHull.InternalComponents) {
                if (internalSlot.concreteComponent != null) {
                    InternalComponent component = internalSlot.concreteComponent;
                    if (component.GetType().IsSubclassOf(typeof(PowerPlant))) {
                        powerPlants.Add((PowerPlant)component);
                    } else if (component.GetType() == typeof(CargoBay)) {
                        cargoBays.Add((CargoBay)component);
                    } else if (component.GetType().IsSubclassOf(typeof(Shield))) {
                        shields.Add((Shield)component);
                    }
                }
            }
            _powerPlantController = new PowerPlantController(powerPlants);
            _shieldController = new ShieldController(shields, _powerPlantController);
            CargoController = new CargoController(cargoBays);
            
            List<MainThruster> mainThrusters = _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent).Where(tc => tc != null).ToList();
            ManoeuvringThruster manoeuvringThrusters = _ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            List<float> centerOffsets = _ship.ShipHull.ManoeuvringThrusterComponents.thrusters.Select(t => t.centerOffset).ToList();

            ThrusterController = new ThrusterController(mainThrusters, (manoeuvringThrusters, centerOffsets), GetShipMass(), _powerPlantController);
            WeaponControllers = new List<WeaponController>();
            foreach (var weaponComponent in ship.ShipHull.WeaponComponents) {
                Weapon weapon = weaponComponent.concreteComponent;
                if (weaponComponent.concreteComponent != null) {
                    var weaponGameObject = weapon.InstantiatedGameObject;
                    GameObject spindle = GameObjectHelper.FindChild(weaponGameObject,"TurretSpindle");

                    var weaponController = spindle.gameObject.AddComponent<WeaponController>();
                    weaponController.Setup(weapon, _powerPlantController, gameObject);
                    WeaponControllers.Add(weaponController);
                }
            }
        }

        private void Update() {
            if (!GameController.isPaused) {
                Thrust();
                Turn();
                AimWeapons();
                FireCheck();
                ChargePowerPlant();
            }
        }

        private void ChargePowerPlant() {
            List<Fuel> depletedFuel = _powerPlantController.ChargePowerPlant(Time.deltaTime, CargoController.GetCargoOfType(typeof(Fuel)).Cast<Fuel>().ToList());
            CargoController.RemoveCargo(depletedFuel.Cast<Cargo>().ToList());
        }


        private float GetShipMass() {
            float mass = _ship.ShipHull.HullMass;

            foreach (InternalComponent internalComponent in _ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent)) {
                if (internalComponent != null) {
                    mass += internalComponent.ComponentMass;
                    if (internalComponent.GetType() == typeof(CargoBay)) {
                        mass += ((CargoBay)internalComponent).GetCargoMass();
                    }
                }
            }

            foreach (Thruster thruster in _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent)) {
                if (thruster != null) {
                    mass += thruster.ComponentMass;
                }
            }

            foreach (Weapon weapon in _ship.ShipHull.WeaponComponents.Select(wc => wc.concreteComponent)) {
                if (weapon != null) {
                    mass += weapon.ComponentMass;
                }
            }

            return mass;
        }

        private void Thrust() {
            Vector2 thrustVector = GetThrustVector();
            if (thrustVector != new Vector2()) {
                ThrusterController.FireThrusters(thrustVector, Time.deltaTime, gameObject.transform.localRotation.eulerAngles.z);
            }

            gameObject.transform.Translate( ThrusterController.Velocity* Time.deltaTime, Space.World);
        }

        private void Turn() {
            var turnDir = GetTurnDirection();

            if (turnDir != 0) {
                ThrusterController.TurnShip(Time.deltaTime, turnDir);
            }
            else if(ThrusterController.AngularVelocity!=0){
                ThrusterController.StopTurn(Time.deltaTime);
            }

            var angularVelocity = ThrusterController.AngularVelocity;
            if (angularVelocity != 0) {
                Vector3 rotationVector = new Vector3(0, 0, ThrusterController.AngularVelocity);
                gameObject.transform.Rotate(rotationVector * Time.deltaTime, Space.World);
            }
        }


        protected abstract void AimWeapons();
        protected abstract void FireCheck();

        protected void FireWeapons() {
            foreach (WeaponController weaponController in WeaponControllers) {
                weaponController.Fire(ThrusterController.Velocity);
            }
        }

        protected abstract Vector2 GetThrustVector();

        protected abstract float GetTurnDirection();

        public void TakeDamage(float damage) {
            damage = _shieldController.TakeDamage(damage);
            if (_ship.ShipHull.HullStrength > 0) {
                if (damage > 0) {
                    _ship.ShipHull.HullStrength -= damage;
                    if (_ship.ShipHull.HullStrength < 0) {
                        ShipDestroyed();
                    }
                }
            }
        }

        protected void ShipDestroyed() {
            GameObject particleSystemObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("Effects/ExplosionEffect"));
            particleSystemObject.transform.SetParent(transform);
            particleSystemObject.transform.localPosition = new Vector3();
            ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
            float duration = particleSystem.main.duration + particleSystem.main.startLifetimeMultiplier;
            Destroy(particleSystemObject, duration);
            Destroy(_ship.ShipObject, duration/2);
            Destroy(this, duration);
        }
    }
}