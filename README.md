# TestUnity3D
This is a set of some classes, methods that implement scenes in 3D using Unity and C#.

## Player possibilities
1. WASD - player movement
2. mouse movement - camera position
3. ctrl - roll (while movement) /backstep (while Idle)
4. shift - sprint (while movement)
5. space - jump
6. LMB - hit

All contrlols have an analogs for gamepad (check Input Actions ```Player Controls```).
   

## Start
For the first session's build of the project in Unity, you need to:
1. reassign the script `Camera Handler` on *CameraHolder* and assign some objects to feilds
   + Target Transform - Player
   + Camera Transform - Main Camera
   + Camera Pivot Transform - CameraPivot
2. reassign the `Animator` on parent object *Player* (optional)
   + Controller - Character
   + Avatar - RemyAvatar

These actions will allow the camera to correctly follow the player and also correctly display the actions in the player's animator.
