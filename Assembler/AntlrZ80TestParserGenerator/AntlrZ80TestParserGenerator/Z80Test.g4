grammar Z80Test;

/*
 * Parser Rules
 */

compileUnit
	:	EOF
	|	NEWLINE* testLanguageBlock (NEWLINE+ testLanguageBlock)* NEWLINE* EOF
	;

testLanguageBlock
	:	testBlock
	|	dataBlock
	;

testBlock:
	|	testTitle NEWLINE+ 
		testContext NEWLINE+
		testOptions NEWLINE+
		(testParams NEWLINE+)?
		(testCases NEWLINE+)?
		(arrange NEWLINE+)?
		act NEWLINE+
		(assert NEWLINE+)?
		END NEWLINE+
	;

testTitle
	:	TEST TITLE
	;

testContexts
	:	testContext (NEWLINE+ testContext)*
	;

testContext
	:	machineContext
	|	sourceContext
	;

machineContext
	:	MACHINE IDENTIFIER
	;

sourceContext
	:	SOURCE STRING SYMBOLS IDENTIFIER (NEWLINE* IDENTIFIER)
	;

testOptions
	:	testOption (NEWLINE* testOption)*
	;

testOption
	:	TIMEOUT expr
	|	NONMI
	;

testParams
	:	PARAMS IDENTIFIER (',' NEWLINE* IDENTIFIER)*
	;

testCases
	:	CASE expr (',' NEWLINE* expr)*
	;

arrange
	:	ARRANGE assignment (',' NEWLINE* assignment)*
	;

assignment
	:	regAssignment
	;

regAssignment
	:	REG '.' (reg8|reg8Idx|reg8Spec|reg16|reg16Idx|reg16Spec) '=' expr
	;

act
	:	ACT
	;

assert
	:	ASSERT
	;

dataBlock
	:	DATA NEWLINE+
		END NEWLINE+
	;

reg8
	:	'a'|'A'
	|	'b'|'B'
	|	'c'|'C'
	|	'd'|'D'
	|	'e'|'E'
	|	'h'|'H'
	|	'l'|'L'
	;

reg8Idx
	:	'xl'|'XL'
	|	'xh'|'XH'
	|	'yl'|'YL'
	|	'yh'|'YH'
	|	'ixl'|'IXL'|'IXl'
	|	'ixh'|'IXH'|'IXh'
	|	'iyl'|'IYL'|'IYl'
	|	'iyh'|'IYH'|'IYh'
	;

reg8Spec
	:	'i'|'I'
	|	'r'|'R'
	;

reg16
	:	'bc'|'BC'
	|	'de'|'DE'
	|	'hl'|'HL'
	|	'sp'|'SP'
	;

reg16Idx
	:	'ix'|'IX'
	|	'iy'|'IY'
	;

reg16Spec
	:	'af'|'AF'
	|	'af\''|'AF\''
	|	'bc\''|'BC\''
	|	'de\''|'DE\''
	|	'hl\''|'HL\''
;

flag
	:	'z'|'Z'
	|	'nz'|'NZ'
	|	'c'|'C'
	|	'nc'|'NC'
	|	'po'|'PO'
	|	'pe'|'PE'
	|	'p'|'P'
	|	'm'|'M'
	|	'n'|'N'
	|	'h'|'H'
	|	'3'
	|	'5'
	;

// --- Expressions
expr
	: orExpr ('?' expr ':' expr)?
	; 

orExpr
	: xorExpr ('|' xorExpr)*
	;

xorExpr
	: andExpr ('^' andExpr)*
	;

andExpr
	: equExpr ('&' equExpr)*
	;

equExpr
	: relExpr (('=='|'!=') relExpr)*
	;

relExpr
	: shiftExpr (('<'|'<='|'>'|'>=') shiftExpr)*
	;

shiftExpr
	: addExpr (('<<' | '>>' ) addExpr)*
	;

addExpr
	: multExpr (('+' | '-' ) multExpr)*
	;

multExpr
	: unaryExpr (('*' | '/' | '%') unaryExpr)*
	;

unaryExpr
	: '+' unaryExpr
	| '-' unaryExpr
	| '~' unaryExpr
	| '[' expr ']'
	| literalExpr
	| symbolExpr
	;

literalExpr
	: DECNUM 
	| HEXNUM 
	| CHAR
	| BINNUM
	| '$'
	;

symbolExpr
	: IDENTIFIER
	;

/*
 * Lexer Rules
 */

fragment CommonCharacter
	: SimpleEscapeSequence
	| HexEscapeSequence
	;

fragment SimpleEscapeSequence
	: '\\i'
	| '\\p'
	| '\\f'
	| '\\b'
	| '\\I'
	| '\\o'
	| '\\a'
	| '\\t'
	| '\\P'
	| '\\C'
	| '\\\''
	| '\\"'
	| '\\\\'
	| '\\0'
	;

fragment HexEscapeSequence
	: '\\x' HexDigit
	| '\\x' HexDigit HexDigit
	;

fragment HexDigit 
	: [0-9] 
	| [A-F] 
	| [a-f]
	;

fragment Digit 
	: '0'..'9' 
	;

fragment BinDigit
	: ('0'|'1') '_'?
	;

COMMENT
	:	';' ~('\r' | '\n')*
	;

WS
	:	(' ' | '\t') -> channel(HIDDEN)
	;

NEWLINE
	:	('\r'? '\n' | '\r')+
	;

// --- Keywords of the Z80 TEST language
TEST	: 'test'|'TEST';
PARAMS	: 'params'|'PARAMS';
END		: 'end'|'END';
DATA	: 'data'|'DATA';
MACHINE	: 'machine'|'MACHINE';
SOURCE	: 'source'|'SOURCE';
SYMBOLS	: 'symbols'|'SYMBOLS';
WITH	: 'with'|'WITH';
TIMEOUT	: 'timeout'|'TIMEOUT';
NONMI	: 'nonmi'|'NONMI';
CASE	: 'case'|'CASE';
ARRANGE	: 'arrange'|'ARRANGE';
ACT		: 'act'|'ACT';
ASSERT	: 'assert'|'ASSERT';
REG		: 'reg'|'REG';

// --- Numeric and character/string literals
DECNUM	: Digit Digit? Digit? Digit? Digit?;
HEXNUM	: ('#'|'0x') HexDigit HexDigit? HexDigit? HexDigit?
		| HexDigit HexDigit? HexDigit? HexDigit? ('H' | 'h') ;
BINNUM	: ('%'| ('0b' '_'?)) BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		;
CHAR	: '"' (~['\\\r\n\u0085\u2028\u2029] | CommonCharacter) '"' ;
STRING	: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;

// --- Title literal
TITLE:	(~[\\\r\n\u0085\u2028\u2029])+ ;
