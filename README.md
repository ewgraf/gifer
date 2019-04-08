# Gifer
Gifer is a .gif & image viewer (.gif, .bmp, .jp[e]g, .png) for Windows.

Features: borderless, draggable, drag&drop, frame-by-frame view, standalone (single gifer.exe, no installation required).
Download⬇️: https://github.com/ewgraf/gifer/releases/download/3/gifer.exe .

<img src="https://github.com/ewgraf/gifer/blob/master/help.png?raw=true">

Release: https://github.com/ewgraf/gifer/releases/tag/v3 .

Demonstration: https://youtu.be/zL0Q6VszwBA (please turn on subtitles {hotkey [C]} to see description).

Requirements: Windows Vista?/7/8/10 x64, [.Net Framework 4.5+](https://www.microsoft.com/ru-ru/download/details.aspx?id=30653).

If you've met any trouble with gifer.exe, please feel free to contact me ewgraf@mail.ru. I would be glad to help you, an investigation of user experience is the point.

horo.gif (a.k.a. lena.bmp).

<img src="https://github.com/ewgraf/gifer/blob/master/horo.gif?raw=true" width="450px">

# Debug
EN
If while Debug the Drag&Drom feature is not working - the cursor is turned into crossed-circle - try to restart Visual Studio as USER, NOT AS ADMIN, as explorer.exe should have same "access level" or smth as gifer.exe, to succeed drug&drop.

RU
Если при дебаге в Visual Studio картинка не перетаскивается Drag&Drop'ом на форму, то нужно запустить студию НЕ от админа а от локального пользователя, т.к. процесс explorer.exe должн иметь те же права доступа что и программа, либо билдить проект и запускать программу из релиза пользователем, такие дела.
