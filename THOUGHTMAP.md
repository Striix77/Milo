1. Tileset (Ground, Platforms, Walls, etc.)
   Recommended size: 32×32 or 64×64
   Why?
   32×32 is a common standard and allows more variation in small spaces.
   64×64 has more detail but takes up more screen space.
   Your character (128×128) would occupy 4×4 tiles at 32×32 or 2×2 tiles at 64×64.
   Stick with a power-of-two scale (16, 32, 64) for better performance and compatibility.
2. Background Assets (Sky, Mountains, Decorations, etc.)
   Recommended size: 256×256, 512×512, or even larger
   Why?
   Backgrounds should be bigger than the character to avoid looking too detailed and cluttered.
   For parallax layers, use large, less detailed elements.
   Sky and faraway mountains can be 512×512 or larger, since they don’t repeat as often.
3. Foreground & Props (Crates, Bushes, Signs, etc.)
   Recommended size: 64×64 or 128×128
   Why?
   Props should match the character’s proportions while keeping enough detail.
   A 64×64 crate would be half the character's height, making it a reasonable obstacle.
   A 128×128 bush could fully hide the character, useful for stealth or hiding spots.
4. Enemies & NPCs
   Recommended size: 64×64 to 128×128
   Why?
   Small enemies (e.g., a rat) could be 64×64.
   Medium enemies (e.g., another dog or humanoid enemy) can match the character at 128×128.
   Bosses can be larger (256×256 or more) for a dramatic presence.
5. UI Elements (Buttons, Icons, HUD)
   Recommended size: 16×16, 32×32, or 64×64
   Why?
   Keep UI elements smaller and readable.
   A 64×64 button works well for an on-screen menu.
   A 32×32 coin icon is clear without taking too much space.

Conclusion (Best Overall Scaling)
Asset Type Recommended Size
Tiles (Ground, Walls, Platforms) 32×32 or 64×64
Backgrounds 256×256 or 512×512
Props (Bushes, Crates, Signs, etc.) 64×64 or 128×128
Enemies/NPCs 64×64 to 128×128
Bosses 256×256 or more
UI Elements (Icons, Buttons, HUD) 16×16 to 64×64

SIZES SETTLED: Milo - 128x128, Tilemap size: 32x32

Main character is twice the size of the tilemap (moreso in length) - wich expresses confidence

Avoid making the player feel floaty

Ground/Air Acceleration + Deceleration
Gravity Controls
Clamped Fall Speed
Jump Cut Controls - how fast you start falling after releasing the jump button
Apex Hang Time
Jump Buffer
Coyote Time

https://www.youtube.com/watch?v=zHSWG05byEc , https://www.youtube.com/watch?v=j1HN7wsFHcY - Perfected movement

randomly generated levels?

https://www.youtube.com/watch?v=Oet5jqoX14E ---- Somewhat perfect roadmap example


scriptable objects
procedural animation
