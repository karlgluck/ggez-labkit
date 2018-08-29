@echo off

REM This file uses 7-Zip to extract CodeFormatter.zip file into a temporary
REM directory. It then runs formatting and deletes the unzipped directory.
REM This is so that new versions of CodeFormatter can be dropped in and only
REM the one binary zip ever needs to be updated.

if not exist "C:\Program Files\7-Zip\7z.exe" (
    echo Auto-formatter requires 7-Zip to be installed
    echo Download it from here ^(opened in web browser^): https://7-zip.org
    start https://7-zip.org
    pause
    exit
)

if not exist "CodeFormatter.zip" (
    echo Auto-formatter requires CodeFormatter.zip in %cd%
    echo Download it from here ^(opened in web browser^): https://github.com/dotnet/codeformatter/releases
    start https://github.com/dotnet/codeformatter/releases
    pause
    exit
)


REM Extract the code formatter
if exist CodeFormatter rmdir /S /Q CodeFormatter
"C:\Program Files\7-Zip\7z.exe" x CodeFormatter.zip



REM Find all files to be formatted
REM I did this because I can't figure out how to ask CodeFormatter to exclude files...

REM 1: find all the CS files except those that are part of FullSerializer or LZF
dir "../ggez-labkit-unity-project/Assets/" /s/b | findstr /e ".cs" | findstr /v /c:"LZF" /c:"FullSerializer" > files_to_format.rsp

REM 2: Quote each of the lines to take care of spaces
for /F "tokens=*" %%i in (files_to_format.rsp) do echo "%%i" >> files_to_format_quoted.rsp

REM 3: Move the quoted file back on top of the original one
move files_to_format_quoted.rsp files_to_format.rsp



REM Run the formatter
".\CodeFormatter\CodeFormatter.exe" /copyright:copyright.txt /c:UNITY_EDITOR,UNITY_EDITOR_WIN /c:UNITY_EDITOR,UNITY_EDITOR_OSX /verbose files_to_format.rsp


REM Clean up temporary files
rd /s /q CodeFormatter
del files_to_format.rsp

pause
