# Astraeus
Astraeus is a procedurally generated space game made in Unity.
Gameplay loops involve trading, combat and exploring.
Currently these activities are fairly basic and serve as more of an example of the usage for the procedural generation techniques involved in the project than the full intended gameplay.

## Texture Generation
![Sample of generated planets](/GitHubPageImgs/GeneratedPlanetsSample.png?raw=true)

This project utilises [FastNoise2](https://github.com/Auburn/FastNoise2Bindings) to replicate noise used for texture generation of planets.
3D noise is mapped to a 2D texture by calculating corresponding positions on the texture and noise.

![Diagram of 3D to 2D coordinate mapping](/GitHubPageImgs/Mapping3DNoiseto2D.png?raw=true)

This technique ensured a seamless connection across the longitude of the texture.

Warping at the poles was reduced by decreasing the size of the sample relative to its proximity to a pole and mapping it to a custom sphere mesh - with the UV map aligning the texture correctly to avoid segmenting.

![Diagram of texture warp correction at planetary pole](/GitHubPageImgs/UnitySphereVsWarpSphereAnnotated.png?raw=true)

## Creating a Galaxy
The galaxy is generated using RNG to provide positions for the numerous solar systems created, in addition to a method inspired by Poisson-Disc Sampling.
A pool of tiled spaces is allocated and removed as systems are assigned to them - removing invalid permutations entirely.
This process vastly speeds up the distribution of solar systems across the galaxy, when compared to Poisson-Disc Sampling, whilst maintaining high variation and uniqueness.

![Sample of a generated galaxy](/GitHubPageImgs/GalaxySample.png?raw=true)

## Customisation

![Hull with different weapons and thrusters](/GitHubPageImgs/ShipCustomisation.png?raw=true)

It was important to display the potential for cosmetic and functional customisation of the player driven entities in the game - the space ships, as a demonstration of the ability to code dynamic, flexible and extensible systems.
Ships and their components are easily swapped inside the outfitting section of a space station.
Some components e.g. weapons/thrusters are limited to assignment in specific locations whereas internal slots can accommodate a range of different items.
As an additional feature - colour customisation was added as a test of the flexibility of the systems responsible for displaying the ship models and physical components.

![Example of a ship with different colour selections](/GitHubPageImgs/ShipColours.png?raw=true)

