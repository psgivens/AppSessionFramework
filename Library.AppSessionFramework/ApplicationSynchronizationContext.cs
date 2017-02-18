using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public class ApplicationSynchronizationContext : SynchronizationContext {
        private readonly SynchronizationContext _innerContext;
        private readonly SessionBase _session;
        private readonly string _operationName;
        private ApplicationSynchronizationContext(SessionBase session, string operationName) {
            _innerContext = SynchronizationContext.Current;
            _session = session;
            _operationName = operationName;
        }
        public static void PushContext(SessionBase session, string operationName) {
            SynchronizationContext.SetSynchronizationContext(
                new ApplicationSynchronizationContext(session, operationName));
        }
        public static void PopContext() {
            var context = (ApplicationSynchronizationContext)SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context._innerContext);
        }
        public override void OperationStarted() {
            _innerContext.OperationStarted();
            base.OperationStarted();
        }
        public override void OperationCompleted() {
            _innerContext.OperationCompleted();
            base.OperationCompleted();
        }
        public override void Post(SendOrPostCallback d, object state) {
            _innerContext.Post(
                _ => _session.ExecuteOperation(_operationName + "Callback",
                    isCallback: true,
                    operationAction: () => d(_)),
                state);
        }
        public override void Send(SendOrPostCallback d, object state) {
            _innerContext.Send(
                _ => _session.ExecuteOperation(_operationName + "Callback",
                    isCallback: true,
                    operationAction: () => d(_)),
                state);
        }
    }
}
