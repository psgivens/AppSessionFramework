using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Sessions {

    public partial class MainSession {
        /// <summary>
        /// This is the .ctor
        /// </summary>
        public MainSession() {

            #region Availability Calculators
            {
                var subSessionLink = CreateLink<SubSession>("/SubSession");
                var myFlagValue = subSessionLink.CreateDynamicBoolean(SubSession.MyFlagProperty, valueWhenUnavailable: false);
                
                var isLoggedIn = CreateDynamicBoolean(IsLoggedInProperty, false);
                SetAvailabilityCalculator(LoginSessionProperty, isLoggedIn.Not);
                SetAvailabilityCalculator(SubSessionProperty, isLoggedIn);
                SetAvailabilityCalculator(LogOutOperation, myFlagValue & isLoggedIn);

                var sampleFlagValue = CreateDynamicBoolean(SampleFlagProperty, false);
                SetAvailabilityCalculator(DoOtherActionOperation, sampleFlagValue);
                SetAvailabilityCalculator(DoActionOperation, sampleFlagValue.Not);
            }
            #endregion

            SubSession = SessionManager.ResolveSession<SubSession>();
            LoginSession = SessionManager.ResolveSession<LoginSession>();
        }

        protected override void OnBuildListeners(ListenerBuilder builder) {
            builder.ListenForTraitChange("./SubSession/MyFlag", (s, e) => {
                System.Diagnostics.Debugger.Break();
            });
            
            base.OnBuildListeners(builder);
        }


        /// <summary>
        /// This is the Operator
        /// </summary>
        /// <remarks>
        /// Have fun.
        /// </remarks>
        /// <example>
        /// mainSession.DoAction()
        /// </example>
        public virtual void DoAction() {
            SampleFlag = !SampleFlag;
            var b = IsAvailable(DoActionOperation);
        }

        public virtual void DoOtherAction() {
            SampleFlag = !SampleFlag;
        }

        public virtual void LogOut() {
            IsLoggedIn = false;
            LoginSession = SessionManager.ResolveSession<LoginSession>();
            SubSession.Close();
            SubSession = null;
        }

        internal void SetLoggedIn() {
            IsLoggedIn = true;
            LoginSession = null;
            SubSession = SessionManager.ResolveSession<SubSession>();
            
        }


        public SubSession SubSession {
            get { return this.GetValue(SubSessionProperty); }
            private set { this.SetValue(SubSessionProperty, value); }
        }

        public LoginSession LoginSession {
            get { return this.GetValue(LoginSessionProperty); }
            private set {
                if (LoginSession != null)
                    LoginSession.Closed -= LoginSession_Closed;
                this.SetValue(LoginSessionProperty, value);
                if (value != null)
                    value.Closed += LoginSession_Closed;

            }
        }
        private void LoginSession_Closed(object sender, EventArgs e) {
            LoginSession = null;
        }
    }
}
