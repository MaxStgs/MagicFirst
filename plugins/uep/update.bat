update.exe -repo MaxStgs/UEP
tar -xf plugin.zip
del plugin.zip
xcopy uep . /S /Y
rmdir uep /S /Q
@PAUSE