; *************************************************
; *** SPECTRUM +3 ROM 0 DISASSEMBLY (+3DOS ROM) ***
; *************************************************

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
; John Elliott
;
; The ROM disassembly was created with the aid of dZ80 V1.10, and incorporates work from
; "The canonical list of +3 oddities" by Ian Collier. 

; -----------------
; Assembler Details
; -----------------

; This file can be assembled to produce a binary image of the ROM
; with Interlogic's Z80ASM assembler (available for Z88, QL, DOS and Linux).
; Note that the defs directive is used and this causes a block of $00 bytes to be created.

        module  rom2

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

        org     $0000

.l0000  defs    8
.l0008  defm    "PLUS3DOS"
.l0010  defs    40

; The maskable interrupt routine

.l0038  push    af
        push    hl
        ld      hl,(FRAMES)
        inc     hl              ; increment FRAMES
        ld      (FRAMES),hl                      
        ld      a,h
        or      l
        jr      nz,l0048
        inc     (iy+$40)        ; increment high byte of FRAMES
.l0048  push    bc
        push    de
        call    l2358           ; scan the keyboard
        call    l0068           ; test for disk motor timeout
        pop     de
        pop     bc
        pop     hl
        pop     af
        ei      
        ret

        defs    16

; The Non-maskable interrupt

.l0066  retn                    ; do nothing

; The disk motor timeout subroutine

.l0068  ld      bc,$7ffd
        ld      a,(BANKM)
        or      $07
        out     (c),a           ; page in page 7
        ld      a,(timeout)
        or      a
        jr      z,l0095         ; exit if motor already off
        ld      a,(FRAMES)
        bit     0,a
        jr      nz,l0095        ; only decrement count every other frame
        ld      a,(timeout)
        dec     a               ; decrement timeout count
        ld      (timeout),a
        jr      nz,l0095        ; exit if not yet zero
        ld      bc,$1ffd
        ld      a,(BANK678)
        and     $f7
        ld      (BANK678),a
        out     (c),a           ; turn motor off
.l0095  ld      bc,$7ffd
        ld      a,(BANKM)
        out     (c),a           ; restore memory configuration
        ret

        defs    98


; The DOS routines jump block

.l0100  jp      l019f           ; DOS_INITIALISE
.l0103  jp      l01cd           ; DOS_VERSION
.l0106  jp      l062d           ; DOS_OPEN
.l0109  jp      l0740           ; DOS_CLOSE
.l010c  jp      l0761           ; DOS_ABANDON
.l010f  jp      l08b1           ; DOS_REF_HEAD
.l0112  jp      l10ea           ; DOS_READ
.l0115  jp      l11fe           ; DOS_WRITE
.l0118  jp      l11a8           ; DOS_BYTE_READ
.l011b  jp      l1298           ; DOS_BYTE_WRITE
.l011e  jp      l0a19           ; DOS_CATALOG
.l0121  jp      l08f2           ; DOS_FREE_SPACE
.l0124  jp      l0924           ; DOS_DELETE
.l0127  jp      l096f           ; DOS_RENAME
.l012a  jp      l1ace           ; DOS_BOOT
.l012d  jp      l090f           ; DOS_SET_DRIVE
.l0130  jp      l08fc           ; DOS_SET_USER
.l0133  jp      l1070           ; DOS_GET_POSITION
.l0136  jp      l108c           ; DOS_SET_POSITION
.l0139  jp      l1079           ; DOS_GET_EOF
.l013c  jp      l01d8           ; DOS_GET_1346
.l013f  jp      l01de           ; DOS_SET_1346
.l0142  jp      l05c2           ; DOS_FLUSH
.l0145  jp      l08c3           ; DOS_SET_ACCESS
.l0148  jp      l0959           ; DOS_SET_ATTRIBUTES
.l014b  jp      l0706           ; DOS_OPEN_DRIVE
.l014e  jp      l02e8           ; DOS_SET_MESSAGE
.l0151  jp      l1847           ; DOS_REF_XDPB
.l0154  jp      l1943           ; DOS_MAP_B
.l0157  jp      l1f27           ; DD_INTERFACE
.l015a  jp      l1f32           ; DD_INIT
.l015d  jp      l1f47           ; DD_SETUP
.l0160  jp      l1e7c           ; DD_SET_RETRY
.l0163  jp      l1bff           ; DD_READ_SECTOR
.l0166  jp      l1c0d           ; DD_WRITE_SECTOR
.l0169  jp      l1c16           ; DD_CHECK_SECTOR
.l016c  jp      l1c24           ; DD_FORMAT
.l016f  jp      l1c36           ; DD_READ_ID
.l0172  jp      l1e65           ; DD_TEST_UNSUITABLE
.l0175  jp      l1c80           ; DD_LOGIN
.l0178  jp      l1cdb           ; DD_SEL_FORMAT
.l017b  jp      l1edd           ; DD_ASK_1
.l017e  jp      l1ee9           ; DD_DRIVE_STATUS
.l0181  jp      l1e75           ; DD_EQUIPMENT
.l0184  jp      l1bda           ; DD_ENCODE
.l0187  jp      l1cee           ; DD_L_XDPB
.l018a  jp      l1d30           ; DD_L_DPB
.l018d  jp      l1f76           ; DD_L_SEEK
.l0190  jp      l20c3           ; DD_L_READ
.l0193  jp      l20cc           ; DD_L_WRITE
.l0196  jp      l212b           ; DD_L_ON_MOTOR
.l0199  jp      l2150           ; DD_L_T_OFF_MOTOR
.l019c  jp      l2164           ; DD_L_OFF_MOTOR


; DOS_INITIALISE

.l019f  ld      hl,pg_buffer
        ld      de,pg_buffer+1
        ld      bc,$09ff
        ld      (hl),$00
        ldir                    ; clear DOS workspace variables
        call    l1f27           ; DD_INTERFACE
        ld      hl,$0080        ; max RAMdisk, zero cache
        ld      d,h
        ld      e,h
        jr      nc,l01c2        ; move on if no interface
        call    l1f32           ; DD_INIT
        call    l17d0           ; initialise A: & B: extended XDPBs
        ld      hl,$0878        ; $78 RAMdisk buffers, $08 cache buffers
        ld      de,$0008
.l01c2  push    de
        call    l1820           ; initialise RAMdisk
        pop     de
        call    l1539           ; setup cache
        jp      l0500           ; set default drive and exit


; DOS_VERSION

.l01cd  xor     a
        ld      b,a
        ld      c,a             ; A=BC=0
        ld      de,$0100        ; DE=version info
        ld      hl,$0069        ; HL=?
        scf                     ; signal success
        ret     

; DOS_GET_1346

.l01d8  call    l1a48           ; get RAMdisk info
        jp      l1530           ; get cache info & exit

; DOS_SET_1346

.l01de  push    de              ; save new cache info
        ex      de,hl           ; HL=new RAMdisk info
        call    l1a48           ; get HL=old RAMdisk info
        or      a
        sbc     hl,de           ; set Z if no change in RAMdisk
        ex      de,hl
        scf                     ; set success flag     
        call    nz,l1a52        ; change RAMdisk if necessary
        pop     de              ; restore new cache info
        ret     nc              ; exit if error changing RAMdisk
        ex      de,hl
        call    l1530           ; get old cache info
        ex      de,hl
        or      a
        sbc     hl,de           ; set Z if no change in cache
        add     hl,de
        scf     
        call    nz,l1535        ; change cache if necessary
        ret

; Subroutine to copy BC bytes from HL to DE within page A, then page
; back original page

.l01fb  inc     c
        dec     c
        jr      nz,l0202
        inc     b
        dec     b
        ret     z               ; exit if no bytes to copy
.l0202  call    l0207           ; page in A
        ldir                    ; copy bytes, then following routine pages
                                ; back original bank & exits

; Subroutine to page in bank A (gives A=previous bank)

.l0207  push    hl
        push    bc
        ld      b,a
        ld      hl,BANKM
        ld      a,(hl)
        and     $07
        push    af              ; stack previous bank
        cp      b
        jr      z,l0227         ; exit if no change
        ld      a,(hl)
        and     $f8     
        or      b               ; new value
        ld      b,a
        ld      a,r             ; interrupt status to P/V
        ld      a,b
        ld      bc,$7ffd
        di      
        ld      (hl),a
        out     (c),a           ; page in new bank
        jp      po,l0227
        ei                      ; restore interrupts if necessary
.l0227  pop     af              ; restore previous bank
        pop     bc
        pop     hl
        ret

; Subroutine to find address of buffer A in 1346 area
; On exit, HL=address, A=bank

.l022b  add     a,a             ; A=offset/256
        ld      l,a
        or      $c0
        ld      h,a             ; H=high byte
        ld      a,l
        ld      l,$00           ; L=start of buffer
        rlca    
        rlca    
        rlca    
        and     $06             ; A=bank 0,2,4 or 6
        cp      $04
        ret     nc              ; exit if 4 or 6
        inc     a               ; else use 1 or 3
        ret

; Subroutine to copy IX bytes from HL in page C to DE in page B

.l023d  ld      a,c
        cp      b
        jr      nz,l0247        ; move on if pages different
        push    ix
        pop     bc
        jp      l01fb           ; else just copy within page (now in A)
.l0247  push    bc
        call    l02c4           ; get #bytes to move if page C below $c000
        call    l01fb           ; move them
        pop     bc
        push    bc
        ld      a,b             ; get #bytes to move if page B below $c000
        ex      de,hl           ; opposite direction
        call    l02c4           ; calculate number
        ex      de,hl
        call    l01fb           ; move them
        push    ix
        pop     bc
        ld      a,b
        or      c
        pop     bc
        ret     z               ; exit if all moved
        ld      a,r             ; get interrupt status
        di                      ; disable interrupts
        push    af
        or      a
        call    l0273           ; save $20 bytes at $bfe0
        call    l0288           ; copy bytes via buffer at $bfe0
        scf     
        call    l0273           ; restore $20 bytes at $bfe0
        pop     af
        ret     po
        ei                      ; restore interrupts if necessary
        ret     

; Subroutine to copy $20 bytes from pg_buffer to $bfe0 (or vice-versa if carry
; set)

.l0273  push    hl
        push    de
        push    bc
        ld      bc,$0020
        ld      de,pg_buffer
        ld      hl,$bfe0
        jr      nc,l0282
        ex      de,hl           ; exchange if required
.l0282  ldir                    ; copy bytes
        pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to copy IX bytes from HL in page C to DE in page B, using
; a $20-byte buffer at $bfe0

.l0288  push    ix
        ex      (sp),hl
        ld      a,h
        or      a
        jr      nz,l0294        ; move on if >=$100 bytes left
        ld      a,l
        cp      $20
        jr      c,l0296         ; move up to $20 bytes at a time
.l0294  ld      a,$20
.l0296  push    bc              ; save page numbers
        ld      c,a
        ld      b,$00
        or      a
        sbc     hl,bc           ; reduce #bytes left to copy
        pop     bc              ; restore page numbers
        ex      (sp),hl
        pop     ix
        or      a
        ret     z               ; exit if no more bytes to copy
        push    de
        push    bc
        push    af
        ld      b,a             
        ld      a,c
        ld      c,b
        ld      b,$00           ; A=page to copy from, BC=#bytes
        ld      de,$bfe0
        call    l01fb           ; copy bytes to $bfe0
        pop     af
        pop     bc
        pop     de
        push    hl
        push    bc
        ld      c,a
        ld      a,b
        ld      b,$00           ; A=page to copy to, BC=#bytes
        ld      hl,$bfe0
        call    l01fb           ; copy bytes from $bfe0
        pop     bc
        pop     hl
        jr      l0288           ; loop back until copied all

; Subroutine to calculate BC=#bytes that can be moved to DE
; If DE is in the top segment, no bytes can be moved
; IX initially contains total #bytes to move, and on exit contains #bytes
; that will be left after this move

.l02c4  push    hl
        ld      bc,$0000        ; move zero bytes if dest in top segment
        ld      hl,$c000
        or      a
        sbc     hl,de
        jr      c,l02e6         ; exit if destination in top segment
        jr      z,l02e6
        push    ix
        pop     bc              ; BC=#bytes
        or      a
        sbc     hl,bc
        add     hl,bc
        jr      nc,l02dd        ; move on if space for all bytes 
        ld      b,h             ; else use space available
        ld      c,l
.l02dd  push    ix
        pop     hl
        or      a
        sbc     hl,bc
        push    hl
        pop     ix              ; IX=bytes left to move after this              
.l02e6  pop     hl
        ret     

; DOS_SET_MESSAGE

.l02e8  or      a
        jr      nz,l02ee
        ld      hl,$0000        ; use $0000 to disable
.l02ee  ld      de,(rt_alert)   ; get old routine address
        ld      (rt_alert),hl   ; set new routine address
        ex      de,hl
        ret     

; Subroutine to do an ALERT message for error A on drive C

.l02f7  ld      b,a
        ld      hl,(rt_alert)
        ld      a,h
        or      l
        ld      a,b
        jr      nz,l0302        ; if ALERT routine exists, go to do it
        inc     l               ; otherwise exit with HL=1
        ret     
.l0302  push    bc              ; save drive
        call    l033a           ; generate error message
        push    hl              ; save address
        ld      de,al_resp
        push    de
        ld      hl,l04cc
        ld      bc,$0007
        ldir                    ; copy response key list
        pop     de
        pop     hl
        push    de
        call    l0332           ; call the ALERT routine
        pop     de
        ld      hl,l04d3        ; address of reply value string
        ld      b,a
.l031e  ld      a,(de)
        cp      $ff
        jr      z,l032b         ; move on if end of reply string
        cp      b
        ld      a,(hl)          ; get reply value
        jr      z,l032c         ; move on if match
        inc     de
        inc     hl
        jr      l031e           ; loop back
.l032b  xor     a               ; use "Cancel"
.l032c  sub     $01             ; carry set for "Cancel", Z set for "Retry"
        ccf                     ; invert carry
        pop     bc              ; restore drive
        ld      a,b             ; restore error
        ret     

; Subroutine to call ALERT subroutine with error message in HL

.l0332  push    hl
        ld      hl,(rt_alert)
        ex      (sp),hl
        ret

; Subroutine to generate change disk message

.l0338  ld      a,$0a           ; message 10

; Subroutine to generate recoverable error message A, returns address in HL

.l033a  ld      ix,al_mess      ; address to place message
        push    ix
        call    l0349           ; generate it
        ld      (ix+$00),$ff    ; add a terminator
        pop     hl              ; restore address
        ret

; Subroutine to put recoverable error message address for error A at IX

.l0349  and     $7f             ; mask off bit 7
        ld      hl,l03ae        ; address of message 0
        ld      b,a
        inc     b
        jr      l0357           ; go to find address
.l0352  ld      a,(hl)
        inc     hl
        inc     a
        jr      nz,l0352        ; skip next message
.l0357  djnz    l0352           ; back until at correct message
.l0359  ld      a,(hl)          ; get next char
        inc     hl
        cp      $ff
        ret     z               ; exit if end of message
        push    hl
        call    l0365           ; process next char
        pop     hl
        jr      l0359           ; back for more chars

; Subroutine to process a character in the error message generator

.l0365  or      a
        jp      p,l03a8         ; move on if standard ASCII value
        cp      $fe
        jr      z,l03a7         ; go to insert drive letter
        cp      $fd
        jr      z,l0378         ; go to insert track number
        cp      $fc
        jr      nz,l0349        ; if not sector, go to include submessage
        ld      a,e             ; sector=E
        jr      l0379
.l0378  ld      a,d             ; track=D
.l0379  push    de
        push    bc
        ld      l,a
        ld      h,$00
        ld      d,h
        ld      bc,$ff9c
        call    l0392           ; generate 100s digit
        ld      bc,$fff6
        call    l0392           ; generate 10s digit
        ld      a,l
        add     a,'0'
        pop     bc
        pop     de
        jr      l03a8           ; insert units digit
.l0392  ld      a,$ff
.l0394  push    hl              
        inc     a
        add     hl,bc
        jr      nc,l039d
        ex      (sp),hl
        pop     hl
        jr      l0394           ; loop back until value in A
.l039d  pop     hl
        or      a
        jr      z,l03a3         ; move on if zero
        ld      d,'0'
.l03a3  add     a,d             ; form character
        ret     z               ; exit if none
        jr      l03a8
.l03a7  ld      a,c             ; drive=C
.l03a8  ld      (ix+$00),a      ; add character to message
        inc     ix
        ret

; Recoverable error message table

.l03ae  defm    $8b&"not ready"&$8f&$ff
        defm    $8c&"write protected"&$8f&$ff
        defm    $8d&"seek fail"&$8f&$ff
        defm    $8e&"data error"&$8f&$ff
        defm    $8e&"no data"&$8f&$ff
        defm    $8e&"missing address mark"&$8f&$ff
        defm    $8b&"bad format"&$8f&$ff
        defm    $8e&"unknown error"&$8f&$ff
        defm    $8c&"changed, please replace"&$8f&$ff
        defm    $8c&"unsuitable"&$8f&$ff
        defm    "Please put the disk for "&$fe&": into the drive then press "
        defm    "any key"&$ff
        defm    "Drive "&$fe&": "&$ff
        defm    $8b&"disk "&$ff
        defm    $8b&"track "&$fd&", "&$ff
        defm    $8d&"sector "&$fc&", "&$ff
        defm    " - Retry, Ignore or Cancel? "&$ff
.l04cc  defm    "rRiIcC"&$ff
.l04d3  defb    1,1,2,2,0,0


; Subroutine to shift DE right A times

.l04d9  or      a
        ret     z               ; exit if A=0
.l04db  srl     d               ; shift right
        rr      e
        dec     a
        jr      nz,l04db        ; back for more
        ret     

; Subroutine to shift DE left A times

.l04e3  or      a
        ret     z               ; exit if A=0
        ex      de,hl
.l04e6  add     hl,hl           ; shift left
        dec     a
        jr      nz,l04e6        ; back for more
        ex      de,hl
        ret     

; Subroutine to call routine address in HL

.l04ec  jp      (hl)

; Subroutine to convert lowercase drive letters to uppercase

.l04ed  cp      'a'
.l04ef  ret     c
        cp      'z'+1
        ret     nc
        add     a,$e0           ; if lowercase, convert to uppercase
        ret     

        defs    10

; Sets default drive to first found with XDPB
; If none, A: is set as default

.l0500  ld      bc,$1041        ; 16 drives, A: to P:
.l0503  ld      a,c
        ld      (def_drv),a     ; set drive as default
.l0507  call    l184d           ; get XDPB for drive
        ret     c               ; exit if XDPB available
        inc     c               ; increment drive letter
        djnz    l0503           ; back for more
        ld      a,'A'
        ld      (def_drv),a     ; set default drive A:
        ret     

; Subroutine to get FCB for file B & test if open for reading

.l0514  call    l0525           ; get FCB & test if open
        ret     nc              ; exit if error
        rra                     ; test if open for read
        ld      a,$1d           ; error "file not open"
        ret

; Subroutine to get FCB for file B & test if open for writing

.l051c  call    l0525           ; get FCB & test if open
        ret     nc              ; exit if error
        rra     
        rra                     ; test if open for write
        ld      a,$1d           ; error "file not open"
        ret     

; Subroutine to get FCB for file B & test if open

.l0525  call    l054b           ; get FCB & access mode
        ret     nc              ; exit if error
        rlca    
        rra                     ; bit 7 to carry
        ret     c               ; return with success if open
        ld      a,$1d           ; file not open error
        ret     

; Subroutine to clear FCB for closed file B
; *BUG* Clears one byte too many; this can result in user area for the
; next FCB being erroneously set to 0

.l052f  call    l054b           ; get FCB & access mode
        ret     nc              ; exit if error
        rla     
        ccf                     ; inverted bit 7 to carry
        ld      a,$1d           ; file not open
        ret     nc              ; return with error if open
        push    hl              ; save registers
        push    de
        push    bc
        ld      h,b
        ld      l,c
        ld      (hl),$00
        ld      d,b
        ld      e,c
        inc     de
        ld      bc,$0038	; *BUG* should be $0037
        ldir                    ; clear FCB
        pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to get FCB (in BC) of file B. Returns access mode/flags in A.

.l054b  push    hl              ; save registers
        push    de
        ld      a,b             ; get file number
        cp      $10
        ld      a,$15
        jr      nc,l0566        ; if >$0f, exit with bad parameter error
        ld      hl,fcbs-$0038
        ld      de,$0038
        inc     b
.l055b  add     hl,de
        djnz    l055b
        ld      b,h
        ld      c,l             ; BC=address of FCB
        ld      hl,$0020
        add     hl,bc
        ld      a,(hl)          ; get access mode
        scf                     ; success
.l0566  pop     de              ; restore registers
        pop     hl
        ret     


; Subroutine to check if file in BC is opened by any other FCB

.l0569  ld      hl,$0020
        add     hl,bc
        ld      e,(hl)		; save access mode
        ld      (hl),$00	; signal "not opened"
        push    hl
        push    de
        call    l0579		; check if file opened in any other FCB
        pop     de
        pop     hl
        ld      (hl),e		; restore access mode
        ret     

; Subroutine to check if file in BC can be opened in required access mode

.l0579  ld      hl,fcbs
        ld      e,$12		; 18 files to check
.l057e  push    hl
        push    bc
        ld      bc,$0020
        add     hl,bc
        ld      d,(hl)		; D=file access mode
        inc     hl
        ld      a,(hl)		; A=file drive
        pop     bc
        pop     hl
        bit     7,d
        jr      z,l05b7		; move on if file not open
        push    hl
        ld      hl,$0021
        add     hl,bc
        cp      (hl)
        pop     hl
        jr      nz,l05b7	; move on if different drive
        ld      a,(bc)
        cp      $22		; check if current file is open drive
        jr      z,l05a3
        ld      a,(hl)
        cp      $22		; or file we are checking
        call    nz,l0d8a	; if not, check if files are the same
        jr      nz,l05b7	; if not, move on
.l05a3  push    hl
        ld      hl,$0020
        add     hl,bc
        ld      a,(hl)		; get access mode of current file
        rrca    
        rrca    
        and     $03
        ld      h,a		; H=1 if open shared
        ld      a,d
        and     $03		
        or      h
        xor     h		; Z set if legal access
        ld      a,$1e		; access denied error
        pop     hl
        ret     nz
.l05b7  push    de
        ld      de,$0038
        add     hl,de
        pop     de
        dec     e
        jr      nz,l057e	; loop back to test more FCBs
        scf     
        ret     

; DOS_FLUSH

.l05c2  call    l04ed           ; make drive uppercase
        call    l0c27           ; ensure disk logged in
        ret     nc              ; exit if error
.l05c9  push    bc
        ld      bc,fcbs         ; first FCB
        ld      e,$12           ; 18 files
.l05cf  ld      hl,$0020
        add     hl,bc
        bit     7,(hl)
        jr      z,l05e4         ; skip if file not open
        inc     hl
        ld      a,(hl)
        cp      (ix+$1c)        ; is it on same drive?
        scf     
        push    de
        call    z,l074c         ; ensure file header & directory up to date
        pop     de
        jr      nc,l05ee        ; exit if error
.l05e4  ld      hl,$0038
        add     hl,bc
        ld      b,h
        ld      c,l             ; get to next FCB
        dec     e
        jr      nz,l05cf        ; back for more
        scf                     ; success
.l05ee  pop     bc
        ret     

; Subroutine to get sector HL to buffer, ensuring all FCBs on same
; drive referencing this sector are up to date
; On entry, D=drive, HL=abs logical sector of a buffer

.l05f0  ld      bc,fcbs         ; first FCB
        ld      e,$12           ; check 18 FCBs (16 user+2 system)
.l05f5  push    hl
        push    de
        ld      a,d             ; A=drive
        ex      de,hl
        ld      hl,$0020
        add     hl,bc
        bit     7,(hl)          ; is file open?
        jr      z,l061f         ; move on if not
        inc     hl
        cp      (hl)            ; is drive the same?
        jr      nz,l061f        ; move on if not
        inc     hl
        bit     3,(hl)          ; is a sector number in +$2b?
        jr      z,l061f         ; move on if not
        ld      hl,$002b
        add     hl,bc
        ld      a,e
        cp      (hl)            ; check low byte of absolute sector
        jr      nz,l061f        ; move on if different
        inc     hl
        ld      a,d
        cp      (hl)            ; check high byte of absolute sector
        jr      nz,l061f        ; move on if different
        call    l0c20           ; ensure correct disk logged in
        call    c,l132a         ; if no error, get sector to buffer
        jr      nc,l0626
.l061f  ld      hl,$0038
        add     hl,bc
        ld      b,h             ; BC=next FCB
        ld      c,l
        scf                     ; success
.l0626  pop     de
        pop     hl
        ret     nc              ; exit if error
        dec     e
        jr      nz,l05f5        ; loop back for more FCBs
        ret     

; DOS_OPEN

.l062d  push    de
        push    bc
        call    l052f		; clear FCB for file B (& ensure closed)
        call    c,l0adf		; parse filename to FCB,disallowing wildcards
        call    c,l0c20         ; ensure correct disk logged in
        pop     hl
        pop     de
        ret     nc		; exit if error
        push    de
        ld      a,l
        ld      hl,$0020
        add     hl,bc
        ld      (hl),a		; set access mode
        call    l0579		; ensure file can be opened in this mode
        ld      hl,l0d8a
        call    c,l0dae		; find first extent of file
        pop     de
        ret     nc		; exit if error
        jr      nz,l067f	; move on if not found
        ld      a,e		; check open action
        or      a
        ld      a,$18
        ret     z		; "file exists" error for open action 0
        dec     e
        jr      nz,l065f        ; move on for actions 2-4
        call    l06c4		; find first entry (open action 1)
        call    c,l0801		; open file, reading any header
        jr      l069b		; mark file as open & exit
