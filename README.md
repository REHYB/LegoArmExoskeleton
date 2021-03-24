# LegoArmExoskeleton

This repository holds the code, logs and other relevant materials for the LEGO arm exoskeleton designed and developed by students at the Technical University of Denmark as part of the [ReHyb](https://rehyb.eu/) project.

The system consists of a low-cost 2 DOF upper-limb exoskeleton made out of LEGO and the software system which makes it possible that this hardware interactively communicates with Unity, is able to log the user's movements and provide supportive or nudging movements.

On the [Wiki pages](https://github.com/REHYB/LegoArmExoskeleton/wiki) tutorials can be found on how to set up the software system. Here, a short introduction to the construction is presented.

The system has been used as part of apparatus for master thesis projects and special courses at the Technical University of Denmark in the field of neurorehabilitation. The exoskeleton acted as feedback device or controller to Unity scenes or games. Some of the simple Raspberry Pi scripts for these projects has also been uploaded here to serve as inspiration. You can find them [in this folder](https://github.com/REHYB/LegoArmExoskeleton/tree/main/example_code).

## Setup

### Lego setup
In order to set up the arm exokeleton you need to assemble the Lego hardware. The [manual](https://github.com/REHYB/LegoArmExoskeleton/blob/main/lego_model/LEGO%20Arm%20Exoskeleton%20Mk1%20manual.pdf) can be found in the repo as a PDF document. 

For additional information and the specific needed brick list please open the [model folder](https://github.com/REHYB/LegoArmExoskeleton/tree/main/lego_model). Here you can find 3D models of the exoskeleton which can be opened and edited with the [BrickLink Studio](https://www.bricklink.com/v3/studio/download.page) program.

Furthermore, there is an [XML file with the needed parts](https://github.com/REHYB/LegoArmExoskeleton/blob/main/lego_model/LEGO%20Arm%20Exoskeleton%20Mk1.xml). This XML contains **ALL** of the needed LEGO parts (including EV3 Large Servo motors and the Mindstorms cables). This XML file can be imported into one of the known LEGO brick database websites ([Bricklink](https://www.bricklink.com/) or [BrickOwl](https://www.brickowl.com/)) as wanted set-list and therefore can be ordered.

### Powering up
To power up the system it needs a 9-12V battery. BrickPi's solution is having 8 AA batteries which are sitting in a battery rack. However the motors are draining the battery quite fast, so a more robust solution fits better. **We recommend this [XTPower powerbank](https://www.xtpower.de/XT-16000QC3-PowerBank-modern-DC-/-USB-battery-with-15600mAh-up-to-24V) which has DC output out of the box and can provide 12V output which allows the motors to function fast and strong enough.**

### Raspberry Pi setup
The exoskeleton is powered by a Raspberry Pi extended by a [BrickPi](https://www.dexterindustries.com/brickpi/) extension board which provides the interface to the Lego EV3 sensors. Follow the instructions on BrickPi's website to assemble the acryllic case. The case can be attached to the right side of the model at the upper-arm part. (NB: This orientation can be changed by restructuring the exoskeleton a little bit)

For the software setup, also follow the corresponding [BrickPi documentation](https://www.dexterindustries.com/BrickPi/brickpi-tutorials-documentation/getting-started/pi-prep/).

In order to use the wireless communication feature of the exoskeleton, the Raspberry Pi needs to be connected to the same network as the host on which the Unity program runs. For connecting the Raspberry Pi to a wireless network please see [the official documentation](https://www.raspberrypi.org/documentation/configuration/wireless/wireless-cli.md).

### Network setup
1. Give the **server** (the computer which is running the Unity program) and the **Raspberry Pi** a static IP (usually in the admin interface of your router)
2. The python program by default is set up as the following:
  * SERVER_IP = '192.168.0.69'
  * SERVER_PORT = 5013
  * CLIENT_IP = '192.168.0.4'
  * CLIENT_PORT = 5011
3. If you wish to change these settings you can do so in the python program
4. **Make sure that in Unity the ports and IP addresses are the same**

### Exoskeleton hardware setup
The recommended way of putting the exoskeleton on the arm is the following:
1. Open the wrist lock the widest position and lock it. (To unlock the wrist lock move the black gears to the center of the circle. To lock it, move the black gears back to the linear gear, that way locking it in. See pictures)
2. Bend the exoskeleton at the elbow joint so the upper-arm part is bended over the wrist part.
3. Slowly move the hand through the wrist cuff.
4. Unlock the wrist cuff, tighten it gently, but well to the user's wrist. The cuff should be located a bit above the wrist bones.
5. Bend the upper-arm part on the user's arm. Make sure that the elbow joint gears are sitting right at the elbow and are aligned.
6. Use the velcro straps to tighten the structure to the arm.

<img src="https://raw.githubusercontent.com/REHYB/LegoArmExoskeleton/main/pictures/velcro.png" alt="Velcro" width="400"/>
<img src="https://raw.githubusercontent.com/REHYB/LegoArmExoskeleton/main/pictures/locking_mechanism.jpg" alt="Locking mechanism" width="400"/>
