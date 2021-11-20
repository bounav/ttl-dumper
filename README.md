# ttl-dumper
Command line utility to help read (and write) to (and from) a TTL memory chip (e.g. asynchronous SRAM)

This program is intended to be published to a [Raspberry Pi](https://www.raspberrypi.com/products/) computer.

The Pi is expected to have an [IO Plus hat](https://www.abelectronics.co.uk/p/54/io-pi-plus) on.
This hat features two MCP23017 IO expanders IC from Microchip.

The program, written in C# leverages the `System.Device.I2c` API in the .NET core framework to interact with the hat and set and read TTL levels on its IO pins.

## Configuration

The `appsettings.json` configuration file defines how the IO pins are used.

The default configuration targets the `LC35256D` 256k SRAM chip made by `Sanyo`.

The program can set a 15 bit address, control the chip enable/read/write pins and read or write a byte on the data pins.

## Limitations

The `IO Plus` hat only as 32 pins, therefore limiting how much can be _addressed_ and _read_ from a chip.
That particular hat is designed to be stacked with others to expand the number of IOs. This expandiiblity could be used to interact with chips of higher capacity (requiring more address pins for example).

## Deployment

You can clone this repo and build the project on a raspberry pi _directly_ but it might not be the most convient setup.

You might prefer developping on another computer, push the program to the pi via SSH and run the program remotely, or even debug remotely with [Visual Studio Code](https://code.visualstudio.com/).

To build the project and copy it to your raspberry pi (assuming you have SSH authentication already setup), run `push2pi.sh` in the `TtlDumper` folder. This will run `donet publish` then copy the files to a folder on your `pi`.

```bash
# Go to the TtlDumper folder
cd TtlDumper
# Open vscode
code .
# Build and publish to the pi via SSH
./push2pi.sh
```

To debug the program remotely via SSH in vscode, choose the relevant debugging profile and hit`F5` on your keyboard (see `.vscode\launch.json`).

## Usage

The program is compiled into a .DLL rather than an executable to avoid having to worry about which OS/platfrom the program will run on.
The flip side of this is you need to have the `dotnet` run time installed on your Raspberry Pi.

Go in the publised folder and type `dotnet ./TtlDumper.dll` to output the help.

### Example 1:

Turn on LEDs connected to the address bus.

A good idea to make sure things are setup correctly could be to wire LEDs to the address bus and run the `blink-address-bus` command.

```bash
# Should toggle the address levels high then low one after the other
dotnet ./TtlDumper.dll blink-address-bus
```

### Example 2:

Dump the content of the chip to a file:

```bash
# This will read all the values in the chip and dump them in a file named "dump.bin"
dotnet ./TtlDumper.dll read-from-chip dump.bin
```

### Example 3:

Dumps the content of a file to the chip.

_**WORK IN PROGRESS**_ do this at your own peril.

```bash
# This will copy the `dump.bin` file byte by byte on to the chip
dotnet ./TtlDumper.dll read-from-chip dump.bin
```

## Remarks

Project status: `work in progress`.

I got as far as testing reading data from an SRAM chip.

## Disclaimer

This project involves hacking circuits and dealing with 5V TTL voltages.

You could easily damage chips or circuits and or your rasberry pi!