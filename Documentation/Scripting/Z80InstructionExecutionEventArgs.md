# Z80InstructionExecutionEventArgs class

This class provides event arguments the the `OperationExecuting` and `OperationExecuted` 
events of the [`CpuZ80`](CpuZ80) class.

### PcBefore

```CSharp
public ushort PcBefore { get; }
```

The value of PC before the execution.

### Instruction

```CSharp
public IList<byte> Instruction { get; }
```

The opcode bytes available at the time of event. For example, if the operation is `LD A,(IX+2)`,
this list contains `#DD` and `#7E` for the `OperationExecuting` event, but `#DD`, `#7E`, 
and `#02` for the `OperationExecuted` event.

### OpCode

```CSharp
public byte OpCode { get; }
```

The main operation code. For example, if the operation is `LD A,(IX+2)`, this value is `#7E` from
the entire operation opcode (`#DD` `#7E` `#02`).

### PcAfter

```CSharp
public ushort? PcAfter { get; }
```

The value of PC after the operation has been executed. It is null during `OerationExecuting`.