namespace Spect.Net.BasicParser
{
    /// <summary>
    /// Represents the type of a token
    /// </summary>
    public enum TokenType
    {
        None,
        ZxbKeyword,
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