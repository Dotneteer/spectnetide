; **************************************************
; *** SPECTRUM +3 ROM 0 DISASSEMBLY (SYNTAX ROM) ***
; **************************************************

; The Spectrum ROMs are copyright Amstrad, who have kindly given permission
; to reverse engineer and publish Spectrum ROM disassemblies.


; =====
; NOTES
; =====

; ------------
; Release Date
; ------------
; 23rd January 2017

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

        module  rom1

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

;        include "p3dos.def"

; +3DOS routine addresses

        defc    DOS_INITIALISE=$0100
        defc    DOS_VERSION=$0103
        defc    DOS_OPEN=$0106
        defc    DOS_CLOSE=$0109
        defc    DOS_ABANDON=$010c
        defc    DOS_REF_HEAD=$010f
        defc    DOS_READ=$0112
        defc    DOS_WRITE=$0115
        defc    DOS_BYTE_READ=$0118
        defc    DOS_BYTE_WRITE=$011b
        defc    DOS_CATALOG=$011e
        defc    DOS_FREE_SPACE=$0121
        defc    DOS_DELETE=$0124
        defc    DOS_RENAME=$0127
        defc    DOS_BOOT=$012a
        defc    DOS_SET_DRIVE=$012d
        defc    DOS_SET_USER=$0130
        defc    DOS_GET_POSITION=$0133
        defc    DOS_SET_POSITION=$0136
        defc    DOS_GET_EOF=$0139
        defc    DOS_GET_1346=$013c
        defc    DOS_SET_1346=$013f
        defc    DOS_FLUSH=$0142
        defc    DOS_SET_ACCESS=$0145
        defc    DOS_SET_ATTRIBUTES=$0148
        defc    DOS_OPEN_DRIVE=$014b
        defc    DOS_SET_MESSAGE=$014e
        defc    DOS_REF_XDPB=$0151
        defc    DOS_MAP_B=$0154

        defc    DD_INTERFACE=$0157
        defc    DD_INIT=$015a
        defc    DD_SETUP=$015d
        defc    DD_SET_RETRY=$0160
        defc    DD_READ_SECTOR=$0163
        defc    DD_WRITE_SECTOR=$0166
        defc    DD_CHECK_SECTOR=$0169
        defc    DD_FORMAT=$016c
        defc    DD_READ_ID=$016f
        defc    DD_TEST_UNSUITABLE=$0172
        defc    DD_LOGIN=$0175
        defc    DD_SEL_FORMAT=$0178
        defc    DD_ASK_1=$017b
        defc    DD_DRIVE_STATUS=$017e
        defc    DD_EQUIPMENT=$0181
        defc    DD_ENCODE=$0184
        defc    DD_L_XDPB=$0187
        defc    DD_L_DPB=$018a
        defc    DD_L_SEEK=$018d
        defc    DD_L_READ=$0190
        defc    DD_L_WRITE=$0193
        defc    DD_L_ON_MOTOR=$0196
        defc    DD_L_T_OFF_MOTOR=$0199
        defc    DD_L_OFF_MOTOR=$019c

; +3DOS Error codes

defgroup {	rc_ready,
		rc_wp,
		rc_seek,
		rc_crc,
		rc_nodata,
		rc_mark,
		rc_unrecog,
		rc_unknown,
		rc_diskchg,
		rc_unsuit,
		rc_badname=20,
		rc_badparam,
		rc_nodrive,
		rc_nofile,
		rc_exists,
		rc_eof,
		rc_diskfull,
		rc_dirfull,
		rc_ro,
		rc_number,
		rc_denied,
		rc_norename,
		rc_extent,
		rc_uncached,
		rc_toobig,
		rc_notboot,
		rc_inuse
	}

;**************************************************

;	include "fpcalc.def"

; The floating-point calculator commands

        defgroup
        {
        jump_true, exchange, delete, subtract, multiply, division,
        to_power, or, no_and_no, no_l_eql, no_gr_eq, nos_neql, no_grtr,
        no_less, nos_eql, addition, str_and_no, str_l_eql, str_gr_eq,
        strs_neql, str_grtr, str_less, strs_eql, strs_add, val_str, usr_str,
        read_in, negate, code, val, len, sin, cos, tan, asn, acs, atn,
        ln, exp, int, sqr, sgn, abs, peek, in, usr_no, str_str, chr_str,
        not, duplicate, n_mod_m, jump, stk_data, dec_jr_nz, less_0,
        greater_0, end_calc, get_argt, truncate, fp_calc_2, e_to_fp,
        re_stack
        }

        defc    series_06=$86
        defc    series_08=$88
        defc    series_0c=$8c
        defc    stk_zero=$a0
        defc    stk_one=$a1
        defc    stk_half=$a2
        defc    stk_pi_2=$a3
        defc    stk_ten=$a4
        defc    st_mem_0=$c0
        defc    st_mem_1=$c1
        defc    st_mem_2=$c2
        defc    st_mem_3=$c3
        defc    st_mem_4=$c4
        defc    st_mem_5=$c5
        defc    get_mem_0=$e0
        defc    get_mem_1=$e1
        defc    get_mem_2=$e2
        defc    get_mem_3=$e3
        defc    get_mem_4=$e4
        defc    get_mem_5=$e5

;**************************************************

        org     $0000

; ROM 1 Header

.l0000  defm    "Syntax"
        defs    2

; RST $08 - The "Error" restart

.l0008  jp      l2ada           ; jump to error handler

        defs    5

; RST $10 - The "Print a character restart"

.l0010  rst     $28
        defw    $0010           ; call RST $10 in ROM 3
        ret

        defs    4

; RST $18 - The "Collect character" restart

.l0018  rst     $28
        defw    $0018           ; call RST $18 in ROM 3
        ret

        defs    4

; RST $20 - The "Collect next character" restart

.l0020  rst     $28
        defw    $0020           ; call RST $20 in ROM 3
        ret

        defs    4

; RST $28 : Call a routine in ROM 3, then return to ROM 1
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
        jp      l00aa           ; jump on

        nop

; The maskable interrupt routine

.l0038  push    af              ; save registers
        push    hl
        ld      hl,(FRAMES)     ; increment FRAMES
        inc     hl
        ld      (FRAMES),hl
        ld      a,h
        or      l
        jr      nz,l0048
        inc     (iy+$40)
.l0048  push    bc
        push    de
        call    l0176           ; scan keyboard
        call    l0074           ; call disk motor timeout routine
        pop     de              ; restore registers
        pop     bc
        pop     hl
        pop     af
        ei                      ; re-enable interrupts & exit
        ret     
        
        defs    $10

; The NMI routine

.l0066  push    af              ; save registers
        push    hl
        ld      hl,(NMIADD)     ; get routine address
        ld      a,h
        or      l
        jr      z,l0070         
        jp      (hl)            ; execute if non-zero address
.l0070  pop     hl              ; restore registers & exit
        pop     af
        retn

; The disk motor timeout routine

.l0074  ld      bc,$7ffd
        ld      a,(BANKM)
        or      $07
        out     (c),a           ; page in page 7
        ld      a,(timeout)
        or      a
        jr      z,l00a1         ; move on if already off
        ld      a,(FRAMES)
        bit     0,a
        jr      nz,l00a1        ; only decrement counter every other frame
        ld      a,(timeout)
        dec     a               ; decrement timeout counter
        ld      (timeout),a
        jr      nz,l00a1        ; move on if non-zero
        ld      bc,$1ffd
        ld      a,(BANK678)
        and     $f7
        ld      (BANK678),a
        out     (c),a           ; turn motor off
.l00a1  ld      bc,$7ffd
        ld      a,(BANKM)
        out     (c),a           ; page in last memory configuration
        ret     

; Continuation of RST 28: call a routine in ROM 3

.l00aa  ld      (TARGET),hl     ; save ROM 3 address in TARGET
        ld      hl,REGNUOY
        ex      (sp),hl         ; stack REGNUOY address beneath TOS
        push    hl
        ld      hl,(TARGET)     ; get HL=target address in ROM 3
        ex      (sp),hl         ; restore HL & save target address on stack
        push    af              ; stack AF & BC
        push    bc
        di                      ; disable interrupts
        jp      STOO            ; jump to STOO - pages in ROM 3, returns to
                                ; target routine which returns to REGNUOY
                                ; where ROM 1 is paged back and jump made
                                ; back to RETADDR


; These are copies of the key tables from ROM 3


; The L-mode keytable with CAPS-SHIFT

.l00bc  defm    "BHY65TGV"
        defm    "NJU74RFC"
        defm    "MKI83EDX"
        defm    $0e&"LO92WSZ"
        defm    " "&$0d&"P01QA"

; The extended-mode keytable (unshifted letters)

.l00e3  defb    $e3,$c4,$e0,$e4
        defb    $b4,$bc,$bd,$bb
        defb    $af,$b0,$b1,$c0
        defb    $a7,$a6,$be,$ad
        defb    $b2,$ba,$e5,$a5
        defb    $c2,$e1,$b3,$b9
        defb    $c1,$b8
        
; The extended mode keytable (shifted letters)

.l00fd  defb    $7e,$dc,$da,$5c
        defb    $b7,$7b,$7d,$d8
        defb    $bf,$ae,$aa,$ab
        defb    $dd,$de,$df,$7f
        defb    $b5,$d6,$7c,$d5
        defb    $5d,$db,$b6,$d9
        defb    $5b,$d7
        
; The control code keytable (CAPS-SHIFTed digits)

.l0117  defb    $0c,$07,$06,$04
        defb    $05,$08,$0a,$0b
        defb    $09,$0f
        
; The symbol code keytable (letters with symbol shift)

.l0121  defb    $e2,$2a,$3f,$cd
        defb    $c8,$cc,$cb,$5e
        defb    $ac,$2d,$2b,$3d
        defb    $2e,$2c,$3b,$22
        defb    $c7,$3c,$c3,$3e
        defb    $c5,$2f,$c9,$60
        defb    $c6,$3a

; The extended mode keytable (SYM-SHIFTed digits)

.l013b  defb    $d0,$ce,$a8,$ca
        defb    $d3,$d4,$d1,$d2
        defb    $a9,$cf


; This is a copy of the "keyboard scanning" subroutine from
; $028e in ROM 3

.l0145  ld      l,$2f
        ld      de,$ffff
        ld      bc,$fefe
.l014d  in      a,(c)
        cpl     
        and     $1f
        jr      z,l0162
        ld      h,a
        ld      a,l
.l0156  inc     d
        ret     nz
.l0158  sub     $08
        srl     h
        jr      nc,l0158
        ld      d,e
        ld      e,a
        jr      nz,l0156
.l0162  dec     l
        rlc     b
        jr      c,l014d
        ld      a,d
        inc     a
        ret     z
        cp      $28
        ret     z
        cp      $19
        ret     z
        ld      a,e
        ld      e,d
        ld      d,a
        cp      $18
        ret

; This is a copy of the "keyboard" subroutines from $02bf in ROM 3

.l0176  call    l0145
        ret     nz
        ld      hl,KSTATE
.l017d  bit     7,(hl)
        jr      nz,l0188
        inc     hl
        dec     (hl)
        dec     hl
        jr      nz,l0188
        ld      (hl),$ff
.l0188  ld      a,l
        ld      hl,KSTATE+$04
        cp      l
        jr      nz,l017d
        call    l01d5
        ret     nc
        ld      hl,KSTATE
        cp      (hl)
        jr      z,l01c7
        ex      de,hl
        ld      hl,KSTATE+$04
        cp      (hl)
        jr      z,l01c7
        bit     7,(hl)
        jr      nz,l01a8
        ex      de,hl
        bit     7,(hl)
        ret     z
.l01a8  ld      e,a
        ld      (hl),a
        inc     hl
        ld      (hl),$05
        inc     hl
        ld      a,(REPDEL)
        ld      (hl),a
        inc     hl
        ld      c,(iy+$07)
        ld      d,(iy+$01)
        push    hl
        call    l01ea
        pop     hl
        ld      (hl),a
.l01bf  ld      (LAST_K),a
        set     5,(iy+$01)
        ret     
.l01c7  inc     hl
        ld      (hl),$05
        inc     hl
        dec     (hl)
        ret     nz
        ld      a,(REPPER)
        ld      (hl),a
        inc     hl
        ld      a,(hl)
        jr      l01bf

; This is a copy of the "K-Test" subroutine from $031e in ROM 3

.l01d5  ld      b,d
        ld      d,$00
        ld      a,e
        cp      $27
        ret     nc
        cp      $18
        jr      nz,l01e3
        bit     7,b
        ret     nz
.l01e3  ld      hl,l00bc        ; the main keytable
        add     hl,de
        ld      a,(hl)
        scf     
        ret

; This is a copy of the "Keyboard decoding" subroutine from $0333 in
; ROM 3

.l01ea  ld      a,e
        cp      $3a
        jr      c,l021e
        dec     c
        jp      m,l0206
        jr      z,l01f8
        add     a,$4f
        ret     
.l01f8  ld      hl,l00e3-'A'
        inc     b
        jr      z,l0201
        ld      hl,l00fd-'A'
.l0201  ld      d,$00
        add     hl,de
        ld      a,(hl)
        ret     
.l0206  ld      hl,l0121-'A'
        bit     0,b
        jr      z,l0201
        bit     3,d
        jr      z,l021b
        bit     3,(iy+$30)
        ret     nz
        inc     b
        ret     nz
        add     a,$20
        ret     
.l021b  add     a,$a5
        ret     
.l021e  cp      $30
        ret     c
        dec     c
        jp      m,l0254
        jr      nz,l0240
        ld      hl,l013b-'0'
        bit     5,b
        jr      z,l0201
        cp      $38
        jr      nc,l0239
        sub     $20
        inc     b
        ret     z
        add     a,$08
        ret     
.l0239  sub     $36
        inc     b
        ret     z
        add     a,$fe
        ret     
.l0240  ld      hl,l0117-'0'
        cp      $39
        jr      z,l0201
        cp      $30
        jr      z,l0201
        and     $07
        add     a,$80
        inc     b
        ret     z
        xor     $0f
        ret     
.l0254  inc     b
        ret     z
        bit     5,b
        ld      hl,l0117-'0'
        jr      nz,l0201
        sub     $10
        cp      $22
        jr      z,l0269
        cp      $20
        ret     nz
        ld      a,$5f
        ret     
.l0269  ld      a,$40
        ret     


; The FORMAT command

.l026c  rst     $28
        defw    $0018           ; get character after FORMAT
.l026f  cp      $e0
        jp      z,l03e3         ; move on if LPRINT
        cp      $ca
        jr      nz,l027e        ; move on if not LINE
        rst     $28
        defw    $0020           ; get next character
        jp      l1e05           ; and move on for FORMAT LINE
.l027e  rst     $28
        defw    $1c8c           ; get a string expression
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; get string from stack
        ld      a,c
        dec     a
        dec     a
        or      b
        jr      z,l0291         ; move on if length is 2
.l028d  call    l2ada
        defb    $4e             ; else error "Invalid drive"
.l0291  inc     de
        ld      a,(de)          ; check 2nd char
        dec     de
        cp      ':'
        jr      z,l029c         
        call    l2ada
        defb    $4e             ; error "Invalid drive" if not colon
.l029c  ld      a,(de)
        and     $df             ; get capitalised drive letter
        cp      'A'
        jr      z,l02ab         ; move on if A:
        cp      'B'
        jr      z,l02ab         ; or B:
        call    l2ada
        defb    $4e             ; else error "Invalid drive"
.l02ab  call    l2b89           ; page in DOS workspace
        sub     'A'
        push    af              ; save unit number to format
        ld      hl,FLAGS3
        bit     4,(hl)
        jr      nz,l02bf        ; move on if disk interface present
        call    l2b64           ; page in normal memory
        call    l2ada
        defb    $4c             ; else error "Format not supported on +2A"
.l02bf  pop     af
        or      a
        jr      z,l02d3         ; move on for unit 0
        push    af
        ld      hl,FLAGS3
        bit     5,(hl)
        jr      nz,l02d2        ; move on if drive B: present
        call    l2b64           ; page in normal memory
        call    l2ada
        defb    $4b             ; else error "Drive B: not present"
.l02d2  pop     af              ; get unit
.l02d3  push    af
        ld      c,a
        push    bc
        add     a,'A'
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_XDPB    ; point IX at XDPB
        call    l32ee           ; restore TSTACK
        jr      c,l02ec         ; move on if no error
        call    l2b64           ; page in DOS memory
        call    l0e9a           ; cause DOS error
        defb    $ff             
.l02ec  pop     bc
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_LOGIN        ; login disk
        call    l32ee           ; restore TSTACK
        jr      nc,l0306        ; move on if error
        or      a
        jr      nz,l0315        ; move on if disk isn't +3 format
        call    l0381           ; ask if wish to abandon
        jr      nz,l0315        ; move on if not
        call    l2b64           ; page in normal memory
        ret                     ; exit
.l0306  cp      $05
        jr      z,l0315         ; move on if error was "missing address mark"
        cp      $09
        jr      z,l0315         ; or "unsuitable media"
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause DOS error
        defb    $ff     
.l0315  pop     af              ; get unit number
        push    af
        add     a,'A'
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_XDPB    ; point IX to XDPB
        call    l32ee           ; restore TSTACK
        jr      c,l032d
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l032d  xor     a                
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_SEL_FORMAT   ; select +3 format
        call    l32ee           ; restore TSTACK
        jr      c,l0342
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l0342  pop     af
        ld      c,a             ; C=unit number
        xor     a               ; start at track 0
.l0345  ld      d,a
        call    l036f           ; fill format buffer
        ld      e,$e5           ; filler byte
        ld      b,$07           ; page 7
        ld      hl,tmp_buff     ; buffer address
        push    af
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_FORMAT       ; format a track
        call    l32ee           ; restore TSTACK
        jr      c,l0365
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l0365  pop     af
        inc     a               ; increment track
        cp      $28
        jr      nz,l0345        ; back if more to do
        call    l2b64           ; page in normal memory
        ret                     ; done

; Subroutine to fill scratch area with format buffer details

.l036f  ld      b,$09           ; 9 sectors
        ld      hl,tmp_buff+$23 ; end of scratch area
.l0374  ld      (hl),$02        ; 512-byte sectors
        dec     hl
        ld      (hl),b          ; sector number
        dec     hl
        ld      (hl),$00        ; head 0
        dec     hl
        ld      (hl),d          ; track number
        dec     hl
        djnz    l0374
        ret     

; Subroutine to display "disk already formatted message",
; and get a key, exiting with Z set if user wishes to abandon

.l0381  ld      hl,l03a7
.l0384  ld      a,(hl)          ; get next char
        or      a
        jr      z,l038e         ; move on if null
        rst     $28
        defw    $0010           ; output char
.l038b  inc     hl
        jr      l0384           ; loop back
.l038e  res     5,(iy+$01)      ; signal "no key"
.l0392  bit     5,(iy+$01)
        jr      z,l0392         ; wait for key
        ld      a,(LAST_K)      ; get key
        and     $df             ; capitalise
        cp      'A'             ; is it "A"?
        push    af              
        push    hl
        rst     $28
        defw    $0d6e           ; clear lower screen
        pop     hl
        pop     af
        ret                     ; exit with Z set if abandon requested

; Formatting message

.l03a7  defm    "Disk is already formatted."&$0d
        defm    "A to abandon, other key continue"&0

; The FORMAT LPRINT command

.l03e3  rst     $28
        defw    $0020           ; get next char
.l03e6  rst     $28
        defw    $1c8c           ; get string expression
        rst     $28
        defw    $0018           ; get next char
.l03ec  cp      ';'
        call    nz,l10b1        ; check for end-of-statement if not ";"
        jr      nz,l041c        ; move on if not ";"
        rst     $28
        defw    $0020           ; get next char
.l03f6  rst     $28
        defw    $1c8c           ; get string expression
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; get 2nd string from stack
        ld      a,c
        dec     a
        or      b               ; check length
        jr      z,l0407
        jp      l028d           ; "Invalid drive" error if not 1
.l0407  ld      a,(de)
        and     $df             ; capitalise 2nd string character
        ld      hl,FLAGS3       ; prepare to change FLAGS3
        cp      'E'     
        jr      nz,l0415        
.l0411  set     2,(hl)          ; if 2nd string "E", set "expand tokens" flag
        jr      l041c
.l0415  cp      'U'
        jp      nz,l028d        ; if 2nd string not "U", error
        res     2,(hl)          ; if "U", reset "expand tokens" flag
.l041c  rst     $28
        defw    $2bf1           ; get first string from stack
        ld      a,c
        dec     a
        or      b               ; check length
        jr      z,l0427
        jp      l028d           ; "Invalid drive" error if not 1
.l0427  ld      a,(de)
        and     $df             ; capitalise 1st string character
        ld      hl,FLAGS3       ; prepare to change FLAGS3
        cp      'R'
        jr      nz,l0434
        set     3,(hl)          ; if "R", set print to RS232 flag
        ret     
.l0434  cp      'C'
        jr      nz,l043b
        res     3,(hl)          ; if "C", reset print to RS232 flag
        ret     
.l043b  cp      'E'
        jr      nz,l0442
        set     2,(hl)          ; if "E", set "expand tokens" flag
        ret     
.l0442  cp      'U'
        jp      nz,l028d        ; if not "U", error
        res     2,(hl)          ; if "U", reset "expand tokens" flag
        ret     

; The ERASE command
; *BUG* No channel is opened before outputting the "Erase (Y/N)?" message,
;       so this is output to the last used stream.
; *BUG* The lower screen is not cleared if "N" is pressed

.l044a  rst     $28
        defw    $2bf1           ; get string from stack
        ld      a,b
        or      c               ; check length
        jr      nz,l0455        
        call    l2ada
        defb    $2c             ; bad filename error if zero
.l0455  push    bc              ; save addresses
        push    de
        push    de
        pop     hl              ; HL=address of filename
        push    bc
        ld      a,'*'
        cpir    
        pop     bc
        jr      z,l046d         ; move on if * wildcard present
        push    de
        pop     hl
        push    bc
        ld      a,'?'
        cpir    
        pop     bc
        jr      z,l046d         ; move on if ? wildcard present
        jr      l0499           ; move on for a single file
.l046d  ld      hl,l04d5
        call    l04c1           ; output "Erase "
        call    l04ca           ; output filespec
        ld      hl,l04dc
        call    l04c1           ; output "? (Y/N"
.l047c  ld      hl,FLAGS
        res     5,(hl)          ; signal "no key available"
.l0481  bit     5,(hl)
        jr      z,l0481         ; loop until keypress
        res     5,(hl)          ; signal "no key available"
        ld      a,(LAST_K)      ; get key
        and     $df             ; make uppercase
        cp      'N'
        jr      nz,l0493        ; move on if not "N"
        pop     de              ; exit without doing anything
        pop     bc		; (lower screen should have been cleared)
        ret     
.l0493  cp      'Y'
        jr      z,l0499
        jr      l047c           ; loop back for another key if not "Y"
.l0499  rst     $28
        defw    $0d6e           ; clear lower screen
        pop     de
        pop     bc
        ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy filespec into page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        ld      hl,tmp_fspec
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_DELETE      ; delete filespec
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        ret     c               ; exit if ok
        call    l0e9a           ; cause DOS error
        defb    $ff

; Subroutine to output a null-terminated string

.l04c1  ld      a,(hl)          ; get next char
        or      a
        ret     z               ; exit if null
        inc     hl
        rst     $28
        defw    $0010           ; output char
.l04c8  jr      l04c1           ; loop back

; Subroutine to output a filespec at DE, length BC

.l04ca  ld      a,(de)          ; get next char
        rst     $28
        defw    $0010           ; output char
.l04ce  inc     de
        dec     bc
        ld      a,b
        or      c
        jr      nz,l04ca        ; back for more
        ret     

; Erase messages

.l04d5  defm    "Erase "&0
.l04dc  defm    " ? (Y/N)"&0
        
        
; The MOVE command

.l04e5  rst     $28
        defw    $2bf1           ; get 2nd string
        ld      a,b
        or      c               ; check length
        jr      nz,l04f0
        call    l2ada
        defb    $2c             ; bad filename error if zero
.l04f0  ld      a,(de)
        cp      '+'
        jp      z,l0541         ; move on if changing attributes
        cp      '-'
        jp      z,l0541         ; move on if changing attributes
        ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy filename to page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
.l0508  inc     de
        call    l2b64           ; page in normal memory
        push    de              ; save pointer for source filename
        rst     $28
        defw    $2bf1           ; get 1st string
        ld      a,b
        or      c               ; check length
        jr      nz,l0518
        call    l2ada
        defb    $2c             ; bad filename error if zero
.l0518  pop     hl              ; HL=address to place source filename
        push    hl
        ex      de,hl
        call    l3f63           ; copy source filename to page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        call    l2b64           ; page in normal memory
        pop     hl
        ld      de,tmp_fspec
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_RENAME      ; do rename
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        ret     c               ; exit if done ok
        call    l0e9a           ; cause DOS error
        defb    $ff

; Here we use MOVE to alter attributes of a file

.l0541  ld      a,c
        dec     a
        dec     a
        or      b
        jr      z,l054b         ; move on if 2nd string length=2
        call    l2ada
        defb    $47             ; invalid attribute error
.l054b  ld      a,(de)
        ld      b,a             ; B='+' or '-'
        inc     de
        ld      a,(de)
        and     $df             ; A=uppercase attribute
        cp      'P'             ; check attribute letter
        jr      z,l0561         
        cp      'S'
        jr      z,l0561
        cp      'A'
        jr      z,l0561
        call    l2ada
        defb    $47             ; invalid attribute error
.l0561  push    bc              ; save attribute flags
        push    af
        rst     $28
        defw    $2bf1           ; get 1st string
        ld      a,b
        or      c               ; check length
        jr      nz,l056e        
        call    l2ada
        defb    $2c             ; bad filename error if zero
.l056e  ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy to page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        call    l2b64           ; page in normal memory
        ld      de,$0000        ; don't set or clear anything yet
        ld      c,$00           ; attribute byte to set/clear
        pop     af              ; get attribute letter
        cp      'P'
        jr      nz,l058c
        set     2,c             ; bit 2 for P
        jr      l0596
.l058c  cp      'S'
        jr      nz,l0594
        set     1,c             ; bit 1 for S
        jr      l0596
.l0594  set     0,c             ; bit 0 for A
.l0596  pop     af              ; get '+' or '-'
        cp      '+'
        jr      nz,l059e
        ld      d,c             ; if +, we're setting attributes
        jr      l059f
.l059e  ld      e,c             ; if -, we're clearing attributes
.l059f  ld      hl,tmp_fspec
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_ATTRIBUTES ; set the attributes
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        ret     c               ; exit if done ok
        call    l0e9a           ; else cause DOS error
        defb    $ff

; The CAT command
; *BUG* Only one buffer of entries is ever considered (64 entries), as a
;       SUB $40 is used (should be SUB $3f)

.l05b8  ld      hl,FLAGS3
        res     6,(hl)          ; signal "standard catalog"
        rst     $28
        defw    $2070           ; consider stream information
        jr      c,l05dd         ; move on if default stream to be used
        ld      hl,(CH_ADD)
        ld      a,(hl)          ; get next char
        cp      ','
        jr      z,l05da         ; move on if comma to get filespec
        cp      $0d
        jr      z,l062b         ; move on if end-of-line
        cp      ':'             
        jr      z,l062b         ; or if end-of-statement
        cp      $b9
        jr      z,l062b         ; or if EXP
        call    l2ada
        defb    $0b             ; else nonsense in BASIC error
.l05da  rst     $20             ; get next char
        jr      l05f8
.l05dd  ld      a,$02           ; use stream 2
        bit     7,(iy+$01)
        jr      z,l05e8         ; move on if only syntax-checking
        rst     $28
        defw    $1601           ; else open channel to stream
.l05e8  ld      hl,(CH_ADD)
        ld      a,(hl)          ; check next char
        cp      $0d
        jr      z,l062b         ; move on if end-of-line
        cp      ':'
        jr      z,l062b         ; or if end-of-statement
        cp      $b9
        jr      z,l062b         ; or if EXP
.l05f8  rst     $28
        defw    $1c8c           ; get string expression
        rst     $28
        defw    $0018           ; get next char
.l05fe  cp      $b9
        jr      nz,l060a        ; move on if not EXP
        ld      hl,FLAGS3
        set     6,(hl)          ; signal "expanded catalog"
        rst     $28
        defw    $0020           ; get next char
.l060a  call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; get string value from stack
        push    bc              
        push    de
        pop     hl              ; HL=string address
        ld      a,':'           ; check for drive specification
        cpir    
        jr      nz,l0623        ; move on if not found
        dec     hl
        dec     hl
        ld      a,(hl)
        and     $df
        ld      (DEFADD),a      ; else save capitalised drive letter
        jr      l0628           ; move on
.l0623  ld      a,$00
        ld      (DEFADD),a      ; signal "use default drive"
.l0628  pop     bc
        jr      l0645           ; move on
.l062b  rst     $28
        defw    $0018           ; get next char
.l062e  cp      $b9
        jr      nz,l063a        ; move on if not EXP
        ld      hl,FLAGS3
        set     6,(hl)          ; signal "expanded catalog"
        rst     $28
        defw    $0020           ; get next char
.l063a  call    l10b1           ; check for end-of-statement
        ld      bc,$0000        ; filespec length=0
        ld      a,$00
        ld      (DEFADD),a      ; signal "use default drive"
.l0645  ld      a,c
        dec     a
        dec     a
        or      b
        jr      nz,l065c        ; move on unless just 2 chars specified
        inc     de
        ld      a,(de)
        dec     de
        cp      ':'
        jr      nz,l065c        ; move on if not drive specifier
        ld      a,(de)
        and     $df             ; get drive letter capitalised
        cp      'T'
        jr      nz,l065c        
        jp      l34c6           ; move on to catalog tape
