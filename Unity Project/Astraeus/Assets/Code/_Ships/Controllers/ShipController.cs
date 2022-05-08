using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._GameControllers;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using Code.GUI.ShipGUI;
using Code.GUI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code._Ships.Controllers {
    public abstract class ShipController : MonoBehaviour {
        public Ship _ship { get; private set; }
        public ThrusterController ThrusterController;
        protected List<WeaponController> WeaponControllers;
        protected PowerPlantController _powerPlantController;
        private ShieldController _shieldController;
        public CargoController CargoController;
        public JumpDriveController JumpDriveController;
        protected GameObject _shipHealthGUI;
        private GameObject speedIndicator;
        
        public List<Ship> hostiles = new List<Ship>();
        public bool beingDestroyed = false;

        public virtual void Setup(Ship ship) {
            _ship = ship;
            List<PowerPlant> powerPlants = new List<PowerPlant>();
            List<CargoBay> cargoBays = new List<CargoBay>();
            List<Shield> shields = new List<Shield>();
            List<JumpDrive> jumpDrives = new List<JumpDrive>();
            foreach (var internalSlot in _ship.ShipHull.InternalComponents) {
                if (internalSlot.concreteComponent != null) {
                    InternalComponent component = internalSlot.concreteComponent;
                    if (component.GetType().IsSubclassOf(typeof(PowerPlant))) {
                        powerPlants.Add((PowerPlant)component);
                    } else if (component.GetType() == typeof(CargoBay)) {
                        cargoBays.Add((CargoBay)component);
                    } else if (component.GetType().IsSubclassOf(typeof(Shield))) {
                        shields.Add((Shield)component);
                    } else if (component.GetType() == typeof(JumpDrive)) {
                        jumpDrives.Add((JumpDrive)component);
                    }
                }
            }
            _powerPlantController = new PowerPlantController(powerPlants);
            _shieldController = new ShieldController(shields, _powerPlantController);
            CargoController = new CargoController(cargoBays);
            JumpDriveController = new JumpDriveController(jumpDrives, CargoController);
            
            List<MainThruster> mainThrusters = _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent).Where(tc => tc != null).ToList();
            var manoeuvringThrusters = _ship.ShipHull.ManoeuvringThrusterComponents;
            float angularAcceleration = manoeuvringThrusters.concreteComponent != null ? ship.ShipHull.CalcAngularAcceleration(manoeuvringThrusters.concreteComponent.ComponentSize) : 0;

            ThrusterController = new ThrusterController(mainThrusters, manoeuvringThrusters.concreteComponent,manoeuvringThrusters.parentTransformNames.Count, angularAcceleration, GetShipMass(), _powerPlantController);
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
            SetupShipHealthGUI();
            ThrustIndicator();
        }

        private void SetupShipHealthGUI() {
            _shipHealthGUI = (GameObject)Instantiate(Resources.Load("GUIPrefabs/Ship/ShipHealthGUI"), _ship.ShipObject.transform, false);
            _shipHealthGUI.AddComponent<RotateToPlayer>();
            
            _shieldController.ClearObservers();
            GameObject shieldBar = GameObjectHelper.FindChild(_shipHealthGUI, "ShieldBar");
            ShipBarObserver shieldObserver = shieldBar.AddComponent<ShipBarObserver>();
            _shieldController.AddObserver(shieldObserver);
            _shieldController.NotifyObservers();
            
            _ship.ShipHull.ClearObservers();
            GameObject healthBar = GameObjectHelper.FindChild(_shipHealthGUI, "HealthBar");
            ShipBarObserver healthObserver = healthBar.AddComponent<ShipBarObserver>();
            _ship.ShipHull.AddObserver(healthObserver);
            _ship.ShipHull.NotifyObservers();
        }

        private void Update() {
            if (!GameController.IsPaused) {
                Thrust();
                Turn();
                AimWeapons();
                FireCheck();
                ChargePowerPlant();
                _shieldController.ChargeShields();
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

        private void ThrustIndicator() {
            if (speedIndicator == null) {
                speedIndicator = Instantiate((GameObject)Resources.Load("GUIPrefabs/Ship/SpeedIndicator"), gameObject.transform);
                GameObjectHelper.FindChild(speedIndicator, "SpeedTxtValue").AddComponent<RotateToPlayer>();;
            }

            
            var speedArea = GameObjectHelper.FindChild(speedIndicator, "SpeedArea");
            float velocityRotation = Vector2.SignedAngle(Vector2.up, ThrusterController.Velocity);
            speedArea.transform.rotation = Quaternion.Euler(0,0,velocityRotation);
            Vector2 speedBarMaxSize = speedArea.GetComponent<RectTransform>().sizeDelta;
            float relativeThrustSize = ThrusterController.Velocity.magnitude / ThrusterController.MaxSpeed;
            
            GameObject speedBar = GameObjectHelper.FindChild(speedIndicator, "SpeedBar");
            speedBar.GetComponent<RectTransform>().sizeDelta = new Vector2(speedBarMaxSize.x, relativeThrustSize * speedBarMaxSize.y);

            speedBar.GetComponent<Image>().color = new Color(1, 1- relativeThrustSize, 0);

            string speedTxt = ThrusterController.Velocity.magnitude.ToString("0.0") + "m/s";
            GameObjectHelper.SetGUITextValue(speedIndicator, "SpeedTxtValue", speedTxt);
        }

        private void Thrust() {
            Vector2 thrustVector = new Vector2();
            if (!beingDestroyed) {
                thrustVector = GetThrustVector();
            }
            
            if (thrustVector != new Vector2()) {//skips thrust calculations if no thrust is being applied
                ThrusterController.FireThrusters(thrustVector, Time.deltaTime, gameObject.transform.localRotation.eulerAngles.z);
            }
            gameObject.transform.Translate( ThrusterController.Velocity * Time.deltaTime, Space.World);
            ThrustIndicator();
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
            if (!beingDestroyed) {
                foreach (WeaponController weaponController in WeaponControllers) {
                    weaponController.Fire(ThrusterController.Velocity);
                }
            }
        }

        protected abstract Vector2 GetThrustVector();

        protected abstract float GetTurnDirection();

        public void TakeDamage(float damage) {
            damage = _shieldController.TakeDamage(damage);
            if (_ship.ShipHull.CurrentHullStrength > 0) {
                if (damage > 0) {
                    _ship.ShipHull.TakeDamage(damage);
                    if (_ship.ShipHull.CurrentHullStrength < 0) {
                        ShipDestroyed();
                    }
                }
            }
        }

        public void RemoveHostility() {
            foreach (Ship hostile in hostiles) {
                if (hostile.ShipObject != null) {//check if the hostile ship hasn't been destroyed
                    hostile.ShipObject.GetComponent<ShipController>().hostiles.Remove(_ship);
                }
            }
        }

        protected virtual void ShipDestroyed() {
            beingDestroyed = true;
            RemoveHostility();
            
            GameObject particleSystemObject = Instantiate((GameObject)Resources.Load("Effects/ExplosionEffect"), transform, true);
            
            particleSystemObject.transform.localPosition = new Vector3();
            ParticleSystem.MainModule particleMain = particleSystemObject.GetComponent<ParticleSystem>().main;
            float duration = particleMain.duration + particleMain.startLifetimeMultiplier;
            Destroy(particleSystemObject, duration);
            Destroy(_ship.ShipObject, duration/4);
            // Destroy(this, duration);
        }
    }
}