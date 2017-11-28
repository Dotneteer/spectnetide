; **************************************************
; *** SPECTRUM +3 ROM 0 DISASSEMBLY (EDITOR ROM) ***
; **************************************************

; The Spectrum ROMs are copyright Amstrad, who have kindly given permission
; to reverse engineer and publish Spectrum ROM disassemblies.


; =====
; NOTES
; =====

; ------------
; Release Date
; ------------
; 17th May 2010

; ------------------------
; Disassembly Contributors
; ------------------------
; Garry Lancaster
;
; The ROM disassembly was created with the aid of dZ80 V1.10, and incorporates work from
; "The canonical list of +3 oddities" by Ian Collier. 

; -----------------
; Assembler Details
; -----------------

; This file can be assembled to produce a binary image of the ROM
; with Interlogic's Z80ASM assembler (available for Z88, QL, DOS and Linux).
; Note that the defs directive is used and this causes a block of $00 bytes to be created.

        module  rom0

;**************************************************

;        include "sysvar48.def"

; System variable definitions for 48K Spectrum

        defc    KSTATE=$5c00
        defc    LAST_K=$5c08
        defc    REPDEL=$5c09
        defc    REPPER=$5c0a
        defc    DEFADD=$5c0b
        defc    K_DATA=$5c0d
        defc    TVDATA=$5c0e
        defc    STRMS=$5c10
        defc    CHARS=$5c36
        defc    RASP=$5c38
        defc    PIP=$5c39
        defc    ERR_NR=$5c3a
        defc    FLAGS=$5c3b
        defc    TV_FLAG=$5c3c
        defc    ERR_SP=$5c3d
        defc    LIST_SP=$5c3f
        defc    MODE=$5c41
        defc    NEWPPC=$5c42
        defc    NSPPC=$5c44 
        defc    PPC=$5c45
        defc    SUBPPC=$5c47
        defc    BORDCR=$5c48
        defc    E_PPC=$5c49
        defc    VARS=$5c4b
        defc    DEST=$5c4d
        defc    CHANS=$5c4f
        defc    CURCHL=$5c51
        defc    PROG=$5c53
        defc    NXTLIN=$5c55
        defc    DATADD=$5c57
        defc    E_LINE=$5c59
        defc    K_CUR=$5c5b
        defc    CH_ADD=$5c5d
        defc    X_PTR=$5c5f
        defc    WORKSP=$5c61
        defc    STKBOT=$5c63
        defc    STKEND=$5c65
        defc    BREG=$5c67
        defc    MEM=$5c68
        defc    FLAGS2=$5c6a
        defc    DF_SZ=$5c6b
        defc    S_TOP=$5c6c
        defc    OLDPPC=$5c6e
        defc    OSPCC=$5c70
        defc    FLAGX=$5c71
        defc    STRLEN=$5c72
        defc    T_ADDR=$5c74
        defc    SEED=$5c76
        defc    FRAMES=$5c78
        defc    UDG=$5c7b
        defc    COORDS=$5c7d
        defc    P_POSN=$5c7f
        defc    PR_CC=$5c80
        defc    ECHO_E=$5c82
        defc    DF_CC=$5c84
        defc    DF_CCL=$5c86
        defc    S_POSN=$5c88
        defc    SPOSNL=$5c8a
        defc    SCR_CT=$5c8c
        defc    ATTR_P=$5c8d
        defc    MASK_P=$5c8e
        defc    ATTR_T=$5c8f
        defc    MASK_T=$5c90
        defc    P_FLAG=$5c91
        defc    MEMBOT=$5c92
        defc    NMIADD=$5cb0            ; only used in +3
        defc    RAMTOP=$5cb2
        defc    P_RAMT=$5cb4

;**************************************************

;        include "sysvarp3.def"

; Additional system variables used in the +3

        defc    SWAP=$5b00
        defc    STOO=$5b10
        defc    YOUNGER=$5b21
        defc    REGNUOY=$5b2a
        defc    ONERR=$5b3a
        defc    OLDHL=$5b52
        defc    OLDBC=$5b54
        defc    OLDAF=$5b56
        defc    TARGET=$5b58
        defc    RETADDR=$5b5a
        defc    BANKM=$5b5c
        defc    RAMRST=$5b5d
        defc    RAMERR=$5b5e
        defc    BAUD=$5b5f
        defc    SERFL=$5b61
        defc    COL=$5b63
        defc    WIDTH=$5b64
        defc    TVPARS=$5b65
        defc    FLAGS3=$5b66
        defc    BANK678=$5b67
        defc    XLOC=$5b68
        defc    YLOC=$5b69
        defc    OLDSP=$5b6a
        defc    SYNRET=$5b6c
        defc    LASTV=$5b6e
        defc    RC_LINE=$5b73
        defc    RC_START=$5b75
        defc    RC_STEP=$5b77
        defc    LODDRV=$5b79
        defc    SAVDRV=$5b7a
        defc    DUMPLF=$5b7b
        defc    STRIP1=$5b7c
        defc    STRIP2=$5b84
        defc    TSTACK=$5bff

;**************************************************

;        include "sysvarp7.def"

; System variables and data structures in DOS workspace (page 7)


; Alternate screen $c000-$daff
; Also used by +3 BASIC commands for temporary storage

        defc    src_add=$c000   ; (2) address of source filespec
        defc    dst_add=$c002   ; (2) address of dest filespec
        defc    src_drv=$c004   ; (1) source drive
        defc    dst_drv=$c005   ; (1) dest drive
        defc    eof=$c006       ; (1) EOF flag
        defc    free_m=$c007    ; (2) space free on M: for copying
        defc    copy_ram=$c009  ; (1) flag: true if copy via RAM, not M:
        defc    dst_open=$c00a  ; (1) flag: true if dst_file is open
        defc    wild=$c00b      ; (1) flag: true for wildcards in filespec
        defc    dst_dev=$c00c   ; (1) dest dev:$00=file,$e0=printer,$aa=screen
        defc    tmp_bytes=$c00d ; (2) #bytes copied to temporary file
        defc    copied=$c00f    ; (1) #files copied
        defc    dst_file=$c010  ; (18) dest filespec ($ff-term)
        defc    src_file=$c022  ; (18) source filespec ($ff-term)
        defc    cat_spec=$c034  ; (13) filespec to search catalog from
        defc    wld_next=$c041  ; (13) next filename formed from wild spec
        defc    tmp_file=$c04e  ; (18) temp filespec ($ff-term)

