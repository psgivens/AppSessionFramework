using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Sessions {
    public partial class LoginSession : SessionBase{

        protected override void OnConstructed() {
            Predicate<string> notEmptyPredicate = value => !string.IsNullOrWhiteSpace(value);

            var validUserName = CreateDynamicBoolean(UserNameProperty,
                valueWhenUnavailable: false,
                valueWhenAvailable: notEmptyPredicate);
            var validPassword = CreateDynamicBoolean(PasswordProperty,
                valueWhenUnavailable: false,
                valueWhenAvailable: notEmptyPredicate);

            SetAvailabilityCalculator(LoginOperation, validUserName & validPassword);

            var validNewUserNmae = CreateDynamicBoolean(NewUserNameProperty,
                valueWhenUnavailable: false,
                valueWhenAvailable: notEmptyPredicate);
            var validNewPassword1 = CreateDynamicBoolean(NewPassword1Property,
                valueWhenUnavailable: false,
                valueWhenAvailable: notEmptyPredicate);
            var validNewPassword2 = CreateDynamicBoolean(NewPassword2Property,
                valueWhenUnavailable: false,
                valueWhenAvailable: notEmptyPredicate);

            SetAvailabilityCalculator(RegisterOperation, validNewUserNmae & validNewPassword1 & validNewPassword2);

            UserName = "SampleUser";
            Password = "SamplePassword";

            base.OnConstructed();
        }

        public LoginSession(int sampleConstructorParameter) {

        }

        public virtual void Login() {
            if (UserName == "SampleUser" && Password == "SamplePassword") {
                AppRoot.MainSession.SetLoggedIn();
                CloseSession();
            }
        }

        public virtual void Register() {

        }
    }
}
