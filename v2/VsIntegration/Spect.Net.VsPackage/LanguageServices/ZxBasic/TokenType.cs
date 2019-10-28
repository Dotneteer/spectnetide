namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// Represents the type of a token
    /// </summary>
    public enum TokenType
    {
        None,
        ZxbConsole,
        ZxbPreProc,
        ZxbControlFlow,
        ZxbStatement,
        ZxbType,
        ZxbComment,
        ZxbFunction,
        ZxbOperator,
        ZxbIdentifier,
        ZxbNumber,
        ZxbString,
        ZxbLabel,
        ZxbAsm,

        Label,
        Comment,
        Pragma,
        Directive,
        IncludeDirective,
        Instruction,
        Number,
        Identifier,
        String,
        Function,
        MacroParam,
        Statement,
        MacroInvocation,
        Operand,
        SemiVar,
        Module
    }
}