for %%f in (*.nuspec) do (
	echo %%~nf
	NuGet pack "%%~nf.nuspec"
)
pause