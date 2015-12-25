@echo Building JavaClass...
javac -classpath "C:\Program Files\Unity4\Editor\Data\PlaybackEngines\androidplayer\bin\classes.jar";"..\comscore.jar" -bootclasspath "%ANDROID_SDK_ROOT%\platforms\android-8\android.jar" MaleficentActivity.java -d .

@echo Signature dump of JavaClass...
javap -s com.disney.maleficent.MaleficentActivity

@echo Creating JavaClass.jar...
jar cvfM ../maleficentandroid.jar com/

@echo Cleaning up
rd com /s/q

pause