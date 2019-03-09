---
layout: documents
categories: 
  - "Unit Testing"
title:  "The Testing Process"
alias: unit-testing-process
seqno: 40
selector: documents
permalink: "documents/unit-testing-process"
---

# The Testing Process

You already learned that tests are organized into a hierarchy of _test files_, _test sets_, _tests_, and parameterized
_test cases_. In this section you will learn how the test engine manages these tests.

The testing process start with the compilation of tests. During this step, all tests are turned into execution plans.
If there is any compilation error, the engine won't start running the tests.

The unit of execution is the __test set__. Test sets are entirelly independent of each other. You can run them in any
sequence; the order won't change whether they fail or succeed. SpectNetIde loops through all test files declared in the
project (you cannot assume deterministic order), and runs all test sets in their declaration order within a file.

Each test sets creates and starts a Spectrum virtual machine, and pauses it when the machine reaches its main execution 
cycle. The point a machine is paused depends on its type (different for Spectrum 48K, 128K, +3E), and also the mode the machine
runs. For example, in a Spectrum 128K machine you can run tests in BASIC 128 mode or in Spectrum 48K mode.

> Right now, the unit test engine supports only Spectrum 48K mode, but soon you will be able to use the other modes, too.

## Running a Test Set

Every test within a test set uses this virtual machine. This machine is paused every time a test completes 
(either successfully, with a failure, or in aborted state). _If a test disrupts the machine's memory (for example, it changes
the code being tested), the behavior of a particular test may prevent the subsequent test from running properly._ This demeanor
of test sets is intentional.

The engine uses these steps to run the tests defined within a tes set:

1. After the Spectrum machine reaches its startup state and pauses, the engine copies the compiled source code into
the memory.
2. If the test set declares an `init` section, the engine sets up the registers and flags accordingly, copies byte array
values into the memory. While initializing, it follows the order of `init` assignments. 
3. The engine loops through the nested `test` blocks in their declaration order, and executes them.
4. When all tests are completed, the engine stops the virtual machine and disposes its state.

## Running a Test and Parameterized Cases

To run a single test, the engine follows these steps:

1. If the test has a `setup` declaration, the engine invokes the Setup code. If that code fails 
&mdash; it exceeds the timeout &mdash; the engine aborts the test.
2. If the test has a single default case, the engine runs that case. If the test has multiple cases, 
the engine iterates through all cases in their declaration order.
3. Provided the test has a `cleanup` section, the engine invokes the Cleanup code. If this code 
exceeds the timeout, the Cleanup code aborts the test.

> According to this method, it might happen that all tests run successfully and still the test is
aborted, because its Cleanup code fails. This behavior is intentional: a faulty cleanup code may influence
the subsequent tests.

The engine carries out the same steps for a default test case and parameterized test cases. If there are 
more cases, these steps are executed in a loop:

1. Provided there are `arrange` declarations, the engine sets up the registers and flags accordingly, 
copies byte array values into the memory &mdash; in the order of their declaration. If any `arrange` 
assignment fails, the test is aborted.
2. The engine invokes the `act` code. If it completes within the timeout, the testing process goes on;
otherwise, the test is aborted.
3. If there is no `assert` section, the test is successful.
4. If there's an `assert` section, the engine evaluates all expresssions within until it iterates through 
all or one of the expression evaluates to false.
5. A false assertion value completes the test as failed. If all assertions are true, the test is successful.

## Side effects

A test can have test options (such as `timeout`, `di`, or `ei`). The engine uses these values whenever
it invokes Z80 code, independently whether that is `setup`, `act`, or `cleanup`. Between these 
code invocations, the engine simply pauses the Spectrum virtual machine and starts it again. There is one
special action the test engine takes: if a code is invoked with `halt`, the test removes the CPU from its
halted state before the next test.

> Again, the code is injected into the virtual machine only once, at the start of a test set. If any code
changes the memory, it may disrupt the test code, and thus remaining test cases in the test set may fail of
even may be aborted. Nonetheless, these tests won't cause any harm in your project, they just cause tests fail.

For example, the following code disrupts the Spectrum virtual machine, because it restarts it, and so it causes
the clearing the RAM:

```
testset Crashing
{
    source "../Z80CodeFiles/CodeSamples.z80asm";

    test CrashingCode
    {
        with timeout 1000;
        act call #0000;
    }
}
```

Running it will abort the test after a second.

## Using Callstub

When you invoke the code with the `call` instruction, the test engine generates a stub
that call your subroutine. By default, the engine places three bytes with a Z80 `CALL`
instruction to the __`#5BA0`__ address, that is an empty area within the ZX Spectrum system
variables (printer buffer in ZX Spectrum 48K). If you do not want to use this address for a
stub, you can change it with the __`callstub`__ attribute of a test set:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";
    callstub #8000;

    // --- Other test code omitted
}
```

This sample code instructs the engine to use the __`#8000`__ address to generate the stub.
Be careful with using your custom stub!
* First, do not forget to provide 3 bytes that the test engine can override.
* Second, take care that you do not declare a `call` code invocation that addresses a routine
starting at the `callstub` + 3 address. The test engine checks if your call is completed so
that it compares __PC__ with `callstub` + 3.

This is a pattern you can use with your own custom `callstub`:

```
.org #8000

; --- Here is some code

; --- We reserve 4 bytes
CallstubAddress:
    .defs 4;
; --- SomeRoutine starts at CallstubAddress + 4
SomeRoutine:
; --- Add routine code here
```

Now, you can define a test line this:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";
    callstub CallstubAddress;

    test 
    {
        // ...
        act call SomeRoutine;
    }
}

```
