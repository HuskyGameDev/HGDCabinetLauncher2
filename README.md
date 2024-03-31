# HGDCabinetLauncher2

This is a launcher for the Husky Game Development arcade cabinet at Michigan Technological University.

## some notes

- This currently uses dotnet 7, which will go EOL in about a month from writing this, please update the used dotnet version among other things when updating or rebuilding this.
- The launcher will expect a "Games" folder in the Desktop directory.
- Each game must have a meta.json file containing at least the name of the game and the name of the executable (including file extension).
- There are many more things that can be in the meta.json, and the sample file contains all of them.
- Game binaries must be in the same directory as the meta.json so the working directory can be properly set.

![example image ](https://raw.githubusercontent.com/Soup-64/HGDCabinetLauncher2/main/samples/IMG_3792.JPG)