.l065f  dec     e
        jr      nz,l066a	; move on for actions 3 & 4
        call    l06c4		; find first entry (open action 2)
        call    c,l0859		; open file, skipping any header
        jr      l069b		; mark file as open & exit
.l066a  push    de
        dec     e
        jr      nz,l0676	; move on for action 4
        call    l06e0		; for open action 3, erase any existing .BAK
        call    c,l0983		; and rename file to .BAK
        jr      l067d		; go to follow create action
.l0676  or      a		
        ld      a,$15		; bad parameter if not action 4
        dec     e
        call    z,l092e		; for action 4, erase file then do create
.l067d  pop     de
        ret     nc		; exit if error
.l067f  ld      a,d
        or      a
        ld      a,$17
        ret     z		; error "file not found" for create action 0
        dec     d
        jr      nz,l068f	; move on for create action 2
        call    l06a9		; for action 1, create an entry
        call    c,l07dc		; and add a header
        jr      l0696
.l068f  or      a
        ld      a,$15		; "bad parameter" for create actions > 2
        dec     d
        call    z,l06a9		; for action 2, just create an entry
.l0696  ret     nc		; exit if error
        xor     a		; A=0 if file was created
        scf     
        jr      l069d
.l069b  ret     nc		; exit if error
        sbc     a,a		; A=$ff if file was opened
.l069d  push    af
        ld      hl,$0020
        add     hl,bc
        set     7,(hl)		; mark FCB as containing an open file
        inc     (ix+$21)	; increment # open files on drive
        pop     af
        ret     

; Subroutine to set up clean directory entry for new file

.l06a9  ld      hl,$0020
        add     hl,bc
        ld      a,(hl)
        rra     
        rra     		; check bit 1 of FCB flags
        ld      a,$1e		; "access denied" error
        call    c,l18f3		; check disk can be written to
        ld      hl,$0000
        call    c,l0cbe		; set up clean extent
        ret     nc		; exit if error
        ld      hl,$0022
        add     hl,bc
        set     0,(hl)		; signal "directory valid"
        scf     
        ret     

; Subroutine to find entry for required extent of file

.l06c4  call    l0d4a		; find directory entry of required extent
        jr      nc,l06d8	; move on if not found
        ld      hl,$0020
        add     hl,bc
        bit     1,(hl)
        scf     
        ret     z		; exit with success if new entry not needed
        call    l0ebe		; check file can be written to
        call    c,l18f3		; check disk can be written to
        ret     
.l06d8  cp      $19		; error "end of file"
        scf     
        ccf     
        ret     nz		; exit unless finding first extent
        ld      a,$17		; error "file not found"
        ret     

; Subroutine to erase any .BAK file existing for file in current FCB

.l06e0  push    bc
        ld      h,b
        ld      l,c
        ld      de,sysfcb0
        ld      bc,$0038
        ldir    		; copy FCB to SYSFCB0
        pop     bc
        ld      a,'B'
        ld      (sysfcb0+9),a
        ld      hl,$4b41
        ld      (sysfcb0+$0a),hl ; set extension in SYSFCB0 to "BAK"
        push    bc
        ld      bc,sysfcb0
        call    l092e		; erase any existing .BAK file
        pop     bc
        ret     c		; exit if success
        cp      $17
        scf     
        ret     z		; or with success if error was "file not found"
        or      a
        ret     

; DOS_OPEN_DRIVE

.l0706  call    l04ed		; make drive letter uppercase
        ld      d,a		; save letter
        ld      e,c		; & access mode
        call    l052f		; clear FCB for file B (ensure closed)
        ret     nc		; exit if error
        ld      a,$22
        ld      (bc),a		; "open drive"
        ld      hl,$0020
        add     hl,bc
        ld      (hl),e		; access mode
        inc     hl
        ld      (hl),d		; drive letter
        ld      a,d
        call    l184d		; get XDPB for drive
        call    c,l0579		; check can open in required access mode
        ret     nc		; exit if error
        ld      e,(ix+$05)
        ld      d,(ix+$06)
        inc     de
        ld      a,(ix+$02)
        call    l04e3
        call    l19c0
        sla     e
        rl      d		; DE="file" length (high bytes)
        ld      hl,$0024
        add     hl,bc
        ld      (hl),e
        inc     hl
        ld      (hl),d		; set file length
        scf     
        jp      l069d		; set file open & exit

; DOS_CLOSE

.l0740  call    l0525           ; get FCB & see if open
        call    c,l0c20         ; ensure correct disk logged in
        call    c,l074c         ; ensure file header & directory up-to-date
        ret     nc		; exit if error
        jr      l077f		; go to abandon FCB

; Subroutine to ensure file's +3DOS header & directory entry are up-to-date

.l074c  ld      hl,$0020
        add     hl,bc
        bit     1,(hl)
        scf     
        ret     z		; exit with success if file not in write mode
        call    l0797		; update any +3DOS header with file length
        call    c,l132a         ; get sector to buffer
        call    c,l1719		; write all changed data on this disk
        call    c,l0cca		; ensure directory up to date for this file
        ret

; DOS_ABANDON

.l0761  call    l0525           ; get FCB & see if open
        call    c,l0c20         ; ensure correct disk logged in
        ret     nc              ; exit if error
        ld      hl,$0020
        add     hl,bc
        bit     1,(hl)
        jr      z,l077f		; move on if not open in write mode
        inc     hl
        inc     hl
        bit     1,(hl)		
        jr      z,l077c		; move on if new directory entry not needed
        call    l1038		; clear flag & increment free entries
        call    l0f40		; deallocate blocks in FCB
.l077c  call    l16c6		; move all inuse BCBs for file to free list
.l077f  ld      hl,$0020
        add     hl,bc
        ld      (hl),$00	; signal "file not open"
        dec     (ix+$21)	; decrement files open on disk
        call    z,l189d		; low-level logout disk if none left
        scf     		; success
        ret

; +3DOS file header signature (issue 1)

.l078d  defm    "PLUS3DOS"&$1a&$01


; Subroutine to update any +3DOS file header with filelength if necessary
; File position is unaffected

.l0797  ld      hl,$0022
        add     hl,bc
        bit     6,(hl)
        scf     
        ret     z		; exit with success if file has no header
        call    l1074		; get DEHL=file position
        push    hl
        push    de
        call    l07af		; update header with filelength
        pop     de
        pop     hl
        push    af
        call    l1090		; restore file position
        pop     af
        ret     

; Subroutine to update +3DOS file header with filelength & +3 BASIC header
; Leaves filepointer positioned after header

.l07af  ld      hl,$000b
        ld      e,h
        call    l1090		; set file position to header file length
        ld      hl,$0023
        add     hl,bc
        ld      e,$03
        call    l07d2		; copy filelength from FCB to file header
        ret     nc		; exit if error
        xor     a
        call    l12a5		; MSB of filelength is zero
        ret     nc		; exit if error
        ld      hl,$0030
        add     hl,bc
        ld      e,$08
        call    l07d2		; copy +3 BASIC header data from FCB to file
        ret     nc		; exit if error
        jp      l12e6		; update header checksum & exit

; Subroutine to copy E bytes from HL into file at current filepointer

.l07d2  ld      a,(hl)		; get next byte
        inc     hl
        call    l12a5		; copy byte into file
        ret     nc		; exit if error
        dec     e
        jr      nz,l07d2	; back for more
        ret     

; Subroutine to create a header for a newly-created file

.l07dc  ld      e,$0a
        ld      hl,l078d	; header signature
        call    l07d2		; copy header signature into file
        ret     nc
        ld      a,$00
        call    l12a5		; version 0
        ret     nc
        ld      e,$74
.l07ed  xor     a
        call    l12a5		; fill rest of header with nulls
        ret     nc
        dec     e
        jr      nz,l07ed
        call    l12e6		; set checksum & place filepointer after it
        ret     nc
.l07f9  ld      hl,$0022
        add     hl,bc
        set     6,(hl)		; set "file has header" flag
        scf     
        ret     

; Subroutine to open file & read any header

.l0801  call    l12de		; check if valid header checksum
        jr      nc,l084d        ; move on if error
        jr      nz,l0852        ; if not, move on (no header)
        ld      e,$0a		; header signature length
        ld      hl,l078d	; header signature
.l080d  call    l11cb		; get byte from file
        jr      nc,l084d        ; move on if error
        cp      (hl)
        inc     hl
        jr      nz,l0852        ; move on if doesn't match header signature
        dec     e
        jr      nz,l080d        ; back for more chars
        call    l11cb		; get version
        jr      nc,l084d        ; move on if error
        cp      $01
        jr      nc,l0852        ; no header if higher version than 0
        ld      hl,$0023
        add     hl,bc
        ld      e,$03
.l0828  call    l11cb
        ret     nc
        ld      (hl),a		; copy filelength from header
        inc     hl
        dec     e
        jr      nz,l0828
        call    l11cb		; skip high byte of length
        ret     nc
        ld      hl,$0030
        add     hl,bc
        ld      e,$08
.l083b  call    l11cb
        ret     nc
        ld      (hl),a		; copy +3 BASIC data from header
        inc     hl
        dec     e
        jr      nz,l083b
        ld      hl,$0080
        ld      e,h
        call    l1090		; set filepointer past header record
        jr      l07f9		; exit, setting "file has header" flag
.l084d  cp      $19
        scf     
        ccf     
        ret     nz		; error unless "end of file"
.l0852  ld      hl,$0000
        ld      e,l
        call    l1090		; set filepointer to start of file
.l0859  ld      hl,$0000
        ld      (filerecs+1),hl
        xor     a
        ld      (filerecs+2),a
        ld      hl,l088d
        call    l0dae		; get last record+1 in file
        ret     nc
        ld      de,(filerecs+1)
        ld      hl,(filerecs-1)
        ld      l,$00		; DEHL=2*filelength
        srl     d
        rr      e
        rr      h
        rr      l		; DEHL=filelength
        ld      a,d
        or      a
        ld      a,$22		; "file too big" error if >8M
        ret     nz
        push    hl
        ld      hl,$0025
        add     hl,bc
        ld      (hl),e		; store filelength in FCB
        pop     de
        dec     hl
        ld      (hl),d
        dec     hl
        ld      (hl),e
        scf     
        ret     

; Subroutine to update largest rec# with one from current extent if larger

.l088d  call    l0d8a
        ret     nz		; exit if filenames don't match
        push    bc
        ld      b,h
        ld      c,l
        call    l14c2		; get ADE=last record number+1
        ld      b,a
        ex      de,hl		; test against largest found so far
        ld      hl,(filerecs)
        or      a
        sbc     hl,de
        ld      a,(filerecs+2)
        sbc     a,b
        jr      nc,l08ad	; move on unless larger
        ld      (filerecs),de
        ld      a,b
        ld      (filerecs+2),a	; set size from current extent
.l08ad  pop     bc
        scf     
        sbc     a,a		; success
        ret     

; DOS_REF_HEAD

.l08b1  call    l0525           ; get FCB & check file is open
        ret     nc              ; exit if not
        ld      ix,$0030
        add     ix,bc           ; IX points to header data
        ld      hl,$0022
        add     hl,bc
        bit     6,(hl)          ; does file have header?
        scf                     ; success
        ret     

; DOS_SET_ACCESS

.l08c3  ld      e,c		; E=required access mode
        push    de
        call    l0525           ; get FCB & current access mode
        call    c,l0c20         ; if ok, ensure correct disk logged in
        call    c,l074c         ; ensure file header & directory up-to-date
        pop     de
        ret     nc		; exit if error
        ld      hl,$0020
        add     hl,bc
        ld      d,(hl)		; get old access mode
        ld      (hl),e		; store new (as closed)
        push    hl
        push    de
        call    l0579		; check if can open in this mode
        pop     de
        pop     hl
        jr      nc,l08ef        ; if not, go to restore old mode & exit
        bit     1,e
        jr      z,l08eb		; move on if not opening for write
        call    l0ebe		; check file can be written to
        call    c,l18f3		; check disk can be written to
        jr      nc,l08ef        ; exit, restoring mode, if error
.l08eb  set     7,(hl)		; set file open
        scf     		; success
        ret     
.l08ef  ld      (hl),d		; restore old mode
        or      a
        ret     

; DOS_FREE_SPACE

.l08f2  call    l04ed		; make drive letter uppercase
        call    l0c27		; ensure allocation bitmap up-to-date
        ret     nc		; exit if error
        jp      l0fa9		; move on to calculate free space

; DOS_SET_USER

.l08fc  cp      $ff
        jr      z,l090a         ; move on to return current default user area
        cp      $10             ; check in range 0-15
        ld      b,a
        ld      a,$15
        ret     nc              ; error 21 - bad parameter if not
        ld      a,b
        ld      (def_user),a    ; set default user area
.l090a  ld      a,(def_user)    ; get default user area
        scf                     ; success
        ret

; DOS_SET_DRIVE

.l090f  call    l04ed           ; make letter uppercase
        cp      $ff
        jr      z,l091f         ; move on if current default drive required
        ld      b,a
        call    l184d           ; check drive has an XDPB
        ret     nc              ; exit with error if not
        ld      a,b
        ld      (def_drv),a     ; set default drive
.l091f  ld      a,(def_drv)     ; get default drive
        scf                     ; success
        ret

; DOS_DELETE

.l0924  ld      bc,sysfcb0
        call    l0af5		; parse filespec, allowing wildcards
        call    c,l0c20         ; ensure correct disk logged in
        ret     nc
.l092e  call    l0569		; check no source files open by any FCBs
        call    c,l18f3		; check disk can be written to
        ret     nc		; exit if error
        ld      hl,l093b	; routine to delete extents
        jp      l09ad

; Subroutine to delete any directory entry matching one in FCB

.l093b  call    l0d84
        ret     nz		; exit if entry doesn't match
        call    l0ebe
        ret     nc		; or if file read-only
        push    hl
        push    de
        xor     a
        call    l0f43		; deallocate blocks in entry
        pop     de
        pop     hl
        ld      (hl),$e5	; set "deleted" mark in FCB
        call    l0e34		; copy "direntry" to directory entry DE
        ret     nc
        call    l1040		; increment #free entries
        sbc     a,a
        ld      (extchg),a	; set "success" flag
        ret     

; DOS_SET_ATTRIBUTES

.l0959  ld      (att_clr),de	; store attribs to set/clear
        ld      bc,sysfcb0
        call    l0af5		; parse filespec, allowing wildcards
        call    c,l0c20         ; ensure correct disk logged in
        call    c,l18f3		; check disk can be written to
        ret     nc		; exit if error
        ld      hl,l09bf	; routine to change attributes
        jr      l09a7           

; DOS_RENAME

.l096f  push    de
        ld      bc,sysfcb1
        call    l0adf		; parse source filespec, ensuring no wildcards
        call    c,l0c20         ; ensure correct disk logged in
        pop     hl
        push    bc
        ld      bc,sysfcb0
        call    c,l0adf		; parse dest filespec, ensuring no wildcards
        pop     bc
        ret     nc		; exit if error
.l0983  ld      hl,$0021
        add     hl,bc
        ld      a,(sysfcb0+$21)
        xor     (hl)		; check drives are the same
        ld      a,$1f
        ret     nz		; error "cannot rename between drives"
        call    l18f3		; check disk can be written to
        push    bc
        ld      bc,sysfcb0
        call    c,l0569		; check dest file not open by any FCB
        ld      hl,l0d84
        call    c,l0dae		; check if dest file exists in directory
        pop     bc
        ret     nc		; exit if error
        ccf     
        ld      a,$18
        ret     z		; file already exists error
        ld      hl,l09f4	; rename routine
.l09a7  push    hl
        call    l0569		; check source file not open by any FCB
        pop     hl
        ret     nc		; exit if error
.l09ad  xor     a
        ld      (extchg),a	; set "no extents changed"
        call    l0dae		; rename/delete all extents of file
        ret     nc
        ld      a,(extchg)
        or      a		; check if any extents changed
        ld      a,$17		; file not found error
        call    nz,l1719
        ret     

; Subroutine to change attributes for directory entries matching
; filespec

.l09bf	call    l0d84
        ret     nz		; exit if no match
        push    bc
        ld      a,(att_set)	; get attribs to set
        ld      c,$ff		; set mask
        push    hl
        call    l09d8		; set them
        pop     hl
        ld      a,(att_clr)	; get attribs to clear
        inc     c		; clear mask
        call    l09d8		; clear them
        pop     bc
        jr      l0a10           ; go to update directory entry

; Subroutine to set/clear attributes. HL points to directory entry, A
; contains attributes to set/clear & C contains set/clear mask

.l09d8  rla			; discard bit 7
        ld      b,$04
        inc     hl
        call    l09e5		; bits 6->3 on first four chars of filename
        inc     hl
        inc     hl
        inc     hl
        inc     hl
        ld      b,$03		; bits 2->0 on extension
.l09e5  rla
        jr      nc,l09f0	; move on if attribute not to be affected
        res     7,(hl)		; reset it
        inc     c
        dec     c
        jr      z,l09f0
        set     7,(hl)		; set it if mask=$ff
.l09f0  inc     hl
        djnz    l09e5		; back for more
        ret     

; Subroutine to rename any directory entry matching filename in SYSFCB1
; to name in SYSFCB0

.l09f4  call    l0d84
        ret     nz		; exit if names don't match
        call    l0ebe
        ret     nc		; exit if file readonly
        push    de
        ex      de,hl
        ld      hl,sysfcb0
        ld      a,(de)
        and     $10		; also rename password control entries
        or      (hl)
        ld      (de),a
        inc     de
        inc     hl
        push    bc
        ld      bc,$000b
        ldir    		; copy new filename to direntry
        pop     bc
        pop     de
.l0a10  call    l0e34		; copy "direntry" to directory entry DE
        ret     nc
        sbc     a,a
        ld      (extchg),a	; if no error, set successful rename flag
        ret     

; DOS_CATALOG

.l0a19  ld      (cat_buff),de	; store buffer address
        ld      (cat_filt),bc	; store filter & buffer size
        ld      a,$01
        ld      (cat_ents),a	; store "1 entry completed"
        ld      bc,sysfcb0
        call    l0af5		; parse filespec to SYSFCB0, wildcards allowed
        call    c,l0c20         ; ensure correct disk logged in
        call    c,l05c9		; flush drive
        ld      hl,l0a3d
        call    c,l0dae		; generate catalog
        ld      bc,(cat_size)	; get B=#completed entries
        ret

; Subroutine to add directory entry to catalog if suitable

.l0a3d  push    bc
        call    l0a45		; process the entry
        pop     bc
        scf     
        sbc     a,a		; set A=$ff, set carry for success
        ret     

; Subroutine to process a directory entry & add to catalog if suitable

.l0a45  call    l0d8a
        ret     nz		; exit if doesn't match filespec
        ld      a,(cat_filt)
        rra     
        jr      c,l0a58		; move on if we should include system files
        push    hl
        ld      bc,$000a
        add     hl,bc
        bit     7,(hl)
        pop     hl
        ret     nz		; exit if system file
.l0a58  ld      de,(cat_buff)
        call    l0ac9		; is it alphabetically less than preloaded?
	ret	nc		; exit if so
        ld      bc,(cat_size)	; C=buffer size, B=#completed entries
.l0a64  push    hl
        ld      hl,$000d
        add     hl,de		; get to next entry in buffer
        ex      de,hl
        pop     hl
        dec     c		; decrement buffer size
        djnz    l0a71		; move on if more entries to check
        ret     z		; exit if no space left in buffer
        jr      l0aa1		; else move to add
