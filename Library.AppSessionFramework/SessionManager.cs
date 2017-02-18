using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public static partial class SessionManager {
        // TODO: Everything regarding SessionManager.sessions

        #region Fields
        internal static List<SessionBase> Sessions = new List<SessionBase>();
        private static Dictionary<SessionBase, bool> _sessionDirtyMap = new Dictionary<SessionBase, bool>();
        private static Queue<NonSessionNotifier> _dirtyNotifiers = new Queue<NonSessionNotifier>();
        private static Stack<string> _operationNames = new Stack<string>();
        private static ISessionResolver _resolver;
        #endregion

        #region Events
        public static event EventHandler<SessionCreatedEventArgs> SessionCreated;
        public static event EventHandler LeavingApplicationStack;
        #endregion

        #region Intialize and Teardown
        /// <summary>
        /// Depricated
        /// </summary>
        /// <param name="proxyAssemblyNamespace"></param>
        public static void Initialize(ISessionResolver resolver) {
            SessionManager._resolver = resolver;
        }
        #endregion

        #region Session Factory
        public static TSession ResolveSession<TSession>()
            where TSession : SessionBase {
            var type = typeof(TSession);
            string name = type.Name;

            TSession session = _resolver.ResolveSession<TSession>();

            //Activator.CreateInstance(proxyAssemblyNamespace,
            //string.Format("{0}.{1}", proxyAssemblyNamespace, name)).Unwrap();

            if (SessionCreated != null)
                SessionCreated(typeof(SessionManager), new SessionCreatedEventArgs(session));

            return session;
        }
        #endregion

        #region Operation handling
        public static void PushOperation(SessionBase session, string operationName) {
            _operationNames.Push(operationName);
        }
        public static void PopOperation() {
            _operationNames.Pop();

            if (!_operationNames.Any()) {
                // TODO: Move this to a SessionManager
                foreach (var pair in _sessionDirtyMap) {
                    if (!pair.Value)
                        continue;

                    pair.Key.NotifyPresentationLayer();

                }
                _sessionDirtyMap.Clear();

                if (LeavingApplicationStack != null)
                    LeavingApplicationStack(null, EventArgs.Empty);
            }

            while (_dirtyNotifiers.Count > 0)
                _dirtyNotifiers.Dequeue().Notify(NotifyLayer.Secondary);
        }
        #endregion

        #region Dirty and Notify Utilities
        public static void EnqueueNotifier(NonSessionNotifier notifier) {
            _dirtyNotifiers.Enqueue(notifier);
        }

        public static void MarkDirty(SessionBase session) {
            SessionManager._sessionDirtyMap[session] = true;
        }
        #endregion

        private static Func<Operation, object> convertOperation;
        public static void RegisterOperationConverter(Func<Operation, object> converter) {
            Debug.Assert(convertOperation == null);
            convertOperation = converter;
        }
        internal static object WrapOperation(Operation operation) {
            return convertOperation == null
                ? operation
                : convertOperation(operation);
        }
        internal static object WrapAsAvailability(AvailabilityStore availabilityStore) {
            return availabilityStore;
        }
    }

    #region class sessionCreatedEventArgs
    public class SessionCreatedEventArgs : EventArgs {
        #region Properties
        public SessionBase Session { get; private set; }
        #endregion

        #region Constructors
        public SessionCreatedEventArgs(SessionBase session) {
            Session = session;
        }
        #endregion
    }
    #endregion
}
