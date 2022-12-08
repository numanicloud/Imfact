using System;

namespace Imfact.Utilities.Coding;

internal interface IFluentCodeBuilder : ICodeBuilder
{
    string OnPlainBuilder(Action<IFluentCodeBuilder> build);
    void EnterBlock(Action<IFluentCodeBuilder> build, bool withTrailingSemicolon = false);
    void EnterSequence(Action<IFluentCodeBuilder> build);
    void EnterChunk(Action<IFluentCodeBuilder> build);
    void EnterCsv(Action<IFluentCodeBuilder> build);
}