; +3DOS permanent structures & variables

        defc    pg_buffer=$db00 ; ($20) buffer for copying between pages
        defc    rt_alert=$db20  ; (2) ALERT routine address
        defc    al_resp=$db22   ; (7) ALERT response string
        defc    al_mess=$db29   ; ($77) message for ALERT routine
        defc    fcbs=$dba0      ; ($380) FCBs ($38 bytes each for $10 files)
	defc	sysfcb0=$df20	; ($38) system FCB 0
	defc	sysfcb1=$df58	; ($38) system FCB 1
	defc	filerecs=$df90	; (3) #recs in file (during open)
        defc    def_user=$df93  ; (1) default user area
        defc    def_drv=$df94   ; (1) default drive
	defc	extchg=$df95	; (1) extents changed in operation flag
	defc	att_clr=$df96	; (1) attributes to clear
	defc	att_set=$df97	; (1) attributes to set
	defc	cat_buff=$df98	; (2) address of catalog buffer
	defc	cat_filt=$df9a	; (1) catalog filter
	defc	cat_size=$df9b	; (1) catalog buffer size (in entries)
	defc	cat_ents=$df9c	; (1) number of completed catalog entries

        ; $df9d-$df9f unused (3 bytes)

        defc    rt_dirent=$dfa0 ; (2) routine to call for every dir entry
        defc    direntry=$dfa2  ; ($20) directory entry buffer

        ; $dfc2-$dfcf unused (14 bytes)
	; Not advisable to use, due to bug in datestamp checking routine

	defc	rw_page=$dfd0	; (1) page to read/write to
	defc	rw_add=$dfd1	; (2) address to read/write to

	; $dfd3-$dfdf unused (13 bytes)

        defc    bcbs=$dfe0      ; ($b0) BCBs ($0b bytes each for $10 buffers)
        defc    cache7=$e090    ; ($200) Page 7 cache buffer (always exists)
        defc    cachenum=$e290  ; (1) number of cache buffers
        defc    cachefst=$e291  ; (1) first cache buffer number
        defc    bcb_inuse=$e292 ; (2) inuse BCB chain
        defc    bcb_free=$e294  ; (2) free BCB chain

        ; $e296-$e29f unused (10 bytes)

        defc    xdpb_ptrs=$e2a0 ; ($20) pointers to XDPBs (or 0) for A: to P:
	defc	xdpb_a=$e2c0	; ($30) XDPB for drive A:
	defc	chksm_a=$e2f0	; ($10) checksum vector for drive A:
	defc	alloc_a=$e300	; ($2d) allocation bitmap for drive A:
	defc	xdpb_b=$e32d	; ($30) XDPB for drive B:
	defc	chksm_b=$e35d	; ($10) checksum vector for drive B:
	defc	alloc_b=$e36d	; ($2d) allocation bitmap for drive B:
        defc    xdpb_m=$e39a    ; ($30) XDPB for drive M:
	defc	alloc_m=$e3ca	; ($20) allocation bitmap for drive M:
	defc	unit0=$e3ea	; (1) drive mapped to unit 0
        defc    rt_chgdsk=$e3eb ; (2) CHANGE_DISK routine address
        defc    rt_temp=$e3ed   ; (2) address of subroutine (temporary)
        defc    spec_m=$e3ef    ; (8) disk spec for drive M:

        ; $e3f7-$e3ff unused (9 bytes)

        defc    ddl_parms=$e400 ; ($19) parameters in calls to DD_L_READ etc
        defc    rt_encode=$e419 ; (2) ENCODE routine address

        ; $e41a-$e41f unused (6 bytes)

        defc    equipment=$e420 ; (8) equipment info for FD units 0 to 3
                ; Byte 0: bits 0..1=side info (0=unknown,1/2=#sides)
                ; Byte 0: bits 2..3=track info (0=unknown,1/2=single/double)
                ; Byte 0: bit 6 set if head position known
                ; Byte 1: track under head
        defc    tm_mtron=$e428  ; (1) motor on time
        defc    tm_mtroff=$e429 ; (1) motor off time
        defc    tm_wroff=$e42a  ; (1) write off time
        defc    tm_hdset=$e42b  ; (1) head settle time
        defc    tm_step=$e42c   ; (1) step rate
        defc    retry_cnt=$e42d ; (1) retry count

        ; $e42e-$e42f unused (2 bytes)

        defc    fdc_res=$e430   ; (8) FDC results buffer

        ; $e438-$e5ff unused (456 bytes)

        defc    timeout=$e600   ; (1) current disk motor timeout

	; $e601 unused (1 byte)

; From this point, "unused" status is not 100% certain, due to limited
; knowledge of the Editor ROM

; Temporary storage used when switching ROMs etc

        defc    tmp_sp=$e602    ; (2) temporary SP store
        defc    tmp_ret=$e604   ; (2) temporary return address store
        defc    tmp_af=$e606    ; (2) temporary AF store
        defc    tmp_hl=$e608    ; (2) temporary HL store
        defc    tmp_de=$e60a    ; (2) temporary DE store
        defc    tmp_bc=$e60c    ; (2) temporary BC store

	; $e60e-$e77b unused (366 bytes)

        defc    tmp_stack=$e7ff ; ($84) temporary TSTACK store
				; from $e77c-$e7ff

	; $e800-$ebff unused (1024 bytes)

; Editor variables

        defc    men_high=$ec0c  ; (1) highlighted menu line
        defc    ed_flags=$ec0d  ; (1) - bit 1 set when processing menu
        defc    process=$ec0e   ; (1) process: $07 Loader, $04 Calculator
        defc    ed_ATTR_P=$ec0f ; (1) editor/saved ATTR_P
        defc    ed_MASK_P=$ec10 ; (1) editor/saved MASK_P
        defc    ed_ATTR_T=$ec11 ; (1) editor/saved ATTR_T
        defc    ed_MASK_T=$ec12 ; (1) editor/saved MASK_T
        defc    ed_P_FLAG=$ec13 ; (1) editor/saved P_FLAG

	; $ec20-$ecff unused (224 bytes)

; Temporary buffers/storage

        defc    tmp_fspec=$ed01 ; (??) temporary filespec workspace
        defc    tmp_buff=$ed11  ; (2048) temporary buffer for FORMAT/COPY
                                ; *BUG* means COPY uses page 0 instead of 7

        ; $f511-$f6e9 unused (473 bytes)

        defc    men_rout=$f6ea  ; (2) address of menu routines table
        defc    men_text=$f6ec  ; (2) address of menu text

	; $f700-$f8ff unused (512 bytes)

	; $fa00-$fbff unused (512 bytes)

        defc    ign_space=$fc9e ; (1) flag: if set, ignore leading space
        defc    line_add=$fc9f  ; (2) 0 or add of line data
        defc    ascii_add=$fca1 ; (2) 0 or add of ASCII expanded token/number
        defc    ascii_txt=$fca3 ; (??) bit-7 terminated ASCII text

        ; IX normally set to $fd98, so this area unknown exactly

        defc    edit_top=$fd99  ; (1) top screen line of editing area

        defc    curs_cols=$fd9e ; (1) cursor colours
        defc    curs_attr=$fd9f ; (1) saved attribute under cursor
        defc    chkword=$fda0   ; (??) word to check if token

	; $fe00-$ffff used to load bootsector by +3DOS ROM

;**************************************************

        org     $0000

.l0000  di                      ; +3 Startup address      
        ld      bc,$6c03

.l0004  dec     bc              ; Delay for approx 0.2s
        ld      a,b
        or      c
        jr      nz,l0004        
        jp      l010f           ; go to test memory

        defm    "ED"            ; Editor ROM ID
        defs    2     

; RST 10: call the RST 10 routine in ROM 3 to print a character

.l0010  rst     $28
        defw    $0010           ; call RST 10 in ROM 3
        ret
        
        defs    4     

; RST 18: call the RST 18 routine in ROM 3 to collect a character

.l0018  rst     $28
        defw    $0018           ; call RST 18 in ROM 3
        ret     

        defs    4

; RST 20: call the RST 20 routine in ROM 3 to collect next character

.l0020  rst     $28
        defw    $0020           ; call RST 20 in ROM 3
        ret     

        defs    4

; RST 28 : Call a routine in ROM 3, then return to ROM 0
; The address following the RST 28 instruction is called, then control
; is returned to the instruction following the address

.l0028  ex      (sp),hl         ; save HL, get return address
        push    af              ; save AF
        ld      a,(hl)          ; A=low byte of address to call
        inc     hl
        inc     hl              ; HL=address of instruction to return to
        ld      (RETADDR),hl    ; save

.l0030  dec     hl
        ld      h,(hl)
        ld      l,a             ; HL=address to call in ROM 3
        pop     af              ; restore AF
        jp      l00ae           ; jump on
        
        defs    1

; The maskable interrupt routine, called every 50ms while in IM1

.l0038  push    hl              ; save HL
        ld      hl,$0048
        push    hl
        ld      hl,SWAP
        push    hl
        ld      hl,$0038
        push    hl
        jp      SWAP            ; call MASK-INT and KEY-INT in ROM 3
        pop     hl              ; restore HL
        di                      ; disable interrupts again
        call    l0074
        ei                      ; re-enable interrupts
        ret     

        defs    $17

; NMI routine

.l0066  push    af              ; save AF & HL
        push    hl
        ld      hl,(NMIADD)
        ld      a,h
        or      l
        jr      z,l0070         ; skip if no routine (NMIADD=0)
        jp      (hl)            ; else execute
.l0070  pop     hl              ; restore registers
        pop     af
        retn    

; Disk motor timeout subroutine
; Called by maskable interrupt to turn off disk motor when timeout occurs

.l0074  push    af              ; save AF & BC
        push    bc
        ld      bc,$7ffd
        ld      a,(BANKM)
        or      $07
        out     (c),a           ; get page 7 (+3DOS workspace)
        ld      a,($e600)       ; check motor off timeout
        or      a
        jr      z,l00a3         ; move on if already off
        ld      a,(FRAMES)
        bit     0,a
        jr      nz,l00a3        ; only decrement every other time
        ld      a,($e600)
        dec     a
        ld      ($e600),a       ; decrement motor off timeout
        jr      nz,l00a3        ; move on if still should be on
        ld      bc,$1ffd
        ld      a,(BANK678)
        and     $f7
        ld      (BANK678),a
        out     (c),a           ; turn motor off
.l00a3  ld      bc,$7ffd
        ld      a,(BANKM)
        out     (c),a           ; page previous memory back in
        pop     bc              ; restore registers
        pop     af
        ret     

; Continuation of RST 28: call a routine in ROM 3

.l00ae  ld      (TARGET),hl     ; save ROM 3 address in TARGET
        ld      hl,YOUNGER
        ex      (sp),hl         ; stack YOUNGER address beneath TOS
        push    hl
        ld      hl,(TARGET)     ; get HL=target address in ROM 3
        ex      (sp),hl         ; restore HL & save target address on stack
        jp      SWAP            ; jump to SWAP - pages in ROM 3, returns to
                                ; target routine which returns to YOUNGER
                                ; where ROM 0 is paged back and jump made
                                ; back to RETADDR


; Here follows the five paging subroutines which are copied into
; the system variables on startup

; Enter at SWAP to change ROM 0<->3 or ROM 1<->2

.l00bd  push    af              ; save AF & BC
        push    bc
        ld      bc,$7ffd
        ld      a,(BANKM)       ; get copy of last OUT to $7ffd
        xor     $10             ; change ROM 0<->1 or ROM 2<->3
        di                      ; disable interrupts
        ld      (BANKM),a
        out     (c),a           ; page new ROM

; Enter at STOO with interrupts disabled and AF/BC stacked
; to change ROM 0<->2 or ROM 1<->3

.l00cd  ld      bc,$1ffd
        ld      a,(BANK678)     ; get copy of last OUT to $1ffd
        xor     $04             ; change ROM 0<->2 or ROM 1<->3
        ld      (BANK678),a
        out     (c),a           ; page new ROM
        ei                      ; re-enable interrupts
        pop     bc              ; restore registers
        pop     af
        ret

; Enter at YOUNGER with return address in RETADDR to swap
; ROM 0<->3 or ROM 1<->2 and return there

.l00de  call    SWAP            ; swap ROM 0<->3 or ROM 1<->2
        push    hl              ; save HL
        ld      hl,(RETADDR)    ; get return address from system vars
        ex      (sp),hl         ; restore return address & HL
        ret

; Enter at REGNUOY with return address in RETADDR to swap
; ROM 0<->2 or ROM 1<->3 and return there

.l00e7  push    hl              ; save HL
        ld      hl,$5b34
        ex      (sp),hl         ; place $5b34 as return address
        push    af              ; save AF & BC
        push    bc
        jp      STOO            ; swap ROM 0<->2 or ROM 1<->3 and return here
.l5b34  push    hl              ; save HL
        ld      hl,(RETADDR)    ; get return address from system vars
        ex      (sp),hl         ; restore return address & HL
        ret

; Enter at ONERR to page in Syntax ROM (ROM 1) and jump to error handler

.l00f7  di                      ; disable interrupts
        xor     a
        ld      bc,$1ffd
        out     (c),a           ; ensure ROM 0 or 1 is paged
        ld      (BANK678),a
        set     4,a
        ld      bc,$7ffd
        out     (c),a           ; ensure ROM 1 is paged
        ld      (BANKM),a
        ei                      ; enable interrupts
        jp      $253a           ; jump to error handler in ROM 1


; Test memory at startup & initialise

.l010f  ld      b,$08           ; 8 pages to clear

.l0111  ld      a,b
        exx     
        dec     a
        ld      bc,$7ffd
        out     (c),a           ; page next RAM page to $c000
        ld      hl,$c000
        ld      de,$c001
        ld      bc,$3fff
        ld      (hl),$00
        ldir                    ; clear it
        exx     
        djnz    l0111           ; back for more pages
        xor     a
        ld      hl,$dcba        ; an address in top 16K of ROM
        ld      bc,$7ffd        ; memory paging address

.l0130  ld      de,$0108        ; E=8 bits to test, D=bit 0
        out     (c),a           ; get next page to segment 3
        ex      af,af'          ; save A'=page

.l0136  ld      a,d             ; test to see if bit can be set
        ld      (hl),a
        ld      a,(hl)
        and     d
        jp      z,l0367         ; jump if memory not re-read correctly
        cpl                     ; test to see if bit can be reset
        ld      (hl),a        
        ld      a,(hl)
        and     d
        jp      nz,l0367        ; jump if memory not re-read correctly
        rlc     d
        dec     e
        jr      nz,l0136        ; loop back to test other bits
        ex      af,af'
        inc     a
        cp      $08
        jr      nz,l0130        ; loop back to test other pages
        ld      c,$fd
        ld      d,$ff
        ld      e,$bf
        ld      b,d
        ld      a,$0e
        out     (c),a           ; select AY register 14 (RS232/AUX)
        ld      b,e
        ld      a,$ff
        out     (c),a           ; set all RS232/AUX lines high
        jr      l0167           ; move on, with page 7 at $c000


; Apparently unused section, possibly originally intended to
; flag a memory error

        exx     
        ld      a,b
        out     ($fe),a
.l0165  jr      l0165


; More initialisation (with page 7 at $c000)

.l0167  xor     a
        ex      af,af'          ; A' clear to show reset, not NEW
        ld      sp,$6000        ; set stack within page 5        

.l016c  ld      b,d
        ld      a,$07
        out     (c),a           ; select AY register $07
        ld      b,e
        ld      a,$ff
        out     (c),a           ; initialise AY chip (?)
        ld      de,SWAP
        ld      hl,l00bd
        ld      bc,$0052
        ldir                    ; copy paging subroutines to system vars
        ld      a,$cf
        ld      (RAMRST),a      ; place RST 8 instruction at RAMRST
        ld      hl,$ffff        
        ld      (P_RAMT),hl     ; set P RAMT to 64K
        ld      de,$3eaf        ; prepare to copy chars A-U from ROM 3
        ld      bc,$00a8        ; to UDG area
        ex      de,hl
        rst     $28             ; execute a LDDR from ROM 3 to copy them
        defw    $1661
        ex      de,hl
        inc     hl
        ld      (UDG),hl        ; store address of first UDG
        dec     hl
        ld      bc,$0040
        ld      (RASP),bc       ; set RASP and PIP
        ld      (RAMTOP),hl     ; set RAMTOP below UDGs
        ld      hl,FLAGS3
        res     7,(hl)          ; reset bit 7 of FLAGS3 (??)
        ld      hl,DUMPLF
        ld      (hl),$09        ; set DUMPLF

; The NEW command enters here

.l01b0  ld      hl,$3c00
        ld      (CHARS),hl      ; set CHARS
        im      1               ; set interrupt mode 1
        ld      iy,ERR_NR       ; IY points to ERR NR
        set     4,(iy+$01)      ; set "+3 BASIC mode"
        ld      hl,FLAGS3
        res     3,(hl)          ; set "print to Centronics"
        set     2,(hl)          ; set "print expanded tokens"
        ld      hl,$000b
        ld      (BAUD),hl       ; set BAUD
        xor     a
        ld      (SERFL),a       ; clear SERFL
        ld      (COL),a         ; clear COL
        ld      (TVPARS),a      ; clear TVPARS
        ld      hl,$ec00
        ld      ($ff24),hl      ; ???
        ld      a,$50
        ld      (WIDTH),a       ; set WIDTH
        ld      hl,$000a
        ld      (RC_START),hl   ; set RCSTART
        ld      (RC_STEP),hl    ; set RCSTEP
        ld      a,$54
        ld      (LODDRV),a      ; set LODDRV to "T"
        ld      (SAVDRV),a      ; set SAVDRV to "T"
        ld      hl,$5cb6
        ld      (CHANS),hl      ; set CHANS immediately after system vars
        ld      de,l03b8
        ld      bc,$0015
        ex      de,hl
        ldir                    ; copy initial channel information
        ex      de,hl
        dec     hl
        ld      (DATADD),hl     ; set DATADD after CHANS
        inc     hl
        ld      (PROG),hl       ; set PROG after DATADD
        ld      (VARS),hl       ; set VARS
        ld      (hl),$80        ; store end of variables marker
        inc     hl
        ld      (E_LINE),hl     ; set ELINE after VARS
        ld      (hl),$0d        ; store end of line marker
        inc     hl
        ld      (hl),$80        ; store end of ELINE marker
        inc     hl
        ld      (WORKSP),hl     ; set WORKSP after ELINE
        ld      (STKBOT),hl     ; set STKBOT
        ld      (STKEND),hl     ; set STKEND
        ld      a,$38
        ld      (ATTR_P),a      ; set ATTR P
        ld      (ATTR_T),a      ; set ATTR T
        ld      (BORDCR),a      ; set BORDCR
        xor     a
        ld      (ed_P_FLAG),a   ; set editor's P_FLAG
        ld      a,$07
        out     ($fe),a         ; white border
        ld      hl,$0523
        ld      (REPDEL),hl     ; set REPDEL and REPPER
        dec     (iy-$3a)        ; set two bytes of KSTATE to $ff
        dec     (iy-$36)        
        ld      hl,l03cd
        ld      de,STRMS
        ld      bc,$000e
        ldir                    ; copy initial stream addresses to STRMS
        res     1,(iy+$01)      ; reset bit 1 of FLAGS
        ld      (iy+$00),$ff    ; set ERR NR to no error
        ld      (iy+$31),$02    ; set DF SZ
        ex      af,af'
        cp      $52
        jp      z,l2675         ; move on if in self-test program
        ld      hl,(RAMTOP)
        inc     hl
        ld      sp,hl           ; set SP to RAMTOP+1
        ei                      ; enable interrupts
        rst     $28
        defw    $0d6b           ; CLS using ROM 3
        call    l02aa           ; display test image if BREAK held down
        ld      de,l03db
        call    l029e           ; display copyright message
        ld      hl,TSTACK
        ld      (OLDSP),hl      ; set OLDSP to TSTACK area
        call    l05cc           ; switch in page 7 with stack in TSTACK
        ld      a,$38
        ld      (ed_ATTR_T),a   ; set editor's ATTR_T
        ld      (ed_ATTR_P),a   ; set editor's ATTR_P
        call    l05a7           ; switch back page 0
        call    l3e80
        defw    $2410           ; initialise DOS & display drive info
        call    l05cc           ; switch in page 7 with stack in TSTACK
        ld      (iy+$31),$02    ; set DFSZ
        set     5,(iy+$02)      ; set bit 5 of TVFLAG
        call    l0633           ; ???
        call    l05a7           ; switch back page 0
        jp      l064e           ; move on

; Print string subroutine
; Displays a string terminated by a byte with bit 7 set
; Entry: DE=address of string
; Exit: DE=address after string, A corrupted

.l029e  ld      a,(de)          ; get next character
        and     $7f             ; mask high bit
        push    de
        rst     $10             ; print it
        pop     de
        ld      a,(de)
        inc     de              ; increment address
        add     a,a
        jr      nc,l029e        ; loop back if bit 7 wasn't set
        ret     

; Check to see if BREAK is held down, entering the test image if so

.l02aa  ld      a,$7f
        in      a,($fe)
.l02ae  rra                          
        ret     c               ; exit if SPACE not held down
        ld      a,$fe
        in      a,($fe)
        rra     
        ret     c               ; exit if CAPS SHIFT not held down
        ld      a,$07
        out     ($fe),a         ; white border
        ld      a,$02
        rst     $28
        defw    $1601           ; open stream 2 for output
        xor     a
        ld      (TV_FLAG),a     ; clear TV FLAG
        ld      a,$16
        rst     $10
        xor     a
        rst     $10
        xor     a
        rst     $10             ; AT 0,0
        ld      e,$08           ; E=8, used many times in routine
        ld      b,e             ; B=8 messages per line
        ld      d,b             ; D=8 lines
.l02ce  ld      a,b             
        dec     a
        rl      a
        rl      a
        rl      a               ; A=paper colour from position along line
        add     a,d             
        dec     a               ; add in ink colour from line number
        ld      (ATTR_T),a      ; set ATTR T
        ld      hl,l03b0        ; address of '1987' test message        
        ld      c,e             ; C=8=length of message
.l02df  ld      a,(hl)
        rst     $10             ; display next character
        inc     hl
        dec     c
        jr      nz,l02df        ; loop back for more characters
        djnz    l02ce           ; loop back for more messages
        ld      b,e             ; B=8 messages per line
        dec     d
        jr      nz,l02ce        ; loop back for more lines
        ld      hl,$4800        ; start of middle third of screen
        ld      d,h
        ld      e,l
        inc     de
        xor     a
        ld      (hl),a
        ld      bc,$0fff
        ldir                    ; clear bottom two thirds of screen
        ex      de,hl
        ld      de,$5900
        ld      bc,$0200
        ldir                    ; copy attribs of top third to rest of screen
        di                      ; disable interrupts
.l0302  ld      de,$0370
        ld      l,$07
.l0307  ld      bc,$0099
.l030a  dec     bc
        ld      a,b
        or      c
        jr      nz,l030a        ; delay
        ld      a,l
        xor     $10
        ld      l,a
        out     ($fe),a         ; generate tone
        dec     de
        ld      a,d
        or      e
        jr      nz,l0307        ; loop back for tone

; Here we test for sets of keys pressed at the test image, and jump 
; to routines to handle them if necessary

        ld      de,$2000        ; DE=number of times to check for keysets
        ld      ix,l03a8        ; IX=start of keyset table
.l0321  ld      l,(ix+$00)      ; HL=next keyset start address-1
        ld      h,(ix+$01)
        inc     ix
        inc     ix              ; IX points to next entry in keyset table
        ld      a,h
        or      l
        jr      nz,l0335        ; test keyset unless at end of table
        ld      ix,l03a8        ; if so, start again at the beginning
        jr      l0321

.l0335  inc     hl              ; HL points to next keyboard scan address
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=next keyboard scan address
        inc     hl
        ld      a,b
        or      c
        jr      z,l034c         ; move on if scanned all for this keyset
        in      a,(c)
        and     $1f             ; mask keyboard (bits 0-4)
        cp      (hl)            ; check against required value
        jr      z,l0335         ; continue checking if OK
        dec     de              ; decrement number of checks counter
        ld      a,d
        or      e
        jr      nz,l0321        ; loop back to scan again
        jr      l0302           ; sound tone again

.l034c  ld      c,(hl)          ; get address of routine to execute
        inc     hl
        ld      b,(hl)
        push    bc              ; stack address
        ret                     ; and "return" to it

; Self-test keyset table
; Program accessed with "QAZPLM" held down on test screen

.l0351  defw    $fbfe
        defb    $1e             ; "Q"
        defw    $fdfe
        defb    $1e             ; "A"
        defw    $fefe
        defb    $1d             ; "Z"
        defw    $dffe
        defb    $1e             ; "P"
        defw    $bffe
        defb    $1d             ; "L"
        defw    $7ffe
        defb    $1b             ; "M"
        defw    0               ; end of keys to scan
        defw    l21df           ; routine address

; Jump here if there is a memory test error:
; if the bit couldn't be set, a border is set to the bit number,
; if it couldn't be reset, an alternating bit number/bit number XOR 7
; border is set.

.l0367  ld      a,8
        sub     e
        ex      af,af'          ; A'=bit number failed on
        and     a
        jr      nz,l0373        ; jump on if bit could be set
        ex      af,af'
        out     ($fe),a         ; else halt with border set to bit number
.l0371  jr      l0371

.l0373  ex      af,af'
        ld      c,a
        ld      b,$07
        xor     b
        ld      b,a             ; B=bit number XOR 7

.l0379  ld      a,c
        out     ($fe),a         ; set bit number border
        ld      de,$0000
.l037f  dec     de
        ld      a,d
        or      e
        jr      nz,l037f        ; pause for approx 0.5s
        ld      a,b
        out     ($fe),a         ; set bit number XOR 7 border
        ld      de,$2aaa
.l038a  dec     de
        ld      a,d
        or      e
        jr      nz,l038a        ; pause for approx 0.1s
        jr      l0379           ; loop back

; Pretty EAR monitor keyset table
; Program accessed with "EUA" held down on test screen

.l0391  defw    $fbfe           
        defb    $1b             ; "E"
        defw    $dffe
        defb    $17             ; "U"
        defw    $fdfe
        defb    $1e             ; "A"
        defw    0               ; end of keys to scan
        defw    l22d0           ; routine address

; Reboot keyset table
; Spectrum rebooted with "BV" held down on test screen

.l039e  defw    $7ffe
        defb    $0f             ; "B"    
        defw    $fefe
        defb    $0f             ; "V"
        defw    0               ; end of keys to scan
        defw    $0000           ; routine address

; The table of keyset addresses-1 scanned at startup

.l03a8  defw    l0351-1         ; self-test keyset table-1
        defw    l0391-1         ; pretty EAR monitor keyset table-1
        defw    l039e-1         ; reboot keyset table-1
        defw    0               ; end of table marker

; Text used for the test display

.l03b0  defm    $13&$0&"19"&$13&$1&"87"

; Here is the initial channel information, copied to CHANS

.l03b8  defw    $09f4
        defw    $10a8
        defb    'K'             ; keyboard/lower screen channel
        defw    $09f4
        defw    $15c4
        defb    'S'             ; main screen channel
        defw    $0f81
        defw    $15c4
        defb    'X'             ; workspace channel
        defw    $3a05
        defw    $3a00
        defb    'P'             ; printer channel
        defb    $80             ; end of channel information

; Here is the initial stream addresses, copied to STRMS

.l03cd  defw    $0001           ; stream -3, 'K'
        defw    $0006           ; stream -2, 'S'
        defw    $000b           ; stream -1, 'X'
        defw    $0001           ; stream 0, 'K'
        defw    $0001           ; stream 1, 'K'
        defw    $0006           ; stream 2, 'S'
        defw    $0010           ; stream 3, 'P'

; Copyright message

.l03db  defm    $7f&"1982, 1986, 1987 Amstrad Plc."&$8d

; Subroutine to ???

.l03fa  ld      hl,$eef5
        res     0,(hl)          ; ???
        set     1,(hl)          ; ???
.l0401  ld      hl,(E_PPC)      ; get current line
        ld      a,h
        or      l
        jr      nz,l040b        ; move on unless 0
.l0408  ld      ($ec06),hl      ; ???
.l040b  ld      a,($f9db)       ; ???
        push    af
        ld      hl,($fc9a)      ; ???
        call    l1418           ; get number of line before (or 0)
        ld      ($f9d7),hl	; ???
        call    l12f0		; ???
        call    l11a4		; ???
        pop     af
.l041f  or      a
        jr      z,l042e		; move on if ???
        push    af
.l0423  call    l11ad		; ???
        ex      de,hl
        call    $1338
        pop     af
        dec     a
        defb    $18
        defb    -15
.l042e  ld      c,$00
        call    $1182
        ld      b,c
        ld      a,($ec15)
        ld      c,a
        push    bc
        push    de
.l043a  call    $11ad
        ld      a,($eef5)
        bit     1,a
        defb    $28
        defb    29
        push    de
        push    hl
.l0446  ld      de,$0020
        add     hl,de
        bit     0,(hl)
        defb    $28
        defb    17
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      e,(hl)
        or      a
        ld      hl,(E_PPC)
        sbc     hl,de
        defb    $20
        defb    5
        ld      hl,$eef5
        set     0,(hl)
.l045f  pop     hl
        pop     de
.l0461  push    bc
        push    hl
        ld      bc,$0023
        ldir    
        pop     hl
        pop     bc
        push    de
        push    bc
        ex      de,hl
        ld      hl,$eef5
        bit     0,(hl)
        defb    $28
        defb    42
        ld      b,$00
.l0476  ld      hl,($ec06)
        ld      a,h
        or      l
        defb    $28
        defb    14
        push    hl
        call    $0f0f
        pop     hl
        defb    $30
        defb    18
        dec     hl
        inc     b
        ld      ($ec06),hl
        defb    $18
        ex      de,hl
        call    $0f0f
        call    nc,$0f31
        ld      hl,$eef5
        ld      (hl),$00
.l0496  ld      a,b
        pop     bc
        push    bc
        ld      c,b
        ld      b,a
        call    $0adc
.l049e  pop     bc
        pop     de
        ld      a,c
        inc     b
        cp      b
        defb    $30
        defb    -107
        ld      a,($eef5)
        bit     1,a
        defb    $28
        defb    33
        bit     0,a
        defb    $20
        defb    29
        ld      hl,(E_PPC)
        ld      a,h
        or      l
        defb    $28
        defb    8
        ld      ($fc9a),hl
        call    $12f0
        defb    $18
        defb    9
.l04bf  ld      ($fc9a),hl
        call    $1420
        ld      (E_PPC),hl
.l04c8  pop     de
        pop     bc
        jp      $0401

.l04cd  pop     de
        pop     bc
        cp      a

.l04d0  push    af
        ld      a,c
        ld      c,b
        call    $1182
        ex      de,hl

.l04d7  push    af
        call    $16fe
        pop     af
        ld      de,$0023
        add     hl,de
        inc     c
        cp      c
        jr      nc,l04d7            ; (-13)
        pop     af
        ret     z
        call    $0ad2

.l04e9  call    $0c43
        ld      hl,($ec06)
        dec     hl
        ld      a,h
        or      l
        ld      ($ec06),hl
        jr      nz,l04e9            ; (-14)
        jp      $0adc
        ret     

.l04fb  ld      b,$00
        ld      a,($ec15)
        ld      d,a
        jp      l1d6b   

.l0504  ld      b,$00
        push    hl
        ld      c,b
        call    $1182
        call    $1338
        pop     hl
        ret     nc
        call    $11ad

.l0513  push    bc
        push    hl
        ld      hl,$0023
        add     hl,de
        ld      a,($ec15)
        ld      c,a
        cp      b
        jr      z,l052e             ; (14)
        push    bc

.l0521  push    bc
        ld      bc,$0023
        ldir    
        pop     bc
        ld      a,c
        inc     b
        cp      b
        jr      nz,l0521            ; (-12)
        pop     bc

.l052e  pop     hl
        call    $1712
        ld      bc,$0023
        ldir    
        scf     
        pop     bc
        ret     

.l053a  ld      b,$00
        call    $12f9
        ret     nc

.l0540  push    bc
        push    hl
        ld      a,($ec15)
        ld      c,a
        call    $1182
        call    $11ec
        jr      nc,l0574            ; (38)
        dec     de
        ld      hl,$0023
        add     hl,de
        ex      de,hl
        push    bc
        ld      a,b
        cp      c
        jr      z,l0565             ; (12)

.l0559  push    bc
        ld      bc,$0023
        lddr    
        pop     bc
        ld      a,b
        dec     c
        cp      c
        jr      c,l0559             ; (-12)

.l0565  ex      de,hl
        inc     de
        pop     bc
        pop     hl
        call    $1726
        ld      bc,$0023
        ldir    
        scf     
        pop     bc
        ret     

.l0574  pop     hl
        pop     bc
        ret     

.l0577  push    de
        ld      h,$00
        ld      l,b
        add     hl,de
        ld      d,a
        ld      a,b

.l057e  ld      e,(hl)
        ld      (hl),d
        ld      d,e
        inc     hl
        inc     a
        cp      $20
        jr      c,l057e             ; (-9)
        ld      a,e
        cp      $00
        pop     de
        ret     

.l058c  push    de
        ld      hl,$0020
        add     hl,de
        push    hl
        ld      d,a
        ld      a,$1f
        jr      l059e               ; (7)

.l0597  ld      e,(hl)
        ld      (hl),d
        ld      d,e
        cp      b
        jr      z,l05a1             ; (4)
        dec     a

.l059e  dec     hl
        jr      l0597               ; (-10)

.l05a1  ld      a,e
        cp      $00
        pop     hl
        pop     de
        ret     

; Subroutine to page in normal memory (page 0) and swap SP with OLDSP

.l05a7  ex      af,af'          ; save AF
        ld      a,$00
        di      
        call    l05c1           ; page in page 0
        pop     af              ; AF holds return address
        ld      (TARGET),hl     ; save HL in TARGET
        ld      hl,(OLDSP)      ; get OLDSP
        ld      (OLDSP),sp      ; save SP in OLDSP
        ld      sp,hl           ; SP now holds what was in OLDSP
        ei      
        ld      hl,(TARGET)     ; restore HL
        push    af              ; push back return address
        ex      af,af'          ; restore AF
        ret     

; Subroutine to page in page A

.l05c1  push    bc              ; save BC
        ld      bc,$7ffd
        out     (c),a           ; change page
        ld      (BANKM),a       ; save copy of OUT
        pop     bc              ; restore BC
        ret     

; Subroutine to page in DOS workspace (page 7) and swap SP with OLDSP

.l05cc  ex      af,af'          ; save AF
        di                      
        pop     af              ; AF holds return address
        ld      (TARGET),hl     ; save HL in TARGET
        ld      hl,(OLDSP)      ; get OLDSP
        ld      (OLDSP),sp      ; save SP in OLDSP
        ld      sp,hl           ; SP now holds what was in OLDSP
        ld      hl,(TARGET)     ; restore HL
        push    af              ; push back return address
        ld      a,$07
        call    l05c1           ; page in page 7
        ei                      
        ex      af,af'          ; restore AF
        ret     

; The editing keys table
; Most of these keys are produced by the external keypad, which was not
; made available in the UK.

.l05e6  defb    $15
        defb    $0b             ; cursor up
        defw    l0b5f
        defb    $0a             ; cursor down
        defw    l0b80
        defb    $08             ; cursor left
        defw    l0ba2
        defb    $09             ; cursor right
        defw    l0bae
        defb    $ad             ; TAB (up 10)
        defw    l0b1a
        defb    $ac             ; AT (down 10)
        defw    l0af0
        defb    $af             ; CODE (left word)
        defw    l0a9f
        defb    $ae             ; VAL$ (right word)
        defw    l0aac
        defb    $a6             ; INKEY$ (top)
        defw    l0a4e
        defb    $a5             ; RND (bottom)
        defw    l0a76
        defb    $a8             ; FN (start of line)
        defw    l0b52
        defb    $a7             ; PI (end of line)
        defw    l0b45
        defb    $aa             ; SCREEN$ (delete char right)
        defw    l09e6
        defb    $0c             ; delete
        defw    l09f6
        defb    $b3             ; COS (delete word right)
        defw    l10e5
        defb    $b4             ; TAN (delete word left)
        defw    l108a
        defb    $b0             ; VAL (delete line right)
        defw    l1140
        defb    $b1             ; LEN (delete line left)
        defw    l110c
        defb    $0d             ; enter
        defw    l0a0f
        defb    $a9             ; POINT (screen)
        defw    l0748
        defb    $07             ; edit
        defw    $07b1

; The menu keys table
        
.l0626  defb    $04
        defb    $0b             ; cursor up
        defw    l07dc
        defb    $0a             ; cursor down
        defw    l07df
        defb    $07             ; edit
        defw    l07c5
        defb    $0d             ; enter
        defw    l07c5

; Subroutine to ????

.l0633  call    l0989
        ld      hl,$0000
        ld      ($fc9a),hl
        ld      a,$82
        ld      (ed_flags),a
        ld      hl,$0000
        ld      (E_PPC),hl
        call    l16b6
        call    l1758
        ret     

; Routine to display main menu & go to process it

.l064e  ld      hl,TSTACK
        ld      (OLDSP),hl      ; set "OLDSP" to temporary stack area
        call    l05cc           ; page in DOS workspace
        ld      a,$02
        rst     $28
        defw    $1601           ; open channel to stream 2
        ld      hl,l07f2
        ld      (men_rout),hl   ; store main menu routine table address
        ld      hl,l07ff        
        ld      (men_text),hl   ; store main menu text address
        push    hl              ; save menu address
        ld      hl,ed_flags
        set     1,(hl)          ; signal "processing menu"
        res     4,(hl)          ; ???
        dec     hl
        ld      (hl),$00        ; set men_high=0
        pop     hl              ; restore menu address
        xor     a
        call    l189a           ; display main menu
        jp      l0703           ; move to process menu


.l067b  ld      ix,$fd98
        ld      hl,TSTACK
        ld      (OLDSP),hl
        call    $05cc
        ld      a,$02
        rst     $28
        defw    $1601
        defb    $cd
        ld      e,d
        defb    $18
        defb    33
        dec     sp
        ld      e,h

.l0693  bit     5,(hl)
        jr      z,l0693             ; (-4)
        ld      hl,ed_flags
        res     3,(hl)
        bit     6,(hl)
        jr      nz,l06b4            ; (20)
        ld      a,($ec0e)
        cp      $04
        jr      z,l06b1             ; (10)
        cp      $00
        jp      nz,$0992
        call    $1a5a
        jr      l06b4               ; (3)

.l06b1  call    l1a5f           ; display "Calculator" bar
.l06b4  call    $11a4
        call    $12f0
        ld      a,($ec0e)	; get current process
        cp      $04
        jr      z,l0703		; move on if its the calculator
        ld      hl,(E_PPC)      ; get number of current line
        ld      a,h
        or      l
        jr      nz,l06dd        ; move on if not zero
        ld      hl,(PROG)
        ld      bc,(VARS)
        and     a
        sbc     hl,bc           ; get length of BASIC program
        jr      nz,l06da        ; move on if not zero
        ld      hl,$0000
        ld      ($ec08),hl      ; ??? last line
.l06da  ld      hl,($ec08)      ; ??? last line
.l06dd  call    l05a7           ; page in normal memory
        rst     $28
        defw    $196e           ; ???
        rst     $28
        defw    $1695
        call    l05cc           ; page in DOS workspace
        ld      (E_PPC),de
        ld      hl,ed_flags
        bit     5,(hl)
        jr      nz,l0703            ; (15)
        ld      hl,$0000
        ld      ($ec06),hl
        call    l03fa
        call    $0abd
        call    $0a0f

; Main routine to process menus & editing functions

.l0703  ld      sp,TSTACK       ; set SP in temporary stack
.l0706  call    l1871           ; get a key
        push    af
        ld      a,(PIP)
        call    l0799           ; sound a 'PIP'
        pop     af
        call    l0716           ; "do" the key
        jr      l0706           ; loop back
.l0716  ld      hl,ed_flags
        bit     1,(hl)          ; check editing/menu flag
        push    af
        ld      hl,l0626        ; use menu keys table
        jr      nz,l0724
        ld      hl,l05e6        ; or editing keys table
.l0724  call    l2166           ; perform pressed key action
        jr      nz,l072e
        call    nc,l0794        ; sound a RASP if action failed
        pop     af              ; restore editing/menu flag status
        ret     
.l072e  pop     af              ; restore editing/menu flag status
        jr      z,l0736         ; move on if editing
        xor     a
        ld      (MODE),a        ; else in menu, so set MODE=0
        ret     
.l0736  ld      hl,ed_flags
        bit     0,(hl)
        jr      z,l0741		; move on if ???
        call    l0794		; sound a RASP
        ret     
.l0741  cp      $a3
        jr      nc,l0706	; loop back if ???
        jp      l09bc

; Editing keys: SCREEN

.l0748  ld      a,($ec0e)
        cp      $04
        ret     z               ; exit if in Calculator
        call    $04fb
        ld      hl,ed_flags
        res     3,(hl)
        ld      a,(hl)
        xor     $40
        ld      (hl),a
        and     $40
        jr      z,l0763             ; (5)
        call    $0768
        jr      l0766               ; (3)
.l0763  call    $077b
.l0766  scf     
        ret     

.l0768  call    $1a8e
        ld      hl,ed_flags
        set     6,(hl)
        call    $0efb
        call    $1c95
        call    $09aa
        jr      l0786               ; (11)

.l077b  ld      hl,ed_flags
        res     6,(hl)
        call    $0989
        call    $1a5a
.l0786  ld      hl,($fc9a)
        ld      a,h
        or      l
        call    nz,$1418
        call    $03fa
        jp      $0abd

; Subroutine to sound a PIP or RASP
; Enter at l0799 with A=PIP, or at l0794 for RASP

.l0794  ld      a,(RASP)
        srl     a               ; A=RASP/2
.l0799  push    ix              ; save IX
        ld      d,$00
        ld      e,a             ; DE=f*t
        ld      hl,$0c80        ; HL=timing constant
.l07a1  rst     $28
        defw    $03b5           ; call BEEPER
        pop     ix              ; restore IX
        ret     

; Another sound

.l07a7  push    ix
        ld      de,$0030
        ld      hl,$0300
        jr      l07a1      

; Editing keys: EDIT
        
.l07b1  call    l0ab7           ; remove cursor
        ld      hl,ed_flags
        set     1,(hl)		; set "processing menu"
        dec     hl
        ld      (hl),$00	; highlight on line 0
        ld      hl,(men_text)
        xor     a
        call    l189a		; display menu
        scf     
        ret     

; The menu ENTER/EDIT routine

.l07c5  ld      hl,ed_flags
        res     1,(hl)          ; signal editing mode
        dec     hl
        ld      a,(hl)          ; A=currently highlighted line
        ld      hl,(men_rout)   ; HL=menu routines table
        push    hl      
        push    af
        call    l1950           ; copy saved area back to screen
        pop     af
        pop     hl
        call    l2166           ; execute required routine
        jp      l0abd           ; move on

; The menu cursor up/down routines
; Enter at l07dc for up, l07df for down

.l07dc  scf                     ; set carry for cursor up     
        jr      l07e0
.l07df  and     a               ; clear carry for cursor down
.l07e0  ld      hl,men_high
        ld      a,(hl)          ; get currently highlighted line number
        push    hl
        ld      hl,(men_text)   ; point to menu
        call    c,l19b9         ; move highlight up
        call    nc,l19c8        ; or down
        pop     hl
        ld      (hl),a          ; replace highlighted line number
.l07f0  scf                     ; signal "action succeeded"
        ret     

; The main menu routine address table

.l07f2  defb    $04
        defb    $00
        defw    l08e8           ; Loader
        defb    $01
        defw    l0937           ; +3 BASIC
        defb    $02
        defw    l0950           ; Calculator
        defb    $03
        defw    l08df           ; 48K BASIC

; The main menu

.l07ff  defb    $05             ; 5 lines total
        defm    "128 +3  "&$ff
.l0809  defm    "Loade"&('r'+$80)
.l080f  defm    "+3 BASI"&('C'+$80)
.l0817  defm    "Calculato"&('r'+$80)
        defm    "48 BASI"&('C'+$80)
        defb    ' '+$80

; The editor menu routine address table

.l082a  defb    $05
        defb    $00
        defw    l07f0           ; +3 BASIC
        defb    $01
        defw    l0917           ; Renumber
        defb    $02
        defw    l08c5           ; Screen
        defb    $03
        defw    l0928           ; Print
        defb    $04
        defw    l08ca           ; Exit

; The editor menu

.l083a  defb    $06
        defm    "Options "&$ff
        defm    "+3 BASI"&('C'+$80)
        defm    "Renumbe"&('r'+$80)
        defm    "Scree"&('n'+$80)
        defm    "Prin"&('t'+$80)
        defm    "Exi"&('t'+$80)
        defb    ' '+$80
        
; The calculator menu routines table

.l0864  defb    $02
        defb    $00
        defw    l07f0           ; Calculator
        defb    $01
        defw    l08ca           ; Exit
        
; The calculator menu

.l086b  defb    $03
        defm    "Options "&$ff
        defm    "Calculato"&('r'+$80)
        defm    "Exi"&('t'+$80)
        defb    ' '+$80

; Cassette loader message

.l0884  defm    $16&$00&$00
        defm    $10&$00&$11&$07
        defm    $13&$00
        defm    "Insert tape and press PLAY"&$0d
        defm    "To cancel - press BREAK twic"&('e'+$80)

; The Screen menu option

.l08c5  call    l0748           ; call SCREEN editing key routine
        jr      l093f		; ???

; The "Exit" from submenu option

.l08ca  ld      hl,ed_flags
        res     6,(hl)		; ???
        call    l0989		; ???
        ld      b,$00
        ld      d,$17
        call    l1d6b           ; clear whole screen to editor colours
        call    l05a7		; page in normal memory
        jp      l064e		; display main menu & process it

; The 48K BASIC menu option
        
.l08df  call    l05a7           ; page in normal memory
        call    l3e80
        defw    $1488           ; enter 48K BASIC via ROM 1
        ret     
        
; The Loader menu option

.l08e8  call    l1a64           ; display "Loader" bar
        ld      hl,TV_FLAG
        set     0,(hl)		; signal "using lower screen"
        ld      de,$0884
        push    hl
.l08f4  ld      hl,FLAGS3
        bit     4,(hl)
        pop     hl
        jr      nz,l08ff	; move on if disk interface present
        call    l029e		; display cassette loader message
.l08ff  res     0,(hl)		; ???
        set     6,(hl)		; ???
        ld      a,$07
        ld      ($ec0e),a       ; signal "current process is Loader"
        ld      bc,$0000
        call    l191d		; output "AT 0,0"
.l090e  call    l05a7		; page in normal memory
        call    l3e80
        defw    $12e8		; execute Loader via ROM 1
        ret     


.l0917  call    $1a95
        call    nc,$0794
        ld      hl,$0000
        ld      (E_PPC),hl
        ld      ($ec08),hl
        jr      l0930               ; (8)

; The Print menu option
        
.l0928  call    l05a7		; page in normal memory
        call    l3e80
        defw    $1451		; execute Print via ROM 1
.l0930  ld      hl,ed_flags
        bit     6,(hl)		; ???
.l0935  jr      nz,l093f

; The +3 BASIC routine - called from the main menu

.l0937  ld      hl,TV_FLAG
        res     0,(hl)          ; signal "main screen"
        call    l1a5a           ; display "+3 BASIC" bar
.l093f  ld      hl,ed_flags
        res     5,(hl)          ; ???
        res     4,(hl)          ; ???
        ld      a,$00           ; ???
        ld      hl,l082a        ; +3 BASIC menu addresses
        ld      de,l083a        ; +3 BASIC menu
        jr      l097c           ; go to set menu

; The Calculator routine - called from the main menu

.l0950  ld      hl,ed_flags
        set     5,(hl)          ; ???
        set     4,(hl)          ; ???
        res     6,(hl)          ; ???
        call    l0989
        call    l1a5f           ; display "Calculator" bar
        ld      a,$04
        ld      ($ec0e),a
        ld      hl,$0000
        ld      (E_PPC),hl
        call    $03fa
        ld      bc,$0000
        ld      a,b
        call    $0ac3
        ld      a,$04
        ld      hl,l0864
        ld      de,l086b

; Routine to set new menu and ???

.l097c  ld      ($ec0e),a       ; ???
        ld      (men_rout),hl   ; store routine address table
        ld      (men_text),de   ; store menu address
        jp      l06b4           ; ???

; Subroutine to ???

.l0989  call    l0eed
        call    l1c8c
        jp      l09b3

.l0992  ld      b,$00
        ld      d,$17
        call    l1d6b           ; clear screen to editor colours
        jp      $065c

.l099c  defb    $06
        defb    0
        defb    0
        defb    0
        defb    $04
        defb    $10
        defb    $14

.l09a3  defb    $06
        defb    0
        defb    0
        defb    0
        defb    0
        defb    $01
        defb    $01

.l09aa  ld      hl,l09a3
        ld      de,$f6ee
        jp      l2152

.l09b3  ld      hl,l099c
        ld      de,$f6ee
        jp      l2152

; Subroutine to ???

.l09bc  ld      hl,ed_flags
        or      a
        or      a
        bit     0,(hl)
        jp      nz,$0abd
        res     7,(hl)
        set     3,(hl)
        push    hl
        push    af
        call    $0ab7
        pop     af
        push    af
        call    $0f4f
        pop     af
        ld      a,b
        call    $0c43
        pop     hl
        set     7,(hl)
        jp      nc,$0abd
        ld      a,b
        jp      c,$0ac3
        jp      $0abd


; Editing keys: DELETE RIGHT

.l09e6  ld      hl,ed_flags
        set     3,(hl)
        call    $0ab7
        call    $0fe0
        scf     
        ld      a,b
        jp      $0ac3

; Editing keys: DELETE

.l09f6  ld      hl,ed_flags
        res     0,(hl)
        set     3,(hl)
        call    $0ab7
        call    $0c26
        ccf     
        jp      c,$0abd
.l0a07  call    $0fe0
        scf     
        ld      a,b
        jp      $0ac3

; Editing keys: ENTER

.l0a0f  call    $0ab7
        push    af
        call    $1182
        push    bc
        ld      b,$00
        call    $0f0f
        pop     bc
        jr      c,l0a29             ; (10)
        ld      hl,$0020
        add     hl,de
        ld      a,(hl)
        cpl     
        and     $09
        jr      z,l0a45             ; (28)
.l0a29  ld      a,(ed_flags)
        bit     3,a
        jr      z,l0a35             ; (5)
        call    $0d59
        jr      nc,l0a4a            ; (21)
.l0a35  call    $0d17
        call    $0c43
        call    $0f9c
        ld      b,$00
        pop     af
        scf     
        jp      $0ac3
.l0a45  pop     af
        scf     
        jp      $0abd
.l0a4a  pop     af
        jp      $0abd

; Editing keys: TOP

.l0a4e  ld      a,($ec0e)
        cp      $04
        ret     z               ; exit if in Calculator
        call    l0ab7           ; remove cursor
        ld      hl,$0000        ; line 0
        call    l05a7           ; page in normal memory
        rst     $28
        defw    $196e           ; get address of first line in HL
        rst     $28
        defw    $1695           ; get line number in DE
        call    l05cc           ; page in DOS workspace
        ld      (E_PPC),de      ; set as current line
        ld      a,$0f
        call    l1ca3           ; set colours to blue ink, white paper
        call    l03fa           ; ???
        scf                     ; success
        jp      l0abd           ; place cursor & exit

; Editing keys: BOTTOM

.l0a76  ld      a,($ec0e)
        cp      $04
        ret     z               ; exit if in Calculator
        call    l0ab7           ; remove cursor
        ld      hl,9999         ; last possible line
        call    l05a7           ; page in normal memory
        rst     $28
        defw    $196e           ; get last line address in DE
        ex      de,hl
        rst     $28
        defw    $1695           ; get last line number in DE
        call    l05cc           ; page in DOS workspace
        ld      (E_PPC),de      ; set as current line
        ld      a,$0f
        call    l1ca3           ; set colours to blue ink, white paper
        call    $03fa           ; ???
        scf                     ; success
        jp      l0abd           ; place cursor & exit

; Editing keys: LEFT WORD

.l0a9f  call    $0ab7
        call    $0cb5
        jp      nc,$0abd
        ld      a,b
        jp      $0ac3

; Editing keys: RIGHT WORD

.l0aac  call    $0ab7
        call    $0cd4
        jr      nc,l0abd            ; (9)
        ld      a,b
        jr      l0ac3               ; (12)

; Subroutine to remove cursor

.l0ab7  call    l0ad2		; get cursor position
        jp      l1749		; remove it

; Subroutine to place cursor

.l0abd  call    l0ad2		; get cursor position
        jp      l173a		; place it

; Subroutine to set cursor to line C, column B, ??? A
; and set colours & place it

.l0ac3  call    l0adc		; set cursor details
        push    af
        push    bc
        ld      a,$0f
        call    l1ca3		; set colours to blue INK, white PAPER
        pop     bc
        pop     af
        jp      l173a		; place cursor

; Subroutine to get cursor line (C), column (B), and ??? (A)

.l0ad2  ld      hl,$f6ee
        ld      c,(hl)		; get line (within editing area)
        inc     hl
        ld      b,(hl)		; get column
        inc     hl
        ld      a,(hl)		; get ???
        inc     hl
        ret     

; Subroutine to set cursor line (C), column (B), and ??? (A)

.l0adc  ld      hl,$f6ee
        ld      (hl),c		; set line
        inc     hl
        ld      (hl),b		; set column
        inc     hl
        ld      (hl),a		; set ???
        ret     



.l0ae5  push    hl
        call    $1182
        ld      h,$00
        ld      l,b
        add     hl,de
        ld      a,(hl)
        pop     hl
        ret     

; Editing keys: DOWN 10 LINES

.l0af0  call    $0ab7
        ld      e,a
        ld      d,$0a
.l0af6  push    de
        call    $0bfb
        pop     de
        jr      nc,l0abd            ; (-64)
        ld      a,e
        call    $0adc
        ld      b,e
        call    $0bc4
        jr      nc,l0b0d            ; (6)
        dec     d
        jr      nz,l0af6            ; (-20)
        ld      a,e
        jr      c,l0ac3             ; (-74)
.l0b0d  push    de
        call    $0bd6
        pop     de
        ld      b,e
        call    $0bc4
        ld      a,e
        or      a
        jr      l0ac3               ; (-87)

; Editing keys: UP 10 LINES

.l0b1a  call    $0ab7
        ld      e,a
        ld      d,$0a
.l0b20  push    de
        call    $0bd6
        pop     de
        jr      nc,l0abd            ; (-106)
        ld      a,e
        call    $0adc
        ld      b,e
        call    $0bcd
        jr      nc,l0b38            ; (7)
        dec     d
        jr      nz,l0b20            ; (-20)
        ld      a,e
        jp      c,$0ac3
.l0b38  push    af
        call    $0bfb
        ld      b,$00
        call    $0c9f
        pop     af
        jp      $0ac3

; Editing keys: END OF LINE
        
.l0b45  call    $0ab7
        call    $0d17
        jp      nc,$0abd
        ld      a,b
        jp      $0ac3

; Editing keys: START OF LINE

.l0b52  call    $0ab7
        call    $0cfc
        jp      nc,$0abd
        ld      a,b
        jp      $0ac3

; Editing keys: CURSOR UP

.l0b5f  call    l0ab7           ; remove cursor
        ld      e,a
        push    de
        call    $0bd6
        pop     de
        jp      nc,$0abd
        ld      b,e
        call    $0bcd
        ld      a,e
        jp      c,$0ac3
        push    af
        call    $0bfb
        ld      b,$00
        call    $0bc4
        pop     af
        jp      $0ac3

; Editing keys: CURSOR DOWN

.l0b80  call    l0ab7           ; remove cursor
        ld      e,a
        push    de
        call    $0bfb
        pop     de
        jp      nc,$0abd
        ld      b,e
        call    $0bcd
        ld      a,e
        jp      c,$0ac3
        push    de
        call    $0bd6
        pop     de
        ld      b,e
        call    $0bc4
        ld      a,e
        or      a
        jp      $0ac3

; Editing keys: CURSOR LEFT

.l0ba2  call    l0ab7           ; remove cursor
        call    $0c26
        jp      c,$0ac3
        jp      $0abd

; Editing keys: CURSOR RIGHT

.l0bae  call    l0ab7           ; remove cursor
        call    $0c43
        jp      c,$0ac3
        push    af
        call    $0bd6
        ld      b,$1f
        call    $0caa
        pop     af
        jp      $0ac3



.l0bc4  push    de
        call    $0c9f
        call    nc,$0caa
        pop     de
        ret     

.l0bcd  push    de
        call    $0caa
        call    nc,$0c9f
        pop     de
        ret     

.l0bd6  call    $0d47
        jr      nc,l0bfa            ; (31)
        push    bc
        call    $1182
        ld      b,$00
        call    $0f0f
        call    nc,$104e
        pop     bc
        ld      hl,$f6f1
        ld      a,(hl)
        cp      c
        jr      c,l0bf8             ; (9)
        push    bc
        call    $053a
        pop     bc
        ret     c
        ld      a,c
        or      a
        ret     z

.l0bf8  dec     c
        scf     

.l0bfa  ret     

.l0bfb  push    bc
        call    $1182
        ld      b,$00
        call    $0f0f
        pop     bc
        jr      c,l0c0a             ; (3)
        jp      $104e

.l0c0a  call    $0d33
        jr      nc,l0c25            ; (22)
        ld      hl,$f6f1
        inc     hl
        ld      a,c
        cp      (hl)
        jr      c,l0c23             ; (12)
        push    bc
        push    hl
        call    $0504
        pop     hl
        pop     bc
        ret     c
        inc     hl
        ld      a,(hl)
        cp      c
        ret     z

.l0c23  inc     c
        scf     

.l0c25  ret     

.l0c26  ld      d,a
        dec     b
        jp      m,$0c31
        ld      e,b
        call    $0caa
        ld      a,e
        ret     c

.l0c31  push    de
        call    $0bd6
        pop     de
        ld      a,e
        ret     nc
        ld      b,$1f
        call    $0caa
        ld      a,b
        ret     c
        ld      a,d
        ld      b,$00
        ret     

.l0c43  ld      d,a
        inc     b
        ld      a,$1f
        cp      b
        jr      c,l0c50             ; (6)
        ld      e,b
        call    $0c9f
        ld      a,e
        ret     c

.l0c50  dec     b
        push    bc
        push    hl
        ld      hl,ed_flags
        bit     7,(hl)
        jr      nz,l0c8b            ; (49)
        call    $1182
        ld      hl,$0020
        add     hl,de
        ld      a,(hl)
        bit     1,a
        jr      nz,l0c8b            ; (37)
        set     1,(hl)
        res     3,(hl)
        ld      hl,$0023
        add     hl,de
        ex      de,hl
        pop     hl
        pop     bc
        push    af
        call    $0bfb
        pop     af
        call    $1182
        ld      hl,$0023
        add     hl,de
        ex      de,hl
        res     0,a
        set     3,a
        call    $0fa1
        call    $16ee
        ld      a,b
        scf     
        ret     

.l0c8b  pop     hl
        pop     bc
        push    de
        call    $0bfb
        pop     de
        ld      a,b
        ret     nc
        ld      b,$00
        call    $0c9f
        ld      a,b
        ret     c
        ld      a,e
        ld      b,$00
        ret     

.l0c9f  push    de
        push    hl
        call    $1182
        call    $0f0f
        jp      $0d30

.l0caa  push    de
        push    hl
        call    $1182
        call    $0f31
        jp      $0d30

.l0cb5  push    de
        push    hl
.l0cb7  call    $0c26
        jr      nc,l0cd2            ; (22)
        call    $0ae5
        cp      $20
        jr      z,l0cb7             ; (-12)
.l0cc3  call    $0c26
        jr      nc,l0cd2            ; (10)
        call    $0ae5
        cp      $20
        jr      nz,l0cc3            ; (-12)
        call    $0c43

.l0cd2  jr      l0d30               ; (92)

.l0cd4  push    de
        push    hl

.l0cd6  call    $0c43
        jr      nc,l0cf6            ; (27)
        call    $0ae5
        cp      $20
        jr      nz,l0cd6            ; (-12)

.l0ce2  call    $0c43
        jr      nc,l0cf6            ; (15)
        call    $0f0f
        jr      nc,l0cf6            ; (10)
        call    $0ae5
        cp      $20
        jr      z,l0ce2             ; (-17)
        scf     
        jr      l0d30               ; (58)

.l0cf6  call    nc,$0c26
        or      a
        jr      l0d30               ; (52)

.l0cfc  push    de
        push    hl

.l0cfe  call    $1182
        ld      hl,$0020
        add     hl,de
        bit     0,(hl)
        jr      nz,l0d10            ; (7)
        call    $0bd6
        jr      c,l0cfe             ; (-16)
        jr      l0d30               ; (32)

.l0d10  ld      b,$00
        call    $0c9f
        jr      l0d30               ; (25)

.l0d17  push    de
        push    hl

.l0d19  call    $1182
        ld      hl,$0020
        add     hl,de
        bit     3,(hl)
        jr      nz,l0d2b            ; (7)
        call    $0bfb
        jr      c,l0d19             ; (-16)
        jr      l0d30               ; (5)

.l0d2b  ld      b,$1f
        call    $0caa

.l0d30  pop     hl
        pop     de
        ret     

.l0d33  ld      a,(ed_flags)
        bit     3,a
        scf     
        ret     z
        call    $1182
        ld      hl,$0020
        add     hl,de
        bit     3,(hl)
        scf     
        ret     z
        jr      l0d59               ; (18)

.l0d47  ld      a,(ed_flags)
        bit     3,a
        scf     
        ret     z
        call    $1182
        ld      hl,$0020
        add     hl,de
        bit     0,(hl)
        scf     
        ret     z

.l0d59  ld      a,$02

.l0d5b  call    $1182
        ld      hl,$0020
        add     hl,de
        bit     0,(hl)
        jr      nz,l0d6e            ; (8)
        dec     c
        jp      p,$0d5b
        ld      c,$00
        ld      a,$01

.l0d6e  ld      hl,$ec00
        ld      de,$ec03
        or      $80
        ld      (hl),a
        ld      (de),a
        inc     hl
        inc     de
        ld      a,$00
        ld      (hl),a
        ld      (de),a
        inc     hl
        inc     de
        ld      a,c
        ld      (hl),a
        ld      (de),a
        ld      hl,$0000
        ld      ($ec06),hl
        call    $142d
        call    $1dfa
        push    ix
        call    $05a7
        call    l3e80
        defw    $24f0
        call    $05cc
        ei      
        pop     ix
        ld      a,(ERR_NR)
        inc     a
        jr      nz,l0dbd            ; (24)
        ld      hl,ed_flags
        res     3,(hl)
        call    $1758
        ld      a,($ec0e)
        cp      $04
        call    nz,$03fa
        call    $07a7
        call    $0ad2
        scf     
        ret     

.l0dbd  ld      hl,$ec00
        ld      de,$ec03
        ld      a,(de)
        res     7,a
        ld      (hl),a
        inc     hl
        inc     de
        ld      a,(de)
        ld      (hl),a
        inc     hl
        inc     de
        ld      a,(de)
        ld      (hl),a
        call    $1df6
        jr      c,l0dd8             ; (4)
        ld      bc,($ec06)

.l0dd8  ld      hl,($ec06)
        or      a
        sbc     hl,bc
        push    af
        push    hl
        call    $0ad2
        pop     hl
        pop     af
        jr      c,l0df8             ; (17)
        jr      z,l0e13             ; (42)

.l0de9  push    hl
        ld      a,b
        call    $0c26
        pop     hl
        jr      nc,l0e13            ; (34)
        dec     hl
        ld      a,h
        or      l
        jr      nz,l0de9            ; (-13)
        jr      l0e13               ; (27)

.l0df8  push    hl
        ld      hl,ed_flags
        res     7,(hl)
        pop     hl
        ex      de,hl
        ld      hl,$0000
        or      a
        sbc     hl,de

.l0e06  push    hl
        ld      a,b
        call    $0c43
        pop     hl
        jr      nc,l0e13            ; (5)
        dec     hl
        ld      a,h
        or      l
        jr      nz,l0e06            ; (-13)

.l0e13  ld      hl,ed_flags
        set     7,(hl)
        call    $0adc
        ld      a,$17
        call    $1ca3
        or      a
        ret     

.l0e22  ld      hl,$ec00
        bit     7,(hl)
        jr      z,l0e30             ; (7)
        ld      hl,($ec06)
        inc     hl
        ld      ($ec06),hl

.l0e30  ld      hl,$ec00
        ld      a,(hl)
        inc     hl
        ld      b,(hl)
        inc     hl
        ld      c,(hl)
        push    hl
        and     $0f
        ld      hl,$0e53
        call    l2166
        ld      e,l
        pop     hl
        jr      z,l0e47             ; (2)
        ld      a,$0d

.l0e47  ld      (hl),c
        dec     hl
        ld      (hl),b
        dec     hl
        push    af
        ld      a,(hl)
        and     $f0
        or      e
        ld      (hl),a
        pop     af
        ret     
        inc     bc
        ld      (bc),a
        ld      a,d
        ld      c,$04
        or      a
        ld      c,$01
        ld      e,l
        ld      c,$cd
        add     a,l
        inc     de

.l0e60  call    $0edc
        jr      nc,l0e6c            ; (7)
        cp      $00
        jr      z,l0e60             ; (-9)
        ld      l,$01
        ret     

.l0e6c  inc     c
        ld      b,$00
        ld      hl,($f9db)
        ld      a,c
        cp      (hl)
        defb    $38
        defb    -25
        ld      b,$00
        ld      c,$00

.l0e7a  push    hl
        ld      hl,$f6ee
        ld      a,(hl)
        cp      c
        jr      nz,l0e8c            ; (10)
        inc     hl
        ld      a,(hl)
        cp      b
        jr      nz,l0e8c            ; (5)
        ld      hl,$ec00
        res     7,(hl)

.l0e8c  pop     hl

.l0e8d  call    $1182
        call    $0edc
        jr      nc,l0e9c            ; (7)
        cp      $00
        jr      z,l0e7a             ; (-31)
        ld      l,$02
        ret     

.l0e9c  ld      hl,$0020
        add     hl,de
        bit     3,(hl)
        jr      z,l0ea9             ; (5)
        ld      l,$08
        ld      a,$0d
        ret     

.l0ea9  ld      hl,$f6f3
        inc     c
        ld      a,(hl)
        cp      c
        ld      b,$00
        jr      nc,l0e8d            ; (-38)
        ld      b,$00
        ld      c,$01

.l0eb7  call    $1291

.l0eba  call    $0edc
        jr      nc,l0ec6            ; (7)
        cp      $00
        jr      z,l0eba             ; (-9)
        ld      l,$04
        ret     

.l0ec6  ld      hl,$0020
        add     hl,de
        bit     3,(hl)
        jr      nz,l0ed7            ; (9)
        inc     c
        ld      b,$00
        ld      a,($f6f5)
        cp      c
        jr      nc,l0eb7            ; (-32)

.l0ed7  ld      l,$08
        ld      a,$0d
        ret     

.l0edc  ld      a,$1f
        cp      b
        ccf     
        ret     nc
        ld      l,b
        ld      h,$00
        add     hl,de
        ld      a,(hl)
        inc     b
        scf     
        ret


.l0ee9  defb    $01
        defb    $14

.l0eeb  defb    $01
        defb    $01

; Subroutine to ???

.l0eed  ld      hl,TV_FLAG
        res     0,(hl)          ; signal "not using lower screen"
        ld      hl,l0ee9
        ld      de,$ec15
        jp      l2152           ; copy $14 into $ec15 and exit

.l0efb  ld      hl,TV_FLAG
        set     0,(hl)          ; signal "using lower screen"
        ld      bc,$0000
        call    l191d           ; output "AT 0,0"
        ld      hl,l0eeb
        ld      de,$ec15
        jp      l2152           ; copy $01 into $ec15 and exit

.l0f0f  ld      h,$00
        ld      l,b
        add     hl,de
        ld      a,(hl)
        cp      $00
        scf     
        ret     nz
        ld      a,b
        or      a
        jr      z,l0f29             ; (13)
        push    hl
        dec     hl
        ld      a,(hl)
        cp      $00
        scf     
        pop     hl
        ret     nz

.l0f24  ld      a,(hl)
        cp      $00
        scf     
        ret     nz

.l0f29  inc     hl
        inc     b
        ld      a,b
        cp      $1f
        jr      c,l0f24             ; (-12)
        ret     

.l0f31  ld      h,$00
        ld      l,b
        add     hl,de
        ld      a,(hl)
        cp      $00
        scf     
        ret     nz

.l0f3a  ld      a,(hl)
        cp      $00
        jr      nz,l0f46            ; (7)
        ld      a,b
        or      a
        ret     z
        dec     hl
        dec     b
        jr      l0f3a               ; (-12)

.l0f46  inc     b
        scf     
        ret     
        ld      h,$00
        ld      l,b
        add     hl,de
        ld      a,(hl)
        ret     

.l0f4f  ld      hl,ed_flags
        or      a
        bit     0,(hl)
        ret     nz
        push    bc
        push    af
        call    $1182
        pop     af

.l0f5c  call    $0577
        push    af
        ex      de,hl
        call    $16fe
        ex      de,hl
        pop     af
        ccf     
        jr      z,l0f9a             ; (49)
        push    af
        ld      b,$00
        inc     c
        ld      a,($ec15)
        cp      c
        jr      c,l0f96             ; (35)
        ld      a,(hl)
        ld      e,a
        and     $d7
        cp      (hl)
        ld      (hl),a
        ld      a,e
        set     1,(hl)
        push    af
        call    $1182
        pop     af
        jr      z,l0f90             ; (13)
        res     0,a
        call    $0fa1
        jr      nc,l0f9a            ; (16)
        call    $16ee
        pop     af
        jr      l0f5c               ; (-52)

.l0f90  call    $0f0f
        pop     af
        jr      l0f5c               ; (-58)

.l0f96  pop     af
        call    $123c

.l0f9a  pop     bc
        ret     

.l0f9c  call    $1182
        ld      a,$09

.l0fa1  push    bc
        push    de
        ld      b,c
        ld      hl,$0fbd
        ld      c,a
        push    bc
        call    $0540
        pop     bc
        ld      a,c
        jr      nc,l0fba            ; (10)
        ld      c,b
        call    $1182
        ld      hl,$0020
        add     hl,de
        ld      (hl),a
        scf     

.l0fba  pop     de
        pop     bc
        ret     
        
        defs    $20
        
        add     hl,bc

        defs    2

.l0fe0  push    bc
        call    $1182
        push    bc

.l0fe5  ld      hl,$0020
        add     hl,de
        bit     1,(hl)
        ld      a,$00
        jr      z,l0fff             ; (16)
        inc     c
        ld      hl,$0023
        add     hl,de
        ex      de,hl
        ld      a,($ec15)
        cp      c
        jr      nc,l0fe5            ; (-22)
        dec     c
        call    $1297

.l0fff  pop     hl

.l1000  push    hl
        call    $1182
        pop     hl
        ld      b,a
        ld      a,c
        cp      l
        ld      a,b
        push    af
        jr      nz,l100f            ; (3)
        ld      b,h
        jr      l1018               ; (9)

.l100f  push    af
        push    hl
        ld      b,$00
        call    $0f0f
        pop     hl
        pop     af

.l1018  push    hl
        ld      hl,$f6f4
        set     0,(hl)
        jr      z,l1022             ; (2)
        res     0,(hl)

.l1022  call    $058c
        push    af
        push    bc
        push    de
        ld      hl,$f6f4
        bit     0,(hl)
        jr      nz,l103d            ; (14)
        ld      b,$00
        call    $0c9f
        jr      c,l103d             ; (7)
        call    $104e
        pop     de
        pop     bc
        jr      l1042               ; (5)

.l103d  pop     hl
        pop     bc
        call    $16fe

.l1042  pop     af
        dec     c

.l1044  ld      b,a
        pop     hl
        pop     af
        ld      a,b
        jp      nz,$1000
        scf     
        pop     bc
        ret     

.l104e  ld      hl,$0020
        add     hl,de
        ld      a,(hl)
        bit     0,(hl)
        jr      nz,l1080            ; (41)
        push    af
        push    bc
        ld      a,c
        or      a
        jr      nz,l1072            ; (21)
        push    bc
        ld      hl,($fc9a)
        call    $1418
        ld      ($fc9a),hl
        ld      a,($f9db)
        ld      c,a
        dec     c
        call    $1385
        pop     bc
        jr      l1076               ; (4)

.l1072  dec     c
        call    $1182

.l1076  pop     bc
        pop     af
        ld      hl,$0020
        add     hl,de
        res     1,(hl)
        or      (hl)
        ld      (hl),a

.l1080  ld      b,c
        call    $1182
        call    $11ad
        jp      $0513


; Editing keys: DELETE WORD LEFT

.l108a  call    $1152
.l108d  push    hl
        call    $1163
        jr      z,l10c5             ; (50)
        call    $0c26
        pop     hl
        jr      nc,l10c6            ; (45)
        call    $0ae5
        push    af
        push    hl
        call    $0fe0
        pop     hl
        pop     af
        cp      $20
        jr      z,l108d             ; (-26)

.l10a7  push    hl
        call    $1163
        jr      z,l10c5             ; (24)
        call    $0c26
        pop     hl
        jr      nc,l10c6            ; (19)
        call    $0ae5
        cp      $20
        jr      z,l10c1             ; (7)
        push    hl
        call    $0fe0
        pop     hl
        jr      l10a7               ; (-26)

.l10c1  push    hl
        call    $0c43

.l10c5  pop     hl

.l10c6  ld      a,b
        push    af
        push    hl
        ld      hl,$eef5
        res     2,(hl)
        ld      a,($ec15)
        push    bc
        ld      b,$00
        ld      c,a
        cp      a
        call    $04d0
        pop     bc
        ld      hl,ed_flags
        set     3,(hl)
        pop     hl
        call    $0ac3
        pop     af
        ret     


; Editing keys: DELETE WORD RIGHT

.l10e5  call    $1152
.l10e8  push    hl
        call    $0ae5
        pop     hl
        cp      $00
        scf     
        jr      z,l10c6             ; (-44)
        push    af
        push    hl
        call    $0fe0
        pop     hl
        pop     af
        cp      $20
        jr      nz,l10e8            ; (-21)

.l10fd  call    $0ae5
        cp      $20
        scf     
        jr      nz,l10c6            ; (-63)
        push    hl
        call    $0fe0
        pop     hl
        jr      l10fd               ; (-15)


; Editing keys: DELETE LINE LEFT

.l110c  call    $1152
.l110f  push    hl
        call    $1182
        ld      hl,$0020
        add     hl,de
        bit     0,(hl)
        jr      nz,l1127            ; (12)
        call    $0c26
        jr      nc,l113b            ; (27)
        call    $0fe0
        pop     hl
        jr      l110f               ; (-23)
        push    hl

.l1127  ld      a,b
        cp      $00
        jr      z,l113b             ; (15)
        dec     b
        call    $0ae5
        inc     b
        cp      $00
        jr      z,l113b             ; (6)
        dec     b
        call    $0fe0
        jr      l1127               ; (-20)

.l113b  pop     hl

.l113c  scf     
        jp      $10c6


; Editing keys: DELETE LINE RIGHT

.l1140  call    $1152
.l1143  call    $0ae5
        cp      $00
        scf     
        jr      z,l113c             ; (-15)
        push    hl
        call    $0fe0
        pop     hl
        jr      l1143               ; (-15)

.l1152  ld      hl,ed_flags
        res     0,(hl)
        call    $0ab7
        ld      hl,$eef5
        set     2,(hl)
        ld      hl,$f6f1
        ret     

.l1163  call    $1182
        ld      hl,$0020
        add     hl,de
        bit     0,(hl)
        jr      z,l117c             ; (14)
        ld      a,b
        cp      $00
        jr      z,l1180             ; (13)
        dec     b
        call    $0ae5
        inc     b
        cp      $00
        jr      z,l1180             ; (4)

.l117c  ld      a,$01
        or      a
        ret     

.l1180  xor     a
        ret     

.l1182  ld      hl,$ec16

.l1185  push    af
        ld      a,c
        ld      de,$0023

.l118a  or      a
        jr      z,l1191             ; (4)
        add     hl,de
        dec     a
        jr      l118a               ; (-7)

.l1191  ex      de,hl
        pop     af
        ret     
        push    de
        call    $1182
        ld      h,$00
        ld      l,b
        add     hl,de
        pop     de
        ret     


.l119e	defb	$05
	defb	$00,$00,$00,$f8,$f6

; Subroutine to ???

.l11a4	ld	hl,l119e
	ld	de,$f6f5
	jp	l2152

; Subroutine to ???

.l11ad	push	bc
	push	de
        ld      hl,$f6f5	; ???
        push    hl
        ld      a,(hl)
        or      a
        jr      nz,l11cf	; move on if ???
        push    hl
        call    l142d		; setup token routines in RAM
        ld      hl,($f9d7)
        call    l1420		; get line number after ???
        jr      nc,l11c6	; move on if none
        ld      ($f9d7),hl	; store it
.l11c6  ld      b,h
        ld      c,l
        pop     hl
        call    l13a4		; ???
        dec     a
        jr      l11e4
.l11cf  ld      hl,ed_flags
        res     0,(hl)
        ld      hl,$f6f8
        ld      d,h
        ld      e,l
        ld      bc,$0023
        add     hl,bc
        ld      bc,$02bc
        ldir    
        dec     a
        scf     
.l11e4  pop     de
        ld      (de),a
        ld      hl,$f6f8
        pop     de
        pop     bc
        ret     


.l11ec  push    bc
        push    de
        ld      hl,$0020
        add     hl,de
        ld      a,(hl)
        cpl     
        and     $11
        jr      nz,l120d            ; (21)
        push    hl
        push    de
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      e,(hl)
        push    de
        call    $142d
        pop     hl
        call    $1418
        jr      nc,l120b            ; (3)
        ld      ($f9d7),hl

.l120b  pop     de
        pop     hl

.l120d  bit     0,(hl)
        ld      hl,$f6f5
        push    hl
        jr      z,l121a             ; (5)
        ld      a,$00
        scf     
        jr      l11e4               ; (-54)

.l121a  ld      a,(hl)
        cp      $14
        jr      z,l11e4             ; (-59)
        ld      bc,$0023
        ld      hl,$f6f8
        ex      de,hl
        ldir    
        ld      hl,$f9d6
        ld      d,h
        ld      e,l
        ld      bc,$0023
        or      a
        sbc     hl,bc
        ld      bc,$02bc
        lddr    
        inc     a
        scf     
        jr      l11e4               ; (-88)

.l123c  push    bc
        push    de
        push    af
        ld      b,$00
        ld      c,$01
        push    hl
        call    $1291
        pop     hl
        bit     3,(hl)
        res     3,(hl)
        jr      nz,l126e            ; (32)

.l124e  call    $0f0f
        pop     af

.l1252  call    $0577
        jr      z,l1288             ; (49)
        push    af
        ld      b,$00
        inc     c
        ld      a,c
        cp      $15
        jr      c,l126e             ; (14)
        dec     hl
        ld      a,(hl)
        inc     hl
        cp      $00
        jr      z,l126e             ; (7)
        push    hl
        ld      hl,ed_flags
        set     0,(hl)
        pop     hl

.l126e  bit     1,(hl)
        set     1,(hl)
        res     3,(hl)
        call    $1291
        jr      nz,l124e            ; (-43)
        push    bc
        push    de
        call    l16e0		; pad line at DE with nulls to length 32
        ld      (hl),$08
        pop     de
        pop     bc
        call    $16ee
        pop     af
        jr      l1252               ; (-54)

.l1288  ld      a,c
        ld      ($f6f5),a
        set     3,(hl)
        pop     de
        pop     bc
        ret     

.l1291  ld      hl,$f6f8
        jp      $1185

.l1297  push    bc
        push    de
        ld      hl,ed_flags
        res     0,(hl)
        ld      a,($f6f5)
        ld      c,a
        or      a
        ld      a,$00
        jr      z,l12e9             ; (66)

.l12a7  call    $1291
        push    af
        ld      b,$00
        call    $0f0f
        jr      nc,l12c0            ; (14)
        pop     af
        call    $058c
        push    af
        push    bc
        ld      b,$00
        call    $0f0f
        pop     bc
        jr      c,l12e4             ; (36)

.l12c0  inc     hl
        ld      a,(hl)
        push    af
        push    bc
        ld      a,c
        cp      $01
        jr      nz,l12d2            ; (9)
        ld      a,($ec15)
        ld      c,a
        call    $1182
        jr      l12d6               ; (4)

.l12d2  dec     c
        call    $1291

.l12d6  pop     bc
        pop     af
        ld      hl,$0020
        add     hl,de
        res     1,(hl)
        or      (hl)
        ld      (hl),a
        ld      hl,$f6f5
        dec     (hl)

.l12e4  pop     af
        dec     c
        jr      nz,l12a7            ; (-65)
        scf     

.l12e9  pop     de
        pop     bc
        ret     

; ???

.l12ec	defb	$03
	defb	$00,$de,$f9

; Subroutine to ???

.l12f0  ld      hl,l12ec
        ld      de,$f9db
        jp      l2152

.l12f9  push    bc
        push    de
        ld      hl,$f9db
        push    hl
        ld      a,(hl)
        or      a
        jr      nz,l1321            ; (30)
        push    hl
        call    $142d
        ld      hl,($fc9a)
        call    $1418
        jr      nc,l1312            ; (3)
        ld      ($fc9a),hl

.l1312  ld      b,h
        ld      c,l
        pop     hl
        inc     hl
        inc     hl
        inc     hl
        jr      nc,l132b            ; (17)
        call    $13a4
        dec     a
        ex      de,hl
        jr      l132b               ; (10)

.l1321  ld      hl,($f9dc)
        ld      bc,$0023
        sbc     hl,bc
        scf     
        dec     a

.l132b  ex      de,hl
        pop     hl
        jr      nc,l1330            ; (1)
        ld      (hl),a

.l1330  inc     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d
        ex      de,hl
        pop     de
        pop     bc
        ret     

.l1338  push    bc
        push    de
        ld      hl,$0020
        add     hl,de
        ld      a,(hl)
        cpl     
        and     $11
        jr      nz,l1350            ; (12)
        push    de
        push    hl
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      e,(hl)
        ld      ($fc9a),de
        pop     hl
        pop     de

.l1350  bit     3,(hl)
        ld      hl,$f9db
        push    hl
        jr      z,l136e             ; (22)
        push    hl
        call    $142d
        ld      hl,($fc9a)
        call    $1420
        ld      ($fc9a),hl
        pop     hl
        inc     hl
        inc     hl
        inc     hl
        ld      a,$00
        scf     
        jr      l132b               ; (-67)

.l136e  ld      a,(hl)
        cp      $14
        jr      z,l1381             ; (14)
        inc     a
        ld      hl,($f9dc)
        ld      bc,$0023
        ex      de,hl
        ldir    
        ex      de,hl
        scf     
        jr      l132b               ; (-86)

.l1381  pop     hl
        pop     de
        pop     bc
        ret     

.l1385  ld      hl,$f9de
        jp      $1185

; Table of routine addresses for printing chars in EDITOR

.l138b  defb    $08
        defb    $0d
        defw    $16c6           ; ???
        defb    $01
        defw    $16d4           ; ???
        defb    $12
        defw    l1428           ; for colour codes, skip embedded code
        defb    $13
        defw    l1428
        defb    $14
        defw    l1428
        defb    $15
        defw    l1428
        defb    $10
        defw    l1428
        defb    $11
        defw    l1428

; Subroutine to ???

.l13a4  ld      d,h
        ld      e,l
        inc     de
        inc     de
        inc     de
.l13a9  push    de
        ld      hl,$0020
        add     hl,de
        ld      (hl),$01        ; store ???
        inc     hl
        ld      (hl),b
.l13b2  inc     hl
        ld      (hl),c          ; store line number
        ld      c,$01           ; coloumn 1 ???
        ld      b,$00           ; character position on screen line
.l13b8  push    bc
        push    de
        ld      a,($ec0e)
        cp      $04
        call    nz,l1611        ; unless in calculator, get next line char
        pop     de
        pop     bc
        jr      c,l13d5         ; move on if found character
        ld      a,c
        cp      $01
        ld      a,$0d
        jr      nz,l13d5        ; do CR unless no chars output
        ld      a,b
        or      a
        ld      a,$01
        jr      z,l13d5         ; ???
        ld      a,$0d           ; CR
.l13d5  ld      hl,l138b
        call    l2166           ; perform actions for special chars
        jr      c,l13fa         ; if successful action, move on
        jr      z,l13b8         ; loop back if need new char
        push    af
        ld      a,$1f
        cp      b
        jr      nc,l13f4        ; move on unless need to start new line
        ld      a,$12
        call    l13ff           ; ???
        jr      c,l13f1         ; ???
        pop     af
        ld      a,$0d
        jr      l13d5
.l13f1  call    $16ee           ; ???
.l13f4  pop     af
        call    l16bf           ; ???
        jr      l13b8
.l13fa  pop     hl
        ld      a,c             ; ???
        ret     z
        scf     
        ret     

; Subroutine to ???

.l13ff  push    af
        call    l16e0		; pad line at DE to length 32 with nulls
        pop     af
        xor     (hl)		; ??
        ld      (hl),a
        ld      a,c
        cp      $14
        ret     nc
        inc     c
        ld      hl,$0023
        add     hl,de
        ex      de,hl
        ld      hl,$0020
        add     hl,de
        ld      (hl),$00
        scf     
        ret     

; Subroutine to get number (HL) of line before HL (or 0 if none)

.l1418  call    l15b0           ; find line number before HL
        ret     c		; exit if found
        ld      hl,$0000	; else use 0
        ret     

; Subroutine to get number (HL) of line after HL (or 0 if none)

.l1420  call    l152a		; get line number after HL
        ret     c		; exit if found
        ld      hl,$0000	; else use 0
        ret     

; Subroutine to skip an embedded colour code in a line

.l1428  call    l1611           ; get next char (skip colour code)
        ccf     
        ret     nc              ; exit if success, else set no data left

; Subroutine to setup token scanning/expanding routines

.l142d  ld      hl,$0000
        ld      ($fc9f),hl	; set "no line data"
        ld      ($fca1),hl	; set "no expanded token"
        ld      hl,l1442
        ld      de,$fcae
        ld      bc,$00e8
        ldir    		; copy token routines into RAM
        ret     

; Routine executed in RAM at $fcae
; On entry, A=zero-based token number (code-$a5), and on exit $fca1
; contains address of expanded token

.l1442  di      
        push    af
        ld      bc,$7ffd
        ld      a,$17
        out     (c),a		; page in ROM 1 & RAM 7
        ld      bc,$1ffd
        ld      a,(BANK678)
        set     2,a
        out     (c),a		; page in ROM 3
        pop     af
        cp      $50
        jr      nc,l148b	; these parts start off search at every
        cp      $40		; 16th token for speed
        jr      nc,l1484
        cp      $30
        jr      nc,l147d
        cp      $20
        jr      nc,l1476
        cp      $10
        jr      nc,l146f
        ld      hl,$0096	; RND in token table
        jr      l1490
.l146f  sub     $10
        ld      hl,$00cf	; ASN in token table
        jr      l1490
.l1476  sub     $20
        ld      hl,$0100	; OR in token table
        jr      l1490
.l147d  sub     $30
        ld      hl,$013e	; MERGE in token table
        jr      l1490
.l1484  sub     $40
        ld      hl,$018b	; RESTORE in token table
        jr      l1490
.l148b  sub     $50
        ld      hl,$01d4	; PRINT in token table
.l1490  ld      b,a		; B=offset of token from current
        or      a
.l1492  jr      z,l149d
.l1494  ld      a,(hl)		; get next character
        inc     hl
        and     $80
        jr      z,l1494		; loop back until end of token found
        dec     b		; decrement token offset
        jr      l1492		; loop back
.l149d  ld      de,$fca3
        ld      ($fca1),de	; set expanded token address
        ld      a,($fc9e)
        or      a		; test "leading space" flag
        ld      a,$00
        ld      ($fc9e),a	; and set to zero
        jr      nz,l14b3
        ld      a,' '
        ld      (de),a		; insert space if necessary
        inc     de
.l14b3  ld      a,(hl)
        ld      b,a
        inc     hl
        ld      (de),a		; copy token byte
        inc     de
        and     $80
        jr      z,l14b3		; back until end of token
        ld      a,b
        and     $7f
        dec     de
        ld      (de),a		; mask off high bit in last char
        inc     de
        ld      a,' '+$80	; and add terminating space
        ld      (de),a
        ld      bc,$7ffd
        ld      a,(BANKM)
        out     (c),a		; restore original ROM/RAM configuration
        ld      bc,$1ffd
        ld      a,(BANK678)
        out     (c),a
        ei      
        ret     

; Routine executed in RAM at $fd43
; On entry, bit-7 terminated word to check for is at $fda0
; On exit, if carry set, A=token code, else A=0

.l14d7	di     
        push    af
        ld      bc,$7ffd
        ld      a,$17
        out     (c),a		; page in ROM 1 & RAM 7
        ld      bc,$1ffd
        ld      a,(BANK678)
        set     2,a
        out     (c),a		; page in ROM 3
        pop     af
        ld      hl,$0096	; token table start
        ld      b,$a5		; first token number
.l14f0  ld      de,$fda0	; start of word to test
.l14f3  ld      a,(de)		; get next letter of word to test
        and     $7f
        cp      $61
        ld      a,(de)
        jr      c,l14fd
        and     $df		; mask lowercase letters to uppercase
.l14fd  cp      (hl)		; test against current token letter
        jr      nz,l1509	; move on if no match
        inc     hl
        inc     de
        and     $80
        jr      z,l14f3		; loop back unless last token character
        scf     		; success
        jr      l1515
.l1509  inc     b		; increment token number
        jr      z,l1514		; exit if all checked
.l150c  ld      a,(hl)
        and     $80
        inc     hl
        jr      z,l150c		; loop back until current token finished
        jr      l14f0		; back to check more tokens
.l1514  or      a		; failure
.l1515  ld      a,b
        ld      d,a
        ld      bc,$7ffd
        ld      a,(BANKM)
        out     (c),a		; page back original ROM/RAM configuration
        ld      bc,$1ffd
        ld      a,(BANK678)
        out     (c),a
        ld      a,d		; A=token number
        ei      
        ret     

; Subroutine to form ASCII line number for next line after HL. Exits
; with DE=address of line data & HL=line number

.l152a  call    l15e4		; set "no line" addresses
        or      a
        ld      ($fc9e),a	; set "leading spaces"
        call    l05a7		; page in normal memory
        call    l15f0		; get address of line HL
        jr      nc,l158b        ; exit if not found
        jr      nz,l1547
        ld      a,b
        or      c
        jr      z,l1547		; move on if first line was required
        call    l15c9		; get address of next line
        call    l15d3
        jr      nc,l158b	; exit if end of program

; Subroutine to form ASCII line number for line at HL.
; Exits with HL=line number, DE=address of line data

.l1547  ld      d,(hl)
        inc     hl
        ld      e,(hl)          ; get line number
        call    l05cc           ; page in DOS workspace
        push    de
        push    hl
        push    ix
        ld      ix,$fca3
        ld      ($fca1),ix      ; set ASCII line number address
        ex      de,hl
        ld      b,$00           ; don't form leading zeros
        ld      de,$fc18
        call    l158f           ; form 1000s
        ld      de,$ff9c
        call    l158f           ; form 100s
        ld      de,$fff6
        call    l158f           ; form 10s
        ld      de,$ffff
        call    l158f           ; form units
        dec     ix
        ld      a,(ix+$00)
        or      $80             ; set bit 7 of last digit
        ld      (ix+$00),a
        pop     ix
        pop     hl
        pop     de
        inc     hl
        inc     hl
        inc     hl              ; HL=address of line data
        ld      ($fc9f),hl      ; save it
        ex      de,hl
        scf                     ; success
        ret
.l158b  call    l05cc           ; page in DOS workspace
        ret                     ; exit

; Subroutine to form next digit of line number at IX
; Line number in HL, unit size in DE, and print zero flag in B

.l158f  xor     a               ; count 0
.l1590  add     hl,de           ; reduce line number by unit size
        inc     a               ; increment count
        jr      c,l1590         ; loop until overflow
        sbc     hl,de           ; add back last try
        dec     a
        add     a,'0'           ; form ASCII digit
        ld      (ix+$00),a      ; store it
        cp      '0'
        jr      nz,l15ab        ; set flag if not zero
        ld      a,b
        or      a
        jr      nz,l15ad        ; if flag set, leave 0 digits
        ld      a,$00
        ld      (ix+$00),a      ; else replace with $00
        jr      l15ad
.l15ab  ld      b,$01           ; set "print 0 digits" flag
.l15ad  inc     ix              ; increment pointer
        ret     

; Subroutine to get number (HL) and address (DE) of line before line HL
; forming ASCII line number in page 7. Carry reset if no prior line.

.l15b0  call    l15e4           ; initialise "no line" addresses
        or      a
        ld      ($fc9e),a       ; ???
        call    l05a7           ; page in normal memory
        call    l15f0           ; find address of line HL
        jr      nc,l158b        ; if not found, exit with error
        ex      de,hl
        ld      a,l
        or      h               ; was there a previous line?
        scf     
        jp      nz,l1547        ; form line number & address of data if so
        ccf     
        jr      l158b           ; else exit with error

; Subroutine to get address of next program line in HL (current is
; saved in DE)

.l15c9  push    hl
        inc     hl
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=line length
        inc     hl
        add     hl,de           ; HL=next line
        pop     de              ; DE=previous line
        ret

; Subroutine to check for end of program (carry reset if so)

.l15d3  ld      a,(hl)          ; check next program byte
        and     $c0
        scf     
        ret     z               ; exit with carry set if not end-of-program
        ccf                     ; clear carry if end-of-program
        ret

; Subroutine to check if line number at HL is equal to BC (carry set if so)

.l15da  ld      a,b
        cp      (hl)
        ret     nz
        ld      a,c
        inc     hl
        cp      (hl)
        dec     hl
        ret     nz
        scf     
        ret     

; Subroutine to set "no line" addresses

.l15e4  push    hl
        ld      hl,$0000
        ld      ($fca1),hl      ; set "no ASCII line number"
        ld      ($fc9f),hl      ; set "no line data"
        pop     hl
        ret

; Subroutine to search for address of line HL, returning in HL with
; carry set. If HL=0 first line address is returned. Carry reset if
; line not found. DE=address of previous line (or 0)

.l15f0  push    hl
        pop     bc		; BC=line to find
        ld      de,$0000        ; no previous line
        ld      hl,(PROG)       ; get start of program
        call    l15d3           ; check if end
        ret     nc              ; exit if so with failure
        call    l15da           ; is it line BC
        ret     c               ; exit if so with success
        ld      a,b
        or      c
        scf     
        ret     z               ; exit with first line if line 0 specified
.l1604  call    l15c9           ; get to next line
        call    l15d3
        ret     nc              ; exit if program end
        call    l15da
        jr      nc,l1604        ; loop back if not line BC yet
        ret     

; Subroutine to get next character (A) from line. Carry reset if none left

.l1611  ld      hl,($fca1)      ; get address of ASCII text
        ld      a,l
        or      h
        jr      z,l1636         ; move on if none
        ld      a,(hl)
        inc     hl
        cp      $a0             ; test for terminating space
        ld      b,a     
        ld      a,$00           ; set "print leading space" if none
        jr      nz,l1623
        ld      a,$ff           ; else suppress
.l1623  ld      ($fc9e),a       ; set flag
        ld      a,b
        bit     7,a
        jr      z,l162e
        ld      hl,$0000        ; if last character, set "no text left"
.l162e  ld      ($fca1),hl      ; update address
        and     $7f             ; mask high bit
        jp      l1689           ; exit with success
.l1636  ld      hl,($fc9f)      ; get address of line data
        ld      a,l
        or      h
        jp      z,l168b         ; exit with fail if none
        call    l05a7           ; page in normal memory
.l1641  ld      a,(hl)
        cp      $0e             ; check for embedded number
        jr      nz,l164e
        inc     hl              ; if found, skip & loop back
        inc     hl
        inc     hl
        inc     hl
        inc     hl
        inc     hl
        jr      l1641
.l164e  call    l05cc           ; page in DOS workspace
        inc     hl
        ld      ($fc9f),hl      ; update address
        cp      $a5
        jr      c,l1661         ; move on unless 48K BASIC token
        sub     $a5             ; get token number (0+)
        call    $fcae           ; expand to ASCII text
        jp      l1611           ; go to get first char
.l1661  cp      $a3
        jr      c,l1675         ; move on unless +3 BASIC token
        jr      nz,l166c
        ld      hl,l168e        ; SPECTRUM token
        jr      l166f
.l166c  ld      hl,l1696        ; PLAY token
.l166f  call    $fd09           ; expand to ASCII text
        jp      l1611           ; go to get first char
.l1675  push    af
        ld      a,$00
        ld      ($fc9e),a       ; flag "print leading space"
        pop     af
        cp      $0d
        jr      nz,l1689        ; exit with success unless end of line
        ld      hl,$0000
        ld      ($fca1),hl      ; set no ASCII text
        ld      ($fc9f),hl      ; and no line data
.l1689  scf                     ; success
        ret     
.l168b  scf     
        ccf                     ; fail
        ret

.l168e  defm    "SPECTRU"&('M'+$80)
.l1696  defm    "PLA"&('Y'+$80)
        defm    "GOT"&('O'+$80)
        defm    "GOSU"&('B'+$80)
        defm    "DEFF"&('N'+$80)
        defm    "OPEN"&('#'+$80)
        defm    "CLOSE"&('#'+$80)




.l16b3  defb    $02
        defb    $01
        defb    $05

.l16b6  ld      hl,l16b3
        ld      de,$fd96
        jp      l2152



; Subroutine to ???

.l16bf  ld      l,b
        ld      h,$00
        add     hl,de
        ld      (hl),a
        inc     b
        ret     
        call    l16e0		; pad line at DE to length 32 with nulls 
        ld      a,(hl)
        or      $18
        ld      (hl),a
        ld      hl,$fd96
        set     0,(hl)
        scf     
        ret     
        call    l16e0		; pad line at DE to length 32 with nulls
        set     3,(hl)
        ld      hl,$fd96
        set     0,(hl)
        scf     
        ret     

; Subroutine to pad line at DE (length B) with nulls to length B=32
; On exit, HL=end of line+1

.l16e0  ld      l,b
        ld      h,0
        add     hl,de		; get past last char on line
        ld      a,$20
.l16e6  cp      b
        ret     z		; exit if 32 chars already 
        ld      (hl),0		; pad with zeros
        inc     hl
        inc     b
        jr      l16e6


.l16ee  ld      a,($fd97)
        ld      b,$00

.l16f3  ld      h,$00
        ld      l,b
        add     hl,de
        ld      (hl),$00
        inc     b
        dec     a
        jr      nz,l16f3            ; (-10)
        ret     

.l16fe  push    bc
        push    de
        push    hl
        push    hl
        ld      hl,$eef5
        bit     2,(hl)
        pop     hl
        jr      nz,l170e            ; (4)
        ld      b,c
        call    $1d2b

.l170e  pop     hl
        pop     de
        pop     bc
        ret     

.l1712  push    bc
        push    de
        push    hl
        push    hl
        ld      hl,$eef5
        bit     2,(hl)
        pop     hl
        jr      nz,l1722            ; (4)
        ld      e,c
        call    $1ccc

.l1722  pop     hl
        pop     de
        pop     bc
        ret     

.l1726  push    bc
        push    de
        push    hl
        push    hl
        ld      hl,$eef5
        bit     2,(hl)
        pop     hl
        jr      nz,l1736            ; (4)
        ld      e,c
        call    $1cd3

.l1736  pop     hl
        pop     de
        pop     bc
        ret     

; Subroutine to place cursor at column B, line C (of editing area)

.l173a  push    af
        push    bc
        push    de
        push    hl
        ld      a,b
        ld      b,c		; B=line of editing area
        ld      c,a		; C=column
        call    l1caa
        pop     hl
        pop     de
        pop     bc
        pop     af
        ret     

; Subroutine to remove cursor from column B, line C (of editing area)

.l1749  push    af
        push    bc
        push    de
        push    hl
        ld      a,b
        ld      b,c
        ld      c,a
        call    $1cbf
        pop     hl
        pop     de
        pop     bc
        pop     af
        ret     

.l1758  ld      a,$00
        ld      (MODE),a
        ld      a,$02
        ld      (REPPER),a
        call    l185a
        ret


        ld      (hl),h
        rst     $18
        sbc     a,$55
        djnz    $17be               ; (82)
        ret     
        sbc     a,(hl)
        sbc     a,(hl)
        cp      l
        ld      h,d
        push    bc
        ret     nz
        ld      d,l
        jp      nz,$1044
        rla     
        cp      $5f
        sub     b
        cp      $d1
        defb    $dd
        push    de
        rla     
        sub     b
        rst     $30
        rst     $18
        rst     $18
        call    nc,$d9c7
        sbc     a,$c3
        cp      l
        di      
        call    c,$d659
        ld      d,(hl)
        djnz    $17a8               ; (23)
        ld      h,h
        ld      b,a
        ld      e,a
        sub     b
        ld      h,b
        rst     $18
        ld      b,b

.l1798  ld      b,e
        sub     a
        djnz    $1798               ; (-4)
        ld      d,c
        rst     $0
        ld      b,e
        ld      e,a
        sbc     a,$bd
        ld      h,(hl)
        exx     

.l17a4  ld      e,e
        sub     b
        rla     
        jp      po,$54d5
        djnz    $17a4               ; (-8)
        ld      d,l
        ld      b,d
        ld      b,d
        ld      e,c
        sbc     a,$57
        rla     
        sub     b
        ld      a,a
        ld      e,h
        ld      e,h
        exx     
        add     a,$55
        jp      nz,$5190
        sbc     a,$bd
        call    po,$d5d8
        sub     b
        ld      (hl),h
        ld      e,a
        jp      nz,$595b
        ld      e,(hl)
        rst     $10
        sub     b
        ld      a,l
        ld      e,a
        jp      nc,$64bd
        ld      e,b
        pop     de

.l17d2  ld      e,(hl)
        in      a,($c3)
        djnz    $179b               ; (-60)
        ld      e,a
        ld      e,$1e
        cp      l
        ld      h,h
        ret     c
        push    de
        djnz    $17d2               ; (-14)
        ld      b,d
        push    de
        ld      b,a
        push    de
        ld      b,d
        ret     
        sub     b
        call    po,$c0d1
        sub     b
        defb    $18
        defb    88
        exx     
        out     ($19),a
        cp      l
        jp      po,$dc5f

.l17f4  ld      d,c
        sbc     a,$54
        inc     e
        sub     b
        ld      h,d
        exx     
        out     ($58),a
        ld      d,c
        jp      nz,$9054
        ld      d,l
        ld      b,h
        sub     b
        pop     de
        ld      e,h
        cp      l
        ld      d,c
        ld      e,(hl)
        ld      d,h
        sub     b
        ld      sp,hl
        add     a,$df
        ld      b,d
        djnz    $17a9               ; (-104)
        rst     $0
        ret     c
        ld      c,c
        djnz    $17f4               ; (-34)
        ld      e,a

.l1817  call    nz,$bd19
        ld      h,a
        ld      b,d
        exx     
        ld      b,h
        call    nz,$5ed5
        sub     b
        rst     $18
        ld      e,(hl)
        sub     b
        ld      h,b
        di      
        ld      h,a
        sub     b
        adc     a,b
        dec     b
        add     a,c
        ld      (bc),a
        ld      b,e
        inc     e
        djnz    $1876               ; (69)
        ld      b,e
        exx     
        ld      e,(hl)
        cp      l
        defb    $fd
        ex      af,af'
        nop     
        sub     b
        ld      d,c

.l183a  sbc     a,$d4
        djnz    $183a               ; (-4)
        ex      af,af'
        nop     
        djnz    $1887               ; (69)
        ld      e,(hl)
        call    nc,$c255
        djnz    $183b               ; (-13)
        ld      h,b
        sbc     a,a
        defb    $fd
        sbc     a,e
        cp      l
        cp      l
        jp      $c490
        rst     $18
        ld      e,$9e
        cp      l
        ld      h,h
        ret     c
        ld      d,l
        djnz    $1817               ; (-67)

.l185a  ld      hl,FLAGS
        ld      a,(hl)
        or      $0c
        ld      (hl),a
        ld      hl,ed_flags
        bit     4,(hl)
        ld      hl,FLAGS3
        jr      nz,l186e
        res     0,(hl)
        ret     
.l186e  set     0,(hl)
        ret     


; Subroutine to get a key - this may include keypad keys which
; send extended mode before a keycode to give a token

.l1871  push    hl              ; save HL
.l1872  ld      hl,FLAGS
.l1875  bit     5,(hl)
        jr      z,l1875         ; loop until a key is available
        res     5,(hl)          ; signal no key available
        ld      a,(LAST_K)      ; get key
        ld      hl,MODE
        res     0,(hl)          ; set "L" mode (?)
        cp      $20
        jr      nc,l1894        ; move on if not a control code
.l1887  cp      $10
        jr      nc,l1872        ; loop back to ignore control codes>=$10
        cp      $06
        jr      c,l1872         ; ignore control codes<$06
        call    l1896           ; change mode if required
        jr      nc,l1872        
.l1894  pop     hl              ; restore HL
        ret     
.l1896  rst     $28
        defw    $10db           ; call key mode change routine
        ret     

; Subroutine to display a menu
; On entry HL=address of menu

.l189a  push    hl
        call    l194d           ; save menu window area of screen
        ld      hl,TV_FLAG
        res     0,(hl)          ; signal "using main screen"
        pop     hl
        ld      e,(hl)          ; E=number of menu lines
        inc     hl
        push    hl
        ld      hl,l19fe
        call    l1925           ; output control codes for top menu line
        pop     hl
        call    l1925           ; output menu title
        push    hl
        call    l1a34           ; output rainbow
        ld      hl,l1a0c
        call    l1925           ; output end of top menu line
        pop     hl
        push    de
        ld      bc,$0807        
        call    l191d           ; output 'AT 8,7'
.l18c3  push    bc              ; save screen position
        ld      b,$0c           ; B=menu width 12 (+2 border)
        ld      a,' '
        rst     $10             ; output border space
.l18c9  ld      a,(hl)          ; get next char
        inc     hl
        cp      $80
        jr      nc,l18d2        ; move on if last char on menu line
        rst     $10             ; else output
        djnz    l18c9           ; & loop back for more
.l18d2  and     $7f             ; mask off end marker bit
        rst     $10             ; output last character
.l18d5  ld      a,$20
        rst     $10             ; output spaces for rest of menu width
        djnz    l18d5           
        pop     bc
        inc     b               ; get to next line
        call    l191d           ; output AT command
        dec     e
        jr      nz,l18c3        ; loop back for more lines
        ld      hl,$6f38        ; HL=pixel coords for top left of menu
        pop     de              ; E=total number of menu lines
        sla     e
        sla     e
        sla     e
        ld      d,e
        dec     d               ; D=menu height in pixel lines-1
        ld      e,$6f           ; E=menu width in pixel lines-1
        ld      bc,$ff00
        ld      a,d
        call    l190b           ; draw line top to bottom
        ld      bc,$0001
        ld      a,e
        call    l190b           ; draw line left to right
        ld      bc,$0100
        ld      a,d
        inc     a
        call    l190b           ; draw line bottom to top
        xor     a
        call    l19dc           ; put highlight on top line
        ret

; Subroutine to draw a line, used in menu display
; On entry, H=Y coord, L=X coord, A=line length, BC=amount to
; add to HL to get to next pixel

.l190b  push    af              ; save registers
        push    hl
        push    de
        push    bc
        ld      b,h
        ld      c,l
        rst     $28
        defw    $22e9           ; plot a pixel
        pop     bc              ; restore registers
        pop     de
        pop     hl
        pop     af
        add     hl,bc           ; get coords of next pixel
        dec     a
        jr      nz,l190b        ; back for rest of line
        ret

; Subroutine to output 'AT b,c'

.l191d  ld      a,$16
        rst     $10
        ld      a,b
        rst     $10
        ld      a,c
        rst     $10
        ret     

; Subroutine to output a $ff-terminated message (used in menus)
; If '+3' is encountered and a disk interface is not present, '+2e'
; is substituted

.l1925  ld      a,(hl)          ; get next character
        inc     hl
        cp      $ff
        ret     z               ; exit if end-of-message marker
        cp      '+'
        jr      z,l1931         ; move on to check '+'
.l192e  rst     $10             ; else output and loop back for more
        jr      l1925          
.l1931  ld      a,(hl)
        cp      '3'             ; check for '+3' string
        ld      a,$2b           ; reload with '+'
        jr      nz,l192e        ; go back to output if not '+3'
        push    hl
        ld      hl,FLAGS3
        bit     4,(hl)
        pop     hl
        jr      nz,l192e        ; go back to output if disk interface present
        inc     hl
        inc     hl              ; skip '+3 '
        ld      a,'+'           ; and output '+2A' instead
        rst     $10
        ld      a,'2'
        rst     $10
        ld      a,'A'
        jr      l192e           ; back

; This routine has a dual purpose: to either copy a "window" area to the
; screen, or to save a "window" area in high memory (of page 7)
; The window area is of fixed size 12 lines x 14 chars, located at
; 7,7
; Enter at l194d to copy the area FROM the screen, or at l1950 to copy
; the area TO the screen
; A total of:  21 (system variables)
;            + 12x14x8 (bitmap)
;            + 12x14 (attributes)
;            = 1533 bytes are saved, at $eef6 to $f4f2

.l194d  scf                     ; set carry (copying FROM screen)     
        jr      l1951
.l1950  and     a               ; reset carry (copying TO screen)
.l1951  ld      de,$eef6        ; DE contains window save area in page 7
        ld      hl,TV_FLAG
        jr      c,l195a
        ex      de,hl           ; swap source & dest if necessary
.l195a  ldi                     ; copy TV_FLAG system variable
        jr      c,l195f
        ex      de,hl           ; swap back
.l195f  ld      hl,COORDS
        jr      c,l1965         ; swap source & dest
        ex      de,hl
.l1965  ld      bc,$0014
        ldir                    ; copy COORDS to ATTR_T system variables
        jr      c,l196d
        ex      de,hl           ; swap back
.l196d  ex      af,af'          ; save carry flag
        ld      bc,$0707        ; Top left of window is 7,7
        call    l1da1           ; get C=33-C and B=24-B-(IX+1)
        ld      a,(ix+$01)
        add     a,b
        ld      b,a             ; correct B to 24-B
        ld      a,$0c           ; for 12 character lines
.l197b  push    bc              ; save registers
        push    af
        push    de
        rst     $28
        defw    $0e9b           ; get HL=address of line B in display file
        ld      bc,$0007
        add     hl,bc           ; HL=address of left of window line
        pop     de              ; restore save area in page 7
        call    l1990           ; copy a character line (width 14)
        pop     af              ; restore registers
        pop     bc
        dec     b               ; move to next character line
        dec     a
        jr      nz,l197b        ; loop back for more lines
        ret

; Subroutine used by menu window transfer routine, to transfer a single
; character line (width 14)
; On entry, HL=screen address of top pixel line
;           DE=address of save area
;           Carry'=save/restore flag

.l1990  ld      bc,$080e        ; B=8 pixel lines,C=14 bytes window width
.l1993  push    bc              ; save counters
        ld      b,$00           ; BC=bytes to copy
        push    hl              ; save screen address
        ex      af,af'          ; get carry flag
        jr      c,l199b
        ex      de,hl           ; swap source & dest if necessary
.l199b  ldir                    ; copy pixel line
        jr      c,l19a0
        ex      de,hl           ; swap back
.l19a0  ex      af,af'          ; save carry flag
        pop     hl              ; restore screen address
        inc     h               ; get to next pixel line
        pop     bc              ; restore counts
        djnz    l1993           ; back for more pixel lines
        push    bc
        push    de
        rst     $28
        defw    $0e88
        ex      de,hl           ; HL=attributes address
        pop     de
        pop     bc
        ex      af,af'          ; get carry flag
        jr      c,l19b2
        ex      de,hl           ; swap source & dest if necessary
.l19b2  ldir                    ; copy attributes
        jr      c,l19b7
        ex      de,hl           ; swap back
.l19b7  ex      af,af'          ; save carry flag
        ret

; Move menu highlight up a line
; On entry, A=current highlight line number, HL=address of menu size

.l19b9  call    l19dc           ; remove current highlight
        dec     a               ; decrement line number
        jp      p,l19c3         ; move on if still positive
        ld      a,(hl)          ; else set to bottom line
        dec     a
        dec     a
.l19c3  call    l19dc           ; replace highlight on new line
        scf                     ; set carry, so calling routine doesn't
        ret                     ;  immediately call "move highlight down"

; Move menu highlight down a line
; On entry, A=current highlight line number, HL=address of menu size

.l19c8  push    de              ; save DE
        call    l19dc           ; remove current highlight
        inc     a               ; increment line number
        ld      d,a             
        ld      a,(hl)
        dec     a
        dec     a
        cp      d               ; check against max line number
        ld      a,d
        jp      p,l19d7
        xor     a               ; set to 0 if too large
.l19d7  call    l19dc           ; replace highlight on new line
        pop     de              ; restore DE
        ret     

; Subroutine to switch menu line A (0=top) between highlighted and
; non-highlighted

.l19dc  push    af              ; save registers
        push    hl
        push    de
        ld      hl,$5907        ; attribute address of top menu line
        ld      de,$0020
        and     a
        jr      z,l19ec
.l19e8  add     hl,de           ; get to attribute address of required line
        dec     a
        jr      nz,l19e8
.l19ec  ld      a,$78
        cp      (hl)            ; is it BRIGHT 1, PAPER 7, INK 0?
        jr      nz,l19f3        ; if not, change to this
        ld      a,$68           ; if so, change to BRIGHT 1, PAPER 5, INK 0
.l19f3  ld      d,$0e           ; 14 characters to do
.l19f5  ld      (hl),a          ; change attributes
        inc     hl
        dec     d
        jr      nz,l19f5
        pop     de              ; restore registers
        pop     hl
        pop     af
        ret

; Control codes for top line of menus

.l19fe  defb    $16,$07,$07     ; AT 7,7
        defb    $15,$00,$14,$00 ; OVER 0,INVERSE 0
        defb    $10,$07,$11,$00 ; INK 7,PAPER 0
        defb    $13,$01         ; BRIGHT 1
        defb    $ff

; Control codes for end of top line of menus

.l1a0c  defb    $11,$00         ; PAPER 0
        defb    ' '
        defb    $11,$07         ; PAPER 7
        defb    $10,$00         ; INK 0
        defb    $ff

; A two-character "character set" used for displaying the
; rainbow on menus and bars

.l1a14  defb    $01,$03,$07,$0f
        defb    $1f,$3f,$7f,$ff
        defb    $fe,$fc,$f8,$f0
        defb    $e0,$c0,$80,$00

; The rainbow string

.l1a24  defb    $10,$02,' '
        defb    $11,$06,'!'
        defb    $10,$04,' '
        defb    $11,$05,'!'
        defb    $10,$00,' '
        defb    $ff

; Subroutine to output the "rainbow" on menus and bars
        
.l1a34  push    bc              ; save registers
        push    de
        push    hl
        ld      hl,l1a14
        ld      de,STRIP1
        ld      bc,$0010
        ldir                    ; copy rainbow charset into RAM
        ld      hl,(CHARS)
        push    hl              ; save CHARS
        ld      hl,STRIP1-$0100
        ld      (CHARS),hl      ; set to rainbow set
        ld      hl,l1a24
        call    l1925           ; output rainbow
        pop     hl
        ld      (CHARS),hl      ; restore CHARS
        pop     hl              ; restore registers
        pop     de
        pop     bc
        ret     

; Subroutines to display the bars for various functions

.l1a5a  ld      hl,l080f        ; +3 BASIC
        jr      l1a67

.l1a5f  ld      hl,l0817        ; Calculator
        jr      l1a67

.l1a64  ld      hl,l0809        ; Loader


; Subroutine to clear the bottom 3 lines to editor colours, and display
; a bar with a rainbow and the text at HL (bit 7-terminated) on line 21

.l1a67  push    hl
        call    l1a8e           ; clear bottom 3 lines to editor colours
        ld      hl,$5aa0        ; attribute address of line 21
        ld      b,$20
        ld      a,$40
.l1a72  ld      (hl),a          ; fill line 21 to BRIGHT 1, PAPER 0, INK 0
        inc     hl
        djnz    l1a72
        ld      hl,l19fe
        call    l1925           ; output control codes for top menu lines
        ld      bc,$1500
        call    l191d           ; output AT 21,0
        pop     de
        call    l029e           ; ouput the bar text
        ld      c,$1a           ; output AT 21,26
        call    l191d
        jp      l1a34           ; output the rainbow

; Subroutine to clear bottom 3 lines to editor colours

.l1a8e  ld      b,$15
        ld      d,$17
.l1a92  jp      l1d6b           ; clear bottom 3 lines to editor colours


; The renumber routine

.l1a95  call    l05a7           ; page in normal memory
        call    l1c12           ; get number of lines in BASIC program
        ld      a,d
        or      e
        jp      z,l1bcd         ; if none, signal "command failed" & exit
        ld      hl,(RC_STEP)
        rst     $28
        defw    $30a9           ; HL=STEP*number of lines
        ex      de,hl           ; DE=STEP*number of lines
        ld      hl,(RC_START)
        add     hl,de           ; HL=projected last line number
        ld      de,$2710
        or      a
        sbc     hl,de
        jp      nc,l1bcd        ; if >9999, signal "command failed" & exit
        ld      hl,(PROG)       ; get start of program
.l1ab7  rst     $28
        defw    $19b8           ; get address of next line
        inc     hl
        inc     hl
        ld      (RC_LINE),hl    ; store address of current line (after number)
        inc     hl
        inc     hl              ; point after line length
        ld      (STRIP2+$11),de ; store address of next line
.l1ac5  ld      a,(hl)          ; get next character
        rst     $28
        defw    $18b6           ; skip past embedded number if necessary
        cp      $0d
        jr      z,l1ad2         ; move on if end of line
        call    l1b1b           ; replace any line number in this command
        jr      l1ac5           ; loop back
.l1ad2  ld      de,(STRIP2+$11) ; get address of next line
        ld      hl,(VARS)
        and     a
        sbc     hl,de
        ex      de,hl
        jr      nz,l1ab7        ; loop back if not end of program
        call    l1c12
        ld      b,d
        ld      c,e             ; BC=number of lines in program
        ld      de,$0000
        ld      hl,(PROG)       ; HL=address of first line
.l1aea  push    bc              ; save registers
        push    de
        push    hl
        ld      hl,(RC_STEP)
        rst     $28
        defw    $30a9           ; HL=(line-1)*STEP
        ld      de,(RC_START)
        add     hl,de
        ex      de,hl           ; DE=new line number
        pop     hl
        ld      (hl),d
        inc     hl
        ld      (hl),e          ; store new number at start of line
        inc     hl
        ld      c,(hl)
        inc     hl
        ld      b,(hl)
        inc     hl
        add     hl,bc           ; get to start of next line
        pop     de
        inc     de              ; increment line count
        pop     bc
        dec     bc              ; decrement lines to do
        ld      a,b
        or      c
        jr      nz,l1aea        ; loop back for more
        call    l05cc           ; page back DOS workspace
        ld      (RC_LINE),bc    ; reset "current line being renumbered"
        scf                     ; signal "command succeeded"
        ret     

; Table of commands containing line numbers

.l1b14  defb    $ca             ; LINE
        defb    $f0             ; LIST
        defb    $e1             ; LLIST
        defb    $ec             ; GOTO
        defb    $ed             ; GOSUB
        defb    $e5             ; RESTORE
        defb    $f7             ; RUN

; Subroutine to replace any line number in the current statement.
; On entry, HL=address of code, A=code
; On exit HL=address of next code to check

.l1b1b  inc     hl
        ld      (STRIP2+$0f),hl ; save pointer after command
        ex      de,hl
        ld      bc,$0007
        ld      hl,l1b14
.l1b26  cpir                    ; check if one of line number commands
        ex      de,hl
        ret     nz              ; exit if not
        ld      c,$00           ; set BC=0
.l1b2c  ld      a,(hl)          ; get next character
        cp      $20
        jr      z,l1b4c         ; go to skip spaces
        rst     $28
        defw    $2d1b           ; is it a digit?
        jr      nc,l1b4c        ; go to skip if so
        cp      '.'
        jr      z,l1b4c         ; go to skip decimal point
        cp      $0e
        jr      z,l1b50         ; move on if found embedded number
        or      $20
        cp      $65
        jr      nz,l1b48        ; if it's not an "e", exit
        ld      a,b
        or      c
        jr      nz,l1b4c        ; found any characters suggesting a number?
.l1b48  ld      hl,(STRIP2+$0f) ; if not, exit with pointer after command
        ret     
.l1b4c  inc     bc              ; increment characters found
        inc     hl
        jr      l1b2c           ; loop back for more
.l1b50  ld      (STRIP2+$07),bc ; save no of characters before embedded #
        push    hl              ; save pointer to embedded number
        rst     $28
        defw    $18b6           ; skip past embedded number 
        call    l1c43           ; skip past spaces
        ld      a,(hl)          ; get following character
        pop     hl              ; restore pointer to embedded number
        cp      ':'
        jr      z,l1b64
        cp      $0d
        ret     nz              ; exit if following character not : or ENTER
.l1b64  inc     hl              ; HL points to next statement/line
        rst     $28
        defw    $33b4           ; stack the embedded number
        rst     $28
        defw    $2da2           ; get embedded number to BC
        ld      h,b
        ld      l,c             ; HL=embedded line number
        rst     $28
        defw    $196e           ; get HL=address of target line
        jr      z,l1b7c         ; move on if the actual line was found
        ld      a,(hl)
        cp      $80
        jr      nz,l1b7c        ; or if there is a line afterwards (not end)
        ld      hl,$270f        ; use 9999 and move on
        jr      l1b8d
.l1b7c  ld      (STRIP2+$0d),hl ; save target line address
        call    l1c18           ; get DE=number of lines before it
        ld      hl,(RC_STEP)
        rst     $28
        defw    $30a9
        ld      de,(RC_START)
        add     hl,de           ; HL=target line's new number
.l1b8d  ld      de,STRIP2+$09
        push    hl              ; save number
        call    l1c49           ; form ASCII representation of it
        ld      e,b
        inc     e
        ld      d,$00           ; DE=length of ASCII string
        push    de              ; save length
        push    hl              ; and address of string
        ld      l,e
        ld      h,$00
        ld      bc,(STRIP2+$07) ; get number of characters available
        or      a
        sbc     hl,bc
        ld      (STRIP2+$07),hl ; save number of extra chars required
        jr      z,l1bdc         ; move on if right size
        jr      c,l1bd2         ; move on if more chars available than needed
        ld      b,h
        ld      c,l             ; BC=chars to insert
        ld      hl,(STRIP2+$0f) ; HL=address to insert at
        push    hl              ; save registers
        push    de
        ld      hl,(STKEND)
        add     hl,bc
        jr      c,l1bcb         ; move on to signal error if no room
        ex      de,hl
        ld      hl,$0082
        add     hl,de
        jr      c,l1bcb         ; error if can't leave $82 bytes free
        sbc     hl,sp
        ccf
        jr      c,l1bcb         ; or if would encroach on stack
        pop     de              ; restore registers
        pop     hl
        rst     $28
        defw    $1655           ; make room
        jr      l1bdc           ; move on
.l1bcb  pop     de
        pop     hl
.l1bcd  call    l05cc           ; page in DOS workspace
        and     a               ; signal "command failed"
        ret                     ; exit
.l1bd2  dec     bc
        dec     e
        jr      nz,l1bd2        ; BC=number of bytes to reclaim
        ld      hl,(STRIP2+$0f)
        rst     $28
        defw    $19e8           ; reclaim room
.l1bdc  ld      de,(STRIP2+$0f)
        pop     hl
        pop     bc
        ldir                    ; copy ASCII text of line number
        ex      de,hl
        ld      (hl),$0e        ; store embedded number marker
        pop     bc              ; BC=new line number
        inc     hl
        push    hl              ; save address to place FP number
        rst     $28
        defw    $2d2b           ; stack BC on FP stack (HL=address)
        pop     de
        ld      bc,$0005
        ldir                    ; copy FP representation
        ex      de,hl
        push    hl              ; save address of next byte to check
        ld      hl,(RC_LINE)
        push    hl              ; save address of current line
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=length of current line
        ld      hl,(STRIP2+$07)  
        add     hl,de
        ex      de,hl           ; DE=new length of current line
        pop     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; store new length
        ld      hl,(STRIP2+$11)
        ld      de,(STRIP2+$07)
        add     hl,de
        ld      (STRIP2+$11),hl ; store new next line address
        pop     hl              ; restore address of next byte to check
        ret     

; Subroutine to count the number of lines in a BASIC program,
; either to the end (enter at l1c12), or to a certain address (enter
; at l1c18 with address in STRIP2+$0d).
; Number of lines is returned in DE

.l1c12  ld      hl,(VARS)
        ld      (STRIP2+$0d),hl ; save VARS
.l1c18  ld      hl,(PROG)
        ld      de,(STRIP2+$0d)
        or      a
        sbc     hl,de
        jr      z,l1c3e         ; move on if no BASIC program in memory
        ld      hl,(PROG)       ; start at PROG
        ld      bc,$0000        ; with 0 lines
.l1c2a  push    bc
        rst     $28
        defw    $19b8           ; find DE=address of next line
        ld      hl,(STRIP2+$0d)
        and     a
        sbc     hl,de
        jr      z,l1c3b         ; move on if no more lines
        ex      de,hl           ; else swap to HL
        pop     bc
        inc     bc              ; increment line count
        jr      l1c2a           ; loop back
.l1c3b  pop     de              ; restore number of lines
        inc     de              ; increment for last line
        ret     
.l1c3e  ld      de,$0000        ; BASIC program length=0
        ret     

; Subroutine to skip spaces

.l1c42  inc     hl
.l1c43  ld      a,(hl)          ; get next char
        cp      ' '
        jr      z,l1c42         ; skip if space
        ret                     ; exit with HL pointing to non-space

; Subroutine to form a text representation of a binary number up to 9999
; On entry, DE=address to form number, HL=number
; On exit, HL=start address of text number, DE=end address+1, B=#digits-1

.l1c49  push    de              ; save start address of text number
        ld      bc,-1000
        call    l1c6d           ; form 1000s digit
        ld      bc,-100
        call    l1c6d           ; form 100s digit
        ld      c,-10
        call    l1c6d           ; form 10s digit
        ld      a,l
        add     a,'0'
        ld      (de),a          ; form units digit
        inc     de
        ld      b,$03           ; check first 3 digits
        pop     hl              ; restore start address of text number
.l1c63  ld      a,(hl)
        cp      '0'
        ret     nz              ; exit if non-zero digit
        ld      (hl),' '        ; replace leading 0s with spaces
        inc     hl
        djnz    l1c63           ; loop back
        ret

; Subroutine to form a decimal digit from a binary number
; On entry, HL=number, DE=address to store digit, BC=-unit size
; On exit, HL is reduced and DE is incremented

.l1c6d  xor     a               ; zero counter
.l1c6e  add     hl,bc           ; subtract unit size
        inc     a               ; and increment digit counter
        jr      c,l1c6e         ; loop back for more until failed
        sbc     hl,bc           ; add back last unit
        dec     a               ; and decrement counter
        add     a,'0'
        ld      (de),a          ; place ASCII digit
        inc     de              ; increment address
        ret

.l1c7a  defb    $08
        defb    0
        defb    0
        defb    $14
        defb    0
        defb    0
        defb    0
        defb    $0f
        defb    0

.l1c83  defb    $08
        defb    0
        defb    $16
        defb    $01
        defb    0
        defb    0
        defb    0
        defb    $0f
        defb    0

; Subroutine to ???

.l1c8c  ld      ix,$fd98
        ld      hl,l1c7a
        jr      l1c98               ; (3)

.l1c95  ld      hl,l1c83
.l1c98  ld      de,$fd98
        jp      l2152


.l1c9e  rst     $10
        ld      a,d
        rst     $10
        scf     
        ret     

; Subroutine to set cursor colours to A

.l1ca3  and     $3f		; mask off FLASH/BRIGHT bits
        ld      (ix+$06),a	; set colours
        scf     
        ret     

; Subroutine to place cursor at column C, line B (of editing area)

.l1caa  ld      a,(ix+$01)	; get line for top of editing area
        add     a,b
        ld      b,a		; B=line
        call    l1dad		; get attribute address
        ld      a,(hl)
        ld      (ix+$07),a	; save attribute
        cpl     
        and     $c0		; get inverse of current FLASH/BRIGHT bits
        or      (ix+$06)	; combine with cursor colour
        ld      (hl),a		; set new attribute
        scf     
        ret     

; Subroutine to remove cursor from column C, line B (of editing area)

.l1cbf  ld      a,(ix+$01)	; get line for top of editing area
        add     a,b
        ld      b,a		; B=line
        call    l1dad		; get attribute address
        ld      a,(ix+$07)
        ld      (hl),a		; restore attribute
        ret     


.l1ccc  push    hl
        ld      h,$00
        ld      a,e
        sub     b
        jr      l1cda               ; (7)

.l1cd3  push    hl
        ld      a,e
        ld      e,b
        ld      b,a
        sub     e
        ld      h,$ff

.l1cda  ld      c,a
        ld      a,b
        cp      e
        jr      z,l1d2a             ; (75)
        push    de
        call    $1da5

.l1ce3  push    bc
        ld      c,h
        rst     $28
        defw    $0e9b
        defb    $eb
        xor     a
        or      c
        jr      z,l1cf0             ; (3)
        inc     b
        jr      l1cf1               ; (1)

.l1cf0  dec     b

.l1cf1  push    de
        rst     $28
        defw    $0e9b
        defb    $d1
        ld      a,c
        ld      c,$20
        ld      b,$08

.l1cfb  push    bc
        push    hl
        push    de
        ld      b,$00
        ldir    
        pop     de
        pop     hl
        pop     bc
        inc     h
        inc     d
        djnz    $1cfb               ; (-14)
        push    af
        push    de
        rst     $28
        defw    $0e88
        defb    $eb
        ex      (sp),hl
        rst     $28
        defw    $0e88
        defb    $eb
        ex      (sp),hl
        pop     de
        ld      bc,$0020
        ldir    
        pop     af
        pop     bc
        and     a
        jr      z,l1d23             ; (3)
        inc     b
        jr      l1d24               ; (1)

.l1d23  dec     b

.l1d24  dec     c
        ld      h,a
        jr      nz,l1ce3            ; (-69)
        pop     de
        ld      b,e

.l1d2a  pop     hl

.l1d2b  call    l1dc5           ; swap editor/BASIC colours & P_FLAG
        ex      de,hl
        ld      a,(TV_FLAG)
        push    af
        ld      hl,ed_flags
        bit     6,(hl)
        res     0,a
        jr      z,l1d3e             ; (2)
        set     0,a

.l1d3e  ld      (TV_FLAG),a
        ld      c,$00
        call    $191d
        ex      de,hl
        ld      b,$20

.l1d49  ld      a,(hl)
        and     a
        jr      nz,l1d4f            ; (2)
        ld      a,$20

.l1d4f  cp      $90
        jr      nc,l1d62            ; (15)
        rst     $28
        defw    $0010

.l1d56  inc     hl
        djnz    $1d49               ; (-16)
        pop     af
        ld      (TV_FLAG),a
        call    l1dc5           ; swap editor/BASIC colours & P_FLAG
        scf     
        ret     

.l1d62  call    $05a7
        rst     $10
        call    $05cc
        jr      l1d56               ; (-21)

; Subroutine to clear an area of screen to the editor's colours
; On entry, B=first line number and D=last line number (0...23)

.l1d6b  call    l1dc5           ; swap editor/BASIC colours & P_FLAG
        ld      a,d
        sub     b
        inc     a
        ld      c,a             ; C=number of lines to clear
        call    l1da5           ; convert line number as required by ROM 3
.l1d75  push    bc
        rst     $28
        defw    $0e9b           ; get HL=address of line B in display file
        ld      c,$08           ; 8 pixel lines per character
.l1d7b  push    hl
        ld      b,$20           ; 32 characters per line
        xor     a
.l1d7f  ld      (hl),a          ; clear a pixel line of a character
        inc     hl
        djnz    l1d7f           ; back for rest of line
        pop     hl
        inc     h
        dec     c
        jr      nz,l1d7b        ; back for rest of pixel lines
        ld      b,$20
        push    bc
        rst     $28
        defw    $0e88           ; get attribute address
        ex      de,hl
        pop     bc
        ld      a,(ATTR_P)
.l1d93  ld      (hl),a          ; clear attributes to editor's ATTR_P
        inc     hl
        djnz    l1d93           ; for rest of line
        pop     bc
        dec     b               ; next line
        dec     c               ; decrement counter
        jr      nz,l1d75        ; back for more
        call    l1dc5           ; swap editor/BASIC colours & P_FLAG
        scf     
        ret

; Subroutine to convert line numbers and column numbers as required
; by certain ROM 3 routines

.l1da1  ld      a,$21
        sub     c
        ld      c,a             ; C=33-oldC
.l1da5  ld      a,$18
        sub     b
        sub     (ix+$01)
        ld      b,a             ; B=24-oldB-??
        ret     

; Subroutine to get attribute address for line B, column C into HL

.l1dad  push    bc
        xor     a
        ld      d,b
        ld      e,a
        rr      d
        rr      e
        rr      d
        rr      e
        rr      d
        rr      e		; DE=B*32
        ld      hl,$5800	; start of attribs
        ld      b,a
        add     hl,bc
        add     hl,de		; form address
        pop     bc
        ret     

; Subroutine to swap some system variables with copies in page 7, allowing
; BASIC & the editor to use different values for colours etc

.l1dc5  push    af              ; save registers
        push    hl
        push    de
        ld      hl,(ATTR_P)     ; swap permanent & temporary colours with
        ld      de,(ATTR_T)     ; editor ones
        exx     
        ld      hl,(ed_ATTR_P)
        ld      de,(ed_ATTR_T)
        ld      (ATTR_P),hl
        ld      (ATTR_T),de
        exx     
        ld      (ed_ATTR_P),hl
        ld      (ed_ATTR_T),de
        ld      hl,ed_P_FLAG    ; swap P_FLAG with editor one
        ld      a,(P_FLAG)
        ld      d,(hl)
        ld      (hl),a
        ld      a,d
        ld      (P_FLAG),a
        pop     de              ; restore registers
        pop     hl
        pop     af
        ret     

.l1df6  ld      a,$01
        jr      l1dfc               ; (2)

.l1dfa  ld      a,$00

.l1dfc  ld      ($fdb6),a
        ld      hl,$0000
        ld      ($fdb1),hl
        ld      ($fdb3),hl
        add     hl,sp
        ld      ($fdb7),hl
        call    $15e4
        ld      a,$00
        ld      ($fdb0),a
        ld      hl,$fda0
        ld      ($fda9),hl
        call    $05a7
        rst     $28
        defw    $16b0
        defb    $cd
        call    z,$3e05
        nop     
        ld      ($fdad),a
        ld      hl,(E_LINE)
        ld      ($fdae),hl
        ld      hl,$0000
        ld      ($fdab),hl

.l1e34  ld      hl,($fdb1)
        inc     hl
        ld      ($fdb1),hl
        call    $1f30
        ld      c,a
        ld      a,($fdad)
        cp      $00
        jr      nz,l1e87            ; (65)

.l1e46  ld      a,c
        and     $04
        jr      z,l1e80             ; (53)

.l1e4b  call    $1f7c
        jr      nc,l1e57            ; (7)
        ld      a,$01
        ld      ($fdad),a
        jr      l1e34               ; (-35)

.l1e57  ld      hl,($fdab)
        ld      a,l
        or      h
        jp      nz,$1eb1

.l1e5f  push    bc
        call    $1f60
        pop     bc
        ld      a,$00
        ld      ($fdad),a

.l1e69  ld      a,c
        and     $01
        jr      nz,l1e46            ; (-40)
        ld      a,b
        call    $1fa9
        ret     nc
        ld      hl,($fdb1)
        inc     hl
        ld      ($fdb1),hl
        call    $1f30
        ld      c,a
        jr      l1e69               ; (-23)

.l1e80  ld      a,b
        call    $1fa9
        ret     nc
        jr      l1e34               ; (-83)

.l1e87  cp      $01
        jr      nz,l1e80            ; (-11)
        ld      a,c
        and     $01
        jr      z,l1e4b             ; (-69)
        push    bc

.l1e91  call    $2113
        pop     bc
        jr      c,l1f10             ; (121)
        ld      hl,($fdab)
        ld      a,h
        or      l
        jr      nz,l1eb1            ; (19)
        ld      a,c
        and     $02
        jr      z,l1e5f             ; (-68)
        call    $1f7c
        jr      nc,l1e57            ; (-81)
        ld      hl,($fda9)
        dec     hl
        ld      ($fdab),hl
        jr      l1e34               ; (-125)

.l1eb1  push    bc
        ld      hl,$fda0
        ld      de,($fdab)
        ld      a,d
        cp      h
        jr      nz,l1ec2            ; (5)
        ld      a,e
        cp      l
        jr      nz,l1ec2            ; (1)
        inc     de

.l1ec2  dec     de
        jr      l1ec6               ; (1)

.l1ec5  inc     hl

.l1ec6  ld      a,(hl)
        and     $7f
        push    hl
        push    de
        call    $1fa9
        pop     de
        pop     hl
        ld      a,h
        cp      d
        jr      nz,l1ec5            ; (-15)
        ld      a,l
        cp      e
        jr      nz,l1ec5            ; (-19)
        ld      de,($fdab)
        ld      hl,$fda0
        ld      ($fdab),hl
        ld      bc,($fda9)
        dec     bc
        ld      a,d
        cp      h
        jr      nz,l1f03            ; (24)
        ld      a,e
        cp      l
        jr      nz,l1f03            ; (20)
        inc     de
        push    hl
        ld      hl,$0000
        ld      ($fdab),hl
        pop     hl
        ld      a,b
        cp      h
        jr      nz,l1f03            ; (7)
        ld      a,c
        cp      l
        jr      nz,l1f03            ; (3)
        pop     bc
        jr      l1f22               ; (31)

.l1f03  ld      a,(de)
        ld      (hl),a
        inc     hl
        inc     de
        and     $80
        jr      z,l1f03             ; (-8)
        ld      ($fda9),hl
        jr      l1e91               ; (-127)

.l1f10  push    bc
        call    $1fa9
        pop     bc
        ld      hl,$0000
        ld      ($fdab),hl
        ld      a,($fdad)
        cp      $04
        jr      z,l1f27             ; (5)

.l1f22  ld      a,$00
        ld      ($fdad),a

.l1f27  ld      hl,$fda0
        ld      ($fda9),hl
        jp      $1e46

.l1f30  call    $0e22
        ld      b,a
        cp      $3f
        jr      c,l1f42             ; (10)
        or      $20
        call    $1f59
        jr      c,l1f56             ; (23)

.l1f3f  ld      a,$01
        ret     

.l1f42  cp      $20
        jr      z,l1f53             ; (13)
        cp      $23
        jr      z,l1f50             ; (6)
        jr      c,l1f3f             ; (-13)
        cp      $24
        jr      nz,l1f3f            ; (-17)

.l1f50  ld      a,$02
        ret     

.l1f53  ld      a,$03
        ret     

.l1f56  ld      a,$06
        ret     

.l1f59  cp      $7b
        ret     nc
        cp      $61
        ccf     
        ret     

.l1f60  ld      hl,$fda0
        ld      ($fda9),hl
        sub     a
        ld      ($fdab),a
        ld      ($fdac),a

.l1f6d  ld      a,(hl)
        and     $7f
        push    hl
        call    $202f
        pop     hl
        ld      a,(hl)
        and     $80
        ret     nz
        inc     hl
        jr      l1f6d               ; (-15)

.l1f7c  ld      hl,($fda9)
        ld      de,$fda9
        ld      a,d
        cp      h
        jr      nz,l1f8b            ; (5)
        ld      a,e
        cp      l
        jp      z,$1fa6

.l1f8b  ld      de,$fda0
        ld      a,d
        cp      h
        jr      nz,l1f96            ; (4)
        ld      a,e
        cp      l
        jr      z,l1f9c             ; (6)

.l1f96  dec     hl
        ld      a,(hl)
        and     $7f
        ld      (hl),a
        inc     hl

.l1f9c  ld      a,b
        or      $80
        ld      (hl),a
        inc     hl
        ld      ($fda9),hl
        scf     
        ret     

.l1fa6  scf     
        ccf     
        ret     

.l1fa9  push    af
        ld      a,($fdb5)
        or      a
        jr      nz,l1fc2            ; (18)
        pop     af
        cp      $3e
        jr      z,l1fbd             ; (8)
        cp      $3c
        jr      z,l1fbd             ; (4)

.l1fb9  call    $1ff7
        ret     

.l1fbd  ld      ($fdb5),a
        scf     
        ret     

.l1fc2  cp      $3c
        ld      a,$00
        ld      ($fdb5),a
        jr      nz,l1fe5            ; (26)
        pop     af
        cp      $3e
        jr      nz,l1fd4            ; (4)
        ld      a,$c9
        jr      l1fb9               ; (-27)

.l1fd4  cp      $3d
        jr      nz,l1fdc            ; (4)
        ld      a,$c7
        jr      l1fb9               ; (-35)

.l1fdc  push    af
        ld      a,$3c
        call    $1ff7
        pop     af
        jr      l1fb9               ; (-44)

.l1fe5  pop     af
        cp      $3d
        jr      nz,l1fee            ; (4)
        ld      a,$c8
        jr      l1fb9               ; (-53)

.l1fee  push    af
        ld      a,$3e
        call    $1ff7
        pop     af
        jr      l1fb9               ; (-62)

.l1ff7  cp      $0d
        jr      z,l201b             ; (32)
        cp      $ea
        ld      b,a
        jr      nz,l2007            ; (7)
        ld      a,$04
        ld      ($fdad),a
        jr      l2015               ; (14)

.l2007  cp      $22
        jr      nz,l2015            ; (10)
        ld      a,($fdad)
        and     $fe
        xor     $02
        ld      ($fdad),a

.l2015  ld      a,b
        call    $202f
        scf     
        ret     

.l201b  ld      a,($fdb6)
        cp      $00
        jr      z,l202c             ; (10)
        ld      bc,($fdb1)
        ld      hl,($fdb7)
        ld      sp,hl
        scf     
        ret     

.l202c  scf     
        ccf     
        ret     

.l202f  ld      e,a
        ld      a,($fdb0)
        ld      d,a
        ld      a,e
        cp      $20
        jr      nz,l2059            ; (32)
        ld      a,d
        and     $01
        jr      nz,l2052            ; (20)
        ld      a,d
        and     $02
        jr      nz,l204a            ; (7)
        ld      a,d
        or      $02
        ld      ($fdb0),a
        ret     

.l204a  ld      a,e
        call    $208e
        ld      a,($fdb0)
        ret     

.l2052  ld      a,d
        and     $fe
        ld      ($fdb0),a
        ret     

.l2059  cp      $a3
        jr      nc,l2081            ; (36)
        ld      a,d
        and     $02
        jr      nz,l206d            ; (11)
        ld      a,d
        and     $fe
        ld      ($fdb0),a
        ld      a,e
        call    $208e
        ret     

.l206d  push    de
        ld      a,$20
        call    $208e
        pop     de
        ld      a,d
        and     $fe
        and     $fd
        ld      ($fdb0),a
        ld      a,e
        call    $208e
        ret     

.l2081  ld      a,d
        and     $fd
        or      $01
        ld      ($fdb0),a
        ld      a,e
        call    $208e
        ret     

.l208e  ld      hl,($fdb3)
        inc     hl
        ld      ($fdb3),hl
        ld      hl,($fdae)
        ld      b,a
        ld      a,($fdb6)
        cp      $00
        ld      a,b
        jr      z,l20c6             ; (37)
        ld      de,(X_PTR)
        ld      a,h
        cp      d
        jr      nz,l20c3            ; (26)
        ld      a,l
        cp      e
        jr      nz,l20c3            ; (22)
        ld      bc,($fdb1)
        ld      hl,($fdb3)
        and     a
        sbc     hl,bc
        jr      nc,l20bd            ; (4)
        ld      bc,($fdb3)

.l20bd  ld      hl,($fdb7)
        ld      sp,hl
        scf     
        ret     

.l20c3  scf     
        jr      l20c8               ; (2)

.l20c6  scf     
        ccf     

.l20c8  call    $05a7
        jr      nc,l20da            ; (13)
        ld      a,(hl)
        ex      de,hl
        cp      $0e
        jr      nz,l20f0            ; (29)
        inc     de
        inc     de
        inc     de
        inc     de
        inc     de
        jr      l20f0               ; (22)

.l20da  push    af
        ld      bc,$0001
        push    hl
        push    de
        call    $20f9
        pop     de
        pop     hl
        rst     $28
        defw    $1664
        defb    $2a
        ld      h,l
        ld      e,h
        ex      de,hl
        lddr    
        pop     af
        ld      (de),a

.l20f0  inc     de
        call    $05cc
        ld      ($fdae),de
        ret     

.l20f9  ld      hl,(STKEND)
        add     hl,bc
        jr      c,l2109             ; (10)
        ex      de,hl
        ld      hl,$0082
        add     hl,de
        jr      c,l2109             ; (3)
        sbc     hl,sp
        ret     c

.l2109  ld      a,$03
        ld      (ERR_NR),a
        call    l3e80
        defw    $25cb

.l2113  call    $142d
        call    $fd43
        ret     c
        ld      b,$f9
        ld      de,$fda0
        ld      hl,$168e
        call    $fd5c
        ret     nc
        cp      $ff
        jr      nz,l212e            ; (4)
        ld      a,$d4
        jr      l2150               ; (34)

.l212e  cp      $fe
        jr      nz,l2136            ; (4)
        ld      a,$d3
        jr      l2150               ; (26)

.l2136  cp      $fd
        jr      nz,l213e            ; (4)
        ld      a,$ce
        jr      l2150               ; (18)

.l213e  cp      $fc
        jr      nz,l2146            ; (4)
        ld      a,$ed
        jr      l2150               ; (10)

.l2146  cp      $fb
        jr      nz,l214e            ; (4)
        ld      a,$ec
        jr      l2150               ; (2)

.l214e  sub     $56

.l2150  scf     
        ret

; Subroutine to transfer a counted string (minus count) from (HL) to (DE)

.l2152  ld      b,(hl)          ; get count
        inc     hl
.l2154  ld      a,(hl)
        ld      (de),a          ; transfer a byte
        inc     de
        inc     hl
        djnz    l2154           ; back for more
        ret

; Subroutine to check if char in A is a digit. If so, carry set & A=value

.l215b  cp      '0'
        ccf     
        ret     nc              ; exit if less than "0"
        cp      ':'
        ret     nc              ; or if > "9"
        sub     '0'             ; convert to value
        scf                     ; success
        ret

; Subroutine to perform a routine found with a table lookup
; The value to look for is in A, and the table address is in HL
; The table consists of:
;   a number of entries byte
;   for each entry: a value byte followed by a routine address
; When a match is found, the routine is called. A is tested for
; zero before returning, carry status is preserved and should be
; set by the routine if action succeeded.
; If no match is found, carry is reset.

.l2166  push    bc              ; save registers
        push    de
        ld      b,(hl)          ; B=# entries in table
        inc     hl
.l216a  cp      (hl)            ; check next entry
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=address associated with current entry
        jr      z,l2179         ; if match, move on
        inc     hl
        djnz    l216a           ; loop back for more
        scf     
        ccf                     ; clear carry to signal "no match"
        pop     de              ; restore registers
        pop     bc
        ret     
.l2179  ex      de,hl           ; HL=address
        pop     de              ; restore registers
        pop     bc
        call    l2186           ; call routine address in HL
        jr      c,l2183         ; if carry set, go to test A & set carry
        cp      a               ; test A (& clear carry)
        ret     
.l2183  cp      a               ; test A
        scf                     ; set carry
        ret     
.l2186  jp      (hl)


; Routine called from ROM 2 to display an error message in HL
; On entry Z is set if DE contains a response key list to use

.l2187  jr      z,l218c
        ld      de,$0000	; no response list
.l218c  push    de
        push    hl
        ld      a,$fd
        rst     $28
        defw    $1601		; open channel to stream -3
        pop	hl
        push    hl
.l2195  ld      b,$20		; 32 chars per line
.l2197  ld      a,(hl)		; get next char
        cp      ' '
        jr      nz,l219e
        ld      d,h		; if space, set DE to current position
        ld      e,l
.l219e  cp      $ff
        jr      z,l21ab		; move on if end of message
        inc     hl
        djnz    l2197		; loop back for more
        ex      de,hl
        ld      a,$0d
        ld      (hl),a		; insert a CR at last space
        jr      l2195		; back for more lines
.l21ab  ld      a,$16		; start at 0,0 in stream -3
        rst     $10
        ld      a,$00
        rst     $10
        ld      a,$00
        rst     $10
        pop     hl		; restore message start
.l21b5  ld      a,(hl)		; get next char
        cp      $ff
        jr      z,l21be		; move on if end
        rst     $10		; output char
        inc     hl
        jr      l21b5		; back for more
.l21be  call    l1871		; get a key
        ld      b,a		; B=key
        pop     hl		; restore response key list
        ld      a,h
        or      l
        push    hl
        jr      z,l21d3		; move on if none required
.l21c8  ld      a,(hl)
        cp      b
        jr      z,l21d3		; move on if response key found
        inc     hl
        cp      $ff
        jr      nz,l21c8	; loop back if more possibilities to check
        jr      l21be		; else get another key
.l21d3  push    af
        rst     $28
        defw    $0d6e		; clear lower screen
        ld      a,$fe
        rst     $28
        defw    $1601		; open channel to stream -2
        pop	af
        pop     hl
        ret     
     
; *************** START OF SELF-TEST PROGRAM SECTION ****************

; The self-test program, entered by pressing "QAZPLM" at the test screen        
        
.l21df  di                      ; disable interrupts
        ld      ix,$ffff        ; IX=top of RAM
        ld      a,$07
        out     ($fe),a         ; white border
        ld      sp,$7fff        ; set stack to top of page 5
        call    l28f7           ; initialise & show title page
        di                      ; disable interrupts
        ld      a,$07
        out     ($fe),a         ; white border
        call    l271a           ; clear screen
.l21f6  ld      bc,$0700
        call    l270b           ; set INK 7, PAPER 0
        ld      hl,l3261
        call    l2703           ; display RAM test message
        call    l269a           ; pause for 0.8s
        ld      a,$04
.l2207  out     ($fe),a         ; green border
        ld      de,$0002        ; D=test pattern,E=pass counter
.l220c  ld      a,$00           ; start with page 0
        ld      bc,$7ffd
.l2211  out     (c),a           ; page in next page
        ex      af,af'          ; save page number
        ld      hl,$c000
.l2217  ld      (hl),d          ; fill page with D
        inc     hl
        ld      a,l
        or      h
        jr      nz,l2217        ; until page filled
        ex      af,af'
        inc     a
        cp      $08
        jr      nz,l2211        ; back for more pages
        ld      a,$00           ; start at page 0 again
.l2225  out     (c),a           ; page in next page
        ex      af,af'          ; save page number
        ld      hl,$c000
.l222b  ld      a,(hl)
        cp      d               ; check pattern
        jr      nz,l2262        ; move on if error
        cpl     
        ld      (hl),a          ; store inverse pattern
        inc     hl
        ld      a,l
        or      h
        jr      nz,l222b        ; until page done
        ex      af,af'
        inc     a
        cp      $08
        jr      nz,l2225        ; back for more pages
        dec     a               ; back to page 7
.l223d  ex      af,af'
        ld      hl,$0000        ; start at end of memory
.l2241  dec     hl
        ld      a,h
        cp      $bf
        jr      z,l224f         ; move on if at start of page
        ld      a,d
        cpl     
        cp      (hl)            ; check inverse pattern
        jr      nz,l2262        ; move on if error
        ld      (hl),d          ; store normal pattern
        jr      l2241           ; loop back
.l224f  ex      af,af'
        cp      $00
        jr      z,l2259
        dec     a
        out     (c),a
        jr      l223d           ; back for more pages
.l2259  dec     e               ; decrement pass counter
        jp      z,l2329         ; move on if successful
        ld      d,$ff
        jp      l220c           ; back for second pass with new pattern
.l2262  ex      af,af'
        push    af              ; save page and address of failure
        push    hl
        call    l266f           ; re-initialise
        xor     a
        ld      bc,$7ffd
        ld      (BANKM),a
        out     (c),a           ; page in page 0
        ld      bc,$0700
        call    l270b           ; set INK 7, PAPER 0
        call    l271a           ; clear screen
        ld      hl,l22ba
        call    l2703           ; display RAM test fail message
        pop     hl
        ld      a,h
        call    l2299           ; output high byte of address
        ld      a,l
        call    l2299           ; output low byte of address
        exx     
        ld      hl,l22fe
        call    l2703           ; output page message
        pop     af
        and     $07
        exx     
        call    l2299           ; output page number
        di      
        halt                    ; halt!

; Subroutine to output A as two hexadecimal digits

.l2299  push    hl              ; save registers
        push    af
        push    af
        srl     a
        srl     a
        srl     a
        srl     a               ; A=A/16
        ld      b,$02           ; two digits
.l22a6  exx     
        ld      d,$00
        ld      e,a             ; DE=0...F
        ld      hl,l2306
        add     hl,de
        ld      a,(hl)          ; A=ASCII hex digit
        call    l2716           ; output it
        pop     af              ; restore value
        and     $0f             ; get low nibble
        exx     
        djnz    l22a6           ; loop back for second digit
        pop     hl
        ret

.l22ba  defm    $16&$0a&$0
        defm    "RAM fail: address "&$ff
        
; The "EUA" routine from the testcard, which changes attributes based
; on input at EAR

.l22d0  ld      a,$00
        out     ($fe),a         ; set black border
        ld      hl,$4000
        ld      de,$4001
        ld      bc,$1800
        ld      (hl),$00
        ldir                    ; clear screen to black
.l22e1  ld      hl,$5800        ; start of attributes
.l22e4  ld      bc,$0300        ; B=loop counter,C=black paper
.l22e7  in      a,($fe)
        and     $40             ; get EAR bit
        or      c
        ld      c,a
        rr      c               ; combine into C
        inc     ix              ; delay
        dec     ix
        djnz    l22e7           ; loop 3 times to form a paper colour
        ld      (hl),c          ; store in attributes
        inc     hl              ; move to next
        ld      a,h
        cp      $5b
        jr      nz,l22e4        ; loop back for whole screen
        jr      l22e1           ; start again


.l22fe  defm    ", page "&$ff
.l2306  defm    "0123456789ABCDEF"
.l2316  defm    $16&$0a&$0
        defm    "RAM test passed"&$ff

; Continuation of self-test program

.l2329  ld      a,$00           ; start with page 0
        ld      hl,$cafe        ; location to use
        ld      bc,$7ffd
.l2331  out     (c),a           ; page in page
        ld      (hl),a          ; and store page number
        inc     a
        cp      $08
        jr      nz,l2331        ; back for more
.l2339  dec     a
        out     (c),a           ; page in page
        cp      (hl)            ; check correct number
        jp      nz,l2457        ; if not, go to signal ULA error
        and     a
        jr      nz,l2339        ; back for more
        di      
        ld      bc,$7ffd
        ld      a,$00           ; start at page 0
.l2349  out     (c),a           ; page in page
        ld      ($e000),a       ; store number at midpoint of page
        inc     a
        cp      $08
        jr      nz,l2349        ; back for more
        dec     a
        call    l235f           ; copy routine to page 7
        ld      a,$03
        call    l235f           ; and to page 3
        jp      $d000           ; jump into it in page 3

; Subroutine to copy the following routine into page A at $d000

.l235f  ld      bc,$7ffd
        out     (c),a           ; page in required page
        ld      hl,l2370
        ld      de,$d000
        ld      bc,$00b9
        ldir                    ; copy routine
        ret     

; Routine to be executed in pages 3 & 7 ($2370-$2328 at $d000-$d0b8)
; to test the RAM configurations (all of which have one of these pages
; at the top)

.l2370  ld      a,$01           ; start with RAM configuration 0,1,2,3
        ld      de,$d0a9        ; start of table of configs
        ld      hl,$2000        ; location of page number within lowest page
.l2378  ld      bc,$1ffd
        out     (c),a           ; page in next configuration
        ex      af,af'
        ld      bc,$4000
.l2381  ld      a,(de)          
        inc     de
        cp      (hl)            ; check correct page in place
        jr      nz,l2394        ; if not, move on
        add     hl,bc           ; add offset to next segment
        jr      nc,l2381        ; loop back for more
        ex      af,af'
        add     a,$02           ; increment configuration
        bit     3,a
        jr      z,l2378         ; loop back if more to test
        ld      d,$01           ; flag "RAM configurations passed"
.l2392  jr      l2397
.l2394  ex      af,af'
        ld      d,$00           ; flag "RAM configurations failed"
.l2397  exx     
        ld      bc,$7ffd
        ld      a,$03
        out     (c),a           ; make sure page 3 is selected
        ld      b,$1f
        xor     a
        out     (c),a           ; and ROM 0
        call    $d09b           ; get checksum of ROM 0
        jr      nz,l23f2        ; if not zero, go to error
        ld      bc,$7ffd
        ld      a,$13
        out     (c),a           ; get ROM 1
        call    $d09b           ; and its checksum
        jr      nz,l23f2        ; if not zero, go to error
        scf     
        call    $d08e           ; rotate ROM0/1 success into flags
.l23b9  ld      bc,$1ffd
        ld      a,$04
        out     (c),a
        ld      bc,$7ffd
        ld      a,$03
        out     (c),a           ; get ROM 2
        call    $d09b           ; and its checksum
        jr      nz,l23f8        ; if not zero, go to error
        ld      a,$0b
        ld      bc,$7ffd
        out     (c),a           ; get ROM 3
        call    $d09b           ; and its checksum
        jr      nz,l23f8        ; if not zero, go to error
        scf     
        call    $d08e           ; rotate ROM2/3 success into flags
.l23dc  ld      a,$03
        ld      bc,$7ffd
        out     (c),a           ; make sure page 3 is paged in
        ld      a,$00
        ld      b,$1f
        out     (c),a           ; with ROM 0
        ld      (BANKM),a
        ld      (BANK678),a
        jp      l2436           ; jump back into routine in ROM
.l23f2  xor     a
        call    $d08e           ; rotate ROM0/1 fail into flags
        jr      l23b9
.l23f8  xor     a
        call    $d08e           ; rotate ROM2/3 fail into flags
        jr      l23dc

; Subroutine to rotate a test flag bit (1=success) into IX

.ld08e  push    de
        push    ix
        pop     de
        rl      e
        rl      d
        push    de
        pop     ix
        pop     de
        ret     

; Subroutine to form an 8-bit addition checksum of the current ROM
; All ROMs should checksum to zero

.ld09b  xor     a               ; zero checksum
        ld      h,a
        ld      l,a             ; and address
.l240e  add     a,(hl)          ; add next byte
        inc     hl
        ld      d,a
        ld      a,h
        cp      $40
        ld      a,d
        jr      nz,l240e        ; back for rest of page
        and     a               ; check if zero
        ret     

; Table of +3 special RAM configurations to test

.ld0a9  defb    $00,$01,$02,$03
        defb    $04,$05,$06,$07
        defb    $04,$05,$06,$03
        defb    $04,$07,$06,$03

; Subroutine to rotate carry flag (1=pass,0=fail) into IX

.l2429  push    de              ; save DE
        push    ix
        pop     de              ; DE=test results
        rl      e               ; rotate carry into test results
        rl      d
        push    de
        pop     ix              ; IX=test results
        pop     de              ; restore DE
        ret     

; Re-entry into self-test routine from RAM here

.l2436  ld      bc,$7ffd
        ld      a,$00
        out     (c),a           ; page in page 0
        ld      sp,$7fff        ; set stack in page 5
        exx     
        xor     a
        cp      d               ; set carry if RAM configs test passed
        call    l2429           ; rotate RAM configs test result into flags
        call    l266f           ; re-initialise
        call    l271a           ; clear screen
        ld      hl,l2316
        call    l2703           ; display RAM test passed message
        call    l269a           ; pause for 0.8s
        jr      l2461           ; move on

; routine comes here if ULA error with normal RAM paging

.l2457  call    l266f           ; reinitialise
        ex      af,af'          ; get page
        ld      hl,$0000        ; specify address 0000 for page error
        jp      l2262           ; display error and halt

; More self-test program

.l2461  call    l2c99           ; GI sound test
        call    l28b6           ; Symshift-A test
        call    l295d           ; ULA test
        call    l274d           ; keyboard test
        call    l2c1f           ; ULA sound test part 1
        call    l2c6e           ; ULA sound test part 2
        call    l29d0           ; joystick test
        call    l2b24           ; cassette output test
        call    l2a4b           ; screen switching test
        call    l2488           ; printer BUSY test & data test
        call    l3558           ; integral disk test
        call    l35a3           ; tape test
        jp      l2b91           ; move on to display results

; Printer BUSY test & data test

.l2488  call    l271a           ; cls
        ld      hl,l25d5        
        call    l2703           ; display test message
        ld      hl,l25f3        ; ask for printer OFFLINE then any key
        call    l2703
        ld      hl,l2613
        call    l2703
        ld      hl,l261e
        call    l2703
        call    l253d           ; wait for a key
        ld      bc,$0ffd
        in      a,(c)
        bit     0,a             ; check BUSY signal
        jr      z,l24c4         ; move on if not set
        ld      hl,l2608
        call    l2703           ; ask for ONLINE then any key
        call    l253d           ; wait for a key
        ld      bc,$0ffd
        in      a,(c)
        bit     0,a             ; check BUSY signal
        jr      nz,l24c4        ; move on if set
        scf                     ; signal success
        jr      l24c6           
.l24c4  scf     
        ccf                     ; signal fail
.l24c6  call    l2429           ; set success/fail flag
        call    l271a           ; cls
        ld      hl,BANK678
        set     4,(hl)
        ld      hl,l2549
        call    l2703           ; display test messages
        ld      hl,l255e
        call    l2703
        ld      e,$00           ; chars per line count
.l24df  ld      a,' '           ; start with space
.l24e1  push    af
        call    l24f4           ; output character
        pop     af
        inc     a               ; get next ASCII code
        cp      $80
        jr      z,l24f0         ; go to skip $80-$9f
        or      a
        jr      z,l24df         ; restart after $ff at space
        jr      l24e1
.l24f0  ld      a,$a0
        jr      l24e1

; Subroutine to output a character to the printer

.l24f4  call    l2507           ; output it
        push    af              
        inc     e               ; increment chars printed this line
        ld      a,e
        cp      $48
        jr      nz,l2505
        ld      e,$00           ; if $48, reset and start new line
        ld      a,$0d
        call    l2507
.l2505  pop     af
        ret     

; Subroutine to output a character to the printer,
; checking for Quit from user, exiting to higher calling
; routine

.l2507  push    af
.l2508  ld      a,$fb
        in      a,($fe)
        rra                     ; get Q status in carry
        jr      nc,l252c        ; move on if pressed
        ld      bc,$0ffd
        in      a,(c)
        bit     0,a             ; check BUSY status
        jr      nz,l2508        ; loop back if busy
        pop     af
        out     (c),a           ; place character at port
        di      
        ld      a,(BANK678)
.l251f  ld      bc,$1ffd
        xor     $10
        out     (c),a           ; STROBE parallel port
        bit     4,a
        jr      z,l251f         ; loop back to change state if necessary
        ei      
        ret     
.l252c  pop     af              ; discard AF
        pop     af              ; discard return address
        pop     af              ; and two stacked values from calling routine
        pop     af
        ld      hl,l2597
        call    l2703           ; ask if printed OK
        call    l26af           ; get SPACE/ENTER
        call    l2429           ; set success/fail
        ret                     ; exit to earlier routine

; Subroutine to wait until a new key is available

.l253d  ld      hl,FLAGS
        res     5,(hl)
.l2542  bit     5,(hl)
        jr      z,l2542
        res     5,(hl)
        ret

; Printer test messages

.l2549  defm    $16&$4&$4
        defm    "Printer data test"&$ff
.l255e  defm    $16&$8&$1
        defm    "Make sure printer is ready"
        defm    $16&$0a&$1
        defm    "Press Q to quit printing"&$ff
.l2597  defm    $16&$0c&$01
        defm    "If characters printed OK,"&$0d
        defm    "Press [ENTER], otherwise [SPACE]"&$ff
.l25d5  defm    $15&$0&$16&$4&$4
        defm    "Printer BUSY signal test"&$ff
.l25f3  defm    $16&$8&$8
        defm    "Turn the printer "&$ff
.l2608  defm    $16&$0a&$0c
        defm    "ONLINE "&$ff
.l2613  defm    $16&$0a&$0c
        defm    "OFFLINE"&$ff
.l261e  defm    $16&$0c&$4
        defm    "Press any key to continue"&$ff
        defm    $16&$10&$5
        defm    "Passed - press [ENTER]"&$ff
        defm    $16&$10&$5
        defm    "Failed - press [SPACE]"&$ff

; Subroutine to do some initialisation

.l266f  ld      a,$52           ; signal "in test program"
        ex      af,af'
        jp      l016c           ; do some initialisation & return here
.l2675  ld      a,$02
        rst     $28
        defw    $1601           ; open channel to stream 2
.l267a  ld      hl,l2681
        call    l2703           ; output normal colour control codes
        ret

; "Normal" colour control codes

.l2681  defb    $10,$00,$11,$07
        defb    $13,$00,$14,$00
        defb    $15,$00,$12,$00
        defb    $ff

; Apparently unused routine, to pause

.l268e  push    bc
        push    hl
        ld      b,$19
        ei      
.l2693  halt    
        djnz    l2693
.l2696  pop     hl
.l2697  pop     bc
        di      
        ret     


; Subroutine to pause for approx 0.8s

.l269a  ld      b,$28
        ei      
.l269d  halt    
        djnz    l269d
        di      
        ret     

; Pause subroutine

.l26a2  ld      hl,$3000
.l26a5  dec     hl
        push    ix
        pop     ix
        ld      a,l
        or      h
        jr      nz,l26a5
        ret     

; Subroutine to wait for ENTER or SPACE to be pressed.
; On exit, carry is set if ENTER was pressed, reset if SPACE.

.l26af  push    hl              ; save registers
        push    de
        push    bc
        ld      bc,$00fe
.l26b5  in      a,(c)           ; scan all keyrows
        and     $1f
        cp      $1f
        jr      nz,l26b5        ; loop back if any are pressed
.l26bd  call    l26d2           ; get a scancode
        cp      $21             ; check for ENTER
        scf     
.l26c3  jr      z,l26c9         ; if so, move on with carry set
        cp      $20             ; check for SPACE
        jr      nz,l26bd        ; loop back if not
.l26c9  push    af              ; save carry
        call    l2720           ; make a beep
        pop     af              ; restore registers
        pop     bc
        pop     de
        pop     hl
        ret     
.l26d2  call    l26d6
        ret     

; Subroutine to read the keyboard, returning a scancode
; in DE. This routine is a virtual copy of the routine
; at 028e in ROM 3, but doesn't return until a scancode
; has been detected.

.l26d6  ld      l,$2f
        ld      de,$ffff
        ld      bc,$fefe
.l26de  in      a,(c)
        cpl     
        and     $1f
        jr      z,l26f4
        ld      h,a
        ld      a,l
.l26e7  inc     d
        jr      nz,l26d6
.l26ea  sub     $08
        srl     h
        jr      nc,l26ea
        ld      d,e
        ld      e,a
        jr      nz,l26e7
.l26f4  dec     l
        rlc     b
        jr      c,l26de
        ld      a,e
        cp      $ff
        ret     nz
        ld      a,d
        cp      $ff
        jr      z,l26d6
        ret

; Subroutine to output an $ff-terminated string

.l2703  ld      a,(hl)          ; get byte
        cp      $ff
        ret     z               ; exit if $ff
        rst     $10             ; else output
        inc     hl
        jr      l2703           ; back for more


; Subroutine to set colours to INK b, PAPER c

.l270b  ld      a,$10
        rst     $10
        ld      a,b
        rst     $10
        ld      a,$11
        rst     $10
        ld      a,c
        rst     $10
        ret     

; Subroutine to output a character

.l2716  rst     $28
        defw    $0010
.l2719  ret     

; Subroutine to clear screen

.l271a  rst     $28
        defw    $0daf           ; call ROM 3 CLS
        jp      l267a           ; go to set "normal" colours & exit

; Subroutine to make a beep

.l2720  ld      hl,$0100        ; parameters for the beep
        ld      de,$00a0
.l2726  call    l34e9           ; call BEEPER
        di                      ; re-disable interrupts
        ret     

; Copy of next routine, apparently unused

        ld      a,$7f
        in      a,($fe)
        rra     
        ret     

; Subroutine to check for SPACE, exiting with carry reset if pressed

.l2731  ld      a,$7f
        in      a,($fe)
        rr      a
        ret     

; Apparently unused routine, to check for SPACE+Symbol shift

        ld      a,$7f
        in      a,($fe)
        or      $e0
        cp      $fc
        ret     

; Apparently unused routine, to output D spaces and set A'='8'

        ld      a,'8'
        ex      af,af'
.l2744  ld      a,' '
        call    l2716
        dec     d
        jr      nz,l2744
        ret     

; Keyboard test

.l274d  call    l271a           ; cls
        ld      a,$38
        ld      (BORDCR),a
        ld      a,$07
        out     ($fe),a         ; white border
        ld      hl,l2f17
        call    l2703           ; display test message
        call    l26a2           ; pause
        call    l2766           ; execute test
        ret     

.l2766  ld      hl,l2841        ; keyboard test table
        push    hl
.l276a  pop     hl
        ld      a,(hl)          ; get next test value
        inc     a
        ret     z               ; exit if end of table
        push    hl
        call    l26d2           ; get keyscan code
        ld      bc,$1000
.l2775  dec     bc
        ld      a,b
        or      c
        jr      nz,l2775        ; timing loop
        ld      a,d
        cp      $ff
        jr      z,l27e0         ; move on if no first key
        cp      $27
        jr      z,l2790         ; move on if capsshift held
        cp      $18
        jr      z,l27db         ; move on if symshift held
        ld      a,e             ; swap scancodes
        ld      e,d
        ld      d,a
        cp      $18
        jr      z,l27db         ; move on if symshift held
        jr      l276a           ; loop back if no shifts
.l2790  ld      a,e             ; here we substitute various codes for
        cp      $23             ;  keys with caps shift
        ld      e,$28
        jr      z,l27f9
        cp      $24
        ld      e,$29
        jr      z,l27f9
        cp      $1c
        ld      e,$2a
        jr      z,l27f9
        cp      $14
        ld      e,$2b
        jr      z,l27f9
        cp      $0c
        ld      e,$2c
        jr      z,l27f9
        cp      $04
        ld      e,$2d
        jr      z,l27f9
        cp      $03
        ld      e,$2e
        jr      z,l27f9
        cp      $0b
        ld      e,$2f
        jr      z,l27f9
        cp      $13
        ld      e,$30
        jr      z,l27f9
        cp      $1b
        ld      e,$31
        jr      z,l27f9
        cp      $20
        ld      e,$32
        jr      z,l27f9
        cp      $18
        ld      e,$37
        jr      z,l27f9
        jr      l276a           ; loop back for untested caps shift codes
.l27db  ld      a,e             ; here we substitute various codes for keys
        cp      $10             ;  with symbol shift
        ld      e,$33
.l27e0  jr      z,l27f9
        cp      $08
        ld      e,$34
        jr      z,l27f9
        cp      $1a
        ld      e,$35
        jr      z,l27f9
        cp      $22
        ld      e,$36
        jr      z,l27f9
        ld      e,$37
        jp      nz,$276a
.l27f9  ld      a,e             ; A=final scancode
        pop     hl
        push    hl
        cp      (hl)            ; test against table entry
        jp      nz,l276a        ; loop back if not equal
        pop     hl
        inc     hl              ; get to next entry
        push    hl
        ld      hl,$0080
        ld      de,$0080
        push    af
        push    bc
        push    ix
        call    l2726           ; make a beep
        pop     ix
        pop     bc
        pop     af
        pop     hl
        push    hl
        ld      a,$11           ; set PAPER 2
        rst     $10
        ld      a,$02
        rst     $10
        dec     hl
        ld      de,$003b
        add     hl,de
        ld      a,(hl)          ; get position code from table
        and     $f0
        rra     
        rra     
        rra     
        ld      b,$06
        add     a,b
        ld      b,a             ; B=line+1
        ld      a,(hl)
        and     $0f
        rla     
        ld      c,$01
        add     a,c             ; C=column
        ld      c,a
        ld      a,$16           ; output space to blank key at correct pos
        rst     $10
        ld      a,b
        dec     a
        rst     $10
        ld      a,c
        rst     $10
        ld      a,' '
        rst     $10
        jp      l276a           ; back for more

; Keyboard test table of scancodes

.l2841  defb    $2b,$2c,$24,$1c
        defb    $14,$0c,$04,$03
        defb    $0b,$13,$1b,$23
        defb    $32,$28,$31,$25
        defb    $1d,$15,$0d,$05
        defb    $02,$0a,$12,$1a
        defb    $22,$37,$29,$26
        defb    $1e,$16,$0e,$06
        defb    $01,$09,$11,$19
        defb    $21,$27,$2a,$1f
        defb    $17,$0f,$07,$00
        defb    $08,$10,$33,$27
        defb    $18,$35,$36,$2d
        defb    $30,$20,$2f,$2e
        defb    $34,$18,$ff

; Keyboard test table of screen positions

        defb    $00,$01,$02,$03
        defb    $04,$05,$06,$07
        defb    $08,$09,$0a,$0b
        defb    $0d,$10,$11,$13
        defb    $14,$15,$16,$17
        defb    $18,$19,$1a,$1b
        defb    $1c,$20,$21,$23
        defb    $24,$25,$26,$27
        defb    $28,$29,$2a,$2b
        defb    $2d,$30,$31,$33
        defb    $34,$35,$36,$37
        defb    $38,$39,$3a,$3d
        defb    $40,$41,$42,$43
        defb    $44,$47,$4a,$4b
        defb    $4c,$4d

; Symbolshift-A test

.l28b6  call    l271a           ; cls
        ld      a,$05
        out     ($fe),a         ; cyan border
        ld      bc,$0600
        call    l270b           ; INK 6, PAPER 0
        ld      hl,l30d9
        call    l2703           ; display test message
.l28c9  call    l28e5
        jr      nz,l28c9        ; loop back until symshift-A pressed
        ld      bc,$0200        ; timing counter
.l28d1  push    bc
        call    l28e5           ; check for symshift-A
        pop     bc
        jr      nz,l28e1        ; exit with error if not pressed
        dec     bc
        ld      a,b
        or      c
        jr      nz,l28d1        ; loop back to test again
        scf     
        jp      l2429           ; exit, setting "success" flag
.l28e1  or      a
        jp      l2429           ; exit, setting "fail" flag

; Subroutine to check if symbol-shift & A are being pressed
; On exit, Z is set if both are being pressed

.l28e5  call    l26d2           ; get key scan code
        ld      a,d
        cp      $18
        jr      z,l28f3         ; move on if first key is sym/shft
        ld      a,e             ; swap keys
        ld      e,d
        ld      d,a
        cp      $18
        ret     nz              ; exit with Z reset if 2nd key not sym/shft
.l28f3  ld      a,e
        cp      $26             ; compare other key with A and exit
        ret     

; Subroutine to initialise test environment, display title & get
; colour test results

.l28f7  call    l266f           ; do some initialisation
        call    l291d           ; fill attributes, do sound registers
        ld      bc,$0000
        ld      hl,l311e
        call    l2703           ; output test program title
        call    l26af           ; wait for ENTER or SPACE
        call    l2429           ; update results flags
        ld      c,$fd           ; some sound stuff
        ld      d,$ff
        ld      e,$bf
        ld      h,$ff
        ld      b,d
        ld      a,$07
        out     (c),a
        ld      b,e
        out     (c),h
        ret

; Subroutine to fill attributes & set up some sound registers

.l291d  xor     a
        ld      hl,$5800        ; start of attributes
        ld      b,$10           ; 16 x 2chars = 1 line
.l2923  ld      (hl),a
        inc     hl
        ld      (hl),a          ; colour next two chars
        add     a,$08           ; increment paper & higher attributes
        inc     hl
        djnz    l2923           ; back for rest of line
        ld      de,$5820
        ld      bc,$02df
        ldir                    ; fill rest of attribs with what's on line 1
        ld      c,$fd
        ld      d,$ff
        ld      e,$bf
        ld      hl,l2d0c        ; sound data
.l293c  ld      a,(hl)          ; loop to set up two sound registers
        inc     hl
        bit     7,a
        jr      nz,l294c
        ld      b,d
        out     (c),a
        ld      a,(hl)
        inc     hl
        ld      b,e
        out     (c),a
        jr      l293c
.l294c  ld      c,$fd           ; set up more sound registers
        ld      d,$ff
        ld      e,$bf
        ld      h,$fb
        ld      b,d
        ld      a,$07
        out     (c),a
        ld      b,e
        out     (c),h
        ret     


; ULA test

.l295d  call    l271a           ; cls
        ld      hl,l324e
        call    l2703           ; display test message
        ld      de,$6000
        call    l298c           ; copy test routine to RAM at $6000
        call    $6000           ; execute test
        ld      a,$aa           ; test RAM integrity
        ld      ($8000),a
        ld      a,($8000)
        cp      $aa
        jr      nz,l2984        ; move on if error
        ld      de,$8000
        call    l298c           ; copy test routine to RAM at $8000
        call    $8000           ; execute test
.l2984  ld      a,$06
        out     ($fe),a         ; yellow border
        scf     
        jp      l2429           ; exit, setting "success" flag

; Subroutine to copy following routine to memory at DE

.l298c  ld      hl,l2995
        ld      bc,$003b
        ldir    
        ret     

; Routine to execute in RAM to test ULA

.l2995  ld      bc,$2000        ; $2000 times
.l2998  ld      a,$00           ; alternately output $00/$ff to port $fe
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        out     ($fe),a
        cpl     
        dec     bc
        ld      a,b
        or      c
        jr      nz,l2998        ; loop back for more
        ret     

; Joystick test

.l29d0  call    l271a           ; cls
        ld      hl,l32e0
        call    l2703           ; display test message
        ld      de,$1f1f        ; initially, no bits reset in D or E ports
.l29dc  push    de
        ld      bc,$effe
        in      a,(c)
        cpl     
        and     d
        xor     d
        ld      d,a             ; mask in reset bits from port 1 to D
        ld      bc,$f7fe
        in      a,(c)
        cpl     
        and     e
        xor     e
        ld      e,a             ; mask in reset bits from port 2 to D
        pop     bc
        ld      a,d
        cp      b
        jr      nz,l2a06        ; move on if change in port 1
        ld      a,e
        cp      c
        jr      nz,l2a06        ; move on if change in port 2
        call    l2731           ; check for SPACE
        jr      c,l29dc
        jr      z,l29dc
        and     a
        jr      l2a03           ; fail if SPACE pressed
.l2a02  scf                     ; set success
.l2a03  jp      l2429           ; exit, setting success/fail flag
.l2a06  push    de
        ld      hl,l3530        ; table of screen positions
        ld      b,$05           ; 5 bits for first joystick port in D
.l2a0c  rrc     d
        call    nc,l2a28        ; blank screen chars if reset
        inc     hl
        inc     hl
        djnz    l2a0c
        ld      b,$05           ; 5 bits for second joystick port in E
.l2a17  rrc     e
        call    nc,l2a28        ; blank screen chars if reset
        inc     hl
        inc     hl
        djnz    l2a17
        pop     de
        ld      a,d
        or      e
        jr      z,l2a02         ; exit with success once all reset
        jp      l29dc           ; back for more

; Joystick subroutine to output two red spaces at position
; in table referenced at HL

.l2a28  push    bc
        ld      b,(hl)
        inc     hl
        ld      c,(hl)          ; get position from table
        ld      a,$16           ; output spaces at position
        rst     $10
        ld      a,b
        dec     a
        dec     a
        dec     a
        rst     $10
        ld      a,c
        rst     $10
        ld      a,$11
        rst     $10
        ld      a,$02
        rst     $10
        ld      a,' '
        rst     $10
        ld      a,' '
        rst     $10
        ld      a,$11
        rst     $10
        ld      a,$07
        rst     $10
        dec     hl
        pop     bc
        ret     

; Screen switching test

.l2a4b  call    l271a           ; cls
        di      
        ld      hl,$5800
        ld      de,$5801
        ld      bc,$02ff
        ld      (hl),$00
        ldir                    ; set page 5 screen attributes to black
        ld      hl,l2acb
        call    l2703           ; display test message
        ld      hl,l2ae8
        call    l2703           ; and success - press ENTER message
        ld      a,(BANKM)
        push    af
        or      $07
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; switch in page 7
        ld      hl,$4000
        ld      de,$c000
        ld      bc,$1800
        ldir                    ; copy screen into page 7
        ld      hl,l2acb        ; display test message
        call    l2703
        ld      hl,l2b06        ; and fail - press SPACE message
        call    l2703
        ld      a,(BANKM)
        set     3,a
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; switch in alternate screen
        ld      hl,$5800
        ld      de,$5801
        ld      bc,$02ff
        ld      (hl),$38
        ldir                    ; change attribs on normal screen to visible
        ld      hl,$d800
        ld      de,$d801
        ld      bc,$02ff
        ld      (hl),$38
        ldir                    ; and on alternate screen
        call    l26af           ; get ENTER or SPACE as appropriate
        call    l2429           ; set success/failure flag
        call    l271a           ; cls
        pop     af
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; switch back original memory & screen
        call    l271a           ; cls
        ei      
        ret     

.l2acb  defm    $16&$8&$3&$11&$0&$10&$0
        defm    "Screen switching test"&$ff
.l2ae8  defm    $16&$0c&$3&$11&$0&$10&$0
        defm    "Passed - press [ENTER]"&$ff
.l2b06  defm    $16&$0c&$3&$11&$0&$10&$0
        defm    "Failed - press [SPACE]"&$ff

; Cassette output test

.l2b24  call    l271a           ; cls
        ld      a,$02
        out     ($fe),a         ; red border
        ld      a,$08
        ld      (BORDCR),a
        ld      hl,l32c1        
        call    l2703           ; display test message
        ld      hl,$0100        ; ouput a tone to MIC
        ld      de,$0a00
        di      
        push    ix
        ld      a,l
        srl     l
.l2b42  srl     l
        cpl     
        and     $03
        ld      c,a
        ld      b,$00
        ld      ix,l2b5a
        add     ix,bc
        ld      a,(BORDCR)
        and     $38
        rrca    
        rrca    
        rrca    
.l2b58  or      $10
.l2b5a  nop     
        nop     
        nop     
        inc     b
        inc     c
.l2b5f  dec     c
        jr      nz,l2b5f
        ld      c,$3f
        dec     b
        jp      nz,l2b5f
        xor     $08
        out     ($fe),a
        ld      b,h
        ld      c,a
        bit     3,a
        jr      nz,l2b7b
        ld      a,d
        or      e
        jr      z,l2b7f
        ld      a,c
        ld      c,l
.l2b78  dec     de
        jp      (ix)
.l2b7b  ld      c,l
        inc     c
        jp      (ix)
.l2b7f  pop     ix
        ld      bc,$1200
        ld      hl,l2d21
.l2b87  call    l2703           ; ask if heard sound
        call    l26af           ; get ENTER/SPACE
        call    l2429           ; signal success/failure
        ret     


; End of test program - display results

.l2b91  call    l271a           ; cls
        push    ix
        pop     de              ; DE=results
        ld      a,e
        and     d
        cp      $ff
        jr      nz,l2ba9        ; move on if any test failed
        ld      a,$04
        out     ($fe),a         ; green border
        ld      hl,l346f
        call    l2703           ; display success message
        jr      l2bee           ; move on
.l2ba9  ld      a,$02
        out     ($fe),a         ; red border
        ld      hl,l3486
        call    l2703           ; display fail message
        ld      bc,$0807
        push    ix
        pop     de
        push    de              ; E=low byte of results
        ld      hl,l2ec7        ; HL=table of message addresses
        ld      d,$08           ; 8 bits
.l2bbf  rr      e               ; get next result
        jr      c,l2bce         ; move on if passed
        push    hl
        push    de
        ld      a,(hl)
        inc     hl
        ld      h,(hl)
        ld      l,a
        call    l2703           ; display appropriate message
        pop     de
        pop     hl
.l2bce  inc     hl              ; get to next table entry
        inc     hl
        dec     d
        jr      nz,l2bbf        ; back for more bits
        pop     de
        ld      e,d             ; E=high byte of results
        ld      d,$08           ; 8 bits
.l2bd7  rr      e               ; get next result
        jr      c,l2be9         ; move on if passed
        push    hl
        push    de
        push    bc
        ld      a,(hl)
        inc     hl
        ld      h,(hl)
        ld      l,a
        call    l2703           ; display appropriate message
        pop     bc
        pop     de
        pop     hl
        inc     b
.l2be9  inc     hl              ; get to next table entry
        inc     hl
        dec     d
        jr      nz,l2bd7        ; loop back for more bits
.l2bee  ld      hl,l2c00
        ld      bc,$1000
        call    l2703           ; display end message
.l2bf7  call    l26af           ; get SPACE or ENTER
        jr      c,l2bf7         ; loop until SPACE
        di      
        jp      $0000           ; reset


.l2c00  defm    $0d&$0d
        defm    "Hold [BREAK] to repeat tests"&$ff

; ULA sound test

.l2c1f  ld      c,$fd           ; some sound stuff
        ld      d,$ff           ; to clear AY registers ?
        ld      e,$bf
        ld      b,d
        ld      a,$0e
        out     (c),a
        ld      a,$ff
        ld      b,e
        out     (c),a
        ld      b,d
        in      a,(c)
        cp      $ff
        jr      nz,l2c6a
        ld      a,$fe
        ld      b,e
        out     (c),a
        ld      b,d
        in      a,(c)
        cp      $7e
        jr      nz,l2c6a
        ld      a,$fd
        ld      b,e
        out     (c),a
        ld      b,d
        in      a,(c)
        cp      $bd
        jr      nz,l2c6a
        ld      a,$fb
        ld      b,e
        out     (c),a
        ld      b,d
        in      a,(c)
        cp      $db
        jr      nz,l2c6a
        ld      a,$f7
        ld      b,e
        out     (c),a
        ld      b,d
        in      a,(c)
        cp      $e7
        jr      nz,l2c6a
        scf     
        jp      l2429           ; exit with success
.l2c6a  or      a
        jp      l2429           ; exit with failure

; ULA sound test part 2

.l2c6e  call    l271a           ; clear screen
        ld      a,$02
        out     ($fe),a         ; red border
        ld      a,$08
        ld      (BORDCR),a
        ld      hl,l32a6
        call    l2703           ; display test message
        ld      hl,$0100
        ld      de,$0a00
        call    l2726           ; make a beep
        ld      bc,$1200
        ld      hl,l2d21
        call    l2703           ; ask if heard sound
        call    l26af           ; get ENTER/SPACE
        call    l2429           ; set success/fail flag
        ret     

; GI Sound Test routine

.l2c99  call    l271a           ; clear screen
        ld      a,$05
        out     ($fe),a         ; cyan border
        ld      hl,l328d
        call    l2703           ; display GI message
        ld      c,$fd           ; make some sounds
        ld      d,$ff
        ld      e,$bf
        ld      hl,l2d0c
.l2caf  ld      a,(hl)
        inc     hl
        bit     7,a
        jr      nz,l2cbf
        ld      b,d
        out     (c),a
        ld      a,(hl)
        inc     hl
        ld      b,e
        out     (c),a
        jr      l2caf
.l2cbf  ld      c,$fd
        ld      d,$ff
        ld      e,$bf
        ld      l,$03
        ld      h,$fe
.l2cc9  ld      b,d
        ld      a,$07
        out     (c),a
        ld      b,e
        out     (c),h
        push    hl
        push    de
        push    bc
        call    l269a           ; pause
        pop     bc
        pop     de
        pop     hl
        scf     
        rl      h
        dec     l
        jr      nz,l2cc9        ; loop back for more
        ld      h,$f8
        ld      b,d
        ld      a,$07
        out     (c),a
        ld      b,e
        out     (c),h
        push    hl
        push    de
        push    bc
        call    l269a           ; pause
        pop     bc
        pop     de
        pop     hl
        ld      h,$ff
        ld      b,d
        ld      a,$07
        out     (c),a
        ld      b,e
        out     (c),h
        ld      bc,$0a00
        ld      hl,l34a8
        call    l2703           ; ask if heard sounds
        call    l26af           ; get ENTER/SPACE
        jp      l2429           ; rotate into flags & exit

.l2d0c  defb    $00,$40,$01,$00
        defb    $02,$80
        defb    $03,$00,$04,$00
        defb    $05,$01,$06,$1f
        defb    $08,$0f,$09,$0f
        defb    $0a,$0f,$80

.l2d21  defm    "Press [ENTER] if you heard the  tone, else press [SPACE]"&$ff
.l2d5a  defm    "colour test failed"&$0d&$ff
.l2d6e  defm    "ULA sound test failed"&$0d&$ff
.l2d85  defm    "Symshft/A key test failed"&$0d&$ff
.l2da0  defm    "ULA test failed"&$0d&$ff
.l2db1  defm    "RS232 test failed"&$0d&$ff
.l2dc4  defm    "GI sound test failed"&$0d&$ff
.l2dda  defm    "All-RAM page test failed"&$0d&$ff
.l2df4  defm    "Joystick test failed"&$0d&$ff
.l2e0a  defm    "IC 7 checksum failed"&$0d&$ff
.l2e20  defm    "IC 8 checksum failed"&$0d&$ff
.l2e36  defm    "Disk tests failed"&$0d&$ff
.l2e49  defm    "Second Screen test failed"&$0d&$ff
.l2e64  defm    "Cassette Output failed"&$0d&$ff
.l2e7c  defm    "Cassette Input failed"&$0d&$ff
.l2e93  defm    "Printer BUSY test failed"&$0d&$ff
.l2ead  defm    "Printer DATA test failed"&$0d&$ff

; Table of test failure message addresses

.l2ec7  defw    l2e7c
        defw    l2e36
        defw    l2ead
        defw    l2e93
        defw    l2e49
        defw    l2e64
        defw    l2df4
        defw    l2d6e
        defw    l2db1
        defw    l2da0
        defw    l2d85
        defw    l2dc4
        defw    l2dda
        defw    l2e20
        defw    l2e0a
        defw    l2d5a

; Test program messages

.l2ee7  defm    $16&$5&$5&$12&$1
        defm    "ROM TEST FAILED"&$12&$0&$0d&$ff
        defm    $16&$5&$5&$12&$1
        defm    "RAM TEST FAILED"&$12&$0&$0d&$ff
.l2f17  defm    $16&$2&$0
        defm    " >>> SPECTRUM KEYBOARD TEST <<< "&$0d&$0d
        defm    $14&$1
        defm    "                                 "
        defm    "t i 1 2 3 4 5 6 7 8 9 0   b"
        defm    "                                     "
        defm    "d g   Q W E R T Y U I O P  "
        defm    "                                     "
        defm    "e e   A S D F G H J K L   e"
        defm    "                                     "
        defm    "c c   Z X C V B N M .     c"
        defm    "                                     "
        defm    "s ; "&$22&" < >     S     ^ v , s"
        defm    "                                     "
        defm    "                               "
        defm    $14&$0&$16&$14&$5&$12&$1
        defm    "TEST ALL THE KEYS"&$12&$0&$ff
.l30d9  defm    $16&$5&$5&$14&$1
        defm    " SYM SHFT/A  TEST "
        defm    $16&$0a&$5
        defm    "   Press "
        defm    $16&$0c&$5&$12&$1
        defm    "SYM SHFT/A"
        defm    $12&$0&$16&$0e&$5
        defm    " for 1 sec "&$14&$0&$ff
.l311e  defm    $16&$9&$0&$14&$1
        defm    " SPECTRUM +3 test program V 4.0  AMSTRAD 1987..."
        defm    "  by RG/CL/VO "&$0d
        defm    "        Check TV tuning         "
        defm    "connect the loopback connector! "&$0d
        defm    "Press [ENTER] if colour is OK,  "
        defm    "press [SPACE] if it is not      "&$0d
        defm    $12&$1&$0d
        defm    "TAKE CARE - THESE TESTS CORRUPT DISKS, AND REQUIRE"
        defm    " FACTORY TEST EQUIPMENT! "
        defm    $14&$0
        defm    "YOU HAVE BEEN WARNED!"
        defm    $14&$0&$12&$0&$ff
.l324e  defm    $16&$5&$5&$14&$1
        defm    " ULA TEST "
        defm    $14&$0&$0d&$ff
.l3261  defm    $16&$5&$7&$14&$1
        defm    "RAM DATA TESTS"&$16&$0a&$5&".. STARTING NOW .."
        defm    $14&$0&$0d&$ff
.l328d  defm    $16&$5&$5&$14&$1
        defm    " GI SOUND TEST "
        defm    $0d&$0d&$14&$0&$ff
.l32a6  defm    $16&$5&$5&$14&$1
        defm    " ULA SOUND TEST "
        defm    $0d&$0d&$14&$0&$ff&$0
.l32c1  defm    $16&$5&$5&$14&$1
        defm    "CASSETTE OUTPUT TEST"
        defm    $14&$0&$0d&$0d&$ff&$0
.l32e0  defm    $16&$0&$0a&$14&$1
        defm    "JOYSTICK TEST"
        defm    $14&$0&$0d&$0d
        defm    "Move both joysticks and press"&$0d
        defm    "the FIRE buttons until the"&$0d
        defm    "letters below are wiped out"&$0d&$0d
        defm    "Press [SPACE] to give up."&$0d&$0d&$0d&$0d
        defm    "+-----J1-----------J2------+"&$0d
        defm    "!                          !"&$0d
        defm    "!     UP           UP      !"&$0d
        defm    "!                          !"&$0d
        defm    "!  LF FI RI     LF FI RI   !"&$0d
        defm    "!                          !"&$0d
        defm    "!     DN           DN      !"&$0d
        defm    "!                          !"&$0d
        defm    "+--------------------------+"&$0d&$ff
.l346f  defm    $16&$5&$5
        defm    " ALL TESTS PASSED "&$0d&$ff
.l3486  defm    $16&$0&$0&$12&$1
        defm    " TEST FAILED, because:- "&$0d&$12&$0&$0d&$ff
.l34a8  defm    "press [ENTER] if you heard four sounds, "
        defm    "else press [SPACE].     "&$ff


; Subroutine to make a beep. This is a copy of the BEEPER
; subroutine at 03B5 in ROM3

.l34e9  di
        push    ix
        ld      a,l
        srl     l
        srl     l
        cpl     
        and     $03
        ld      c,a
        ld      b,$00
        ld      ix,l3507
        add     ix,bc
        ld      a,(BORDCR)
        and     $38
        rrca    
        rrca    
        rrca    
        or      $08
.l3507  nop     
.l3508  nop     
        nop     
.l350a  inc     b
        inc     c
.l350c  dec     c
        jr      nz,l350c
        ld      c,$3f
        dec     b
        jp      nz,l350c
        xor     $10
        out     ($fe),a
        ld      b,h
        ld      c,a
        bit     4,a
        jr      nz,l3528
        ld      a,d
.l3520  or      e
        jr      z,l352c
        ld      a,c
        ld      c,l
        dec     de
.l3526  jp      (ix)
.l3528  ld      c,l
        inc     c
.l352a  jp      (ix)
.l352c  ei      
        pop     ix
        ret     
        
; Table of screen positions for joystick test

.l3530  defb    $11,$06,$0f,$06
        defb    $13,$06,$11,$09
        defb    $11,$03,$11,$10
        defb    $11,$16,$13,$13
        defb    $0f,$13,$11,$13
        
; These bits don't seem to be used

.l3544  defm    "FIUPDNRILFLFRIDNUPFI"

; Integral disk test

.l3558  ld      hl,l3566        ; address of routine to execute in RAM
        ld      de,$5f00
        ld      bc,$003d
        ldir                    ; copy it 
        jp      $5f00           ; jump into it

; Integral disk test routine to execute in RAM

.l3566  ld      a,$04
        ld      bc,$1ffd
        out     (c),a           ; switch in ROM 2
        ld      hl,$245c        ; copy routine from ROM 2 to RAM
        ld      de,$6000
        ld      bc,$0c00
        ldir    
        ld      a,$00
        ld      bc,$1ffd
        out     (c),a           ; switch in ROM 0
        ld      (BANK678),a
        ld      a,$10
        ld      b,$7f
        out     (c),a           ; switch in ROM 1
        ld      (BANKM),a
        ei      
        push    ix
        call    $6000           ; call ROM 2 routine in RAM
        pop     ix
        push    af              ; save success/fail flag
        ld      a,$00
        ld      bc,$7ffd
        out     (c),a           ; switch in ROM 0
        ld      (BANKM),a
        pop     af
        call    l2429           ; set success/fail in flags
        ret                     ; done

; Tape test

.l35a3  call    l271a           ; cls
        ld      hl,l362b
        call    l2703           ; display test message
        call    l3612           ; short pause
        di      
        ld      hl,$58e1        ; set up attribs
        ld      de,$0006
        ld      b,e
        ld      a,d
.l35b8  ld      (hl),a
        add     hl,de
        djnz    l35b8
.l35bc  ld      hl,$0000        ; tape testing
        ld      de,$1000
        ld      c,$fe
        ld      b,$7f
        in      a,(c)
        bit     0,a
        jp      z,l361f         ; move on if SPACE pressed
        ld      bc,$bffe
        in      a,(c)
        bit     0,a
        jp      z,l3625         ; move on if ENTER pressed
.l35d7  dec     de
        ld      a,d
        or      e
        jr      z,l35e7
        in      a,($fe)
        and     $40
        cp      c
        jr      z,l35d7
        inc     hl
        ld      c,a
        jr      l35d7
.l35e7  rl      l
        rl      h
        rl      l
        rl      h
        rl      l
        rl      h
        ld      l,h
        ld      a,$20
        cp      h
        jr      nc,l35fb
        ld      l,$20
.l35fb  xor     a
        ld      h,a
        ld      de,$591f
        ld      b,$20
        ld      a,$48
        ei      
        halt    
        di      
.l3607  ld      (de),a
        dec     de
        djnz    l3607
        inc     de
        add     hl,de
        ld      a,$68
        ld      (hl),a
        jr      l35bc

; Subroutine to pause for a short while

.l3612  ei      
        ld      b,$19
.l3615  halt    
        djnz    l3615           ; pause
        ld      hl,FLAGS
        res     5,(hl)          ; clear "new key" flag
        scf     
        ret     

; Set "tape test fail" flag

.l361f  and     a
        call    l2429
        jr      l3612

; Set "tape test succeed" flag

.l3625  scf     
        call    l2429
        jr      l3612

.l362b  defm    $16&$0&$0
        defm    "Insert test tape, press PLAY,"&$0d
        defm    "and adjust azimuth screw for"&$0d
        defm    "maximum reading on screen."&$0d
        defm    "Press [ENTER] if successful,"&$0d
        defm    "press [SPACE] if failed"&$0d&$ff


; *********** END OF SELF-TEST PROGRAM SECTION ***********



.l36ba  ld      (hl),h
        rst     $18
        sbc     a,$55
        djnz    $3712               ; (82)
        ret     
        sbc     a,(hl)
        sbc     a,(hl)
        cp      l
        ld      h,d
        push    bc
        ret     nz
        ld      d,l
        jp      nz,$1044
        rla     

.l36cc  cp      $5f
        sub     b
        cp      $d1
        defb    $dd
        push    de

.l36d3  rla     
        sub     b
        rst     $30
        rst     $18
        rst     $18
        call    nc,$d9c7
        sbc     a,$c3
        cp      l
        di      
        call    c,$d659
        ld      d,(hl)
        djnz    $36fc               ; (23)
        ld      h,h

.l36e6  ld      b,a
        ld      e,a
        sub     b

.l36e9  ld      h,b
        rst     $18
        ld      b,b

.l36ec  ld      b,e
        sub     a
        djnz    $36ec               ; (-4)

.l36f0  ld      d,c
        rst     $0
        ld      b,e
        ld      e,a
        sbc     a,$bd
        ld      h,(hl)
        exx     

.l36f8  ld      e,e
        sub     b
        rla     
        jp      po,$54d5
        djnz    $36f8               ; (-8)
        ld      d,l
        ld      b,d
        ld      b,d

.l3703  ld      e,c
        sbc     a,$57
        rla     
        sub     b
        ld      a,a

.l3709  ld      e,h
        ld      e,h
        exx     
        add     a,$55
        jp      nz,$5190
        sbc     a,$bd
        call    po,$d5d8
        sub     b
        ld      (hl),h
        ld      e,a

.l3719  jp      nz,$595b
        ld      e,(hl)
        rst     $10
        sub     b
        ld      a,l
        ld      e,a
        jp      nc,$64bd
        ld      e,b
        pop     de

.l3726  ld      e,(hl)
        in      a,($c3)
        djnz    $36ef               ; (-60)
        ld      e,a
        ld      e,$1e
        cp      l
        ld      h,h
        ret     c
        push    de
        djnz    $3726               ; (-14)
        ld      b,d
        push    de
        ld      b,a
        push    de
        ld      b,d
        ret     
        sub     b
        call    po,$c0d1
        sub     b
        defb    $18
        defb    88
        exx     
        out     ($19),a
        cp      l
        jp      po,$dc5f

.l3748  ld      d,c
        sbc     a,$54
        inc     e
        sub     b
        ld      h,d
        exx     
        out     ($58),a
        ld      d,c
        jp      nz,$9054
        ld      d,l
        ld      b,h
        sub     b
        pop     de
        ld      e,h
        cp      l
        ld      d,c
        ld      e,(hl)
        ld      d,h
        sub     b
        ld      sp,hl
        add     a,$df
        ld      b,d
        djnz    $36fd               ; (-104)
        rst     $0
        ret     c
        ld      c,c
        djnz    $3748               ; (-34)
        ld      e,a

.l376b  call    nz,$bd19
        ld      h,a
        ld      b,d
        exx     
        ld      b,h
        call    nz,$5ed5
        sub     b
        rst     $18
        ld      e,(hl)
        sub     b
        ld      h,b
        di      
        ld      h,a
        sub     b
        adc     a,b
        dec     b
        add     a,c
        ld      (bc),a
        ld      b,e
        inc     e
        djnz    $37ca               ; (69)
        ld      b,e
        exx     
        ld      e,(hl)
        cp      l
        defb    $fd
        ex      af,af'
        nop     
        sub     b
        ld      d,c

.l378e  sbc     a,$d4
        djnz    $378e               ; (-4)
        ex      af,af'
        nop     
        djnz    $37db               ; (69)
        ld      e,(hl)
        call    nc,$c255
        djnz    $378f               ; (-13)
        ld      h,b
        sbc     a,a
        defb    $fd
        sbc     a,e
        cp      l
        cp      l
        jp      $c490
        rst     $18
        ld      e,$9e
        cp      l
        ld      h,h
        ret     c
        ld      d,l
        djnz    $376b               ; (-67)

        defs    $1c

.l37ca  defs    $11

.l37db  defs    $59

.l3834  defs    $05cc      
        
        ld      (OLDHL),hl
        push    af
        pop     hl

.l3e05  ld      (OLDAF),hl
        ex      (sp),hl
        ld      c,(hl)
        inc     hl
        ld      b,(hl)
        inc     hl
        ex      (sp),hl
        push    bc
        pop     hl
        ld      a,(BANK678)
        ld      bc,$1ffd
        res     2,a
        di      
        ld      (BANK678),a
        out     (c),a
        ei      
        ld      bc,$3e2d
        push    bc
        push    hl
        ld      hl,(OLDAF)
        push    hl
        pop     af
        ld      hl,(OLDHL)
        ret     
        push    bc
        push    af
        ld      a,(BANK678)
        ld      bc,$1ffd
        set     2,a
        di      
        ld      (BANK678),a
        out     (c),a
        ei      
        pop     af
        pop     bc
        ret     

        defs    $3f

; Subroutine to call a subroutine in ROM 1
; The address to call is stored inline after the call to this routine

.l3e80  ld      (OLDHL),hl      ; save HL in OLDHL
        ld      (OLDBC),bc      ; save BC in OLDBC
        push    af              
        pop     hl
        ld      (OLDAF),hl      ; save AF in OLDAF
        ex      (sp),hl         ; HL=return address
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=inline address for ROM 1
        inc     hl
        ex      (sp),hl         ; restore proper return address
        push    bc                      
        pop     hl              ; HL=address in ROM 1 to call
        ld      a,(BANKM)
        or      $10
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page in ROM 1

; The rest of the routine continues at $3ea2 in ROM 1
; The following is a continuation of a mirrored routine in ROM 1 for
; returning to this ROM

.l3ea2  ei      
        ld      bc,$3eb5
        push    bc              ; stack return add to swap back ROMS
        push    hl              ; stack address of routine to call
        ld      hl,(OLDAF)
        push    hl
        pop     af              ; restore AF
        ld      bc,(OLDBC)      ; restore BC
        ld      hl,(OLDHL)      ; restore HL
        ret                     ; exit to routine in this ROM

; This is the routine which returns control to the calling routine in ROM 1

.l3eb5  push    af              ; save AF & BC
        push    bc
        ld      a,(BANKM)
        or      $10
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page back ROM 1

; The rest of the routine continues at $3ec5 in ROM 1
; The following is a continuation of a mirrored routine in ROM 1 for
; returning to this ROM

.l3ec5  ei      
        pop     bc
        pop     af
        ret     

        defs    $37

; Subroutine to call a subroutine in ROM 2, with inline address
; This routine is not used in this ROM, but is a duplicate of a
; routine in ROM 1, which takes over during ROM switching to ROM 2
; via this ROM, and back again at the end.

.l3f00  ld      (OLDHL),hl      ; save HL, BC and AF
        ld      (OLDBC),bc
        push    af
        pop     hl
        ld      (OLDAF),hl
        ex      (sp),hl
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=inline address
        inc     hl
        ex      (sp),hl         ; restack updated return address
        push    bc
        pop     hl
        ld      a,(BANKM)
        and     $ef
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; switch in ROM 0
        ld      a,(BANK678)
        or      $04
        ld      (BANK678),a
        ld      bc,$1ffd
        out     (c),a           ; switch in ROM 2
        ei      
        ld      bc,l3f42
        push    bc              ; stack routine address to return to ROM 1
        push    hl              ; stack routine address to call in ROM 2
        ld      hl,(OLDAF)      ; restore registers
        push    hl
        pop     af
        ld      bc,(OLDBC)
        ld      hl,(OLDHL)
        ret                     ; exit to routine

; This part of the routine then returns control to ROM 1

.l3f42  push    bc              ; save registers
        push    af
        ld      a,(BANK678)
        and     $fb
        di      
        ld      (BANK678),a
        ld      bc,$1ffd
        out     (c),a           ; page in ROM 0
        ld      a,(BANKM)
        or      $10
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page in ROM 1
        ei      
        pop     af              ; restore registers
        pop     bc
        ret                     ; done!

        defs    $8d

; This routine is called from ROM 2 to display error messages, and
; optionally get a response

.l3ff0  jp      l2187		; go to the routine


        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        rst     $38
        adc     a,e

