#pragma once

#include "z80spec.h"

namespace Z80
{
	extern const Z80Operation standardOpCodes[];

	// --- Standard Z80 operation codes
	Z80OPERATION nop_op(TZ80Cpu& cpu);
	Z80OPERATION ld_qq_nn_op(TZ80Cpu& cpu);
};