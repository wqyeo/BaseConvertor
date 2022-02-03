using System;
using System.Collections.Generic;
using System.Text;

namespace BaseConvertor.Exceptions {
    public class InvalidBaseValueException : Exception {
        public InvalidBaseValueException() : base() { }
        public InvalidBaseValueException(string message) : base(message) { }
        public InvalidBaseValueException(string message, Exception inner) : base(message, inner) { }
        protected InvalidBaseValueException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
