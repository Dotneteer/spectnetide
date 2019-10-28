lexer grammar Z80TestLexer;

channels { COMMENT }

// === Lexer Rules
SINGLE_LINE_COMMENT:     '//'  InputCharacter*    -> channel(COMMENT);
DELIMITED_COMMENT:       '/*'  .*? '*/'           -> channel(COMMENT);

WHITESPACES:   
	(Whitespace | NewLine)+  -> channel(HIDDEN)
	;

// --- Keywords of the Z80 TEST language
TESTSET	: 'testset';
SOURCE	: 'source';
SP48MODE: 'sp48mode';
CALLSTUB: 'callstub';
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

// --- Pther tokens
OpenBrace: '{';
CloseBrace: '}'; 
Semicolon: ';';
Comma: ',';
Colon: ':';
AngleL: '<';
AngleR: '>';
Ellipse: '..';
BracketL: '[';
BracketR: ']';
ReachL: '<.';
ReachR: '.>';
MemrL: '<|';
MemrR: '|>';
MemwL: '<||';
MemwR: '||>';
Qmark: '?';
Or: '|';
And: '&';
Xor: '^';
Equal: '==';
NotEqual: '!=';
LessThanO: '<=';
GreatherThanO: '>=';
ShiftL: '<<';
ShiftR: '>>';
Plus: '+';
Minus: '-';
Mult: '*';
Div: '/';
Mod: '%';
Tilde: '~';
Exclm: '!';
ParenL: '(';
ParenR: ')';

Reg8Bit
	:	'a'|'A'
	|	'b'|'B'
	|	'c'|'C'
	|	'd'|'D'
	|	'e'|'E'
	|	'h'|'H'
	|	'l'|'L'
	;
Reg8BitIdx
	:	'xl'|'XL'
	|	'xh'|'XH'
	|	'yl'|'YL'
	|	'yh'|'YH'
	|	'ixl'|'IXL'|'IXl'
	|	'ixh'|'IXH'|'IXh'
	|	'iyl'|'IYL'|'IYl'
	|	'iyh'|'IYH'|'IYh'
	;

Reg8BitSpec
	:	'i'|'I'
	|	'r'|'R'
	;

Reg16Bit
	:	'bc'|'BC'
	|	'de'|'DE'
	|	'hl'|'HL'
	|	'sp'|'SP'
	;

Reg16BitIdx
	:	'ix'|'IX'
	|	'iy'|'IY'
	;

Reg16BitSpec
	:	'af\''|'AF\''
	|	'bc\''|'BC\''
	|	'de\''|'DE\''
	|	'hl\''|'HL\''
	;

FlagSpec
	:	'.z'|'.Z'|'.nz'|'.NZ'
	|	'.c'|'.C'|'.nc'|'.NC'
	|	'.pe'|'.PE'|'.po'|'.PO'
	|	'.p'|'.P'|'.m'|'.M'
	|	'.n'|'.N'|'.a'|'.A'
	|	'.h'|'.H'|'.nh'|'.NH'
	|	'.3'|'.n3'|'.N3'
	|	'.5'|'.n5'|'.N5'
	;

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

// --- Any invalid charecter should be converted into an ErrorCharacter token.
ErrorCharacter: . ;

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

