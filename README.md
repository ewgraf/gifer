# Gifer
Pure gif&images viewer for Windows
Download: https://github.com/ewgraf/Gifer/releases/tag/v1

# Debug Drug&Drop
Если при дебаге картинка не перетаскивается на форму, то нужно запустить студию не от админа а от локального пользователя, т.к. файловая система, из которой перетаскивается файл должна иметь те же права доступа что и программа, либо билдить проект и запускать программу из релиза пользователем, такие дела.

If while Debug the Drag&Drom feature is not working - the cursor is turned into crossed-circle - try to re-run VS as USER, NOT AS ADMIN, as explorer.exe should have same "access level" as gifer.exe, to succeed drug&drop.
