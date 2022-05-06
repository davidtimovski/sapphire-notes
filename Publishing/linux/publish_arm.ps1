$Version = "0.8.1"

cd "C:\Users\david\source\repos\sapphire-notes\Publishing\linux"

New-Item -Path . -Name "output\sapphire-notes_${Version}_armhf" -ItemType "directory"
Copy-Item -Path "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\template_arm\*" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_armhf\" -Recurse

Copy-Item "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\template_arm\DEBIAN\control" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_armhf\DEBIAN\control"

Copy-Item "C:\Users\david\source\repos\sapphire-notes\Src\SapphireNotes\bin\Release\net6.0\publish\SapphireNotes" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_armhf\usr\bin\sapphire-notes"
