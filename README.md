# StalkbotGUI

## Features
+ GUI that let's you toggle most commands on and off
+ Autostarting the discord client when you start the app
+ Minimizing to system tray so that you may be stalked without having that pesky icon in your taskbar
+ Alerts for every command, just name it appropriately and it should play
+ Undo functionality for relevant commands
## Commands
For the sake of convenience <sub><sup>smh</sup></sub>, assume your prefix would be `p! [camIndex]`.\
Arguments in `<>` are required, those in `[]` are optional.
+ Webcam => This command is split into 3 "modes"
    + Just taking a picture: After a set delay a picture will be captured and uploaded. `p!webcam [camIndex]`
    + Recording a gif: After a set delay a gif will be recorded (length can be set in config). `p!webcamgif [camIndex]`
    + Listing webcams: Lists the available webcams and their indices. `p!webcams`
+ Screenshot
    + Takes a screenshot of all monitors and optionally blurs it. `p!screenshot`
+ Play
    + Takes files, urls, or youtube links and plays it for the user. `p!play [url or file]`
+ TTS
    + Uses the Windows built-in TTS Engine to tell the user something. `p!tts <text>`
+ Processes
    + Lists your 15 most RAM-heavy processes with their uptimes. `p!processes`
+ Clipboard
    + Either posts what you have currently in your clipboard, or sets it to whatever the command was called with. `p!clipboard [content]`
+ Folder
    + Sends a random (or specific) file from the configured folder (if it's below discord 8mb size limit). `p!folder [search]`
