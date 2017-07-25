using System.Reflection.Emit;
using My.Helpers;

namespace My.Emit
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ConstructorBuilderExtension
    {
        public static EmitGenerator GetEmitGenerator(this ConstructorBuilder builder)
        {
        	Requires.NotNull(builder, "builder");
            var ilGen = builder.GetILGenerator();
            var emitGen = new EmitGenerator(ilGen);
            return emitGen;
        }
    }
}
