namespace Code.CelestialObjects.Planet {
    public class Planet : CelestialBody {
        public Planet(Body primary, BodyTier tier) : base(primary, tier) { }

        public static BodyTier maxPlanetTier = BodyTier.T6;
        public static BodyTier minPlanetTier = BodyTier.T1;
    }
}