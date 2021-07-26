# Eco Bulletin Board Mod
A server mod for Eco 9.3 that allows citizens to publish bulletins to a public bulletin board found on the Ecopedia.

## Installation

- Copy the `EcoBulletinBoardMod.dll` file to `Mods` folder of the dedicated server.
- Restart the server.

## Usage

Bulletins must be published to a Bulletin Channel. Bulletin Channels must be created by admins and can't be created or edited by players. Each Bulletin Channel gets a tab in the Ecopedia, this is where all the bulletins for a channel are found.

### Configuring Bulletin Channels

To create a new Bulletin Channel, use:
`/objects add bulletin channel`

By default the channel will have a name like "Bulletin Channel 1". To change it, use:
`/objects edit bulletin channels,{id}` (the id can be found by hovering over the tagged Bulletin Channel in chat)

With the object edit UI, you can change the following properties of the Bulletin Channel:
- Name - as well as the name of the channel object, this will affect the associated tab in the Ecopedia
- Allowed to Publish - who are allowed to publish bulletins to the channel (this can be a specific player, title or demographic)

TODO - experiment with adding Ecopedia xml files to customise where the Bulletin Channel tabs show up, document findings here

### Publishing Bulletins

To publish a bulletin to a Bulletin Channel, follow these steps:
1. Place a sign somewhere in the world (the material and type doesn't matter).
2. Write the message to publish onto the sign. The usual formatting (color tags, icons etc) will work, but don't expect the text wrapping to behave the same way when published.
3. Aim at the sign and run the chat command `/bulletins publish`.
4. A popup will appear asking which Bulletin Channel to publish to. Only channels that the player is allowed to publish to will show up. Select one and click "OK".
5. The bulletin will be published immediately and a notice sent out to everyone. The sign can safely be edited or removed.

### Removing/Editing Bulletins

Currently only admins can remove or edit bulletins. Bulletins can be manipulated using the objects command in the same way as the Bulletin Channels.

`/objects edit bulletins,{id}` (the id can be found by hovering over the tagged bulletin in chat)

`/objects remove bulletins,{id}` (the id can be found by hovering over the tagged bulletin in chat)

Any changes or removals will take immediate effect.


## Building Mod from Source

### Windows

1. Login to the [Eco Website](https://play.eco/) and download the latest modkit
2. Extract the modkit and copy the dlls from `ReferenceAssemblies` to `eco-dlls` in the root directory (create the folder if it doesn't exist)
3. Open `EcoBulletinBoardMod.sln` in Visual Studio 2019
4. Build the `EcoBulletinBoardMod` project in Visual Studio
5. Find the artifact in `EcoBulletinBoardMod\bin\{Debug|Release}\netcoreapp3.1`

### Linux

1. Run `MODKIT_VERSION="0.9.3.4-beta" fetch-eco-reference-assemblies.sh` (change the modkit version as needed)
2. Enter the `EcoBulletinBoardMod` directory and run:
`dotnet restore`
`dotnet build`
3. Find the artifact in `EcoBulletinBoardMod/bin/{Debug|Release}/netcoreapp3.1`

## License
[MIT](https://choosealicense.com/licenses/mit/)