# UnityAddressablesTest

This project contains two example scripts for loading remote unity addressable assets and scenes. 

Scripts check if addressable asset is available using Addressables.LoadResourceLocations(key).

Scripts exposes some debug info and progress bars.

I'm using Miniweb at path C:/miniweb/htdocs/[ServerData] to quickly test assets deployment, but you can change preferred paths and profiles in AddressableAssetSettings.

Sometimes I'm expiriencing issues in windows and android builds failing to load a scene with error: 
>'Scene 'Assets/Scenes/NewScene.unity' couldn't be loaded because it has not been added to the build settings or the AssetBundle has not been loaded.'
