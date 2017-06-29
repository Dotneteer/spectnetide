#pragma once

#define Z80OPERATION void __fastcall

typedef unsigned long long uBit64;
typedef long long bit64;
typedef unsigned long uBit32;
typedef unsigned short uBit16;
typedef signed short bit16;
typedef unsigned char byte;

class TZ80Cpu;

// Memory reader and writer hooks
typedef byte(__fastcall * TReadOperation)(uBit16 addr);
typedef void(__fastcall * TWriteOperation)(uBit16 addr, byte val);

// Z80 event callback functions
typedef void(__fastcall * TZ80CpuEventCallback)(TZ80Cpu& cpu);
typedef void(__fastcall * TZ80CpuOpCodeEventCallback)(TZ80Cpu& cpu, byte opCode);

// Z80 operation related types
typedef void(__fastcall *Z80Operation)(TZ80Cpu& cpu);