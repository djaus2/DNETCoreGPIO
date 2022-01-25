 #!/bin/sh
# Place this in ~
# chmod +x DNETCoreGPIO.sh
# DNETCoreGPIO location in PATH as is ~
# espeak: https://www.dexterindustries.com/howto/make-your-raspberry-pi-speak/#:~:text=Make%20sure%20your%20Raspberry%20Pi%20is%20powered%20up,to%20convert%20text%20to%20speech%20on%20the%20speakers.
DNETCoreGPIO $1
if [ "$1" -eq 14 ] ; then
    espeak -f /tmp/temperature.txt 2>/dev/null
fi