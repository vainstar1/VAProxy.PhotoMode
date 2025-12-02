VA Proxy PhotoMode
==================

What this does
- BepInEx plugin (`vainstar.FreecamPause`) that pauses/resumes the game and toggles freecam plus UI together on one key (default `F`).
- Freecam stays controllable while paused; Shift = 4x speed, Alt = half speed.
- UI objects `UI/Stuff/UI/ui` and `UI/Stuff/UI/ui/StandardUI` are hidden while paused/freecam and restored when unpaused.
- Automatically attaches freecam to the active `vThirdPersonCamera` in any scene except `Intro` and `Menu`.

Install
- Build the project (net48) and copy `PhotoMode.dll` to your game’s `BepInEx/plugins` folder (build.bat is not required for end users).
- Launch the game once to generate the config at `BepInEx/config/vainstar.FreecamPause.cfg`.

Configure
- Edit `Keybinds.FreecamPauseKey` in the config to change the toggle key.

Use
- In gameplay scenes, press the configured key to pause + enable freecam + hide UI; press again to resume + disable freecam + show UI. Shift/Alt adjust freecam speed.
