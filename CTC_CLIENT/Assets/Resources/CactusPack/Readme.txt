This is mainly a prop pack so usage is pretty straightforward. Drop in assets in your scene from Prefabs folder and add/remove components according to your project and preferences.

Scene also contains bonus shader that blends two textures based on vertex color and greyscale blendmask (/Shaders/VertexBlend.shader) Tested with forward rendering only so far. You'll have to rely on 3D app to paint vertex colors or use custom unity extension.


Version 1.2
	- Fixed axis orientation issues (mesh components lying sideways in preview window). Now assets are compatible with terrain engine or anything that spawns them dynamically
	
Version 1.3
	- Added /Prefabs/Sprites/. These are same cactus and rock assets, but rendered on flat geometry as sprites. Can be strategically placed in backgrounds to reduce polycount of scene or even used independently for of 2.5D projects or sidescrolling games.
	- Modified blend shader. Blend mask is now driven by separate map instead of alpha channel of diffuse. You can still access old one - it was renamed to VertexBlend_Old.shader
