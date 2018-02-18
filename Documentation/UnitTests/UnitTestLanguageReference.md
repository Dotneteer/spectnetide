# SpectNetIde Unit Test Language Reference

__SpectNetIde__ provides a somple programming language to describe Z80-related unit tests. You can add the 
`.z80test` files to your project and run the test in the __Unit Test Explorer__ tool window. This document
treats the syntax and semantics of the language.

## Syntax Basics

The test language uses a special way of case-sensitivity. You can write the reserved
keywords on lowercase. When you refer to Z80 CPU registers or flags, you can use either with lowercase or
uppercase letters, but you cannot mix these cases. You can mix cases for identifiers, though they are
searched for a case-insensitive manner.

## Test Structure

A ZX Spectrum project may have zero, one, or more test files (files with `.z80test` extension). Before 
running them, the engine collects all `.z80test` files from the current project, and compiles them all.

A single test file may contain one or more _test set_. A test set is a collection of cohesive tests that 
all use the very same source code, as the basis of testing:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";

    // --- Other testset attributes    

    test AddAAndBWorksAsExpected
    {
        // --- Other test attributes

        act call AddAAndB;

        assert
        {
            // --- Here we describe the test assertions
        }
    }
}
```

A _test_ can have a default _act_, such as in the sample above, or may have parameterized test cases:

```
// --- Wrapping test set omitted
test AddAAndBCallWorksAsExpected2
{
    params parA, parB, Z;
    case 1, 2, 0;
    case 2, 3, 0;
    case -6, 6, 1;

    arrange
    {
        a: parA;
        b: parB;
    }

    act call AddAAndB;

    assert
    {
        a == parA + parB;
        b == parB;
        .z == Z;
    }
}
```

This test has three cases, as declared by the lines starting with the `case` keyword.
When running them, the engine substitutes the `parA`, `parB`, and `Z` values with the
values after `case` (fore each `case`).

To summarize:

Concept | Description |
--------|-------------|
_test file_ | A single container for _test sets_. Besides keeping test sets together in a single file, there's no additional semantics.
_test set_ | A cohesive set of tests. A test set has a single source code file &mdash; this contains the code to test &mdash; shared between the test within the set.
_test_ | A single test that runs a piece of the source code to test. It may nest test cases.
_test case_ | Parameterized test. It runs the same code (although you can run different code) with the case-related parameters.

## Syntax Elements

The test language contains several constituting elements that you can use in many 
places within the code, such as comments, expressions, identifiers, and so on. Here 
you can learn about them.

### Comments

Comments can be single line or multi-line comments with the same syntax construct as you may 
use them in many curly-brace-languages, such as C++, Java, C#, etc.:

```
// --- This is a single line comment you can add to the end of the code lines

/* This is a multi-line comment the spans accross multiple lines, 
   including empty ones

*/
```

### Literals

The language syntax provides these types of literals:
* __Decimal numbers.__ You can use adjacent digits (0..9) to declare a decimal number. Examples:
16, 32768, 2354.
* __Hexadecimal numbers.__ You can use up to 4 hexadecimal digits (0..9, a..f or A..F) to declare
a hexadecimal literal. The compiler looks for one of the ```#``` or ```0x``` prefix, or one of 
the ```h``` or ```H``` suffixes to recognize them as hexadecimal. Here are a few samples:

```
    #12AC
    0x12ac
    12ACh
    12acH
```

* __Binary numbers.__ Literal starting with the one of the `%` or `0b` prefix are taken into 
account as binary literals. You can follow the prefix with up to 16 `0` or `1` digits. To make
them more readable, you can separate adjacent digits with the underscore (`_`) character. These 
are all valid binary literals:

```
    %01011111
    0b01011111
    0b_0101_1111
```

> You can use negative number with the minus sign in front of them. Actually, the sign is not
> the part of the numeric literal, it is an operator. 

* __Characters__. You can put a character between double quotes (for example: ```"Q"```). 
* __Strings__. You can put a series of character between double quotes (for example: ```"Sinclair"```).

Here are a few samples:

```
"This is a string. The next sample is a single character"
"c"
```

### Character and String Escapes

ZX Spectrum has a character set with special control characters such as AT, INK, PAPER, and so on.

__SpectNetIde__ allows you to utilize special escape sequences to define ZX Spectrum-specific characters:

