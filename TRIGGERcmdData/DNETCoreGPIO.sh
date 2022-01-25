 #!/bin/sh
# Place this in ~
# chmod +x DNETCoreGPIO.sh
# DNETCoreGPIO location in PATH as is ~
# espeak: https://www.dexterindustries.com/howto/make-your-raspberry-pi-speak/
DNETCoreGPIO $1
if [ "$1" -eq 14 ] ; then
    espeak -f /tmp/temperature.txt 2>/dev/null
fi