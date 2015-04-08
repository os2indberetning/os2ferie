﻿using System;
using System.Runtime.Serialization;

namespace Core.DomainServices.RoutingClasses
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

        public AddressLaunderingException(string message, int errorCode)
            : base(message)
        {
            _errorCode = errorCode;
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

        public AddressLaunderingException(string message, Exception e) : base(message, e)
        {
            
        }
    }

    public class AddressCoordinatesException : ApplicationException
    {
        public AddressCoordinatesException()
        {
        }

        public AddressCoordinatesException(string message)
            : base(message)
        {
        }

        public AddressCoordinatesException(string message, Exception inner)
            : base(message, inner)
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