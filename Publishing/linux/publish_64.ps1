$Version = "0.8.2"

cd "C:\Users\david\source\repos\sapphire-notes\Publishing\linux"

New-Item -Path . -Name "output\sapphire-notes_${Version}_amd64" -ItemType "directory"
Copy-Item -Path "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\template_64\*" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_amd64\" -Recurse

Copy-Item "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\template_64\DEBIAN\control" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_amd64\DEBIAN\control"

Copy-Item "C:\Users\david\source\repos\sapphire-notes\Src\SapphireNotes\bin\Release\net6.0\publish\SapphireNotes" -Destination "C:\Users\david\source\repos\sapphire-notes\Publishing\linux\output\sapphire-notes_${Version}_amd64\usr\bin\sapphire-notes"