Escape | Code | Character
-------|------|----------
```\i``` | 0x10 | INK 
```\p``` | 0x11 | PAPER
```\f``` | 0x12 | FLASH
```\b``` | 0x13 | BRIGHT
```\I``` | 0x14 | INVERSE
```\o``` | 0x15 | OVER
```\a``` | 0x16 | AT
```\t``` | 0x17 | TAB
```\P``` | 0x60 | pound sign
```\C``` | 0x7F | copyright sign
```\\``` | 0x5C | backslash
```\'``` | 0x27 | single quote
```\"``` | 0x22 | double quote
```\0``` | 0x00 | binary zero

> Observe, some of these sequences have different values than their corresponding
> pairs in other languages, such as C, C++, C#, or Java.

To declare a character by its binary code, you can use the ```\xH``` or  
```\xHH``` sequences (```H``` is a hexadecimal digit). For example, these
escape sequence pairs are equivalent:

```
"\i"
"\x10"

"\C by me"
"\x7f \x62y me"
```

### Identifiers

You can use identifiers to refer to labels and other constants. Identifiers must start with 
a letter (a..z or A..Z) or the underscore character (```_```). The subsequent characters 
can be digits (0..9), too. Here are a few samples:

```
MyCycle
ERR_NO
Cycle_4_Wait  
```

> Theoretically, you can use as long identifiers as you want. I suggest you to make them no longer than
32 characters so that readers may read your code easily.

## Expressions
The test language has a rich syntax for evaluating expressions. You can use operands 
and operators just like in most programming languages. Nevertheless, the test language
implementation has its particular semantics of evaluating expression, as you will learn soon.

### Operands
You can use the following operands in epressions:
* Decimal and hexadecimal literals
* Character literals
* Identifiers
* Test language specific constructs, such as the register, flag, and memory access operands

> String literals cannot be used as operands.

### Operators

You can use about a dozen operators, including unary, binary and ternary ones. In this section
you will learn about them. I will introduce them in descending order of their precendence.

#### Conditional Operator

The assembler supports using only one ternary operator, the conditional operator:

_conditional-expression_ __```?```__ _true-value_ __```:```__ _false-value_

This operation results in -1:

```2 > 3 ? 2 : -1```

When the _conditional-expression_ evaluates to true, the operation results 
in _true-value_; otherwise in _false-value_.

> Conditional expressions are evaluated from right to left, in contrast to binary operators,
> which use left-to-right evaluation.

#### Binary Bitwise Operators

Operator token | Precedence | Description
---------------|------------|------------
```|``` | 1 | Bitwise OR
```^``` | 2 | Bitwise XOR
```&``` | 3 | Bitwise AND

#### Relational Operators

Operator token | Precedence | Description
---------------|------------|------------
```==``` | 4 | Equality
```!=``` | 4 | Non-equality
```<```  | 5 | Less than
```<=``` | 5 | Less than or equal
```>```  | 5 | Greater than
```>=``` | 5 | Greater than or equal

#### Shift Operators

The bits of the left operand are shifted by the number of bits given by the right operand.

Operator token | Precedence | Description
---------------|------------|------------
```<<``` | 6 | Shift left
```>>``` | 6 | Shift right

#### Basic Arithmetic Operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 7 | Addition
```-``` | 7 | Subtraction
```*``` | 8 | Multiplication
```/``` | 8 | Division
```%``` | 8 | Modulo calculation

#### Unary operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 9 | Unary plus
```-``` | 9 | Unary minus
```~``` | 9 | Unary bitwise NOT
```!``` | 9 | Unary logical NOT

> Do not forget, you can change the defult precendence with ```(``` and ```)```.

### Type semantics

Expression values can be one of these types:
* Boolean
* Number
* Byte array

There are implicit conversions amont these types. The test language compiler automatically applies
these conversions, knowing the types to operate on.

Internally, values are stored either as 64-bit numbers, or byte arrays. The compiler utilizes these 
conversions:

