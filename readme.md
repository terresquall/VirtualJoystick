Setting up
----------
Clone this folder into the Assets folder of an existing Unity project and you should be ready to roll. If you are using Unity 2019 and above, this should work. I suspect it will also work with 2018 and 2017, but it has not been tested.

This repo contains only a folder in the Assets folder instead of the entire Unity project because it is meant to become an Asset Store asset in the future, so doing it this way makes it easier for us to test the files across different versions of Unity.

How to use
----------
To use the Virtual Joystick, pick one of the prefabs from the `VirtualJoystick/Prefabs` folder into your Scene's HUD Canvas and it should be ready to roll.

1. To retrieve input data from the Joystick, use `VirtualJoystick.GetAxis("Horizontal")` to get movement on the x-axis, and `VirtualJoystick.GetAxis("Vertical")` to get movement on the y-axis.
2. You can also use `VirtualJoystick.GetAxis()` to get a `Vector2` containing the horizontal and vertical inputs.
3. You can have multiple joysticks on your Canvas. In the case of multiple joysticks, you will need to specify an index behind `GetAxis()`, like so: `VirtualJoystick.GetAxis("Horizontal", 1)`. In this case, the code will give you the horizontal input of the 2nd joystick on the Scene (the first joystick is index 0).
4. Some mobile games have joysticks that will shift to the player's finger. To enable this feature on your virtual joystick instance, set the **W** and **H** properties in the **Bounds** property and this will denote the area where the joystick can move around in.

Other properties
----------------
To see a comprehensive list of properties, go here: terresquall.com/games/virtual-joystick-pack/#guide

ChangeLog
----------------
**Version 1.0.3**

Added two mechanics.
- When reaching the edge of the joystick radius, the joystick will play a sound clip only if there is an Audio Source.
- The joystick can be pressed like a button if clicked in the middle and calls a seperate method to activate pressed functions.

To make use of the pressable joystick mechanic, set your functions that you want to activate inside of the new method `OnPressEvent()`.

Added three variables for playing audio
- public bool playAudio   //Sets whether the joystick will play a sound when touching radius.
- public AudioClip edgeReachedSound   //Audio clip played when touching radius.
- bool audioPlayed   // Keeps track of whether the edgeReachedSound audio clip has already played.

Added five variables for pressing joystick
- public bool pressableButton   //Set whether the button can be pressed.
- [Range(0, 1)] public float pressableRange   //The range determining whether a joystick is pressed on the button
- [Range(0, 1)] public float pressTimeLenience;   //How long a joystick can be held down before the action is considered a drag.
- bool triggerPressCountdown   // Starts pressTimeCountdown in Update().
- float pressTimeCountdown   // Times how long before press time ends.