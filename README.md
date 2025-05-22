# Virtual-City
Prototype 3D City Game
- After donwload install the following assets (from the Unity Asset Store) using Package Manager:
    -   Sleek essential UI pack by F3jry (https://assetstore.unity.com/packages/2d/gui/icons/sleek-essential-ui-pack-170650)

- The Prototype Functionalities
    - 3D City that can be moved around (left mouse button) and tiled (left mouse button)
    - Deployable assets by clicking on their UI button and dragging to desired spawn location
        - If they are outside the city or in contact with already existing scene items they won't appear
        - If they can spawn, the trees near them are destroyed
        - Each spawned object has a particle effect
    -  2 Ranomized Events
        - These events are triggered by a UI button for testing
        - Events spawn physics-based objects which prevent new deployable assets placements
        - Can be removed by moving or tilting the city
        - One event spawns 1 hazard near each existing city item (including spawned ones)
        - The other spawns a multitude of small hazards on the city location
        - Each have unique particle effects
    - Sounds
        - Simple sound effects were implemented for some game actions
    - Skybox & Background
        - Changed the preset skybox with a new one and added transparent spheres in the far background to appear as halos (my take on the halo-like effect seen in the example)
    - UI made to be intuitive and appealing
        - Zooming in UI buttons when hovered on and the icon fade outs when trying to place an deployable asset are good examples.
     