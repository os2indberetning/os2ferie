using System;
using System.Runtime.Serialization;

namespace Infrastructure.AddressServices.Classes
{
    [Serializable]
    public class AddressLaunderingException : ApplicationException
    {
        private int _errorCode;

        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        public AddressLaunderingException()
        {
        }

        public AddressLaunderingException(string message)
            : base(message)
        {
        }

        public AddressLaunderingException(string message, int errorCode) : base(message)
        {
            _errorCode = errorCode;
        }

        public AddressLaunderingException(string message, Exception inner) : base(message, inner)
        {
            
        }

        public AddressLaunderingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("_errorCode", _errorCode);
            }
        }
    }

    public class AddressCoordinatesException : ApplicationException
    {
        public AddressCoordinatesException()
        {
        }

        public AddressCoordinatesException(string message) : base(message)
        {
        }

        public AddressCoordinatesException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class RouteInformationException : ApplicationException
    {
        public RouteInformationException()
        {
        }

        public RouteInformationException(string message)
            : base(message)
        {
        }

        public RouteInformationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
