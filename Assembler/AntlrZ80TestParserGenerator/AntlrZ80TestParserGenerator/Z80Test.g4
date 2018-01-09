grammar Z80Test;

/*
 * Parser Rules
 */

compileUnit
	:	testLanguageBlock* EOF
	;

testLanguageBlock
	:	testBlock
	|	dataBlock
	|	includeDirective
	;

includeDirective
	:	INCLUDE STRING
	;

testBlock
	:	testTitle
		testCategory?
		machineContext?
		sourceContext
		testOptions?
		testParams?
		testCases*
		arrange?
		act
		assert?
		END
	;

testTitle
	:	TEST IDENTIFIER
	;

testCategory
	:	CATEGORY IDENTIFIER
	;

machineContext
	:	MACHINE IDENTIFIER
	;

sourceContext
	:	SOURCE STRING (SYMBOLS IDENTIFIER+)?
	;

testOptions
	:	WITH testOption+
	;

testOption
	:	TIMEOUT expr
	|	NONMI
	;

testParams
	:	PARAMS IDENTIFIER (',' IDENTIFIER)*
	;

testCases
	:	CASE expr (',' expr)*
	;

arrange
	:	ARRANGE assignment (',' assignment)*
	;

assignment
	:	regAssignment
	|	flagStatus
	|	memAssignment
	;

regAssignment
	:	registerSpec ':' expr
	;

flagStatus
	:	flag
	|	'!' flag
	;

memAssignment
	:	memSpec ':' expr (',' expr)?
	;

memSpec
	: '[' expr ']'
	;

act
	:	ACT START expr (STOP expr|HALT) 
	;

assert
	:	ASSERT expr (',' expr)*
	;

dataBlock
	:	DATA
		(valueDef | memPattern)? (',' (valueDef | memPattern))*
		END
	;

valueDef
	:	IDENTIFIER ':' expr
	;

memPattern
	:	IDENTIFIER (byteSet | wordSet | text)+
	;

byteSet
	:	BYTE expr (',' expr)
	;

wordSet
	:	WORD expr (',' expr)
	;

text
	:	TEXT STRING
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
	:	'@z'|'@Z'
	|	'@c'|'@C'
	|	'@p'|'@P'
	|	'@s'|'@S'
	|	'@n'|'@N'
	|	'@h'|'@H'
	|	'@3'
	|	'@5'
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
	: '.' (reg8|reg8Idx|reg8Spec|reg16|reg16Idx|reg16Spec)
	;

addrSpec
	: '[' expr ('..' expr) ']'
	;

reachSpec
	: '{' expr ('..' expr) '}'
	;

/*
 * Lexer Rules
 */

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

COMMENT
	:	';' ~('\r' | '\n')* -> channel(HIDDEN)
	;

WHITESPACES:   
	(Whitespace | NewLine)+  -> channel(HIDDEN)
	;

// --- Keywords of the Z80 TEST language
TEST	: 'test'|'TEST';
CATEGORY: 'category'|'CATEGORY';
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
START	: 'start'|'START';
STOP	: 'stop'|'STOP';
HALT	: 'halt'|'HALT';
BYTE	: '.byte'|'.BYTE';
WORD	: '.word'|'.WORD';
TEXT	: '.text'|'.TEXT';
INCLUDE	: 'include'|'INCLUDE' ;

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