.l0a71  call    l0ac9		; does filespec match next buffer entry?
        jr      c,l0a64		; loop back if alphabetically greater
        jr      z,l0ab9		; move on if the same
        push    hl
        push    de
        ld      hl,(cat_size)
        ld      h,$00
        dec     hl		; HL=#catalog buffer entries-1
        ld      b,h
        ld      c,l
        add     hl,hl
        add     hl,bc
        add     hl,hl
        add     hl,hl
        add     hl,bc		; HL=13*(#catalog buffer entries-1)
        ld      bc,(cat_buff)
        add     hl,bc		; HL=address of last entry in catalog buffer
        ld      a,l
        sub     e
        ld      c,a
        ld      a,h
        sbc     a,d
        ld      b,a		; BC=distance between last & current entries
        dec     hl
        ld      de,$000d
        ex      de,hl
        add     hl,de
        ex      de,hl		; DE=address of end of catalog buffer
        ld      a,b
        or      c
        jr      z,l0a9f		
        lddr    		; shift catalog entries down one (may lose one)
.l0a9f  pop     de
        pop     hl
.l0aa1  push    hl
        push    de
        inc     hl
        ld      bc,$000b
        ldir    		; copy entry into buffer
        xor     a
        ld      (de),a
        inc     de
        ld      (de),a		; set zero size
        ld      hl,(cat_size)
        ld      a,h
        cp      l
        adc     a,$00
        ld      (cat_ents),a	; increment # completed entries (max=bufsize)
        pop     de
        pop     hl
.l0ab9  call    l0f75		; calculate extent size in K
        ex      de,hl
        ld      bc,$000b
        add     hl,bc
        ld      a,(hl)
        add     a,e		; add extent size into directory entry size
        ld      (hl),a
        inc     hl
        ld      a,(hl)
        adc     a,d
        ld      (hl),a
        ret     

; Subroutine to compare filenames at DE & HL (ignoring attributes)
; and setting Z if they match. Carry set if filename at HL > one at DE

.l0ac9  push    hl
        push    de
        push    bc
        ld      b,$0b		; 11 chars to check
        inc     hl
.l0acf  ld      a,(hl)
        add     a,a
        ld      c,a
        ld      a,(de)
        add     a,a
        cp      c		; compare chars without attribute bits
        jr      nz,l0adb	; if different, exit
        inc     de
        inc     hl
        djnz    l0acf		; back for more
.l0adb  pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to parse filespec at HL into FCB at BC, giving
; error if filespec illegal or contains wildcards

.l0adf  call    l0b3f		; parse filespec
        ret     nc		; exit if error
        ld      hl,$0001
        add     hl,bc
        ld      e,$0b
.l0ae9  ld      a,(hl)
        inc     hl
        cp      '?'		; check for wildcard characters
        ld      a,$14
        ret     z		; exit with "bad filename" if found
        dec     e
        jr      nz,l0ae9	; back for more
        scf     
        ret     

; Subroutine to parse filespecs, allowing wildcards

.l0af5  jp      l0b3f		; jump to the routine

; Subroutine to set user area & drive letter specified in filename
; Should exit with HL pointing to ":"
; On exit, carry reset if neither found

.l0af8  call    l0b02		; set user area if specified
        jr      nc,l0b21
        call    l0b21		; set drive letter if specified
        scf     
        ret     

; Subroutine to check if chars at HL are a user area
; If so, exits with user area set, carry set & HL pointing after user area
; in filename

.l0b02  call    l0b38		; check if char is digit
        ret     nc		; exit if not
        ld      e,a		; save digit
        call    l0bd3		; get next char
        call    c,l0b38		; check if its a digit
        jr      nc,l0b1b	; if not, use single digit
        ld      d,a		; save ls digit
        ld      a,e
        add     a,a
        ld      e,a
        add     a,a
        add     a,a
        add     a,e
        add     a,d
        ld      e,a		; E=user area
        call    l0bd3		; get next char
.l0b1b  ld      a,e
        cp      $10
        ret     nc		; exit if invalid user area
        ld      (bc),a		; save user area in FCB
        ret     

; Subroutine to check if char at HL is a drive letter
; If so, exits with drive letter set, carry set & HL pointing after drive
; letter in filename

.l0b21  call    l0bc9		; get next char
        ret     nc		; exit if none
        cp      'A'
        ccf     
        ret     nc		; or if <"A"
        cp      'Q'
        ret     nc		; or if >"P"
        push    hl
        ld      hl,$0021
        add     hl,bc
        ld      (hl),a		; save as drive letter in FCB
        pop     hl
        call    l0bd3		; get next char
        scf     
        ret     

; Subroutine to test if A is a digit ('0'-'9')
; If so, exits with carry set & A=value

.l0b38  sub     '0'
        ccf			; error if <"0"
        ret     nc
        cp      $0a		; set carry if user area
        ret     

; Subroutine to parse filename at HL to FCB in BC (preserved)
; Carry reset & error $14 if illegal filespec

.l0b3f  push    bc
        call    l0b47		; parse filename
        pop     bc
        ld      a,$14		; error "bad filename"
        ret     

; Subroutine to parse filename at HL to FCB in BC (not preserved)
; Exits with carry set if legal filespec

.l0b47  push    hl
        ld      hl,$0021
        add     hl,bc
        ld      a,(def_drv)
        ld      (hl),a		; set drive as default
        pop     hl
        ld      a,(def_user)
        ld      (bc),a		; set user area as default
        call    l0bc9		; get next filename char
        jr      nc,l0b7a	; move on if none
        ld      e,a		; save char
        push    hl
.l0b5c  cp      ':'
        scf     
        jr      z,l0b66         ; if drive/user spec found, move on
        call    l0bd3		; get next char
        jr      c,l0b5c         ; back if found
.l0b66  pop     hl
        ld      a,e		; A=last drive/user char
        jr      nc,l0b80        ; but skip if no drive/user spec
        call    l0af8		; set user area and/or drive in FCB
        ret     nc		; exit if not found
        call    l0bc9		; get char
        ret     nc
        xor     ':'
        ret     nz		; exit if not ":"
        call    l0bc5		; get char after ":"
        jr      c,l0b80
.l0b7a  inc     bc		; if no filename, point to start
        ld      e,$0b		; with 11 chars to blank
        scf     
        jr      l0bbc
.l0b80  inc     bc		; move to filename in FCB
        cp      '.'
        ret     z		; exit if first char is "."
        ld      e,$08
        call    l0b96		; get up to 8 filename chars
        ccf     
        ld      e,$03		; and up to 3 extension chars
        jr      nc,l0bb3
        xor     '.'
        ret     nz		; exit if no extension
        call    l0bc5		; skip "." and get next char
        jr      nc,l0bb3	; move on if none
.l0b96  push    hl
        cp      ' '
        ld      hl,l0bef
        call    nc,l0be5	; check if char legal
        pop     hl
        jr      c,l0bb3		; if not, fill rest with spaces
        dec     e
        ret     m		; exit with error if filename too long
        cp      '*'
        call    z,l0bbc		; fill rest with ? if "*" wildcard
        ld      (bc),a		; else, store character
        inc     bc
        call    l0bd3		; get next
        jr      nz,l0b96	; loop back if more
        call    c,l0bc5		; skip any spaces
.l0bb3  push    af
        ld      a,' '
        call    l0bbe		; fill rest with spaces
        pop     af
        ccf     		; set carry (success)
        ret     

; Subroutine to fill E+1 chars in filename with "?" wildcard chars
; Or enter at l0bbe to fill with char in A

.l0bbc  ld      a,'?'
.l0bbe  inc     e
.l0bbf  dec     e
        ret     z		; exit if done
        ld      (bc),a		; fill
        inc     bc
        jr      l0bbf

; Subroutine to skip char in filename & get next one

.l0bc5  call    l0bd3		; get next char
        ret     nc		; exit if end

; Subroutine to get next filename char in A, Z set if end-of-filename

.l0bc9  call    l0bd8		; check for end of filename
.l0bcc  ret     nz		; exit if not
        call    l0bd3		; check if ended by space or $ff
        jr      c,l0bcc         ; skip until $ff encountered
        ret     		

; Subroutine to get next filename char (exits with C reset
; if already end of filename)

.l0bd3  ld      a,(hl)
        cp      $ff
        ret     z		; exit if end of filename
        inc     hl		; next character

; Subroutine to get current filename char as uppercase in A,
; checking for end of filename ($ff or space), setting Z if so

.l0bd8  ld      a,(hl)		; get next char
        cp      $ff
        ret     z		; exit if end of filename
        and     $7f		; mask to ASCII code
        call    l04ed		; convert to uppercase
        cp      ' '		; space?
        scf     		; set "not $ff"
        ret     

; Subroutine to check for illegal chars in filenames
; On entry, A=char & on exit carry & Z set if illegal

.l0be5  cp      (hl)		; test against next illegal char
        scf     
        ret     z		; exit if illegal
        inc     hl		; get to next char in list
        bit     7,(hl)	
        jr      z,l0be5 	; move back for more
        or      a		; clear carry & reset Z (character legal)
        ret   

; List of illegal chars in filenames

.l0bef  defm    "!&()+,-./:;<=>[\]|"&$80
        defs    30

; Subroutine to ensure disk for FCB in BC is logged in, if
; necessary building checksum vector, allocation bitmap & directory
; entry information for a drive

.l0c20  push    hl
        ld      hl,$0021
        add     hl,bc
        ld      a,(hl)          ; get drive from FCB
        pop     hl
.l0c27  call    l1871           ; possibly re-log drive
        ret     nc              ; exit if error
        bit     0,(ix+$1b)
        scf     
        ret     nz              ; exit if disk logged in
        push    hl
        push    de
        push    bc
        call    l0ecc           ; initialise allocation bitmap
        set     1,(ix+$1b)      ; set "collecting sector checksums"
        xor     a
        ld      (ix+$22),a
        ld      (ix+$23),a      ; set no free directory entries
        call    l0c74           ; copy last dir entry number to ext XDPB info
        ld      bc,$0000
        ld      hl,l0c61
        call    l0dae           ; generate allocation bitmap & get free entries
        ld      (ix+$24),c
        ld      (ix+$25),b      ; set last used directory entry number
        pop     bc
        pop     de
        pop     hl
        ret     nc              ; exit if error
        set     0,(ix+$1b)      ; drive logged in
        res     1,(ix+$1b)      ; just check sector checksums
        ret

; Subroutine to process a directory entry, either incrementing the number
; of free entries, or updating the allocation bitmap for the files blocks

.l0c61  call    l0c67
        scf                     ; success
        sbc     a,a             ; A=$ff
        ret     
.l0c67  ld      a,(hl)
        cp      $e5             ; is entry unused?
        jp      z,l1040         ; if so, inc #free directory entries & exit
        ld      b,d             ; BC=directory entry number
        ld      c,e
        ld      a,$ff           ; add to allocation bitmap
        jp      l0f43           ; add entry's blocks into bitmap

; Subroutine to copy last directory entry number to extended XDPB info

.l0c74  ld      a,(ix+$07)
        ld      (ix+$24),a
        ld      a,(ix+$08)
        ld      (ix+$25),a
        ret     

; Subroutine to find extent HL for current file

.l0c81  call    l0d19           ; is correct extent in FCB?
        ret     c               ; exit if so
        push    hl
        call    l0cca		; ensure directory up to date for this file
        pop     hl
        ret     nc		; exit if error
.l0c8b  ex      de,hl
        ld      hl,$000c
        add     hl,bc
        ld      (hl),d		; store low 5 bits of extent counter in FCB
        inc     hl
        inc     hl
        ld      (hl),e		; store high 8 bits of extent counter in FCB
        ld      e,$11
        xor     a
.l0c97  inc     hl
        ld      (hl),a		; clear allocation list & records in extent
        dec     e
        jr      nz,l0c97
        call    l0d4a		; find entry
        ret     c		; exit if success
        ld      hl,$0022
        add     hl,bc
        set     2,(hl)		; signal "new extent required"
        or      a
        ret     

; Subroutine to get new extent for file if required

.l0ca8  call    l0d19		; check if correct extent in FCB
        jr      nc,l0cb8	; move on if not
        ld      hl,$0022
        add     hl,bc
        bit     2,(hl)
        jp      nz,l0fd6	; move on if new extent required
        scf     		; else success
        ret     
.l0cb8  push    hl
        call    l0cca		; ensure directory up to date for this file
        pop     hl
        ret     nc
.l0cbe  call    l0c8b		; set up FCB with clean extent
        ret	c
        cp      $19		; error "end of file"
        scf     
        ccf     
        ret     nz		; exit with error except new extent required
        jp      l0fd6		; get new directory entry

; Subroutine to ensure directory is up-to-date for current file

.l0cca  ld      hl,$0022
        add     hl,bc
        ld      a,(hl)          ; get FCB flags
        and     $03
        scf     
        ret     z               ; exit with success if directory up to date
        cp      $02
        jp      z,l1038         ; move on if new entry flag set but not needed
        and     $02
        jr      nz,l0ce9        ; move on if need new entry
        ld      hl,l0d60
        call    l0dae           ; search directory for correct extent
        ret     nc              ; exit if error
        ld      a,$20           ; extent missing error
        ccf     
        ret     nz              ; exit with error if extent not found
        jr      l0cff           ; update directory from FCB & exit
.l0ce9  call    l0ff6		; get new directory entry number (DE)
        ret     nc		; exit if error
        push    hl
        push    de
        push    bc
        call    l1049		; check if datestamps in use
        jr      nz,l0cfc	; move on if not
        ld      b,$0a
.l0cf7  ld      (hl),$00	; else zeroise date stamp
        inc     hl
        djnz    l0cf7
.l0cfc  pop     bc
        pop     de
        pop     hl

; Subroutine to update directory with current FCB info

.l0cff  push    de
        push    bc
        ex      de,hl
        ld      h,b
        ld      l,c
        ld      bc,$0020
        ldir                    ; copy directory entry from FCB to "direntry"
        pop     bc
        pop     de
        call    l0e34           ; copy "direntry" to directory entry DE
        call    c,l1719         ; write all changed buffers back on this disk
        ld      hl,$0022
        add     hl,bc
        res     0,(hl)		; flag "directory up to date"
        scf     
        ret

; Subroutine to check if correct extent for record is in FCB
; On entry, DE=record number
; On exit, carry set if record is within this extent

.l0d19  push    bc
        ld      a,(ix+$04)      ; A=EXM extent mask
        cpl     
        and     $1f
        ld      b,a             ; B=low 5 bits of extent mask (inverted)
        ld      a,d
        rra     
        rra     
        rra     
        rra     
        and     $0f
        ld      l,a             ; L=high 8 bits of required extent counter
        ld      a,e
        add     a,a
        ld      a,d
        adc     a,a
        and     b
        ld      h,a             ; H=low 5 bits of required extent counter
        ld      a,b
        pop     bc
        push    hl
        push    de
        push    bc
        ex      de,hl
        ld      hl,$000e
        add     hl,bc
        ld      b,a
        ld      a,(hl)          ; A=high 8 bits of extent counter from FCB
        xor     e
        jr      nz,l0d46        ; if not same, exit
        dec     hl
        dec     hl
        ld      a,(hl)          ; A=low 5 bits of extent counter from FCB
        xor     d
        and     b
        jr      nz,l0d46        ; if not same, exit
        scf                     ; signal "correct extent"
.l0d46  pop     bc
        pop     de
        pop     hl
        ret

; Subroutine to find directory entry with filename & extent counter in FCB

.l0d4a  ld      hl,l0d60
        call    l0dae		; find entry with correct extent counter
        ret     nc		; exit if error
        ccf     
        ld      a,$19
        ret     nz		; error "end of file" if not found
        push    bc
        ld      d,b
        ld      e,c
        ld      bc,$0020
        ldir			; copy entry into FCB
        pop     bc
        scf     
        ret     
        
; Subroutine to check if directory entry found (HL) matches one in FCB
; FCB can contain wildcards & $ff for extent bytes (match any extent)
; Z set if match found

.l0d60  push    hl
        push    de
        push    bc
        ld      a,(bc)
        xor     (hl)            ; check user area
        call    z,l0d97         ; and filename
        jr      nz,l0d82        ; move on if no match
        ld      a,(de)          ; get EX
        inc     a
        jr      z,l0d78         ; move on if don't care about extent
        ld      a,(ix+$04)
        cpl     
        ld      b,a             ; B=inverse EXM
        ld      a,(de)
        xor     (hl)
        and     b
        jr      nz,l0d82        ; move on if wrong extent
.l0d78  inc     de
        inc     hl
        inc     de
        inc     hl
        ld      a,(de)          ; get S2
        cp      $ff
        jr      z,l0d82         ; move on if don't care about extent
        xor     (hl)
.l0d82  jr      l0d92           ; exit with Z set if match

; Subroutine to check if current file (BC) and file HL are the same
; Enter at l0d84 if user area flag $20 should be ignored, or at
; l0d8a if user areas must exactly match. Z set if match.

.l0d84  ld      a,(bc)
        xor     (hl)
        and     $ef		; mask off user area flag $20
        jr      l0d8c
.l0d8a  ld      a,(bc)
        xor     (hl)		; check if user areas match
.l0d8c  push    hl
        push    de
        push    bc
        call    z,l0d97		; check if filenames match
.l0d92  pop     bc
        pop     de
        pop     hl
        scf     
        ret     

; Subroutine to check if filename in directory entry at HL matches one
; in FCB at BC (may contain ? wildcards). Z set if match.
; On exit, HL points to directory entry after filename & DE points to
; FCB after filename (or at failed chars)

.l0d97  push    bc
        ld      d,b
        ld      e,c
        inc     de              ; DE points to FCB's name
        inc     hl              ; HL points to directory entry's name
        ld      b,$0b           ; 11 bytes to check
.l0d9e  ld      a,(de)          ; get next char from FCB
        cp      '?'
        jr      z,l0da8         ; ? matches anything
        xor     (hl)            ; compare chars
        and     $7f             ; mask off attributes
        jr      nz,l0dac        ; move on if no match
.l0da8  inc     de
        inc     hl
        djnz    l0d9e           ; loop back for more chars
.l0dac  pop     bc
        ret

; Subroutine to call subroutine HL with every directory entry in turn
; The subroutine it calls should set Z if it doesn't require any more
; entries. On exit, this routine leaves address of last-accessed entry
; in HL.

.l0dae  ld      (rt_dirent),hl  ; store subroutine address
        call    l1653           ; free buffers referencing directory sectors
        ld      de,$0000        ; DE=first directory entry
        push    af
.l0db8  ld      a,e
        and     $0f
        jr      nz,l0dc3        ; only get a sector every 16th entry
        pop     af
        call    l0dff           ; get sector & get/set checksum
        ret     nc              ; exit if error
        push    af
.l0dc3  pop     af
        push    af
        push    hl
        push    ix
        push    de
        push    bc
        ld      c,a
        ld      b,$07
        ld      a,e
        call    l0e6a           ; calc offset of entry in sector
        call    l023d           ; copy entry to page 7
        pop     bc
        pop     de
        pop     ix
        push    de
        call    l0ded           ; call subroutine rt_dirent
        pop     de
        pop     hl
        jr      nc,l0de7        ; move on if error
        jr      z,l0de7         ; or if done
        call    l0df5           ; increment dir entry number
        jr      nc,l0db8        ; loop back if more
.l0de7  ld      hl,direntry     ; HL points to last entry obtained
        inc     sp
        inc     sp              ; drop original AF from stack
        ret     

; Subroutine to call the subroutine whose address is at rt_dirent.
; It enters with HL=address of directory entry (direntry)

.l0ded  ld      hl,(rt_dirent)
        push    hl              ; stack routine address
        ld      hl,direntry     ; address of directory entry
        ret                     ; "return" to call routine


; Subroutine to increment DE (current directory entry)
; On exit, carry is set if no more entries
        
.l0df5  inc     de              ; increment
        ld      a,(ix+$24)
        sub     e
        ld      a,(ix+$25)
        sbc     a,d             ; test against last
        ret

; Subroutine to get sector for directory entry DE, and get/set checksum
; On exit, AHL=sector buffer address. Error if checksums didn't match

.l0dff  push    bc
        push    de
        ld      a,$02
        call    l04d9           ; DE=logical usable record number
        call    l19c0           ; DE=absolute logical sector number
.l0e09  call    l15f4           ; get AHL=page & address of buffer
        jr      nc,l0e31        ; exit if error
        ld      b,a             ; save buffer details
        push    hl
        call    l0e7d           ; get sector checksum & address to place
        jr      c,l0e2d         ; skip reserved sectors
        bit     1,(ix+$1b)
        jr      z,l0e1c         ; move on if just checking
        ld      (hl),a          ; save checksum
.l0e1c  cp      (hl)            ; set Z if checksum matches
        scf                     ; set success
        jr      z,l0e2d         ; if checksums match, exit
        call    l166c           ; free buffers referencing unchanged sectors
        ld      a,$08
        call    l1a34           ; recoverable error 8 - disk changed
        jr      nz,l0e2d        ; exit if didn't recover
        pop     hl
        jr      l0e09           ; else loop back to retry
.l0e2d  pop     hl              ; restore buffer address to AHL
        jr      nc,l0e31        ; but don't overwrite an error code
        ld      a,b
.l0e31  pop     de
        pop     bc
        ret     

; Subroutine to copy "direntry" to directory entry (DE), marking
; buffer as changed & updating checksum

.l0e34  push    hl
        push    de
        push    bc
        ld      c,e		; save low byte of directory entry number
        ld      a,$02
        call    l04d9           ; DE=directory record number
        call    l19c0           ; get DE=absolute logical sector
        push    bc
        ld      bc,$0001	; dummy FCB address
        call    l1624		; get buffer to AHL, and flag changed
        pop     bc
        jr      nc,l0e66	; exit if error
        ld      b,a		; save buffer page
        push    ix
        push    hl
        push    de
        push    bc
        ld      a,c
        call    l0e6a		; get address of directory entry
        ex      de,hl
        ld      c,$07
        call    l023d		; copy "direntry" into directory
        pop     bc
        pop     de
        pop     hl
        pop     ix
        call    l0e7d		; calculate sector checksum
        jr      c,l0e66
        ld      (hl),a		; update checksum vector
        scf     
.l0e66  pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to calculate an offset into a sector buffer for a
; directory entry
; On entry, A=lowbyte of entry number, HL=buffer address
; On exit, DE=direntry, HL=entry address, IX=$20

.l0e6a  and     $0f             ; 16 entries per sector
        jr      z,l0e75
        ld      de,$0020
.l0e71  add     hl,de
        dec     a
        jr      nz,l0e71
.l0e75  ld      de,direntry
        ld      ix,$0020
        ret     

; Subroutine to find address of sector DE (abs log) within checksum table
; (returned in HL) returning sector checksum in B
; On entry, BHL=sector buffer address

.l0e7d  push    hl
        push    de
        ex      de,hl           ; HL=abs logical sector
        ld      e,(ix+$0b)
        ld      a,(ix+$0c)
        and     $7f
        ld      d,a             ; DE=#directory records
        call    l19c0           ; DE=first non-directory sector
        sbc     hl,de           ; test logical sector
        ccf     
        pop     de
        pop     hl
        ret     c               ; exit if not after directory
        push    bc
        ld      a,b
        call    l0207           ; page in bank containing buffer
        push    af
        xor     a
        ld      bc,$0002
.l0e9c  add     a,(hl)          ; A=8-bit checksum of sector
        inc     hl
        djnz    l0e9c
        dec     c
        jr      nz,l0e9c
        ld      b,a             ; save checksum
        pop     af
        call    l0207           ; page back original bank
        ld      l,(ix+$26)
        ld      h,(ix+$27)      ; HL=address of checksum table
        add     hl,de           ;    +abs logical sector
        push    de
        ld      de,$0000
        call    l19c0           ; DE=first non-reserved sector
        or      a
        sbc     hl,de           ; HL=address of sector checksum
        pop     de
        ld      a,b
        or      a
        pop     bc              ; restore checksum
        ret     

; Subroutine to check if file (HL=FCB) is read-only, giving error if so

.l0ebe  push    de
        ex      de,hl
        ld      hl,$0009
        add     hl,de
        ld      a,(hl)		; get read-only bit
        add     a,a		; move to carry
        ex      de,hl
        pop     de
        ccf     		; carry is inverse of read-only bit
        ld      a,$1c		; "read-only file" error
        ret     

; Subroutine to initialise the allocation bitmap of a drive with the
; directory bitmaps

.l0ecc  call    l0fc9           ; get HL=allocation bitmap, DE=last block #
        ld      a,$03
        call    l04d9
        inc     de              ; DE=(last block #)/8+1
        push    hl
.l0ed6  ld      (hl),$00        ; zero allocation for 8 blocks
        inc     hl
        dec     de
        ld      a,d
        or      e
        jr      nz,l0ed6        ; loop back for more
        pop     hl
        ld      a,(ix+$09)      ; store directory bitmaps
        ld      (hl),a
        inc     hl
        ld      a,(ix+$0a)
        ld      (hl),a
        ret


; Subroutine to allocate/deallocate a block
; On entry, DE=block number and C=$00 to remove block from allocation bitmap
; or $ff to add block to allocation bitmap

.l0ee9  push    bc
        push    hl
        push    de
        ld      a,$03
        call    l04d9           ; DE=block/8
        push    de
        call    l0fc9           ; get HL=address of allocation bitmap
        pop     de
        add     hl,de           ; HL points to correct allocation byte
        pop     de
        ld      a,e
        and     $07
        ld      b,a             ; B=bit within byte
        ld      a,$01
        inc     b
.l0eff  rrca
        djnz    l0eff           
        ld      b,a             ; B=bitmask
        and     c
        ld      c,a             ; C=bit if allocating, 0 if deallocating
        ld      a,b
        cpl     
        and     (hl)
        or      c
        ld      (hl),a          ; update bit in allocation bitmap
        pop     hl
        pop     bc
        ret

; Subroutine to allocate a new block for a file (returned in DE)

.l0f0d  push    hl
        push    bc
        call    l0fc9		; get HL=allocation bitmap & DE=last block
.l0f12  ld      bc,$0880	; bit counter & mask
.l0f15  ld      a,(hl)		; get byte from allocation bitmap
        and     c
        jr      z,l0f27		; move on if found free block
        rrc     c		; shift bit mask
        ld      a,d
        or      e
        ld      a,$1a
        jr      z,l0f3d		; move on if no blocks left
        dec     de
        djnz    l0f15		; loop back for more blocks in allocation byte
        inc     hl
        jr      l0f12		; back for more allocation bytes
.l0f27  ld      a,(hl)
        or      c
        ld      (hl),a		; allocate this block
        ld      a,(ix+$05)
        sub     e
        ld      e,a
        ld      a,(ix+$06)
        sbc     a,d
        ld      d,a		; calculate DE=allocated block
        pop     bc
        push    bc
        ld      hl,$0022
        add     hl,bc
        set     0,(hl)		; signal directory contains valid FCB data
        scf     
.l0f3d  pop     bc
        pop     hl
        ret     

; Subroutine to deallocate all blocks in current FCB

.l0f40  ld      h,b		; HL=FCB address
        ld      l,c
        xor     a		; deallocate

; Subroutine to add/remove all the blocks in a directory entry to
; the allocation bitmap
; On entry, HL=address of directory entry, A=$ff to add or $00 to remove

.l0f43  push    bc              ; save BC
        ld      c,a             ; C=allocate/deallocate flag
        ld      a,$0f
        cp      (hl)
        jr      c,l0f72         ; exit if not a file entry
        ld      de,$0010
        add     hl,de           ; HL points to allocation list
        ld      b,$10           ; 16 allocation entries to consider
        inc     b
        jr      l0f70           ; go to start loop
.l0f53  ld      e,(hl)          ; E=next allocation entry
        inc     hl
        ld      a,(ix+$06)      ; more than 256 blocks on disk?
        or      a
        ld      d,a             ; DE=block number if 8-bit block numbers
        jr      z,l0f5f
        dec     b
        ld      d,(hl)          ; DE=16-bit block number
        inc     hl
.l0f5f  ld      a,d
        or      e
        jr      z,l0f70         ; move on if allocation entry null
        push    hl
        ld      a,(ix+$05)
        sub     e
        ld      a,(ix+$06)
        sbc     a,d             ; test against last block number
        call    nc,l0ee9        ; if in range, allocate/deallocate block
        pop     hl
.l0f70  djnz    l0f53           ; loop back
.l0f72  pop     bc
        scf     
        ret     

; Subroutine to calculate extent size in K (to HL)

.l0f75  push    de
        ex      de,hl
        ld      a,(de)
        cp      $10		; test user area
        ld      hl,$0000	; zero blocks so far
        jr      nc,l0f99	; skip calculation if password entry
        ld      hl,$0010
        add     hl,de		; point to allocation list
        ld      de,$1000	; D=#bytes to test,E=#blocks found
.l0f86  ld      a,(ix+$06)
        or      a		; more than 255 blocks?
        ld      a,(hl)		; get block number
        inc     hl
        jr      z,l0f91		; if not move on
        or      (hl)		; else incorporate 2nd byte
        dec     d		; and move over
        inc     hl
.l0f91  or      a
        jr      z,l0f95		
        inc     e		; increment #blocks if not null
.l0f95  dec     d
        jr      nz,l0f86	; loop back for rest of allocation list
        ex      de,hl		; HL=#blocks, to be converted to K next
.l0f99  pop     de

; Subroutine to convert HL=blocks to K

.l0f9a  ld      a,(ix+$02)	; get block size
        dec     a
        dec     a
.l0f9f  dec     a
        jr      z,l0fa5		; move on when HL=size in K
        add     hl,hl		; double it
        jr      l0f9f		; loop back
.l0fa5  ld      a,h
        or      l		; set Z if HL=0
        scf     		; success
        ret     

; Subroutine to calculate HL=free space (in K) on current drive

.l0fa9  ld      hl,$0000	; 0 free blocks so far
        push    hl		; stack it
        call    l0fc9		; HL=alloc bitmap, DE=last block number
.l0fb0  ld      bc,$0880	; bit count & mask
.l0fb3  ld      a,(hl)
        and     c		; check if block free
        jr      nz,l0fba
        ex      (sp),hl
        inc     hl		; increment #free if so
        ex      (sp),hl
.l0fba  rrc     c		; shift bit mask
        ld      a,d
        or      e
        jr      z,l0fc6		; move on if no more blocks
        dec     de
        djnz    l0fb3           ; loop back for rest of allocation byte
        inc     hl
        jr      l0fb0           ; loop back
.l0fc6  pop     hl		; HL=#free blocks
        jr      l0f9a           ; convert blocks to K & exit

; Subroutine to get HL=allocation bitmap & DE=last block number from XDPB

.l0fc9  ld      l,(ix+$28)
        ld      h,(ix+$29)      ; HL=add of allocation bitmap
        ld      e,(ix+$05)
        ld      d,(ix+$06)      ; DE=last block number
        ret

; Subroutine to check if new directory entry available

.l0fd6  ld      a,(ix+$22)
        or      (ix+$23)
        ld      a,$1b
        ret     z		; error "directory full" if no new entries
        ld      hl,$0022
        add     hl,bc
        set     1,(hl)		; new directory entry needed
        res     2,(hl)		; extent now created
        ld      a,(ix+$22)
        sub     $01		; decrement #free directory entries
        ld      (ix+$22),a
        jr      nc,l0ff4
        dec     (ix+$23)
.l0ff4  scf
        ret     

; Subroutine to get a new directory entry (DE)

.l0ff6  push    bc
        ld      c,(ix+$24)
        ld      b,(ix+$25)	; BC=last used directory entry number
        call    l0c74		; set last used=last entry
        ld      hl,l1033
        call    l0dae		; find first unused entry
        jr      nc,l1031        ; exit if error
        ex      (sp),hl
        push    hl
        push    de
        push    af
        ld      de,$0022
        add     hl,de
        res     1,(hl)		; reset "new directory entry" flag
        pop     af
        pop     de
        pop     hl
        ex      (sp),hl
        jr      nz,l102b        ; if no free entry found, cause error
        ex      de,hl
        or      a
        sbc     hl,bc
        add     hl,bc
        ex      de,hl
        jr      c,l1022         ; move on if entry lower than last used
        ld      b,d
        ld      c,e		; else last used=this entry
.l1022  ld      (ix+$24),c
        ld      (ix+$25),b	; set last used entry
        scf			; success
        jr      l1031
.l102b  call    l1040		; re-increment free directory entries
        ld      a,$20		; "extent missing" error
        or      a
.l1031  pop     bc
        ret     

; Subroutine to check if directory entry at HL is free

.l1033  ld      a,(hl)
        xor     $e5		; set Z if free
        scf     		; success
        ret     

; Subroutine to clear new entry flag & increment # free entries

.l1038  push    hl
        ld      hl,$0022
        add     hl,bc
        res     1,(hl)          ; we don't need a new entry
        pop     hl
.l1040  scf                     ; success
        inc     (ix+$22)        ; increment # free directory entries
        ret     nz
        inc     (ix+$23)
        ret     

; Subroutine to check for datestamps, and if present to exit with Z set
; and HL pointing to address to place datestamp for entry DE

; *BUG* This routine assumes that on entry, HL contains the address of a
; directory entry within a record, and tries to access the last entry in
; the record. However, HL points to a copy of the entry at "direntry",
; so the routine checks for a datestamp at $dfc2 (unused), $dfe2 (byte 2 of
; BCB 0) or $e002 (byte 1 of BCB 3).
; Luckily, none of these locations can ever hold a datestamp identifier,
; so the effect of the bug is that datestamps are always ignored. 

.l1049  ld      a,e
        and     $03
        cpl     
        add     a,$04
        jr      z,l1058
        ld      bc,$0020
.l1054  add     hl,bc		; generate HL=address of last entry in record
        dec     a
        jr      nz,l1054
.l1058  ld      a,(hl)
        cp      $21		; check if datestamp entry
        ret     nz		; exit if not
        ld      a,e
        and     $03
        jr      z,l1068
        ld      bc,$000a
.l1064  add     hl,bc		; generate address of datestamp for entry
        dec     a
        jr      nz,l1064
.l1068  inc     hl
        ret     

        defs    6

; DOS_GET_POSITION

.l1070  call    l0525           ; get FCB
        ret     nc              ; exit if error
.l1074  ld      hl,$0026        ; offset for file position
        jr      l1080           ; move on

; DOS_GET_EOF

.l1079  call    l0525           ; get FCB
        ret     nc              ; exit if error
        ld      hl,$0023        ; offset for EOF
.l1080  add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      a,(hl)
        ex      de,hl
        ld      e,a
        ld      d,$00           ; DEHL=file position/EOF
        scf                     ; success
        ret     

; DOS_SET_POSITION

.l108c  call    l0525           ; get FCB
        ret     nc              ; exit if error
.l1090  ld      a,l
        ld      d,e
        ld      e,h             ; DEA=new filepos
        ld      hl,$0026
        add     hl,bc           ; HL=add of filepos in FCB
        jr      l10ad           ; move on

; Subroutine to increment filepointer by 1 (l09d), $80 (l1099) or A (l109f)
; If this caused the filepointer to cross a record boundary, bit 5 of
; the FCB flags byte is reset

.l1099  ld      a,$80
        jr      l109f
.l109d  ld      a,$01
.l109f  ld      hl,$0026
        add     hl,bc           ; HL=add of filepos in FCB
        add     a,(hl)          ; add in A
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        jr      nc,l10ab
        inc     de
.l10ab  dec     hl              ; move HL back to start of filepos
        dec     hl
.l10ad  push    hl              ; save registers
        push    af
        xor     (hl)
        jp      m,l10bd         ; move on if record changed
        inc     hl
        ld      a,(hl)
        cp      e
        jr      nz,l10bd
        inc     hl
        ld      a,(hl)
        cp      d
        jr      z,l10c3         ; skip next bit if record unchanged
.l10bd  ld      hl,$0022
        add     hl,bc
        res     5,(hl)          ; flag "record changed"
.l10c3  pop     af
        pop     hl
        ld      (hl),a
        inc     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; set new filepointer
        scf                     ; success
        ret     

; Subroutine to ensure filelength is at least as large as current filepointer

.l10cc  push    bc
        push    af
        ld      hl,$0022
        add     hl,bc
        ex      de,hl		; DE points to filelength-1
        ld      hl,$0025
        add     hl,bc		; HL points to filepointer-1
        ld      b,$03
        or      a
.l10da  inc     de
        inc     hl
        ld      a,(de)
        sbc     a,(hl)		; compare filepointer with filelength
        djnz    l10da
        jr      nc,l10e7	; exit if filepointer within current file
        ld      bc,$0003
        lddr    		; make filelength=filepointer
.l10e7  pop     af
        pop     bc
        ret     

; DOS_READ

.l10ea  ld      a,c
        ld      (rw_page),a	; save page & address to read to
        ld      (rw_add),hl
        call    l0514		; test if file B open for reading
        ret     nc		; exit if not
        add     hl,de		; calculate final address
        push    hl
        call    l1107		; do the read
        pop     hl
        ret     c		; exit if successful
        push    af
        ld      de,(rw_add)	; get current address
        or      a
        sbc     hl,de
        ex      de,hl		; DE=unread bytes
        pop     af
        ret     

; Subroutine to read DE bytes from file

.l1107  push    bc
        push    de
        ld      hl,$0023
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      b,(hl)		; get BDE=file length
        inc     hl
        ld      a,e
        scf     
        sbc     a,(hl)
        ld      e,a
        inc     hl
        ld      a,d
        sbc     a,(hl)
        ld      d,a
        inc     hl
        ld      a,b
        sbc     a,(hl)
        ex      de,hl		; AHL=filelength available to read
        pop     de
        pop     bc
        jr      c,l1130         ; exit if none
        dec     de		; DE=#bytes to read-1
        sbc     hl,de
        add     hl,de
        sbc     a,$00
        jr      nc,l1134        ; if enough bytes in file, move on
        ex      de,hl
        call    l1134		; else read what's available
        ret     nc		; exit if error
.l1130  ld      a,$19		; "end of file" error
        or      a
        ret     
.l1134  ld      hl,$0026
        add     hl,bc
        ld      a,(hl)
        and     $7f
        jr      z,l1144         ; move on if filepointer on record boundary
        call    l1158		; read byte
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l1134           ; loop back
.l1144  ld      hl,$ff81
        add     hl,de
        jr      nc,l1151	; move on if don't need any more full records
        call    l1175		; read a record
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l1144		; loop back
.l1151  call    l1158		; read byte
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l1151		; loop back

; Subroutine to read a byte from the file to correct address
; On exit, Z set if finished

.l1158  call    l11cb		; read a byte
        ret     nc		; exit if error
        push    de
        ld      e,a
        ld      a,(rw_page)
        ld      hl,(rw_add)
        call    l0207		; page in bank
        ld      (hl),e		; store byte
        call    l0207		; page back original bank
        inc     hl
        ld      (rw_add),hl	; update address to read to
        pop     de
        ld      a,d
        or      e		; set Z if finished
        dec     de		; decrement #bytes to read
        scf     		; success
        ret     

; Subroutine to read a record from the file to correct address
; On exit, Z set if finished

.l1175  push    de
        call    l137d		; get record to buffer & update FCB
        pop     de
        ret     nc		; exit if error
        push    de
        call    l1099		; increment filepointer by $80
        ld      hl,$002d
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)		; DE=address of record
        inc     hl
        push    bc
        ld      c,(hl)		; C=page of buffer
        ld      a,(rw_page)
        ld      b,a
        ld      hl,(rw_add)
        ex      de,hl
        ld      ix,$0080
        call    l023d		; copy record from buffer
        pop     bc
        ld      (rw_add),de	; update address to read to
        pop     de
        ld      hl,$ff81
        add     hl,de
        ex      de,hl		; subtract $7f from bytes to read
        ld      a,d
        or      e		; set Z if done
        dec     de		; subtract further byte
        scf     		; success
        ret     

; DOS_BYTE_READ

.l11a8  call    l0514           ; get FCB & test if open for reading
        ret     nc              ; exit if error
        ld      hl,$0026
        add     hl,bc
        ex      de,hl           ; DE=filepointer address
        ld      hl,$0023
        add     hl,bc           ; HL=filelength address
        push    bc
        ld      b,$03
        or      a
.l11b9  ld      a,(de)          ; test filepointer (carry must be set
        sbc     a,(hl)          ; if pointer within file)
        inc     de
        inc     hl
        djnz    l11b9
        pop     bc
        ld      a,$19           ; error "end of file"
        call    c,l11cb         ; get a byte if within file
        ret     nc              ; exit if error
        ld      c,a             ; C=byte read
        cp      $1a             ; set Z if soft-EOF
        scf                     ; success
        ret     

; Subroutine to read a byte (A) from the file

.l11cb  push    hl
        push    de
        ld      hl,$0022
        add     hl,bc
        bit     5,(hl)          ; has record been changed?
        jr      nz,l11da
        call    l137d           ; if so, get new record details into FCB
        jr      nc,l11fb        ; exit if error
.l11da  ld      hl,$0026
        add     hl,bc
        ld      a,(hl)          ; low byte of filepointer
        and     $7f             ; offset into record
        ld      hl,$002d
        add     hl,bc
        add     a,(hl)
        ld      e,a
        inc     hl
        adc     a,(hl)
        sub     e
        ld      d,a             ; DE=address of byte in buffer
        inc     hl
        ld      a,(hl)          ; A=bank of buffer
        ex      de,hl
        call    l0207           ; page in buffer bank
        ld      d,(hl)          ; get byte
        call    l0207           ; page back original bank
        push    de
        call    l109d           ; increment filepointer
        pop     af              ; A=byte
        scf                     ; success
.l11fb  pop     de
        pop     hl
        ret

; DOS_WRITE

.l11fe  ld      a,c
        ld      (rw_page),a	; store bank & address to write from
        ld      (rw_add),hl
        call    l051c		; test file B open for writing
        ret     nc		; exit if error
        add     hl,de		; calculate final address
        push    hl
        call    l121e		; do the write
        call    l10cc		; ensure filelength includes filepointer
        pop     hl
        ret     c		; exit if successful
        push    af
        ld      de,(rw_add)
        or      a
        sbc     hl,de
        ex      de,hl		; DE=bytes unwritten
        pop     af
        ret     

; Subroutine to write DE bytes to file

.l121e  dec     de
.l121f  ld      hl,$0026
        add     hl,bc
        ld      a,(hl)
        and     $7f
        jr      z,l122f		; move on if filepointer on record boundary
        call    l1243		; write a byte
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l121f		; loop back
.l122f  ld      hl,$ff81
        add     hl,de
        jr      nc,l123c	; move on if don't need to write full record
        call    l1261		; write a record
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l122f		; loop back
.l123c  call    l1243		; write a byte
        ret     nc		; exit if error
        ret     z		; exit if finished
        jr      l123c		; loop back

; Subroutine to write a byte to file

.l1243  ld      a,(rw_page)	; get bank & address
        ld      hl,(rw_add)
        call    l0207		; page in bank
        ld      l,(hl)		; get byte
        call    l0207		; page back original bank
        ld      a,l
        call    l12a5		; write the byte
        ret     nc		; exit if error
        ld      hl,(rw_add)
        inc     hl
        ld      (rw_add),hl	; update address to write from
        ld      a,d
        or      e		; test if finished
        dec     de		; decrement bytes to write
        scf     
        ret     

; Subroutine to write a record to file

.l1261  push    de
        call    l134e		; get record to buffer at AHL
        pop     de
        ret     nc		; exit if error
        ld      hl,$0022
        add     hl,bc
        set     4,(hl)		; signal "current record changed"
        push    de
        call    l1099		; increment filepointer by $80
        ld      hl,$002d
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        inc     hl
        push    bc
        ld      b,(hl)		; get B=bank, DE=address of buffer
        ld      a,(rw_page)
        ld      c,a
        ld      hl,(rw_add)
        ld      ix,$0080
        call    l023d		; copy record to buffer
        pop     bc
        ld      (rw_add),hl	; update address to write from
        pop     de
        ld      hl,$ff81
        add     hl,de
        ex      de,hl		; subtract $7f from bytes to read
        ld      a,d
        or      e		; set Z if finished
        dec     de		; subtract further byte
        scf     
        ret     

; DOS_BYTE_WRITE

.l1298  ld      e,c
        call    l051c		; check file open for writing
        ret     nc		; exit if not
        ld      a,e
        call    l12a5		; write byte A at filepointer
        call    c,l10cc		; ensure filelength includes filepointer
        ret     

; Subroutine to place byte A in file at current filepointer, and increment
; filepointer

.l12a5  push    hl
        push    de
        ld      e,a		; save byte in E
        ld      hl,$0022
        add     hl,bc
        bit     5,(hl)
        jr      nz,l12b9	; move on if FCB contains valid record details
        push    hl
        push    de
        call    l134e		; get record to buffer
        pop     de
        pop     hl
        jr      nc,l12db	; exit if error
.l12b9  set     4,(hl)		; signal "record changed"
        ld      hl,$0026
        add     hl,bc
        ld      a,(hl)
        and     $7f		; A=offset into current record
        ld      hl,$002d
        add     hl,bc
        push    de
        add     a,(hl)
        ld      e,a
        inc     hl
        adc     a,(hl)
        sub     e
        ld      d,a		; DE=address in file
        inc     hl
        ld      a,(hl)		; A=bank of file
        ex      de,hl
        pop     de
        call    l0207		; page in file bank
        ld      (hl),e		; store byte at filepointer
        call    l0207		; page back original bank
        call    l109d		; increment filepointer
.l12db  pop     de
        pop     hl
        ret     

; Subroutine to test if file has valid header checksum (Z set if so)

.l12de  call    l12f7		; get stored & calculated checksum
        ret     nc		; exit if error
        ld      a,d
        cp      e		; set Z if they match
        scf     		; success
        ret     

; Subroutine to update checksum in file header & set filepointer after it

.l12e6  call    l12f7		; get header checksums
        ret     nc		; exit if error
        call    l0207		; page in bank file bank
        ld      (hl),e		; store calculated header checksum
        call    l0207		; page back original bank
        call    l1099		; increment filepointer past header
        jp      l10cc		; ensure filelength includes filepointer

; Subroutine to get stored header checksum (D) & calculated checksum (E)

.l12f7  ld      hl,$0000
        ld      e,h
        call    l1090		; set position to file start
        ld      hl,$0022
        add     hl,bc
        bit     5,(hl)
        jr      nz,l130a	; move on if valid record details in FCB
        call    l137d		; get record to buffer and update FCB
        ret     nc		; exit if error
.l130a  ld      hl,$002d
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)		; DE=address of file start
        inc     hl
        ld      a,(hl)		; A=bank of file
        ex      de,hl
        push    af
        call    l0207		; page in file bank
        push    af
        xor     a		; zero checksum
        ld      e,$7f