1. When a byte array value is required but a numeric value is found, the numeric value is converted 
into an array of a single byte using the rightmost 8 bits of the number stored in the memory.
2. When a number is required but a byte array is found, the conversion goes like this: A byte array 
of all 0 bytes results 0, any other values retrieve 1.
3. When the compiler needs a boolean number, the zero numeric value results 0 (false), any other value 
values retrieve 1 (true).
4. When the compiler needs a boolean number but it finds a byte array, the conversion takes two steps.
First, it turns the byte array into a number if first converts a byte array into a number (rule #1), 
and then these number into a Boolean (rule #3).

### Operator-Specific Behavior

The compiler does not apply the conversion rules above automatically for all operators to avoid
unintended programming errors, instead retrieves an error:
* No binary operators (including relational operators) accept mixing numeric and byte array values
* Binary operators (except binary `+`) do not allow two byte array operands
* The conditional operator ( `?` `:` ) does not allow a byte array as the condition
* Unary operators except `!` and `~` allow only numbers.

There are a few operators that work with byte arrays:
* The binary `+` operator concatenates two byte arrays.
* The bitwise logical operators work with two byte array operands. The result will be
a byte array with the shorter size of the operands. The elements of the result array are the
corresponding bitwise operations on the bits of the operand arrays.
* The bitwise NOT unary operation inverts all bits in the byte array.
* The logical NOT unary operator converts the byte array to a number and applies the logical NOT 
* operator on that number.

## Detailed Test Language Syntax

In the following section, I will use a kind of abstract syntax notation to describe the grammar of the 
SpectNetIde test language. Bold characters mark terminal symbols (keywords and other tokens), while 
italic strings ara non-terminal symbols. The `?` after a symbol means that it's optional. A `*` 
means zero, one, or more occurrence. `+` means one or more occurrence. The `|` character separates options, exactly one of them can be used.
The language uses parantheses to specify groups of tokens.

### Compilation Units

Each file is a compilation unit that containz zero, one, or more test sets:

*`compileUnit:`* *`testSet*`*

### Test Sets

*`testSet:`** 
&nbsp;&nbsp;&nbsp;&nbsp;__testset__ *`identifier`* __{__  
&nbsp;&nbsp;&nbsp;&nbsp;*`sourceContext`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`callstub?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`dataBlock?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`initSettings?`*  
&nbsp;&nbsp;&nbsp;&nbsp;*`testBlock*`*  
&nbsp;&nbsp;&nbsp;&nbsp;__}__

#### The Source Context

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

#### The Callstub Option

*`callstub:`*	__callstub__ *`expr`* __;__  

This option defines the address to use for a three-byte call stub when running unit tests. You will learn about 
this option later when I treat the semantics of test execution.

#### Defining Data Blocks

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

#### Test Set Initialization Settings

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

### Test Blocks

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

#### Test Options

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

#### Running Code in Tests

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

#### Test Parameters and Test Cases

You can define parameterized test cases. You name test parameters, and test cases declare values to substitute a
particular parameter:

*`testParams:`* __params__ *`identifier`* ( __,__ *`identifier`*) *`*`* __;__  
*`testCase:`* __case__ *`expr`* ( __,__ *`expr`*) *`*`* __;__

Of course, the number of parameters and expressions after the `case` keyword must match for each test cases.

> Soon, you will see a complete sample that demonstrates these concepts.

#### Arrange Declarations

Each test block has an `arrange` declaration that runs before the engine invokes the `act` code of a test case.
It has the same assignment syntax, as the `init` construct of a test set:

*`arrange:`* __arrange__ __{__ *`assignment+`* __}__

#### Assertions

After the test code ran, you can run assertions. Assertions are a list of Boolean expressions, and the test engine 
evaluates them in their declaration order. All of them should be true to make the test case successful:

*`assert:`* __assert__ __{__ ( *`expr`* __;__)*`+*` __}__

You can use special assertion expressions:

*`addrSpec:`* __[__ *`expr`* ( __..__ *`expr`*) *`?`* __]__  
*`reachSpec:`* __[.__ *`expr`* ( __..__ *`expr`*) *`?`* __.]__

An `addressSpec` retrieves a byte array that is a copy of the memory. It can be a single byte (only one address specified)
or a sequence of bytes (both a start address and an inclusive end address is specified).

A `reachSpec` retrieves a byte array. Each byte indicates if the test's control flow reached that address 
(the instruction at that address was executed) with a Boolean value.

> Note, this feature has not been implemented in the test engine yet.

#### Test Sample

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

## The Testing Process

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

### Running a Test Set

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

### Running a Test and Parameterized Cases

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

### Side effects

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

### Using Callstub

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