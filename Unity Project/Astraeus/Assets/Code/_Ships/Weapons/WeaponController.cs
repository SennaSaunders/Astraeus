using System.Collections.Generic;

namespace Code._Ships.Weapons {
    public class WeaponController {
        public List<(Weapon weapon, int fireGroup)> Weapons; //holds the Weapons from ship components with their fire group
        public int currentFireGroup = 0;
        public WeaponController(List<Weapon> weapons) {
            SetWeapons(weapons);
        }

        public void SetWeapons(List<Weapon> weapons) {
            List<(Weapon weapon, int fireGroup)> weaponsFireGroup = new List<(Weapon weapon, int fireGroup)>();

            foreach (Weapon weapon in weapons) {//need to check for whether the weapons are already assigned a fire group and not change them if they do
                weaponsFireGroup.Add((weapon, 0));
            }

            Weapons = weaponsFireGroup;
        }
        
        //fire
        public void Fire() {
        }
    }
}