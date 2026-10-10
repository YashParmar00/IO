# Script Organization

Keep one main Unity component per C# file and keep the file name identical to the class name.

- Config: shared Inspector-editable tuning values in GameConfig.cs.
- Core: global run state and camera behavior.
- Input: device reading and steering targets only; no transform movement.
- Player: head heading and Rigidbody2D movement.
- Gameplay: arena bounds, food spawning/pickup, worm rules, worm body trail, collision sensing, segment ownership, and bot decision-making.
- UI: score persistence, score rendering, and game-flow controls.

Read the root README's "Every source file" and "Which components belong on which GameObject?" sections before adding a new component. Keep the current division of responsibilities instead of putting the whole game in GameManager.cs or PlayerController.cs.
