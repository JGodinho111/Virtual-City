# Virtual-City
Prototype 3D City Game
- After donwload install the following assets (from the Unity Asset Store) using Package Manager:
    -   Sleek essential UI pack by F3jry (https://assetstore.unity.com/packages/2d/gui/icons/sleek-essential-ui-pack-170650)
    - SimplePoly City - Low Poly Assets by VenCreations (https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899)
        - If any issues arise regarding materials select them and go to Window > Rendering > Render Pipeline Converter, and tick Material Upgrade, then press "Initialize And Convert".
- Other external assets used:
    - [non copyright music] Lofi Type Beat - imperfect | aesthetic lofi music / Lofiru on Youtube (https://www.youtube.com/watch?v=YTQAjKBNp8g)

- The Prototype Functionalities
    - 3D 30x30 City
        -  Can be moved around (left mouse button) and tiled (left mouse button)
        - Originally all assets were created manually using Unity ProBuilder to test all features
        - Later changed to use external assets to be more visually appealing
        - Playable version also available on itch.io at (https://jgodinho.itch.io/virtual-city-demo)
    - UI made to be intuitive and appealing
        - Button Layout: the buttons are layout elements within a horizontal layout group panel.
        - Zooming in UI buttons when hovered on and the icon fade outs when trying to place an deployable asset, info panel pop-up when hovering over info icon.
    - Deployable assets by clicking on their UI button and dragging to desired spawn location
        - If they are outside the city or in contact with already existing scene items they won't appear
        - If they can spawn, the trees near them are destroyed
        - Each spawned object has a particle effect
    - Animation & Icon to Object Transition
        - Moves object around itself or "falls down" as UI icon fades away.
        - Created in a way that combinations of these animations can be extended with wanted parameter combinations
    -  2 Ranomized Events
        - These events are triggered by a UI button for testing
        - Events spawn physics-based objects which prevent new deployable assets placements
        - Can be removed by moving or tilting the city
        - One event spawns 1 hazard near each existing city item (including spawned ones)
        - The other spawns a multitude of small hazards on the city location
        - Each have unique particle effects
    
    - Other Features:
        - Sounds
            - Simple sound effects were implemented for some game actions
        - Skybox & Background
            - Changed the preset skybox with a new one and added transparent spheres in the far background to appear as halos (my take on the halo-like effect seen in the example)
        - Simple car driving around 4 set points on the road for aesthetics
        - Simple actual game structure (place as many buildings in the city within 30 seconds, while events happen every 6 to see what's the highest you can get)
    - Future Iteration Possibilities:
        - Better game structure (e.g. place as many buildings in the city without destroying trees within a set ammount of time, while events happen frequently and need to be dealt with made modularly so there could be levels, more requirements, etc)
        - Color change of City while being held
        - Object Rotation to confirm drop
        - More complex events
        - Pop Up Notifications of Events
        - Working traffic system
        - Civilian NPCs (with set movement/random movement, based on NavMeshes for example)
        - Camera Shake on City Grab (not relevant for XR)
        - Additional camera movement (not relevant for XR)
     
- Unity Scene Descriptions
    - The Sample Scene (where I created and tested most of the features before changing the assets & has already changed UI).
    - TEST CHALLENGE SCENE: The Scene With New Assets which has all the requested features plus the car driving around four map points.
    - The Gameplay Scene which has everything previously created with simple game logic. This is the version available on itch.io (https://jgodinho.itch.io/virtual-city-demo).

- Code Structure
    - *Generate Input Action Map C# Class in Unity Editor

    - CameraControlls.cs handles camera movement (not relevant for XR)

    - UIPanelPositionChange.cs changes the dimensions of the layout group panel where buttons are when entering and exiting the layout group panel
    - UIButtonHoverZoom.cs zooms the inner image of a button on hover

    - DragNDropPlacer.cs handles deployable object placement including fading out the UI image, checking if placement is possible and instantiating gameobject if so (via input action, so with an XR binding there should not be an issue)
        - UIButtonPlacementAction.cs calls DragNDropPlacer.cs to StartGameObjectPlacement given a deployable object prefab and a deployable object image prefab

    - ImageFollowMouseCursor.cs is part of deployable image prefab and follows the player mouse position (via input action, so with an XR binding there should not be an issue)
    - ObjectStartAnimation.cs on startup plays an animation and deploys particle effects
        - VerticalAnimation.cs, SidewaysAnimation.cs and ComingDownAnimation.cs extend ObjectStartAnimation.cs and are part of deployable object prefab and set the type of animation based on 3 parameters.

    - CityMover.cs handles city movement based on left or right mouse clicks and position (via input action, so with an XR binding there should not be an issue) via its rigidbody
        - UIBlockDetector.cs is called by CityMover.cs to verify that the pointer is not on a UI item (via input action, so with an XR binding there should not be an issue)

    - ElementalEffectsPlacer.cs generates the random physics-based events
        - HazardItem.cs is the class that is extended to handle random event gameobjects
            - TrashItem.cs and SnowItem.cs extend HazardItem.cs to handle the random event gameobjects' own destruction (when too far away from the city)
    
    - UIInfoHover.cs simply displays a UI panel on hover and hides when exiting
    
    - SoundManager.cs is a singleton that handles playing sounds using a queue of audio sources.
        - SoundLibrary.cs is a scriptable object that holds all sounds in the game
            - Sound.cs defines sounds as an audioClip, volume, pitch, loop and id to later be played by the SoundManager.cs

    - Extra Features
        - SimpleCarMover.cs hanldes moving a car across 4 preset points in the scene (which move with the city)
        - GameManager.cs handles the game logic for the GameplayScene
            - GameSceneReloader.cs is used by the end screen to reload the scene

- Proposed XR Scene Implementation Changes
    - Add needed XR Packages and Input Action bindings to action map
    -------
    - Have a button that is available after plane detector detects ground
    - Button spawns city with the y above the real-world ground
        - OR
    - After ground detection simply show the city (maybe each city is a level)
    -------
    - Start the game