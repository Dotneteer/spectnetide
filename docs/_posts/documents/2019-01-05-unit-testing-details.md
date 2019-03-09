---
layout: documents
categories: 
  - "Unit Testing"
title:  "Detailed Syntax"
alias: unit-testing-details
seqno: 30
selector: documents
permalink: "documents/unit-testing-details"
---

In the following section, I will use a kind of abstract syntax notation to describe the grammar of the 
SpectNetIde test language. Bold characters mark terminal symbols (keywords and other tokens), while 
italic strings ara non-terminal symbols. The `?` after a symbol means that it's optional. A `*` 
means zero, one, or more occurrence. `+` means one or more occurrence. The `|` character separates options, exactly one of them can be used.
The language uses parantheses to specify groups of tokens.

## Compilation Units

Each file is a compilation unit that contains zero, one, or more test sets:

*`compileUnit:`* *`testSet*`*

## Test Sets

*`testSet:`** 
&nbsp;&nbsp;&nbsp;&nbsp;__testset__ *`identifier`* __{__  
&nbsp;&nbsp;&nbsp;&nbsp;*__sp48mode__ *`?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`sourceContext`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`callstub?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`dataBlock?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`initSettings?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`testBlock*`*  
&nbsp;&nbsp;&nbsp;&nbsp;__}__

### Using sp48mode

When you work with Spectrum 128K, Spectrum ++, or Spectrum Next, you can specify that you intend to
start the Spectrum virtual machine in Spectrum 48K mode:

```
testset Introduction
{
    sp48mode;
    source "../Z80CodeFiles/CodeSamples.z80asm";
    // ...
}
```

In this mode, you can be sure that the Spectrum 48K ROM is paged into the `#0000`-`#3FFF` address range,
and memory paging is forbidden &mdash;, and thus, the test behaves exactly as if it were run on a Spectrum 48K model.

### The Source Context

*`sourceContext:`*	 __source__ *`string`* *`(`* __symbols__ *`identifier+)?`* __;__  

Each test set is named with a unique identifier. The only mandatory element of a __testset__ declaration is 
the source context that names the source file used within the test set. You can add optional conditional symbols
that to compile the source code. Here are a few samples:

```
testset MyFirstSet
{
    source "../Z80CodeFiles/CodeSamples.z80asm";
}

testset MySecondSet
{
    source "../Z80CodeFiles/CodeSamples.z80asm" symbols DEBUG SP128;
}
```

Both test sets use the `../Z80CodeFiles/CodeSamples.z80asm` source code file (path relative to the test file).
Nonetheless, `MySecondSet` passes the `DEBUG` and `SP128` pre-defined symbols to the Z80 Assembler.

The test engine recognizes the symbols you declare in the source context. For example, if you has a `MyRoutine` 
symbol in the source code, you can use it within the test file. The compiler will understand it, and replaces the
symbol with its value.

### The Callstub Option

*`callstub:`*	__callstub__ *`expr`* __;__  

This option defines the address to use for a three-byte call stub when running unit tests. You will learn about 
this option later when I treat the semantics of test execution.

### Defining Data Blocks

A test set's data block is to define values and byte array patterns that represent a block of the memory. Data block 
entries have unique identifiers you can use in the tests to reference them.

*`dataBlock:`*	__data__ __{__ *`dataBlockBody*`* __}__ __;__*`?`*  
*`dataBlockBody:`* *`valueDef`* | *`memPattern`* 
*`valueDef:`*	*`identifier`* __:__ *`expr`* __;__  
*`memPattern:`* *`identifier`* __{__ *`memPatternBody+`* __}__ | *`identifier`* __:__ *`text`*  
*`memPatternBody:`* *`(byteSet`* | *`wordSet`* | *`text)`*  
*`byteSet:`* ( __byte__*`?`*) *`expr`* ( __,__ *`expr`*)*`*`* __;__  
*`wordSet:`* ( __word__*`?`*) *`expr`* ( __,__ *`expr`*)*`*`* __;__  
*`text:`* ( __text__ *`?*` ) *`string*` __;__

