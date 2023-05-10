Setting up
----------
Clone this folder into the Assets folder of an existing Unity project and you should be ready to roll. If you are using Unity 2019 and above, this should work. I suspect it will also work with 2018 and 2017, but it has not been tested.

This repo contains only a folder in the Assets folder instead of the entire Unity project because it is meant to become an Asset Store asset in the future, so doing it this way makes it easier for us to test the files across different versions of Unity.

How to use
----------
To use the Virtual Joystick, drag the Virtual Joystick prefab from the Prefabs folder into your Scene's HUD Canvas and it should be ready to roll.

1. To retrieve input data from the Joystick, use `VirtualJoystick.GetAxis("Horizontal")` to get movement on the x-axis, and `VirtualJoystick.GetAxis("Vertical")` to get movement on the y-axis.
2. You can also use `VirtualJoystick.GetAxis()` to get a `Vector2` containing the horizontal and vertical inputs.
3. You can have multiple joysticks on your Canvas. In the case of multiple joysticks, you will need to specify an index behind `GetAxis()`, like so: `VirtualJoystick.GetAxis("Horizontal", 1)`. In this case, the code will give you the horizontal input of the 2nd joystick on the Scene (the first joystick is index 0).
4. Some mobile games have joysticks that will shift to the player's finger. To enable this feature on your virtual joystick instance, set the **W** and **H** properties in the **Bounds** property and this will denote the area where the joystick can move around in.
5. To create joystick dead zones, select the type (value or radius) in the inspector and adjust the corresponding variable. Radius dead zone registers no input in a certain radius, while the value dead zone registers no input if the value of the axis is lower than it.
6. The increase and decrease buttons in the inspector change the size of the joystick, control stick, dead zone radius, and control stick radius.
7. The "Directions" variable is the number of directions that the joystick can snap to. Keep this value at 0 to make this joystick a free one.

Other properties
----------------
- **Radius:** You can set the radius property to set how far away from the joystick base the control stick can move away from.
- **Sensitivity:** This is the limit to how fast the control stick of the joystick can move to the position where your finger is.