How to Import This Into Your Unity Project :
    - Download the files
    - Open Your Project in unity
    - Drag and drop the UnityLineOfSight2D folder (the one you just downloaded)
      from your file explorer into the asset folder in your unity project

How to Setup the Script in Your Game For a Tilemap (Using Recogmended Settings) :
    - Attach the LignOfSight Script (located in the UnityLineOfSight2D folder)
      to your main camera
    - Set the Mat varable to the material you want the area your player cant see
      to use (I would recogmend the one used in the DemoScene folder)
    - Set the Cam varable the the camera the scrpit is attached to.
    - Set the ViewPoint varable to the Transform component of the main camera.
    - Set the MaxDistanceToRender to roughly 2.5 multiplied by your camera size.
      This can be increaced with no visual difference but it may make your game run
      slightly slower depending on how many tiles are in your scene.
    - Run the game to see if it works. If it does not then reffer to the demo scene
      provided for how you could set things up.
    
    - Please note that changing some of these inspector varables during run time may
      not work. Also, if you want to edit your tilemap during run time you will have
      to call the UpdateTileMap function in your LineOfSight script for the changes
      to take effect. Calling this function will remove any LignOfSightObjects that
      are not generated from the tile map. As a workaround to this you could either
      have two LinOfSight scripts on your main camera (one for the tile map and one
      for other) or you could remake the other LignOfSightObjects every time you call
      the UpdateTileMap function. But if you are only using this script for a tile map
      then dont worry it sould work fine.

Usign the LignOfSight Script for Things Other Than A Tile Map :
    - In the Start function of the LignOfSight Script there is a piece of code (commented 
      out) to show you how to create your own LignOfSightObject. Feel free to adapt this
      to fit your needs.

If you get stuck, need help or want to ask a question then contact: christopher-d-lang@outlook.com