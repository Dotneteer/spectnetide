---
layout: documents
categories: 
  - "Spectrum48 BASIC"
title:  "Appendix A: The character set"
alias: basic-appa
seqno: 280
selector: spectrum
permalink: "spectrum/basic-appa"
---

This is the complete Spectrum character set, with codes in decimal and hex. If one imagines the codes as being Z80 machine code instructions, then the right
hand columns give the corresponding assembly language mnemonics. As you are probably aware if you understand these things, certain Z80 instructions are
compounds starting with CBh or EDh; the two right hand columns give these.

|Code|Character|Hex|Z80 Assembler|after CB|after ED|
|---|---|---|---|---|---|
|0|not used|00|nop|rlc b||
|1|not used|01|ld bc,NN|rlc c||
|2|not used|02|ld (bc),a|rlc d||
|3|not used|03|inc bc|rlc e||
|4|not used|04|inc b|rlc h||
|5|not used|05|dec b|rlc l||
|6|PRINT comma|06|ld b,N|rlc (hl)||
|7|EDIT|07|rlca|rlc a||
|8|Cursor left|08|ex af,af'|rrc b|
|9|Cursor right|09|add hl,bc|rrc c|
|10|Cursor Down|0A|ld a,(bc)|rrc d||
|11|Cursor Up|0B|dec bc|rrc e||
|12|DELETE|0C|incc|rrch||
|13|ENTER|0D|dec c|rrc||
|14|number|0E|ld c,N|rrc (hl)||
|15|not used|0F|rrca|rrc a||
|16|INK control|10|djnz DIS|rl b||
|17|PAPER control|11|ld de,NN|rlc||
|18|FLASH control|12|ld (de),a|rl d||
|19|BRIGHT control|13|inc de|rl e||
|20|INVERSE control|14|inc d|rl h||
|21|OVER control|15|dec d|rl l||
|22|AT control|16|ld d,N|rl (hl)||
|23|TAB control|17|rla|rl a||
|24|not used|18|jr DIS|rr b||
|25|not used|19|add hl,de|rr c||
|26|not used|1A|ld a,(de)|rr d||
|27|not used|1B|dec de|rr e||
|28|not used|1C|inc e|rr h||
|29|not used|1D|dec e|rr l||
|30|not used|1E|ld e,N|rr (hl)||
|31|not used|1F|rra|rr a||
|32|space|20|jr nz, DlS|sla b||
|33|!|21|ld hl,NN|sla c||
|34|"|22|ld (NN),hl|sla d||
|35|#|23|inc hl|sla e||
|36|$|24|inc h|sla h||
|37|%|25|dec h|sla l|
|38|&|26|ld h,N|sla (hl)||
|39|,|27|daa|sla a||
|40|(|28|jr z,DlS|sra b||
|41|)|29|add hl,hl|sra c||
|42|*|2A|ld hl,(NN)|sra d|
|43|+|2B|dec hl|sra e|
|44|,|2C|inc ll|sra h|
|45|-|2D|dec l|sra l|
|46|.|2F|ld l,N|sra (hl)|
|47|/|2F|cpl|sra a|
|48|0|30|jr nc,DlS||
|49|1|31|ld sp,NN||
|50|2|32|ld (NN),a||
|51|3|33|inc sp||
|52|4|34|inc (hl)||
|53|5|35|dec (hl)||
|54|6|36|ld (hl),N||
|55|7|37|scf||
|56|8|38|lr c,DlS|srl b|
|57|9|39|add hl,sp|srl c|
|58|:|3A|ld a,(NN)|srl d|
|59|;|3B|dec sp|srl e|
|60|<|3C|inc a|srl h|
|61|=|3D|dec a|srl l|
|62|>|3E|ld a,N|srl (hl)|
|63|?|3F|ccf|srl a|
|64|@|40|ld b,b|bit 0,b|in b,(c)|
|65|A|41|ld b,c|bit 0,c|out (c),b|
|66|B|42|ld b,d|bit 0,d|sbc hl,bc|
|67|C|43|ld b,e|bit 0,e|ld (NN),bc|
|68|D|44|ld b,h|bit 0,h|neg|
|69|E|45|ld b,l|bit 0,1|retn|
|70|F|46|ld b,(hl)|bit 0,(hl)|im 0|
|71|G|47|ld b,a|bit 0,a|ld i,a|
|72|H|48|ld c,b|bit 1,b|in c,(c)|
|73|I|49|ld cc|bit 1,c|out (c),c|
|74|J|4A|ld c,d|bit i,d|adc hl,bc|
|75|K|4B|ld c,e|bit 1,e|ld bc,(NN)|
|76|L|4C|ld c,h|bit 1,h|
|77|M|4D|ld c,l|bit 1,l|reti|
|78|N|4E|ld c,(hl)|bit 1,(hl)|
|79|O|4F|ld c,a|bit 1,a|ld r,a|
|80|P|50|ld d,b|bit 2,b|in d,(c)|
|81|Q|51|ld d,c|bit 2,c|out (c),d|
|82|R|52|ld d,d|bit 2,d|sbc hl,de|
|83|S|53|ld d,e|bit 2,e|ld (NN),de|
|84|T|54|ld d,h|bit 2,h|
|85|U|55|ld d,l|bit 2,l|
|86|V|56|ld d,(hl)|bit 2,(hl)|im 1|
|87|W|57|ld d,a|bit 2,a|ld a,i|
|88|X|58|ld e,b|bit 3,b|in e,(c)|
|89|Y|59|ld e,c|bit 3,c|out (c),e|
|90|Z|5A|ld e,d|bit 3,d|adc hl,de|
|91|[|5B|ld e,e|bit 3,e|ld de,(NN)|
|92|/|5C|ld e,h|bit 3,h|
|93|]|5D|ld e,l|bit 3,l|
|94|^|5E|ld e,(hl)|bit 3,(hl)|im 2|
|95|_|5F|ld e,a|bit 3,a|ld a,r|
|96|ukp|60|ld h,b|bit 4,b|in h,(c)|
|97|a|61|ld h,c|bit 4,c|out (c),h|
|98|b|62|ld h,d|bit 4,d|sbc hl,hl|
|99|c|63|ld h,e|bit 4,e|ld (NN),hl|
|100|d|64|ld h,h|bit 4,h|
|101|e|65|ld h,l|bit 4,1|
|102|f|66|ld h,(hl)|bit 4,(hl)|
|103|g|67|ld h,a|bit 4,a|rrd|
|104|h|68|ld l,b|bit 5,b|in l,(c)|
|105|i|69|ld l,c|bit 5,c|out (c),l|
|106|j|6A|ld l,d|bit 5,d|adc hl,hl|
|107|k|6B|ld l,e|bit 5,e|ld hl,(NN),sp|
|108|l|6C|ld l,h|bit 5,h|
|109|m|6D|ld l,l|bit 5,l|
|110|n|6E|ld l,(hl)|bit 5,(hl)|
|111|o|6F|ld l,a|bit 5,a|rld|
|112|p|70|ld (hl),b|bit 6,b|in f,(c)|
|113|q|71|ld (hl),c|bit 6,c|
|114|r|72|ld (hl),d|bit 6,d|sbc hl,sp|
|115|s|73|ld (hl),e|bit 6,e|ld (NN),sp|
|116|t|74|ld (hl),h|bit 6,h|
|117|u|75|ld (hl),l|bit 6,l|
|118|v|76|halt|bit 6,(hl)|
|119|w|77|ld (hl),a|bit 6,a|
|120|x|78|ld a,b|bit 7,b|in a,(c)|
|121|y|79|ld a,c|bit 7,c|out (c),a|
|122|z|7A|ld a,d|bit 7,d|adc hl,sp|
|123|{|7B|ld a,e|bit 7,e|ld sp,(NN)|
|124|||7C|lda,h|bit 7,h|
|125|}|7D|ld al|bit 7,l|
|126|-|7E|ld a,(hl)|bit 7,(hl)|
|127|©|7F|ld a,a|bit 7,a|
|128|![ ]({{ site.baseurl }}/assets/images/basic/128.gif)|80|add a,b|res 0,b|
|129|![ ]({{ site.baseurl }}/assets/images/basic/129.gif)|81|add a,c|res 0,c|
|130|![ ]({{ site.baseurl }}/assets/images/basic/130.gif)|82|add a,d|res 0,d|
|131|![ ]({{ site.baseurl }}/assets/images/basic/131.gif)|83|add a,e|res 0,e|
|132|![ ]({{ site.baseurl }}/assets/images/basic/132.gif)|84|add a,h|res 0,h|
|133|![ ]({{ site.baseurl }}/assets/images/basic/133.gif)|85|add a,l|res 0,l|
|134|![ ]({{ site.baseurl }}/assets/images/basic/134.gif)|86|add a,(hl)|res 0,(hl)|
|135|![ ]({{ site.baseurl }}/assets/images/basic/135.gif)|87|add a,a|res 0,a|
|136|![ ]({{ site.baseurl }}/assets/images/basic/136.gif)|88|adc a,b|res 1,b|
|137|![ ]({{ site.baseurl }}/assets/images/basic/137.gif)|89|adc a,c|res 1,c|
|138|![ ]({{ site.baseurl }}/assets/images/basic/138.gif)|8A|adc a,d|res 1,d|
|139|![ ]({{ site.baseurl }}/assets/images/basic/139.gif)|8B|adc a,e|res 1,e|
|140|![ ]({{ site.baseurl }}/assets/images/basic/140.gif)|8C|adc a,h|res 1,h|
|141|![ ]({{ site.baseurl }}/assets/images/basic/141.gif)|8D|adc a,l|res 1,i|
|142|![ ]({{ site.baseurl }}/assets/images/basic/142.gif)|8E|adc a,(hl)|res 1,(hl)|
|143|![ ]({{ site.baseurl }}/assets/images/basic/143.gif)|8F|adc a,a|res 1,a|
|144|(a)|90|sub b|res 2,b|
|145|(b)|91|sub c|res 2,c|
|146|(c)|92|sub d|res 2,d|
|147|(d)|93|sub e|res 2,e|
|148|(e)|94|sub h|res 2,h|
|149|(f)|95|sub l|res 2,l|
|150|(g)|96|sub (hl)|res 2,(hl)|
|151|(h)|97|sub a|res 2,a|
|152|(i)|98|sbc a,b|res 3,b|
|153|(j)|99|sbc a,c|res 3,c|
|154|(k)|9A|sbc a,d|res 3,d|
|155|(l)|9B|sbc a,e|res 3,e|
|156|(m)|9C|sbc a,h|res 3,h|
|157|(n)|9D|sbc a,l|res 3,l|
|158|(o)|9E|sbc a,(hl)|res 3,(hl)|
|159|(p)|9F|sbc a,a|res 3,a|
|160|(q)|A0|and b|res 4,b|ldi|
|161|(r)|A1|and c|res 4,c|cpi|
|162|(s)|A2|and d|res 4,d|ini|
|163|(t)|A3|and e|res 4,e|outi|
|164|(u)|A4|and h|res 4,h|
|165|**RND**|A5|and l|res 4,l|
|166|**INKEY$**|A6|and (hl)|res 4,(hl)|
|167|**PI**|A7|and a|res 4,a|
|168|**FN**|A8|xor b|res 5,b|ldd|
|169|**POINT**|A9|xor c|res 5,c|cpd|
|170|**SCREEN$**|AA|xor d|res 5,d|ind|
|171|**ATTR**|AB|xor e|res 5,e|outd|
|172|**AT**|AC|xor h|res 5,h|
|173|**TAB**|AD|xor l|res 5,i|
|174|**VAL$**|AE|xor (hl)|res 5,(hl)|
|175|**CODE**|AF|xor a|res 5,a|
|176|**VAL**|B0|or b|res 6,b|ldir|
|177|**LEN**|B1|or c|res 6,c|cpir|
|178|**SIN**|B2|or d|res 6,d|inir|
|179|**COS**|B3|or e|res 6,e|otir|
|180|**TAN**|B4|or h|res 6,h|
|181|**ASN**|B5|or l|res 6,l|
|182|**ACS**|B6|or (hl)|res 6,(hl)|
|183|**ATN**|B7|or a|res 6,a|
|184|**LN**|B8|cp b|res 7,b|lddr|
|185|**EXP**|B9|cp c|res 7,c|cpdr|
|186|**INT**|BA|cp d|res 7,d|indr|
|187|**SOR**|BB|cp e|res 7,e|otdr|
|188|**SGN**|BC|cp h|res 7,h|
|189|**ABS**|BD|cp l|res 7,l|
|190|**PEEK**|BE|cp (hl)|res 7,(hl)|
|191|**IN**|BF|cp a|res 7,a|
|192|**USR**|C0|ret nz|set 0,b|
|193|**STR$**|C1|pop bc|set 0,c|
|194|**CHR$**|C2|jp nz,NN|set 0,d|
|195|**NOT**|C3|jp NN|set 0,e|
|196|**BIN**|C4|call nz,NN|set 0,h|
|197|**OR**|C5|push bc|set 0,l|
|198|**AND**|C6|add a,N|set 0,(hl)|
|199|**<=**|C7|rst 0|set 0,a|
|200|**>=**|C8|ret z|set 1,b|
|201|**<>**|C9|ret|set l,c|
|202|**LINE**|CA|jp z,NN|set l,d|
|203|**THEN**|CB|set l,e|
|204|**TO**|CC|call z,NN|set l,h|
|205|**STEP**|CD|call NN|set 1,l|
|206|**DEF FN**|CE|adc a,N|set 1,(hl)|
|207|**CAT**|CF|rst B|set 1,a|
|208|**FORMA**T|D0|ret nc|set 2,b|
|209|**MOVE**|D1|pop de|set 2,c|
|210|**ERASE**|D2|jpnc,NN|set 2,d|
|211|**OPEN #**|D3|out (N),a|set 2,e|
|212|**CLOSE #**|D4|call nc,NN|set 2,h|
|213|**MERGE**|D5|push de|set 2,l|
|214|**VERIFY**|D6|sub N|set 2,(hl)|
|215|**BEEP**|D7|rst 16|set 2,a|
|216|**CIRCLE**|D8|ret c|set 3,b|
|217|**INK**|D9|exx|set 3,c|
|218|**PAPER**|DA|jpc,NN|set 3,d|
|219|**FLASH**|DB|in a,(N)|set 3,e|
|220|**BRIGHT**|DC|call c,NN|set 3,h|
|221|**INVERSE**|DD|[prefixes instructions using ix]|set 3,l|
|222|**OVER**|DE|sbc a,N|set 3,(hl)|
|223|**OUT**|DF|rst 24|set 3,a|
|224|**LPRINT**|E0|ret po|set 4,b|
|225|**LLIST**|E1|pop hl|set 4,c|
|226|**STOP**|E2|jp po,NN|set 4,d|
|227|**READ**|E3|ex (sp),hl|set 4,e|
|228|**DATA**|E4|call po,NN|set 4,h|
|229|**RESTORE**|E5|push hl|set 4,l|
|230|**NEW**|E6|and N|set 4,(hl)|
|231|**BORDER**|E7|rst 32|set 4,a|
|232|**CONTINUE**|E8|ret pe|set 5,b|
|233|**DIM**|E9|jp (hl)|set 5,c|
|234|**REM**|EA|jp pe,NN|set 5,d|
|235|**FOR**|EB|ex de,hl|set 5,e|
|236|**GO TO**|EC|call pe,NN|set 5,h|
|237|**GO SUB**|ED||set 5,l|
|238|**INPUT**|EE|xor N|set 5,(hl)|
|239|**LOAD**|EF|rst 40|set 5,a|
|240|**LIST**|F0|ret p|set 6,b|
|241|**LET**|Fl|pop af|set 6,c|
|242|**PAUSE**|F2|jp p,NN|set 6,d|
|243|**NEXT**|F3|di|set 6,e|
|244|**POKE**|F4|call p,NN|set 6,h|
|245|**PRINT**|F5|push af|set 6,l|
|246|**PLOT**|F6|or N|set 6,(hl)|
|247|**RUN**|F7|rst 48|set 6,a|
|248|**SAVE**|F8|ret m|set 7,b|
|249|**RANDOMIZE**|F9|ld sp,hl|set 7,c|
|250|**IF**|FA|jp m,NN|set 7,d|
|251|**CLS**|FB|ei|set 7,e|
|252|**DRAW**|FC|call m,NN|set 7,h|
|253|**CLEAR**|FD|[prefixes instructions using iy]|set 7,l|
|254|**RETUR**N|FE|cp N|set 7,(hl)|
|255|**COPY**|FF|rst 56|set 7,a||