As the abstract syntax notation shows, it's pretty easy to define values:

```
data
{
     four: 1 + 3;
     helloWorld: "hello";
}
```

Here, the `four` identifier represents the constant 4; `helloWorld` represents an array of 5 bytes, each 
byte stands for the corresponding character.

You can define more complex byte arrays with the `byte`, `word`, and optional `text` keywords:

```
data
{
    myMemBlock
    {
        byte #12, #34, #36;
        word #AC38, #23;
        "012"; 
    }
}
```

Here, `myMemblock` is a 10 bytes long array with these values:
#12, #34, #36, #38, #AC, #23, #00, #30, #31, #32.

> Observe, words are stored in LSB/MSB order. The word #23 is two bytes: #23 and #00.

### Test Set Initialization Settings

Before running tests, you can initialize a test set by assigning values to Z80 registers, 
setting or reseting flags, copying values into the memory.

*`initSettings:`*	__init__ __{__ *`assignment+`* __}__  
*`assignment:`* ( *`regAssignment`* | *`flagStatus`* | *`memAssignment`* ) __;__  
*`regAssignment:`*  *`registerSpec`* __:__ *`expr`*  
*`registerSpec:`* __a__ | __A__ | __b__ | __B__ | __c__ | __C__ | __d__ | __D__  
&nbsp;&nbsp;&nbsp;&nbsp;| __e__ | __E__	| __h__ | __H__ | __l__ | __L__ | __xl__ | __XL__  
&nbsp;&nbsp;&nbsp;&nbsp;| __xh__ | __XH__ |	__yl__ | __YL__ | __yh__ | __YH__ |	__ixl__| __IXL__ | __IXl__  
&nbsp;&nbsp;&nbsp;&nbsp;| __ixh__| __IXH__ | __IXh__ | __iyl__ | __IYL__ | __IYl__ | __iyh__ | __IYH__ | __IYh__  
&nbsp;&nbsp;&nbsp;&nbsp;| __i__ | __I__ | __r__ | __R__ | __bc__ | __BC__ |	__de__ | __DE__  
&nbsp;&nbsp;&nbsp;&nbsp;| __hl__ | __HL__ | __sp__ | __SP__ | __ix__ | __IX__ | __iy__ | __IY__  
&nbsp;&nbsp;&nbsp;&nbsp;| __af'__ | __AF'__ | __bc'__ | __BC'__	| __de'__ | __DE'__ | __hl'__ | __HL'__  
*`flagStatus:`* __.z__ | __.Z__ | __.nz__ | __.NZ__ |	__.c__ | __.C__ | __.nc__ | __.NC__  
&nbsp;&nbsp;&nbsp;&nbsp;| __.pe__ | __.PE__ | __.po__ | __.PO__	| __.p__ | __.P__ | __.m__ | __.M__  
&nbsp;&nbsp;&nbsp;&nbsp;| __.n__| __.N__ | __.a__ | __.A__ | __.h__ | __.H__ | __.nh__ | __.NH__  
&nbsp;&nbsp;&nbsp;&nbsp;| __.3__ | __.n3__ | __.N3__ | __.5__ | __.n5__| __.N5__  
*`memAssignment:`*	 __[__ *`expr`* __]__ __:__ *`expr`* ( __:__ *`expr`*)*`?`*

Here is a short sample:

```
init
{
    bc: 0;
    hl: CustomBuffer;
    .z;
    .nc;
    [#4000]: myLogoArray;
}
```

This `init` declaration sets the valuu of the __BC__ and __HL__ register pairs to `0`, and `CustomBuffer`, respectively.
The Zero flag is set, while Carry is reset. The declaration copies the contents of the `myLogoArray` to the `#4000` address.

