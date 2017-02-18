using System.Text;

namespace PhillipScottGivens.Library.PsgCore.Generators
{
    public interface ITextTransform
    {
        /// <summary>
        /// Gets the value of the wrapped TextTranformation instance's GenerationEnvironment property
        /// </summary>
        StringBuilder GenerationEnvironment { get; }

        /// <summary>
        /// Calls the wrapped TextTranformation instance's Write method.
        /// </summary>
        void Write(string text);

        /// <summary>
        /// Calls the wrapped TextTranformation instance's WriteLine method.
        /// </summary>
        void WriteLine(string text);
                
        CodeGenerationTools Code { get; }

        void PushIndent(string p);

        string PopIndent();
    }
}
