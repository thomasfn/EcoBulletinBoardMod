# Eco Bulletin Board Mod
A server mod for Eco 9.3 that allows citizens to publish bulletins to a public bulletin board found on the Ecopedia.

## Installation

- Copy the `EcoBulletinBoardMod.dll` file to `Mods` folder of the dedicated server.
- Restart the server.

## Usage

TODO - add usage instructions once the mod's usage surface has beeen finalised

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