With an alternative `memAssignment` syntax, you can declare the length of a byte array before copying it into the memory.
For example, the following code snippet copies only the first 32 bytes to the `#4000` address:

```
init
{
    [#4000]: myLogoArray:32;
}
```

## Test Blocks

You can declare default and parameterized test cases within test blocks:

*`testBlock:`*	__test__ *`identifier`* __{__  
&nbsp;&nbsp;&nbsp;&nbsp;( __category__ *`identifier`* __;__) *`?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`testOptions?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`setupCode?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`testParams?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`testCase*`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`arrange?*`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`act`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`assert?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`cleanupCode?`*  
&nbsp;&nbsp;&nbsp;&nbsp;__}__

Each test must have a uniuque indentifier, and may declare a category, which is reserved for future extension. Of course,
multiple tests may have the same category. The only required part of a test is the `act` declaration that describes what code
to run within the test.

### Test Options

Tests may have options the engine uses when running them:

*`testOptions:`* __with__ *`testOption`* ( __,__ *`testOption`*)*`*`* __;__  
*`testOption:`* __timeout__ *`expr`* | __di__ | __ei__  

The timeout option sets the timeout value for the specified test. When it expires, the engine aborts the test, and thus 
you can even create code with infinite loops, it does not freezes the test engine.

> The default timeout value in 100 milliseconds.

With the `ei` and `di` options you can enable or disable interrupts explicitly before running any code. These 
options are just helpers. For tighter control, use the `EI` and `DI` Z80 instructions explicitly in your code.

> By default, interrupts are enabled.

The following options run a test case with 40 milliseconds of timeout and disable the interrupt before starting the code:

```
with timeout 40, di;
```

### Running Code in Tests

You can declare three kinds of code to run in a single test. Setup code runs once before each test cases, Cleanup code once 
after all cases completed (either successfully or failed). The Act code runs for every test cases.

*`setupCode:`* __setup__ *`invokeCode`* __;__  
*`act:`* __act__ *`invokeCode`* __;__  
*`cleanupCode:`* __cleanup__ *`invokeCode`* __;__  
*`invokeCode:`* __call__ *`expr`* | __start__ *`expr`* ( __stop__ *`expr`* | __halt__ )

You have three ways to invoke code:
* __`call`__ uses the Z80 `CALL` instruction to call the code with the specified address. Your code should have 
a `RET` instruction. When the code successfully executes the `RET` statement, the engine completes the test code.
* With the __`start`__ and __`stop`__ combination, you can explicitly specify a start and a stop address. The engine jumps
to the start address, and completes the test code as soon as it reaches the stop address. It does not executes the instruction
at the stop address.
* With the __`start`__ and __`halt`__ combination the test engine starts the code at the specified address, and completes it
when it reaches a `HALT` statement.

### Test Parameters and Test Cases

You can define parameterized test cases. You name test parameters, and test cases declare values to substitute a
particular parameter:

*`testParams:`* __params__ *`identifier`* ( __,__ *`identifier`*) *`*`* __;__  
*`testCase:`* __case__ *`expr`* ( __,__ *`expr`*) *`*`* __;__

Of course, the number of parameters and expressions after the `case` keyword must match for each test cases.

> Soon, you will see a complete sample that demonstrates these concepts.

### Arrange Declarations

Each test block has an `arrange` declaration that runs before the engine invokes the `act` code of a test case.
It has the same assignment syntax, as the `init` construct of a test set:

*`arrange:`* __arrange__ __{__ *`assignment+`* __}__

### Assertions

After the test code ran, you can run assertions. Assertions are a list of Boolean expressions, and the test engine 
evaluates them in their declaration order. All of them should be true to make the test case successful:

*`assert:`* __assert__ __{__ ( *`expr`* __;__)*`+*` __}__

You can use special assertion expressions:

*`addrSpec:`* __[__ *`expr`* ( __..__ *`expr`*) *`?`* __]__  
*`reachSpec:`* __<.__ *`expr`* ( __..__ *`expr`*) *`?`* __.>__  
*`memReadSpec:`* __<|__ *`expr`* ( __..__ *`expr`*) *`?`* __|>__  
*`memWriteSpec:`* __<||__ *`expr`* ( __..__ *`expr`*) *`?`* __||>__

These assertions retrieves a byte array. This may contain only a single byte (only one address specified)
or a sequence of bytes (both a start address and an inclusive end address is specified).

An `addrSpec` retrives the contenst of memory specified by the address range. Each byte in the array is the
copy of the corresponding memory address.

In the `reachSpec` byte array each byte indicates if the test's control flow reached that address 
(the instruction at that address was executed) with a Boolean value. Similarly, the arrays retrieved by 
`memReadSpec` and `memWriteSpec` indicate if the specified memory address was read, or written, respectively.

Let's see an example of using these assertions:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";
    data
    {
        str100: "00100";
        str1000:  "001000";
        str12345: "12345";
        str11211: "11211";
        str23456: "23456";
    }

    test BufferIncEachByteWorks
    {
        params value, result;
        case str100, str11211;
        case str12345, str23456;
        case str1000, str11211;
        arrange
        {
            hl: ConversionBuffer;
            b: 5;
            [ConversionBuffer]:value;
        }

        act call IncLoop;

        assert
        {
            [ConversionBuffer..ConversionBuffer+4] == result;
            <. IncLoop .>;
            <| ConversionBuffer .. ConversionBuffer + 4 |>;
            <|| ConversionBuffer .. ConversionBuffer + 4 ||>;
        }
    }
}
```

Here, the `IncLoop` method accepts a bufferr address in __HL__ and 
a byte count in __B__. `IncLoop` increments each byte by 1 within the
buffer. As you can see, the assert section checks these conditions:
* The contents of the conversion buffer is the one specified in `result`.
* The code executions reaches the `IncLoop` address.
* The contents of the buffer (5 bytes starting from `ConversionBuffer`) is read.
* The contents of the buffer is written.

### Test Sample

Here is a short sample that demonstrates the concepts you learned:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";

    test AddAAndBCallWorksAsExpected
    {
        params parA, parB, parExpected;
        case 1, 2, 3;
        case 2, 3, 5;
        case 6, 8, 14;

        arrange
        {
            a: parA;
            b: parB;
        }

        act call AddAAndB;

        assert 
        {
            a == parExpected;
        }
    }

    test AddAAndBWithStartWorksAsExpected
    {
        arrange
        {
            a: 3;
            b: 5;
        }

        act start AddAAndBWithStop stop StopPoint;

        assert 
        {
            a == 8;
        }
    }
}
```

The code file behind this test is the following:

```
Start:
	.org #8000

; You can invoke this test with 'call AddAAndB'
AddAAndB:
    add a,b
    ret

; You can invoke this test with 'start AddAAndBWithStop stop StopPoint'
AddAAndBWithStop:
    add a,b
StopPoint:
    nop
```

The first test, `AddAAndBCallWorksAsExpected`, has three test cases with parameters `parA`, `parB`, 
and `parExpected`, respectively. The engine executes each test case with these steps:
* Sets the __A__ and __B__ CPU registers with the current value of `parA` and `parB`. For the first case, 
these are 1, and 2. The second case runs with 2, and 3.
* Calls the `AddAAndB` subroutine, that executes the `add a,b` instruction, and then completes the 
call with `RET`
* Checks if the content of __A__ equals with the expected result, as declared in `parExpected`.

The second test has only a default case. It sets up __A__ and __B__ explicitly in the `arrange` section 
and checks the result in `assert`. You can observe that it does not call into the code, instead, it the test 
jumps to the `AddAAndBWithStop` address, and completes when it reaches `StopPoint`.
