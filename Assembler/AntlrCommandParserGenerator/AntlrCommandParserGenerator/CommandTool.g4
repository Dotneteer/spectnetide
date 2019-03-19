grammar CommandTool;

/*
 * Parser Rules
 */

compileUnit
	:	toolCommand EOF
	;

toolCommand
	: gotoCommand
	| romPageCommand
	| bankPageCommand
	| memModeCommand
	| labelCommand
	| commentCommand
	| prefixCommentCommand
	| setBreakpointCommand
	| toggleBreakpointCommand
	| removeBreakpointCommand
	| updateBreakpointCommand
	| eraseAllBreakpointsCommand
	| retrieveCommand
	| literalCommand
	| disassemblyTypeCommand
	| reDisassemblyCommand
	| jumpCommand
	| sectionCommand
	| addWatchCommand
	| removeWatchCommand
	| updateWatchCommand
	| labelWidthCommand
	| exchangeWatchCommand
	| eraseAllWatchCommand
	| compactCommand
	| exportDisassemblyCommand
	| exportMemoryCommand
	;

gotoCommand: G WS? LITERAL ;
romPageCommand: R WS? LITERAL ;
bankPageCommand: B WS? LITERAL ;
memModeCommand: M ;
labelCommand: L WS? LITERAL (WS LITERAL)? ;
commentCommand: C WS? LITERAL WS? .*? ;
prefixCommentCommand: P WS? LITERAL WS? .*? ;
setBreakpointCommand: SB WS? LITERAL
	(WS H WS? (LESS|LESSEQ|GREAT|GREATEQ|EQ|MULT) WS? LITERAL)? 
	(WS C WS .*?)? ;
toggleBreakpointCommand: TB WS? LITERAL ;
removeBreakpointCommand: RB WS? LITERAL ;
updateBreakpointCommand: UB WS? LITERAL ;
eraseAllBreakpointsCommand: EB ;
retrieveCommand: RETRIEVE WS? LITERAL ;
literalCommand: D WS? LITERAL (WS (HASH|LITERAL))? ;
disassemblyTypeCommand: T WS? .*? ;
reDisassemblyCommand: RD ;
jumpCommand: J WS? LITERAL ;
sectionCommand: SECTION WS? LITERAL WS LITERAL ;
addWatchCommand: ADD .*? ;
removeWatchCommand: DASH WS? LITERAL ;
updateWatchCommand: MULT WS? LITERAL .*? ;
labelWidthCommand: LW WS? LITERAL ;
exchangeWatchCommand: XW WS? LITERAL WS LITERAL ;
eraseAllWatchCommand: EW ;
compactCommand: LITERAL .*?;
exportDisassemblyCommand: XD WS? LITERAL WS LITERAL ;
exportMemoryCommand: XM WS? LITERAL WS LITERAL ;

/*
 * Lexer Rules
 */

WS:	(' '|'\t')+	;

LESS: '<' ;
LESSEQ: '<=' ;
GREAT: '>' ;
GREATEQ: '>=' ;
EQ: '=' ;
MULT: '*' ;
HASH: '#';
ADD: '+' ;
DASH: '-' ;
COLON: ':' ;

B: 'b'|'B' ;
C: 'c'|'C' ;
D: 'd'|'D' ;
EB: 'eb'|'eB'|'Eb'|'EB' ;
EW: 'ew'|'eW'|'Ew'|'EW' ;
G: 'g'|'G' ;
GS: 'gs'|'gS'|'Gs'|'GS' ;
G1: 'g1'|'G1' ;
G2: 'g2'|'G2' ;
G3: 'g3'|'G3' ;
G4: 'g4'|'G4' ;
H: 'h'|'H' ;
J: 'j'|'J' ;
L: 'l'|'L' ;
LW: 'lw'|'Lw'|'lW'|'LW' ;
M: 'm'|'M' ;
P: 'p'|'P' ;
R: 'r'|'R' ;
RD: 'rd'|'rD'|'Rd'|'RD' ;
S: 's'|'S' ;
SB: 'sb'|'sB'|'Sb'|'SB' ;
T: 't'|'T' ;
TB: 'tb'|'tB'|'Tb'|'TB' ;
RB: 'rb'|'rB'|'Rb'|'RB' ;
UB: 'ub'|'uB'|'Ub'|'UB' ;
XD: 'xd'|'xD'|'Xd'|'XD' ;
XM: 'xm'|'xM'|'Xm'|'XM' ;
XW: 'xw'|'xW'|'Xw'|'XW' ;
W: 'w'|'W' ;
RETRIEVE: R (L|C|P) ;
SECTION: M (B|D|W|S|C|G|G1|G2|G3|G4) ;

LITERAL: COLON? LITCH+	;
LITCH: '_' | '@' | '!' | '?' | '#' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;

OTHER: . ;
