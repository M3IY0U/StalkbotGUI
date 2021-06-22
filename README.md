# StalkbotGUI
[![forthebadge](https://forthebadge.com/images/badges/thats-how-they-get-you.svg)](https://forthebadge.com) \
[![CodeFactor](https://www.codefactor.io/repository/github/m3iy0u/stalkbotgui/badge)](https://www.codefactor.io/repository/github/m3iy0u/stalkbotgui)

## Features
+ GUI that let's you toggle most commands on and off
+ Autostarting the discord client when you start the app
+ Minimizing to system tray so that you may be stalked without having that pesky icon in your taskbar
+ Alerts for every command, just name it appropriately and it should play
+ Undo functionality for relevant commands

## Installation
1. Create an Application in the [Discord Developer Portal](https://discord.com/developers/applications/) and add a Bot to it
2. Download the latest release from the [Releases](https://github.com/M3IY0U/StalkbotGUI/releases) tab
3. Extract the folder somewhere and start the executable
4. Obtain your token from the Bot tab in the Developer Portal
4. Configure your token and prefix in the settings (don't forget to close the config window to save them)
5. Restart the app
6. Invite your bot to a server by clicking on OAuth2 -> Check `bot` -> Copy Link into Webbrowser of choice
7. Profit


## Commands
For the sake of convenience <sub><sup>smh</sup></sub>, assume your prefix would be `p!`.\
Arguments in `<>` are required, those in `[]` are optional.
+ Webcam => This command is split into 3 "modes"
    + Just taking a picture: After a set delay a picture will be captured and uploaded. `p!webcam [camIndex]`
    + Recording a gif: After a set delay a gif will be recorded (length can be set in config). `p!webcamgif`
    + Listing webcams: Lists the available webcams and their indices. `p!webcams`
+ Screenshot
    + Takes a screenshot of all monitors and optionally blurs it. `p!screenshot`
+ Play
    + Takes files, urls, or youtube links and plays it for the user. `p!play [url or file] [start time in sec]`
+ TTS
    + Uses the Windows built-in TTS Engine to tell the user something. `p!tts <text>`
+ Processes
    + Lists your 15 most RAM-heavy processes with their uptimes. `p!processes`
+ Microphone
    + Records a configurable clip from your microphone (length and delay configurable). `p!mic [sampleRate]`
+ Folder
    + Sends a random (or specific) file from the configured folder (if it's below discord 8mb size limit). `p!folder [search]`

## Stuff you can (and can't) configure
### CONFIGURABLE
+ Which webcam will be used
+ The webcams resolution
+ The delay after the webcam starts recording
+ The length of the webcam gif
+ The original resolution of the gif
+ Whether the gif has a constant framerate
+ The amount of blur on a screenshot
+ The maximum duration for Play/TTS
+ The source for the folder command
+ The bot token and prefix (might require restart)
\
### NOT CONFIGURABLE (yet):
+ Different timeouts for tts and play
+ Probably some more stuff that i don't remember right now

## Remarks
+ Everything the bot needs should be downloaded/created on first startup
+ Config gets saved on closing the window, there is no save button
+ While the GUI should theoretically be able to do everything, you can still adjust settings via the generated `config.json` file like in the previous version of the bot, you just might need to restart it.
+ The play command uses youtube-dl for downloading, so everything that can be downloaded with that _should_ work
+ The webcam gif command is more experimental than it should be. Some possible issues:
    + Memory usage might rise significantly when using this command, but should settle down after a while
    + Gifs might be too large for Discord, aka above 8mb in size (although they should be scaled to avoid that) 

## Alternatives
For when you are not using the windows 
+ [Stalkbot-Rewrite](https://gitlab.com/Jerrynicki/stalkbot-rewrite)
+ [Watchdog](https://github.com/TheLastZombie/Watchdog)
