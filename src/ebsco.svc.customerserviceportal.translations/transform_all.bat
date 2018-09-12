@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

echo executing transform_all

set directory=%1
set exe_path=%2

attrib -r  %directory%*.*

dir %directory%*.tt /b /s > t4list.txt

echo the following T4 templates will be transformed:
type t4list.txt

for /f "tokens=* delims= usebackq" %%d in (t4list.txt) do (
set file_name=%%d
set file_name=!file_name:~0,-3!.cs
!exe_path!TextTransform.exe -out "!file_name!" "%%d"
)

echo transformation complete