.l131c  add     a,(hl)		; form checksum
        inc     hl
        dec     e
        jr      nz,l131c	; back for more bytes in header
        ld      e,a		; E=calculated checksum
        ld      d,(hl)		; D=checksum stored in file header
        pop     af
        call    l0207		; page back original bank
        pop     af
        scf     
        ret     

; Subroutine to get sector in FCB to a buffer

.l132a  push    hl
        ld      hl,$0022
        add     hl,bc           ; HL points to flags of FCB
        bit     3,(hl)
        jr      z,l1347         ; move on if sector already in buffer
        bit     4,(hl)
        jr      z,l1347         ; or if current record not changed
        push    hl
        push    de
        ld      hl,$002b
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=logical sector
        call    l1624           ; get AHL=buffer address (flag changed)
        pop     de
        pop     hl
        jr      nc,l134c        ; move on if error
.l1347  ld      a,(hl)
        and     $c7             ; reset bits 3,4 & 5
        ld      (hl),a
        scf                     ; success
.l134c  pop     hl
        ret     

; Subroutine to get abs logical sector (DE) & record address (AHL)
; for record (DE) in current file, creating new record if required

.l134e  ld      a,(bc)
        cp      $22		; test for "drive open as file"
        ld      hl,l135b	; routine to use for normal file
        jr      nz,l1388
        ld      hl,l14eb	; routine to use for drive
        jr      l1388

; Subroutine to find abs log sector (DE) and record address (AHL)
; for possibly new record DE in current file

.l135b  push    de
        call    l0ca8		; get new extent if required
        pop     de
        call    c,l13f0		; get record address & sector number
        ret     nc		; exit if error
        push    hl
        push    de
        push    af
        call    l13d2		; get DE=record number from FCB
        call    l14b0		; check in current extent
        call    nc,l1493	; if so, set FCBs extent counter & #records
        pop     af
        pop     de
        pop     hl
        ret

; Subroutine to get the abs log sector (DE) and address (AHL) of
; record DE in the current file

.l1374  push    de
        call    l0c81		; find extent HL for file
        pop     de
        call    c,l1423		; if found, get address of record 
        ret     

; Subroutine to get the current record into a buffer and update the FCB
; with its details

.l137d  ld      a,(bc)
        cp      $22		; test for "drive open as file"
        ld      hl,l1374        ; routine to use for normal file
        jr      nz,l1388
        ld      hl,l14eb        ; routine to use for drive
.l1388  call    l13dc           ; DE=record number for filepointer
        ret     nc              ; exit if file too big
        push    hl
        ld      hl,$0022
        add     hl,bc
        bit     3,(hl)
        pop     hl
        jr      z,l13a2         ; move on if no sector currently in buffer
        push    hl
        ex      de,hl
        call    l13d2           ; get record number from FCB
        ex      de,hl
        or      a
        sbc     hl,de
        pop     hl
        jr      z,l13c8         ; move on if record numbers match
.l13a2  call    l0c20           ; ensure correct disk logged in
        call    c,l132a         ; get sector in FCB to buffer
        ret     nc              ; exit if error
        push    hl
        ld      hl,$0029
        add     hl,bc
        ld      (hl),e
        inc     hl
        ld      (hl),d
        pop     hl
        call    l13d2           ; DE=record number required
        call    l04ec           ; call routine in HL
        ret     nc              ; exit if error
        push    hl
        ld      hl,$002b
        add     hl,bc
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; store abs logical sector number
        pop     de
        inc     hl
        ld      (hl),e
        inc     hl
        ld      (hl),d          ; store add of record
        inc     hl
        ld      (hl),a          ; store bank for buffer
.l13c8  ld      hl,$0022
        add     hl,bc
        ld      a,(hl)
        or      $28             ; set bit 3 (valid sector) & bit 5
        ld      (hl),a          ; (valid filepointer)
        scf     
        ret

; Subroutine to get DE=record number from FCB

.l13d2  push    hl
        ld      hl,$0029
        add     hl,bc
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        pop     hl
        ret

; Subroutine to get DE=record number for filepointer

.l13dc  push    hl
        ld      hl,$0026
        add     hl,bc           ; HL points to filepointer
        ld      a,(hl)
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl           ; HLA=filepointer
        add     a,a
        adc     hl,hl           ; double filepointer
        ex      de,hl           ; DE=record number
        ccf                     ; successful if no carry
        ld      a,$22           ; error "file too big"
        pop     hl
        ret     

; Subroutine to get record address (AHL) and sector (DE) for record DE,
; creating new record if required

.l13f0  push    de
        call    l1459		; get block & offset
        ex      de,hl
        ex      (sp),hl
        ex      de,hl
        jr      c,l140a		; move on if not end-of-file
        call    l0f0d		; allocate a new block
        jr      nc,l1413	; move on if error
        ld      (hl),e		; insert block in allocation list
        ld      a,(ix+$06)
        or      a
        jr      z,l1407
        inc     hl
        ld      (hl),d
.l1407  ex      de,hl		; HL=block number
        jr      l1411
.l140a  ld      a,e
        and     $03
        scf     
        call    z,l14b0		; if first record in a sector, check in extent
.l1411  sbc     a,a		; Z set if record newly created
        scf     
.l1413  pop     de
        call    c,l143c		; get abs logical sector & offset
        push    hl
        call    c,l141d		; get sector to buffer
        jr      l1435		; exit, getting required record to AHL

; Subroutine to get sector DE to a buffer, don't bother reading in if
; newly created record (Z set)

.l141d  jp      z,l160c		; if created, don't read sector
        jp      l15f4		; get sector, reading if necessary

; Subroutine to find abs log sector (DE) and record address (AHL)
; for record DE in current file

.l1423  push    de
        call    l1459		; get block & offset
        ex      de,hl
        ex      (sp),hl
        ex      de,hl
        call    c,l14b0		; check record is in this extent
        pop     de
        call    c,l143c		; calculate sector & offset
.l1431  push    hl		; stack offset into sector
        call    c,l15f4		; get AHL=address of sector DE
.l1435  ex      de,hl
        ex      (sp),hl		; stack sector, restore offset
        push    af
        add     hl,de		; now AHL=address of record
        pop     af
        pop     de		; restore sector
        ret     

; Subroutine to calculate abs logical sector (DE) and offset (HL) from
; block number (HL) offset into block (DE)

.l143c  push    bc
        push    af
        ex      de,hl
        ld      a,(ix+$02)
        call    l04e3		; DE=logical usable record
        call    l19c0		; DE=abs logical sector of block start
        ex      de,hl
        ld      b,d		; save high byte of offset
        ld      a,d
        and     $01
        ld      d,a
        ex      de,hl		; HL=offset into sector
        xor     b
        rrca    		; A=#sectors offset
        add     a,e
        ld      e,a
        adc     a,d
        sub     e
        ld      d,a		; DE=abs logical sector
        pop     af
        pop     bc
        ret     

; Subroutine to calculate block number (HL) and offset (DE) of record
; DE in file, assuming correct extent is in FCB

.l1459  push    bc
        ld      h,b		; HL=FCB
        ld      l,c
        ld      a,(ix+$03)
        and     e		; A=record within block
        rra     
        ld      b,a
        ld      a,$00
        rra     
        ld      c,a		; BC=byte offset within block
        ld      a,(ix+$02)
        call    l04d9
        ld      d,$00		; DE=block number (low byte)
        ld      a,(ix+$06)
        or      a
        ld      a,e
        jr      z,l1480		; move on if <256 blocks on disk with A=block
        and     $07
        add     a,a
        add     a,$11
        ld      e,a
        add     hl,de		; for 256+ blocks, each uses 2 bytes in list
        ld      d,(hl)		; D=high byte of actual block number
        dec     hl		; HL points to low byte
        jr      l1486
.l1480  and     $0f
        add     a,$10
        ld      e,a
        add     hl,de		; for <256 blocks, each uses 1 byte in list
.l1486  ld      e,(hl)		; now, DE=block number on disk
        ld      a,d
        or      e
        ld      a,$19
        jr      z,l148f		; if 0, exit with "end of file" error
        ex      de,hl		; HL=block number
        scf     		; success
.l148f  ld      d,b
        ld      e,c
        pop     bc
        ret     

; Subroutine to set extent counter & number of records in extent

.l1493  push    hl
        ld      a,e
        and     $7f
        inc     a
        ld      hl,$000f
        add     hl,bc
        ld      (hl),a		; set number of records in extent
        ld      a,e
        rla     
        ld      a,d
        rla     
        and     $1f
        dec     hl
        dec     hl
        dec     hl
        ld      (hl),a		; set extent counter (low 5 bits)
        ld      hl,$0022
        add     hl,bc
        set     0,(hl)		; directory contains valid copy of FCB data
        scf     
        pop     hl
        ret     

; Subroutine to check record number DE is in the current extent

.l14b0	push    de
        push    hl
        call    l14c2		; find last record number in extent
        or      a
        ld      a,$22
        jr      nz,l14bf	; "file too big" error
        ex      de,hl
        sbc     hl,de		; check record is in this extent
        ld      a,$19		; "end of file" error if not
.l14bf  pop     hl
        pop     de
        ret     

; Subroutine to calculate ADE=last record in extent+1

.l14c2  push    de
        ld      hl,$000c
        add     hl,bc
        ld      d,(hl)
        ld      e,$00
        srl     d
        rr      e
        inc     hl
        inc     hl
        inc     hl
        ld      a,(hl)
        or      a
        jp      p,l14d8
        ld      a,$80
.l14d8  add     a,e
        ld      e,a
        adc     a,d
        sub     e
        dec     hl
        ld      l,(hl)
        ld      h,$00
        add     hl,hl
        add     hl,hl
        add     hl,hl
        add     hl,hl
        add     a,l
        ld      d,a
        adc     a,h
        sub     d		; A=#records in this extent
        ex      de,hl		; DE=last record in extent+1
        pop     de
        ret

; Subroutine to find abs log sector (DE) and record address (AHL)
; for record DE in current file (which is an open drive)

