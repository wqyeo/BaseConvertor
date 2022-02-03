using System;
using System.Collections.Generic;
using System.Text;

namespace BaseConvertor {
    internal struct TryMessage {

        internal bool Success {
            get; private set;
        }

        internal Exception Exception {
            get; private set;
        }

        public TryMessage(bool success, Exception exception = null) {
            Success = success;
            Exception = exception;
        }
    }
}
