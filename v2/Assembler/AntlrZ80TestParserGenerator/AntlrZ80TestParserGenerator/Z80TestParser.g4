parser grammar Z80TestParser;

options {
    tokenVocab=Z80TestLexer;
}

// === Parser rules

compileUnit
	:	testSet* EOF
	;

testSet
	:	TESTSET IDENTIFIER OpenBrace 
		sp48Mode?
		sourceContext
		callstub?
		dataBlock?
		initSettings?
		testBlock*
		CloseBrace
	;

sp48Mode
	:	SP48MODE Semicolon
	;

sourceContext
	:	SOURCE STRING (SYMBOLS IDENTIFIER+)? Semicolon
	;

callstub
	:	CALLSTUB expr Semicolon
	;

testOptions
	:	WITH testOption (Comma testOption)* Semicolon
	;

testOption
	:	TIMEOUT expr
	|	DI
	|	EI
	;

dataBlock
	:	DATA OpenBrace 
		dataBlockBody*
		CloseBrace Semicolon?
	;

dataBlockBody
	:	valueDef 
	|	memPattern 
	|	portMock
	;

valueDef
	:	IDENTIFIER Colon expr Semicolon
	;

memPattern
	:	IDENTIFIER  OpenBrace memPatternBody+ CloseBrace Semicolon?
	|	IDENTIFIER Colon text
	;

memPatternBody
	:	(byteSet | wordSet | text)
	;

byteSet
	:	(BYTE?) expr (Comma expr)* Semicolon
	;

wordSet
	:	WORD expr (Comma expr)* Semicolon
	;

text
	:	(TEXT?) STRING Semicolon
	;

portMock
	:	IDENTIFIER AngleL expr AngleR Colon portPulse (Comma portPulse)* Semicolon
	;

portPulse
	: OpenBrace expr Colon expr ((Colon | Ellipse) expr)? CloseBrace
	;

initSettings
	:	INIT OpenBrace assignment+ CloseBrace Semicolon?
	;

setupCode
	:	SETUP invokeCode Semicolon
	;

cleanupCode
	:	CLEANUP invokeCode Semicolon
	;

invokeCode
	:	CALL expr
	|	START expr (STOP expr|HALT) 
	;

testBlock
	:	TEST IDENTIFIER OpenBrace
		(CATEGORY IDENTIFIER Semicolon)?
		testOptions?
		setupCode?
		testParams?
		testCase*
		arrange?
		act
		breakpoint?
		assert?
		cleanupCode?
		CloseBrace
	;

testParams
	:	PARAMS IDENTIFIER (Comma IDENTIFIER)* Semicolon
	;

testCase
	:	CASE expr (Comma expr)* (PORTMOCK IDENTIFIER (Comma IDENTIFIER)*)? Semicolon
	;

arrange
	:	ARRANGE OpenBrace assignment+ CloseBrace Semicolon?
	;

assignment
	:	(regAssignment
	|	flagStatus
	|	memAssignment) Semicolon
	;

regAssignment
	:	registerSpec Colon expr
	;

flagStatus
	:	flag
	;

memAssignment
	:	BracketL expr BracketR Colon expr (Colon expr)?
	;

act
	:	ACT invokeCode Semicolon
	;

breakpoint
	:	BREAKPOINT expr (Comma expr)* Semicolon
	;

assert
	:	ASSERT OpenBrace (expr Semicolon)+ CloseBrace Semicolon?
	;

reg8: Reg8Bit;	

reg8Idx: Reg8BitIdx;

reg8Spec: Reg8BitSpec;

reg16: Reg16Bit;

reg16Idx: Reg16BitIdx;

reg16Spec: Reg16BitSpec;

flag: FlagSpec;

// --- Expressions
expr
	: orExpr (Qmark expr Colon expr)?
	; 

orExpr
	: xorExpr (Or xorExpr)*
	;

xorExpr
	: andExpr (Xor andExpr)*
	;

andExpr
	: equExpr (And equExpr)*
	;

equExpr
	: relExpr ((Equal|NotEqual) relExpr)*
	;

relExpr
	: shiftExpr ((AngleL|LessThanO|AngleR|GreatherThanO) shiftExpr)*
	;

shiftExpr
	: addExpr ((ShiftL | ShiftR ) addExpr)*
	;

addExpr
	: multExpr ((Plus | Minus ) multExpr)*
	;

multExpr
	: unaryExpr ((Mult | Div | Mod) unaryExpr)*
	;

unaryExpr
	: Plus unaryExpr
	| Minus unaryExpr
	| Tilde unaryExpr
	| Exclm unaryExpr
	| Mult unaryExpr
	| Qmark unaryExpr
	| ParenL expr ParenR
	| literalExpr
	| symbolExpr
	| registerSpec
	| flag
	| reachSpec
	| memReadSpec
	| memWriteSpec
	| addrSpec
	;

literalExpr
	: DECNUM 
	| HEXNUM 
	| CHAR
	| BINNUM
	| 
	;

symbolExpr
	: IDENTIFIER
	;

registerSpec
	: reg8|reg8Idx|reg8Spec|reg16|reg16Idx|reg16Spec
	;

addrSpec
	: BracketL expr (Ellipse expr)? BracketR
	;

reachSpec
	: ReachL expr (Ellipse expr)? ReachR
	;

memReadSpec
	: MemrL expr (Ellipse expr)? MemrR
	;

memWriteSpec
	: MemwL expr (Ellipse expr)? MemwR
	;
