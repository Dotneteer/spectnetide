---
layout: documents
categories: 
  - "Unit Testing"
title:  "Expressions"
alias: unit-testing-expressions
seqno: 20
selector: documents
permalink: "documents/unit-testing-expressions"
---

The test language has a rich syntax for evaluating expressions. You can use operands 
and operators just like in most programming languages. Nevertheless, the test language
implementation has its particular semantics of evaluating expression, as you will learn soon.

## Operands
You can use the following operands in epressions:
* Decimal and hexadecimal literals
* Character literals
* Identifiers
* Test language specific constructs, such as the register, flag, and memory access operands

> String literals cannot be used as operands.

## Operators

You can use about a dozen operators, including unary, binary and ternary ones. In this section
you will learn about them. I will introduce them in descending order of their precendence.

### Conditional Operator

The assembler supports using only one ternary operator, the conditional operator:

_conditional-expression_ __```?```__ _true-value_ __```:```__ _false-value_

This operation results in -1:

```2 > 3 ? 2 : -1```

When the _conditional-expression_ evaluates to true, the operation results 
in _true-value_; otherwise in _false-value_.

> Conditional expressions are evaluated from right to left, in contrast to binary operators,
> which use left-to-right evaluation.

### Binary Bitwise Operators

Operator token | Precedence | Description
---------------|------------|------------
```|``` | 1 | Bitwise OR
```^``` | 2 | Bitwise XOR
```&``` | 3 | Bitwise AND

### Relational Operators

Operator token | Precedence | Description
---------------|------------|------------
```==``` | 4 | Equality
```!=``` | 4 | Non-equality
```<```  | 5 | Less than
```<=``` | 5 | Less than or equal
```>```  | 5 | Greater than
```>=``` | 5 | Greater than or equal

### Shift Operators

The bits of the left operand are shifted by the number of bits given by the right operand.

Operator token | Precedence | Description
---------------|------------|------------
```<<``` | 6 | Shift left
```>>``` | 6 | Shift right

### Basic Arithmetic Operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 7 | Addition
```-``` | 7 | Subtraction
```*``` | 8 | Multiplication
```/``` | 8 | Division
```%``` | 8 | Modulo calculation

### Unary operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 9 | Unary plus
```-``` | 9 | Unary minus
```~``` | 9 | Unary bitwise NOT
```!``` | 9 | Unary logical NOT

> Do not forget, you can change the defult precendence with ```(``` and ```)```.

## Type semantics

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

## Operator-Specific Behavior

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