.l065c  ld      hl,tmp_fspec
        ex      de,hl
        push    bc
        ld      a,b
        or      c
        jr      z,l0668         ; move on if no filespec
        call    l3f63           ; copy to page 7 (entry 0)
.l0668  pop     bc
        ld      hl,tmp_fspec
        add     hl,bc
        call    l2b89
        ld      (hl),$ff        ; add terminator
        ld      hl,tmp_buff
        ld      de,tmp_buff+1
        ld      bc,$000b
        ld      (hl),$00
        ldir                    ; zero entry 0
.l067f  ld      b,$40           ; 64 entries in buffer
        ld      c,$00           ; C=0 for standard catalog
        ld      hl,FLAGS3
        bit     6,(hl)
        jr      z,l068c         
        ld      c,$01           ; C=1 for expanded catalog (inc system files)
.l068c  ld      de,tmp_buff
        ld      hl,tmp_fspec
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CATALOG     ; get next lot of entries
        call    l32ee           ; restore TSTACK
        jp      nc,$06ab        ; move on if error
        ld      hl,tmp_buff+$0d ; address of first returned entry
        dec     b		; B=# entries found (discard preloaded one)
        ld      a,b
        or      a
        jr      nz,l06b7        ; move on if entries to display
        jp      l07ba           ; move on if catalog finished
.l06ab  cp      $17
        jp      z,l07ba         ; move on if error "file not found"
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause DOS error
        defb    $ff
.l06b7  push    bc              ; save number of entries to do
.l06b8  push    af             
        ld      b,$08           ; 8 bytes in first half of filename
.l06bb  ld      a,(hl)
        and     $7f             ; get byte and mask bit
        call    l2b64           ; page in normal memory
        rst     $28
        defw    $0010           ; output char
.l06c4  call    l2b89           ; page in DOS workspace
        inc     hl
        djnz    l06bb           ; loop back for rest of filename
        call    l2b64           ; page in normal memory
        ld      a,'.'
        rst     $28
        defw    $0010           ; output "."
.l06d2  xor     a
        ld      (RAMERR),a      ; zeroise attributes
        ld      b,$03
.l06d8  call    l2b89           ; page in DOS workspace
        ld      a,(hl)          ; get next byte
        bit     7,a
        jr      z,l06fc         ; move on if bit 7 not set
        push    af
        push    hl
        ld      hl,RAMERR
        ld      a,b
        cp      $03
        jr      nz,l06ee
        set     3,(hl)          ; set bit 3 if first extension byte
        jr      l06f8
.l06ee  cp      $02
        jr      nz,l06f6
        set     2,(hl)          ; set bit 2 if second extension byte
        jr      l06f8
.l06f6  set     1,(hl)          ; set bit 1 if third extension byte
.l06f8  pop     hl              ; restore values
        pop     af
        and     $7f             ; mask bit 7
.l06fc  call    l2b64           ; page in normal memory
        rst     $28
        defw    $0010           ; output char
.l0702  inc     hl
        djnz    l06d8           ; loop back for more extension
        push    hl
        ld      hl,FLAGS3
        bit     6,(hl)          ; test if want expanded catalog
        pop     hl
        jr      z,l073e         ; if not, move on
.l070e  ld      a,(RAMERR)      ; get attributes
        push    hl
        ld      hl,l0812        ; blank message
        bit     3,a
        jr      z,l071c
        ld      hl,l0818        ; if bit 3 set, PROT message
.l071c  push    af
        call    l07e2           ; output message
        pop     af
        ld      hl,l0812+1      ; blank message
        bit     2,a
        jr      z,l072b
        ld      hl,l081e        ; if bit 2 set, SYS message
.l072b  push    af
        call    l07e2           ; output message
        pop     af
        ld      hl,l0812+1      ; blank message
        bit     1,a
        jr      z,l073a
        ld      hl,l0823        ; if bit 1 set, ARC message
.l073a  call    l07e2           ; output message
        pop     hl
.l073e  ld      a,' '
        rst     $28
        defw    $0010           ; output space
.l0743  push    hl
        call    l2b89           ; page in DOS workspace
        ld      a,(hl)
        inc     hl
        ld      h,(hl)
        inc     hl              ; HA=filesize in K
        call    l2b64           ; page in normal memory
        ld      l,a
        ld      e,' '
        call    l0800           ; output filesize
        pop     hl
        inc     hl
        inc     hl              ; move to next file entry
        ld      a,'K'
        rst     $28
        defw    $0010           ; output "K"
.l075c  call    l07eb           ; output CR
        call    l2b89           ; page in DOS workspace
        pop     af
        dec     a
        jp      nz,l06b8        ; move back for more files in buffer
        pop     bc
        ld      a,b
        sub     $40             ; was buffer full? (*BUG* should be $3f)
        jr      c,l077b         ; if not, move on
        ld      hl,$f044
        ld      de,tmp_buff
        ld      bc,$000d
        ldir                    ; if so, copy last entry to first
        jp      l067f           ; and back for more
.l077b  call    l2b64           ; page in normal memory
        call    l07eb           ; output CR
        call    l2b89           ; page in DOS workspace
.l0784  ld      a,(DEFADD)      ; get drive letter
        or      a
        jr      nz,l079a        ; move on if not default
        ld      a,$ff
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_DRIVE   ; get default drive
        call    l32ee           ; restore TSTACK
        jp      nc,l06ab        ; go if error
.l079a  call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_FREE_SPACE  ; get free space on drive
        call    l32ee           ; restore TSTACK
        jp      nc,l06ab        ; go if error
        call    l2b64           ; page in normal memory
        ld      e,' '
        call    l0800           ; output number
        ld      hl,l07c9
        call    l07e2           ; output "K free" message
        call    l07eb           ; output CR
        ret                     ; done
.l07ba  call    l2b64           ; page in normal memory
        ld      hl,l07d1
        call    l07e2           ; output no files message
        call    l2b89           ; page in DOS workspace
        jp      l0784           ; go to display free space

.l07c9  defm    "K free"&$0d&0
.l07d1  defm    "No files found"&$0d&$0d&0

; Subroutine to output a null-terminated message

.l07e2  ld      a,(hl)          ; get next char
        or      a
        ret     z               ; exit if null
        rst     $28
        defw    $0010           ; output char
.l07e8  inc     hl
        jr      l07e2           ; loop back

; Subroutine to output a CR char

.l07eb  ld      a,$0d
        rst     $28
        defw    $0010           ; output CR
.l07f0  ret

        
; Subroutine to output a number up to 65535 (in HL)

.l07f1  push    hl
        ld      bc,$d8f0	; -10000
        rst     $28
        defw    $192a		; output 10000s
        ld      bc,$fc18	; -1000
        rst     $28
        defw    $192a		; output 1000s
        jr	l0801

; Subroutine to output a number up to 999 (in HL)

.l0800  push    hl
.l0801  ld      bc,$ff9c        ; -100
        rst     $28
        defw    $192a           ; output 100s
        ld      c,$f6           ; -10
        rst     $28
        defw    $192a           ; output 10s
        ld      a,l             ; units
        rst     $28
        defw    $15ef           ; output units
        pop     hl              ; restore number
        ret     

; Catalog attribute messages

.l0812  defm    "     "&0
.l0818  defm    " PROT"&0
.l081e  defm    " SYS"&0
.l0823  defm    " ARC"&0

; Subroutine to save a block to tape

.l0828  ld      hl,l0830
        push    hl              ; stack SA-RET routine address (why??)
        rst     $28
        defw    $04c6           ; save bytes
        ret     
.l0830  rst     $28
        defw    $053f           ; SA-RET
        ret     

; Subroutine to LOAD/VERIFY a block of data, from tape or disk
; On entry, IX=start, DE=length, A=type (usually $ff), carry set for LOAD
; or reset for VERIFY
; File 0 will be open for disk operations, which should be closed before exit
; On exit, carry is set if okay, reset if error

