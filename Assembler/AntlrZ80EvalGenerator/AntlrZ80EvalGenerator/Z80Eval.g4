grammar Z80Eval;

/*
 * Parser Rules
 */

compileUnit
	: expr? formatSpec? EOF
	;

formatSpec
	: BYTEF
	| CHARF
	| HEX2F
	| HEX4F
	| HEX8F
	| WORDF
	| DWORDF
	| BITV8F
	| BITV16F
	| STR0F
	| STRF
	;

// --- Expressions
expr
	: orExpr (QMARK expr COLON expr)?
	; 

orExpr
	: xorExpr (VBAR xorExpr)*
	;

xorExpr
	: andExpr (UPARR andExpr)*
	;

andExpr
	: equExpr (AMP equExpr)*
	;

equExpr
	: relExpr ((EQOP | NEQOP | CIEQOP | CINEQOP) relExpr)*
	;

relExpr
	: shiftExpr ((LTOP | LTEOP | GTOP | GTEOP) shiftExpr)*
	;

shiftExpr
	: addExpr ((LSHOP | RSHOP) addExpr)*
	;

addExpr
	: multExpr ((PLUS | MINUS ) multExpr)*
	;

multExpr
	: unaryExpr ((MULOP | DIVOP | MODOP) unaryExpr)*
	;

unaryExpr
	: PLUS unaryExpr
	| MINUS unaryExpr
	| TILDE unaryExpr
	| EXCLM unaryExpr
	| LPAR expr RPAR
	| literalExpr
	| symbolExpr
	;

literalExpr
	: HEXNUM 
	| DECNUM 
	| CHAR
	| BINNUM
	| z80Spec
	;

symbolExpr
	: IDENTIFIER
	;

z80Spec
	: reg8
	| reg8Idx
	| reg8Spec
	| reg16
	| reg16Idx
	| reg16Spec
	| regIndirect
	| memIndirect
	| flags
	;

reg8: A | B | C | D | E | H | L ;
reg8Idx: XL | XH | YL | YH ;
reg8Spec: I | R ;
reg16: BC | DE | HL | SP ;
reg16Idx: IX | IY ;
reg16Std: BC | DE | HL | IX | IY ;
reg16Spec: AF | AF_ ;
regIndirect: LPAR (reg16) RPAR ;
memIndirect: LSBRAC expr RSBRAC ;
flags: ZF | NZF | CF | NCF | POF | PEF | PF | MF ;

/*
 * Lexer Rules
 */

WS
	:	' ' -> channel(HIDDEN)
	;

COLON	: ':' ;
SCOLON	: ';' ;
COMMA	: ',' ;
ASSIGN	: '=' ;
LPAR	: '(' ;
RPAR	: ')' ;
LSBRAC	: '[' ;
RSBRAC	: ']' ;
QMARK	: '?' ;
PLUS	: '+' ;
MINUS	: '-' ;
VBAR	: '|' ;
UPARR	: '^' ;
AMP		: '&' ;
EQOP	: '==' ;
CIEQOP	: '===' ;
NEQOP	: '!=' ;
CINEQOP	: '!==' ;
LTOP	: '<' ;
LTEOP	: '<=' ;
GTOP	: '>' ;
GTEOP	: '>=' ;
LSHOP	: '<<' ;
RSHOP	: '>>' ;
MULOP	: '*' ;
DIVOP	: '/' ;
MODOP	: '%' ;
TILDE	: '~';
LDBRAC	: '{{' ;
RDBRAC	: '}}' ;
EXCLM	: '!' ;

// --- Register and flag tokens
A	: 'a'|'A' ;
B	: 'b'|'B' ;
C	: 'c'|'C' ;
D	: 'd'|'D' ;
E	: 'e'|'E' ;
H	: 'h'|'H' ;
L	: 'l'|'L' ;
I	: 'i'|'I' ;
R	: 'r'|'R' ;
XL	: 'xl'|'XL'|'ixl'|'IXL'|'IXl' ;
XH	: 'xh'|'XH'|'ixh'|'IXH'|'IXh' ;
YL	: 'yl'|'YL'|'iyl'|'IYL'|'IYl' ;
YH	: 'yh'|'YH'|'iyh'|'IYH'|'IYh' ;
BC	: 'bc'|'BC' ;
DE	: 'de'|'DE' ;
HL	: 'hl'|'HL' ;
SP	: 'sp'|'SP' ;
IX	: 'ix'|'IX' ;
IY	: 'iy'|'IY' ;
AF	: 'af'|'AF' ;
AF_	: 'af\''|'AF\'' ;
ZF	: '`z'|'`Z' ;
NZF	: '`nz'|'`NZ' ;
CF	: '`c'|'`C' ;
NCF	: '`nc'|'`NC' ;
POF	: '`po'|'`PO' ;
PEF	: '`pe'|'`PE' ;
PF	: '`p'|'`P' ;
MF	: '`m'|'`M' ;

// --- Basic literals
HEXNUM	: ('#'|'0x'|'$') HexDigit HexDigit? HexDigit? HexDigit?
		| Digit HexDigit? HexDigit? HexDigit? HexDigit? ('H' | 'h') ;
BINNUM	: ('%'| ('0b' '_'?)) BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		;
DECNUM	: Digit Digit? Digit? Digit? Digit?;
CHAR	: '\'' (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter) '\'' ;

// --- Boolean literals;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;

// --- Format specifiers
BYTEF	: ':b' | ':B' ;
CHARF	: ':c' | ':C' ;
HEX2F	: ':h2' | ':H2' ;
HEX4F	: ':h4' | ':H4' ;
HEX8F	: ':h8' | ':H8' ;
WORDF	: ':w' | ':W' ;
DWORDF	: ':dw' | ':DW' ;
BITV8F	: ':bv8' | ':BV8' ;
BITV16F	: ':bv16' | ':BV16' ;
STR0F	: ':s0' | ':S0' ;
STRF	: ':s' | ':S' ;

// --- Any invalid character should be converted into an ErrorCharacter token.
ErrorCharacter
    :   .
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
	| '\\'
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
