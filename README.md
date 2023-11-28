# Expedition RegionTools

the mod folder is [Expedition RegionTools](https://github.com/Mehr1us/Expedition-RegionTools/tree/master/Expedition%20RegionTools)  
the code src folder is [src](https://github.com/Mehr1us/Expedition-RegionTools/tree/master/src)  
the batch file is for copying the contents of bin/release/net42 to the plugins folder in the mod folder upon build success   
if you decide to clone this repo and use with Visual Studio, make sure to change the second line in copyDLL.bat 
to target your StreamingAssets/mod/Expedition RegionTools/plugins so that it copies the updated DLL to the correct place upon successful build

---  

### To Use:  
Create a randomstarts.txt file in your mods modify folder.  

In the randomstarts.txt, anything in a [MERGE] [ENDMERGE] block will be appended to the randomstarts of the expedition mod  
and then any other instances from other mods

In the [MERGE] block, one can create rules for region spawning for non campaign story regions  
example below shows excluding expedition spawning for saint and technomancer for PA and adding an exclusive 
```
[MERGE]

REGIONS
PA [4] : X-Saint,Technomancer
D7 : Dreamer
END REGIONS

[ENDMERGE]
```  
It should be formatted as `REGION [Weight] : (X-)SLUGCAT1,SLUGCAT2` where the Weight is optional, if not included it will use the default of 5  
and where adding the X- prefix it will negate **all** subsequent Slugcats meaning that you specify which characters this region shouldn't be availible for as opposed to which it should be availible for  

### (possibly? no promises) planned features: 
* add tool to tell expedition to use custom pearls/custom regions for pearl collecting quest
