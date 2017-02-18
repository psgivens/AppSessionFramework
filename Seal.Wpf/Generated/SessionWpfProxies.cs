using System.Collections;
using System.Windows;
using System.Windows.Input;
using PhillipScottGivens.Library.AppSessionFramework;
using PhillipScottGivens.Library.AppSessionFramework.WPF;
namespace PhillipScottGivens.Seal.Proxies.Wpf
{
    public class MyApplicationSession : PhillipScottGivens.Seal.Proxies.MyApplicationSession
    {
        private class OperationCommands
        {
            public OperationCommands(SessionBase sessionBase)
            {
                this._doSomethingAwesome = new OperationCommand(sessionBase["DoSomethingAwesome"]);
            }
            private ICommand _doSomethingAwesome;
            public ICommand DoSomethingAwesome { get { return _doSomethingAwesome; } }
        }
        private readonly OperationCommands _commands;
        public object Commands { get { return _commands; } }
        public MyApplicationSession()
        {
            this._commands = new OperationCommands(this);
        }
    }
    public class MyGeneratedClass : PhillipScottGivens.Seal.Proxies.MyGeneratedClass
    {
        private class OperationCommands
        {
            public OperationCommands(SessionBase sessionBase)
            {
            }
        }
        private readonly OperationCommands _commands;
        public object Commands { get { return _commands; } }
        public MyGeneratedClass()
        {
            this._commands = new OperationCommands(this);
        }
    }
}