.l0834  push    af
        ld      a,(RAMERR)
        cp      'T'
        jp      z,l0883         ; move on for tape operations
        pop     af
        jr      nc,l087a        ; go to exit for disk verify (won't get here)
        push    hl              ; save registers
        push    de
        push    bc
        ld      b,$00           ; file 0
        ld      c,$00           ; page 0
        push    ix
        pop     hl              ; HL=address
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_READ        ; read the block
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        jr      c,l0865         ; move on to exit if okay
        cp      $19
        jr      nz,l087f        ; move on if error not end-of-file
        call    l0e9a           ; cause error
        defb    $31
.l0865  ld      b,$00
        call    l2b89           ; page in DOS workspace
.l086a  call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        jr      nc,l087f        ; move on if error
.l087a  scf                     ; signal success
        pop     bc              ; restore registers
        pop     de
        pop     hl
        ret     
.l087f  call    l0e9a           ; cause DOS error
        defb    $ff
.l0883  pop     af              ; if tape,restore flags and enter next routine

; Subroutine to call LD-BYTES subroutine in ROM 3

.l0884  rst     $28
        defw    $0556           ; call it
        ret     

; The SAVE/LOAD/VERIFY/MERGE commands

; section 1 - initialisation

.l0888  pop     af              ; discard return address of scan-loop
        ld      a,(T_ADDR)
        sub     l0f83~$ff       ; store command code (0=SAVE,1=LOAD,
        ld      (T_ADDR),a      ; 2=VERIFY,3=MERGE)
        call    l1129           ; get a string expression
        bit     7,(iy+$01)
        jp      z,l09ba         ; move on if syntax-checking
        ld      bc,$0011        ; 17 bytes required for LOAD
        ld      a,(T_ADDR)
        and     a
        jr      z,l08a6         
        ld      c,$22           ; but 34 for others
.l08a6  rst     $28
        defw    $0030           ; make space
        push    de
        pop     ix              ; IX points to space
        ld      b,$0b
        ld      a,' '
.l08b0  ld      (de),a          ; fill 11-byte name with spaces
        inc     de
        djnz    l08b0 
        ld      (ix+$01),$ff    ; place terminator in 2nd byte
        rst     $28
        defw    $2bf1           ; get string value from stack
        push    de
        push    bc

; section 2 - booting a disk

        ld      a,c
        dec     a
        or      b               ; check length
        jr      nz,l08e4        ; move on if not 1
        ld      a,(de)
        cp      '*'
        jr      nz,l08e4        ; or if not "*"
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_BOOT        ; boot a disk
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        cp      $23
        jr      nz,l08e0        ; if error isn't "disk not bootable", move on
        call    l0e9a           ; cause error
        defb    $3b
.l08e0  call    l0e9a           ; cause DOS error
        defb    $ff

; section 3 - setting drive for operation in RAMERR

.l08e4  inc     de
        ld      a,(de)
        dec     de
        cp      ':'
        jr      nz,l08fe        ; move on if no drive specified
        ld      a,(de)
        and     $df             ; get capitalised drive letter
        cp      'A'             ; check for valid drives
        jr      z,l090f         ; moving on if found
        cp      'B'
        jr      z,l090f
        cp      'M'
        jr      z,l090f
        cp      'T'
        jr      z,l090f
.l08fe  ld      a,(T_ADDR)      
        or      a
        ld      a,(SAVDRV)      ; use SAVDRV as drive for SAVE
        jr      z,l090a
        ld      a,(LODDRV)      ; or LODDRV otherwise
.l090a  ld      (RAMERR),a      ; store drive in RAMERR
        jr      l096c           ; move on

; section 4 - changing default drives for LOAD "A:" etc

.l090f  ld      l,a             ; save drive in L
        ld      a,c
        dec     a
        dec     a
        or      b               ; check string length
        jr      nz,l0966        ; move on if not 2
        ld      a,(T_ADDR)
        or      a
        jr      z,l0923         ; if SAVE, go to set default SAVE drive
        cp      $01
        jr      z,l0960         ; if LOAD, go to set default LOAD drive
        ld      a,l
        jr      l096c           ; else move on
.l0923  ld      a,l
        cp      'M'
        jr      z,l093f         ; move on if setting drive M:
        cp      'T'
        jr      z,l093f         ; or T: as default
        ld      hl,FLAGS3
        bit     4,(hl)
        jr      z,l093b         ; go to error if no disk interface
        cp      'A'
        jr      z,l093f         ; move on if setting A:
        bit     5,(hl)
        jr      nz,l093f        ; move on if setting B: and drive B: present
.l093b  call    l2ada
        defb    $4e             ; cause "Invalid drive" error
.l093f  ld      (SAVDRV),a      ; store in SAVDRV
.l0942  cp      'T'
        jr      z,l095d         ; move on for T:
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_DRIVE   ; set default drive
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        jr      c,l095d         ; move on if no error
        call    l0e9a
        defb    $ff             ; cause DOS error
.l095d  pop     bc              ; exit
        pop     de
        ret     
.l0960  ld      a,l
        ld      (LODDRV),a      ; store in LODDRV
        jr      l0942           ; go to set default DOS drive
.l0966  ld      a,(de)
        and     $df
        ld      (RAMERR),a      ; save capitalised drive in RAMERR

; section 5 - copying filename to page 7 (disk operations only)

.l096c  cp      'T'
        jr      z,l0998         ; move on for tape operations
        ld      a,(T_ADDR)
        cp      $02
        jr      nz,l097a        ; move on if not VERIFY
        pop     hl              ; for VERIFY on disk, just exit
        pop     hl
        ret     
.l097a  ld      a,b
        or      c               ; test length of string
        jr      nz,l0982
        call    l0e9a
        defb    $0e             ; invalid filename error if zero
.l0982  ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy filename to page 7
        pop     bc
        ld      bc,$000a
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        call    l2b64           ; page in normal memory
        jr      l0999           ; move on with disk operations

; section 6 - copying filename into 1st header
; *BUG* If filename was specified as "T:name", the "T:" is never stripped

.l0998  pop     bc              ; restore length & add of filename
.l0999  pop     de
        ld      hl,$fff6
        dec     bc
        add     hl,bc
        inc     bc
        jr      nc,l09b3        ; move on if filename 10 letters or less
        ld      a,(T_ADDR)
        and     a
        jr      nz,l09ac
        call    l0e9a
        defb    $0e             ; bad filename error if SAVEing
.l09ac  ld      a,b
        or      c
        jr      z,l09ba         ; move on if no filename
        ld      bc,$000a        ; copy 10 chars
.l09b3  push    ix
        pop     hl
        inc     hl
        ex      de,hl
        ldir                    ; copy filename to header+1 in workspace

; At this point, syntax-checking rejoins the routines
; Each of the following sections fills in the remaining header information
; for their filetype & ensures syntax is correct to end-of-statement.
; At run-time the sections recombine at section 11, with HL=address
; to load/verify/merge/save at

; section 7 - DATA operations

.l09ba  rst     $28
        defw    $0018           ; get next char
        cp      $e4
        jr      nz,l0a11        ; move on if not DATA
        ld      a,(T_ADDR)
        cp      $03
        jp      z,l1125         ; error if used with MERGE
        rst     $28
        defw    $0020           ; get current char
.l09cc  rst     $28
        defw    $28b2           ; search for variable
        set     7,c             ; set bit 7 of array's name
        jr      nc,l09e0        ; jump if handling existing array
        ld      hl,$0000
        ld      a,(T_ADDR)
        dec     a
        jr      z,l09f4
        call    l0e9a
        defb    $01             ; error 2 if trying to SAVE/VERIFY new array
.l09e0  jp      nz,l1125        ; error if just a numeric variable
        bit     7,(iy+$01)
        jr      z,l0a01         ; move on if checking syntax
        inc     hl
        ld      a,(hl)
        ld      (ix+$0b),a      ; copy array length into workspace header
        inc     hl
        ld      a,(hl)
        ld      (ix+$0c),a
        inc     hl
.l09f4  ld      (ix+$0e),c      ; copy array name into workspace header
        ld      a,$01           ; type 1
        bit     6,c
        jr      z,l09fe         ; move on if numeric array
        inc     a               ; else type 2
.l09fe  ld      (ix+$00),a      ; copy type into workspace header
.l0a01  ex      de,hl
        rst     $28
        defw    $0020           ; get next char
        cp      ')'
        jr      nz,l09e0        ; error if not ")"
        rst     $20
        call    l10b1           ; check for end-of-statement
.l0a0d  ex      de,hl
        jp      l0ad5           ; jump on

; section 8 - SCREEN$ operations

.l0a11  cp      $aa             ; check for SCREEN$
        jr      nz,l0a36        ; move on if not
        ld      a,(T_ADDR)
        cp      $03
        jp      z,l1125         ; error if trying to MERGE
        rst     $28
        defw    $0020           ; get next char
        call    l10b1           ; check for end-of-statement
        ld      (ix+$0b),$00    ; store screen length
        ld      (ix+$0c),$1b
        ld      hl,$4000
        ld      (ix+$0d),l      ; and start
        ld      (ix+$0e),h
        jr      l0a89           ; jump on

; section 9 - CODE operations

.l0a36  cp      $af             ; check for CODE
        jr      nz,l0a8f        ; move on if not
        ld      a,(T_ADDR)
        cp      $03
        jp      z,l1125         ; error if trying to MERGE
        rst     $28
        defw    $0020           ; get next char
.l0a45  call    l0e94
        jr      nz,l0a56        ; move on if not end-of-statement
        ld      a,(T_ADDR)
        and     a
        jp      z,l1125         ; error if trying to SAVE with no parameters
        rst     $28
        defw    $1ce6           ; get zero to calculator stack
        jr      l0a67           ; move on
.l0a56  call    l1121           ; get numeric expression
        rst     $28
        defw    $0018           ; get next char
        cp      ','
        jr      z,l0a6c         ; move on if comma
        ld      a,(T_ADDR)
        and     a
        jp      z,l1125         ; error if trying to SAVE with 1 parameter
.l0a67  rst     $28
        defw    $1ce6           ; get zero to calculator stack
        jr      l0a72           ; move on
.l0a6c  rst     $28
        defw    $0020           ; get next char
.l0a6f  call    l1121           ; get numeric expression
.l0a72  call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $1e99           ; get length to BC
        ld      (ix+$0b),c      ; store in workspace header
        ld      (ix+$0c),b
        rst     $28
        defw    $1e99           ; get address to BC
        ld      (ix+$0d),c      ; store in workspace header
        ld      (ix+$0e),b
        ld      h,b             ; HL=address
        ld      l,c
.l0a89  ld      (ix+$00),$03    ; type 3 to workspace header
        jr      l0ad5           ; move on

; section 10 - BASIC operations

.l0a8f  cp      $ca             ; check for LINE
        jr      z,l0a9c         ; move on if present
        call    l10b1           ; check for end-of-statement
        ld      (ix+$0e),$80    ; no auto-run
        jr      l0ab5           ; move on
.l0a9c  ld      a,(T_ADDR)
        and     a
        jp      nz,l1125        ; error unless SAVE with LINE
        rst     $28
        defw    $0020           ; get next char
.l0aa6  call    l1121           ; get numeric expression
        call    l10b1           ; check for end-of-line
        rst     $28
        defw    $1e99           ; get line to BC
        ld      (ix+$0d),c      ; store in workspace header
        ld      (ix+$0e),b
.l0ab5  ld      (ix+$00),$00    ; type 0
        ld      hl,(E_LINE)     
        ld      de,(PROG)
        scf     
        sbc     hl,de           ; HL=program+vars length
        ld      (ix+$0b),l      ; store in workspace header
        ld      (ix+$0c),h
        ld      hl,(VARS)
        sbc     hl,de           ; HL=program only length
        ld      (ix+$0f),l      ; store in workspace header
        ld      (ix+$10),h
        ex      de,hl

; section 11 - LOAD/VERIFY/MERGE tape operations

.l0ad5  ld      a,(T_ADDR)
        and     a
        jp      z,l0d6e         ; move on if saving
        push    hl
        ld      bc,$0011
        add     ix,bc           ; IX points to 2nd header
        ld      a,(RAMERR)
        cp      'T'
        jr      nz,l0b41        ; move on if disk operation
.l0ae9  push    ix
        ld      de,$0011
        xor     a
        scf     
        call    l0884           ; load header from tape to 2nd header area
        pop     ix
        jr      nc,l0ae9        ; loop back if error
        ld      a,$fe
        rst     $28
        defw    $1601           ; open channel to stream -2
        ld      (iy+$52),$03    ; set scroll count
        ld      c,$80           ; signal "names don't match"
        ld      a,(ix+$00)
        cp      (ix-$11)        ; compare types
        jr      nz,l0b0c        ; jump if no match
        ld      c,$f6           ; C must be incremented 10 times to match
.l0b0c  cp      $04
        jr      nc,l0ae9        ; error for types 4+
        ld      de,$09c0        ; address of message block in ROM 3
        push    bc
        rst     $28
        defw    $0c0a           ; print filetype message
        pop     bc
        push    ix
        pop     de              ; DE points to filename to check for
        ld      hl,$fff0        
        add     hl,de           ; HL points to loaded filename
        ld      b,$0a           ; check 10 chars
        ld      a,(hl)          ; get next char
        inc     a
        jr      nz,l0b28        ; move on if name to check not null
        ld      a,c             ; if null, signal "10 chars match"
        add     a,b
        ld      c,a
.l0b28  inc     de
        ld      a,(de)
        cp      (hl)            ; compare names
        inc     hl
        jr      nz,l0b2f
        inc     c               ; increment C if chars match
.l0b2f  rst     $28
        defw    $0010           ; output char
        djnz    l0b28           ; loop back
        bit     7,c
        jr      nz,l0ae9        ; loop back if no match
        ld      a,$0d
        rst     $28
        defw    $0010           ; output CR
        pop     hl
        jp      l0ba6           ; move on

; section 12 - LOAD/MERGE disk operations

.l0b41  ld      a,(T_ADDR)
        cp      $02
        jr      z,l0ba6         ; move on if VERIFY (can't be here if so!)
        push    ix
        ld      b,$00           ; file 0
        ld      c,$01           ; exclusive-read
        ld      d,$00
        ld      e,$01
        ld      hl,tmp_fspec
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open file
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        jr      c,l0b6c
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l0b6c  ld      b,$00
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_HEAD    ; IX to header
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        ex      (sp),ix
        pop     hl
        call    l2b89           ; page in DOS workspace
        ld      a,(hl)
        call    l2b64           ; page in normal memory
        cp      (ix-$11)        ; compare types
        jr      z,l0b92         
        call    l0e9a
        defb    $1d             ; error if different
.l0b92  ld      (ix+$00),a      ; store in 2nd header area
        push    ix
        pop     de
        ex      de,hl
        ld      bc,$000b
        add     hl,bc
        ex      de,hl
        inc     hl
        ld      bc,$0006
        call    l3f8a           ; copy parameters from page 7 into 2nd header
        pop     hl

; section 13 - Perform tape/disk VERIFY (any type) or LOAD (CODE only)

.l0ba6  ld      a,(ix+$00)
        cp      $03
        jr      z,l0bb9         ; move on for type 3
        ld      a,(T_ADDR)
        dec     a
        jp      z,l0c04         ; move on if LOADing
        cp      $02
        jp      z,l0cb2         ; move on if MERGEing
.l0bb9  push    hl              ; save address to LOAD/VERIFY at
        ld      l,(ix-$06)
        ld      h,(ix-$05)      ; HL=length from 1st header
        ld      e,(ix+$0b)
        ld      d,(ix+$0c)      ; DE=length from 2nd header
        ld      a,h
        or      l
        jr      z,l0bd7         ; move on if length not specified (CODE only)
        sbc     hl,de
        jr      c,l0c00         ; error if block larger than requested
        jr      z,l0bd7
        ld      a,(ix+$00)
        cp      $03
        jr      nz,l0bfc        ; error for uneven lengths except in CODE
.l0bd7  pop     hl              ; restore address
        ld      a,h
        or      l
        jr      nz,l0be2
        ld      l,(ix+$0d)
        ld      h,(ix+$0e)      ; if zero, use start address of 2nd header
.l0be2  push    hl
        pop     ix              ; IX=address to load
        ld      a,(T_ADDR)
        cp      $02
        scf                     ; set carry for LOAD
        jr      nz,l0bf6
        and     a               ; reset carry for VERIFY
        ld      a,(RAMERR)
        cp      'T'
        jr      z,l0bf6
        ret                     ; exit if VERIFY with disk (won't get here!)
.l0bf6  ld      a,$ff           ; data block
.l0bf8  call    l0834           ; load/verify block from tape/disk
        ret     c               ; exit if okay
.l0bfc  call    l0e9a           ; error R - tape loading error
        defb    $1a
.l0c00  call    l0e9a           ; error ???
        defb    $4f

; section 14 - Perform tape/disk LOAD (types 0-2)

.l0c04  ld      e,(ix+$0b)
        ld      d,(ix+$0c)      ; DE=length from 2nd header
.l0c0a  push    hl
        ld      a,h
        or      l               ; test start=0 (previously undeclared array)
        jr      nz,l0c15        ; move on if not
        inc     de
        inc     de
        inc     de              ; add 3 bytes for name (1) & length (2)
        ex      de,hl
        jr      l0c21           ; move on
.l0c15  ld      l,(ix-$06)
        ld      h,(ix-$05)      ; HL=size of existing prog+vars or array
        ex      de,hl
        scf     
        sbc     hl,de
        jr      c,l0c2a         ; move on if no extra space required
.l0c21  ld      de,$0005        ; allow for 5-byte overhead
        add     hl,de
        ld      b,h
        ld      c,l
        rst     $28
        defw    $1f05           ; test if space available
.l0c2a  pop     hl              ; restore destination address
        ld      a,(ix+$00)
        and     a
        jr      z,l0c6f         ; move on for BASIC programs
        ld      a,h
        or      l
        jr      z,l0c48         ; move on if loading new array
        dec     hl
        ld      b,(hl)
        dec     hl
        ld      c,(hl)          ; get existing array length from vars area
        dec     hl
        inc     bc
        inc     bc
        inc     bc              ; add 3 for name & length
        ld      (X_PTR),ix      ; save IX
        rst     $28
        defw    $19e8           ; reclaim space
        ld      ix,(X_PTR)      ; restore IX
.l0c48  ld      hl,(E_LINE)
        dec     hl              ; HL points to $80 at end of vars
        ld      c,(ix+$0b)
        ld      b,(ix+$0c)      ; get length of new array
        push    bc              ; save
        inc     bc
        inc     bc
        inc     bc              ; add 3 for name & length
        ld      a,(ix-$03)
        push    af              ; save array name (from old header)
        rst     $28
        defw    $1655           ; make the room
        defb    $23
        pop     af
        ld      (hl),a          ; store name
        pop     de
        inc     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; and length
        inc     hl
        push    hl
        pop     ix              ; IX points to array start
        scf                     ; set carry for LOAD
        ld      a,$ff           ; data block
        jp      l0bf8           ; go to load it
.l0c6f  ex      de,hl           ; save DE=destination
        ld      hl,(E_LINE)
        dec     hl              ; end of vars
        ld      (X_PTR),ix      ; save IX
        ld      c,(ix+$0b)
        ld      b,(ix+$0c)      ; get length of new data block
        push    bc
        rst     $28
        defw    $19e5           ; reclaim entire prog+vars
        pop     bc
        push    hl
        push    bc
        rst     $28
        defw    $1655           ; make room for new block
        ld      ix,(X_PTR)      ; restore IX
        inc     hl
        ld      c,(ix+$0f)
        ld      b,(ix+$10)      ; BC=length of program only
        add     hl,bc
        ld      (VARS),hl       ; set new start of vars
        ld      h,(ix+$0e)
        ld      a,h
        and     $c0
        jr      nz,l0ca9        ; move on if no auto-run number
        ld      l,(ix+$0d)
        ld      (NEWPPC),hl     ; set new line & statement
        ld      (iy+$0a),$00
.l0ca9  pop     de              ; restore length & start
        pop     ix
        scf                     ; set carry for LOAD
        ld      a,$ff           ; data block
        jp      l0bf8           ; go to load it

; section 15 - Perform tape/disk MERGE

.l0cb2  ld      c,(ix+$0b)
        ld      b,(ix+$0c)      ; fetch length of new block
        push    bc
        inc     bc
        rst     $28
        defw    $0030           ; make length+1 bytes in workspace
.l0cbd  ld      (hl),$80        ; terminate with an end-marker
        ex      de,hl           ; HL=start
        pop     de              ; DE=length
        push    hl              ; save start
        push    hl
        pop     ix              ; IX=start
        scf                     ; set carry for LOAD
        ld      a,$ff           ; data block
        call    l0bf8           ; load the block
        pop     hl              ; HL=start of new prog
        ld      de,(PROG)       ; DE=start of old prog
.l0cd0  ld      a,(hl)
        and     $c0
        jr      nz,l0cee        ; move on if all lines done
.l0cd5  ld      a,(de)
        inc     de
        cp      (hl)            ; compare high bytes of line number
        inc     hl
        jr      nz,l0cdd        ; skip next test if no match
        ld      a,(de)
        cp      (hl)            ; compare low bytes of line number
.l0cdd  dec     de
        dec     hl
        jr      nc,l0ce9        ; move on if can place line here
        push    hl
        ex      de,hl
        rst     $28
        defw    $19b8           ; get address of next line in old prog
        pop     hl
        jr      l0cd5           ; loop back
.l0ce9  call    l0d2a           ; enter the new line
        jr      l0cd0           ; loop back
.l0cee  ld      a,(hl)          ; get var name from workspace
        ld      c,a
        cp      $80
        ret     z               ; exit if all done
        push    hl
        ld      hl,(VARS)       ; fetch start of vars
.l0cf7  ld      a,(hl)
        cp      $80
        jr      z,l0d21         ; move on if reached end
        cp      c
        jr      z,l0d07         ; move on if found match
.l0cff  push    bc
        rst     $28
        defw    $19b8           ; get to next var
        pop     bc
        ex      de,hl
        jr      l0cf7           ; loop back
.l0d07  and     $e0
        cp      $a0
        jr      nz,l0d1f        ; move on if not long-named var
        pop     de
        push    de
        push    hl
.l0d10  inc     hl
        inc     de
        ld      a,(de)
        cp      (hl)            ; compare long names
        jr      nz,l0d1c        ; move on if mismatch
        rla     
        jr      nc,l0d10        ; loop back
        pop     hl
        jr      l0d1f
.l0d1c  pop     hl
        jr      l0cff           ; go back if unsuccessful
.l0d1f  ld      a,$ff           ; signal "replace variable"
.l0d21  pop     de
        ex      de,hl
        inc     a
        scf                     ; signal "variables"
        call    l0d2a           ; merge in the variable
        jr      l0cee           ; loop back

; Subroutine to merge a line or variable (part of section 15)

.l0d2a  jr      nz,l0d3c        ; move on if not replacing a line/variable
        ex      af,af'          ; save flags
        ld      (X_PTR),hl      ; save pointer in new program/vars
        ex      de,hl
        rst     $28
        defw    $19b8
        rst     $28
        defw    $19e8           ; reclaim old line/var
        ex      de,hl
        ld      hl,(X_PTR)      ; restore
        ex      af,af'
.l0d3c  ex      af,af'          ; save flags
        push    de
        rst     $28
        defw    $19b8           ; find length of new line/var
        ld      (X_PTR),hl      ; save pointer in new program/vars
        ld      hl,(PROG)
        ex      (sp),hl         ; save PROG to avoid corruption
        push    bc
        ex      af,af'
        jr      c,l0d53         ; move on if adding a variable
        dec     hl
        rst     $28
        defw    $1655           ; make room for new line
        inc     hl
        jr      l0d56
.l0d53  rst     $28
        defw    $1655           ; make room for new var
.l0d56  inc     hl              ; point to first new location
        pop     bc
        pop     de
        ld      (PROG),de       ; restore PROG
        ld      de,(X_PTR)      ; retrieve new pointer
        push    bc
        push    de
        ex      de,hl
        ldir                    ; copy new var/line into space made
        pop     hl
        pop     bc
        push    de
        rst     $28
        defw    $19e8           ; reclaim workspace holding new var/line
        pop     de              
        ret     

; section 16 - Perform disk SAVE

.l0d6e  ld      a,(RAMERR)
        cp      'T'
        jp      z,l0e10         ; move on for tape operations
        call    l2b89           ; page in DOS workspace
        push    hl
        ld      b,$00           ; file 0
        ld      c,$03           ; exclusive read-write
        ld      d,$01
        ld      e,$03
        ld      hl,tmp_fspec
        push    ix
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; create the file
        call    l32ee           ; restore TSTACK
        jr      c,l0d9b         ; move on unless error
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause DOS error
        defb    $ff
.l0d9b  ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_HEAD    ; get IX=header data
        call    l32ee           ; restore TSTACK
        jr      c,l0db1
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause DOS error
        defb    $ff
.l0db1  ex      (sp),ix         ; IX=pointer to header in normal memory
        pop     hl              ; HL=pointer to header in page 7
        call    l2b64           ; page in normal memory
        ld      a,(ix+$00)
        call    l2b89           ; page in DOS workspace
        ld      (hl),a          ; transfer type
        inc     hl
        push    ix
        pop     de
        ex      de,hl           ; DE=DOS header address+1
        ld      bc,$000b
        add     hl,bc           ; HL=page 0 header parameters
        ld      bc,$0006
        call    l2b64           ; page in normal memory
        call    l3f63           ; copy parameters to DOS header
        ld      b,$00           ; file 0
        ld      c,$00
        ld      e,(ix+$0b)
        ld      d,(ix+$0c)      ; DE=length
        ld      a,d
        or      e
        call    l2b89           ; page in DOS workspace
        jr      z,l0df6         ; move on if zero length
        pop     hl              ; restore start address
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_WRITE       ; write block to file
        call    l32ee           ; restore TSTACK
        jr      c,l0df6
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l0df6  ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file
        call    l32ee           ; restore TSTACK
        jr      c,l0e0c
        call    l2b64
        call    l0e9a           ; cause any DOS error
        defb    $ff
.l0e0c  call    l2b64           ; page in normal memory
        ret                     ; done

; section 17 - Perform tape SAVE

.l0e10  push    hl
        ld      a,$fd
        rst     $28
        defw    $1601           ; open channel to stream -3
        xor     a
        ld      de,$09a1
        rst     $28
        defw    $0c0a           ; output ROM 3's start tape message
        set     5,(iy+$02)      ; signal "screen needs clearing"
        rst     $28
        defw    $15d4           ; wait for a key
        push    ix              ; save header address
        ld      de,$0011
        xor     a               ; header block
        call    l0828           ; save header
        pop     ix
        ld      b,$32
.l0e31  halt                    ; delay for 1 sec
        djnz    l0e31
        ld      e,(ix+$0b)
        ld      d,(ix+$0c)      ; DE=length
        ld      a,$ff           ; data block
        pop     ix              ; IX=start
        jp      l0828           ; save & exit

; Looks like these bits aren't used

        defb    $80        
.l0e42  defm    "Press REC & PLAY, then any key"&$ae
        defm    $0d&"Program:"&$a0
        defm    $0d&"Number array:"&$a0
        defm    $0d&"Character array:"&$a0
        defm    $0d&"Bytes:"&$a0

; Subroutine to check if char in A is a statement terminator

.l0e94  cp      $0d
        ret     z
        cp      ':'
        ret     

; Subroutine to cause a +3DOS error
; Routine will attempt to close file 0 before causing error
; On entry, A=+3DOS error code and byte following call is $ff
; or, byte following call is +3 BASIC error

.l0e9a  push    af              ; save error code
.l0e9b  ld      a,(RAMERR)
        cp      $54
        jr      z,l0eca         ; move on if SAVE/LOAD was using drive T:
        ld      b,$00
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file 0
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
        jr      c,l0eca         ; move on if no error
        ld      b,$00
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_ABANDON     ; else abandon file 0
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in normal memory
.l0eca  pop     af              ; restore error code
        pop     hl              ; get return address
        ld      e,(hl)          ; get inline code
        bit     7,e
        jr      z,l0edc         ; use it as error code if not $ff
        cp      $0a             ; convert DOS error code to +3 BASIC error
        jr      nc,l0ed9        
.l0ed5  add     a,$3d
        jr      l0edb
.l0ed9  add     a,$18
.l0edb  ld      e,a
.l0edc  ld      h,e             ; place code CALL l2ada, DEFB error
        ld      l,l2ada/$100    ; on stack
        push    hl
        ld      l,$cd
        ld      h,l2ada~$ff
        push    hl
        xor     a
        ld      hl,$0000
        add     hl,sp
        jp      (hl)            ; jump to execute code on stack & cause error

; The parameter offset table
; This contains offsets from each entry's position into the following
; parameter table

.l0eeb  defb    l0f9c-ASMPC     ; DEF FN
        defb    l0fb6-ASMPC     ; CAT
        defb    l0fa9-ASMPC     ; FORMAT
        defb    l0fac-ASMPC     ; MOVE
        defb    l0fb2-ASMPC     ; ERASE
        defb    l0f9f-ASMPC     ; OPEN#
        defb    l0fa5-ASMPC     ; CLOSE#
        defb    l0f85-ASMPC     ; MERGE
        defb    l0f84-ASMPC     ; VERIFY
        defb    l0f86-ASMPC     ; BEEP
        defb    l0f8a-ASMPC     ; CIRCLE
        defb    l0f8e-ASMPC     ; INK
        defb    l0f8f-ASMPC     ; PAPER
        defb    l0f90-ASMPC     ; FLASH
        defb    l0f91-ASMPC     ; BRIGHT
        defb    l0f92-ASMPC     ; INVERSE
        defb    l0f93-ASMPC     ; OVER
        defb    l0f94-ASMPC     ; OUT
        defb    l0f7c-ASMPC     ; LPRINT
        defb    l0f7f-ASMPC     ; LLIST
        defb    l0f2d-ASMPC     ; STOP
        defb    l0f6c-ASMPC     ; READ
        defb    l0f6f-ASMPC     ; DATA
        defb    l0f72-ASMPC     ; RESTORE
        defb    l0f4b-ASMPC     ; NEW
        defb    l0f98-ASMPC     ; BORDER
        defb    l0f5b-ASMPC     ; CONTINUE
        defb    l0f45-ASMPC     ; DIM
        defb    l0f48-ASMPC     ; REM
        defb    l0f33-ASMPC     ; FOR
        defb    l0f20-ASMPC     ; GOTO
        defb    l0f29-ASMPC     ; GOSUB
        defb    l0f42-ASMPC     ; INPUT
        defb    l0f83-ASMPC     ; LOAD
        defb    l0f51-ASMPC     ; LIST
        defb    l0f1d-ASMPC     ; LET
        defb    l0f68-ASMPC     ; PAUSE
        defb    l0f3b-ASMPC     ; NEXT
        defb    l0f54-ASMPC     ; POKE
        defb    l0f3f-ASMPC     ; PRINT
        defb    l0f64-ASMPC     ; PLOT
        defb    l0f4e-ASMPC     ; RUN
        defb    l0f82-ASMPC     ; SAVE
        defb    l0f58-ASMPC     ; RANDOMIZE
        defb    l0f24-ASMPC     ; IF
        defb    l0f61-ASMPC     ; CLS
        defb    l0f75-ASMPC     ; DRAW
        defb    l0f5e-ASMPC     ; CLEAR
        defb    l0f30-ASMPC     ; RETURN
        defb    l0f79-ASMPC     ; COPY

; The parameter table for the BASIC commands
; These comprise command classes ($00-$0e), separators and
; where appropriate, command addresses
; The classes are:
;       00      No further operands
;       01      Used in LET: a variable is required
;       02      Used in LET: an expression (numeric or string) must follow
;       03      A numeric expression may follow (default=0)
;       04      A single character variable must follow
;       05      A set of items may be given
;       06      A numeric expression must follow
;       07      Handles colour items
;       08      Two numeric expressions separated by a comma must follow
;       09      As 08, but colour items may precede the expressions
;       0a      A string expression must follow
;       0b      Handles cassette/disk routines
;       0c      As 00, but handled in ROM 1 not ROM 3
;       0d      As 03, but handled in ROM 1 not ROM 3
;       0e      As 05, but handled in ROM 1 not ROM 3

.l0f1d  defb    $01,'=',$02     ; LET
.l0f20  defb    $06,$00
        defw    $1e67           ; GOTO
.l0f24  defb    $06,$cb,$0e
        defw    l115e           ; IF
.l0f29  defb    $06,$0c
        defw    l124a           ; GOSUB
.l0f2d  defb    $00
        defw    $1cee           ; STOP
.l0f30  defb    $0c
        defw    l1266           ; RETURN
.l0f33  defb    $04,'=',$06
        defb    $cc,$06,$0e
        defw    l1178           ; FOR
.l0f3b  defb    $04,$00
        defw    $1dab           ; NEXT
.l0f3f  defb    $0e
        defw    l217b           ; PRINT
.l0f42  defb    $0e
        defw    l218f           ; INPUT
.l0f45  defb    $0e
        defw    l22ad           ; DIM
.l0f48  defb    $0e
        defw    l1072           ; REM
.l0f4b  defb    $0c
        defw    l2280           ; NEW
.l0f4e  defb    $0d
        defw    l11f9           ; RUN
.l0f51  defb    $0e
        defw    l1539           ; LIST
.l0f54  defb    $08,$00
        defw    $1e80           ; POKE
.l0f58  defb    $03
        defw    $1e4f           ; RANDOMIZE
.l0f5b  defb    $00
        defw    $1e5f           ; CONTINUE
.l0f5e  defb    $0d
        defw    l1204           ; CLEAR
.l0f61  defb    $00
        defw    $0d6b           ; CLS
.l0f64  defb    $09,$00
        defw    $22dc           ; PLOT
.l0f68  defb    $06,$00
        defw    $1f3a           ; PAUSE
.l0f6c  defb    $0e
        defw    l11a2           ; READ
.l0f6f  defb    $0e
        defw    l11e2           ; DATA
.l0f72  defb    $03
        defw    $1e42           ; RESTORE
.l0f75  defb    $09,$0e
        defw    l2296           ; DRAW
.l0f79  defb    $0e
        defw    l21aa           ; COPY
.l0f7c  defb    $0e
        defw    l2177           ; LPRINT
.l0f7f  defb    $0e
        defw    l1535           ; LLIST
.l0f82  defb    $0b             ; SAVE
.l0f83  defb    $0b             ; LOAD
.l0f84  defb    $0b             ; VERIFY
.l0f85  defb    $0b             ; MERGE
.l0f86  defb    $08,$00
        defw    $03f8           ; BEEP
.l0f8a  defb    $09,$0e
        defw    l2286           ; CIRCLE
.l0f8e  defb    $07             ; INK
.l0f8f  defb    $07             ; PAPER
.l0f90  defb    $07             ; FLASH
.l0f91  defb    $07             ; BRIGHT
.l0f92  defb    $07             ; INVERSE
.l0f93  defb    $07             ; OVER
.l0f94  defb    $08,$00
        defw    $1e7a           ; OUT
.l0f98  defb    $06,$00
        defw    $2294           ; BORDER
.l0f9c  defb    $0e
        defw    l1283           ; DEF FN
.l0f9f  defb    $06,',',$0a,$00
        defw    $1736           ; OPEN#
.l0fa5  defb    $06,$00
        defw    $16e5           ; CLOSE#
.l0fa9  defb    $0e
        defw    l026c           ; FORMAT
.l0fac  defb    $0a,$cc,$0a,$0c
        defw    l04e5           ; MOVE
.l0fb2  defb    $0a,$0c
        defw    l044a           ; ERASE
.l0fb6  defb    $0e
        defw    l05b8           ; CAT
.l0fb9  defb    $0c
        defw    l1465           ; SPECTRUM
.l0fbc  defb    $0e
        defw    l23f1           ; PLAY

; The main parser entry point
; Enter here for syntax checking

.l0fbf  res     7,(iy+$01)      ; signal "syntax checking"
        rst     $28
        defw    $19fb           ; point to the first code after any line no
        xor     a
        ld      (SUBPPC),a      ; initialise SUBPPC to zero statements
        dec     a
        ld      (ERR_NR),a      ; signal "OK" error code
        jr      l0fd1           ; jump to start checking

; The statement loop

.l0fd0  rst     $20             ; advance CH_ADD
.l0fd1  rst     $28
        defw    $16bf           ; clear workspace
        inc     (iy+$0d)        ; increment SUBPPC on each statement
        jp      m,l1125         ; error if more than 127 statements on line
        rst     $18             ; fetch character
        ld      b,$00
        cp      $0d
        jp      z,l1073         ; move on if end-of-line
        cp      ':'
        jr      z,l0fd0         ; loop back if end-of-statement  
        ld      hl,l1031
        push    hl              ; load stack with return address to STMT-RET
        ld      c,a             ; save command code
        rst     $20             ; advance CH_ADD
        ld      a,c
        sub     $ce             ; put command code in range $00..$31
        jr      nc,l1004        ; move on if valid keyword
        add     a,$ce           ; else reform character
        ld      hl,l0fb9        ; address of SPECTRUM parameter entries
        cp      $a3
        jr      z,l1010         ; move on if SPECTRUM command
        ld      hl,l0fbc        ; address of PLAY parameter entries
        cp      $a4
        jr      z,l1010         ; move on if PLAY command
        jp      l1125           ; else give Nonsense in BASIC
.l1004  ld      c,a
        ld      hl,l0eeb        ; syntax offset table start
        add     hl,bc
        ld      c,(hl)
        add     hl,bc           ; get start of entries in parameter table
        jr      l1010           ; move on
.l100d  ld      hl,(T_ADDR)     ; get pointer into parameter table
.l1010  ld      a,(hl)          ; get next parameter type
        inc     hl
        ld      (T_ADDR),hl     ; save pointer
        ld      bc,l100d
        push    bc              ; stack return address back into this loop
        ld      c,a
        cp      $20
        jr      nc,l102a        ; move on if entry is a separator
        ld      hl,l10c5        ; base of command class table
        ld      b,$00
        add     hl,bc
        ld      c,(hl)          ; get offset
        add     hl,bc
        push    hl              ; stack computed command class routine address
        rst     $18             ; get next char to A
        dec     b               ; B=$ff
        ret                     ; call command class routine
.l102a  rst     $18             ; get next char
        cp      c
        jp      nz,l1125        ; nonsense in BASIC if not required separator
        rst     $20             ; get next character
        ret                     ; back into loop at l100d
        
; The "STMT-RET" routine. A return is made here after correct interpretation
; of a statement

.l1031  call    l2af9           ; test BREAK key
        jr      c,l103a         ; move on if not pressed
        call    l2ada
        defb    $14             ; error L - BREAK into program
.l103a  bit     7,(iy+$0a)
        jp      nz,l10b8        ; move on if a jump is not being made
        ld      hl,(NEWPPC)     ; get new line number
        bit     7,h             ; check if running line in edit area
        jr      z,l105c         ; move on if not

; Enter here if running a line in the edit area

.l1048  ld      hl,$fffe
        ld      (PPC),hl        ; this is line "-2"
        ld      hl,(WORKSP)
        dec     hl              ; HL points to end of edit area
        ld      de,(E_LINE)
        dec     de              ; DE points to location before edit area
        ld      a,(NSPPC)       ; fetch number of next statement to handle
        jr      l1092           ; move on

; Perform a jump in the program

.l105c  rst     $28
        defw    $196e           ; get start address of line to jump to
        ld      a,(NSPPC)       ; get statement number
        jr      z,l1080         ; move on if correct line was found
        and     a               ; else check statement number
        jr      nz,l10ad        ; if not zero, N - statement lost error
        ld      b,a
        ld      a,(hl)
        and     $c0             ; check for end of program
        ld      a,b
        jr      z,l1080         ; move on if not
        call    l2ada
        defb    $ff             ; else end with 0 - OK error

.l1072  pop     bc              ; REM command - drop STMT-RET address to
                                ; ignore rest of command

; The Line-end routine

.l1073  bit     7,(iy+$01)
        ret     z               ; exit if syntax-checking
        ld      hl,(NXTLIN)     ; get address of NXTLIN
        ld      a,$c0
        and     (hl)
        ret     nz              ; exit if end of program
        xor     a               ; use statement zero

; The line-use routine

.l1080  cp      $01
        adc     a,$00           ; change statement zero to 1
        ld      d,(hl)
        inc     hl
        ld      e,(hl)
        ld      (PPC),de        ; store line number in PPC
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl           ; DE holds start of line
        add     hl,de
        inc     hl              ; HL holds start of next line
.l1092  ld      (NXTLIN),hl     ; store next line address
        ex      de,hl
        ld      (CH_ADD),hl     ; update CH_ADD to current line start
        ld      d,a
        ld      e,$00
        ld      (iy+$0a),$ff    ; signal "no jump"
        dec     d
        ld      (iy+$0d),d      ; statement number-1 to SUBPPC
        jp      z,l0fd0         ; enter loop if want first statement
        inc     d
        rst     $28             
        defw    $198b           ; else find required statement
        jr      z,l10b8         ; move on if found
.l10ad  call    l2ada
        defb    $16             ; report N - statement lost


; The "Check-end" subroutine. During syntax-checking, it ensures that
; the end of the statement has been reached, generating an error if not.

.l10b1  bit     7,(iy+$01)      ; check bit 7 of FLAGS
        ret     nz              ; return if run-time
        pop     bc              ; drop return address of statement routine
        pop     bc              ; drop return address of scan-loop routine
.l10b8  rst     $18             ; get next character
        cp      $0d
        jr      z,l1073         ; move back if end-of-line
        cp      $3a
        jp      z,l0fd0         ; move back if end-of-statement
        jp      l1125           ; else Nonsense in BASIC error


; The command class offset table
; This contains offsets from the entry in the table to the
; actual command class routines following

.l10c5  defb    l10e9-ASMPC
        defb    l110c-ASMPC
        defb    l1110-ASMPC
        defb    l10e6-ASMPC
        defb    l1118-ASMPC
        defb    l10ea-ASMPC
        defb    l1121-ASMPC
        defb    l112d-ASMPC
        defb    l111d-ASMPC
        defb    l1157-ASMPC
        defb    l1129-ASMPC
        defb    l115b-ASMPC
        defb    l10d7-ASMPC
        defb    l10d4-ASMPC
        defb    l10d8-ASMPC

; Class $0c,$0d,$0e routines
; Enter at l10d4 for $0d, l10d7 for $0c and l10d8 for $0e

.l10d4  rst     $28
        defw    $1cde           ; fetch a number (or 0 if none)
.l10d7  cp      a               ; set zero flag for classes $0c & $0d
.l10d8  pop     bc              ; drop the scan-loop return address
        call    z,l10b1         ; for classes $0c,$0d check for statement end
        ex      de,hl           ; save line pointer in DE
        ld      hl,(T_ADDR)     ; get address in parameter table
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=command address
        ex      de,hl           ; restore line pointer in HL
        push    bc              ; stack command address
        ret                     ; and "return" to it

; Class $00,$03,$05 routines
; Enter at l10e6 for $03, l10e9 for $00 and l10ea for $05
        
.l10e6  rst     $28
        defw    $1cde           ; fetch a number (or 0 if none)
.l10e9  cp      a               ; set zero flag for classes $00 & $03
.l10ea  pop     bc              ; drop the scan-loop return address
        call    z,l10b1         ; for classes $00,$03 check for statement end
        ex      de,hl           ; save line pointer in DE
        ld      hl,(T_ADDR)     ; get address in parameter table
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=command address in ROM 3
        ex      de,hl           ; restore line pointer
        push    hl              ; and stack it
        ld      hl,l110b        ; place ROM 1 return address in RETADDR
        ld      (RETADDR),hl
        ld      hl,REGNUOY
        ex      (sp),hl         ; place REGNUOY routine as return address
        push    hl
        ld      h,b
        ld      l,c
        ex      (sp),hl         ; stack ROM 3 address, restore line pointer
        push    af              ; stack registers
        push    bc              
        di                      ; disable interrupts
        jp      STOO            ; call ROM 3 routine
.l110b  ret                     ; done

; Class $01 routine

.l110c  rst     $28
        defw    $1c1f           ; use ROM 3 to deal with class $01
        ret     

; Class $02 routine

.l1110  pop     bc              ; drop scan-loop return address
        rst     $28
        defw    $1c56           ; fetch a value
        call    l10b1           ; check for end of statement
        ret     

; Class $04 routine

.l1118  rst     $28
        defw    $1c6c           ; use ROM 3 to deal with class $04
        ret     

; Class $08 routine

        rst     $20
.l111d  rst     $28             ; use ROM 3 to deal with class $08
        defw    $1c7a
        ret     

; Class $06 routine

.l1121  rst     $28
        defw    $1c82           ; use ROM 3 to deal with class $06
        ret     

; Generate C - Nonsense in BASIC error

.l1125  call    l2ada
        defb    $0b             ; error C

; Class $0a routine

.l1129  rst     $28
        defw    $1c8c           ; use ROM 3 to deal with class $0a
        ret     
        
; Class $07 routine

.l112d  bit     7,(iy+$01)      ; are we running or checking syntax?
        res     0,(iy+$02)      ; signal "main screen"
        jr      z,l113a
        rst     $28
        defw    $0d4d           ; if running, make sure temp colours are used
.l113a  pop     af              ; drop scan-loop return address
        ld      a,(T_ADDR)
        sub     l0f8e~$ff+$28   ; form token code INK to OVER
        rst     $28
        defw    $21fc           ; change temporary colours as directed
        call    l10b1           ; check for statement end
        ld      hl,(ATTR_T)
        ld      (ATTR_P),hl     ; make temporary colours permanent
        ld      hl,P_FLAG       ; now copy even bits of P_FLAG to odd bits
        ld      a,(hl)
        rlca    
        xor     (hl)
        and     $aa
        xor     (hl)
        ld      (hl),a
        ret     

; Class $09 routine

.l1157  rst     $28
        defw    $1cbe           ; use ROM 3 to handle class $09
        ret     

; Class $0b routine

.l115b  jp      l0888           ; jump to cassette/disk handling routines


; The IF command

.l115e  pop     bc              ; drop return address to STMT-RET
        bit     7,(iy+$01)
        jr      z,l1175         ; move on if syntax-checking
        ld      hl,(STKEND)     ; "delete" item on calculator stack
        ld      de,$fffb
        add     hl,de
        ld      (STKEND),hl
        rst     $28
        defw    $34e9           ; call "test zero" with HL holding add of value
        jp      c,l1073         ; if false, go to next line
.l1175  jp      l0fd1           ; if true or syntax-checking, do next statement

; The FOR command

.l1178  cp      $cd
        jr      nz,l1185        ; move on if no STEP
        rst     $20             ; advance CH_ADD
        call    l1121           ; fetch step value
        call    l10b1           ; check end of statement if syntax-checking
        jr      l119d           ; else move on
.l1185  call    l10b1           ; if no STEP, check end of statement
        ld      hl,(STKEND)     ; and stack value "1"
        ld      (hl),$00
        inc     hl
        ld      (hl),$00
        inc     hl
        ld      (hl),$01
        inc     hl
        ld      (hl),$00
        inc     hl
        ld      (hl),$00
        inc     hl
        ld      (STKEND),hl
.l119d  rst     $28
        defw    $1d16           ; use ROM 3 to perform command
        ret     


; The READ command (enter at l11a2)

.l11a1  rst     $20             ; move along statement
.l11a2  call    l110c           ; check for existing variable
        bit     7,(iy+$01)
        jr      z,l11d9         ; move on if syntax-checking
        rst     $18             ; save current CH_ADD in X_PTR
        ld      (X_PTR),hl
        ld      hl,(DATADD)     ; fetch data list pointer
        ld      a,(hl)
        cp      ','
        jr      z,l11c2         ; move on unless new statement must be found
        ld      e,$e4
        rst     $28
        defw    $1d86           ; search for "DATA" statement
        jr      nc,l11c2
        call    l2ada
        defb    $0d             ; error E - out of data if not found
.l11c2  inc     hl              ; advance pointer
        ld      (CH_ADD),hl
        ld      a,(hl)
        rst     $28
        defw    $1c56           ; assign value to variable
        rst     $18
        ld      (DATADD),hl     ; store CH_ADD as data pointer
        ld      hl,(X_PTR)      ; get pointer to READ statement
        ld      (iy+$26),$00    ; clear high byte of X_PTR
        ld      (CH_ADD),hl
        ld      a,(hl)          ; get next READ statement character
.l11d9  rst     $18
        cp      ','
        jr      z,l11a1         ; loop back if more items
        call    l10b1           ; check for statement end
        ret     
        
; The DATA command

.l11e2  bit     7,(iy+$01)
        jr      nz,l11f3        ; move on if not syntax-checking
.l11e8  rst     $28
        defw    $24fb           ; scan next expression
        cp      ','
        call    nz,l10b1        ; if no more items, check for statement end
        rst     $20
        jr      l11e8           ; loop back for more
.l11f3  ld      a,$e4           ; we're passing by a DATA statement

; Subroutine to pass by a DEF FN or DATA statement during run-time

.l11f5  rst     $28
        defw    $1e39           ; use ROM 3 routine
        ret

; The RUN command

.l11f9  rst     $28
        defw    $1e67           ; set NEWPPC as required
        ld      bc,$0000
        rst     $28
        defw    $1e45           ; do a RESTORE 0
        jr      l1207           ; exit via CLEAR command

; The CLEAR command

.l1204  rst     $28
        defw    $1e99           ; get operand, use 0 as default
.l1207  ld      a,b
        or      c
        jr      nz,l120f        ; move on if non-zero
        ld      bc,(RAMTOP)     ; use existing RAMTOP if zero
.l120f  push    bc
        ld      de,(VARS)
        ld      hl,(E_LINE)
        dec     hl
        rst     $28
        defw    $19e5           ; reclaim whole variables area
        rst     $28
        defw    $0d6b           ; cls
        ld      hl,(STKEND)
        ld      de,$0032
        add     hl,de
        pop     de
        sbc     hl,de
        jr      nc,l1232        ; move on if ramtop value too low
        ld      hl,(P_RAMT)
        and     a
        sbc     hl,de
        jr      nc,l1236        ; move on if ramtop value not too high
.l1232  call    l2ada
        defb    $15             ; error M - RAMTOP no good
.l1236  ld      (RAMTOP),de     ; store new RAMTOP
        pop     de
        pop     hl
        pop     bc
        ld      sp,(RAMTOP)     ; reset SP
        inc     sp
        push    bc
        push    hl
        ld      (ERR_SP),sp     ; reset ERR_SP
        push    de
        ret     

; The GOSUB command

.l124a  pop     de              ; save STMT_RET address
        ld      h,(iy+$0d)
        inc     h               ; increment SUBPPC statement number
        ex      (sp),hl         ; exchange error address with statement number
        inc     sp              ; reclaim a location
        ld      bc,(PPC)
        push    bc              ; save line number
        push    hl              ; restack error address
        ld      (ERR_SP),sp     ; reset ERR_SP to error address
        push    de              ; restack STMT_RET address
        rst     $28
        defw    $1e67           ; set NEWPPC & NSPPC to required values
        ld      bc,$0014
        rst     $28
        defw    $1f05           ; test for room before making jump
        ret     
        
; The RETURN command

.l1266  pop     bc              ; get STMT_RET address
        pop     hl              ; get error address
        pop     de              ; get next stack entry
        ld      a,d
        cp      $3e
        jr      z,l127d         ; move on if end of GOSUB stack
        dec     sp              ; full entry only uses 3 bytes
        ex      (sp),hl         ; exchange error address with statement no
        ex      de,hl
        ld      (ERR_SP),sp     ; reset ERR_SP
        push    bc              ; restack STMT_RET
        ld      (NEWPPC),hl     ; store new line
        ld      (iy+$0a),d      ; and statement
        ret     
.l127d  push    de              ; reform stack
        push    hl
        call    l2ada
        defb    $06             ; error 7 - RETURN without GOSUB
        
; The DEF FN command
        
.l1283  bit     7,(iy+$01)
        jr      z,l128e         ; move on if checking syntax
        ld      a,$ce
        jp      l11f5           ; else go to skip DEF FN
.l128e  set     6,(iy+$01)      ; signal "numeric variable" in FLAGS
        rst     $28
        defw    $2c8d           ; check present code is a letter
        jr      nc,l12ad        ; error C if not
        rst     $20             ; get next char
        cp      '$'
        jr      nz,l12a1        ; move on if not a string
        res     6,(iy+$01)      ; signal "string variable" in FLAGS
        rst     $20             ; get next char
.l12a1  cp      '('
        jr      nz,l12e1        ; error if not (
        rst     $20
        cp      ')'
        jr      z,l12ca         ; move on if no parameters
.l12aa  rst     $28
        defw    $2c8d           ; check present code is letter
.l12ad  jp      nc,l1125        ; error if not
        ex      de,hl
        rst     $20
        cp      '$'
        jr      nz,l12b8        ; move on if not string
        ex      de,hl
        rst     $20
.l12b8  ex      de,hl
        ld      bc,$0006
        rst     $28
        defw    $1655           ; make 6 bytes of room after parameter name
        inc     hl
        inc     hl
        ld      (hl),$0e        ; store a number marker in first location
        cp      ','
        jr      nz,l12ca        ; move on if end of parameters
        rst     $20
        jr      l12aa           ; loop back
.l12ca  cp      ')'
        jr      nz,l12e1        ; error if no )
        rst     $20
        cp      '='
        jr      nz,l12e1        ; error if no =
        rst     $20
        ld      a,(FLAGS)
        push    af              ; save nature (number/string) of variable
        rst     $28
        defw    $24fb           ; scan the expression
        pop     af
        xor     (iy+$01)
        and     $40
.l12e1  jp      nz,l1125        ; error if expression not correct type
        call    l10b1           ; check for end of statement
        ret     


; The Loader routine, called from ROM 0

.l12e8  call    l2b89		; page in DOS workspace
        ld      hl,process
        ld      (hl),$ff        ; signal "current process is Loader"
        ld      hl,FLAGS3
        bit     4,(hl)
        jp      z,l13c6		; move on if no disk interface present
        xor     a
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_MESSAGE	; disable ALERT routine
        call    l32ee           ; restore TSTACK
	push	hl
	call	l32b6		; save TSTACK in page 7
	call	l3f00
	defw	DOS_BOOT	; attempt to boot a disk from the boot sector
	call	l32ee		; restore TSTACK
        call    l2b64		; page in normal memory
        rst     $28
        defw    $16b0		; clear editing workspaces
        ld      hl,(E_LINE)
        ld      bc,$0007
        rst     $28
        defw    $1655		; create 7 bytes of space at E_LINE
        ld      hl,l152e
        ld      de,(E_LINE)
        ld      bc,$0007
        ldir    		; copy LOAD "disk" into E_LINE
        ld      hl,(E_LINE)
        ld      (CH_ADD),hl	; set CH_ADD
        call    l22c7		; clear whole display if necessary
        bit     6,(iy+$02)
        jr      nz,l133d        ; move on if shouldn't clear lower screen
        rst     $28
        defw    $0d6e		; clear lower screen
.l133d  res     6,(iy+$02)	; signal "lower screen can be cleared"
        call    l2b89		; page in DOS workspace
        ld      hl,ed_flags
        bit     6,(hl)
        jr      nz,l1356        ; ???
        inc     hl
        ld      a,(hl)
        cp      $00
        jr      nz,l1356        ; ???
        call    l3e80
        defw    $1a8e		; clear bottom 3 lines to editor colours
.l1356  call    l2b64		; page in normal memory
        ld      hl,TV_FLAG
        res     3,(hl)		; signal "mode unchanged"
        ld      a,$19
        sub     (iy+$4f)
        ld      (SCR_CT),a	; set scroll count according to current line
        set     7,(iy+$01)	; signal "execution mode"
        ld      (iy+$0a),$01	; statement 1
        ld      hl,$3e00
        push    hl
        ld      hl,ONERR
        push    hl
        ld      (ERR_SP),sp	; set up error stack
        ld      hl,l1383
        ld      (SYNRET),hl	; error return address
        jp      l1048		; execute the edit line, returning here on error
.l1383  call    l2b89		; page in DOS workspace
        pop     hl		; retrieve old ALERT address
        ld      a,$ff
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_MESSAGE	; re-enable ALERT routine
        call    l32ee           ; restore TSTACK
	call	l2b64		; page in normal memory
        ld      a,(ERR_NR)	; get error code
        bit     7,a
        jp      nz,l25cb	; if OK, exit via standard syntax return
        ld      hl,(PPC)
        ld      de,$fffe
        xor     a
        sbc     hl,de
        ld      a,h
        or      l
        jp      nz,l25cb	; or if error occurred in program
        ld      a,'T'
        ld      (LODDRV),a	; set default load drive to cassette
        ld      hl,TV_FLAG
        set     0,(hl)		; signal "using lower screen"
        ld      hl,l14e2
        call    l1524		; output cassette loader message
        ld      hl,TV_FLAG
        res     0,(hl)		; signal "using main screen"
        set     6,(hl)		; signal "don't clear lower screen"
        jr      l13c9
.l13c6  call    l2b64		; page in normal memory
.l13c9  rst     $28
        defw    $16b0		; clear editing workspaces
        ld      hl,(E_LINE)
        ld      bc,$0003
        rst     $28
        defw    $1655		; make 3 bytes space at E_LINE
        ld      hl,l14df
        ld      de,(E_LINE)
        ld      bc,$0003
        ldir    		; copy LOAD "" command into E_LINE
        ld      hl,(E_LINE)
        ld      (CH_ADD),hl	; set interpretation address
        call    l22c7		; clear whole screen if necessary
        bit     6,(iy+$02)
        jr      nz,l13f3
        rst     $28
        defw    $0d6e		; clear lower screen if necessary
.l13f3  res     6,(iy+$02)	; signal "lower screen can be cleared"
        call    l2b89		; page in DOS workspace
        ld      hl,ed_flags
        bit     6,(hl)
        jr      nz,l140c	; ???
        inc     hl
        ld      a,(hl)
        cp      $00
        jr      nz,l140c	; ???
        call    l3e80
        defw    $1a8e		; clear bottom 3 lines to editor colours
.l140c  call    l2b64		; page in normal memory
        ld      hl,TV_FLAG
        res     3,(hl)		; signal "mode unchanged"
        ld      a,$19
        sub     (iy+$4f)
        ld      (SCR_CT),a	; set scroll count according to current line
        set     7,(iy+$01)	; signal "execution mode"
        ld      (iy+$0a),$01	; set statement 1
        ld      hl,$3e00
        push    hl
        ld      hl,ONERR
        push    hl
        ld      (ERR_SP),sp	; set up error stack
        ld      hl,l1439
        ld      (SYNRET),hl	; set error return address
        jp      l1048		; execute line in editing area
.l1439  ld      a,(ERR_NR)	; get error code
        bit     7,a
        jp      nz,l25cb	; exit via standard error return if OK
        ld      hl,FLAGS3
        bit     4,(hl)
        jp      z,l25cb		; or if no disk interface present
        ld      a,'A'
        ld      (LODDRV),a	; else set default load drive to A:
        jp      l25cb		; and exit

; The Print option, called from ROM 0

.l1451  rst     $28
        defw    $16b0		; clear E_LINE, WORKSP, STKBOT etc
        ld      hl,(E_LINE)
        ld      bc,$0001
        rst     $28
        defw    $1655		; make a byte of space at E_LINE
        ld      hl,(E_LINE)
        ld      (hl),$e1	; insert LLIST command
        call    l24f0           ; execute it

; The SPECTRUM command

.l1465  call    l14c4		; set "P" channel to use screen
        ld      sp,(ERR_SP)
        pop     hl		; discard current error return address
        ld      hl,$1303
        push    hl		; address to enter ROM 3 at, in main loop
        ld      hl,$0013	; "AF"
        push    hl
        ld      hl,$0008	; "BC"
        push    hl
        ld      a,$20
        ld      (BANKM),a	; page 0, paging disabled
        push    af
        push    bc
        di      
        res     4,(iy+$01)	; signal "48K BASIC" mode
        jp      STOO		; enter 48K ROM

; The routine to enter 48K BASIC, called from ROM 0

.l1488  ld      hl,$6000
        push    hl		; stack address to return to (in RAM)
        ld      de,$6000
        ld      hl,$14b5
        ld      bc,$000f
        ldir    		; copy following routine into RAM
        ld      a,(BANK678)
        res     3,a		; turn off disk motor
        ld      bc,$1ffd
        di      
        ld      (BANK678),a
        out     (c),a
        ei      
        ld      a,$30		; select ROM 3 with paging disabled
        di      
        res     4,(iy+$01)	; signal "48K BASIC" mode
        ld      (BANKM),a
        push    af
        push    bc
        jp      STOO		; page in ROM 3 & jump to following routine

.l14b5  ld      a,$30
        ld      bc,$7ffd
        di      
        out     (c),a		; page in ROM 3 & disable paging
        ei      
        jp      $0000		; reset machine with 48K ROM locked in

; Cliff J Lawson's initials

	defm	"CJL"

; Subroutine to copy screen channel addresses over printer channel addresses

.l14c4  ld      hl,(CHANS)
        ld      de,$0005
        add     hl,de		; HL=channel info for "S"
        ld      de,$000a
        ex      de,hl
        add     hl,de
        ex      de,hl		; DE=channel info for "P"
        ld      bc,$0004
        ldir    		; overwrite it
        res     3,(iy+$30)	; set caps lock off
        res     4,(iy+$01)	; signal "48K BASIC" mode
        ret     

; LOAD "" command

.l14df	defb	$ef,$22,$22
        
; The Loader message
    
.l14e2	defm	$16&$00&$00
.l14e5  defm    $10&$00&$11&$07&$13&$00
        defm    "Insert tape and press PLAY"&$0d
        defm    "To cancel - press BREAK twice"&$ff


; Subroutine to output a $ff-terminated message

.l1524  ld      a,(hl)		; get next char
        cp      $ff
        ret     z		; exit if end
        rst     $28
        defw    $0010		; output
        inc     hl
        jr      l1524		; loop back

; LOAD "disk" command

.l152e	defb	$ef		; LOAD keyword
.l152f  defm    $22&"disk"&$22

; The LIST & LLIST commands (enter at l1535 for LLIST, l1539 for LIST)

.l1535  ld      a,$03           ; default stream 3 for LLIST
        jr      l153b
.l1539  ld      a,$02           ; default stream 2 for LIST
.l153b  ld      (iy+$02),$00    ; signal ordinary listing in main screen
.l153f  rst     $28
        defw    $2530           ; are we checking syntax?
        jr      z,l1547
        rst     $28
        defw    $1601           ; open channel if not
.l1547  rst     $28
        defw    $0018           ; get character
        rst     $28
        defw    $2070           ; see if stream must be altered
        jr      c,l1567         ; move on if not
        rst     $28
        defw    $0018           ; get character
        cp      ';'
        jr      z,l155a         ; move on if ;
        cp      ','
        jr      nz,l1562        ; move on if not ,
.l155a  rst     $28
        defw    $0020           ; get next character
.l155d  call    l1121           ; get numeric expression
        jr      l156a           ; move on with line number to list from
.l1562  rst     $28             
        defw    $1ce6           ; else use zero
        jr      l156a            
.l1567  rst     $28
        defw    $1cde           ; fetch a numeric expression or use zero
.l156a  call    l10b1           ; check for end of statement
.l156d  rst     $28
        defw    $1825           ; use ROM 3 for actual listing operation
        ret

; PLAY command (enters here after syntax-checking)

.l1571  di
        push    bc              ; save count of strings
        ld      de,$0037        ; $37 bytes required per string
        ld      hl,$003c        ; plus $3c bytes overhead
.l1579  add     hl,de
        djnz    l1579
        ld      c,l
        ld      b,h             ; BC=workspace required
        rst     $28
        defw    $0030           ; make space
        di
        push    de
.l1583  pop     iy              ; IY=start of space
        push    hl		; stack HL=end of space+1
.l1586  pop     ix
        ld      (iy+$10),$ff
.l158c  ld      bc,$ffc9
        add     ix,bc           ; IX=start of next string parameter space
        ld      (ix+$03),$3c
        ld      (ix+$01),$ff
        ld      (ix+$04),$0f
        ld      (ix+$05),$05
        ld      (ix+$21),$00
        ld      (ix+$0a),$00
        ld      (ix+$0b),$00
        ld      (ix+$16),$ff
        ld      (ix+$17),$00
        ld      (ix+$18),$00
        rst     $28
        defw    $2bf1           ; get string from top of stack
        di      
        ld      (ix+$06),e
        ld      (ix+$07),d      ; store address of string (twice)
        ld      (ix+$0c),e
        ld      (ix+$0d),d
        ex      de,hl
        add     hl,bc
        ld      (ix+$08),l
        ld      (ix+$09),h      ; store address of end-of-string+1
        pop     bc              ; restore count of strings
        push    bc
        dec     b
        ld      c,b
        ld      b,$00           ; BC=number of strings left on stack
        sla     c
        push    iy
        pop     hl
        add     hl,bc		; HL=overheadspace+2*(string number-1)
        push    ix
        pop     bc
        ld      (hl),c
        inc     hl
        ld      (hl),b          ; store string parameter block address
        or      a
        rl      (iy+$10)	; shift in 0 bit for each string
        pop     bc
        dec     b		; decrement string count
        push    bc
        ld      (ix+$02),b	; store string number (0...7) in parameters
        jr      nz,l158c        ; back for another string
        pop     bc		; restore count of strings
        ld      (iy+$27),$1a
        ld      (iy+$28),$0b
        push    iy
        pop     hl
        ld      bc,$002b
        add     hl,bc
        ex      de,hl
        ld      hl,l161d
        ld      bc,$000d
        ldir                    ; copy FP routine in
        ld      d,$07
        ld      e,$f8
        call    l1a6a		; output $f8 to AY register 7
        ld      d,$0b
        ld      e,$ff
        call    l1a6a		; output $ff to AY register 11
        inc     d
        call    l1a6a		; output $ff to AY register 12
        jr      l1669		; move on

; FP routine used to calculate tempo values, executed in RAM with ROM 3
; paged in

.l161d	rst	$28		; engage FP-calculator
	defb	stk_ten		; X,10
	defb	exchange	; 10,X
	defb	division	; 10/X
	defb	stk_data	; 10/X,Y
	defb	$df,$75,$f4,$38,$75
	defb	division	; 10/(X*Y)
	defb	end_calc
	ret

; Subroutine to check if BREAK is being pressed (exit with carry reset if so)

.l162a  ld      a,$7f
        in      a,($fe)
        rra     
        ret     c               ; exit with carry set if not pressed
        ld      a,$fe
        in      a,($fe)
        rra                     ; test other key & exit
        ret     

; Subroutine to initialise string pointers to first string (l163b)
 
.l1636  ld      bc,$0011
        jr      l163e
.l163b  ld      bc,$0000
.l163e  push    iy
        pop     hl
        add     hl,bc		; get address of pointer to string
        ld      (iy+$23),l
        ld      (iy+$24),h	; store address
        ld      a,(iy+$10)
        ld      (iy+$22),a	; copy available strings byte
        ld      (iy+$21),$01	; set string bit marker
        ret     

; Subroutine to get address of current string parameter block in IX

.l1653  ld      e,(hl)
        inc     hl
        ld      d,(hl)
        push    de
        pop     ix
        ret     

; Subroutine to increment pointer to next string parameter block address

.l165a  ld      l,(iy+$23)
        ld      h,(iy+$24)	; get current parameter block pointer address
        inc     hl
        inc     hl		; move to next
        ld      (iy+$23),l
        ld      (iy+$24),h	; store
        ret     

; More PLAY command

.l1669  call    l163b		; copy initial info
.l166c  rr      (iy+$22)	; rotate string counter
        jr      c,l1678		; move on if no string
        call    l1653		; get IX=address of current string parm block
        call    l1748           ; interpret string for standard parms
.l1678  sla     (iy+$21)
        jr      c,l1683		; move on if tried 8 strings
        call    l165a		; increment pointer to string parms address
        jr      l166c		; loop back
.l1683  call    l1b7f           ; find shortest current notelength
        push    de
        call    l1b30           ; output next note from each string
        pop     de
.l168b  ld      a,(iy+$10)
        cp      $ff
        jr      nz,l1697        ; move on unless no strings,or stop encountered
        call    l1a81           ; close down AY channels for this command
        ei      
        ret                     ; exit
.l1697  dec     de
        call    l1b64           ; pause
        call    l1baf           ; decrement note lengths & change notes if nec
        call    l1b7f           ; find shortest current notelength
        jr      l168b           ; loop back

; List of PLAY string parameters

.l16a3	defm	"HZYXWUVMT)(NO!"

; Subroutine to get next character from string and increment string
; interpretation address (carry set if no char available)

.l16b1  call    l1ad1           ; get next character
        ret     c               ; exit if end of string
        inc     (ix+$06)        ; increment string interpretation address
        ret     nz
        inc     (ix+$07)
        ret

; Subroutine to get a note from the string, returning semitone value A (or 0)
; If A=$80, a rest was found

.l16bd  push    hl
        ld      c,$00           ; C=initial semitone value (natural)
.l16c0  call    l16b1           ; get next char from string
        jr      c,l16cd         ; move on if none
        cp      '&'
        jr      nz,l16d8        ; move on if not a rest
        ld      a,$80           ; signal "rest"
.l16cb  pop     hl
        ret     
.l16cd  ld      a,(iy+$21)
        or      (iy+$10)
        ld      (iy+$10),a      ; set string to not in use & exit
        jr      l16cb
.l16d8  cp      '#'             ; test for sharp sign
        jr      nz,l16df
        inc     c               ; if so, increment semitone value & loop back
        jr      l16c0
.l16df  cp      '$'             ; test for flat sign
        jr      nz,l16e6
        dec     c               ; if so, decrement semitone value & loop back
        jr      l16c0
.l16e6  bit     5,a
        jr      nz,l16f0        ; move on if lowercase letter
        push    af
        ld      a,$0c
        add     a,c             ; for uppercase, add octave to semitone value
        ld      c,a
        pop     af
.l16f0  and     $df             ; make uppercase
        sub     'A'
        jp      c,l1b10         ; error k if <A
        cp      $07
        jp      nc,l1b10        ; or if >G
        push    bc
.l16fd  ld      b,$00
        ld      c,a
        ld      hl,l19e7
        add     hl,bc
        ld      a,(hl)          ; get note semitone value
        pop     bc
        add     a,c             ; add octave/sharp/flat value
        pop     hl
        ret     

; Subroutine to get a numeric value from the string into BC (defaults to 0)

.l1709  push    hl
        push    de
        ld      l,(ix+$06)
        ld      h,(ix+$07)      ; get string interpretation address
        ld      de,$0000        ; initial value 0
.l1714  ld      a,(hl)          ; get next char
        cp      '0'
        jr      c,l1731
        cp      '9'+1
        jr      nc,l1731        ; move on if not a digit
        inc     hl
        push    hl
        call    l173c           ; multiply current value by 10
        sub     '0'
        ld      h,$00
        ld      l,a
        add     hl,de           ; add in digit
        jr      c,l172e         ; jump if overflow
        ex      de,hl
        pop     hl
        jr      l1714           ; back for more digits
.l172e  jp      l1b08           ; error l - number too big
.l1731  ld      (ix+$06),l
        ld      (ix+$07),h      ; replace updated interpretation address
        push    de
        pop     bc              ; BC=value
        pop     de              ; restore registers
        pop     hl
        ret

; Subroutine to multiply DE by 10

.l173c  ld      hl,$0000        ; start with zero
        ld      b,$0a
.l1741  add     hl,de           ; add DE 10 times
        jr      c,l172e         ; jump if overflow
        djnz    l1741
        ex      de,hl
        ret     

; Subroutine to interpret a string

.l1748  call    l162a		; check for break
        jr      c,l1755		; move on if not pressed
        call    l1a81           ; close down channels for this command
        ei			; re-enable interrupts
        call    l2ada
        defb	20		; error L - BREAK into program
.l1755  call    l16b1           ; get next character of string
        jp      c,l1990         ; move on if no more available
        call    l19de           ; search for char in parameter list
        ld      b,$00
        sla     c
        ld      hl,l19b8
        add     hl,bc           ; form pointer to routine address
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl           ; HL=routine address
        call    l1770           ; call routine
        jr      l1748           ; loop back for more standard params
        ret			; exit when note length changed, or note found
.l1770  jp      (hl)

; Parameter !: comment

.l1771  call    l16b1           ; get next character from string
        jp      c,l198f         ; move on if end of string
        cp      '!'
        ret     z               ; exit if end of comment
        jr      l1771           ; loop back

; Parameter O: octave

.l177c  call    l1709           ; get number from string
        ld      a,c
        cp      $09
        jp      nc,l1b00        ; error n if not 1-8
        sla     a
        sla     a
        ld      b,a
        sla     a
        add     a,b
        ld      (ix+$03),a      ; store base note number (12*octave)
        ret     

; Parameter N: number separator

.l1791  ret                     ; ignore

; Parameter (: start of repeat section

.l1792  ld      a,(ix+$0b)      ; get current bracket depth
        inc     a               ; increment
        cp      $05
        jp      z,l1b18         ; if depth now 5, cause error d
        ld      (ix+$0b),a      ; store new depth
        ld      de,$000c        ; offset for bracket addresses
        call    l1813           ; get pointer to next bracket address down
        ld      a,(ix+$06)
        ld      (hl),a
        inc     hl
        ld      a,(ix+$07)
        ld      (hl),a          ; store address to repeat from
        ret     

; Parameter ): end of repeat section

.l17ae  ld      a,(ix+$16)	; get number of )s encountered so far
        ld      de,$0017	; offset for close bracket addresses
        or      a
        jp      m,l17dc		; move on if none so far
        call    l1813		; get address of current
        ld      a,(ix+$06)
        cp      (hl)
        jr      nz,l17dc
        inc     hl
        ld      a,(ix+$07)
        cp      (hl)
        jr      nz,l17dc	; move on if not the same
        dec     (ix+$16)	; decrement close bracket depth
        ld      a,(ix+$16)
        or      a
        ret     p		; exit if still positive
        bit     0,(ix+$0a)
        ret     z		; exit if not infinite repeat
        ld      (ix+$16),$00	; set no close brackets
        xor     a
        jr      l17f7
.l17dc  ld      a,(ix+$16)
        inc     a		; increment close bracket depth
        cp      $05
        jp      z,l1b18		; error d if depth of 5
        ld      (ix+$16),a	; restore depth
        call    l1813		; get pointer to next close bracket address
        ld      a,(ix+$06)
        ld      (hl),a
        inc     hl
        ld      a,(ix+$07)
        ld      (hl),a		; store address to repeat to
        ld      a,(ix+$0b)	; get current open bracket depth
.l17f7  ld      de,$000c
        call    l1813		; get pointer to address (or string start)
        ld      a,(hl)
        ld      (ix+$06),a
        inc     hl
        ld      a,(hl)
        ld      (ix+$07),a	; reset interpretation address to correct point
        dec     (ix+$0b)	; decrement open bracket depth
        ret     p
        ld      (ix+$0b),$00	; reset to zero if no open brackets
        set     0,(ix+$0a)	; set "infinite repeat"
        ret     

; Subroutine to get HL=Ath word entry after IX+DE
; Used to find address for bracket address entries

.l1813  push    ix
        pop     hl
        add     hl,de           ; add offset to string area
        ld      b,$00
        ld      c,a
        sla     c
        add     hl,bc           ; add offset to Ath word
        ret     

; Parameter T: tempo

.l181e  call    l1709           ; get number from string
        ld      a,b
        or      a
        jp      nz,l1b00        ; error n if >255
        ld      a,c
        cp      $3c
        jp      c,l1b00         ; error n if <60
        cp      $f1
        jp      nc,l1b00        ; error n if >240
        ld      a,(ix+$02)
        or      a
        ret     nz              ; ignore unless in first string
        ld      b,$00
        push    bc
        pop     hl
        add     hl,hl
        add     hl,hl
        push    hl
        pop     bc              ; BC=tempo*4
        push    iy
        rst     $28
        defw    $2d2b           ; stack BC on calculator stack
        di      
        pop     iy
        push    iy
        push    iy
        pop     hl
        ld      bc,$002b	; offset to FP calculation routine
        add     hl,bc
        ld      iy,ERR_NR
        push    hl		; stack FP routine address
        ld      hl,l1864
        ld      (RETADDR),hl	; set up return address
        ld      hl,REGNUOY
        ex      (sp),hl
        push    hl
        push    af
        push    hl
        jp      STOO		; call FP calculator - TOS=10/(tempo*4*val)
.l1864  di      
        rst     $28
        defw    $2da2		; get value to BC
        di      
        pop     iy
        ld      (iy+$27),c
        ld      (iy+$28),b	; store tempo value
        ret     

; Parameter M: channel

.l1872  call    l1709           ; get number from string
        ld      a,c
        cp      $40
        jp      nc,l1b00        ; error n if >63
        cpl     
        ld      e,a
        ld      d,$07
        call    l1a6a           ; output channel complement to AY register 7
        ret     

; Parameter V: volume level

.l1883  call    l1709           ; get number from string
        ld      a,c
        cp      $10
        jp      nc,l1b00        ; error n if >15
        ld      (ix+$04),a      ; store volume level
        ld      e,(ix+$02)
        ld      a,$08
        add     a,e
        ld      d,a             ; AY register=channel+8 (channel=0..7)
        ld      e,c
        call    l1a6a           ; output volume level to register
        ret     

; Parameter U: volume effect in a string

.l189b  ld      e,(ix+$02)
        ld      a,$08
        add     a,e
        ld      d,a             ; AY register=channel+8 (channel=0..7)
        ld      e,$1f
        ld      (ix+$04),e      ; store volume effect marker
        ret     

; Parameter W: volume effect

.l18a8  call    l1709           ; get number from string
        ld      a,c
        cp      $08
        jp      nc,l1b00        ; error n if >7
        ld      b,$00
        ld      hl,l19d6
        add     hl,bc
        ld      a,(hl)          ; get envelope byte
        ld      (iy+$29),a      ; store it
        ret     

; Parameter X: volume duration

.l18bc  call    l1709           ; get number from string
        ld      d,$0b
        ld      e,c
        call    l1a6a           ; output duration to AY registers 11
        inc     d
        ld      e,b
        call    l1a6a           ; and 12
        ret     

; Parameter Y: MIDI channel

.l18cb  call    l1709           ; get number from string
        ld      a,c
        dec     a               ; decrement
        jp      m,l1b00         ; error n if was 0
        cp      $10
        jp      nc,l1b00        ; error n if >15
        ld      (ix+$01),a      ; store channel
        ret     

; Parameter Z: MIDI programming code

.l18dc  call    l1709           ; get number from string
        ld      a,c
        call    l1d91           ; output code to MIDI
        ret

; Parameter H: stop PLAY command
     
.l18e4  ld      (iy+$10),$ff    ; signal "no strings in use"
        ret     

; Notes and other parameters

.l18e9  call    l1a07		; is char a digit? (ie note length)
        jp      c,l196f		; move on if not
        call    l199a           ; get HL=pointer to note lengths for string
        call    l19a2           ; save in overhead area
        xor     a
        ld      (ix+$21),a	; zero number of tied notes
        call    l1ab6		; decrement interpretation address
        call    l1709		; get number from string
        ld      a,c
        or      a
        jp      z,l1b00		; error n if <1
        cp      $0d
        jp      nc,l1b00	; or >12
        cp      $0a
        jr      c,l1920		; move on unless dealing with triplets
        call    l19ee		; get note length value
        call    l1962		; increment number of tied notes
        ld      (hl),e
        inc     hl
        ld      (hl),d		; store note length for first note
.l1916  call    l1962		; increment number of tied notes
        inc     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d		; store note length for next tied note
        inc     hl
        jr      l1926
.l1920  ld      (ix+$05),c	; save new note length
        call    l19ee		; get note length value
.l1926  call    l1962		; increment number of tied notes
.l1929  call    l1ad1		; test next character
        cp      '_'
        jr      nz,l195c	; move on unless tieing notes
        call    l16b1		; get the character
        call    l1709		; get number from string
        ld      a,c
        cp      $0a
        jr      c,l194d		; move on if not triplet
        push    hl
        push    de
        call    l19ee		; get new note length value
        pop     hl
        add     hl,de
        ld      c,e
        ld      b,d		; BC=old note length value+new note length val
        ex      de,hl		; so does DE
        pop     hl		; restore address to store
        ld      (hl),e
        inc     hl
        ld      (hl),d		; store value
        ld      e,c
        ld      d,b
        jr      l1916		; loop back
.l194d  ld      (ix+$05),c	; store new note length
        push    hl
        push    de
        call    l19ee		; get note length value
        pop     hl
        add     hl,de
        ex      de,hl		; DE=old note length val+new note length val
        pop     hl
        jp      l1929		; loop back

; Store note length value & move on

.l195c  ld      (hl),e
        inc     hl
        ld      (hl),d		; store note length value
        jp      l198a		; move on

; Subroutine to increment number of tied notes for a string

.l1962  ld      a,(ix+$21)	; get number of tied notes
        inc     a		; increment
.l1966  cp      $0b
        jp      z,l1b28		; error o - too many tied notes
        ld      (ix+$21),a
        ret     

; Notes and other parameters (continued)

.l196f  call    l1ab6		; decrement string interpretation pointer
        ld      (ix+$21),$01	; set 1 tied note
        call    l199a           ; get pointer to note lengths for string
        call    l19a2           ; save in overhead area
        ld      c,(ix+$05)	; get current note length
        push    hl
        call    l19ee		; calc note length value
        pop     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d		; store it
        jp      l198a
.l198a  pop     hl		; retrieve return address
.l198b  inc     hl
        inc     hl		; move on by two
        push    hl		; restack
        ret     		; return

; Subroutine to set current string to "finished"

.l198f  pop     hl              ; discard return address
.l1990  ld      a,(iy+$21)      ; get string mask bit
        or      (iy+$10)
        ld      (iy+$10),a      ; place into strings counter
        ret     

; Subroutine to set HL=pointer to note lengths for current string

.l199a  push    ix
        pop     hl
        ld      bc,$0022
        add     hl,bc
        ret     

; Subroutine to save note lengths pointer of string in overhead area

.l19a2  push    hl              ; save note lengths pointer
        push    iy
        pop     hl
        ld      bc,$0011
        add     hl,bc		; HL=overhead area+$11
        ld      b,$00
        ld      c,(ix+$02)
.l19af  sla     c
        add     hl,bc		; HL=overhead area+$11+(string*2)
        pop     de
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; store note lengths pointer
        ex      de,hl           ; restore HL=note lengths pointer
        ret     

; Table of routine addresses for elements of PLAY strings

.l19b8  defw    l18e9           ; note or other parameter
	defw	l1771		; Z
	defw	l177c		; Y
	defw	l1791		; X
	defw	l1792		; W
	defw	l17ae		; U
	defw	l181e		; V
	defw	l1872		; M
	defw	l1883		; T
	defw	l189b		; )
	defw	l18a8		; (
	defw	l18bc		; N
	defw	l18cb		; O
	defw	l18dc		; !
        defw    l18e4           ; H

; Table of waveforms for volume effects

.l19d6  defb    $00,$04,$0b,$0d
        defb    $08,$0c,$0e,$0a

; Subroutine to search for string character A in parameter list
; Z set if found

.l19de  ld      bc,$000f
        ld      hl,l16a3
        cpir    
        ret

; Table of note semitone values

.l19e7  defb    $09,$0b,$00,$02,$04,$05,$07

; Subroutine to get note length value (DE) for note length C (1-12)

.l19ee  push    hl
        ld      b,$00
        ld      hl,l19fa	; start of table
        add     hl,bc
        ld      d,$00
        ld      e,(hl)		; DE=note length value
        pop     hl
        ret     

; Table of note length values

.l19fa	defb	$80
	defb	$06,$09,$0c,$12
	defb	$18,$24,$30,$48
	defb	$60,$04,$08,$10

; Subroutine to test if A is a digit (carry reset if so)

.l1a07	cp	'0'
	ret	c
        cp      '9'+1
        ccf     
        ret     

; Subroutine to play note A through AY channel for current string

.l1a0e  ld      c,a             ; save semitone value
        ld      a,(ix+$03)
        add     a,c             ; add in base note value
        cp      $80
        jp      nc,l1b20        ; error m if out of range
        ld      c,a             ; save note value
        ld      a,(ix+$02)
        or      a
        jr      nz,l1a2d        ; move on unless first string
        ld      a,c
        cpl     
        and     $7f
        srl     a
.l1a25  srl     a               
        ld      d,$06
        ld      e,a
        call    l1a6a           ; output value to AY register 6
.l1a2d  ld      (ix+$00),c      ; save last note value
        ld      a,(ix+$02)
        cp      $03
        ret     nc              ; exit unless outputting AY channel 0-2
        ld      hl,l1c84
        ld      b,$00
        ld      a,c
        sub     $15
        jr      nc,l1a45
        ld      de,l0fbf        ; lowest note possible
        jr      l1a4c
.l1a45  ld      c,a
        sla     c               ; form offset into semitone table
        add     hl,bc
        ld      e,(hl)
        inc     hl
.l1a4b  ld      d,(hl)          ; get DE=note value
.l1a4c  ex      de,hl
        ld      d,(ix+$02)
        sla     d               ; AY register=0,2 or 4
        ld      e,l
        call    l1a6a           ; output low byte of note value
        inc     d               ; AY register=1,3 or 5
        ld      e,h
        call    l1a6a           ; output high byte of note value
        bit     4,(ix+$04)
        ret     z               ; exit unless envelope to be used
        ld      d,$0d
        ld      a,(iy+$29)
        ld      e,a
        call    l1a6a           ; output waveform number to AY register 13
        ret     

; Subroutine to output value E to sound register D

.l1a6a  push    bc
        ld      bc,$fffd
        out     (c),d		; select register D
        ld      bc,$bffd
        out     (c),e		; output value E
        pop     bc
        ret     

; Subroutine to get value of AY register A
        
.l1a77  push    bc
        ld      bc,$fffd
        out     (c),a           ; select register
        in      a,(c)           ; get value
        pop     bc
        ret     

; Subroutine to close down AY channels associated with this PLAY command

.l1a81  ld      d,$07
        ld      e,$ff
        call    l1a6a		; output $ff to AY register 7
        ld      d,$08
        ld      e,$00
        call    l1a6a		; output 0 to AY register 8
        inc     d
        call    l1a6a		; output 0 to AY register 9
        inc     d
        call    l1a6a		; output 0 to AY register 10
        call    l163b		; initialise string pointer info
.l1a9a  rr      (iy+$22)	; test for string
        jr      c,l1aa6		; move on if none
        call    l1653		; get IX=address of string parameter block
        call    l1d7b           ; output terminator if MIDI channel
.l1aa6  sla     (iy+$21)
        jr      c,l1ab1		; move on when 8 strings tested for
        call    l165a		; increment pointer to next string block
        jr      l1a9a		; loop back
.l1ab1  ld      iy,ERR_NR       ; reset IY to system variables
        ret                     ; done

; Subroutine to decrement string interpretation pointer (skipping white space)

.l1ab6  push    hl
        push    de
        ld      l,(ix+$06)
        ld      h,(ix+$07)	; get current pointer
.l1abe  dec     hl		; decrement
        ld      a,(hl)
        cp      $20
        jr      z,l1abe
        cp      $0d
        jr      z,l1abe		; loop back while on white space
        ld      (ix+$06),l
        ld      (ix+$07),h	; store updated pointer
        pop     de
        pop     hl
        ret     

; Subroutine to get next character from string in A, skipping any white space
; Carry set on exit if end of string reached

.l1ad1  push    hl
        push    de
        push    bc
        ld      l,(ix+$06)
        ld      h,(ix+$07)      ; get HL=string interpretation address
.l1ada  ld      a,h
        cp      (ix+$09)        ; compare against string end address
        jr      nz,l1ae9
        ld      a,l
        cp      (ix+$08)
        jr      nz,l1ae9
        scf                     ; set carry if end of string
        jr      l1af3
.l1ae9  ld      a,(hl)
        cp      ' '
        jr      z,l1af7         ; move to skip any spaces
        cp      $0d
        jr      z,l1af7         ; or CRs
        or      a               ; reset carry
.l1af3  pop     bc
        pop     de
        pop     hl
.l1af6  ret
.l1af7  inc     hl              ; increment string interpretation address
        ld      (ix+$06),l
        ld      (ix+$07),h
        jr      l1ada           ; loop back

; Error routines for PLAY

.l1b00  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    41              ; error n - Out of range
.l1b08  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    39              ; error l - Number too big
.l1b10  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    38              ; error k - Invalid note name
.l1b18  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    31              ; error d - Too many brackets
.l1b20  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    40              ; error m - Note out of range
.l1b28  call    l1a81           ; close down
        ei      
        call    l2ada
        defb    42              ; error o - Too many tied notes

; Subroutine to output next note from each string

.l1b30  call    l163b           ; initialise string pointer info
.l1b33  rr      (iy+$22)        ; test for next string
        jr      c,l1b5a         ; move on if not present
        call    l1653           ; get address of string parameter block to IX
        call    l16bd           ; get note from string
        cp      $80
        jr      z,l1b5a         ; move on if rest found
        call    l1a0e           ; calculate semitone & play if string 0-2
        ld      a,(ix+$02)
        cp      $03
        jr      nc,l1b57        ; move on if strings 3-7
        ld      d,$08
        add     a,d
        ld      d,a
        ld      e,(ix+$04)
        call    l1a6a           ; output volume level to AY register 8+channel
.l1b57  call    l1d5c           ; output semitone to MIDI channels
.l1b5a  sla     (iy+$21)
        ret     c               ; exit when 8 strings done
        call    l165a           ; get to next string parameter block
        jr      l1b33           ; loop back

; Subroutine to pause for current notelength (DE)

.l1b64  push    hl
        ld      l,(iy+$27)
        ld      h,(iy+$28)      ; HL=tempo value
        ld      bc,$0064
        or      a
        sbc     hl,bc
        push    hl
        pop     bc
        pop     hl
.l1b74  dec     bc
        ld      a,b
        or      c
        jr      nz,l1b74        ; timing delay
        dec     de
        ld      a,d
        or      e
        jr      nz,l1b64        ; loop DE times
        ret     

; Subroutine to find shortest cuurent note across all strings

.l1b7f  ld      de,$ffff        ; largest notelength so far (-1)
        call    l1636           ; initialise pointers to first string
.l1b85  rr      (iy+$22)	; test for next string
        jr      c,l1b9d         ; move on if not present
        push    de
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; get notelength pointer
        ex      de,hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; get note length
        push    de
        pop     hl
        pop     bc
        or      a
        sbc     hl,bc
        jr      c,l1b9d         ; move on if already found smaller
        push    bc
        pop     de              ; keep current
.l1b9d  sla     (iy+$21)        ; shift string bit marker
        jr      c,l1ba8         ; move on if done 8 strings
        call    l165a           ; increment pointer to next strings pointer
        jr      l1b85           ; loop back
.l1ba8  ld      (iy+$25),e
        ld      (iy+$26),d      ; store shortest current note length
        ret     

; Subroutine to decrement remaining note lengths for each string, changing
; notes if necessary

.l1baf  xor     a
        ld      (iy+$2a),a      ; set no strings have changed notes
        call    l163b           ; initialise string pointers
.l1bb6  rr      (iy+$22)
        jp      c,l1c48         ; move on if string not present
        call    l1653           ; get address of current string parameter block
        push    iy
        pop     hl
        ld      bc,$0011
        add     hl,bc
        ld      b,$00
        ld      c,(ix+$02)
        sla     c
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl
        push    hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl           ; HL=notelength for this string
        ld      e,(iy+$25)
        ld      d,(iy+$26)      ; DE=length to play
        or      a
        sbc     hl,de
        ex      de,hl
        pop     hl
        jr      z,l1bea         ; move on if same length
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; else store remaining length
        jr      l1c48           ; and move on
.l1bea  ld      a,(ix+$02)
        cp      $03
        jr      nc,l1bfa        ; move on if MIDI channel
        ld      d,$08
        add     a,d
        ld      d,a             ; select AY register 8+channel
        ld      e,$00
        call    l1a6a           ; output 0 to register
.l1bfa  call    l1d7b           ; output terminator if MIDI channel
        push    ix
        pop     hl
        ld      bc,$0021
        add     hl,bc
        dec     (hl)            ; decrement number of tied notes
        jr      nz,l1c14        ; move on if still some left
        call    l1748           ; interpret string for parameters
        ld      a,(iy+$21)
        and     (iy+$10)
        jr      nz,l1c48        ; move on if string no longer in use
        jr      l1c2b           ; move on
.l1c14  push    iy
        pop     hl
        ld      bc,$0011
        add     hl,bc
        ld      b,$00
        ld      c,(ix+$02)
        sla     c
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        inc     de
        inc     de
        ld      (hl),d
        dec     hl
        ld      (hl),e          ; store pointer to next tied note length
.l1c2b  call    l16bd           ; get note from string
        ld      c,a
        ld      a,(iy+$21)
        and     (iy+$10)
        jr      nz,l1c48        ; move on if string no longer in use
        ld      a,c
        cp      $80
        jr      z,l1c48         ; move on if rest found
        call    l1a0e           ; play note through channel
        ld      a,(iy+$21)
        or      (iy+$2a)
        ld      (iy+$2a),a      ; signal "note changed for this string"
.l1c48  sla     (iy+$21)
        jr      c,l1c54         ; move on if no more strings
        call    l165a           ; get to next string parameter block
        jp      l1bb6           ; loop back
.l1c54  ld      de,$0001
        call    l1b64           ; pause
        call    l163b           ; initialise pointers to first string
.l1c5d  rr      (iy+$2a)
        jr      nc,l1c7a        ; move on if note didn't change
        call    l1653           ; get pointer to string parameter block
        ld      a,(ix+$02)
        cp      $03
        jr      nc,l1c77        ; move on if MIDI channel
        ld      d,$08
        add     a,d
        ld      d,a
        ld      e,(ix+$04)
        call    l1a6a           ; output volume to AY register 8+channel
.l1c77  call    l1d5c           ; output semitone to MIDI channel
.l1c7a  sla     (iy+$21)
        ret     c               ; exit if 8 strings done
        call    l165a           ; move to next string parameter block
        jr      l1c5d           ; loop back

; The semitone table of note values

.l1c84  defw    $0fbf,$0edc,$0e07,$0d3d
        defw    $0c7f,$0bcc,$0b22,$0a82
        defw    $09eb,$095d,$08d6,$0857
        defw    $07df,$076e,$0703,$069f
        defw    $0640,$05e6,$0591,$0541
        defw    $04f6,$04ae,$046b,$042c
        defw    $03f0,$03b7,$0382,$034f
        defw    $0320,$02f3,$02c8,$02a1
        defw    $027b,$0257,$0236,$0216
        defw    $01f8,$01dc,$01c1,$01a8
        defw    $0190,$0179,$0164,$0150
        defw    $013d,$012c,$011b,$010b
        defw    $00fc,$00ee,$00e0,$00d4
        defw    $00c8,$00bd,$00b2,$00a8
        defw    $009f,$0096,$008d,$0085
        defw    $007e,$0077,$0070,$006a
        defw    $0064,$005e,$0059,$0054
        defw    $004f,$004b,$0047,$0043
        defw    $003f,$003b,$0038,$0035
        defw    $0032,$002f,$002d,$002a
        defw    $0028,$0025,$0023,$0021
        defw    $001f,$001e,$001c,$001a
        defw    $0019,$0018,$0016,$0015
        defw    $0014,$0013,$0012,$0011
        defw    $0010,$000f,$000e,$000d
        defw    $000c,$000c,$000b,$000b
        defw    $000a,$0009,$0009,$0008

; Subroutine to output a semitone if a MIDI channel

.l1d5c  ld      a,(ix+$01)
        or      a
        ret     m               ; exit if not a MIDI channel
        or      $90
        call    l1d91           ; output channel selector to MIDI
        ld      a,(ix+$00)
        call    l1d91           ; output last semitone value
        ld      a,(ix+$04)
        res     4,a             ; ignore waveform flag
        sla     a
        sla     a
        sla     a
        call    l1d91           ; output volume to MIDI
        ret     

; Subroutine to output terminator to MIDI channel

.l1d7b  ld      a,(ix+$01)
        or      a
        ret     m               ; exit if not a MIDI channel
        or      $80
        call    l1d91           ; output channel selector to MIDI
        ld      a,(ix+$00)
        call    l1d91           ; output semitone to MIDI
        ld      a,$40
        call    l1d91           ; output terminator to MIDI
        ret     

; Subroutine to output a value (A) to the MIDI port (uses AUX)

.l1d91  ld      l,a             ; save value
        ld      bc,$fffd
        ld      a,$0e
        out     (c),a           ; select AY register 14 (RS232/AUX)
        ld      bc,$bffd
        ld      a,$fa
        out     (c),a           ; output data low to AUX
        ld      e,$03
.l1da2  dec     e
        jr      nz,l1da2        ; delay loop
        nop     
        nop     
        nop     
        nop     
        ld      a,l
        ld      d,$08           ; 8 bits to output
.l1dac  rra                     ; get next LSB of value to carry
        ld      l,a
        jp      nc,l1db7
        ld      a,$fe
        out     (c),a           ; if set, output data high to AUX
        jr      l1dbd
.l1db7  ld      a,$fa
        out     (c),a           ; if reset, output data low to AUX
        jr      l1dbd
.l1dbd  ld      e,$02
.l1dbf  dec     e
        jr      nz,l1dbf        ; delay loop
        nop     
        add     a,$00
        ld      a,l
        dec     d
        jr      nz,l1dac        ; loop back for more bits
        nop     
        nop     
        add     a,$00
        nop     
        nop     
        ld      a,$fe
        out     (c),a           ; output data high to register
        ld      e,$06
.l1dd5  dec     e
        jr      nz,l1dd5        ; delay loop
        ret     

; Unused code for a FORMAT "P";n command, used in same way as FORMAT LINE n

.l1dd9  rst     $28
        defw    $0018           ; get character at CH_ADD
        rst     $28
        defw    $1c8c           ; get a string expression
        bit     7,(iy+$01)
        jr      z,l1df9         ; move on if just syntax-checking
        rst     $28
        defw    $2bf1           ; get string from stack
        ld      a,c
        dec     a
        or      b
        jr      z,l1df1         ; if length 1, move on
        call    l2ada
        defb    36              ; else error j - invalid baud rate
.l1df1  ld      a,(de)
        and     $df             ; get string char & make uppercase
        cp      'P'
        jp      nz,l1125        ; nonsense in BASIC error if not "P"
.l1df9  ld      hl,(CH_ADD)
        ld      a,(hl)
        cp      ';'
        jp      nz,l1125        ; nonsense in BASIC error if next char not ";"
        rst     $28
        defw    $0020           ; get next char & continue into FORMAT LINE

; The FORMAT LINE command

.l1e05  rst     $28
        defw    $1c82           ; get numeric expression
        bit     7,(iy+$01)
        jr      z,l1e15         ; move on if syntax-checking
        rst     $28
        defw    $1e99           ; get value to BC
        ld      (BAUD),BC       ; set BAUD rate
.l1e15  rst     $28
        defw    $0018           ; get next char
        cp      $0d
        jr      z,l1e21         ; move on if end-of-line
        cp      ':'
        jp      nz,l1125        ; error if not end-of-statement
.l1e21  call    l10b1           ; check for end-of-statement
        ld      bc,(BAUD)
        ld      a,b
        or      c
        jr      nz,l1e30        ; move on if baud rate not zero
        call    l2ada
        defb    $25             ; else error "invalid baud rate"
.l1e30  ld      hl,l1e50        ; baud rate table
.l1e33  ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; get next baud rate
        inc     hl
        ex      de,hl
        ld      a,h
        cp      $25
        jr      nc,l1e47        ; move on if end of table
        and     a
        sbc     hl,bc
        jr      nc,l1e47        ; move on if >= required rate
        ex      de,hl
        inc     hl              ; skip timing constant
        inc     hl
        jr      l1e33           ; loop back for next
.l1e47  ex      de,hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; get appropriate timing constant
        ld      (BAUD),de       ; save in BAUD
        ret     

; The baud rate table

.l1e50  defw    $0032,$0aa5     ; 50
        defw    $006e,$04d4     ; 110
        defw    $012c,$01c3     ; 300
        defw    $0258,$00e0     ; 600
        defw    $04b0,$006e     ; 1200
        defw    $0960,$0036     ; 2400
        defw    $12c0,$0019     ; 4800
        defw    $2580,$000b     ; 9600

; Printer input channel routine

.l1e70  ld      hl,FLAGS3
        bit     3,(hl)
        jp      z,l1e85		; move on if using Centronics
        ld      hl,SERFL
        ld      a,(hl)
        and     a
        jr      z,l1e89		; move on if no RS232 character waiting
        ld      (hl),$00	; reset SERFL flag
        inc     hl
        ld      a,(hl)		; and get character
        scf     
        ret     
.l1e85  rst     $28
        defw    $15c4		; invalid I/O device error
        ret
.l1e89  call    l2af9		; test for BREAK
        di      
        exx     
        ld      de,(BAUD)	; DE=BAUD
        ld      hl,(BAUD)
        srl     h
        rr      l		; HL=BAUD/2
        or      a
        ld      b,$fa		; B=timing constant
        exx     
        ld      c,$fd
        ld      d,$ff
        ld      e,$bf
        ld      b,d
        ld      a,$0e
        out     (c),a		; select AY register 14
        in      a,(c)		; get RS232/AUX value
        or      $f0
        and     $fb		; set CTS low
        ld      b,e
        out     (c),a		; output CTS low
        ld      h,a		; save RS232/AUX value with CTS low
.l1eb2  ld      b,d
        in      a,(c)		; get RS232/AUX value
        and     $80
        jr      z,l1ec2         ; move on if TXD was low (ie data ready)
.l1eb9  exx
        dec     b		; decrement timer
        exx     
        jr      nz,l1eb2        ; loop back
        xor     a		; carry reset for no data
        push    af
        jr      l1efb           ; move on if no data received
.l1ec2  in      a,(c)
        and     $80
        jr      nz,l1eb9        ; back if TXD high
        in      a,(c)
        and     $80
        jr      nz,l1eb9	; back if TXD high
        exx     
        ld      bc,$fffd
        ld      a,$80		; A'=char to build (carry will be set when
        ex      af,af'		; all 8 bits have been read)
.l1ed5  add     hl,de
        nop     
        nop     
        nop     
        nop     
.l1eda  dec     hl
        ld      a,h
        or      l
        jr      nz,l1eda	; baud rate timing loop
        in      a,(c)		; get RS232/AUX data
        and     $80		; mask data bit
        jp      z,l1eef		; move on if zero
        ex      af,af'
        scf     
        rra			; rotate a 1 bit in     
        jr      c,l1ef8		; move on if byte complete
        ex      af,af'
        jp      l1ed5		; loop back for more bits
.l1eef  ex      af,af'
        or      a
        rra     		; rotate a 0 bit in
        jr      c,l1ef8		; move on if byte complete
        ex      af,af'
        jp      l1ed5		; loop back for more bits
.l1ef8  scf			; carry set for data received
        push    af
        exx     
.l1efb  ld      a,h
        or      $04
        ld      b,e
        out     (c),a		; set RS232 CTS high
        exx     
        ld      h,d
        ld      l,e		; HL=BAUD
        ld      bc,$0007
        or      a
        sbc     hl,bc
.l1f0a  dec     hl
        ld      a,h
        or      l
        jr      nz,l1f0a	; timing loop
        ld      bc,$fffd
        add     hl,de
        add     hl,de
        add     hl,de
.l1f15  in      a,(c)
        and     $80
        jr      z,l1f23		; move on if TXD low (2nd byte available)
        dec     hl
        ld      a,h
        or      l
        jr      nz,l1f15	; timing loop
        pop     af		; restore value
        ei      
        ret     		; exit
.l1f23  in      a,(c)
        and     $80
        jr      nz,l1f15	; move back if TXD high
        in      a,(c)
        and     $80
        jr      nz,l1f15	; move back if TXD high
        ld      h,d
        ld      l,e
        ld      bc,$0002
        srl     h
        rr      l
        or      a
        sbc     hl,bc
        ld      bc,$fffd
        ld      a,$80		; prepare 2nd byte in A'
        ex      af,af'
.l1f41  nop
        nop     
        nop     
        nop     
        add     hl,de
.l1f46  dec     hl
        ld      a,h
        or      l
        jr      nz,l1f46	; timing loop
        in      a,(c)
        and     $80		; test bit
        jp      z,l1f5b		; move on if zero
        ex      af,af'
        scf     
        rra     		; rotate a 1 bit in
        jr      c,l1f64		; move on if byte complete
        ex      af,af'
        jp      l1f41		; back for more bits
.l1f5b  ex      af,af'
        or      a
        rra     		; rotate a 0 bit in
        jr      c,l1f64		; move on if byte complete
        ex      af,af'
        jp      l1f41		; back for more bits
.l1f64  ld      hl,SERFL
        ld      (hl),$01	; flag "2nd byte available"
        inc     hl
        ld      (hl),a		; store 2nd byte
        pop     af		; restore the 1st byte
        ei      
        ret     		; done

; Printer output channel routine

.l1f6e  push    hl
        ld      hl,FLAGS3
        bit     2,(hl)
        pop     hl
        jp      z,l2051		; go to output if pure binary channel
        push    af
        ld      a,(TVPARS)
        or      a
        jr      z,l1f8e         ; move on if no inline parameters expected
        dec     a
        ld      (TVPARS),a	; decrement # parameters
        jr      nz,l1f89        ; move on if more still needed
        pop     af
        jp      l2020		; move on 
.l1f89  pop     af
        ld      (TVDATA+1),a	; save first parameter & exit
        ret     
.l1f8e  pop     af
        cp      $a3
        jr      c,l1fa0         ; move on unless BASIC token
        ld      hl,(RETADDR)
        push    hl		; save RETADDR
        rst     $28
        defw    $0b52		; output tokens using ROM 3
        pop     hl
        ld      (RETADDR),hl	; restore RETADDR
        scf     
        ret     
.l1fa0  ld      hl,FLAGS
        res     0,(hl)		; reset "outputting space" flag
        cp      ' '
        jr      nz,l1fab
        set     0,(hl)		; set "outputting space" flag
.l1fab  cp      $7f
        jr      c,l1fb1
        ld      a,'?'		; substitute ? for graphics
.l1fb1  cp      ' '
        jr      c,l1fcc		; move on for control codes
        push    af
        ld      hl,COL
        inc     (hl)		; increment column
        ld      a,(WIDTH)
        cp      (hl)
        jr      nc,l1fc8	; if within width, move on to print
        call    l1fd0		; output CRLF
        ld      a,$01
        ld      (COL),a		; set first column
.l1fc8  pop     af
        jp      l2051		; output character
.l1fcc  cp      $0d
        jr      nz,l1fde	; move on unless CR
.l1fd0  xor     a
        ld      (COL),a		; reset column counter
        ld      a,$0d
        call    l2051		; output CRLF
        ld      a,$0a
        jp      l2051		; & exit
.l1fde  cp      $06
        jr      nz,l2001	; move on unless PRINT comma
        ld      bc,(COL)	; B=WIDTH, C=COL
        ld      e,$00
.l1fe8  inc     e		; increment COL & E
        inc     c
        ld      a,c
        cp      b
        jr      z,l1ff6		; if end of line, go to do E spaces
.l1fee  sub     $08
        jr      z,l1ff6		; or if at a tab stop
        jr      nc,l1fee
        jr      l1fe8		; loop back until reach a tab stop or eol
.l1ff6  push    de
        ld      a,' '
        call    l1f6e		; output a space
        pop     de
        dec     e
        ret     z
        jr      l1ff6		; loop back for more
.l2001  cp      $16
        jr      z,l200e		; move on for AT (2 inline codes)
        cp      $17
        jr      z,l200e		; move on for TAB (*BUG* 2 inline codes)
        cp      $10
        ret     c		; exit for codes 0-15
        jr      l2017		; move on for colour codes (1 inline code)
.l200e  ld      (TVDATA),a	; store control code
        ld      a,$02
        ld      (TVPARS),a	; & number of codes required
        ret     
.l2017  ld      (TVDATA),a	; store control code
        ld      a,$02		; *BUG* should be 1
        ld      (TVPARS),a	; & number of codes required
        ret     

; Here, we deal with inline parameters

.l2020  ld      d,a		; save last parameter
        ld      a,(TVDATA)	; get control code
        cp      $16
        jr      z,l2030		; move on for AT
        cp      $17
        ccf     
        ret     nz		; ignore other codes except TAB
        ld      a,(TVDATA+1)	; use first parameter as column
        ld      d,a
.l2030  ld      a,(WIDTH)	; get width
        cp      d
        jr      z,l2038
        jr      nc,l203e
.l2038  ld      b,a
        ld      a,d
        sub     b		; reduce column by width until in range
        ld      d,a
        jr      l2030
.l203e  ld      a,d
        or      a
        jp      z,l1fd0		; for column 0, do CRLF
.l2043  ld      a,(COL)
        cp      d
        ret     z		; exit if at right column
        push    de
        ld      a,' '
        call    l1f6e		; output a space
        pop     de
        jr      l2043		; loop back

; Subroutine to output a character to the printer (Centronics or RS232)

.l2051  push    hl
        ld      hl,FLAGS3
        bit     3,(hl)
        pop     hl
        jp      z,l20a8         ; move on if print output is centronics
        push    af              ; save character
        ld      c,$fd
        ld      d,$ff
        ld      e,$bf
        ld      b,d
        ld      a,$0e
        out     (c),a           ; select AY register 14
.l2067  call    l2af9           ; test for BREAK
        in      a,(c)           ; read RS232/AUX
        and     $40
        jr      nz,l2067        ; loop until DTR low
        ld      hl,(BAUD)
        ld      de,$0002
        or      a
        sbc     hl,de
        ex      de,hl           ; DE=BAUD-2
        pop     af              ; restore character
        cpl                     ; invert it
        scf                     ; set carry for initial bit
        ld      b,$0b           ; 11 bits to output
        di                      ; disable interrupts
.l2080  push    bc              ; save registers
        push    af
        ld      a,$fe
        ld      h,d             ; HL=BAUD-2
        ld      l,e
        ld      bc,$bffd
        jp      nc,l2092        ; move on to output a one bit
        and     $f7             ; mask RS232 RXD off
        out     (c),a           ; output zero bit to RS232
        jr      l2098
.l2092  or      $08             ; set RS232 RXD on
        out     (c),a           ; output one bit to RS232
        jr      l2098
.l2098  dec     hl
        ld      a,h
        or      l
        jr      nz,l2098        ; timing loop for baud rate
        nop                     ; more timing values
        nop     
        nop     
        pop     af              ; restore registers
        pop     bc
        or      a               ; clear carry (for stop bits)
        rra                     ; rotate next bit
        djnz    l2080           ; loop back for more
        ei      
        ret     
.l20a8  push    af              ; save character
.l20a9  call    l2af9           ; test for BREAK
        ld      bc,$0ffd
        in      a,(c)
        bit     0,a
        jr      nz,l20a9        ; wait for printer ready
        pop     af
        out     (c),a           ; output character
        di      
.l20b9  ld      bc,$1ffd
        ld      a,(BANK678)
        xor     $10             ; toggle strobe bit
        ld      (BANK678),a
        out     (c),a           ; output strobe
        bit     4,a
        jr      z,l20b9         ; loop back to finish with strobe high
        ei      
        scf     
        ret     
        ret     

; The COPY (to printer) command routine

.l20ce  bit     7,(iy+$01)
        ret     z               ; exit if syntax-checking
        ld      a,(BANK678)
        ld      bc,$1ffd
        set     4,a
        di                      ; disable interrupts
        ld      (BANK678),a
        out     (c),a           ; set strobe high
        ei      
        ld      hl,YLOC
        ld      (hl),$2b        ; set Y location to 43 (4 bits on each row)
.l20e7  ld      hl,l216b
        call    l2151           ; output 120dpi line command
        call    l2107           ; output raster image
        ld      hl,l2172
        call    l2151           ; output linefeed
        ld      hl,YLOC
        xor     a
        cp      (hl)
        jr      z,l2100         
        dec     (hl)
        jr      l20e7           ; loop back for more lines
.l2100  ld      hl,l2174
        call    l2151           ; reset linespacing and done
        ret     

; Subroutine to output a raster line for a non-expanded copy

.l2107  ld      hl,XLOC
        ld      (hl),$ff        ; set XLOC
.l210c  call    l2118           ; output a pixel's width
        ld      hl,XLOC
        xor     a
        cp      (hl)
        ret     z               ; exit if done line
        dec     (hl)
        jr      l210c           ; back for next pixel width

; Subroutine to output a pixel's width of a non-expanded copy

.l2118  ld      de,$c000        ; D=pixel position mask
        ld      bc,(XLOC)       ; C=x-position, B=y-position
        scf     
        rl      b
        scf     
        rl      b               ; B=top row of required bits
        ld      a,c
        cpl     
        ld      c,a             ; start at left of screen
        xor     a               ; initialise raster byte
        push    af
        push    de
        push    bc
.l212c  call    l215f           ; get pixel state
        pop     bc
        pop     de
        ld      e,$00
        jr      z,l2136         ; set E=0 for no pixel
        ld      e,d             ; or E=mask for pixel
.l2136  pop     af
        or      e               ; combine pixel into raster byte
        push    af
        dec     b               ; decrement Y position
        srl     d
        srl     d               ; shift mask right twice (4 pixels per row)
        push    de
        push    bc
        jr      nc,l212c        ; loop back if more pixels to get
        pop     bc
        pop     de
        pop     af
        ld      b,$03
.l2147  push    bc
        push    af
        call    l2051           ; output raster byte
        pop     af
        pop     bc
        djnz    l2147           ; loop back for 3 passes
        ret     

; Subroutine to output a counted string at HL to the printer

.l2151  ld      b,(hl)          ; get count
        inc     hl
.l2153  ld      a,(hl)          ; get next char
        push    hl
        push    bc
        call    l2051           ; output char to printer
        pop     bc
        pop     hl
        inc     hl
        djnz    l2153           ; loop back for more
        ret     

; Subroutine to check pixel at B=y,C=x
; On exit, Z is reset if pixel is ink, set if pixel is paper

.l215f  rst     $28
        defw    $22aa           ; get address of pixel in HL
        ld      b,a
        inc     b               ; B=counter to get required pixel
        xor     a               ; zero A
        scf                     ; set carry
.l2166  rra                     ; rotate bit into position
.l2167  djnz    l2166
        and     (hl)            ; mask against screen byte
        ret

; The line header for a non-expanded copy

.l216b  defb    $06             ; 6 bytes
        defb    $1b,'1'         ; select 7/72" linespacing
        defb    $1b,'L',$00,$03 ; print 768 dots in 120dpi

; The line terminator for a non-expanded copy

.l2172  defb    $01             ; 1 byte
        defb    $0a             ; linefeed

; The terminator for a non-expanded copy

.l2174  defb    $02             ; 2 bytes
        defb    $1b,'2'         ; select 1/6" linespacing

; The PRINT & LPRINT commands (enter at l2177 for LPRINT, l217b for PRINT)

.l2177  ld      a,$03           ; use stream 3 for LPRINT
        jr      l217d
.l217b  ld      a,$02           ; use stream 2 for PRINT
.l217d  rst     $28
        defw    $2530           ; are we syntax-checking?   
        jr      z,l2185
        rst     $28
        defw    $1601           ; open channel if not
.l2185  rst     $28
        defw    $0d4d           ; set temporary colours
        rst     $28
        defw    $1fdf           ; use ROM 3 for command routine
        call    l10b1           ; check for end-of-statement
        ret     

; The INPUT command

.l218f  rst     $28
        defw    $2530
        jr      z,l219c         ; move on if syntax-checking
        ld      a,$01
.l2196  rst     $28
        defw    $1601           ; open channel to stream 1
        rst     $28
        defw    $0d6e           ; clear lower screen
.l219c  ld      (iy+$02),$01    ; set DF_SZ to 1
        rst     $28
        defw    $20c1           ; deal with the input items
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $20a0           ; use ROM 3 for actual routine
        ret

; The COPY command

.l21aa  rst     $18
        cp      $0d
        jp      z,l20ce         ; go to do printer copy if end of line
        cp      ':'
        jp      z,l20ce         ; or end of statement
        cp      $b9
        jp      z,l3328         ; go to do expanded copy if COPY EXP
        cp      $f9
        jp      z,l35c4         ; move on if COPY RANDOMIZE
        rst     $28
        defw    $1c8c           ; get a string expression
        rst     $28
        defw    $0018           ; get character
.l21c5  cp      $cc
        jr      z,l21cd         ; move on if found TO
        call    l2ada
        defb    $0b             ; error C - nonsense in BASIC
.l21cd  rst     $28
        defw    $0020           ; get next char
.l21d0  cp      $aa
        jp      z,l2237         ; move on if COPY f$ TO SCREEN$
        cp      $a3
        jp      z,l2257         ; move on if COPY f$ TO SPECTRUM FORMAT
        cp      $e0
        jp      z,l2237         ; move on if COPY f$ TO LPRINT
        rst     $28
        defw    $1c8c           ; get a string expression
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; fetch last value from calculator stack
        ld      a,b
        or      c               ; check length of second string
        jr      nz,l21f0
        call    l2ada
        defb    $2c             ; error "Bad filename"
.l21f0  inc     de
        ld      a,(de)          ; check 2nd char of 2nd string
        dec     de
        cp      ':'
        jr      nz,l21fb        ; move on if not specifying a drive
        ld      a,(de)
        and     $df             ; convert drive letter to uppercase
        ld      (de),a
.l21fb  ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy 2nd string to page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; terminate 2nd filename with $ff
        inc     de              ; increment pointer after 2nd filename
        call    l2b64           ; page in normal memory
        push    de              ; save pointer
        rst     $28
        defw    $2bf1           ; fetch value from calculator stack
        ld      a,b
        or      c
        jr      nz,l2218        ; check length of first string
        call    l2ada
        defb    $2c             ; error "Bad filename"
.l2218  inc     de
        ld      a,(de)          ; check 2nd char of first string
        dec     de
        cp      ':'
        jr      nz,l2223        ; move on if not specifying a drive
        ld      a,(de)
        and     $df             ; convert drive letter to uppercase
        ld      (de),a
.l2223  pop     hl              ; restore address in page 7
        ex      de,hl
        call    l3f63           ; copy 1st filename to page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; terminate 1st filename with $ff
        call    l2b64           ; page in normal memory
        xor     a               ; signal "copying to a file"
        scf     
        call    l2ba3           ; do the copy
        ret     

; The COPY...TO SCREEN$/LPRINT commands

.l2237  push    af              ; save keyword
        rst     $28
        defw    $0020           ; get next char
.l223b  call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; get string
        ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy into page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        call    l2b64           ; page in normal memory
        pop     af              ; restore keyword as destination flag
        and     a               ; reset Z flag
        call    l2ba3           ; copy the file
        ret     

; The COPY....TO SPECTRUM FORMAT command

.l2257  rst     $28
        defw    $0020           ; get next char
.l225a  cp      $d0             ; check for FORMAT
        jr      z,l2262
        call    l2ada
        defb    $0b             ; nonsense in BASIC if not
.l2262  rst     $28
        defw    $0020           ; get to next char
.l2265  call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2bf1           ; get string
        ld      hl,tmp_fspec
        ex      de,hl
        call    l3f63           ; copy into page 7
        call    l2b89           ; page in DOS workspace
        ld      a,$ff
        ld      (de),a          ; add terminator
        call    l2b64           ; page in normal memory
        xor     a
        call    l2ba3           ; copy the file
        ret     

; The NEW command

.l2280  di      
        call    l3e80
        defw    $01b0           ; use routine in ROM 0

; The CIRCLE command

.l2286  rst     $18             ; get current char
        cp      ','
        jr      nz,l22c3        ; error C if not comma
        rst     $20             ; get next char
        rst     $28
        defw    $1c82           ; get numeric expression
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $232d           ; use ROM 3 for actual routine
        ret     

; The DRAW command

.l2296  rst     $18             ; get current char
        cp      ','
        jr      z,l22a2         ; move on if comma
        call    l10b1           ; check for end-of-statement
        rst     $28
        defw    $2477           ; use ROM 3 to draw line
        ret     
.l22a2  rst     $20             ; get next char
        rst     $28
        defw    $1c82           ; get numeric expression
        call    l10b1           ; check for end of statement
        rst     $28
        defw    $2394           ; use ROM 3 to draw curve
        ret     

; The DIM command

.l22ad  rst     $28             
        defw    $28b2           ; search variables area
        jr      nz,l22c3        ; move on if error
        rst     $28
        defw    $2530
        jr      nz,l22bf        ; move on if runtime
        res     6,c             ; test string syntax as if numeric
        rst     $28
        defw    $2996           ; check syntax of parenthesised expression
        call    l10b1           ; check for end-of-statement
.l22bf  rst     $28
        defw    $2c15           ; use ROM 3 for actual command
        ret     
.l22c3  call    l2ada
        defb    $0b             ; error C - nonsense in BASIC


; Subroutine to clear whole display unless unnecessary

.l22c7  bit     0,(iy+$30)      ; check FLAGS2
        ret     z               ; exit if not necessay
        rst     $28
        defw    $0daf           ; cls
        ret     

; Subroutine to evaluate an expression for the calculator, & set the
; result to be used by the next calculation

.l22d0  ld      hl,$fffe
        ld      (PPC),hl	; set statement -2
        res     7,(iy+$01)	; signal "syntax checking"
        call    l2368		; set interpreter to start of line
        rst     $28
        defw    $24fb		; evaluate an expression
        bit     6,(iy+$01)
        jr      z,l2312         ; move on if value not numeric
        rst     $18
        cp      $0d
        jr      nz,l2312        ; or if next character isn't end-of-line
        set     7,(iy+$01)	; signal "executing"
        call    l2368		; set interpreter to start of line
        ld      hl,l25cb
        ld      (SYNRET),hl	; set up error return address
        rst     $28
        defw    $24fb		; evaluate an expression
        bit     6,(iy+$01)
        jr      z,l2312         ; move on if value not numeric
        ld      de,LASTV
        ld      hl,(STKEND)
        ld      bc,$0005
        or      a
        sbc     hl,bc
        ldir    		; copy result into LASTV variable
        jp      l2316
.l2312  call    l2ada
        defb	25		; error Q - parameter error
.l2316  ld      a,$0d
        call    l2347		; do a newline
        ld      bc,$0001
        rst     $28
        defw    $0030		; make a byte in the workspace
.l2321  ld      (K_CUR),hl	; save address of cursor
        push    hl
        ld      hl,(CURCHL)	; get address of current channel information
        push    hl
        ld      a,$ff
        rst     $28
        defw    $1601		; open channel to stream -1
        rst     $28
        defw    $2de3		; print the result value
        pop     hl
        rst     $28
        defw    $1615		; restore old channel
        pop     de
        ld      hl,(K_CUR)	; get new cursor address
        and     a
        sbc     hl,de		; HL=# of result chars
.l233c  ld      a,(de)
        call    l2347		; "type" each result character
        inc     de
        dec     hl
        ld      a,h
        or      l
        jr      nz,l233c	; loop back for more
        ret     

; Subroutine to "do" a key (A) using ROM 0's editing keys

.l2347  push    hl
        push    de
        call    l2b89		; page in DOS workspace
        ld      hl,ed_flags
        res     3,(hl)		; ???
        push    af
        ld      a,$02
        rst     $28
        defw    $1601		; open channel to stream 2
        pop     af
        call    l3e80
        defw    $0716		; "do" the key
        ld      hl,ed_flags
        res     3,(hl)		; ???
        call    l2b64		; page in normal memory
        pop     de
        pop     hl
        ret     

; Subroutine to set interpreter to entered line, with A=first char

.l2368  ld      hl,(E_LINE)
        dec     hl
        ld      (CH_ADD),hl	; CH_ADD=E_LINE-1
        rst     $20		; get next char
        ret     

; Subroutine to determine if line is a single LET statement (Z set if so)

.l2371  call    l2368		; get first char in E_LINE
        cp      $f1
        ret     nz		; exit unless LET
        ld      hl,(CH_ADD)
.l237a  ld      a,(hl)
        inc     hl
        cp      $0d
        ret     z		; exit when end of line found (with Z set)
        cp      ':'
        jr      nz,l237a
        or      a
        ret     		; or when end of statement found (with Z reset)

; Subroutine to check if character is a binary operator (Z set if so)

.l2385  ld      b,a		; save char
        ld      hl,l2397	; list of operators
.l2389  ld      a,(hl)		; get next
        inc     hl
        or      a
        jr      z,l2393		; if end of list, exit with Z reset
        cp      b
        jr      nz,l2389	; loop back if no match
        ld      a,b		; restore char
        ret     		; exit with Z set
.l2393  or      $ff		; reset Z
        ld      a,b		; restore character
        ret     

; List of valid binary operators for numeric calculations

.l2397	defm	"+-*/^=><"
	defb	$c7,$c8,$c9	; <=,>=,<>
	defb	$c5,$c6		; OR,AND	
	defb	0

; Subroutine to check if a character is a valid function (Z set if so)

.l23a5  cp      $a5
        jr      c,l23b7		; move on if before RND
        cp      $c4
        jr      nc,l23b7	; or after NOT
        cp      $ac
        jr      z,l23b7		; or if AT
        cp      $ad
        jr      z,l23b7		; or if TAB
        cp      a		; set Z for valid functions
        ret     
.l23b7  cp      $a5		; reset Z
        ret     

; Subroutine to check if character is start of a value

.l23ba  ld      b,a		; save character
        or      $20		; make lowercase
        cp      'a'
        jr      c,l23c7
        cp      'z'+1
        jr      nc,l23c7
        cp      a		; set Z if character is a letter
        ret     
.l23c7  ld      a,b
        cp      '.'
        ret     z		; exit with Z set if "."
        call    l23e4		; check for digits
        jr      nz,l23e1	; if not, junk character & exit
.l23d0  rst     $20		; get next char
        call    l23e4
        jr      z,l23d0		; loop back while still digits
        cp      '.'
        ret     z		; exit with Z set if "."
        cp      'E'
        ret     z		; or "E"
        cp      'e'
        ret     z		; or "e"
        jr      l2385		; else check for a binary operator
.l23e1  or      $ff		; junk character & reset Z
        ret     

; Subroutine to check if A is a digit (Z set if so)

.l23e4  cp      '0'
        jr      c,l23ee
        cp      '9'+1
        jr      nc,l23ee
        cp      a		; set Z if char is a digit
        ret     
.l23ee  cp      '0'		; reset Z otherwise
        ret     

; The PLAY command

.l23f1  ld      b,$00           ; string counter
        rst     $18             ; get char
.l23f4  push    bc
        rst     $28
        defw    $1c8c           ; get a string
        pop     bc
        inc     b               ; increment counter
        cp      ','             
        jr      nz,l2401        ; move on if no more strings
        rst     $20             ; get next char
        jr      l23f4           ; loop back
.l2401  ld      a,b
        cp      $09
        jr      c,l240a         ; up to 9 strings allowed
        call    l2ada
        defb    $2b             ; error "Too many tied notes"
.l240a  call    l10b1           ; check for end-of-statement
        jp      l1571           ; go to execute command

; Subroutine called from ROM 0 to initialise DOS & check the status
; of drives on the system, displaying information to the user.

.l2410  call    l2b89           ; page in DOS workspace (page 7)
        ld      hl,FLAGS3
        bit     7,(hl)
        jr      nz,l242a        ; move on if DOS already initialised
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_INITIALISE
        call    l32ee           ; restore TSTACK
        ld      hl,FLAGS3
        set     7,(hl)          ; signal "DOS initialised"
.l242a  ld      a,$ff
        ld      hl,$244e        ; standard ALERT routine in ROM ?
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_MESSAGE
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in page 0
        ld      hl,l24be
        call    l24b5           ; display "Drive"
        ld      hl,FLAGS3
        res     4,(hl)          ; signal "disk interface not present"
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_INTERFACE
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in page 0
        jr      c,l2463         ; move on if interface present
        ld      hl,l24c4
        call    l24b5           ; display " M:" if no interface
        jr      l24ae           ; move on
.l2463  ld      a,'A'           ; set "A:" as default load/save drive
        ld      (LODDRV),a
        ld      (SAVDRV),a
        ld      hl,FLAGS3
        set     4,(hl)          ; signal "disk interface present"
        res     5,(hl)          ; signal "no drive B:"
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_ASK_1
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in page 0
        jr      c,l24a3         ; move on if drive B exists
        ld      c,$00
        ld      hl,$2455        ; change disk routine in ROM ?
        call    l2b89           ; page in DOS workspace
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_MAP_B       ; map drive B: to unit 0
        call    l32ee           ; restore TSTACK
        call    l2b64           ; page in page 0
        ld      hl,l24c8
        call    l24b5           ; display " A:"
        jr      l24ae           ; move on
.l24a3  ld      hl,FLAGS3
        set     5,(hl)          ; signal "drive B: present"
        ld      hl,l24d4
        call    l24b5           ; display "s A: and B:"
.l24ae  ld      hl,l24e4
        call    l24b5           ; display " available."
        ret     

; Subroutine to print a null-terminated string

.l24b5  ld      a,(hl)          ; get next char
        or      a
        ret     z               ; exit if null
        rst     $28
        defw    $0010           ; print it
        inc     hl
        jr      l24b5           ; back for more

; Drives present messages

.l24be  defm    "Drive"&0
.l24c4  defm    " M:"&0
.l24c8  defm    "s A: and M:"&0
.l24d4  defm    "s A:, B: and M:"&0
.l24e4  defm    " available."&0

; Subroutine used to execute a command line or evaluate an expression

.l24f0  ld      (iy+$00),$ff	; clear error
        ld      (iy+$31),$02	; set lower screen to 2 lines
        ld      hl,ONERR
        push    hl
        ld      (ERR_SP),sp	; set up error stack
        ld      hl,l2560
        ld      (SYNRET),hl	; set error return address
        call    l2368		; set interpretation address & get first char
        call    l23a5		; test for function
.l250c  jp      z,l22d0		; if so, evaluate the expression
        cp      '('
        jp      z,l22d0		; or if bracketed expression
        cp      '-'
        jp      z,l22d0		; or a unary operator (+ or -)
        cp      '+'
        jp      z,l22d0
        call    l23ba		; check for start of a value (var or number)
        jp      z,l22d0		; if so, evaluate the expression
        call    l2b89		; page in DOS workspace
        ld      a,(process)     ; get current process
        call    l2b64		; page in normal memory
        cp      $04		; is it the calculator?
.l252f  jp      nz,l0fbf	; if not, execute it
        call    l2371		; is line a single LET statement?
        jp      z,l0fbf		; if so, execute it
        pop     hl		; unstack ONERR address
        ret     		; exit

; The +3-specific error-handling routine
; ONERR jumps here
        
.l253a  call    l2b89           ; page in DOS workspace
        ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file 0
        call    l32ee           ; restore TSTACK
        jr      c,l2559         ; move on if no error
        ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_ABANDON     ; abandon file 0
        call    l32ee           ; restore TSTACK
.l2559  call    l2b64           ; page back normal memory
        ld      hl,(SYNRET)
        jp      (hl)            ; return to syntax return address

; This is one of the syntax return addresses, used when entering a line
; as a direct command

.l2560  bit     7,(iy+$00)
        jr      nz,l2567        
        ret                     ; exit if error in line
.l2567  ld      hl,(E_LINE)
        ld      (CH_ADD),hl     ; reset CH_ADD to editing line
        rst     $28
        defw    $19fb           ; get line number of editing line
        ld      a,b
        or      c
        jp      nz,l268e        ; move on if it exists, to add to program
        rst     $18             ; get character
        cp      $0d
        ret     z               ; exit if empty line
        call    l22c7           ; clear display if necessary
        bit     6,(iy+$02)
        jr      nz,l2585        
        rst     $28
        defw    $0d6e           ; clear lower screen if necessary
.l2585  res     6,(iy+$02)      ; signal "lower screen clear"
        call    l2b89           ; page in DOS workspace
        ld      hl,ed_flags
        bit     6,(hl)          ; ???
        jr      nz,l259e        
        inc     hl
        ld      a,(hl)          ; ???
        cp      $00
        jr      nz,l259e          
        call    l3e80
        defw    $1a8e           ; clear bottom 3 lines to editor colours
.l259e  call    l2b64           ; page in normal memory
        ld      hl,TV_FLAG
        res     3,(hl)          ; signal "mode hasn't changed"
        ld      a,$19
        sub     (iy+$4f)
        ld      (SCR_CT),a      ; set appropriate scroll count
        set     7,(iy+$01)      ; signal "execution"
        ld      (iy+$0a),$01    ; jump to statement 1

; [*BUG* - Whenever a typed in command is executed directly from the editing workspace, a new GO SUB marker is set up on the
;          stack. Any existing GO SUB calls that were on the stack are lost and as a result attempting to continue the program
;          (without the use of CLEAR or RUN) will likely lead to a "7 RETURN without GOSUB" error report message being displayed.
;          However, the stack marker will already have been lost due to the error handler routine at $25CB. The first action it
;          does is to reset the stack pointer to point to the location of RAMTOP, i.e. after the GO SUB marker. This is why it is
;          necessary for a new GO SUB marker needs to be set up. Credit: Michal Skrzypek]

        ld      hl,$3e00
        push    hl              ; stack GOSUB stack end marker
        ld      hl,ONERR
        push    hl              ; stack error address
        ld      (ERR_SP),sp     ; reset ERR_SP
        ld      hl,l25cb
        ld      (SYNRET),hl     ; store execution error handler address
        jp      l1048           ; execute immediate command

; This is one of the syntax return addresses, used during execution

; [*BUG* - Upon terminating a BASIC program, either via reaching the end of the program or due to an error occurring,
;          execution is passed to this routine. The first action it does is to reset the stack pointer to point to the
;          location of RAMTOP, i.e. after the GO SUB marker. However, this means that any existing GO SUB calls that
;          were on the stack are lost and so attempting to continue the program (without the use of CLEAR or RUN) will
;          likely lead to a "7 RETURN without GOSUB" error report message being displayed. When a new typed in command
;          is executed, the code above sets up a new GO SUB marker on the stack. Credit: Michal Skrzypek]

.l25cb  ld      sp,(RAMTOP)
        inc     sp              ; clear return stack
        ld      hl,TSTACK
        ld      (OLDSP),hl      ; set OLDSP to temporary stack area
        halt                    ; wait for an interrupt
        res     5,(iy+$01)      ; signal no key available
        ld      a,(ERR_NR)
        inc     a               ; A=error code
.l25df  push    af              ; save error code
        ld      hl,$0000
        ld      (iy+$37),h      ; clear FLAGX
        ld      (iy+$26),h      ; clear high byte of X_PTR
        ld      (DEFADD),hl     ; clear DEFADD
        ld      hl,$0001
        ld      (STRMS+6),hl    ; reset stream 0
        rst     $28
        defw    $16b0           ; clear editing areas and calculator etc
        res     5,(iy+$37)      ; ???
        rst     $28
        defw    $0d6e           ; clear lower screen
        set     5,(iy+$02)      ; signal "clear lower screen after keystroke"
        pop     af              ; get back error code
        ld      b,a             ; save it 
        cp      $0a
        jr      c,l2614         ; move on if 0-9
        cp      $1d
        jr      c,l2612         ; move on if A-R
        cp      $2c
        jr      nc,l261a        ; move on if +3DOS error
        add     a,$14           ; else convert for errors a-o
        jr      l2614
.l2612  add     a,$07           ; convert to code to letter
.l2614  rst     $28
        defw    $15ef           ; output error character (0-9 or A-R or a-o)
        ld      a,' '
        rst     $10             ; output space
.l261a  ld      a,b             ; get back error code
        cp      $1d
        jr      c,l2631         ; move on if old 48K Spectrum error
        sub     $1d
        ld      b,$00
        ld      c,a
        ld      hl,l2705
        add     hl,bc
        add     hl,bc           ; HL points to address of error message
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=error message address
        call    l2ace           ; output it
        jr      l2637
.l2631  ld      de,$1391        ; base address of ROM 3 message table
        rst     $28
        defw    $0c0a           ; output 48K Spectrum error message
.l2637  xor     a
        ld      de,$1536
        rst     $28
        defw    $0c0a           ; output "comma" message
        ld      bc,(PPC)        ; get error line number
        rst     $28
        defw    $1a1b           ; output it
        ld      a,':'
        rst     $10             ; output ":"
        ld      c,(iy+$0d)      ; get error statement number
        ld      b,$00
        rst     $28
        defw    $1a1b           ; output it
        rst     $28
        defw    $1097           ; clear editing area/workspace
        ld      a,(ERR_NR)
        inc     a
        jr      z,l2674         ; move on if error "OK"
        cp      $09
        jr      z,l2661         ; move on if error "9 - STOP statement"
        cp      $15
        jr      nz,l2664        ; move on if not "L - BREAK into program"
.l2661  inc     (iy+$0d)        ; increment statement for CONTINUE
.l2664  ld      bc,$0003
        ld      de,OSPCC
        ld      hl,NSPPC
        bit     7,(hl)
        jr      z,l2672
        add     hl,bc
.l2672  lddr                    ; copy line/statement to CONTINUE at
.l2674  ld      (iy+$0a),$ff    ; clear NSPPC
        res     3,(iy+$01)      ; signal "K" mode
        ld      hl,FLAGS3
        res     0,(hl)          ; ???
        call    l3e80
        defw    $067b           ; call ROM 0
.l2686  ld      a,$10		; error G - no room for line
        ld      bc,$0000
        jp      l25df		; loop back

; Routine to ???

.l268e  ld      (E_PPC),bc	; ???
        call    l2b89		; page in DOS workspace
        ld      a,b
        or      c
        jr      z,l26a1
        ld      (E_PPC),bc	; ???
        ld      ($ec08),bc
.l26a1  call    l2b64		; page in normal memory
        ld      hl,(CH_ADD)
        ex      de,hl
        ld      hl,l2686	; error return address (no room)
        push    hl
        ld      hl,(WORKSP)
        scf     
        sbc     hl,de
        push    hl		; HL=line length
        ld      h,b
        ld      l,c
        rst     $28
        defw    $196e		; get address of line in program
        jr      nz,l26c0	; if line not in program yet, move on
        rst     $28
        defw    $19b8		; get address of next line
        rst     $28
        defw    $19e8		; delete the existing line
.l26c0  pop     bc		; restore line length
        ld      a,c
        dec     a
        or      b
        jr      nz,l26db	; move on if no line body (just deleting)
        call    l2b89		; page in DOS workspace
        push    hl
        ld      hl,(E_PPC)
        call    l3e80
        defw    $1418		; ???
        ld      (E_PPC),hl
        pop     hl
        call    l2b64		; page in normal memory
        jr      l2703
.l26db  push    bc
        inc     bc
        inc     bc
        inc     bc
        inc     bc
        dec     hl
        ld      de,(PROG)
        push    de
.l26e6  rst     $28
        defw    $1655		; make space for ???
        pop     hl
.l26ea  ld      (PROG),hl
        pop     bc
        push    bc
        inc     de
        ld      hl,(WORKSP)
        dec     hl
        dec     hl
        lddr    
        ld      hl,(E_PPC)
        ex      de,hl
        pop     bc
        ld      (hl),b
        dec     hl
        ld      (hl),c
        dec     hl
        ld      (hl),e
        dec     hl
        ld      (hl),d
.l2703  pop     af		; ???
        ret     

; Table of error message addresses

.l2705  defw    l276d
        defw    l2778
        defw    l2787
        defw    l2791
        defw    l27a2
        defw    l27b5
        defw    l27c1
        defw    l27c1
        defw    l27d4
        defw    l27e2
        defw    l27f3
        defw    l2804
        defw    l2812
        defw    l2823
        defw    l282f
        defw    l2842
        defw    l2842
        defw    l284e
        defw    l285c
        defw    l286b
        defw    l2879
        defw    l288c
        defw    l289d
        defw    l28a6
        defw    l28b4
        defw    l28c5
        defw    l28d2
        defw    l28e5
        defw    l28fd
        defw    l290b
        defw    l2913
        defw    l291f
        defw    l2933
        defw    l293f
        defw    l294e
        defw    l2965
        defw    l296e
        defw    l297c
        defw    l2983
        defw    l2997
        defw    l29af
        defw    l29c1
        defw    l29d6
        defw    l29e6
        defw    l29f7
        defw    l2a0f
        defw    l2a29
        defw    l2a42
        defw    l2a59
        defw    l2a74
        defw    l2a8a
        defw    l2a97

; The +3 BASIC and +3DOS error messages

.l276d  defm    "MERGE erro"&('r'+$80)
.l2778  defm    "Wrong file typ"&('e'+$80)
.l2787  defm    "CODE erro"&('r'+$80)
.l2791  defm    "Too many bracket"&('s'+$80)
.l27a2  defm    "File already exist"&('s'+$80)
.l27b5  defm    "Invalid nam"&('e'+$80)
.l27c1  defm    "File does not exis"&('t'+$80)
.l27d4  defm    "Invalid devic"&('e'+$80)
.l27e2  defm    "Invalid baud rat"&('e'+$80)
.l27f3  defm    "Invalid note nam"&('e'+$80)
.l2804  defm    "Number too bi"&('g'+$80)
.l2812  defm    "Note out of rang"&('e'+$80)
.l2823  defm    "Out of rang"&('e'+$80)
.l282f  defm    "Too many tied note"&('s'+$80)
.l2842  defm    "Bad filenam"&('e'+$80)
.l284e  defm    "Bad parameter"&('s'+$80)
.l285c  defm    "Drive not foun"&('d'+$80)
.l286b  defm    "File not foun"&('d'+$80)
.l2879  defm    "File already exist"&('s'+$80)
.l288c  defm    "End of file foun"&('d'+$80)
.l289d  defm    "Disk ful"&('l'+$80)
.l28a6  defm    "Directory ful"&('l'+$80)
.l28b4  defm    "File is read onl"&('y'+$80)
.l28c5  defm    "File not ope"&('n'+$80)
.l28d2  defm    "File already in us"&('e'+$80)
.l28e5  defm    "No rename between drive"&('s'+$80)
.l28fd  defm    "Missing exten"&('t'+$80)
.l290b  defm    "Uncache"&('d'+$80)
.l2913  defm    "File too bi"&('g'+$80)
.l291f  defm    "Disk is not bootabl"&('e'+$80)
.l2933  defm    "Drive in us"&('e'+$80)
.l293f  defm    "Drive not read"&('y'+$80)
.l294e  defm    "Disk is write protecte"&('d'+$80)
.l2965  defm    "Seek fai"&('l'+$80)
.l296e  defm    "CRC data erro"&('r'+$80)
.l297c  defm    "No dat"&('a'+$80)
.l2983  defm    "Missing address mar"&('k'+$80)
.l2997  defm    "Unrecognised disk forma"&('t'+$80)
.l29af  defm    "Unknown disk erro"&('r'+$80)
.l29c1  defm    "Disk has been change"&('d'+$80)
.l29d6  defm    "Unsuitable medi"&('a'+$80)
.l29e6  defm    "Invalid attribut"&('e'+$80)
.l29f7  defm    "Cannot copy to/from tap"&('e'+$80)
.l2a0f  defm    "Destination cannot be wil"&('d'+$80)
.l2a29  defm    "Destination must be driv"&('e'+$80)
.l2a42  defm    "Drive B: is not presen"&('t'+$80)
.l2a59  defm    "+2A does not support forma"&('t'+$80)
.l2a74  defm    "Drive must be A: or B"&(':'+$80)
.l2a8a  defm    "Invalid driv"&('e'+$80)
.l2a97  defm    "Code length erro"&('r'+$80)
.l2aa8  defm    "You should never see thi"&('s'+$80)
        defm    "Hello there !"

; Subroutine to output an error message (terminated by
; a byte with bit 7 set)
; Enter with DE=message address

.l2ace  ld      a,(de)          ; get next char
        and     $7f             ; mask bit 7
.l2ad1  push    de              
        rst     $10             ; output
        pop     de
        ld      a,(de)
.l2ad5  inc     de
        add     a,a             ; check bit 7
        jr      nc,l2ace        ; loop back if not set
        ret     

; The Error Handling routine
; Enter here with inline error code-1

.l2ada  pop     hl              ; get address of error code
        ld      sp,(ERR_SP)     ; reset SP
        ld      a,(hl)
        ld      (RAMERR),a      ; store error number-1
        inc     a               ; get error code
        cp      $1e
        jr      nc,l2aeb        ; move on if a +3-specific error
.l2ae8  rst     $28
        defw    RAMRST          ; else call ROM 3 error handler
.l2aeb  dec     a
        ld      (iy+$00),a      ; save code in ERR_NR
.l2aef  ld      hl,(CH_ADD)     ; get address at which error occurred
        ld      (X_PTR),hl      ; save it
        rst     $28
        defw    $16c5           ; clear calculator stack
        ret                     ; exit to error address

; Subroutine to test the BREAK key
; Terminates with error L - BREAK into program if so

.l2af9  ld      a,$7f
        in      a,($fe)
        rra     
        ret     c               ; exit if SPACE not pressed 
        ld      a,$fe
        in      a,($fe)
        rra     
        ret     c               ; or if CAPS not pressed
        call    l2ada
        defb    $14             ; error L

; Subroutine to execute routine at HL, returning to l3a1b in order to
; return control to ROM 3
; It is provided to allow serial input into 48K BASIC, but looks buggy

.l2b09  ei
        ex      af,af'
        ld      de,l3a1b
        push    de              ; stack return address to paging routine
        res     3,(iy+$02)      ; set "mode unchanged"
        push    hl
        ld      hl,(ERR_SP)
        ld      e,(hl)
        inc     hl
        ld      d,(hl)		; DE=current error return address
        and     a
        ld      hl,$107f	; test against ROM 3 editing error address
        sbc     hl,de
.l2b20  jr      nz,l2b5a	; move on if not
.l2b22  pop     hl
        ld      sp,(ERR_SP)
        pop     de		; drop two addresses from error return stack
        pop     de
        ld      (ERR_SP),de
.l2b2d  push    hl		; save routine address
        ld      de,l2b33	; address to return to here
        push    de
.l2b32  jp      (hl)		; execute routine
.l2b33  jr      c,l2b3e		; if no error, move on
        jr      z,l2b3b		; if no character, move on
.l2b37  call    l2ada
        defb	7		; error 8 ???
.l2b3b  pop     hl
.l2b3c  jr      l2b2d		; call the routine again
.l2b3e  cp      $0d
        jr      z,l2b50		; if character was CR, move on
        ld      hl,(RETADDR)
        push    hl
        rst     $28
        defw    $0f85		; else add a character to current input line
        pop     hl
        ld      (RETADDR),hl
        pop     hl
        jr      l2b2d		; loop back to get another character
.l2b50  pop     hl		; discard routine address
        ld      a,(BANKM)
        or      $10		; select ROM 3
        push    af
        jp      l3a1b		; go to exit
.l2b5a  pop     hl
        ld      de,l2b60	; address to return to here
        push    de
        jp      (hl)		; execute routine
.l2b60  ret     c		; exit if no errors
        ret     z
        jr      l2b37		; else go to act on them

; Subroutine to page in normal memory (page 0) and swap SP with OLDSP

.l2b64  ex      af,af'          ; save AF
        ld      a,$10
        di      
        call    l2b7e           ; page in page 0
        pop     af              ; AF=return address
        ld      (TARGET),hl     ; save HL
        ld      hl,(OLDSP)
        ld      (OLDSP),sp
        ld      sp,hl           ; SP now swapped with OLDSP
        ei      
        ld      hl,(TARGET)     ; restore HL
        push    af              ; restack return address
        ex      af,af'          ; restore AF
        ret     

; Subroutine to page in a RAM/ROM/screen combination (in A)

.l2b7e  push    bc
        ld      bc,$7ffd
        out     (c),a           ; page it in
        ld      (BANKM),a
        pop     bc
        ret     

; Subroutine to page in DOS workspace (page 7) and swap SP with OLDSP

.l2b89  ex      af,af'          ; save AF
        di      
        pop     af              ; AF=return address
        ld      (TARGET),hl     ; save HL
        ld      hl,(OLDSP)
        ld      (OLDSP),sp
        ld      sp,hl           ; SP swapped with OLDSP
        ld      hl,(TARGET)     ; restore HL
        push    af              ; push back return address
        ld      a,$17
        call    l2b7e           ; page in page 7
        ei      
        ex      af,af'          ; restore AF
        ret     

; Subroutine to copy a file
; Enter with A=destination flag
; A=$00 - file
;  =$e0 - printer
;  =$aa - screen
; Z flag set if A=$00
; C flag reset if copying TO SPECTRUM FORMAT

.l2ba3  call    l2b89           ; page in DOS workspace
        ld      (dst_dev),a     ; save destination flag
        push    af
        jr      z,l2bb2         ; move on if copying to a file
        cp      $e0             ; copy to LPRINT?
        ld      a,$03           ; use stream 3
        jr      z,l2bb4         
.l2bb2  ld      a,$02           ; use stream 2
.l2bb4  call    l2b64           ; page in normal memory
        rst     $28
        defw    $1601           ; open channel to stream
        call    l2b89           ; page in DOS workspace
        pop     af              ; restore destination flag
        jr      z,l2c1e         ; move on if copying to another file
        ld      hl,tmp_fspec    ; stored filename address
        ld      bc,$0001        ; use file number 0,exclusive-read
        ld      de,$0001        ; open action - error if doesn't exist
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error opening
.l2bd7  ld      b,$00           ; file 0
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_BYTE_READ   ; read a byte
        call    l32ee           ; restore TSTACK
        jr      c,l2bed         ; move on if no error
        cp      $19
        jp      nz,l3219        ; move on if not end-of-file error
        jr      l2c0a           ; end of file, so move on to close file
.l2bed  ld      a,(dst_dev)     ; check destination flag
        cp      $aa
        ld      a,c             ; A=byte from file
        jr      nz,l2bff        ; move on unless copying to screen
        cp      $0d
        jr      z,l2bff         ; okay to output CRs
        cp      $20
        jr      nc,l2bff        
        ld      a,$20           ; but replace other control chars with spaces
.l2bff  call    l2b64           ; page in normal memory
        rst     $28
        defw    $0010           ; output byte
.l2c05  call    l2b89           ; page in DOS workspace
        jr      l2bd7           ; back for more
.l2c0a  ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error closing
        call    l2b64           ; page in normal memory
        ret                     ; done

; This part of the copy routine copies a file to another file

.l2c1e  push    af              ; save destination flag
        ld      hl,tmp_fspec
        ld      (dst_add),hl    ; store address of destination filespec
        ld      de,dst_file
        call    l3109           ; copy filespec, error if too long
        push    hl              ; save address of source filespec
        ld      a,$ff
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_DRIVE   ; set default drive
        call    l32ee           ; restore TSTACK
        ld      (dst_drv),a     ; use default drive for destination
        ld      (src_drv),a     ; use default drive for source
        ld      hl,dst_file
        call    l30f0           ; is destination drive specified?
        jr      nz,l2c4d        ; move on if not
        ld      de,dst_drv
        call    l30e3           ; place drive letter at dst_drv
.l2c4d  pop     hl              ; restore address of source filespec
        pop     af              ; restore destination flag
        jp      nc,l3123        ; move on if copying TO SPECTRUM FORMAT
        ld      (src_add),hl    ; save address of source filespec
        ld      de,src_file
        call    l3109           ; copy filespec, error if too long
        ld      hl,src_file
        call    l30f0           ; is source drive specified?
        jr      nz,l2c69        ; move on if not
        ld      de,src_drv
        call    l30e3           ; place drive letter at src_drv
.l2c69  ld      (SCR_CT),a      ; zeroise scroll count
        ld      a,$0d
        rst     $10             ; output CR
        xor     a
        ld      (wild),a        ; clear "wild" flag
        ld      (copied),a      ; zero # files copied
        ld      (dst_dev),a     ; destination is a file
        ld      hl,dst_file
        call    l30b6           ; check if destination wild
        ld      a,(wild)
        or      a
        jr      z,l2c8c         
        call    l2b64           ; if so, page in normal memory
        call    l2ada           ; and cause error "destination cannot be wild"
        defb    $49
.l2c8c  ld      hl,l3283
        ld      de,tmp_file
        ld      bc,$000e
        ldir                    ; copy temporary filename
        ld      hl,src_file
        call    l30b6           ; check if source wild
        ld      a,(wild)
        or      a
        jr      nz,l2ca9        ; move on if so
        call    l2d58           ; copy a single file
        jp      l2d26           ; finish up
.l2ca9  ld      hl,(dst_add)    ; get address of dest filespec
        call    l30f0           ; get past drive
        ld      a,$ff
        cp      (hl)
        jr      z,l2cbb         ; move on if just drive
        call    l2b64           ; page in normal memory
        call    l2ada
        defb    $4a             ; else error "destination must be drive"
.l2cbb  ld      hl,wld_next
        xor     a
        ld      b,$0d
.l2cc1  ld      (hl),a          ; zeroise directory entry 1
        inc     hl
        djnz    l2cc1
.l2cc5  ld      hl,wld_next
        ld      de,cat_spec
        ld      bc,$000d
        ldir                    ; and zeroise directory entry 0
        ld      hl,(src_add)
        ld      bc,$0200        ; 1 entry required, include system files
        ld      de,cat_spec
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CATALOG     ; get next filename
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      hl,dst_dev
        ld      a,(hl)
        or      a
        jr      nz,l2cf7        ; move on if not copying first file
        inc     a
        ld      (hl),a          ; set "first file copied" flag
        ld      a,$17
        dec     b
        jp      z,l3219         ; cause error "File not found" if none
        inc     b
.l2cf7  dec     b               ; B=0 if no more matches
        jr      z,l2d26         ; move to finish if done
        ld      hl,src_file
        call    l30f0           ; get past drive of source
        ex      de,hl
        ld      hl,wld_next     ; address of found entry
        ld      b,$08
        call    l30d9           ; copy filename part into source
        ld      hl,wld_next+8   ; get to extension of found entry
        ld      a,'.'
        ld      (de),a          ; insert "."
        inc     de
        ld      b,$03
        call    l30d9           ; copy extension part into source
        ld      a,$ff
        ld      (de),a          ; insert terminator
        ld      hl,dst_file
        call    l30f0           ; get past drive name in dest
        ld      (hl),$ff        ; insert terminator
        call    l2d58           ; copy a file
        jp      l2cc5           ; loop back for more

; Copy file routines - end part

.l2d26  ld      hl,tmp_file
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_DELETE      ; delete temp file
        call    l32ee           ; restore TSTACK
        ld      a,(copied)
        dec     a               ; A=# files copied-1
        ld      hl,l3291        ; "1 file copied" message
        jr      z,l2d4c         ; move on if 1
        inc     a
        ld      l,a
        ld      h,$00           ; HL=# files copied
        ld      a,$0d
        rst     $10             ; output CR
        ld      e,' '
        call    l0800           ; output #
        ld      hl,l32a5        ; "files copied" message
.l2d4c  call    l3268           ; output message
        ld      a,$17
        ld      (SCR_CT),a      ; set scroll count
        call    l2b64           ; page in normal memory
        ret                     ; done!

; Subroutine to copy a single file

.l2d58  ld      hl,dst_file     ; dest filespec
        ld      de,src_file     ; source filespec
.l2d5e  ld      a,(de)
        cp      (hl)            ; compare filespecs
        jr      nz,l2d72        ; move on if different
        ld      a,$ff
        cp      (hl)
        jr      nz,$2d6e      
        call    l2b64           ; page in normal memory
        call    l2ada
        defb    $30             ; error if filespecs the same
.l2d6e  inc     hl              ; increment pointers
        inc     de
        jr      l2d5e           ; loop back
.l2d72  ld      hl,dst_file
        call    l30f0           ; move past drive specifier in dest filespec
        ld      a,(hl)
        cp      $ff
        jr      nz,l2d94        ; move on if a destination filename specified
        ld      hl,dst_file
        call    l30f0
        push    hl              ; store address after drive specifier
        ld      hl,src_file
        call    l30f0           ; get to filename of source
        pop     de
.l2d8b  ld      a,(hl)
        ld      (de),a          ; copy filename to destination
        inc     a
        jr      z,l2d94         ; move on when done
        inc     de
        inc     hl
        jr      l2d8b           ; loop back
.l2d94  xor     a
        ld      (copy_ram),a    ; signal "copy via M:"
        ld      a,'M'
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_FREE_SPACE  ; find free space on drive M:
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,h
        or      a
        jr      z,l2daf
        ld      hl,$003f        ; use max 63K on drive M:
.l2daf  ld      a,l
        cp      $40
        jr      c,l2db7
        ld      hl,$003f        ; use max 63K on drive M:
.l2db7  ld      h,l
        ld      l,$00
        add     hl,hl
        add     hl,hl
        ld      (free_m),hl     ; store free bytes on drive M:
        ld      de,$0800
        or      a
        sbc     hl,de
        jr      nc,l2dd1        ; move on if >=2K free
        ld      a,$ff
        ld      (SCR_CT),a      ; set scroll count
        ld      a,$01
        ld      (copy_ram),a    ; signal "copy via RAM"
.l2dd1  xor     a
        ld      (dst_open),a    ; signal no temporary file open
        ld      (eof),a         ; signal not EOF
        ld      hl,dst_file
        call    l30f0		; get past drive of dest
        ld      a,(hl)
        cp      $ff
        jp      nz,l2e5d        ; if dest filename specified, jump on
        ld      hl,src_file
        call    l30f0		; get past drive of source
        ld      a,(hl)
.l2deb  cp      $ff
        jp      nz,l2e5d        ; if source filename specified, jump on
        ld      a,(dst_drv)     ; check destination drive
        cp      'M'             
        jp      z,l2e5d         ; move on if M: (will fail on attempted copy)
        ld      a,(src_drv)     ; check source drive
        cp      'M'
        jp      z,l2e5d         ; move on if M: (will fail on attempted copy)
        ld      a,'A'           ; by this stage, we must be copy A:<-->B:
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_XDPB    ; get XDPB for drive A:
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      c,0
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DD_LOGIN        ; login disk in drive A:
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        or      a
        ld      a,$06
        jp      nz,l3219        ; cause error 6 if not a standard +3 disk
        ld      a,(src_drv)     ; get source drive letter
        ld      bc,$0001
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN_DRIVE  ; open source drive as exclusive-read file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,(dst_drv)     ; get dest drive letter
        ld      bc,$0102
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN_DRIVE  ; open dest drive as exclusive-write file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,$01
        ld      (dst_open),a    ; signal temporary file open
        ld      a,(copy_ram)
        or      a
        jp      z,l2f44         ; copy via M: if >=2K free on drive M:
        jp      l2ecd           ; else copy via RAM
.l2e5d  ld      hl,src_file     ; source name
        ld      a,$ff
        ld      (SCR_CT),a	; set max scroll count
        push    hl
        push    hl
        call    l3268		; display filespec
        pop     de
        ex      de,hl
        or      a
        sbc     hl,de
        ld      de,$0011
        add     hl,de
        ld      b,l		; B=# spaces required
.l2e74  push    bc
        ld      a,' '
        rst     $10		; output a space
        pop     bc
        djnz    l2e74           ; loop back
        pop     hl
        ld      a,(dst_drv)     ; get dest drive letter
        or      $20		; make lowercase
        cp      'm'
        jr      z,l2e95         ; move on if copying to M:
        ld      a,(copy_ram)
        or      a
        jr      nz,l2e95        ; or if >=2K free on M:
        ld      a,(src_drv)     ; get source drive letter
        or      $20
        cp      'm'
        jp      nz,l2f2d	; if not copying from M:, move on
.l2e95  ld      hl,src_file     ; source filename
        ld      bc,$0001	; file 0,excl read
        ld      de,$0002	; must be openable
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN
        call    l32ee           ; restore TSTACK
        jp      nc,l3219	; move on if error
        ld      hl,dst_file     ; dest filename
        push    hl
        call    l3268		; display filename
        pop     hl
        ld      bc,$0102	; file 1, exc write
        ld      de,$0204	; create new file
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN
        call    l32ee           ; restore TSTACK
        jp      nc,l3219	; move on if error
        ld      a,$01
        ld      (dst_open),a    ; signal temporary file open

; Subroutine to copy everything from file 0 to file 1, via a 2K area
; in page 0 (bug: this should be page 7!)

.l2ecd  ld      bc,$0000        ; file 0, page 0 (oops, should be page 7!)
        ld      de,$0800        ; 2K to read
        ld      hl,tmp_buff     ; address to read
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_READ        ; read bytes
        call    l32ee           ; restore TSTACK
        jr      c,l2f08         ; move on if no error
        cp      $19
        jp      nz,l3219        ; if error not end-of-file, cause error
        ld      a,$01
        ld      (eof),a         ; signal end-of-file reached
.l2eed  push    de              ; save # unread bytes
        ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file 0
        call    l32ee           ; restore TSTACK
        pop     de
        jp      nc,l3219        ; move on if error
        ld      hl,$0800
        or      a
        sbc     hl,de
        ex      de,hl           ; DE=number of bytes read
        jr      l2f0b           ; move on
.l2f08  ld      de,$0800        ; DE=2048 bytes read
.l2f0b  ld      a,e
        or      d
        jr      z,l2f23         ; if no bytes read, move on
        ld      hl,tmp_buff
        ld      bc,$0100
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_WRITE       ; write bytes to file 1 from page 0 (oops)
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
.l2f23  ld      a,(eof)
        or      a
        jp      z,l2ecd         ; loop back if not end-of-file
        jp      l309e           ; close file 1 and exit

; Continuation of copy command, where M: is involved

.l2f2d  ld      hl,src_file     ; source filename
        ld      bc,$0001	; file 0, excl read
        ld      de,$0002	; must be openable
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN
        call    l32ee           ; restore TSTACK
        jp      nc,l3219	; move on if error

; Subroutine to copy everything from file 0 to file 1, via a temporary
; file in drive M:
; Each 2K is read via RAM in page 0 - this should be page 7 (oops!)

.l2f44  ld      hl,tmp_file     ; temporary filename
        ld      bc,$0203        ; file 2, exclusive read-write mode
        ld      de,$0204        ; open & create actions
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open temporary file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      hl,$0000
        ld      (tmp_bytes),hl  ; zero # bytes copied to temp file
.l2f61  ld      bc,$0000
        ld      de,$0800
        ld      hl,tmp_buff
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_READ        ; read 2K into RAM
        call    l32ee           ; restore TSTACK
        jr      c,l2f9c         ; move on if no error
        cp      $19
        jp      nz,l3219        ; cause error if it wasn't end-of-file
        ld      a,$01
        ld      (eof),a         ; signal end-of-file reached
        push    de              ; save # unread bytes
        ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file 0
        call    l32ee           ; restore TSTACK
        pop     de
        jp      nc,l3219        ; move on if error
        ld      hl,$0800
        or      a
        sbc     hl,de
        ex      de,hl           ; DE=# bytes read
        jr      l2f9f
.l2f9c  ld      de,$0800        ; DE=2048 bytes read
.l2f9f  ld      a,e
        or      d
        jr      z,l2fb9         ; move on if no bytes read
        push    de
        ld      hl,tmp_buff
        ld      bc,$0200
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_WRITE       ; write bytes to temporary file
        call    l32ee           ; restore TSTACK
        pop     de
        jp      nc,l3219        ; move on if error
.l2fb9  ld      hl,(tmp_bytes)
        add     hl,de
        ld      (tmp_bytes),hl  ; update number of bytes copied to temp file
        ld      de,$0800
        add     hl,de
        ex      de,hl
        ld      hl,(free_m)
        ld      a,(eof)
        or      a
        jr      nz,l2fd2        ; move on if end-of-file reached
        sbc     hl,de
        jr      nc,l2f61        ; loop back if temp file can take 2K more
.l2fd2  ld      a,(src_drv)
        and     $df             ; get source drive (capitalised)
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_FLUSH       ; flush for source drive
        call    l32ee           ; restore TSTACK        
        jp      nc,l3219        ; move on if error
        ld      b,$02
        ld      hl,$0000
        ld      e,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_SET_POSITION ; get to start of temp file
        call    l32ee           ; restore TSTACK
        ld      a,(dst_open)
        or      a
        jr      nz,l301e        ; move on if dst_file contains spec of temp file
        ld      hl,dst_file
        push    hl
        call    l3268           ; else display filespec
        pop     hl
        ld      bc,$0102
        ld      de,$0204
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open file 1 in exclusive-write mode
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,$01
        ld      (dst_open),a    ; signal dest file is open
.l301e  ld      hl,tmp_buff
        ld      de,$0800
        ld      bc,$0200
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_READ        ; read 2K from temp file
        call    l32ee           ; restore TSTACK
        ld      hl,$0800        ; HL=$0800 bytes read
        jr      c,l3042         ; move on if no error
        cp      $19
        jp      nz,l3219        ; cause non-EOF errors
        ld      hl,$0800
        or      a
        sbc     hl,de           ; HL=# bytes read
.l3042  ex      de,hl
        ld      bc,$0100
        ld      hl,tmp_buff
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_WRITE       ; write bytes to file 1
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      hl,(tmp_bytes)
        ld      de,$0800
        or      a
        sbc     hl,de
        jr      c,l3069         ; move on if temp file empty
        ld      a,h
        or      l
        ld      (tmp_bytes),hl  ; update bytes left in temp file
        jr      nz,l301e        ; loop back to copy more
.l3069  ld      b,$02
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close temp file
        call    l32ee           ; restore TSTACK
        ld      a,(dst_drv)
        and     $df             ; get dest drive (capitalised)
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_FLUSH       ; flush dest drive
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,(eof)
        or      a
        jp      z,l2f44         ; loop back if not EOF
        ld      hl,tmp_file
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_DELETE      ; delete temp file
        call    l32ee           ; restore TSTACK
        
; Enter here if copying via 2K area in RAM

.l309e  ld      b,$01
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close file 1
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,$0d
        rst     $10             ; output CR
        ld      hl,copied
        inc     (hl)            ; increment # files copied
        ret     

; Subroutine to check whether filespec at HL is wild
; Causes error if filespec longer than $11 (inc terminator)

.l30b6  ld      b,$11
        ld      a,(hl)
        cp      '?'
        jr      nz,l30c4        ; move on if not ? wildcard
        push    af
        ld      a,$01
        ld      (wild),a        ; set wildcard flag
        pop     af
.l30c4  cp      '*'
        jr      nz,l30cf        ; move on if not * wildcard
        push    af
        ld      a,$01
        ld      (wild),a        ; set wildcard flag
        pop     af
.l30cf  inc     hl              ; increment pointer
        inc     a
        ret     z               ; exit if done
        djnz    l30b6           ; loop back
        ld      a,$14
        jp      l3219           ; cause bad filename error if too long

; Subroutine to copy up to B chars from HL to DE, stopping at first space

.l30d9  ld      a,(hl)          ; get next char
        cp      ' '
        ret     z               ; exit if space
        ld      (de),a          ; copy char
        inc     hl              ; increment pointers
        inc     de
        djnz    l30d9           ; loop back
        ret     

; Subroutine to get a drive letter from a filespec & place it
; in the address at DE. HL points past the colon of the filespec

.l30e3  dec     hl
        dec     hl
        ld      a,(hl)          ; get character before colon
        or      $20             ; make lowercase
        cp      'a'     
        ret     c               ; exit if < 'a'
        cp      '{'             
        ret     nc              ; or if > 'z'
        ld      (de),a          ; store drive letter
        ret     

; Subroutine to check if filespec includes drive specification
; On entry, HL=address of filespec
; On exit, Z flag set if drive specified, and HL points to
; start of filename after colon.

.l30f0  push    hl
        pop     de              ; copy address of filename to DE
        ld      a,(hl)          ; get first char
        inc     a
        jr      z,l3103         ; move to exit if end of filename
        ld      b,$03           ; check first 3 chars
.l30f8  ld      a,(hl)
        cp      ':'             ; is char a ':' ?
        jr      z,l3107         ; move on if so
        inc     a
        jr      z,l3103         ; exit if end of filename
        inc     hl
        djnz    $30f8           ; back for more
.l3103  or      $ff             ; reset Z flag - no drive specified
        ex      de,hl           ; HL=start of filename
        ret     
.l3107  inc     hl              ; HL=start of filename after drive spec
        ret                     ; exit with Z set

; Subroutine to copy a $ff-terminated filename from HL to DE
; If max length of $11 (inc terminator) exceeded, cause error

.l3109  ld      b,$11           ; allow 17 characters in a filename
        ld      a,(hl)
        ld      (de),a          ; copy filename
        inc     hl              ; increment pointers
        inc     de
        inc     a               ; test for end of filename
        jr      z,l3119         ; exit if found
        djnz    l3109           ; loop back
        ld      a,$14 
        jp      l3219           ; cause +3DOS error $14, "Bad filename"
.l3119  ret
        
        
; Subroutine to clear screen and open channel to stream 2
        
.l311a  rst     $28
        defw    $0d6b           ; cls
        ld      a,$02
        rst     $28
        defw    $1601           ; open channel to stream 2
        ret

; Routine to copy files to spectrum format

.l3123  xor     a
        ld      (wild),a        ; no wildcard
        ld      (dst_open),a    ; dest not open file
        ld      hl,dst_file
        call    l30b6           ; is dest filespec wild?
        ld      a,(wild)
        or      a
        jr      z,l313d         ; move on if not
        call    l2b64           ; page in normal memory
        call    l2ada
        defb    $49             ; else error "destination cannot be wild"
.l313d  ld      hl,dst_file
        ld      b,$12
.l3142  ld      a,(hl)
        cp      '.'             ; has file got extension?
        inc     hl
        jr      z,l314b         ; move on if so
        inc     a
        jr      nz,l3142
.l314b  dec     hl
        ex      de,hl
        ld      hl,l3214
        ld      bc,$0004        ; length 4 misses terminator (oops!)
        ldir                    ; copy ".HED" extension    
        ld      hl,(dst_add)
        ld      bc,$0001        ; file 0, exclusive read
        ld      de,$0001
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open source file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      hl,dst_file     ; dest filename
        ld      bc,$0102        ; file 1, exclusive write
        ld      de,$0104
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_OPEN        ; open dest file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,$01
        ld      (dst_open),a    ; signal dest open
        ld      hl,$0000
        ld      (tmp_bytes),hl  ; signal 0 bytes copied
.l318e  ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_BYTE_READ   ; read a byte
        call    l32ee           ; restore TSTACK
        jr      c,l31a4         ; move on if no error
        cp      $19
        jp      nz,l3219        ; cause non-EOF error
        jr      z,l31bd         ; move on
.l31a4  ld      b,$01
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_BYTE_WRITE  ; write byte
        call    l32ee           ; restore TSTACK
        ld      hl,(tmp_bytes)
        inc     hl              ; update bytes copied
        ld      (tmp_bytes),hl
        jr      c,l318e         ; loop back if no error
        jp      l3219           ; cause error
.l31bd  ld      b,$00
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close source file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        ld      a,(dst_drv)
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_FLUSH       ; flush dest drive
        call    l32ee           ; restore TSTACK
        ld      b,$01
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_REF_HEAD    ; point at header data for dest file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219
        ld      a,$03
        ld      (ix+$00),a      ; set CODE type
        ld      hl,(tmp_bytes)
        ld      (ix+$01),l
        ld      (ix+$02),h      ; set length
        xor     a
        ld      (ix+$03),a      ; set load address to zero
        ld      (ix+$04),a
        ld      b,$01
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; close dest file
        call    l32ee           ; restore TSTACK
        jp      nc,l3219        ; move on if error
        call    l2b64           ; page in normal memory
        ret                     ; done

.l3214  defm    ".HED"&$ff

; Routine to close files 0-2, delete temporary files and
; generate the +3DOS error held in A

.l3219  push    af              ; save +3DOS error code
        ld      b,$03           ; three files
.l321c  push    bc              ; stack counter
        dec     b               ; decrement counter
        push    bc              ; stack file number
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_CLOSE       ; try to close file B
        call    l32ee           ; restore TSTACK
        pop     bc              ; restore file number
        jr      c,l3238         ; move on if closed okay
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_ABANDON     ; else abandon it
        call    l32ee           ; restore TSTACK
.l3238  pop     bc
        djnz    l321c           ; back for other files
        ld      a,$0d
        rst     $10             ; new line on screen
        ld      a,(dst_open)
        or      a              
        jr      z,l3252         ; move on if no temporary file created
        ld      hl,dst_file     ; HL=address of temporary filename
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_DELETE      ; delete temporary file
        call    l32ee           ; restore TSTACK
.l3252  ld      hl,tmp_file
        call    l32b6           ; save TSTACK in page 7
        call    l3f00
        defw    DOS_DELETE      ; delete other temporary file
        call    l32ee           ; restore TSTACK
        pop     af              ; restore +3DOS error code
        call    l2b64           ; page in normal memory
        call    l0e9a           ; cause +3DOS error
        defb    $ff

; Subroutine to display filename/message at HL

.l3268  ld      a,(hl)          ; get next char
        inc     hl
        or      a
        ret     z               ; exit if null
        cp      $ff
        ret     z               ; or $ff
        and     $7f
        rst     $10             ; display character
        jr      l3268           ; loop back


; Subroutine to get a key (apparently unused)

.l3274  ld      hl,FLAGS
        res     5,(hl)		; set "no key"
.l3279  bit     5,(hl)
        jr      z,l3279         ; loop until key available
        res     5,(hl)		; set "no key"
        ld      a,(LAST_K)	; get it
        ret     

; Temporary filespec, used in COPY

.l3283  defm    "M:VAXNSUZ.$$$"&$ff

; Files copied messages

.l3291  defm    $0d&"  1 file copied."&$0d&$0d&0
.l32a5  defm    " files copied."&$0d&$0d&0

; Subroutine to copy TSTACK to a temporary area in page 7, and
; reset SP to use whole of TSTACK again

.l32b6  di
        ld      (tmp_hl),hl     ; save HL
        push    af
        pop     hl
        ld      (tmp_af),hl     ; save AF
        ld      (tmp_de),de     ; save DE
        ld      (tmp_bc),bc     ; save BC
        ld      hl,TSTACK
        ld      de,tmp_stack
        ld      bc,$0084
        lddr                    ; copy TSTACK area into page 7
        pop     bc              ; BC=return address
        ld      (tmp_sp),sp     ; save SP
        ld      hl,TSTACK        
        ld      sp,hl           ; set SP back to top of TSTACK
        push    bc              ; restack return address
        ld      bc,(tmp_bc)     ; restore BC
        ld      de,(tmp_de)     ; restore DE
        ld      hl,(tmp_af) 
        push    hl
        pop     af              ; restore AF
        ld      hl,(tmp_hl)     ; restore HL
        ei      
        ret     

; Subroutine to restore TSTACK from where it's been saved in a temporary
; area in page 7

.l32ee  di
        ld      (tmp_hl),hl     ; save HL
        push    af
        pop     hl
        ld      (tmp_af),hl     ; save AF
        ld      (tmp_de),de     ; save DE
        ld      (tmp_bc),bc     ; save BC
        pop     hl
        ld      (tmp_ret),hl    ; save return address
        ld      hl,tmp_stack
        ld      de,TSTACK
        ld      bc,$0084
        lddr                    ; restore TSTACK from saved location
        ld      hl,(tmp_sp)
        ld      sp,hl           ; restore SP
        ld      hl,(tmp_ret)
        push    hl              ; restack return address
        ld      bc,(tmp_bc)     ; restore BC
        ld      de,(tmp_de)     ; restore DE
        ld      hl,(tmp_af)
        push    hl
        pop     af              ; restore AF
        ld      hl,(tmp_hl)     ; restore HL
        ei      
        ret     

; The COPY EXP command

.l3328  xor     a
        call    l2b89           ; page in DOS workspace
        ld      (tmp_buff+5),a  ; flag "normal copy exp"
        call    l2b64           ; page in normal memory
        rst     $28
        defw    $0020           ; get next character
        cp      $dd
        jr      nz,l3347        ; move on if not INVERSE
        ld      a,$fc
        call    l2b89           ; page in DOS workspace
        ld      (tmp_buff+5),a  ; flag "inverse copy exp"
        call    l2b64           ; page in normal memory
        rst     $28
        defw    $0020           ; get to next char
.l3347  call    l10b1           ; check for end-of-statement
        ld      a,(BANK678)
        ld      bc,$1ffd
        set     4,a             ; set strobe high
        di      
        ld      (BANK678),a
        out     (c),a           ; output strobe
        ei      
        call    l2b89           ; page in DOS workspace
        di      
        ld      a,$1b           ; set DUMPLF/216" linespacing
        call    l33b9
        ld      a,'3'
        call    l33b9
        ld      hl,DUMPLF
        ld      a,(hl)
        call    l33b9
        ld      hl,$401f        ; address of top right corner of display
        ld      e,$20           ; number of chars per line
.l3373  push    hl
        ld      d,$01           ; start with bit 0 pixel
.l3376  push    de
        push    hl
        ld      hl,l34bf
        call    l33c5           ; output raster line header
        pop     hl
        pop     de
        push    hl
.l3381  call    l33d1           ; output raster data for next two pixels
        ld      a,h
        and     $07             
        inc     h               ; get to next pixel line down
        cp      $07
        jr      nz,l3381        ; loop back if still in same character line
        ld      a,h
        sub     $08             ; back to top line of a character
        ld      h,a
        ld      a,l
        add     a,$20           ; move to next character line down
        ld      l,a
        jr      nc,l3381        ; loop back if same screen third
        ld      a,h
        add     a,$08           ; increment screen third
        ld      h,a
        cp      $58
        jr      nz,l3381        ; loop back if all thirds not done
        pop     hl              ; restore top of screen address
        sla     d
        sla     d               ; shift left two pixels
        jr      nc,l3376        ; loop back if within same char
        pop     hl              ; restore top of screen address
        dec     hl              ; previous character
        dec     e
        jr      nz,l3373        ; loop back if not finished
        ld      a,$1b           ; reset printer
        call    l33b9
        ld      a,'@'
        call    l33b9
        ei      
        call    l2b64           ; page in normal memory
        ret                     ; done

; Subroutine to page in normal memory, output a character to the
; printer, and page back DOS workspace

.l33b9  ei
        call    l2b64           ; page in normal memory
        call    l2051           ; output char to printer
        call    l2b89           ; page in DOS workspace
        di      
        ret     

; Subroutine to output a $ff-terminated string to the printer

.l33c5  ld      a,(hl)          ; get next char
        cp      $ff
        ret     z               ; exit if $ff
        push    hl
        call    l33b9           ; output char
        pop     hl
        inc     hl
        jr      l33c5           ; loop back

; Subroutine to output 4 raster bytes for the next 2 pixels

.l33d1  push    af              ; save registers
        push    hl
        push    de
        push    hl
        call    l3402           ; E=attribute for address in HL
        pop     hl
        call    l3412           ; clear 4-byte buffer
        call    l343f           ; copy appropriate pattern for pixel to buffer
        call    l341f           ; shift pattern left 3 bits
        sla     d               ; shift pixel number
        call    l343f           ; merge in pattern for next pixel
        call    l3432           ; shift patterns left 2 bits
        ld      b,$04           ; get ready to output 4 raster bytes
        ld      hl,tmp_buff
.l33ef  ld      a,(hl)          ; get pattern
        push    bc
        push    hl
        ld      hl,tmp_buff+5
        xor     (hl)            ; invert if required
        call    l33b9           ; output byte
        pop     hl
        pop     bc
        inc     hl
        djnz    l33ef           ; loop back
        pop     de
        pop     hl
        pop     af
        ret     

; Subroutine to get attribute byte in E for screen address in HL

.l3402  push    af
        ld      a,h
        and     $18
        srl     a
        srl     a
        srl     a
        or      $58             ; address attribs
        ld      h,a
        ld      e,(hl)          ; get attrib
        pop     af
        ret     

; Subroutine to clear a 4-byte area at tmp_buff

.l3412  push    hl
        ld      hl,tmp_buff
        ld      b,$04
.l3418  ld      (hl),$00        ; clear the buffer
        inc     hl
        djnz    l3418
        pop     hl
        ret     

; Subroutine to shift patterns in buffer left 3 bits

.l341f  push    hl
        push    bc
        ld      hl,tmp_buff
        ld      b,$04
.l3426  sla     (hl)            ; shift left
        sla     (hl)
        sla     (hl)
        inc     hl
        djnz    l3426
        pop     bc
        pop     hl
        ret     

; Subroutine to shift patterns in buffer left 2 bits

.l3432  ld      hl,tmp_buff
        ld      b,$04
.l3437  sla     (hl)            ; shift left
        sla     (hl)
        inc     hl
        djnz    l3437
        ret     

; Subroutine to merge required pattern for pixel into buffer at tmp_buff

.l343f  push    de              ; save registers
        push    hl
        ld      a,d             
        and     (hl)            ; mask required pixel
        ld      a,e             ; A=attribute
        jr      nz,l344c        ; move on if need ink
        srl     a               ; shift paper colour to ink position
        srl     a
        srl     a
.l344c  and     $07             ; mask off ink/paper colour as required
        bit     6,e             ; check BRIGHT
        jr      z,l3454
        or      $08             ; add 8 if bright
.l3454  ld      hl,l346f        ; address of colour offsets table
        ld      d,$00
        ld      e,a
        add     hl,de
        ld      e,(hl)          ; DE=offset into pattern table
        ld      hl,l347f
        add     hl,de           ; HL=required pattern address
        ld      b,$04
        ld      de,tmp_buff
.l3465  ld      a,(de)
        or      (hl)
        ld      (de),a          ; merge pattern into buffer
        inc     hl
        inc     de
        djnz    l3465
        pop     hl              ; restore registers
        pop     de
        ret     

; Table of offsets into following pattern table

.l346f  defb    $00,$04,$08,$0c
        defb    $10,$14,$18,$1c
        defb    $20,$24,$28,$2c
        defb    $30,$34,$38,$3c

; Pattern table for expanded copy
        
.l347f  defb    $07,$07,$07,$07 ; black
        defb    $07,$05,$07,$07 ; blue
        defb    $03,$07,$06,$07 ; red
        defb    $07,$03,$06,$03 ; magenta
        defb    $06,$03,$06,$03 ; green
        defb    $06,$05,$02,$05 ; cyan
        defb    $02,$05,$02,$05 ; yellow
        defb    $01,$06,$03,$04 ; white
        defb    $07,$07,$07,$07 ; black
        defb    $05,$02,$03,$04 ; bright blue
        defb    $06,$01,$02,$01 ; bright red
        defb    $01,$04,$02,$04 ; bright magenta
        defb    $04,$00,$04,$01 ; bright green
        defb    $01,$00,$04,$00 ; bright cyan
        defb    $00,$02,$00,$00 ; bright yellow
        defb    $00,$00,$00,$00 ; bright white

; Raster line header for expanded copy
        
.l34bf  defb    $0d,$0a         ; CRLF
        defb    $1b,'L',$00,$03 ; 768 bytes in 120dpi mode
        defb    $ff

; CAT "T:" routine

.l34c6  ld      bc,$0011
        rst     $28
        defw    $0030           ; make space for tape header
        push    de
        pop     ix              ; IX=address of space
.l34cf  ld      a,$0d
        rst     $28
        defw    $0010           ; output CR
.l34d4  ld      a,$7f
        in      a,($fe)
        rra     
        jr      c,l34e3         ; move on if BREAK not pressed
        ld      a,$fe
        in      a,($fe)
        rra     
        jr      c,l34e3         ; move on if BREAK not pressed
        ret                     ; done
.l34e3  ld      a,$00
        ld      de,$0011
        scf     
        push    ix
        rst     $28
        defw    $0556           ; read a header
        pop     ix
        jr      nc,l34d4        ; loop back if failed
        push    ix
        ld      a,'"'
        rst     $28
        defw    $0010           ; output quote
        ld      b,$0a           ; name length 10
.l34fb  ld      a,(ix+$01)
        rst     $28
        defw    $0010           ; output next byte
        inc     ix              
        djnz    l34fb           ; loop back
        pop     ix
        ld      hl,l35a1
        call    l3591           ; output quote and space
        ld      a,(ix+$00)      ; get file type
        cp      $00
        jr      nz,l3537        ; move on if not program
        ld      a,(ix+$0e)
        cp      $80
        jr      z,l352f         ; move on if no auto-run line number
        ld      hl,l35be
        call    l3591           ; display "LINE" message
        ld      c,(ix+$0d)
        ld      b,(ix+$0e)
        call    l359a           ; output line number
        ld      a,' '
        rst     $28
        defw    $0010           ; output space
.l352f  ld      hl,l35a4
        call    l3591           ; output "BASIC" message
        jr      l34cf           ; loop back
.l3537  cp      $01
        jr      nz,l3554        ; move on if not number array
        ld      hl,l35ad
        call    l3591           ; output "DATA" message
        ld      a,(ix+$0e)
        and     $7f
        or      $40
        rst     $28
        defw    $0010           ; output variable name
        ld      hl,l35b9+1
        call    l3591           ; output "()" message
        jp      l34cf           ; loop back
.l3554  cp      $02
        jr      nz,l3571        ; move on if not character array
        ld      hl,l35ad
        call    l3591           ; output "DATA" message
        ld      a,(ix+$0e)
        and     $7f
        or      $40
        rst     $28
        defw    $0010           ; output variable name
        ld      hl,l35b9
        call    l3591           ; output "$()" message
        jp      l34cf           ; loop back
.l3571  ld      hl,l35b3
        call    l3591           ; output "CODE" message
        ld      c,(ix+$0d)
        ld      b,(ix+$0e)
        call    l359a           ; output load address
        ld      a,','
        rst     $28
        defw    $0010           ; output comma
        ld      c,(ix+$0b)
        ld      b,(ix+$0c)
        call    l359a           ; output length
        jp      l34cf           ; loop back

; Subroutine to output a null-terminated string

.l3591  ld      a,(hl)          ; get next char
        or      a
        ret     z               ; exit if null
        rst     $28
        defw    $0010           ; output char
        inc     hl
        jr      l3591           ; loop back

; Subroutine to output number in BC

.l359a  rst     $28
        defw    $2d2b           ; stack number on calculator
        rst     $28
        defw    $2de3           ; output number
        ret     

; Messages for tape catalogs
        
.l35a1  defm    $22&" "&$00
.l35a4  defm    "(BASIC) "&0
.l35ad  defm    "DATA "&0
.l35b3  defm    "CODE "&0
.l35b9  defm    "$() "&0
.l35be  defm    "LINE "&0

; The "COPY RANDOMIZE" command
; This is a silly command

.l35c4  ld      c,$40           ; loop timing values
.l35c6  ld      b,$00
.l35c8  ld      a,$fe
        in      a,($fe)
        bit     3,a
        jr      nz,l35dc        ; move on if "C" not pressed
        ld      a,$bf
        in      a,($fe)
        bit     3,a
        jr      nz,l35dc        ; move on if "J" not pressed
        bit     1,a             ; move on if "L" not pressed
        jr      z,l35e5         ; go to silly routine if "CJL" held
.l35dc  djnz    l35c8           ; loop back
        dec     c
        jr      nz,l35c6        ; loop back
        call    l2ada
        defb    $0b             ; nonsense in BASIC error
.l35e5  xor     a
        out     ($fe),a         ; black border
        ld      hl,$5800
        ld      de,$5801
        ld      bc,$02ff
        ld      (hl),$00
        ldir                    ; black screen
        ld      hl,$4000
        ld      de,$4001
        ld      bc,$17ff
        ld      (hl),$ff
        ldir                    ; fill screen with ink
        ld      c,$07           ; base ink colour
.l3604  ld      a,c
        and     $07
        jr      nz,l360c
        inc     c               ; increment base ink colour
        jr      l3604
.l360c  ld      d,$58           ; high byte of start of attribs
        ld      hl,l3631        ; table of attrib offsets
.l3611  ld      e,(hl)
        push    af
        ld      a,e
        or      a
        jr      nz,l3624        ; if not end of third, move on
        inc     d               ; get to next third
        ld      a,d
        cp      $5b             
        jr      nz,l3621        ; move on if still in attribs area
        pop     af
        inc     c               ; increment base ink colour
        jr      l3604           ; loop back
.l3621  pop     af
        jr      l362e           ; go to loop back for another offset
.l3624  pop     af
        and     $07
        or      a
        jr      nz,l362c        ; is ink colour 0 or 8?
        ld      a,$07           ; set to white if so
.l362c  ld      (de),a          ; set attribute
        dec     a               ; decrement ink colour
.l362e  inc     hl              ; increment table pointer 
        jr      l3611           ; loop back

; The table of attribute positions for the COPY RANDOMIZE routine

.l3631  defb    $21,$41,$61,$81
        defb    $a1,$c1,$e1,$82
        defb    $83,$84,$25,$45
        defb    $65,$85,$a5,$c5
        defb    $e5,$27,$47,$67
        defb    $87,$a7,$c7,$e7
        defb    $28,$29,$2a,$2b
        defb    $88,$89,$8a,$e8
        defb    $e9,$ea,$eb,$2d
        defb    $4d,$6d,$8d,$ad
        defb    $cd,$ed,$ee,$ef
        defb    $f0,$f1,$33,$53
        defb    $73,$93,$b3,$d3
        defb    $f3,$f4,$f5
        defb    $f6,$f7,$59,$79
        defb    $99,$b9,$d9,$3a
        defb    $3b,$3c,$3d,$fa
        defb    $fb,$fc,$fd,$5e
        defb    $7e,$9e,$be,$de
        defb    $00              ; end of first screen third
        defb    $21,$22,$23,$24
        defb    $25,$43,$63,$83
        defb    $a3,$c3,$e3,$27
        defb    $47,$67,$87,$a7
        defb    $c7,$e7,$88,$89
        defb    $8a,$2b,$4b,$6b
        defb    $8b,$ab,$cb,$eb
        defb    $2d,$4d,$6d,$8d
        defb    $ad,$cd,$ed,$2e
        defb    $2f,$30,$31,$8e
        defb    $8f,$90,$ee,$ef
        defb    $f0,$f1,$33,$53
        defb    $73,$93,$b3,$d3
        defb    $f3,$34,$35
        defb    $36,$94,$95,$96
        defb    $57,$77,$b5,$d6
        defb    $f7,$39,$59,$79
        defb    $99,$b9,$d9,$f9
        defb    $3a,$3b,$3c,$3d
        defb    $9a,$9b,$9c,$fa
        defb    $fb,$fc,$fd,$00 ; end of second screen third
        defb    $20,$40,$60,$80
        defb    $a0,$c0,$e0,$22
        defb    $24,$44,$64,$84
        defb    $a4,$c4,$e4,$45
        defb    $66,$47,$28,$48
        defb    $68,$88,$a8,$c8
        defb    $e8,$4c,$6c,$8c
        defb    $ac,$cc,$ec,$2d
        defb    $2e,$2f,$8d,$8e
        defb    $8f,$50,$70,$90
        defb    $b0,$d0,$f0,$94
        defb    $95,$96,$97,$98
        defb    $56,$76,$b6,$d6
        defb    $5a,$3b,$3c,$3d
        defb    $5e,$7e,$9d,$9c
        defb    $be,$de,$fd,$fc
        defb    $fb,$da,$00     ; end of table

.l3713  defs    $19

.l372c  defs    $02d4

; The printer input (l3a00) and output (l3a05) routines
; This is a copy of a routine in ROM 3 which switches in this ROM,
; at which point it takes over

.l3a00  ld      hl,l3d03        ; printer input routine
        jr      l3a08
.l3a05  ld      hl,l3d06        ; printer output routine
.l3a08  ex      af,af'
        ld      bc,$1ffd
        ld      a,(BANK678)
        push    af
        and     $fb             ; select ROM 1
        di      
        ld      (BANK678),a
        out     (c),a
        jp      l3d00           ; this ROM takes over at this point
.l3a1b  ex      af,af'
        pop     af
        ld      bc,$1ffd
        di      
        ld      (BANK678),a     ; previous value
        out     (c),a           ; at this point, control passes back to ROM 3
        ei      
        ex      af,af'
        ret     

        defs    $02d7

.l3d00  jp      l2b09           ; call the required routine
.l3d03  jp      l1e70           ; jump to input routine
.l3d06  jp      l1f6e           ; jump to output routine

        defs    $0177

; Subroutine to call a subroutine in ROM 0
; The subroutine address is inline after the call to this routine

.l3e80  ld      (OLDHL),hl      ; save HL in OLDHL
        ld      (OLDBC),bc      ; save BC in OLDBC
        push    af
        pop     hl
        ld      (OLDAF),hl      ; save AF in OLDAF
        ex      (sp),hl         ; HL=address of inline address
        ld      c,(hl)
        inc     hl
        ld      b,(hl)          ; BC=inline ROM 0 address
        inc     hl
        ex      (sp),hl         ; stack return address
        push    bc
        pop     hl              ; HL=routine address in ROM 0
        ld      a,(BANKM)
        and     $ef
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page in ROM 0

; The rest of the routine continues at $3ea2 in ROM 0
; The following is a continuation of a mirrored routine in ROM 0 for
; calling this ROM

.l3ea2  ei
        ld      bc,$3eb5
        push    bc              ; stack return add to swap back ROMs
        push    hl              ; stack routine address
        ld      hl,(OLDAF)
        push    hl
        pop     af              ; restore AF
        ld      bc,(OLDBC)      ; restore BC
        ld      hl,(OLDHL)      ; restore HL
        ret                     ; execute routine in this ROM

; This part is the routine which returns control to ROM 0

.l3eb5  push    af              ; save AF & BC
        push    bc
        ld      a,(BANKM)
        and     $ef
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page back ROM 0

; The rest of the routine continues at $3ec5 in ROM 0
; The following is a continuation of a mirrored routine in ROM 0 for
; returning to this ROM

.l3ec5  ei
        pop     bc              ; restore registers
        pop     af
        ret                     ; return
                                
        defs    $37

; Subroutine to call a subroutine in ROM 2
; The subroutine address is inline after the call to this routine
; This routine is duplicated in ROMs 0 & 2, so that when we start switching
; (first to ROM 0, then to ROM 2) there is no problem.

.l3f00  ld      (OLDHL),hl      ; save HL,BC and AF
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
        pop     hl              ; HL=address to call in ROM
        ld      a,(BANKM)
        and     $ef
        di      
        ld      (BANKM),a
        ld      bc,$7ffd
        out     (c),a           ; page in ROM 0
        ld      a,(BANK678)
        or      $04
        ld      (BANK678),a
.l3f2a  ld      bc,$1ffd
        out     (c),a           ; page in ROM 2
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

; This part of the routine returns control to ROM 1

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

; Subroutine to copy a block of memory from HL in page 0 to
; DE in page 7 (length BC bytes)

.l3f63  di                      ; ensure interrupts disabled
        exx     
        ld      bc,$7ffd        ; BC'=paging port
        exx     
.l3f69  exx
        ld      a,$10
        out     (c),a           ; page in page 0
        exx     
        ld      a,(hl)
        ex      af,af'          ; get A'=byte from page 0
        exx     
        ld      a,$17
        out     (c),a           ; page in page 7
        exx     
        ex      af,af'
        ld      (de),a          ; store byte from page 0 in page 7
        inc     hl              ; increment addresses
        inc     de
        dec     bc              ; decrement counter
        ld      a,b
        or      c
        jr      nz,l3f69        ; loop back for more
        ld      a,(BANKM)
        ld      bc,$7ffd
        out     (c),a           ; page in previous memory
        ei                      ; enable interrupts
        ret     

; Subroutine to copy a block of memory from HL in page 7 to
; DE in page 0 (length BC bytes)

.l3f8a  di
        exx     
        ld      bc,$7ffd        ; BC'=paging port
        exx     
.l3f90  exx
        ld      a,$17
        out     (c),a           ; page in page 7
        exx     
        ld      a,(hl)
        ex      af,af'          ; A'=byte from page 7
        exx     
        ld      a,$10
        out     (c),a           ; page in page 0
        exx     
        ex      af,af'
        ld      (de),a          ; store byte in page 0
        inc     hl              ; increment addresses
        inc     de
        dec     bc              ; decrement pointers
        ld      a,b
        or      c
        jr      nz,l3f90        ; loop back for more
        ld      a,(BANKM)
        ld      bc,$7ffd
        out     (c),a           ; page in previous memory
        ei                      ; enable interrupts
        ret

; Unused space
	
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
	defb	$ff,$ff,$ff,$ff,$ff,$ff,$72

; ----------------------------------------------------------------------------------------------------

; ============================
; PLAY command data structures
; ============================
; 
; During execution of the PLAY command, an area of memory $3c bytes long plus
; $37 bytes per string is reserved. IY is used to point to the "overhead" area,
; and IX points to the data for the string currently being considered.
; A maximum of 8 strings are allowed.
; 
; 
; Overhead Area: IY+n
; -------------------
; 
; Offset  Length  Description
; ------  ------  -----------
; +00     10      Address of data area for each string
; +10     1       String presence flags (bit reset if string in use)
; +11     10      Note length of current note for each string
; +21     1       String counter
; +22     1       String presence flags, shifted with string counter
; +23     2       Address of pointer to string data area
; +25     2       Shortest current note length (=length to play)
; +27     2       Tempo value
; +29     1       Waveform number for volume effects
; +2a     1       Notes changed flag
; +2b     0d      FP routine used to calculate tempo value
; +38     4       Unused
; 
; 
; String Data Areas: IX+n
; -----------------------
; 
; Offset  Length  Description
; ------  ------  -----------
; +00     1       Current semitone number
; +01     1       $ff, or MIDI channel 0-15
; +02     1       String number 0-7
; +03     1       Base semitone number (12*octave)
; +04     1       Volume level (bit 4 set=use volume effect)
; +05     1       Note length
; +06     2       String interpretation pointer
; +08     2       String end+1
; +0a     1       Infinite repeat flag
; +0b     1       Open bracket depth
; +0c     2       String start
; +0e     8       Opening bracket addresses (max depth 4)
; +16     1       Close bracket depth
; +17     0a      Close bracket addresses (max depth 5)       
; +21     1       # tied notes
; +22     16      Note lengths for tied notes (max 11)

