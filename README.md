# VRChat 8 Ball Pool source mirror
Original world: https://vrchat.com/home/world/wrld_d02883d1-7d79-4d51-87e2-b318ca4c2b37

![](https://i.imgur.com/3cHrbf1.jpg)

### Thank you:
**Merlin**: UdonSharp

**dragonash2019**: Lots of quality feedback

### Features
- Hand written, pool oriented physics engine based on real world values
- CCD for cue ball
- Deterministic Networking model and full late-joiner support
- Automatically enforced game rules
- Desktop / VR / Quest support
- 1, 2, 3, or 4 players

### Modes
- 8 ball English/American mix Pool
- 9 ball American standard Pool
- 4 ball Japanese Yotsudama and Korean Sagu modes

### Planned
- Tournament mode
- Visual events and information
- Translations into 10 different languages

## For World Creators
Follow the Collisions layers steps first before importing package!

This project can be downloaded from [The releases page](https://github.com/Terri00/vrc8ball/releases)

### Dependencies / Setup
- Install **latest** VRCSDK 3
- Install **latest** [Udon Sharp](https://github.com/MerlinVR/UdonSharp)
- Import the package

#### Collision layers
There are some objects that need to be set to only collide on a seperate layer.

Check the scene `ht8b_public` for a full setup example

Recommended steps:
- Edit > Project Settings > Tags and Layers
- Set User Layer 23 to: `ht8b`
- Edit > Project Settings > Physics
- In the collision matrix deselect all apart from itself for the new layer as so:

![](https://i.imgur.com/jhku3V2.png)

### Quest / PC Toggles
The project includes some small scripts to change / toggle stuff between quest/pc versions

It has to be manually changed


On the top of the prefab there is one:

![](https://i.imgur.com/HPtMBiH.png)

And in the scene `__MAIN__` also has one of these scripts

### Caveats
- HT8B once again has a position requirement, this time its that the Y position in the scene of this prefab should equal 0.0 
- This project is currently not designed / tested with more than one instance of a table in a world and is currently unsupported
