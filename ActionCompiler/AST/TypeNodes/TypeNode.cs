namespace ActionCompiler.AST.TypeNodes
{
    public abstract record TypeNode : ValueNode 
    {
        public abstract string GetTypeName();
    };
}