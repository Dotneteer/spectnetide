namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the command line options of the ZXB executable.
    /// </summary>
    public class ZxbOptions
    {
        private ushort _optimize = 1;

        /// <summary>
        /// Signs in raw arguments are used.
        /// </summary>
        public string RawArgs { get; set; }

        /// <summary>
        /// Sets the program file name.
        /// </summary>
        public string ProgramFilename { get; set; }

        /// <summary>
        /// --optimize
        /// The default optimization level is 1. Setting this to a value greater than 1 will enable
        /// the compiler code optimizations (e.g.Peephole optimizer). Setting this to 0 will produce
        /// slower code, but could be useful for debugging purposes (both for the compiler or the BASIC
        /// program). A value of 3 will enable aggressive optimizations not fully tested yet! So, beware!
        /// </summary>
        public ushort Optimize
        {
            get => _optimize;
            set
            {
                _optimize = value;
                if (_optimize < 1) _optimize = 1;
                else if (_optimize > 3) _optimize = 3;
            }
        }

        /// <summary>
        /// --output
        /// Sets the output file name. By default it will be the same as the input file, but with the
        /// extension changed as appropriated (.BIN, .TAP, .ASM, .TZX).
        /// </summary>
        public string OutputFilename { get; set; }

        /// <summary>
        /// --stderr
        /// This specifies an output file name for error messages. This is useful if you want to capture
        /// compilation error messages (for example, to call ZX BASIC compiler from within an IDE).
        /// </summary>
        public string ErrorFilename { get; set; }

        /// <summary>
        /// --tzx
        /// Outputs the binary result in TZX Format. This format can be converted to sound (.WAV or .MP3).
        /// </summary>
        public bool? TzxFormat { get; set; }

        /// <summary>
        /// --tap
        /// Outputs the binary result in TAP Format.
        /// </summary>
        public bool? TapFormat { get; set; }

        /// <summary>
        /// --BASIC
        /// This is a very useful option. It will prepend a ZX spectrum BASIC loader that will load the
        /// rest of your binary compiled program. This option requires the --tap or --tzx to be specified.
        /// This way you can type LOAD "" to load your program.
        /// </summary>
        public bool? BasicLoader { get; set; }

        /// <summary>
        /// --autorun
        /// Makes your program to run automatically. If specified with the -B or --basic option, your program
        /// will automatically run if loaded with LOAD "". If --BASIC is not used this option is ignored.
        /// </summary>
        public bool? AutoRun { get; set; }

        /// <summary>
        /// --org
        /// This will change the default machine code ORiGin. By default your code will start at memory position
        /// 32768 (8000h). But you can change this with this parameter.
        /// </summary>
        public ushort OrgValue { get; set; } = 0x8000;

        /// <summary>
        /// --stderr
        /// This specifies an output file name for error msgs. This is useful if you want to capture compilation
        /// error messages (for example, to call ZX BASIC compiler from within an IDE).
        /// </summary>
        public string StdErr { get; set; }

        /// <summary>
        /// --array-base
        /// Unlike original Sinclair BASIC, array indexes starts from 0, not from 1 (see DIM). You can change this
        /// behavior. For example setting --array-base=1 will make array indexes start from 1 (like in Sinclair BASIC).
        /// This option (array-base=1) is active when --sinclair compatibility flag is specified.
        /// </summary>
        public bool? ArrayBaseOne { get; set; }

        /// <summary>
        /// --string-base
        /// Unlike original Sinclair BASIC, string indexes starts from 0, not from 1. That is, in ZX BASIC, a$(1)
        /// is the 2nd letter in a$ string variable, and a$(0) the 1st. You can change this behavior, setting which
        /// position in the string refers to the 1st letter. For example, setting --string-base=1 will make strings
        /// start from 1 (like in Sinclair BASIC). This option (string-base=1) is active when --sinclair compatibility
        /// flag is specified.
        /// </summary>
        public bool? StringBaseOne { get; set; }

        /// <summary>
        /// --heap-size
        /// Set the size of the heap. Default heap size is above 4K (4768 bytes exactly). The heap is a memory zone
        /// used to store and manipulate strings (and other dynamic size objects where available) If you don't make
        /// use of strings, you can get back part of the heap memory. You might also need more heap space, so set it
        /// with this flag. The heap zone comes at the end of your program, and it size is fixed (won't change during
        /// program execution).
        /// </summary>
        public ushort HeapSize { get; set; } = 0x1000;

        /// <summary>
        /// --debug-memory
        /// During your program execution, using strings might fail due to lack of memory, but your program won't
        /// report it will continue executing (except the strings not fitting into the heap will be converted to
        /// NULL string or ""). The same applies to other dynamic objects. So enabling this flag, will make your
        /// program to stop reporting a ROM Out of memory error. This will add a little overhead to your program
        /// execution, but it's useful to detect Out of Memory errors.
        /// </summary>
        public bool? DebugMemory { get; set; }

        /// <summary>
        /// --debug-array
        /// As above, using wrong subscript (out of range) in arrays won't trigger an error. Setting this flag will
        /// raise ROM error Subscript out of Range. This flag will add a little overhead to your program execution,
        /// but it's useful to detect Out of Range errors.
        /// </summary>
        public bool? DebugArray { get; set; }

        /// <summary>
        /// --strict-bool
        /// By default, ZX BASIC will treat boolean values as 0 = False, Any other value = True. Some programmers
        /// expect TRUE = 1 always. Using this option will enforce boolean results to be always 0 or 1. Using this
        /// option might add a little overhead to your program. Using --sinclair option will also enable this feature.
        /// </summary>
        public bool? StrictBool { get; set; }

        /// <summary>
        /// --enable-break
        /// Unlike Sinclair BASIC, Your program, being converted to machine code, won't be affected by BREAK. If
        /// you enable this flag, you will be able to stop your program pressing BREAK (Shift + SPACE). The ROM Break
        /// message will also report the line in which the program was interrupted. This option will add some
        /// overhead to your program.
        /// </summary>
        public bool? EnableBreak { get; set; }

        /// <summary>
        /// --explicit
        /// Requires all variables to be declared with DIM before being used. This is something similar to Sinclair
        /// BASIC, in which when you tried to read a variable not previously set, a "Variable not Found" error was
        /// triggered. This option is really useful and you should enable it for large programs.
        /// </summary>
        public bool? ExplicitDim { get; set; }

        /// <summary>
        /// --strict
        /// Requires all variables (and parameters and functions!) to have an explicit type declared (e.g. Uinteger).
        /// Otherwise, forgetting a type will cause an error and the program won't compile. This is very useful to
        /// avoid forgetting type declarations. When the type is explicitly declared the compiler can make better
        /// assumptions and further error checking and optimizations.
        /// </summary>
        public bool? StrictTypes { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var argRoot = $"\"{ProgramFilename}\" "
                + $"--output \"{OutputFilename}\" "
                + "--asm ";
            var additional = string.IsNullOrWhiteSpace(RawArgs)
                ? (ArrayBaseOne ?? false ? "--array-base=1 " : "")
                    + $"--optimize {Optimize} "
                    + $"--org {OrgValue} "
                    + $"--heap-size {HeapSize} "
                    + "--sinclair "
                    + (StringBaseOne ?? false ? "--string-base=1 " : "")
                    + (TzxFormat ?? false ? "--tzx " : "")
                    + (TapFormat ?? false ? "--tap " : "")
                    + (BasicLoader ?? false ? "--BASIC " : "")
                    + (AutoRun ?? false ? "--autorun " : "")
                    + (DebugMemory ?? false ? "--debug-memory " : "")
                    + (DebugArray ?? false ? "--debug-array " : "")
                    + (StrictBool ?? false ? "--strict-bool " : "")
                    + (EnableBreak ?? false ? "--enable-break " : "")
                    + (ExplicitDim ?? false ? "--explicit " : "")
                    + (StrictTypes ?? false ? "--strict " : "")
                : RawArgs;
            return (argRoot + additional).Trim();
        }
    }
}
