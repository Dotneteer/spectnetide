grammar Z80Test;

// === Parser rules

compileUnit
	:	testSet* EOF
	;

testSet
	:	TESTSET IDENTIFIER '{' 
		sourceContext
		testOptions?
		dataBlock?
		initSettings?
		setupCode?
		cleanupCode?
		testBlock*
		'}'
	;

sourceContext
	:	SOURCE STRING (SYMBOLS IDENTIFIER+)? ';'
	;

testOptions
	:	WITH testOption (',' testOption)* ';'
	;

testOption
	:	TIMEOUT expr
	|	DI
	|	EI
	;

dataBlock
	:	DATA '{' 
		dataBlockBody*
		'}' ';'?
	;

dataBlockBody
	:	valueDef 
	|	memPattern 
	|	portMock
	;

valueDef
	:	IDENTIFIER ':' expr ';'
	;

memPattern
	:	IDENTIFIER '{' memPatternBody+ '}' ';'?
	;

memPatternBody
	:	(byteSet | wordSet | text)
	;

byteSet
	:	(BYTE?) expr (',' expr)* ';'
	;

wordSet
	:	WORD expr (',' expr)* ';'
	;

text
	:	(TEXT?) STRING ';'
	;

portMock
	:	IDENTIFIER '<' expr '>' ':' portPulse (',' portPulse)* ';'
	;

portPulse
	: '{' expr ':' expr ((':' | '..') expr)? '}'
	;

initSettings
	:	INIT '{' assignment+ '}' ';'?
	;

setupCode
	:	SETUP invokeCode ';'
	;

cleanupCode
	:	CLEANUP invokeCode ';'
	;

invokeCode
	:	CALL expr
	|	START expr (STOP expr|HALT) 
	;

testBlock
	:	TEST IDENTIFIER '{'
		(CATEGORY IDENTIFIER ';')?
		testOptions?
		testParams?
		testCase*
		arrange?
		act
		breakpoint?
		assert?
		'}'
	;

testParams
	:	PARAMS IDENTIFIER (',' IDENTIFIER)* ';'
	;

testCase
	:	CASE expr (',' expr)* (PORTMOCK IDENTIFIER (',' IDENTIFIER)*)? ';'
	;

arrange
	:	ARRANGE '{' assignment+ '}' ';'?
	;

assignment
	:	(regAssignment
	|	flagStatus
	|	memAssignment) ';'
	;

regAssignment
	:	registerSpec ':' expr
	;

flagStatus
	:	flag
	;

memAssignment
	:	'[' expr ']' ':' expr (':' expr)?
	;

act
	:	ACT invokeCode ';'
	;

breakpoint
	:	BREAKPOINT expr (',' expr)* ';'
	;

assert
	:	ASSERT '{' (expr ';')+ '}' ';'?
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
	:	'af\''|'AF\''
	|	'bc\''|'BC\''
	|	'de\''|'DE\''
	|	'hl\''|'HL\''
;

flag
	:	'.z'|'.Z'|'.nz'|'.NZ'
	|	'.c'|'.C'|'.nc'|'.NC'
	|	'.pe'|'.PE'|'.po'|'.PO'
	|	'.p'|'.P'|'.m'|'.M'
	|	'.n'|'.N'|'.a'|'.A'
	|	'.h'|'.H'|'.nh'|'.NH'
	|	'.3'|'.n3'|'.N3'
	|	'.5'|'.n5'|'.N5'
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
	| '!' unaryExpr
	| '(' expr ')'
	| literalExpr
	| symbolExpr
	| registerSpec
	| flag
	| addrSpec
	| reachSpec
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
	: '[' expr ('..' expr)? ']'
	;

reachSpec
	: '[.' expr ('..' expr)? '.]'
	;

// === Lexer Rules

// --- Fragments

fragment InputCharacter:
	~[\r\n\u0085\u2028\u2029]
	;

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;

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

SINGLE_LINE_COMMENT:     '//'  InputCharacter*    -> channel(HIDDEN);
DELIMITED_COMMENT:       '/*'  .*? '*/'           -> channel(HIDDEN);

WHITESPACES:   
	(Whitespace | NewLine)+  -> channel(HIDDEN)
	;

// --- Keywords of the Z80 TEST language
TESTSET	: 'testset';
SOURCE	: 'source';
SYMBOLS	: 'symbols';
WITH	: 'with';
TIMEOUT	: 'timeout';
DI		: 'di';
EI		: 'ei';
DATA	: 'data';
BYTE	: 'byte';
WORD	: 'word';
TEXT	: 'text';
INIT	: 'init';
SETUP	: 'setup';
CALL	: 'call';
START	: 'start';
STOP	: 'stop';
HALT	: 'halt';
CLEANUP	: 'cleanup';
TEST	: 'test';
CATEGORY: 'category';
PARAMS	: 'params';
CASE	: 'case';
ARRANGE	: 'arrange';
ACT		: 'act';
ASSERT	: 'assert';
PORTMOCK: 'portmock';
BREAKPOINT: 'breakpoint';

// --- Numeric and character/string literals
DECNUM	: Digit Digit*;
HEXNUM	: ('#'|'0x') HexDigit HexDigit? HexDigit? HexDigit?
		| HexDigit HexDigit? HexDigit? HexDigit? ('H' | 'h') ;
BINNUM	: ('%'| ('0b' '_'?)) BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?;

CHAR	: '"' (~['\\\r\n\u0085\u2028\u2029] | CommonCharacter) '"' ;
STRING	: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;
