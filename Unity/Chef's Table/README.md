﻿# Magic Leap Unity Project

# To group members

You will need to add package TextMesh Pro in Unity

Classes:
ApplicationState: keeping and updating all ingredients & utensils info
Ingredient: name
Utensil: name, Occupied or not
MainScheduler: managing process and steps
Step: a step in the recipe, has timer and set of ingredients and utensils

## Project

Chef's Table!

## Versions

### Unity

2019.3.x

### MLSDK

v0.24.0

### LuminOS

0.98.10+

## Instructions After Downloading

1) Using Unity Hub, download Unity 2019.3.x and make sure Lumin support is checked during installation
2) `ADD` the project using Unity Hub
3) Open the project using Unity Hub
4) Under File > Build Settings, make sure the build target is Lumin
5) Under Unity preferences, set the MLSDK path
6) Under project settings > publishing settings, set your cert path (and make sure the privkey file is in the same directory. If this is confusing, refer to and read our docs. There’s also a `README` in the privkey folder after unzipping)
7) Make sure USB debugging is enabled between your device and computer (which requires MLDB access) and you’re allowing untrusted sources
8) Open the `Game` Scene from `Assets`>`Scenes`>`Game`