.l14eb  ld      a,e
        and     $03
        ld      h,a
        ld      l,$00
        srl     h
        rr      l		; HL=offset of record into sector
        ld      a,$02
        call    l04d9		; DE=absolute logical sector
        push    hl
        push    de
        ex      de,hl
        ld      a,(ix+$02)
        ld      e,(ix+$05)
        ld      d,(ix+$06)
        inc     de
        call    l04e3
        call    l19c0		; DE=max sector on disk
        or      a
        sbc     hl,de		; no carry (error) if sector > max
        pop     de
        pop     hl
        ld      a,$19		; error "end of file"
        jp      l1431		; go to get sector & calculate address

        defs    25

; Subroutine to get D=first buffer for cache, E=number of cache buffers

.l1530  ld      de,(cachenum)   ; get the values
        ret

; Subroutine to set cache to D=first buffer, E=# buffers

.l1535  call    l1706           ; clear current cache
        ret     nc              ; exit if error
.l1539  ld      hl,$0000
        ld      (bcb_inuse),hl  ; clear last in-use BCB address
        ld      (bcb_free),hl   ; clear last free BCB address
        ld      h,d
        ld      (cachenum),hl   ; set first buffer=D, no buffers yet
        ld      ix,bcbs         ; address for first BCB
        ld      b,$10           ; max 16 buffers required
        ld      a,$07
        ld      hl,cache7       ; first buffer in bank 7
        jr      l1561           ; move on
.l1553  ld      a,e
        or      a
        scf                     ; exit if created all required BCBs
        ret     z
        ld      hl,cachenum
        inc     (hl)            ; increment # buffers
        ld      a,d
        inc     d               ; increment buffer number
        dec     e               ; decrement buffers left to create
        call    l022b           ; get address & bank of buffer D
.l1561  ld      (ix+$08),l      
        ld      (ix+$09),h      ; store buffer address
        ld      (ix+$0a),a      ; and bank
        ld      hl,(bcb_free)
        ld      (ix+$00),l
        ld      (ix+$01),h      ; store add of previous BCB
        ld      (bcb_free),ix   ; current BCB is now last
        ex      de,hl
        ld      de,$000b
        add     ix,de           ; address of next BCB
        ex      de,hl
        djnz    l1553           ; back for more
        scf                     ; success
        ret

; Subroutine to find BCB (address in DE) of buffer holding log sector in BC
; on current drive

.l1582  ld      de,bcb_inuse    ; last in-use buffer address
.l1585  ex      de,hl
        call    l179b           ; get next in-use BCB
        ccf     
        ld      a,$21
        ret     z               ; fail with error "uncached" if none
        push    hl
        ld      hl,$0005
        add     hl,de
        ld      a,(hl)
        cp      (ix+$1c)        ; is buffer from current drive?
        jr      nz,l15a0        ; move on if not
        inc     hl
        ld      a,(hl)
        inc     hl
        ld      h,(hl)          ; HL=abs logical sector in buffer
        ld      l,a
        or      a
        sbc     hl,bc
.l15a0  pop     hl
        jr      nz,l1585        ; loop back if not required buffer
        scf                     ; success
        ret     

; Subroutine to get a buffer for logical sector BC
; On exit, DE=BCB

.l15a5  call    l1798           ; get last free BCB
        jr      nz,l15c4        ; move on if got one
        ld      de,bcb_inuse    ; address of in-use BCB chain
.l15ad  ex      de,hl
        call    l179b           ; get next in-use BCB
        ccf     
        ld      a,$21
        ret     z               ; "uncached" error if no more inuse BCBs
        call    l17a3           ; is this last BCB in chain?
        jr      nz,l15ad        ; loop back if not
        call    l15db           ; get sector for BCB to memory
        call    l16ec           ; write back data from this buffer if required
        ret     nc              ; exit if error
        call    l17aa           ; move BCB to the free chain
.l15c4  push    hl
        ld      hl,$0002
        add     hl,de
        xor     a
        ld      (hl),a          ; no data changed
        inc     hl
        ld      (hl),a
        inc     hl
        ld      (hl),a          ; no file owner
        inc     hl
        ld      a,(ix+$1c)
        ld      (hl),a          ; drive
        inc     hl
        ld      (hl),c
        inc     hl
        ld      (hl),b          ; absolute logical sector
        pop     hl
        scf                     ; success
        ret

; Subroutine to get sector for BCB into memory

.l15db  push    ix
        push    hl
        push    de
        push    bc
        ld      hl,$0005
        add     hl,de
        ld      a,(hl)
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        ex      de,hl
        ld      d,a             ; D=drive letter, HL=abs logical sector
        call    l05f0           ; get sector to buffer, updating FCBs
        pop     bc
        pop     de
        pop     hl
        pop     ix
        ret

; Subroutine to return page (A) & address (HL) for absolute logical sector DE
; reading to a new buffer if necessary

.l15f4  bit     3,(ix+$1b)
        jp      nz,l1ab6        ; move on if RAMdisk
        push    de
        push    bc
        ld      b,d
        ld      c,e             ; BC=abs logical sector
        call    l1582           ; check if sector held in a buffer
        jr      c,l160a         ; move on if so
        call    l15a5           ; get a new buffer for it
        call    c,l1778         ; if no error, read the sector to the buffer
.l160a  jr      l161d           ; move on

; Subroutine to return page (A) & address (HL) for absolute logical sector DE
; creating a new buffer if necessary (but not reading sector from disk)

.l160c  bit     3,(ix+$1b)
        jp      nz,l1ab6        ; move on if RAMdisk
        push    de
        push    bc
        ld      b,d
        ld      c,e             ; BC=abs logical sector
        call    l1582           ; check if sector held in a buffer
        call    nc,l15a5        ; get a new buffer if not
.l161d  push    af
        call    c,l17b2         ; if no error, move BCB to top of in-use chain
        pop     af
        jr      l1643           ; go to return buffer address as AHL

; Subroutine to get page (A) and address (HL) of buffer for files
; current logical sector, & flag as changed

.l1624  bit     3,(ix+$1b)
        jp      nz,l1ab6        ; move on if RAMdisk
        push    de
        push    bc
        push    bc
        ld      b,d
        ld      c,e
        call    l1582           ; find buffer holding logical sector
        pop     bc
        jr      nc,l1643        ; move on if error
        push    hl
        ld      hl,$0002
        add     hl,de
        set     0,(hl)          ; flag "data changed"
        inc     hl
        ld      (hl),c
        inc     hl
        ld      (hl),b          ; store FCB address of file using buffer
        pop     hl
        scf                     ; successful so far
.l1643  jr      nc,l1650        ; move on if error
        ld      hl,$0008
        add     hl,de
        ld      e,(hl)
        inc     hl
        ld      d,(hl)
        inc     hl
        ld      a,(hl)          ; A=buffer bank
        ex      de,hl           ; HL=buffer address
        scf                     ; success
.l1650  pop     bc
        pop     de
        ret     

; Subroutine to free all buffers referencing unchanged sectors on drive
; Enter at l166c, or l1653 to free only buffers referencing directory sectors

.l1653  call    l18ab           ; has drive been recently accessed?
        ret     c               ; exit if so
        push    hl
        push    de
        push    bc
        ld      e,(ix+$07)
        ld      d,(ix+$08)
        inc     de              ; DE=#directory entries
        ld      a,$02
        call    l04d9           ; DE=#directory records (32 bytes per entry)
        call    l19c0
        dec     de              ; DE=last absolute directory sector
        jr      l1672
.l166c  push    hl
        push    de
        push    bc
        ld      de,$ffff        ; max abs sector number
.l1672  ld      b,d
        ld      c,e             ; BC=absolute sector number
        ld      de,bcb_inuse    ; DE=last inuse buffer address
.l1677  ex      de,hl
.l1678  call    l179b           ; get next inuse buffer
        jr      z,l16e8         ; move on if no more in-use buffers
        push    hl
        ld      hl,$0005
        add     hl,de
        ld      a,(hl)
        cp      (ix+$1c)
        jr      nz,l16a4        ; move on if buffer isn't on same drive
        inc     hl
        ld      a,c
        sub     (hl)
        inc     hl
        ld      a,b
        sbc     a,(hl)
        jr      c,l16a4         ; move on if larger sector than required
        call    l15db           ; get sector for BCB into memory
        dec     hl
        dec     hl
        dec     hl
        dec     hl
        dec     hl
        bit     0,(hl)
        jr      nz,l16a4        ; move on if buffer contains changed data
        pop     hl
        push    hl
        call    l17aa           ; else move buffer to free chain
        pop     hl
        jr      l1678           ; back for more
.l16a4  pop     hl
        jr      l1677           ; back for more

; Subroutine to move all in-use BCBs for current drive to the free list

.l16a7  push    hl
        push    de
        push    bc
        ld      de,bcb_inuse    ; last in-use BCB address
.l16ad  ex      de,hl
.l16ae  call    l179b           ; get next BCB
        jr      z,l16e8         ; exit if no more
        push    hl
        ld      hl,$0005
        add     hl,de
        ld      a,(hl)
        pop     hl
        cp      (ix+$1c)
        jr      nz,l16ad        ; move on if buffer not for current drive
        push    hl
        call    l17aa           ; move BCB from in-use chain to free chain
        pop     hl
        jr      l16ae           ; loop back for more

; Subroutine to move all in-use BCBs for current file to the free list

.l16c6  push    hl
        push    de
        push    bc
        ld      de,bcb_inuse    ; last in-use BCB address
.l16cc  ex      de,hl
.l16cd  call    l179b           ; get next BCB
        jr      z,l16e8         ; exit if no more
        push    hl
        ld      hl,$0003
        add     hl,de
        ld      a,(hl)
        inc     hl
        ld      h,(hl)
        ld      l,a             ; HL=FCB of file for current buffer
        or      a
        sbc     hl,bc
        pop     hl
        jr      nz,l16cc        ; loop back if not current file
        push    hl
        call    l17aa           ; move BCB from in-use chain to free chain
        pop     hl
        jr      l16cd           ; loop back
.l16e8  pop     bc
        pop     de
        pop     hl
        ret

; Subroutine to release in-use cache buffer (address in DE)

.l16ec  push    ix
        push    hl
        ld      hl,$0002
        add     hl,de
        bit     0,(hl)          ; test flags
        scf     
        jr      z,l1702         ; exit if no changed data in buffer
        inc     hl
        inc     hl
        inc     hl
        ld      a,(hl)          ; A=drive letter for buffer
        call    l184d           ; get IX=XDPB
        call    l1719           ; write data back for all changed buffers
.l1702  pop     hl              ;  on this disk
        pop     ix
        ret     

; Subroutine to clear current cache

.l1706  push    hl
        push    de
        ld      de,bcb_inuse    ; last in-use BCB address
.l170b  ex      de,hl
        call    l179b           ; get address of previous in-use BCB
        jr      z,l1716         ; exit if none
        call    l16ec           ; release cache buffer
        jr      c,l170b         ; loop back if no error
.l1716  pop     de
        pop     hl
        ret     

; Subroutine to write data back to disk
; On entry, DE=BCB and IX=XDPB
; The routine writes all changed sectors for the disk back in order
; (smallest absolute logical sector first)

.l1719  push    hl
        push    de
        push    bc
.l171c  ld      de,bcb_inuse
        ld      bc,$ffff        ; max absolute logical sector number
        call    l173a           ; get a smaller one
        jr      z,l1736         ; exit if none left
.l1727  push    de
        call    l173a           ; get a smaller one
        jr      z,l1730         ; move on if smallest found
        pop     af
        jr      l1727           ; loop back
.l1730  pop     de
        call    l1762           ; write buffer to disk
        jr      c,l171c         ; loop back if no error
.l1736  pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to find BCB on same drive with smaller logical sector number
; than BC. On entry DE contains address of current BCB.
; On exit, Z set if no BCB with smaller logical sector number found.

.l173a  ex      de,hl
        call    l179b           ; get address of previous BCB
        ret     z               ; exit if none with Z set
        ld      hl,$0002
        add     hl,de
        bit     0,(hl)
        jr      z,l173a         ; loop back if no data to write
        inc     hl
        inc     hl
        inc     hl
        ld      a,(hl)          ; get drive letter for this buffer
        cp      (ix+$1c)        ; same as buffer we're freeing?
        jr      nz,l173a        ; back if not
        inc     hl
        ld      a,(hl)
        inc     hl
        ld      h,(hl)
        ld      l,a             ; HL=absolute logical sector
        or      a
        sbc     hl,bc
        add     hl,bc
        jr      z,l175d         ; if same, move on
        jr      nc,l173a        ; if larger, loop back
.l175d  ld      b,h             ; BC=smaller absolute logical sector
        ld      c,l
        scf     
        sbc     a,a             ; exit with Z reset
        ret     

; Subroutine to write buffer (DE=BCB) to disk

.l1762  push    de
        call    l1785           ; get buffer & sector details
        call    l1a13           ; write the sector
        pop     de
        ret     nc              ; exit if error
        ld      hl,$0002
        add     hl,de
        res     0,(hl)          ; signal "buffer doesnt contain unwritten data"
        inc     hl
        xor     a
        ld      (hl),a
        inc     hl
        ld      (hl),a          ; set null FCB using this buffer
        scf                     ; success
        ret

; Subroutine to read buffer (DE=BCB) from disk

.l1778  push    hl
        push    de
        push    bc
        call    l1785           ; get buffer & sector details
        call    l1a0a           ; read the sector
        pop     bc
        pop     de
        pop     hl
        ret

; Subroutine to get buffer bank (B), address (HL), logical track (D) and
; logical sector (E), given BCB address in DE

.l1785  ld      hl,$000a
        add     hl,de
        ld      b,(hl)          ; B=buffer bank
        dec     hl
        ld      d,(hl)
        dec     hl
        ld      e,(hl)
        push    de              ; stack buffer address
        dec     hl
        ld      d,(hl)
        dec     hl
        ld      e,(hl)          ; DE=absolute logical sector number
        call    l19df           ; get D=logical track, E=logical sector
        pop     hl              ; HL=buffer address
        ret     

; Subroutine to get address of last free/in-use BCB (Z set if none)
; Enter at l179b with HL=bcb_inuse for last in-use BCB

.l1798  ld      hl,bcb_free     ; address of last free BCB
.l179b  ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=BCB address
        dec     hl
        ld      a,d
        or      e               ; set Z if null
        scf                     ; success
        ret

; Subroutine to check if current BCB is last in chain
; Exits with Z set if so

.l17a3  ex      de,hl
        ld      a,(hl)
        inc     hl
        or      (hl)
        dec     hl
        ex      de,hl
        ret     

; Subroutine to move a BCB from the free chain to the in-use chain (l17b2)
; or vice-versa (l17aa)
; If the BCB is already in the chain its being moved to, this has the effect
; of moving it to the top of the chain
; On entry, HL is address of pointer to this BCB and DE holds address of BCB
; On exit, HL is address of start of other chain, and DE is unchanged

.l17aa  call    l17c3           ; remove current BCB from chain
        ld      hl,bcb_free
        jr      l17b8           ; go to insert into free chain
.l17b2  call    l17c3           ; remove current BCB from chain
        ld      hl,bcb_inuse    ; address of in-use chain
.l17b8  ld      a,(hl)          ; store start of chain into current BCB
        ld      (de),a
        inc     hl
        inc     de
        ld      a,(hl)
        ld      (de),a
        dec     de
        ld      (hl),d          ; store address of current BCB at list address
        dec     hl
        ld      (hl),e
        ret

; Subroutine to transfer a word value from DE to HL
; On exit, DE is unchanged, but HL is incremented

.l17c3  ld      a,(de)
        ld      (hl),a
        inc     hl
        inc     de
        ld      a,(de)
        ld      (hl),a
        dec     de
        ret     

        defs    5

; Subroutine to setup extended XDPB info for drives A: and B:

.l17d0  ld      a,'A'
        ld      (unit0),a	; unit 0 is drive A:
        ld      hl,l17f6
        ld      de,xdpb_a+$1b
        ld      bc,$0015
        ldir    		; copy extended XDPB info for A:
        ld      hl,xdpb_a
        ld      (xdpb_ptrs),hl	; set pointer to A:'s XDPB
        ld      hl,l180b
        ld      de,xdpb_b+$1b
        ld      bc,$0015
        ldir			; copy extended XDPB info for B:    
        ld      c,$01		; B: should be unit 1 or disabled
        jp      l1943		; exit via DOS_MAP_B

; The extended XDPB info for drive A:

.l17f6	defb	$04,'A',$00	; flags,drive,unit
	defb	$00,$00,$00,$00	; last access,filesopen
	defw	$0000,$0000	; #free direntries,last used
	defw	chksm_a,alloc_a	; checksum vector,alloc bitmap
        defw	l1988		; login disk
	defw	l197c		; read sector
	defw	l1982		; write sector

; The extended XDPB info for drive B:

.l180b	defb	$04,'B',$01	; flags,drive,unit
	defb	$00,$00,$00,$00	; last access,filesopen
	defw	$0000,$0000	; #free direntries,lastused
	defw	chksm_b,alloc_b	; checksum vector,alloc bitmap
	defw	l1988		; login disk
	defw	l197c		; read sector
	defw	l1982		; write sector

; Subroutine to initialise RAMdisk

.l1820  push    hl
        ld      hl,l1830
        ld      de,xdpb_m+$1b
        ld      bc,$0015
        ldir    		; copy extended XDPB info for M:
        pop     hl
        jp      l1a5d		; setup RAMdisk & exit

; The extended XDPB info for drive M:

.l1830	defb	$08,'M',$ff	; flags,drive,unit
	defb	$00,$00,$00,$00	; last access,filesopen
	defw	$0000,$0000	; #free direntries,lastused
	defw	$0000,alloc_m	; no checksum;alloc bitmap
	defw	l1845		; login disk
	defw	l1845		; read sector
	defw	l1845		; write sector

; Dummy low-level routines for RAMdisk

.l1845  scf			; success
        ret     

; DOS_REF_XDPB

.l1847  call    l04ed           ; ensure drive letter uppercase
        ld      hl,xdpb_ptrs
.l184d  sub     'A'
        jr      c,l186d         ; error if <A
        cp      $10
        jr      nc,l186d        ; error if >P
        push    hl
        add     a,a
        add     a,xdpb_ptrs~$ff
        ld      l,a
        adc     a,xdpb_ptrs/$100
        sub     l
        ld      h,a             ; HL=xdpb_ptrs+2*drive
        ld      a,(hl)
        inc     hl
        ld      h,(hl)          ; get address of XDPB for drive (or 0)
        ld      l,a
        push    hl
        pop     ix              ; IX points to XDPB
        ld      a,h
        or      l
        add     a,$ff           ; set carry unless no XDPB (success)
        pop     hl              ; restore HL
        ld      a,$16           ; error 22 - drive not found if error
        ret     
.l186d  ld      a,$15           ; error 21 - bad parameter
        or      a
        ret

; Subroutine to possibly logout drive & low-level login another disk

.l1871  call    l184d           ; get XDPB for drive
        ret     nc              ; exit if none
        push    hl
        push    de
        push    bc
        call    l188f           ; possibly logout disk
        bit     0,(ix+$1b)
        scf     
        call    z,l1887         ; if disk logged out, try to log another in
        pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to low-level login a disk

.l1887  ld      a,(ix+$1a)      ; get freeze flag
        rla     
        ret     c               ; exit if auto-detect not required
        jp      l19fe           ; do a low-level login & exit

; Subroutine to log out a disk if no files open & not recently accessed

.l188f  bit     0,(ix+$1b)
        ret     z               ; exit if drive not logged in
        ld      a,(ix+$21)
        or      a
        ret     nz              ; exit if files open on drive
        call    l18ab
        ret     c               ; exit if accessed within last 2secs, or fixed

; Subroutine to low-level log out a disk (IX=XDPB)

.l189d  ld      a,(ix+$21)      ; # files open on drive
        or      a
        ld      a,$24           ; drive in use error if so
        ret     nz
        res     0,(ix+$1b)      ; flag drive logged out
        jp      l16a7           ; move all BCBs for this drive to free list

; Subroutine to see if drive has been accessed within last 2secs
; If so (or if fixed drive), carry is set

.l18ab  bit     7,(ix+$0c)
        scf     
        ret     nz              ; exit if drive is fixed
        push    hl
        push    de
        push    bc
        ld      a,r             ; get interrupt status
        di      
        ld      a,(FRAMES)      ; get HLA=FRAMES
        ld      hl,(FRAMES+1)
        jp      po,l18c1
        ei                      ; re-enable interrupts if necessary
.l18c1  ld      b,a             ; HLB=current FRAMES
        ld      a,(ix+$1e)
        ld      e,(ix+$1f)
        ld      d,(ix+$20)      ; DEA=last access FRAMES
        add     a,$64
        jr      nc,l18d0
        inc     de              ; DEA=last access FRAMES + 2secs
.l18d0  ld      c,a
        ld      a,b
        sub     c
        sbc     hl,de
        push    af              ; save carry (set if accessed within last 2s)
        ld      hl,FRAMES
        ld      a,r             ; get interrupt status
        di      
        ld      a,(hl)
        ld      (ix+$1e),a      ; update last access time to current FRAMES
        inc     hl
        ld      a,(hl)
        ld      (ix+$1f),a
        inc     hl
        ld      a,(hl)
        ld      (ix+$20),a
        jp      po,l18ee
        ei                      ; re-enable interrupts if necessary
.l18ee  pop     af
        pop     bc
        pop     de
        pop     hl
        ret                     ; done

; Subroutine to call l18fd, preserving BC/DE/HL

.l18f3  push    hl
        push    de
        push    bc
        call    l18fd
        pop     bc
        pop     de
        pop     hl
        ret     

; Subroutine to check disk can be written to

.l18fd  bit     2,(ix+$1b)
        scf     
        ret     z		; exit with success if not floppy drive
        call    l1918		; change disk if necessary
        call    l1e65		; DD_TEST_UNSUITABLE
        ret     nc		; exit if error
        call    l1ee9		; DD_DRIVE_STATUS
        ld      c,a
        and     $20
        ret     z		; exit if not present
        bit     6,c
        ld      a,$01
        ret     nz		; error if disk write-protected
        scf     		; success
        ret     

; Subroutine to check whether "change disk" routine should be called

.l1918  push    hl
        ld      c,(ix+$1d)
        ld      a,c
        or      a
        jr      nz,l1935	; exit if drive not mapped to unit 0
        ld      hl,unit0
        ld      a,(ix+$1c)
        cp      (hl)
        jr      z,l1935         ; exit if unit 0 is currently mapped to drive
        ld      (hl),a		; change unit 0 mapping to this drive
        push    ix
        push    de
        push    bc
        call    l1937		; ask user to change disk
        pop     bc
        pop     de
        pop     ix
.l1935  pop     hl
        ret     

; Change disk subroutine

.l1937  push    af
        ld      c,a
        call    l0338		; generate change disk message
        pop     af
        push    hl
        ld      hl,(rt_chgdsk)
        ex      (sp),hl		; stack change disk routine address
        ret     		; exit to routine

; DOS_MAP_B

.l1943  ld      a,'B'
        call    l184d           ; get XDPB of drive B:
        ccf     
        call    nc,l189d        ; logout drive at low-level
        ret     nc              ; exit if error
        ld      a,c
        or      a
        jr      z,l1954
        ld      hl,$0000        ; no change-disk routine if B: is unit 1
.l1954  ld      de,(rt_chgdsk)  ; get old routine address
        ld      (rt_chgdsk),hl  ; store new routine address
        ld      hl,$0000
        ld      (xdpb_ptrs+2),hl ; clear XDPB pointer for B:
        ld      ix,xdpb_b       ; IX=XDPB for B:
        ld      (ix+$1d),c      ; set unit number
        call    l1f27           ; DD_INTERFACE
        jr      nc,l1979        ; move on if no interface
        ld      a,c
        or      a
        scf     
        call    nz,l1edd        ; for unit 1 only, test if present
        jr      nc,l1979        ; if not, no drive B:
        ld      (xdpb_ptrs+2),ix ; set pointer to B:'s XDPB
.l1979  scf
        ex      de,hl
        ret                     ; exit with HL=old change-disk routine

; Low-level read sector subroutine for drives A: & B:

.l197c  call    l1918		; check if disk change required
        jp      l1bff		; DD_READ_SECTOR

; Low-level write sector subroutine for drives A: & B:

.l1982  call    l1918		; check if disk change required
        jp      l1c0d		; DD_WRITE_SECTOR

; Low-level login disk subroutine for drives A: & B:

.l1988  call    l1918		; check if disk change required
        call    l1c80		; DD_LOGIN
        ret     nc		; exit if error
        ld      a,(ix+$0f)
        xor     $02
        ld      a,$06		; "unrecognised disk format"
        ret     nz		; error if sectorsize <> 512
        rr      d
        rr      e
        ld      hl,$ffd2
        add     hl,de
        ccf     
        ret     nc		; error if alloc vector size/2 >$2d
        ld      e,(ix+$0b)
        ld      a,(ix+$0c)
        and     $7f
        ld      d,a
        ld      hl,$ffbf
        add     hl,de
        ccf     
        ret     c		; success if chksum size <= $40
        ld      (ix+$0b),$40
        ld      a,(ix+$0c)
        and     $80
        or      $00
        ld      (ix+$0c),a	; else set chksum size to $40
        scf     
        ret

; Subroutine to convert logical usable record number (in DE) to
; absolute logical sector number by allowing for reserved tracks

