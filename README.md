# VRChat / Harry_T 8 Ball source mirror

![](https://i.imgur.com/3cHrbf1.jpg)

### Bugs / Feature requests
Please feel free to submit anything to the issues page here

### Source files
(tab size: 3, a lot of the code doc relys on this)

### For World Creators
This project can be downloaded from [The releases page](https://github.com/Terri00/vrc8ball/releases)

#### Dependencies / Setup
- Install VRCSDK 3
- Install [Udon Sharp](https://github.com/MerlinVR/UdonSharp)
- Import the package

#### Quest / PC Toggles
The project includes some small scripts to change / toggle stuff between quest/pc versions

It has to be manually changed


On the top of the prefab there is one:

![](https://i.imgur.com/HPtMBiH.png)

And in the scene `__MAIN__` also has one of these scripts

#### Caveats
HT8B once again has a position requirement, this time its that the Y position in the scene of this prefab should equal 0.0 

This is due to one of the shaders (the contact shadow one)
