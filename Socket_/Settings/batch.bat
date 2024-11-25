@echo off

REM Comando para copiar el archivo ejecutable a la carpeta de inicio de Windows
copy "C:\ProgramData\Homini\ValidaCedula\Socket\Socket.lnk" "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\" /Y
REM Pausa para ver el resultado