.l19c0  push    hl
        push    bc
        ld      c,(ix+$0d)
        ld      b,(ix+$0e)      ; BC=# reserved tracks
        ex      de,hl
        ld      e,(ix+$00)
        ld      d,(ix+$01)      ; DE=# records per track
        jr      l19d3
.l19d1  add     hl,de
        dec     bc
.l19d3  ld      a,b
        or      c
        jr      nz,l19d1
        ex      de,hl           ; DE=origHL+#reserved records
        pop     bc
        pop     hl
        ld      a,$02
        jp      l04d9           ; DE=absolute logical sector

; Subroutine to find logical track (D) & sector (E) from absolute logical
; sector number (DE). IX=XDPB for disk

.l19df  push    hl
        push    bc
        ex      de,hl
        add     hl,hl
        add     hl,hl           ; HL=absolute logical record number
        ld      e,(ix+$00)
        ld      d,(ix+$01)      ; DE=records per track
        ld      bc,$ffff        ; BC=logical track counter
        or      a
.l19ee  inc     bc
        sbc     hl,de
        jr      nc,l19ee        ; loop until found correct track in BC
        add     hl,de
        ex      de,hl           ; DE=logical record number
        ld      a,$02
        call    l04d9           ; DE=logical sector number on track
        ld      d,c             ; D=logical track, E=logical sector
        pop     bc
        pop     hl
        ret

; Subroutine to login a disk at low-level

.l19fe  push    hl
        ld      l,(ix+$2a)
        ld      h,(ix+$2b)      ; HL=routine address to login a disk
        ld      de,$0000        ; track 0, sector 0
        jr      l1a1a           ; move on to perform operation

; Subroutine to read a sector at BHL on logical track D, logical sector E

.l1a0a  push    hl
        ld      l,(ix+$2c)
        ld      h,(ix+$2d)      ; HL=routine address to read a sector
        jr      l1a1a           ; move on to perform operation

; Subroutine to write a sector at BHL on logical track D, logical sector E

.l1a13  push    hl
        ld      l,(ix+$2e)
        ld      h,(ix+$2f)      ; HL=routine address to write a sector
.l1a1a  ld      (rt_temp),hl    ; store ready to call
        pop     hl
.l1a1e  push    hl
        push    de
        push    bc
        call    l1a2e           ; call routine to write a sector
        pop     bc
        pop     de
        pop     hl
        ret     c               ; exit if no error
        call    l1a34           ; retry message
        jr      z,l1a1e         ; loop back to retry
        ret                     ; exit

; Subroutine to call address at rt_temp

.l1a2e  push    hl              ; save HL
        ld      hl,(rt_temp)    ; stack routine address
        ex      (sp),hl         ; stack address & restore HL
        ret                     ; return to call routine

; Subroutine to do an ALERT message for error A (IX=XDPB)

.l1a34  push    ix
        push    hl
        push    de
        push    bc
        call    l2164           ; turn off motor
        ld      c,(ix+$1c)      ; C=drive
        call    l02f7           ; run the ALERT routine
        pop     bc
        pop     de
        pop     hl
        pop     ix
        ret

; Subroutine to get H=first RAMdisk buffer, L=number of RAMdisk buffers

.l1a48  ld      a,(spec_m+5)
        ld      h,a             ; H=first buffer
        ld      a,(spec_m+2)    ; A=last buffer+1
        sub     h
        ld      l,a             ; L=# buffers
        ret

; Subroutine to change the RAMdisk to first buffer=H, number of buffers=L

.l1a52  ld      a,'M'
        call    l184d           ; get XDPB of drive M:
        jr      nc,l1a5d        ; move on if doesn't exist
        call    l189d           ; log drive M: out
        ret     nc              ; exit if error
.l1a5d  push    hl
        ld      hl,l1aae
        ld      de,spec_m
        ld      bc,$0008
        ldir                    ; copy initial disk spec for M:
        pop     de
        ld      hl,$0000
        ld      (xdpb_ptrs+$18),hl ; set M:s XDPB to null
        ld      a,e
        cp      $04
        ret     c               ; exit with M: disabled if <2K allowed
        add     a,d
        ld      (spec_m+2),a    ; #tracks=size+offset (=last buffer+1)
        ld      a,d
        ld      (spec_m+5),a    ; #reserved tracks (=first buffer)
.l1a7c  ld      a,d
        push    de
        call    l022b           ; get address of buffer
        call    l0207           ; page in the bank
        ld      d,h
        ld      e,l
        inc     de
        ld      (hl),$e5
        ld      bc,$01ff
        ldir                    ; fill buffer with $e5 filler bytes
        call    l0207           ; repage previous bank
        pop     de
        inc     d               ; next buffer
        dec     e
        jr      nz,l1a7c        ; back for rest
        ld      ix,xdpb_m
        ld      hl,spec_m       ; disk spec for M:
        call    l1d30           ; initialise DPB
        ld      (ix+$0b),$00
        ld      (ix+$0c),$80    ; M: is permanent (fixed)
        ld      (xdpb_ptrs+$18),ix ; set pointer to XDPB for M:
        scf                     ; success
        ret     

; Disk spec for RAMdisk

.l1aae  defb    $00,$00,$00,$01
        defb    $02,$00,$03,$00

; Subroutine to get page (A) and address (HL) of RAMdisk logical sector
; for current file

.l1ab6  push    de
        call    l19df           ; get logical track & sector
        ld      a,e
        or      a
        jr      nz,l1ac9        ; RAMdisk has 1sec/trk, so error if not 0
        ld      a,d
        ld      hl,spec_m+2
        cp      (hl)
        jr      nc,l1ac9        ; error if track out of range
        call    l022b           ; convert track=buffer# to address & bank
        scf     
.l1ac9  pop     de
        ret     c
        ld      a,$02           ; seek fail error
        ret     

; DOS_BOOT

.l1ace  ld      a,'A'
        call    l1871           ; login a disk
        call    c,l189d         ; log back out if no error
        ld      de,$0000        ; logical sector 0
        call    c,l15f4         ; get sector to memory at AHL if no error
        ret     nc              ; exit if error
        ld      c,a             ; save bank
        push    hl
        call    l0207           ; page bank in
        push    af              ; save old bank
        xor     a
        ld      b,a
        ld      e,$02
.l1ae7  add     a,(hl)          ; form checksum of all bytes in sector
        inc     hl
        djnz    l1ae7
        dec     e
        jr      nz,l1ae7
        ld      e,a
        pop     af
        call    l0207           ; page back original bank
        pop     hl
        ld      a,e
        xor     $03             ; checksum must be 3
        ld      a,$23
        ret     nz              ; if not, exit with "disk not bootable" error
        di                      ; disable interrupts
        ld      b,$03
        ld      de,$fe00
        ld      ix,$0200
        call    l023d           ; copy bootsector to $fe00 in bank 3
        ld      a,$03
        call    l0207           ; page in bank 3
        ld      hl,l1b22
        ld      de,$fdfb
        ld      bc,$0005
        ldir                    ; copy routine to bank 3
        ld      bc,$1ffd
        ld      a,$07
        ld      sp,$fe00
        jp      $fdfb           ; jump to following routine in bank 3
.l1b22  out     (c),a           ; page in RAM 4,7,6,3; keep disk motor on
        jp      $fe10           ; jump to boot routine

        defs    9


; ********************** LOW-LEVEL ROUTINES *********************


; Subroutine to setup parameter block for format

.l1b30  ld      a,(ix+$19)
        and     $40             ; A=modulation mode
        or      $0d             ; format command
        call    l1b9c           ; set up parameter block as for read/write etc
        ld      l,(ix+$0f)
        ld      h,(ix+$13)
        ld      (ddl_parms+8),hl ; store sector size & #sectors per track
        ld      h,e
        ld      l,(ix+$18)
        ld      (ddl_parms+$0a),hl ; store format gap length & filler byte
        ld      a,$06
        ld      (ddl_parms+5),a ; store # of command bytes
        ret     

; Setup parameter block for sector check

.l1b50  ld      a,(ix+$19)      ; flags
        or      $11             ; scan equal command
        call    l1b69           ; set up parameter block
        ld      (hl),$01        ; store data length 1
        ret     

; Setup parameter block for sector read

.l1b5b  ld      a,(ix+$19)      ; flags
        or      $06             ; read data command
        jr      l1b69           ; set up parameter block

; Setup parameter block for sector read

.l1b62  ld      a,(ix+$19)      ; flags
        and     $c0             ; mask off deleted address mark bit
        or      $05             ; write data command

; Subroutine to setup parameter block for low-level read/writes

.l1b69  call    l1b9c           ; setup basic parameter block data
        ld      a,e
        add     a,(ix+$14)
        ld      e,a             ; E=physical sector number
        push    de              ; save physical track & sector numbers
        ld      hl,(rt_encode)
        ld      a,h
        or      l
        call    nz,l1ef2        ; call encode routine if required
        ld      a,e
        ld      (ddl_parms+$0a),a ; store 1st sector ID
        ld      l,(ix+$0f)
        ld      h,e
        ld      (ddl_parms+$0b),hl ; store sector size & last(=1st) sector ID
        ld      a,(ix+$17)
        ld      (ddl_parms+$0d),a ; store gap length
        ld      h,b
        ld      l,d
        ld      (ddl_parms+8),hl ; store track & side numbers
        ld      a,$09
        ld      (ddl_parms+5),a       ; store # command bytes
        ld      hl,ddl_parms+$0e
        ld      (hl),$ff        ; store dummy data length
        pop     de
        ret     

