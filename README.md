# Start
For the first session's build of the project in Unity, you need to:
1. reassign the script `Camera Handler` on *CameraHolder* and assign some objects to feilds
   + Target Transform - Player
   + Camera Transform - Main Camera
   + Camera Pivot Transform - CameraPivot
2. reassign the `Animator` on parent object *Player* (optional)
   + Controller - Character
   + Avatar - RemyAvatar

These actions will allow the camera to correctly follow the player and also correctly display the actions in the player's animator.
