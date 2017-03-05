using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class stored the description of ZX Spectrum system variables
    /// </summary>
    public class SystemVariables
    {
        private static readonly IList<SystemVariableInfo> s_Variables = new List<SystemVariableInfo>
        {
            new SystemVariableInfo("KSTATE", 0x5C00, 8, "Used in reading the keyboard"),
            new SystemVariableInfo("LAST_K", 0x5C08, 1, "Stores newly pressed key"),
            new SystemVariableInfo("REPDEL", 0x5C09, 1, "Time that a key must be held down before it repeats: initially 35"),
            new SystemVariableInfo("REPPER", 0x5C0A, 1, "Delay between successive repeats of a key held down: initially 5"),
            new SystemVariableInfo("REPPER", 0x5C0A, 1, "Delay between successive repeats of a key held down: initially 5"),
            new SystemVariableInfo("DEFADD", 0x5C0B, 2, "Address of arguments of user defined function if one is being evaluated; otherwise 0"),
            new SystemVariableInfo("K_DATA", 0x5C0D, 1, "Stores 2nd byte of colur controls entered from keyboard"),
            new SystemVariableInfo("TVDATA", 0x5C0E, 2, "Stores bytes of color, AT and TAB controls going to television"),
            new SystemVariableInfo("STRMS", 0x5C10, 38, "Addresses of channels attached to streams"),
            new SystemVariableInfo("CHARS", 0x5C36, 2, "256 less than address of character set"),
            new SystemVariableInfo("RASP", 0x5C38, 1, "Length of warning buzz"),
            new SystemVariableInfo("PIP", 0x5C39, 1, "Length of keyboard click"),
            new SystemVariableInfo("ERR_NR", 0x5C3A, 1, "1 less than the report code. Starts off at 255"),
            new SystemVariableInfo("FLAGS", 0x5C3B, 1, "Various flags to control the BASIC system"),
            new SystemVariableInfo("TV_FLAG", 0x5C3C, 1, "Flags associated with the television"),
            new SystemVariableInfo("ERR_SP", 0x5C3D, 2, "Address of item on machine stack to be used as error return"),
            new SystemVariableInfo("LIST_SP", 0x5C3F, 2, "Address of return address from automatic listing"),
            new SystemVariableInfo("MODE", 0x5C41, 1, "Specifies K, L, C, E or G cursor"),
            new SystemVariableInfo("NEWPPC", 0x5C42, 2, "Line to be jumped to"),
            new SystemVariableInfo("NSPPC", 0x5C44, 1, "Statement number in line to be jumped to"),
            new SystemVariableInfo("PPC", 0x5C45, 2, "Line number of statement currently being executed"),
            new SystemVariableInfo("SUBPPC", 0x5C47, 1, "Number within line of statement being executed"),
            new SystemVariableInfo("BORDCR", 0x5C48, 1, "Border colour * 8; also contains the attributes normally used for the lower half of the screen"),
            new SystemVariableInfo("E_PPC", 0x5C49, 2, "Number of current line (with program cursor)"),
            new SystemVariableInfo("VARS", 0x5C4B, 2, "Address of variables"),
            new SystemVariableInfo("DEST", 0x5C4D, 2, "Address of variable in assignment"),
            new SystemVariableInfo("CHANS", 0x5C4F, 2, "Address of channel data"),
            new SystemVariableInfo("CURCHL", 0x5C51, 2, "Address of information currently being used for input and output"),
            new SystemVariableInfo("PROG", 0x5C53, 2, "Address of BASIC program"),
            new SystemVariableInfo("NXTLIN", 0x5C55, 2, "Address of next line in program"),
            new SystemVariableInfo("DATADD", 0x5C57, 2, "Address of terminator of last DATA item"),
            new SystemVariableInfo("E_LINE", 0x5C59, 2, "Address of command being typed in"),
            new SystemVariableInfo("K_CUR", 0x5C5B, 2, "Address of cursor"),
            new SystemVariableInfo("CH_ADD", 0x5C5D, 2, "Address of the next character to be interpreted"),
            new SystemVariableInfo("X_PTR", 0x5C5F, 2, "Address of the character after the ? marker"),
            new SystemVariableInfo("WORKSP", 0x5C61, 2, "Address of temporary work space"),
            new SystemVariableInfo("STKBOT", 0x5C63, 2, "Address of bottom of calculator stack"),
            new SystemVariableInfo("STKEND", 0x5C65, 2, "Address of start of spare space"),
            new SystemVariableInfo("BREG", 0x5C67, 1, "Calculator's b register"),
            new SystemVariableInfo("MEM", 0x5C68, 2, "Address of area used for calculator's memory"),
            new SystemVariableInfo("FLAGS2", 0x5C6A, 1, "More flags"),
            new SystemVariableInfo("DF_SZ", 0x5C6B, 1, "The number of lines (including one blank line) in the lower part of the screen"),
            new SystemVariableInfo("S_TOP", 0x5C6C, 2, "The number of the top program line in automatic listings"),
            new SystemVariableInfo("OLDPPC", 0x5C6E, 2, "Line number to which CONTINUE jumps"),
            new SystemVariableInfo("OSPPC", 0x5C70, 1, "Number within line of statement to which CONTINUE jumps"),
            new SystemVariableInfo("FLAGX", 0x5C71, 1, "Various flags"),
            new SystemVariableInfo("STRLEN", 0x5C72, 2, "Length of string type destination in assignment"),
            new SystemVariableInfo("T_ADDR", 0x5C74, 2, "Address of next item in syntax table"),
            new SystemVariableInfo("SEED", 0x5C76, 2, "The seed for RND. This is the variable that is set by RANDOMIZE"),
            new SystemVariableInfo("FRAMES", 0x5C78, 3, "3 byte (least significant first), frame counter. Incremented every 20ms"),
            new SystemVariableInfo("UDG", 0x5C7B, 2, "Address of 1st user defined graphic"),
            new SystemVariableInfo("COORDS_X", 0x5C7D, 1, "x-coordinate of last point plotted"),
            new SystemVariableInfo("COORDS_Y", 0x5C7E, 1, "y-coordinate of last point plotted"),
            new SystemVariableInfo("P_POSN", 0x5C7F, 1, "33 column number of printer position"),
            new SystemVariableInfo("PR_CC", 0x5C80, 2, "Full address of next position for LPRINT to print at (in ZX printer buffer). Legal values 5B00 - 5B1F"),
            new SystemVariableInfo("ECHO_E", 0x5C82, 2, "33 column number and 24 line number (in lower half) of end of input buffer"),
            new SystemVariableInfo("DF_CC", 0x5C84, 2, "Address in display file of PRINT position"),
            new SystemVariableInfo("DFCCL", 0x5C86, 2, "Like DF_CC for lower part of screen"),
            new SystemVariableInfo("S_POSN", 0x5C88, 2, "33 column number/24 line number for PRINT position"),
            new SystemVariableInfo("SPOSNL", 0x5C8A, 2, "Like S_POSN for lower part"),
            new SystemVariableInfo("SCR_CT", 0x5C8C, 1, "Counts scrolls: it is always 1 more than the number of scrolls that will be done before stopping with scroll?"),
            new SystemVariableInfo("ATTR_P", 0x5C8D, 1, "Permanent current colors"),
            new SystemVariableInfo("MASK_P", 0x5C8E, 1, "Used for transparent colors"),
            new SystemVariableInfo("ATTR_T", 0x5C8F, 1, "Temporary current colors"),
            new SystemVariableInfo("MASK_T", 0x5C90, 1, "Like MASK_P, but temporary"),
            new SystemVariableInfo("P_FLAG", 0x5C91, 1, "More flags"),
            new SystemVariableInfo("MEMBOT", 0x5C91, 30, "Calculator's memory area; used to store numbers that cannot conveniently be put on the calculator stack"),
            new SystemVariableInfo("NMIADD", 0x5CB0, 2, "This is the address of a user supplied NMI address which is read by the standard ROM when a peripheral activates the NMI"),
            new SystemVariableInfo("RAMTOP", 0x5CB2, 2, "Address of last byte of BASIC system area"),
            new SystemVariableInfo("P_RAMT", 0x5CB4, 2, "Address of last byte of physical RAM")
        };

        /// <summary>
        /// Read only access to variables
        /// </summary>
        public static IReadOnlyList<SystemVariableInfo> Variables { get; }

        /// <summary>
        /// Static member initialization
        /// </summary>
        static SystemVariables()
        {
            Variables = new ReadOnlyCollection<SystemVariableInfo>(s_Variables);
        }
    }
}