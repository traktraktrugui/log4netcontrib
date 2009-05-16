rem builds using nant defaults to not running tests
@echo off
echo building log4netContrib
nant "-D:runtests=false"
pause
@echo on
