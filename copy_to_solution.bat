@echo off
setlocal

set "SRC=D:\#M\PG_DATA\evo_新規開発課\Original-Sticky-App\製造\StickyNoteApp"
set "DEST=C:\Users\h.nema\source\repos\StickyNoteApp\StickyNoteApp"

xcopy "%SRC%\*" "%DEST%\" /E /I /H /Y

endlocal
