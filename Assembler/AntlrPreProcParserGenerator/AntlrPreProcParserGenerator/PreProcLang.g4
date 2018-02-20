grammar PreProcLang;

/*
 * Parser Rules
 */

compileUnit
	:	preprocessorGroup* EOF
	;

preprocessorGroup
	:	preprocessorPart+
	;

preprocessorPart
	:	ifGroup	
	|	controlLine
	|	sourceLine
	;

ifGroup
	:	ifSection elifSection* elseSection? endifDirective
	;

ifSection
	:	WS* ifDirective WS* NEWLINE
		sourceLine*
	;

ifDirective
	:	'#if' WS* expr
	|	'#ifdef' WS* IDENTIFIER
	|	'#ifndef' WS* IDENTIFIER
	;

elifSection
	:	WS* '#elif' WS* expr WS* NEWLINE
		sourceLine*
	;

elseSection
	:	WS* '#else' WS* NEWLINE
		sourceLine*
	;

endifDirective
	:	WS* '#endif' WS* NEWLINE
	;

controlLine
	:	WS* controlDirective WS* NEWLINE
	;

controlDirective
	:	includeDir
	|	macroDir
	|	defineDir
	|	undefDir
	;

includeDir
	:	'#include' WS* (STRING | FSTRING)
	; 

macroDir
	:	'#define' WS* IDENTIFIER 
			WS* '('
			WS* IDENTIFIER (WS* ',' WS* IDENTIFIER)* 
			WS* ')' WS*
			INPUTCH*
	;

defineDir
	:	'#define' WS* IDENTIFIER WS* INPUTCH*
	;

undefDir
	:	'#undef' WS* IDENTIFIER
	;

sourceLine
	:	INPUTCH* NEWLINE
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
	| '(' expr ')'
	| literalExpr
	| symbolExpr
	;

literalExpr
	: DECNUM 
	| HEXNUM 
	| CHAR
	| BINNUM
	;

symbolExpr
	: IDENTIFIER
	;

/*
 * Lexer Rules
 */

fragment Whitespace
	:	UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	|	'\u0009' //'<Horizontal Tab Character (U+0009)>'
	|	'\u000B' //'<Vertical Tab Character (U+000B)>'
	|	'\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	:	'\u0020' // SPACE
	|	'\u00A0' // NO_BREAK SPACE
	|	'\u1680' // OGHAM SPACE MARK
	|	'\u180E' // MONGOLIAN VOWEL SEPARATOR
	|	'\u2000' // EN QUAD
	|	'\u2001' // EM QUAD
	|	'\u2002' // EN SPACE
	|	'\u2003' // EM SPACE
	|	'\u2004' // THREE_PER_EM SPACE
	|	'\u2005' // FOUR_PER_EM SPACE
	|	'\u2006' // SIX_PER_EM SPACE
	|	'\u2008' // PUNCTUATION SPACE
	|	'\u2009' // THIN SPACE
	|	'\u200A' // HAIR SPACE
	|	'\u202F' // NARROW NO_BREAK SPACE
	|	'\u3000' // IDEOGRAPHIC SPACE
	|	'\u205F' // MEDIUM MATHEMATICAL SPACE
	;

fragment NewLine
	:	'\r\n' | '\r' | '\n'
	|	'\u0085' // <Next Line CHARACTER (U+0085)>'
	|	'\u2028' //'<Line Separator CHARACTER (U+2028)>'
	|	'\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment InputCharacter
	:	~[\r\n\u0085\u2028\u2029]
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

WS		: Whitespace;
NEWLINE	: NewLine;
INPUTCH	: InputCharacter;

// --- Basic literals
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
FSTRING	: '<'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '>' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;
