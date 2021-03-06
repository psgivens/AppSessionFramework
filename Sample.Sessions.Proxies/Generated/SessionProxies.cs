using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.ComponentModel;
namespace Sample.Sessions.Proxies
{
    public static class SessionContainer
    {
        public static void Initialize(ISessionRegistrar registrar)
        {
            registrar.RegisterSession<Sample.Sessions.LoginSession, LoginSession>();
            registrar.RegisterSession<Sample.Sessions.MainSession, MainSession>();
            registrar.RegisterSession<Sample.Sessions.SubSession, SubSession>();
        }
    }
}
namespace Sample.Sessions.Proxies
{
    public class LoginSession : Sample.Sessions.LoginSession
    {
        public LoginSession(int sampleConstructorParameter)
        	: base(sampleConstructorParameter)
        {
        	OnConstructed();	
        }
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void Login()
        {
        	base.ExecuteOperation("Login", () => base.Login());
        }	
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void Register()
        {
        	base.ExecuteOperation("Register", () => base.Register());
        }	
    }
}
namespace Sample.Sessions.Proxies
{
    public class MainSession : Sample.Sessions.MainSession
    {
        public MainSession()
        	: base()
        {
        	OnConstructed();	
        }
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void DoAction()
        {
        	base.ExecuteOperation("DoAction", () => base.DoAction());
        }	
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void DoOtherAction()
        {
        	base.ExecuteOperation("DoOtherAction", () => base.DoOtherAction());
        }	
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void LogOut()
        {
        	base.ExecuteOperation("LogOut", () => base.LogOut());
        }	
    }
}
namespace Sample.Sessions.Proxies
{
    public class SubSession : Sample.Sessions.SubSession
    {
        public SubSession()
        	: base()
        {
        	OnConstructed();	
        }
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void DoSubAction()
        {
        	base.ExecuteOperation("DoSubAction", () => base.DoSubAction());
        }	
        // Generated by SessionProxyEmitter.EmitVoidOperation
        public override void DoOtherSubAction()
        {
        	base.ExecuteOperation("DoOtherSubAction", () => base.DoOtherSubAction());
        }	
    }
}
