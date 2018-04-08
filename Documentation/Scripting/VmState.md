# VmState enum

The values of this enumeration show the possible states of a ZX Spectrum virtual machine.

Value | Description
------|------------
`None` | The virtual machine has just been created, but has not run yet
`Runnig` | The virtual machine is successfully started in the background
`Pausing` | The pause request has been sent to the virtual machine, now it prepares to get paused
`Paused` | The virtual machine has been paused
`Stopping` | The stop request has been sent to the virtual machine, now it prepares to get stopped
`Stopped` | The virtual machine has been stopped



