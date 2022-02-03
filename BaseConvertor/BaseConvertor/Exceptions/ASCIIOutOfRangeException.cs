using System;
using System.Collections.Generic;
using System.Text;

namespace BaseConvertor.Exceptions {
    public class ASCIIOutOfRangeException : Exception {
        public ASCIIOutOfRangeException() : base() { }
        public ASCIIOutOfRangeException(string message) : base(message) { }
        public ASCIIOutOfRangeException(string message, Exception inner) : base(message, inner) { }
        protected ASCIIOutOfRangeException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
