#!/usr/bin/env python
from __future__ import print_function # use python 3 syntax but make it compatible with python 2
from __future__ import division

import brickpi3 # import the BrickPi3 drivers
import socket # import the socket library to communicate using UDP protocol
import threading # using threading to listen to UDP messages
import time

BP = brickpi3.BrickPi3() # Create an instance of the BrickPi3 class. BP will be the BrickPi3 object.
BP.set_led(0) # For indicating succesful setup, we turn off the LED here at the beginning and turn it on if the system is succesfully set up

print("Setting up UDP connection...")

# Unity program
SERVER_IP = '192.168.0.101'
SERVER_PORT = 5013

# Motors
CLIENT_IP = '192.168.0.200'
CLIENT_PORT = 5011

# Create the socket
sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP

# Bind to the socket
sock.bind((CLIENT_IP, CLIENT_PORT))

print("UDP connection set up!")

# print("Sending test UDP message...")
# sock.sendto(str.encode("from pi"), (SERVER_IP, SERVER_PORT))

def moveMotor():
    target = BP.get_motor_encoder(BP.PORT_B) + 90 # Read the position of the motor and add 90 degrees, so in the next line it will be moved by 90 degrees
    BP.set_motor_position(BP.PORT_B, target)
    time.sleep(0.5) # Wait for the movement to be over and then set the motor to float again, so it can be rotated by hand.
    BP.set_motor_power(BP.PORT_B, BP.MOTOR_FLOAT)

def messageCallback(data):
    message = data.decode("utf-8")
    print(message)

    if message == 'move':
        moveMotor()
    
def receiveMsg():
    while True:
        data, addr = sock.recvfrom(1024) # buffer size is 1024 bytes
        if (addr[0] == SERVER_IP):
            messageCallback(data)

def main():
    try:
        try:
            BP.offset_motor_encoder(BP.PORT_B, BP.get_motor_encoder(BP.PORT_B))
        except IOError as error:
            print(error)

        # Starting a thread which is listening to the UDP messages until the program is closed
        receiveMsgThread = threading.Thread(target=receiveMsg, args=())
        receiveMsgThread.daemon = True # Making the Thread daemon, so it stops when the main program has quit
        receiveMsgThread.start()

        BP.set_led(100) # Light up the LED to show the setup is successful
        print("Main loop running...")
    
        while True:       
            value_motor = BP.get_motor_encoder(BP.PORT_B) # get the position of the motor
            value_motor = str(value_motor) # turn it into a string, so it can be sent through UDP

            udp_message = str.encode(f"{value_motor}")
            sock.sendto(udp_message, (SERVER_IP, SERVER_PORT))
            time.sleep(0.01) # Without sleep the system logs and sends data to the server ~820 times per second (820 Hz)

    except KeyboardInterrupt: # except the program gets interrupted by Ctrl+C on the keyboard.
        sock.close() # close the UDP socket
        print('\nsocket closed')
        BP.reset_all()        # Unconfigure the sensors, disable the motors, and restore the LED to the control of the BrickPi3 firmware.
        print('program stopped')

if __name__ == "__main__":
    main()
