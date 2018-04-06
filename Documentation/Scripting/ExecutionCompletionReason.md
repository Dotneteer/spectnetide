# ExecutionCompletionReason enum

The values of this enumeration tells the reason the virtual machine is in paused or in stopped state.

Value | Description
------|------------
`None` | The machine is still executing, or it has not been ever started
`Cancelled` | The execution has explicitly cancelled by the user or by the scripting code
`Timeout` | The specified timeout period expired
`TerminationPointReached` | The virtual machine reached its termintation point specified by its start
`BreakpointReached` | The virtual machine reached a breakpoint during its execution in debug mode
`Halted` | The virtaul machine reached a `HALT` statement and paused
`FrameCompleted` | The virtual machine has just rendered a new screen frame and paused
`Exception` | The virtual machine stopped because of an unexpected exception