; Subroutine to setup some of the parameter block for sector read/writes
; (except # command bytes & additional command bytes)

.l1b9c  ld      (ddl_parms+1),hl ; store buffer address
        ld      l,a
        ld      a,b
        ld      (ddl_parms),a   ; store buffer page
        call    l1bb5           ; C=physical side & unit byte
        ld      h,c
        ld      (ddl_parms+6),hl ; store command & unit byte
        ld      l,(ix+$15)
        ld      h,(ix+$16)
        ld      (ddl_parms+3),hl ; store sector size as # bytes to transfer
        ret     

; Subroutine to return physical side (B) and track (D) given logical track (D)
; Physical side is also ORed with unit number in C

.l1bb5  ld      a,(ix+$11)
        and     $7f             ; A=sidedness
        ld      b,$00           ; side 0
        ret     z               ; exit if single-sided (physical=logical)
        dec     a
        jr      nz,l1bc8        ; move on if double-sided: successive sides
        ld      a,d
        rra                     ; for alternate sides, halve track
        ld      d,a
        ld      a,b
        rla                     ; with side=remainder
        ld      b,a
        jr      l1bd4           ; move on to OR into unit number
.l1bc8  ld      a,d
        sub     (ix+$12)        ; subtract # tracks
        jr      c,l1bd4         ; if < # tracks, physical=logical so move on
        sub     (ix+$12)        ; on successive side, tracks count back down
        cpl     
        ld      d,a
        inc     b               ; and use side 1
.l1bd4  ld      a,b
        add     a,a
        add     a,a
        or      c
        ld      c,a             ; update unit number with side bit
        ret     

; DD_ENCODE

.l1bda  or      a
        jr      nz,l1be0        ; move on if routine supplied
        ld      hl,$0000        ; disable
.l1be0  ld      de,(rt_encode)  ; get old encode routine address
        ld      (rt_encode),hl  ; store new encode routine address
        ex      de,hl           ; HL=old routine
        ret     

; Subroutine to read A bytes from a sector

.l1be9  push    af
        call    l1b5b           ; setup parameter block for sector read
        pop     af
        ld      l,a
        ld      h,$00
        ld      (ddl_parms+3),hl ; store #bytes required from sector
        ld      hl,l1bf9        ; the operation to try
        jr      l1c4f           ; go to try it
.l1bf9  ld      hl,ddl_parms    ; address of parameter block
        jp      l20ba           ; read bytes and exit

; DD_READ_SECTOR

.l1bff  call    l1b5b           ; set up parameter block
        ld      hl,l1c07        ; the operation to try
        jr      l1c4f           ; go to try it
.l1c07  ld      hl,ddl_parms    ; address of parameter block
        jp      l20c3           ; do a DD_L_READ

; DD_WRITE_SECTOR

.l1c0d  call    l1e65           ; test if suitable XDPB
        ret     nc              ; exit if error
        call    l1b62           ; set up parameter block
        jr      l1c2b           ; move on

; DD_CHECK_SECTOR

.l1c16  call    l1b50           ; set up parameter block
        call    l1c2b           ; scan the data
        ret     nc              ; exit if error
        ld      a,(fdc_res+3)   ; get ST2
        cp      $08             ; set Z if scan was equal
        scf                     ; success
        ret     

; DD_FORMAT

.l1c24  call    l1e65           ; test if suitable XDPB
        ret     nc              ; exit if not
        call    l1b30           ; setup parameter block
.l1c2b  ld      hl,l1c30        ; the operation to try
        jr      l1c4f           ; go to try it
.l1c30  ld      hl,ddl_parms    ; address of parameter block
        jp      l20cc           ; do a DD_L_WRITE

; DD_READ_ID

.l1c36  call    l1c41           ; read a sector ID
        ld      hl,fdc_res      ; get results buffer address
        ret     nc              ; exit if error
        ld      a,(fdc_res+6)   ; get sector number
        ret     

; Subroutine to read a sector ID

.l1c41  call    l1bb5           ; convert logical track to physical
        ld      hl,l1c49        ; the operation to try
        jr      l1c4f           ; go to do it
.l1c49  ld      a,(ix+$19)      ; get modulation mode
        jp      l2103           ; read sector ID & exit

; Routine to turn on motor, try an operation in HL on track D multiple
; times & then start the motor off timeout

.l1c4f  call    l212b           ; turn on motor
        call    l1e80           ; try the operation multiple times
        jp      l2150           ; start motor off timeout & exit


; Tables of specifications for disk types 0-3
; Format as on p215 of +3 Manual

; Type 0 - Spectrum +3 format

.l1c58  defb    $00,$00,$28,$09
        defb    $02,$01,$03,$02
        defb    $2a,$52

; Type 1 - CPC system format

        defb    $01,$00,$28,$09
        defb    $02,$02,$03,$02
        defb    $2a,$52

; Type 2 - CPC data format

        defb    $02,$00,$28,$09
        defb    $02,$00,$03,$02
        defb    $2a,$52

; Type 3 - PCW format

        defb    $03,$81,$50,$09
        defb    $02,$01,$04,$04
        defb    $2a,$52

; DD_LOGIN

.l1c80  xor     a
        call    l1cdb           ; get IX=XDPB of +3 format disk
        ld      d,$00
        push    bc
        call    c,l1c36         ; if no error, read a sector ID on track 0
        pop     bc
        ret     nc              ; exit if error
        and     $c0
        ld      e,$01           ; CPC system format if IDs ~$40
        cp      $40
        jr      z,l1c99
        inc     e               ; CPC data format if IDs ~$c0
        cp      $c0
        jr      nz,l1c9f        ; otherwise, +3 or PCW format
.l1c99  ld      a,e
        call    l1cdb           ; get IX=XDPB of CPC format disks
        jr      l1cd3           ; move on
.l1c9f  push    bc
        ld      hl,ddl_parms+$0f ; buffer for disk spec
        ld      de,$0000        ; track 0, sector 0
        ld      b,$07           ; use page 7
        ld      a,$0a           ; 10 bytes required (disk spec)
        push    hl
        call    l1be9           ; read disk spec from boot sector
        pop     hl
        pop     bc
        jr      c,l1cba         ; move on if no error
        cp      $08
        scf     
        ccf     
        ret     nz              ; exit with errors except disk changed
        ld      a,$06           ; substitute "unrecognised disk format"
        ret     
.l1cba  push    bc
        ld      d,h
        ld      e,l
        ld      c,(hl)
        ld      b,$0a           ; check 10 bytes of specifier
.l1cc0  ld      a,(de)
        inc     de
        cp      c
        jr      nz,l1cca        ; if any differ, use this as disk spec
        djnz    l1cc0
        ld      hl,l1c58        ; use +3 format if all bytes the same
.l1cca  pop     bc
        ld      a,(hl)
        cp      $04             ; test format type
        ld      a,$06           ; signal unrecognised disk format
        call    c,l1cee         ; initialise XDPB if no error
.l1cd3  push    hl
        push    bc
        call    c,l1dee         ; if no errors, update drive equipment & 
        pop     bc              ; check suitable for disk
        pop     hl
        ret     

; DD_SEL_FORMAT

.l1cdb  ld      e,a
        cp      $04             ; test format type
        ld      a,$06
        ret     nc              ; error 6 if not type 0-3
        ld      a,e
        add     a,a
        ld      e,a             ; E=2*type
        add     a,a
        add     a,a
        add     a,e             ; A=10*type
        adc     a,l1c58~$ff
        ld      l,a             
        adc     a,l1c58/$100
        sub     l
        ld      h,a             ; HL=address of info for type

; DD_L_XDPB
; Enter at this point to initialise an XDPB (HL=disk spec)

.l1cee  push    hl              ; save info address
        push    bc
        ld      a,(hl)          ; get format type
        ld      b,$41           ; first sector number for CPC system
        dec     a
        jr      z,l1cfd
        ld      b,$c1           ; first sector number for CPC data
        dec     a
        jr      z,l1cfd
        ld      b,$01           ; first sector number for +3/PCW
.l1cfd  ld      (ix+$14),b      ; set first sector number
        inc     hl
        ld      a,(hl)
        ld      (ix+$11),a      ; set sidedness & single/double track
        inc     hl
        ld      a,(hl)
        ld      (ix+$12),a      ; set # tracks
        inc     hl
        ld      a,(hl)
        ld      (ix+$13),a      ; set # sectors
        inc     hl
        ld      b,(hl)          ; B=sector size (0..2)
        inc     hl
        inc     hl
        inc     hl
        inc     hl
        ld      a,(hl)
        ld      (ix+$17),a      ; set gap length (r/w)
        inc     hl
        ld      a,(hl)
        ld      (ix+$18),a      ; set gap length (format)
        ld      hl,$0080
        call    l1efd           ; HL=128x2^B
        ld      (ix+$15),l
        ld      (ix+$16),h      ; set sector size (bytes)
        ld      (ix+$19),$60    ; set multitrack,modulation flags
        pop     bc              ; restore registers
        pop     hl

; DD_L_DPB
; Enter at this point to initialise a DPB (HL=disk spec)
; On exit, DE=ALS, HL=HASH and A=format type

.l1d30  push    bc              ; save registers
        push    hl
        ex      de,hl
        ld      hl,$0004
        add     hl,de
        ld      a,(hl)
        ld      (ix+$0f),a      ; set PSH=log2(sectorsize/128)
        push    af
        call    l1ef3
        ld      (ix+$10),a      ; set PHM=(sectorsize/128)-1
        dec     hl
        ld      l,(hl)
        ld      h,$00
        pop     bc
        call    l1efd           ; HL=(#sectors)x(records per sector)
        ld      (ix+$00),l
        ld      (ix+$01),h      ; set SPT (records per track)
        ld      hl,$0006
        add     hl,de
        ld      a,(hl)
        ld      (ix+$02),a      ; set BSH=log2(blocksize/128)
        ld      c,a             ; save it
        push    hl
        call    l1ef3
        ld      (ix+$03),a      ; set BLM=(blocksize/128)-1
        dec     hl
        ld      e,(hl)
        ld      (ix+$0d),e      ; set OFF (#reserved tracks)
        ld      (ix+$0e),$00
        dec     hl
        dec     hl
        ld      b,(hl)          ; B=#sectors
        dec     hl
        ld      d,(hl)          ; D=#tracks
        dec     hl
        ld      a,(hl)          ; A=sidedness
        ld      l,d
        ld      h,$00           ; HL=tracks per side
        ld      d,h             ; DE=reserved tracks
        and     $7f
        jr      z,l1d79
        add     hl,hl           ; HL=total tracks on disk
.l1d79  sbc     hl,de
        ex      de,hl           ; DE=total tracks-reserved tracks
        ld      hl,$0000
.l1d7f  add     hl,de
        djnz    l1d7f           ; HL=total non-reserved sectors
        ld      a,c
        sub     (ix+$0f)
        ld      b,a
        call    l1f04           ; HL=total non-reserved blocks
        dec     hl
        ld      (ix+$05),l
        ld      (ix+$06),h      ; set DSM (last block number)
        ld      b,$03
        ld      a,h
        or      a
        jr      z,l1d98
        inc     b               ; B=3, or 4 if >256 blocks
.l1d98  ld      a,c             ; A=log2(blocksize/128)
        sub     b               ; A=log2(bs/2^7)-log2(2^B)=log2(bs/2^(B+7))
        call    l1ef3           ; A=(bs/2^(B+7))-1
        ld      (ix+$04),a      ; set EXM (extent mask)
        pop     de
        push    hl              ; save DSM
        ld      b,$02
        call    l1f04
        inc     hl
        inc     hl              ; HL=Floor(DSM/4)+2=ALS (allocation vector size)
        ex      (sp),hl         ; save ALS and retrieve DSM
        inc     de
        ld      a,(de)          ; A=#dir blocks
        or      a
        jr      nz,l1db7        ; move on unless need to calculate it
        add     hl,hl
        ld      a,h
        inc     a
        cp      $02
        jr      nc,l1db7
        inc     a
.l1db7  ld      b,a
        ld      hl,$0000
.l1dbb  scf
        rr      h
        rr      l
        djnz    l1dbb           ; HL=2^16-2^(16-#dir blocks)
        ld      (ix+$09),h      ; set AL0 directory bitmap
        ld      (ix+$0a),l      ; set AL1 directory bitmap
        ld      h,$00
        ld      l,a             ; HL=#dirblocks
        ld      b,c             ; B=log2(blocksize/128)
        inc     b
        inc     b               ; B=log2(bs/128)+log2(4)=log2(bs/32)
        call    l1efd
        push    hl              
        dec     hl              ; HL=(#dirblocks*bs/32)-1=DRM
        ld      (ix+$07),l
        ld      (ix+$08),h      ; set DRM (last directory entry number)
        ld      b,$02
        call    l1f04
        inc     hl              ; HL=((DRM+1)/4)+1=CKS
        ld      (ix+$0b),l
        ld      (ix+$0c),h      ; set CKS (checksum vector size)
        pop     hl
        add     hl,hl
        add     hl,hl           ; HL=4*(DRM+1) (hash table size)
        pop     de              ; DE=ALS (2bit allocation vector size)
        pop     bc
        ld      a,(bc)          ; A=format type
        scf                     ; signal success
        pop     bc              ; restore BC
        ret     

; Subroutine to update drive equipment info & check is suitable for disk

.l1dee  ld      b,a
        push    de
        push    bc
        call    l1e10           ; test drive equipment, updating if necessary
        pop     bc
        pop     de
        ret     nc              ; exit if error
        ld      a,(ix+$11)
        and     $03
        jr      z,l1e02         ; move on if disk is single-sided
        bit     1,(hl)
        jr      z,l1e0c         ; error 9 if drive single-sided
.l1e02  ld      a,b
        scf     
        bit     7,(ix+$11)
        ret     z               ; exit with success if disk not double-track
        bit     3,(hl)
        ret     nz              ; or if drive double-track
.l1e0c  ld      a,$09           ; error 9 - unsuitable media for drive
        or      a
        ret     

; Subroutine to test drive equipment for sidedness and double/single track

.l1e10  call    l1f6a           ; get equipment address
        ld      a,(hl)
        and     $0c             ; check if single/double track drive
        jr      z,l1e24         ; move on if not known
        ld      a,(hl)
        and     $03             ; check if single/double sided drive
        scf     
        ret     nz              ; exit with success if known
        ld      a,(ix+$11)
        and     $03             ; check # sides for disk
        scf     
        ret     z               ; exit with success if disk single-sided
.l1e24  ld      a,(ix+$11)
        and     $03             ; check # sides for disk
        ld      d,$02           ; track 2
        jr      z,l1e39         ; move on if single-sided
        dec     a
        ld      d,$05           ; track 2 on side 1 if alternating
        jr      z,l1e39         ; move on if alternating sides
        ld      a,(ix+$12)      ; 
        add     a,a
        sub     $03
        ld      d,a             ; track 2 on side 1 if successive sides
.l1e39  push    hl
        call    l1c36           ; read a sector ID
        pop     hl
        ret     nc              ; exit if error
        ld      de,(fdc_res+4)  ; get E=track, D=head detected
        ld      a,(ix+$11)
        and     $03
        jr      z,l1e53         ; move on if single-sided disk
        dec     d               ; D should now be zero
        jr      z,l1e51
        set     0,(hl)          ; if not, set single-sided drive
        jr      l1e53
.l1e51  set     1,(hl)          ; set double-sided drive
.l1e53  ld      a,(ix+$11)
        dec     e
        dec     e
        jr      z,l1e5b         ; move on if track 2 was detected
        cpl                     ; else drive has inverse d-track bit of disk
.l1e5b  rla                     ; get double-track bit
        jr      nc,l1e61
        set     3,(hl)          ; set double-track
        ret     
.l1e61  set     2,(hl)          ; set single-track
        scf     
        ret     

; DD_TEST_UNSUITABLE

.l1e65  push    hl
        call    l1f6a           ; get equipment address
        bit     3,(hl)          ; test if double-track
        pop     hl
        scf     
        ret     z               ; exit with okay if single-track
        ld      a,(ix+$11)
        rla                     ; set carry (okay) if disk double-track
        ld      a,$09           ; error 9 - unsuitable media
        ret     

; DD_EQUIPMENT

.l1e75  call    l1f6a           ; get equipment address
        ld      a,(hl)          ; get flags byte
        and     $0f             ; mask side & track info
        ret

; DD_SET_RETRY

.l1e7c  ld      (retry_cnt),a   ; set it
        ret

; Subroutine to try operation in HL on track D multiple times

.l1e80  ld      a,(retry_cnt)
        ld      b,a             ; B=retry count
.l1e84  push    bc
        call    l1eb0           ; try the operation
        pop     bc
        ret     z               ; exit for success or unrecoverable error
        cp      $04
        jr      nz,l1eac        ; move on if not "no data" error
        push    hl
        push    de
        push    bc
        ld      a,(ix+$19)      ; flags (for modulation mode)
        call    l2103           ; read a sector ID
        call    l204a           ; process results
        pop     bc
        pop     de
        pop     hl
        jr      nz,l1eac        ; move on if recoverable error
        ret     nc              ; exit if non-recoverable error
        ld      a,(fdc_res+6)   ; get sector ID found
        xor     (ix+$14)
        and     $c0             ; compare to XDPB sector ID (detect if
        ld      a,$08           ; different CPC format than expected)
        ret     nz              ; error 8 - disk changed
        rra     
.l1eac  djnz    l1e84           ; back for more tries
        or      a               ; exit with error 4 - no data
        ret     

; Subroutine to position head on track D and call subroutine in HL,
; processing results buffer afterwards
; B contains try number

.l1eb0  ld      a,b
        and     $07
        jr      z,l1ec2         ; reseek to high track every 8 tries
        and     $03
        jr      nz,l1ecc        ; recalibrate every other 8 tries
        push    hl
        call    l1f6a           ; get equipment address
        res     6,(hl)          ; signal head not positioned
        pop     hl
        jr      l1ecc
.l1ec2  push    de
        ld      d,(ix+$12)
        dec     d               ; D=high track number
        call    l1f76           ; seek to it
        pop     de
        ret     nc              ; exit if error
.l1ecc  call    l1f76           ; seek to track D
        ret     nc              ; exit if error
        push    hl
        push    de
        push    bc
        call    l1ef2           ; call routine address in HL
        pop     bc
        pop     de
        call    l204a           ; process results buffer
        pop     hl
        ret     

; DD_ASK_1

.l1edd  push    bc
        ld      c,$01
        call    l1ee9           ; get status of unit 1
        pop     bc
        and     $60
        ret     z               ; exit if not ready or write-protected
        scf                     ; signal "unit 1 present"
        ret     

; DD_DRIVE_STATUS

.l1ee9  call    l212b           ; turn on motor
        call    l2087           ; get status in A
        jp      l2150           ; start motor off timeout & exit

; Call is made here to call the address in HL

.l1ef2  jp      (hl)

; Subroutine to calculate N/128 - 1
; given A=log2 (N/128)

.l1ef3  or      a
        ret     z               ; exit if zero
        ld      b,a
        ld      a,$01
.l1ef8  add     a,a             ; calculate N/128
        djnz    l1ef8
        dec     a               ; decrement
        ret     

; Subroutine to multiply HL by 2^B

.l1efd  ld      a,b
        or      a
        ret     z               ; exit when B=0
.l1f00  add     hl,hl           ; double HL
        djnz    l1f00           ; loop back
        ret

; Subroutine to divide HL by 2^B

.l1f04  ld      a,b
        or      a
        ret     z               ; exit when B=0
.l1f07  srl     h
        rr      l               ; halve HL
        djnz    l1f07           ; loop back
        ret

        defs    18

; Default setup data, used by DD_INIT

.l1f20  defb    $0a             ; motor on time
        defb    $32             ; motor off time
        defb    $af             ; write off time
        defb    $1e             ; head settle time
        defb    $0c             ; step rate
        defb    $0f             ; head unload time
        defb    $03             ; head load time x2+1

; DD_INTERFACE

.l1f27  push    bc
        ld      bc,$2ffd
        in      a,(c)           ; read FD status register ($ff if no i/f)
        add     a,$01
        ccf                     ; carry not set if no i/f
        pop     bc
        ret

; DD_INIT

.l1f32  ld      hl,equipment    ; zero equipment data, timing consts and
        ld      b,$10           ; FDC results buffer
.l1f37  ld      (hl),$00
        inc     hl
        djnz    l1f37
        ld      a,$0f
        ld      (retry_cnt),a   ; set retry count to $0f
        call    l2164           ; turn off motor
        ld      hl,l1f20        ; enter DD_SETUP with default setup data

; DD_SETUP

.l1f47  ld      de,tm_mtron
        ld      bc,$0005
        ldir                    ; copy first 5 bytes of setup info
        ld      a,(tm_step)
        dec     a
        rlca    
        rlca    
        rlca    
        cpl     
        and     $f0
        or      (hl)
        inc     hl
        ld      h,(hl)
        ld      l,a             ; HL=setup info for FDC
        ld      a,$03           ; send setup info & exit
        call    l2114
        ld      a,l
        call    l2114
        ld      a,h
        jp      l2114

; Subroutine to convert unit/head byte (A) to equipment address (in HL)

.l1f6a  ld      a,c
        and     $03
        add     a,a             ; A=2*unit
        add     a,equipment~$ff
        ld      l,a
        adc     a,equipment/$100
        sub     l
        ld      h,a             ; HL=equipment+2*unit
        ret     

; DD_L_SEEK

.l1f76  push    hl
        call    l1f6a           ; get equipment address
        call    l1f7f           ; do the seek
        pop     hl
        ret     

; Subroutine to seek to track D on equipment HL (XDPB=IX)

.l1f7f  ld      a,(retry_cnt)
        ld      b,a             ; B=# tries
.l1f83  bit     6,(hl)
        jr      nz,l1f92        ; move on if head positioned
        inc     hl
        ld      (hl),$00        ; set track zero
        dec     hl
        call    l1fb7           ; seek to track zero
        jr      nc,l1fa8        ; move on if error
        set     6,(hl)          ; signal head positioned
.l1f92  ld      a,d
        inc     hl
        cp      (hl)            ; compare required track to current
        dec     hl
        scf     
        ret     z               ; exit with carry set if same
        or      a
        jr      nz,l1fa0
        call    l1fb7           ; recalibrate if track 0 required
        jr      l1fa3
.l1fa0  call    l1fdb           ; else seek to track D
.l1fa3  jr      nc,l1fad        ; move on if error
        inc     hl
        ld      (hl),d          ; store new track number
        ret                     ; done
.l1fa8  push    de
        call    nz,l1fd7        ; attempt to seek to highest track
        pop     de
.l1fad  res     6,(hl)          ; signal head not positioned
        ret     z               ; exit if FDC wasn't ready
        call    l206f           ; wait until ready
        djnz    l1f83           ; loop back for retries
        cp      a               ; exit with carry reset & error 2 - seek fail
        ret     

; Subroutine to seek to track zero

.l1fb7  call    l1fbb           ; try once
        ret     z               ; and again if unsuccessful
.l1fbb  push    bc
        ld      b,(ix+$12)
        dec     b               ; B=max track
        bit     7,(ix+$11)
        jr      nz,l1fcd        ; move on unless double-track disk
        bit     3,(hl)
        jr      z,l1fcd         ; move on if single-track drive
        ld      a,b
        add     a,a
        ld      b,a             ; double max track #
.l1fcd  ld      a,$07
        call    l2114           ; send recalibrate command
        ld      a,c
        and     $03             
        jr      l1ffe           ; do the seek

; Subroutine to seek to track D (enter at l1fd7 for highest track)

.l1fd7  ld      d,(ix+$12)
        dec     d               ; D=high track
.l1fdb  push    bc
        ld      a,d
        inc     hl
        sub     (hl)            ; A=#tracks head must move
        dec     hl
        jr      nc,l1fe4
        cpl                     
        inc     a               ; ensure A is positive
.l1fe4  ld      b,a
        ld      a,$0f
        call    l2114           ; send seek command
        ld      a,c
        call    l2114           ; send unit number
        ld      a,d             ; A=track number
        bit     7,(ix+$11)
        jr      nz,l1ffe        ; move on if double-track disk
        bit     3,(hl)
        jr      z,l1ffe         ; move on unless double-track drive
        ld      a,b
        add     a,a
        ld      b,a             ; double tracks to move
        ld      a,d
        add     a,a             ; double track number required

; Subroutine to complete a seek command. Carry set on exit if successful

.l1ffe  push    hl
        call    l2114           ; send unit number
.l2002  ld      a,(tm_step)
        call    l201c           ; delay for step rate time
        djnz    l2002           ; for max tracks
        ld      a,(tm_hdset)
        call    l201c           ; delay for head settle time
        ld      hl,fdc_res
        call    l2080           ; sense interrupt status
        call    l2025           ; wait until seek successfully completed
        pop     hl
        pop     bc
        ret     

; Subroutine to delay for approx A milliseconds

.l201c  ld      l,$dc
.l201e  dec     l
        jr      nz,l201e
        dec     a
        jr      nz,l201c
        ret     

; Subroutine to wait for end of seek command
; On exit, carry set if seek completed successfully
; If unsuccessful, Z set if FDC was not ready

.l2025  ld      a,c             ; unit number
        or      $20             ; seek end bit
        inc     hl
        xor     (hl)            ; mask against ST0
        and     $fb             ; ignore side information
        scf     
        ret     z               ; return with carry set if seek ended
        ld      a,(hl)
        and     $c0             ; test error code
        xor     $80
        jr      z,l2046         ; move on if invalid command
        ld      a,(hl)
        xor     c
        and     $03
        jr      z,l2040         ; move on if no error
        call    l2080           ; else sense interrupt status again
        jr      l2025           ; and loop back
.l2040  ld      a,(hl)
        and     $08
        xor     $08             ; test not ready bit
        ret     z               ; exit with carry reset if so
.l2046  ld      a,$02
        or      a
        ret                     ; else exit with Z reset

; Subroutine to process results buffer, exiting with error or carry set
; Z is set for success or write-protect/not ready error

.l204a  inc     hl
        ld      a,(hl)          ; get ST0
        xor     c
        scf     
        ret     z               ; exit with success if no error bits
        and     $08
        xor     $08
        ret     z               ; exit with error 0 if not ready
        inc     hl
        ld      a,(hl)          ; get ST1
        cp      $80             ; bit 7 always set on +3
        scf     
        ret     z               ; exit with success if no error
        xor     $02
        ld      a,$01           ; error 1 if write-protected
        ret     z
        ld      a,$03           ; error 3 if CRC data error
        bit     5,(hl)
        ret     nz
        inc     a               ; error 4 if no data
        bit     2,(hl)
        ret     nz
        inc     a               ; error 5 if missing address mark
        bit     0,(hl)
        ret     nz
        inc     a
        inc     a               ; else error 7 (unknown error)
        ret

; Subroutine to wait until FDC ready for new command

.l206f  push    hl
        push    af
        ld      hl,fdc_res
.l2074  call    l2080           ; sense interrupt status
        and     $c0
        cp      $80
        jr      nz,l2074        ; wait for bit 7=1, bit 6=0
        pop     af
        pop     hl
        ret

; Subroutine to perform "sense interrupt status" command

.l2080  ld      a,$08           ; Sense interrupt status command
        call    l2114           ; send it
        jr      l2093           ; get results & exit

; Subroutine to perform "get unit status" command

.l2087  ld      a,$04           ; get unit status command
        call    l2114           ; send it
        ld      a,c             ; C=unit number
        call    l2114           ; send it & follow in to next routine

; Subroutine to get results string from FDC

.l2090  ld      hl,fdc_res      ; HL=buffer for results
.l2093  push    de
        push    bc
        ld      bc,$2ffd
        ld      d,$00           ; total bytes read
        inc     hl              ; step past length byte
        push    hl
.l209c  in      a,(c)           ; get FD status register
        add     a,a
        jr      nc,l209c        ; loop back until ready
        jp      p,l20b3         ; move on if no more bytes (bit 6 reset)
        ld      b,$3f
        in      a,(c)           ; get byte
        ld      b,$2f
        ld      (hl),a          ; store in buffer
        inc     hl
        inc     d               ; increment count
        ex      (sp),hl         ; short delay
        ex      (sp),hl
        ex      (sp),hl
        ex      (sp),hl
        jr      l209c           ; back for more
.l20b3  pop     hl
        ld      a,(hl)          ; A=first byte of results
        dec     hl
        ld      (hl),d          ; store results length
        pop     bc
        pop     de
        ret                     ; exit with HL=address of results

; Subroutine to read E bytes only

.l20ba  call    l20de           ; output command except last byte
        call    l2185           ; read the bytes
        jp      l2090           ; get results string & exit


; DD_L_READ

.l20c3  call    l20de           ; output command except last byte
        call    l21b7           ; read the bytes
        jp      l2090           ; get results string & exit

; DD_L_WRITE

.l20cc  call    l20de           ; output command except last byte
        call    l21d4           ; write the bytes
        ld      a,(tm_wroff)    ; get write off time value
.l20d5  dec     a
        inc     bc
        inc     bc
        inc     bc
        jr      nz,l20d5        ; write off delay
        jp      l2090           ; get results string & exit


; Subroutine to get page (D) and address (HL) of buffer for low-level
; command, and output all command bytes except last (in A)
; On entry, HL=address of parameter block

.l20de  call    l206f           ; wait until ready for new command
        ld      a,(BANKM)       ; get old BANKM
        and     $f8
        or      (hl)            ; set page required
        ld      b,a
        inc     hl
        ld      e,(hl)
        inc     hl
        ld      d,(hl)          ; DE=buffer address
        inc     hl
        ld      c,(hl)          ; C=# bytes to transfer (low)
        push    bc
        inc     hl
        inc     hl
        ld      b,(hl)          ; B=# command bytes
        inc     hl
        dec     b
.l20f4  ld      a,(hl)          ; get next command byte
        inc     hl
        call    l2114           ; send it
        djnz    l20f4           ; back for all except last
        ld      a,(hl)
        ex      de,hl
        pop     de              ; D=page required, E=#bytes to transfer (low)
        ld      bc,$7ffd
        di                      ; turn off interrupts
        ret     

; Subroutine to read a sector ID

.l2103  call    l206f           ; wait for FDC ready
        and     $40             ; get modulation mode
        or      $0a             ; read sector ID command
        call    l2114           ; send command
        ld      a,c
        call    l2114           ; send unit
        jp      l2090           ; get results

; Subroutine to wait for FD ready & ouput A to data register if
; controller wants input

.l2114  push    de
        push    bc
        ld      d,a
        ld      bc,$2ffd
.l211a  in      a,(c)           ; get FD status register
        add     a,a
        jr      nc,l211a        ; loop back if not ready
        add     a,a
        jr      c,l2128         ; exit if controller doesn't want input
        ld      b,$3f
        out     (c),d           ; output byte to data register
        ex      (sp),hl         ; short delay
        ex      (sp),hl
.l2128  pop     bc
        pop     de
        ret


; DD_L_ON_MOTOR

.l212b  push    bc
        push    af
        xor     a
        ld      (timeout),a     ; zero timeout value
        ld      a,(BANK678)
        bit     3,a
        jr      nz,l214d        ; exit if motor already on
        or      $08
        call    l2173           ; set motor on
        ld      a,(tm_mtron)    ; get motor spinup timing value
.l2140  push    af
        ld      bc,$3548
.l2144  dec     bc              ; delay loop
        ld      a,b
        or      c
        jr      nz,l2144
        pop     af
        dec     a
        jr      nz,l2140
.l214d  pop     af              ; restore registers and exit
        pop     bc
        ret

; DD_L_T_OFF_MOTOR

.l2150  push    af
        xor     a
        ld      (timeout),a     ; zero timeout value
        ld      a,(BANK678)
        and     $08
        jr      z,l2162         ; go to exit if motor already off
        ld      a,(tm_mtroff)
        ld      (timeout),a     ; else set timeout value
.l2162  pop     af
        ret     

; DD_L_OFF_MOTOR

.l2164  push    af
        xor     a
        ld      (timeout),a     ; zero timeout value
        ld      a,(BANK678)
        and     $f7             ; get current BANK678 value & mask motor bit
        call    l2173           ; change the value to switch off motor
        pop     af
        ret

; Subroutine to set the BANK678 value to A

.l2173  push    bc
        ld      b,a
        ld      a,r             ; get interrupt status
        ld      a,b
        ld      bc,$1ffd
        di      
        ld      (BANK678),a
        out     (c),a           ; set the value
        pop     bc
        ret     po
        ei                      ; re-enable interrupts if necessary
        ret     

; Subroutine to output last command byte to FDC and read E bytes to buffer

.l2185  call    l2114           ; send command
        out     (c),d           ; page in required bank
        ld      bc,$2ffd
        ld      d,$20
        jr      l219b
.l2191  ld      b,$3f
        ini                     ; read a byte
        ld      b,$2f
        dec     e
        jp      z,l21ac         ; move on if got all bytes required
.l219b  in      a,(c)
        jp      p,l219b         ; wait until FDC ready
        and     d
        jp      nz,l2191        ; loop back if more bytes on offer
        jr      l21ef           ; page bank 7 back and exit
.l21a6  ld      b,$3f
        in      a,(c)           ; discard a byte
        ld      b,$2f
.l21ac  in      a,(c)
        jp      p,l21ac         ; wait until FDC ready
        and     d
        jp      nz,l21a6        ; loop back if more bytes on offer
        jr      l21ef           ; page bank 7 back and exit

; Subroutine to output last byte of command to FDC and read bytes to buffer

.l21b7  call    l2114           ; send command
        out     (c),d           ; page in required bank
        ld      bc,$2ffd
        ld      d,$20
        jr      l21c9
.l21c3  ld      b,$3f
        ini                     ; read a byte
        ld      b,$2f
.l21c9  in      a,(c)
        jp      p,l21c9         ; wait until FDC ready
        and     d
        jp      nz,l21c3        ; loop back if more bytes to read
        jr      l21ef           ; go to repage bank 7 & exit

; Subroutine to output last byte of command to FDC and write bytes from buffer

.l21d4  call    l2114           ; send command
        out     (c),d           ; page in required bank
        ld      bc,$2ffd
        ld      d,$20
        jr      l21e6
.l21e0  ld      b,$40
        outi                    ; write a byte
        ld      b,$2f
.l21e6  in      a,(c)
        jp      p,l21e6         ; wait until FDC ready
        and     d
        jp      nz,l21e0        ; loop back if more bytes to write
.l21ef  ld      a,(BANKM)
        ld      bc,$7ffd
        out     (c),a           ; page bank 7 back in
        ei      
        ret

        defs    165


; ******************** KEYBOARD SCANNING ROUTINES *****************

; These are copies of the keytables from ROM 3

; The L-mode keytable with CAPS-SHIFT

.l229e  defm    "BHY65TGV"
        defm    "NJU74RFC"
        defm    "MKI83EDX"
        defm    $0e&"LO92WSZ"
        defm    " "&$0d&"P01QA"

; The extended-mode keytable (unshifted letters)

.l22c5  defb    $e3,$c4,$e0,$e4
        defb    $b4,$bc,$bd,$bb
        defb    $af,$b0,$b1,$c0
        defb    $a7,$a6,$be,$ad
        defb    $b2,$ba,$e5,$a5
        defb    $c2,$e1,$b3,$b9
        defb    $c1,$b8
        
; The extended mode keytable (shifted letters)

.l22df  defb    $7e,$dc,$da,$5c
        defb    $b7,$7b,$7d,$d8
        defb    $bf,$ae,$aa,$ab
        defb    $dd,$de,$df,$7f
        defb    $b5,$d6,$7c,$d5
        defb    $5d,$db,$b6,$d9
        defb    $5b,$d7
        
; The control code keytable (CAPS-SHIFTed digits)

.l22f9  defb    $0c,$07,$06,$04
        defb    $05,$08,$0a,$0b
        defb    $09,$0f
        
; The symbol code keytable (letters with symbol shift)

.l2303  defb    $e2,$2a,$3f,$cd
        defb    $c8,$cc,$cb,$5e
        defb    $ac,$2d,$2b,$3d
        defb    $2e,$2c,$3b,$22
        defb    $c7,$3c,$c3,$3e
        defb    $c5,$2f,$c9,$60
        defb    $c6,$3a

; The extended mode keytable (SYM-SHIFTed digits)

.l231d  defb    $d0,$ce,$a8,$ca
        defb    $d3,$d4,$d1,$d2
        defb    $a9,$cf


; This is a copy of the "keyboard scanning" subroutine from
; $028e in ROM 3

.l2327  ld      l,$2f
        ld      de,$ffff
        ld      bc,$fefe
.l232f  in      a,(c)
        cpl     
        and     $1f
        jr      z,l2344
        ld      h,a
        ld      a,l
.l2338  inc     d
        ret     nz
.l233a  sub     $08
        srl     h
        jr      nc,l233a
        ld      d,e
        ld      e,a
        jr      nz,l2338
.l2344  dec     l
        rlc     b
        jr      c,l232f
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

.l2358  call    l2327
        ret     nz
        ld      hl,KSTATE
.l235f  bit     7,(hl)
        jr      nz,l236a
        inc     hl
        dec     (hl)
        dec     hl
        jr      nz,l236a
        ld      (hl),$ff
.l236a  ld      a,l
        ld      hl,KSTATE+$04
        cp      l
        jr      nz,l235f
        call    l23b7
        ret     nc
        ld      hl,KSTATE
        cp      (hl)
        jr      z,l23a9
        ex      de,hl
        ld      hl,KSTATE+$04
        cp      (hl)
        jr      z,l23a9
        bit     7,(hl)
        jr      nz,l238a
        ex      de,hl
        bit     7,(hl)
        ret     z
.l238a  ld      e,a
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
        call    $23cc
        pop     hl
        ld      (hl),a
.l23a1  ld      (LAST_K),a
        set     5,(iy+$01)
        ret     
.l23a9  inc     hl
        ld      (hl),$05
        inc     hl
        dec     (hl)
        ret     nz
        ld      a,(REPPER)
        ld      (hl),a
        inc     hl
        ld      a,(hl)
        jr      l23a1

; This is a copy of the "K-Test" subroutine from $031e in ROM 3

.l23b7  ld      b,d
        ld      d,$00
        ld      a,e
        cp      $27
        ret     nc
        cp      $18
        jr      nz,l23c5
        bit     7,b
        ret     nz
.l23c5  ld      hl,l229e        ; the main keytable
        add     hl,de
        ld      a,(hl)
        scf     
        ret     

; This is a copy of the "keyboard decoding" subroutine from $0333 in
; ROM 3

.l23cc  ld      a,e
        cp      $3a
        jr      c,l2400
        dec     c
        jp      m,l23e8
        jr      z,l23da
        add     a,$4f
        ret     
.l23da  ld      hl,l22c5-'A'
        inc     b
        jr      z,l23e3
        ld      hl,l22df-'A'
.l23e3  ld      d,$00
        add     hl,de
        ld      a,(hl)
        ret     
.l23e8  ld      hl,l2303-'A'
        bit     0,b
        jr      z,l23e3
        bit     3,d
        jr      z,l23fd
        bit     3,(iy+$30)
        ret     nz
        inc     b
        ret     nz
        add     a,$20
        ret     
.l23fd  add     a,$a5
        ret     
.l2400  cp      $30
        ret     c
        dec     c
        jp      m,l2436
        jr      nz,l2422
        ld      hl,l231d-'0'
        bit     5,b
        jr      z,l23e3
        cp      $38
        jr      nc,l241b
        sub     $20
        inc     b
        ret     z
        add     a,$08
        ret     
.l241b  sub     $36
        inc     b
        ret     z
        add     a,$fe
        ret     
.l2422  ld      hl,l22f9-'0'
        cp      $39
        jr      z,l23e3
        cp      $30
        jr      z,l23e3
        and     $07
        add     a,$80
        inc     b
        ret     z
        xor     $0f
        ret     
.l2436  inc     b
        ret     z
        bit     5,b
        ld      hl,l22f9-'0'
        jr      nz,l23e3
        sub     $10
        cp      $22
        jr      z,l244b
        cp      $20
        ret     nz
        ld      a,$5f
        ret     
.l244b  ld      a,$40
        ret     
       
; These routines display error messages (in RAM at HL) using ROM 0.
; The first requires a response (DE=list of possible keys on entry)
        
.l244e  xor     a		; we need a response
        call    l3e00
	defw	$3ff0		; display message
	ret     
        
.l2455  or      a		; no response required
        call    l3e00
	defw	$3ff0		; display message
        ret            

; ******************* SELF-TEST PROGRAM SECTION START *******************
; This routine is copied to RAM at $6000 and executed there with ROM 1        
; paged in. A total of $C00 bytes is copied from here ($245c-$305b).

	defc	stst=$6000-ASMPC

.l6000  call    l683a+stst	; page ROM 1/bank 0
        rst     $28
	defw	$0d6b		; cls
        ld      a,$02
        rst     $28
        defw    $1601		; open stream 2
        call    l6038+stst	; do the tests
        push    af
        ld      hl,l6544+stst	; pass message
        jr      c,l601c		; move on if no error
        cp      $ff		; was error "no interface"?
        call    nz,l6588+stst	; if not, display error
        ld      hl,l6561+stst	; fail message
.l601c  call    l65a4+stst	; display pass/fail message
        ld      a,$0d
        call    l6391+stst	; display repeat/quit message
        pop     hl
.l6025  push    hl
        call    l67f9+stst	; get key
        pop     hl
        and     $df		; capitalise
        cp      'R'
        jp      z,l6000+stst	; re-start
        cp      'Q'
        jr      nz,l6025	; don't exit unless "Q"
        push    hl
        pop     af
        ret     		; finished!

; Subroutine to perform the tests

.l6038  ld      a,$01
        call    l6391+stst	; display signon message
        call    l6817+stst	; page ROM 2/bank 7
        call    DOS_INITIALISE
        call    l683a+stst	; page ROM 1/bank 0
        ret     nc		; exit if error
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_INTERFACE
        call    l683a+stst	; page ROM 1/bank 0
        jr      c,l605b		; move on if interface found
        ld      a,$03
        call    l6391+stst	; "no interface" message
        xor     a
        ld      a,$ff
        ret     		; exit
.l605b  xor     a
        ld      ($6aa4),a	; set unit 0
        ld      a,$02
        call    l6391+stst	; display "found drives" message
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_ASK_1
        call    l683a+stst	; page ROM 1/bank 0
        ld      a,'A'
        jr      nc,l6079	; if only one drive, move on
        ld      a,'&'		; else output "&B"
        rst     $10
        ld      a,'B'
        rst     $10
        ld      a,'B'
.l6079  ld      ($6aa3),a	; store max drive to test
        ld      a,$0d
        rst     $10		; CR
        ld      ix,$6a61
        ld      a,$00		; initialise a standard XDPB
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_SEL_FORMAT
        call    l683a+stst	; page ROM 1/bank 0
        ret     nc		; exit if error
        call    l60a1+stst	; test unit 0
        ret     nc		; exit if error
        ld      a,$01
        ld      ($6aa4),a	; set unit 1
        ld      a,($6aa3)
        cp      'B'
        call    z,l60a1+stst	; if B: present, test it
        ret     

; Subroutine to test unit at $6aa4

.l60a1  ld      a,$04
        call    l6391+stst	; display "testing drive" message
        ld      a,($6aa4)
        add     a,'A'
        rst     $10		; output drive letter
        ld      a,$0d
        rst     $10		; CR
        call    l636a+stst	; get drive status
        jr      z,l60be		; move on if no disk
        ld      a,$06
        call    l6391+stst	; display "remove disk" message
.l60b9  call    l636a+stst	; get drive status
        jr      nz,l60b9	; loop while disk present
.l60be  ld      a,$05
        call    l6391+stst	; display "insert side 1" message
.l60c3  call    l636a+stst	; get drive status
        jr      z,l60c3		; loop until disk present
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_INIT
        call    l683a+stst	; page ROM 1/bank 0
        ld      d,$00
        call    l62a8+stst	; format track 0
        ret     nc		
        call    l61f5+stst	; spin speed test
        ret     nc
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_INIT
        call    l683a+stst	; page ROM 1/bank 0
        ld      a,$0e
        call    l6391+stst	; display "formatting tracks" message
        ld      a,$e5
        ld      ($6860),a	; use $e5 as filler
        ld      d,$13
        call    l62a8+stst	; format track $13
        ret     nc
        ld      d,$00
        call    l62a8+stst	; format track 0
        ret     nc
        ld      d,$27
        call    l62a8+stst	; format track $27
        ret     nc
        ld      a,$aa
        ld      ($6860),a	; use $aa as filler
        ld      d,$12
        call    l62a8+stst	; format track $12
        ret     nc
        ld      d,$01
        call    l62a8+stst	; format track 1
        ret     nc
        ld      d,$26
        call    l62a8+stst	; format track $26
        ret     nc
        ld      a,$e5
        ld      ($6860),a	; test $e5 filled tracks
        ld      d,$00
        call    l6269+stst	; test track 0
        ret     nc
        ld      d,$13
        call    l6269+stst	; test track $13
        ret     nc
        ld      d,$27
        call    l6269+stst	; test track $27
        ret     nc
        ld      a,$aa
        ld      ($6860),a	; test $aa filled tracks
        ld      d,$01
        call    l6269+stst	; test track 1
        ret     nc
        ld      d,$12
        call    l6269+stst	; test track $12
        ret     nc
        ld      d,$26
        call    l6269+stst	; test track $26
        ret     nc
        xor     a
        ld      ($6860),a	; use 0 as filler
        ld      d,$00
        call    l62a8+stst	; format track 0
        ret     nc
        ld      d,$01
        call    l62a8+stst	; format track 1
        ret     nc
        ld      d,$12
        call    l62a8+stst	; format track $12
        ret     nc
        ld      d,$13
        call    l62a8+stst	; format track $13
        ret     nc
        ld      d,$26
        call    l62a8+stst	; format track $26
        ret     nc
        ld      d,$27
        call    l62a8+stst	; format track $27
        ret     nc
        ld      d,$00
        call    l6269+stst	; test track 0
        ret     nc
        ld      d,$01
        call    l6269+stst	; test track 1
        ret     nc
        ld      d,$12
        call    l6269+stst	; test track $12
        ret     nc
        ld      d,$13
        call    l6269+stst	; test track $13
        ret     nc
        ld      d,$26
        call    l6269+stst	; test track $26
        ret     nc
        ld      d,$27
        call    l6269+stst	; test track $27
        ret     nc
        ld      a,$06
        call    l6391+stst	; display "remove disk" message
.l6196  call    l636a+stst	; get drive status
        jr      nz,l6196	; loop until no disk present
        rst     $28
	defw	$0daf		; cls
        ld      a,$07
        call    l6391+stst	; display "insert side 2" message
.l61a3  call    l636a+stst	; get drive status
        jr      z,l61a3		; loop until disk present
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_INIT
        call    l683a+stst	; page ROM 1/bank 0
        ld      a,($6aa4)
        ld      c,a
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_DRIVE_STATUS
        call    l683a+stst	; page ROM 1/bank 0
        and     $40
        jr      nz,l61cb	; move on if write-protected
        ld      a,$08
        call    l6391+stst	; display "disk not w/p" message
        xor     a		; fail
        ld      a,$ff
        ret     
.l61cb  ld      a,$0a
        call    l6391+stst	; display "checking data" message
        ld      b,$28		; $28 tracks to check
.l61d2  push    bc
        ld      a,b
        add     a,a
        add     a,a
        add     a,a
        add     a,a
        add     a,b
        ld      ($6860),a	; filler is $11*(track+1)
        dec     b
        ld      d,b		; track number to check
        call    l6269+stst	; check track
        pop     bc
        ret     nc		; exit if error
        djnz    l61d2		; back for more
        ld      a,$06
        call    l6391+stst	; display "remove disk" message
.l61ea  call    l636a+stst	; get drive status
        jr      nz,l61ea	; loop back until no disk
        rst     $28
	defw	$0daf		; cls
        xor     a
        scf     		; success
        ret     

; Subroutine to test spin speed

.l61f5  ld      a,$0b
        call    l6391+stst	; display "starting spin speed test"
        call    l62de+stst	; time a read data command
        call    l62de+stst	; and again
        push    hl
        ld      hl,$3030
        ld      (l6522+stst),hl	; set spin speed difference 00%
        pop     hl
        push    de
        ld      ($685e),hl	; save low word of time taken
        ld      de,$4c2d
        or      a
        sbc     hl,de		; test time taken (low word)
        ld      a,'-'		; set spin speed +/- flag
        jr      nc,l6218
        ld      a,'+'
.l6218  ld      (l6521+stst),a	; set sign of spin speed difference
        pop     de
        ld      a,d
        push    af
        or      e		; test time taken (high word)
        jr      z,l6227
        pop     af
        ld      a,$63		; if not zero, spin speed out by 99%
        jp      l623f+stst
.l6227  pop     af
        ld      hl,($685e)	; get low word of time taken
        ld      de,$4c2d
        jr      nc,l6231
        ex      de,hl		; exchange if necessary
.l6231  xor     a		; zero difference percentage
        sbc     hl,de		; get timing difference
        ld      de,$00c3	; DE is 1% difference
.l6237  or      a
        sbc     hl,de
        jr      c,l623f		; move on when within range
        inc     a		; increment percentage difference
        jr      l6237		; loop back
.l623f  push    af
        or      a
        jr      z,l624c		; move on if spin speed exactly right
        ld      b,a
.l6244  ld      hl,l6523+stst	; address of %age in "spin diff" message
        call    l625e+stst	; increment by B%
        djnz    l6244
.l624c  ld      a,$0f
        call    l6391+stst	; display "spin speed diff" message
        pop     af
        cp      $03
        ld      a,$10
        push    af
        call    nc,l6391+stst	; if 3% out or more, "spin speed incorrect"
        pop     af
        ld      a,$ff
        ret     

; Subroutine to increment %age figure (ASCII) at HL

.l625e  ld      a,(hl)
        inc     a		; increment low digit
        ld      (hl),a
        cp      $3a
        ret     nz		; exit if in range
        ld      (hl),'0'	; else zero
        dec     hl		; and loop back for next higher digit
        jr      l625e

; Subroutine to test track D contains sector filled with correct filler

.l6269  ld      b,$09		; 9 sectors
        push    bc
        dec     b
        ld      a,($6aa4)
        ld      e,b		; E=sector (0 base)
        ld      c,a		; C=unit
        ld      b,$00		; page 0
        ld      e,$00		; sector 0
        ld      hl,$6861	; buffer to read to
        ld      ix,$6a61	; XDPB
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_READ_SECTOR
        call    l683a+stst	; page ROM 1/bank 0
        pop     bc
        ret     nc		; exit if error
        push    bc
        ld      hl,$6861
        ld      bc,$0200	; $200 bytes to test
.l628f  ld      a,($6860)
        cp      (hl)		; test against filler
        jr      nz,l629e	; move on if different
        inc     hl
        dec     bc
        ld      a,b
        or      c
        jr      nz,l628f	; loop back
        pop     bc
        scf     		; success
        ret     
.l629e  pop     bc
        ld      a,$0c
        call    l6391+stst	; display "data not read correctly" message
        xor     a
        ld      a,$ff
        ret     

; Subroutine to format track D with filler byte from $6860

.l62a8  ld      a,($6860)
        ld      e,a		; E=filler byte
        ld      b,$00		; B=page 0
        ld      a,($6aa4)
        ld      c,a		; C=unit
        ld      hl,$6a7b
        call    l62c6+stst	; fill format buffer
        ld      ix,$6a61	; IX=XDPB
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_FORMAT
        call    l683a+stst	; page ROM 1/bank 0
        ret     

; Subroutine to fill format buffer at HL for track D

.l62c6  push    af
        push    bc
        push    hl
        ld      b,$09		; 9 sectors
.l62cb  ld      (hl),d		; insert track number
        inc     hl
        ld      (hl),$00	; insert head number (0)
        inc     hl
        ld      a,$0a
        sub     b
        ld      (hl),a		; insert sector number (1-9)
        inc     hl
        ld      (hl),$02	; insert sector size (512 bytes)
        inc     hl
        djnz    l62cb		; loop back
        pop     hl
        pop     bc
        pop     af
        ret     

; Subroutine to turn on motor & time a read data command

.l62de  call    l636a+stst	; get drive status
        jr      z,l62de		; loop until disk present
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_L_ON_MOTOR
        call    l683a+stst	; page ROM 1/bank 0
        di      		; disable interrupts for timing
        call    l62f2+stst	; time a read data command
        ei      		; re-enable interrupts
        ret     

; Subroutine to time an impossible "read data" command

.l62f2  ld      bc,$2ffd
        in      a,(c)		; get FDC status
        bit     4,a
        jr      nz,l62f2	; loop if busy
        ld      a,$66
        call    l6353+stst	; send "read data" command
        ld      a,($6aa4)
        call    l6353+stst	; send unit
        xor     a
        call    l6353+stst	; send track 0
        xor     a
        call    l6353+stst	; send head 0
        ld      a,$fe
        call    l6353+stst	; send sector $fe
        ld      a,$03
        call    l6353+stst	; send 1024 bytes/sector
        ld      a,$fe
        call    l6353+stst	; send endsector $fe
        ld      a,$2a
        call    l6353+stst	; send gaplength $2a
        ld      a,$ff
        call    l6353+stst	; send datalength $ff
        ld      hl,$0000	; zero timer
        ld      de,$0000
.l632d  ld      bc,$2ffd
.l6330  in      a,(c)
        bit     7,a
        jr      nz,l633e	; move on if FDC ready
        inc     hl		; increment timer DEHL
        ld      a,h
        or      l
        jr      nz,l6330
        inc     de
        jr      l6330		; loop back
.l633e  bit     5,a
        jr      z,l6349		; move on if not in execution phase
        ld      bc,$3ffd
        in      a,(c)		; discard a byte
        jr      l632d		; loop back
.l6349  ld      bc,$2ffd
        in      a,(c)
        bit     7,a
        jr      z,l6349		; loop until FDC ready
        ret     

; Subroutine to output a byte (A) to FDC data register

.l6353  ld      d,a		; save byte
        ld      bc,$2ffd
.l6357  in      a,(c)
        and     $e0
        cp      $80
        jr      nz,l6357	; loop back until FDC ready for data
        ld      bc,$3ffd
        ld      a,d
        out     (c),a		; output byte
        ex      (sp),hl		; short delay
        ex      (sp),hl
        ex      (sp),hl
        ex      (sp),hl
        ret     

; Subroutine to get bit 5 of drive status

.l636a  ld      a,($6aa4)	; get unit
        ld      c,a
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_DRIVE_STATUS
        call    l683a+stst	; page ROM 1/bank 0
        ld      bc,$4000
.l637a  push    bc
        pop     bc
        dec     bc
        ld      a,b
        or      c
        jr      nz,l637a	; delay loop
        ld      a,($6aa4)
        ld      c,a
        call    l6817+stst	; page ROM 2/bank 7
        call    DD_DRIVE_STATUS
        call    l683a+stst	; page ROM 1/bank 0
        and     $20		; mask bit 5
        ret     

; Subroutine to set scroll count to max & display message A

.l6391  push    af
        ld      a,$ff
        ld      (SCR_CT),a
        pop     af
        ld      hl,l639e+stst	; first message
        jp      l6598+stst	; display message A

; Message table

.l639e  defm    $0d&0
        defm    "Integral disk test  V1.5"&$0d&0
        defm    $0d&"Found Drive(s) :A"&0
        defm    "NO DISK INTERFACE !"&$0d&$0d&"Test Aborted."&0 
        defm    $0d&"Testing Drive "&0
        defm    $0d&"Insert side 1 of test disk."&$0d&0
        defm    $0d&"Remove disk from drive"&$0d&0
        defm    $0d&"Insert side 2 of test disk."&0
        defm    $0d&"Disk is not write-protected."&$0d&0
        defm    $0d&"Spin test not implemented."&$0d&0
        defm    $0d&"Checking Data."&$0d&0
        defm    $0d&"Starting spin-speed test."&$0d&0
        defm    $0d&"Data not read correctly."&$0d&0
        defm    $0d&"Press R to repeat, Q to quit"&$0d&0
        defm    "Formatting tracks."&$0d&0
        defm    "Spin speed difference "
.l6521  defm    "*"
.l6522  defm    "0"
.l6523	defm	"0 %"&$0d&0
        defm    $0d&"Spin speed is INCORRECT !"&$0d&0
.l6544  defm    $0d&"The disk drive test passed."&0
.l6561  defm    $0d&"The disk drive test failed."&0

; Subroutine to set print position to top left - appears unused

.l657e  push    af
        ld      a,$16
.l6581  rst     $10
        xor     a
        rst     $10
.l6584  xor     a
        rst     $10
        pop     af
        ret     

; Subroutine to display error message A 

.l6588  ld      hl,l65b0+stst	; error message table start
        cp      $0a
        jr      c,l6591
        sub     $0a		; adjust error number to fit table
.l6591  call    l6598+stst	; display message
        ld      a,$0d
        rst     $10		; CR
        ret     

; Subroutine to display null-terminated message A, table start HL

.l6598  ld      b,a
        inc     b		; increment message count
        xor     a		; find null		
.l659b  push    hl
        pop     de
.l659d  cp      (hl)
.l659e  inc     hl
        jr      nz,l659d	; loop back until null found
        djnz    l659b		; loop back until at start of correct message
        ex      de,hl		; DE=message start
.l65a4  ld      a,(hl)		; get next char
        inc     hl
        or      a
        ret     z		; exit if null
        cp      $ff
        ret     z		; or $ff
        rst     $28
	defw	$0010		; output char
	jr      l65a4		; loop back

; Error message table

.l65b0  defm    "Drive not ready."&0
        defm    "Disc is write protected."&0
        defm    "Disc seek fail."&0
        defm    "Disc CRC data error."&0
        defm    "No data on disc."&0
        defm    "Address mark missing."&0
        defm    "Unrecognized disc format."&0
        defm    "Unknown disc error. - Well done !"&0
        defm    "Disc changed while in use."&0
        defm    "Wrong type of disc in drive."&0
        defm    "Bad filename."&0
        defm    "Bad parameter."&0
        defm    "Drive not found."&0
        defm    "File not found."&0
        defm    "File already exists."&0
        defm    "End of file."&0
        defm    "Disc full."&0
        defm    "Directory full."&0
        defm    "Read-only file."&0
        defm    "File not open, or wrong access."&0
        defm    "Access denied, file in use."&0
        defm    "Cannot rename between drives"&0
        defm    "Extent is missing."&0
        defm    "Uncached. (software error)"&0
        defm    "File too big."&0
        defm    "Disc not bootable"&0
        defm    "Drive was in use."&0
        defm    "You should never see this."&0

; Subroutine to wait for a keypress, returning in A

.l67f9  ld      hl,FLAGS
        res     5,(hl)		; set "no key"
.l67fe  bit     5,(hl)
        jr      z,l67fe		; loop back until key available
        ld      a,(LAST_K)	; get it
        res     5,(hl)		; set "no key"
        ret     

; Subroutine to reset the computer - appears to be unused

.l6808  di      
        ld      bc,$1ffd
        xor     a
        out     (c),a
        ld      bc,$7ffd
        out     (c),a
        jp      $0000

; Subroutine to make sure ROM 2 & bank 7 are paged in

.l6817  di
.l6818  push    af
        push    bc
        push    hl
        ld      bc,$7ffd
        ld      hl,BANKM
        ld      a,(hl)
        or      $07		; ensure ROM 0/2 & bank 7 paged
        and     $ef
.l6826  ld      (hl),a
        out     (c),a
        ld      hl,BANK678
        ld      a,(hl)
        or      $04		; ensure ROM 2 paged
        ld      (hl),a
        ld      bc,$1ffd
        out     (c),a
        pop     hl
        pop     bc
        pop     af
        ei      
        ret     

; Subroutine to make sure ROM 1 and bank 0 are paged in

.l683a  di
        push    af
        push    bc
        push    hl
        ld      bc,$7ffd
        ld      hl,BANKM
        ld      a,(hl)
        or      $10		; ensure ROM 1/3 & bank 0 paged
        and     $f8
        ld      (hl),a
.l684a  out     (c),a
        ld      a,(BANK678)
.l684f  and     $fb		; ensure ROM 1 paged
        ld      (BANK678),a
        ld      bc,$1ffd
        out     (c),a
        pop     hl
        pop     bc
        pop     af
        ei      
        ret     
        nop     
        nop     
        push    hl
        nop     
        nop     

; *********************** END OF SELF-TEST PROGRAM **********************


.l2cc4  defs    4417

; Paging routine, for calling a routine (address inline) in ROM 0

.l3e00  ld      (OLDHL),hl	; save HL & AF
        push    af
        pop     hl
        ld      (OLDAF),hl
        ex      (sp),hl
        ld      c,(hl)
        inc     hl
        ld      b,(hl)		; BC=inline address to call
        inc     hl
        ex      (sp),hl		; restack updated return address
        push    bc
        pop     hl		; HL=address to call
        ld      a,(BANK678)
        ld      bc,$1ffd
        res     2,a		; select ROM 0
        di      
        ld      (BANK678),a
        out     (c),a
        ei      
        ld      bc,l3e2d
        push    bc		; stack address to return to repage ROM 2
        push    hl		; stack address to call in ROM 0
        ld      hl,(OLDAF)	; restore AF & HL
        push    hl
        pop     af
        ld      hl,(OLDHL)
        ret     		; "return" to call routine
.l3e2d  push    bc		; stack registers
        push    af
        ld      a,(BANK678)
        ld      bc,$1ffd
        set     2,a		; select ROM 2
        di      
        ld      (BANK678),a
        out     (c),a
        ei      
        pop     af		; restore registers
        pop     bc
        ret			; done

        defs    191

; This routine is a duplicate of a routine to call a ROM 2 routine
; residing in ROM 1 (another copy is in ROM 0). As soon as ROM 2 is
; paged in, it takes control.

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
        out     (c),a           ; page in ROM 0
        ld      a,(BANK678)
        or      $04
        ld      (BANK678),a
.l3f2a  ld      bc,$1ffd
        out     (c),a           ; page in ROM 2
        ei      
        ld      bc,l3f42
        push    bc              ; stack routine address to return to ROM 1
        push    hl              ; stack routine address to call
        ld      hl,(OLDAF)      ; restore registers
        push    hl
        pop     af
        ld      bc,(OLDBC)
        ld      hl,(OLDHL)
        ret                     ; exit to ROM 2 routine

; This part then returns control to ROM 1
        
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

; Unused space

        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff,$ff,$ff,$ff,$ff
        defb    $ff,$ff,$ff,$ff
        defb    $66

; ----------------------------------------------------------------------------------------------------

; ======================================
; BCB (Buffer Control Block) description
; ======================================
; 
; Each buffer has a BCB consisting of $0B bytes.
; BCBs are contained in two chains: an "in-use" chain and a "free" chain.
; The most recently accessed buffers rise to the top of the in-use chain.
; 
; DE contains the address of the BCB for the current buffer during
; DOS operations.
; 
; 
; Offset  Length  Description
; ------  ------  -----------
; +00     2       Pointer to previous BCB in chain
; +02     1       Flags:  Bit 0 set if data changed & should be written to disk
; +03     2       FCB address of file using this buffer
; +05     1       Drive letter (ASCII)
; +06     2       Absolute logical sector
; +08     2       Buffer address
; +0A     1       Buffer bank

; ----------------------------------------------------------------------------------------------------

; ====================================
; FCB (File Control Block) description
; ====================================
; 
; Each file has an FCB consisting of $38 bytes. The first $20 bytes
; of this is a straight copy of the directory entry for the file.
; 
; BC contains the address of the FCB for the current file during
; DOS operations.
; 
; 
; Offset  Length  Description
; ------  ------  -----------
; +00     1       User area
; +01     8       Filename
; +09     3       Extension
; +0C     1       Extent counter (low 5 bits)
; +0D     1       Reserved (0)
; +0E     1       Extent counter (high 8 bits)
; +0F     1       Number of records in extent (low byte)
; +10     10      Allocation list
; +20     1       Access mode (bit 7 set if file open)
; +21     1       Drive file resides on (ASCII)
; +22     1       Flags:  Bit 0 set if dir contains valid copy of FCB info
; 			Bit 1 set if new directory entry needed
; 			Bit 2 set if new extent needs to be created
;                         Bit 3 set if +2B contains valid sector number
;                         Bit 4 set if current record changed
;                         Bit 5 set if valid record details in FCB
;                         Bit 6 set if file has header
; +23     3       File length
; +26     3       Filepointer
; +29     2       Record number
; +2B     2       Absolute logical sector
; +2D     2       Address of record start
; +2F     1       Bank containing record
; +30     8       +3 BASIC header data (as on p225 of +3 manual)

; ----------------------------------------------------------------------------------------------------

; ================================================
; XDPB (eXtended Disk Parameter Block) description
; ================================================
; 
; Each drive has an XDPB, largely as described in the +3 manual.
; However, +3DOS keeps extra information for each drive directly
; after its copies of the XDPBs, so we can regard these as being
; "extended" XDPBs of $30 bytes each.
; 
; IX contains the address of the XDPB for the current drive during
; DOS operations.
; 
; 
; Offset  Length  Description
; ------  ------  -----------
; +00     2       SPT records per track
; +02     1       BSH log2 (blocksize/128)
; +03     1       BLM (blocksize/128)-1
; +04     1       EXM extent mask
; +05     2       DSM last block number
; +07     2       DRM last directory entry number
; +09     1       ALO directory bitmap
; +0A     1       AL1 directory bitmap
; +0B     2       CKS checksum vector size (bit 15=fixed drive flag)
; +0D     2       OFF reserved tracks
; +0F     1       PSH log2 (sectorsize/128)
; +10     1       PHM (sectorsize/128)-1
; +11     1       Flags:  Bit 7 set if double-track
;                         Bits 0..1=sidedness (0=single,1=alternate,2=successive)
; +12     1       tracks per side
; +13     1       sectors per track
; +14     1       1st physical sector number
; +15     2       sector size
; +17     1       gap length (r/w)
; +18     1       gap length (format)
; +19     1       Flags:  Bit 7 = multitrack
;                         Bit 6 = MFM (FM=0)
;                         Bit 5 = skip deleted address marks
; +1A     1       Freeze flag: $ff=don't auto-detect disk format
; +1B     1       Flags:  Bit 3 set for RAMdisk
; 			Bit 2 set for floppy disks
;                         Bit 1 set if getting checksums (0=checking)
;                         Bit 0 set if drive logged in
; +1C     1       Drive letter (ASCII)
; +1D     1       FD unit number (for drive B:)
; +1E     3       Last access (FRAMES)
; +21     1       # files open on drive
; +22     2       # free directory entries
; +24     2       last used directory entry number
; +26     2       address of checksum vector
; +28     2       address of allocation bitmap
; +2A     2       low-level routine to login a disk
; +2C     2       low-level routine to read a sector
; +2E     2       low-level routine to write a sector

