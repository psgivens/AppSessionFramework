using PhillipScottGivens.Library.AppSessionFramework;
using PhillipScottGivens.Library.AppSessionFramework.ComponentModel;
using System;
using System.ComponentModel;
namespace PhillipScottGivens.Seal.Proxies
{
    public static class SessionContainer
    {
        public static void Initialize()
        {
            SessionManager.Register<PhillipScottGivens.Seal.MyApplicationSession, MyApplicationSession>();
            SessionManager.Register<PhillipScottGivens.Seal.MyGeneratedClass, MyGeneratedClass>();
        }
    }
}
namespace PhillipScottGivens.Seal.Proxies
{
    [TypeDescriptionProvider(typeof(SessionTypeDescriptionProvider<MyApplicationSession>))]
    public class MyApplicationSession : PhillipScottGivens.Seal.MyApplicationSession
    {
        public MyApplicationSession()
        {
        	InitializeSession();
        	OnConstructed();	
        }
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void DoSomethingAwesome()
        {
        	OnOperationBegin("DoSomethingAwesome");
            try
        	{
        	    base.DoSomethingAwesome();
        		OnOperationSuccess("DoSomethingAwesome");
        	}
        	catch(Exception e)
        	{
        		OnOperationError("DoSomethingAwesome", e);
        	}		
        }	
    }
}
namespace PhillipScottGivens.Seal.Proxies
{
    [TypeDescriptionProvider(typeof(SessionTypeDescriptionProvider<MyGeneratedClass>))]
    public class MyGeneratedClass : PhillipScottGivens.Seal.MyGeneratedClass
    {
        public MyGeneratedClass()
        {
        	InitializeSession();
        	OnConstructed();	
        }
    }
}
