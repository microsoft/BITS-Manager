REM Make the installer programing using the WiX Toolset.
REM The version 3.11 was the most recent when this sample was made.

set WIXDIR="c:\Program Files (x86)\WiX Toolset v3.11\bin"
%WIXDIR%\candle.exe BITSManager.wxs
%WIXDIR%\light.exe BITSManager.wixobj
del BITSManager.wixobj
del BITSManager.wixpdb