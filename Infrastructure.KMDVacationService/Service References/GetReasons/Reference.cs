﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.KMDVacationService.GetReasons {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", ConfigurationName="GetReasons.GetReasons_OS_SI")]
    public interface GetReasons_OS_SI {
        
        // CODEGEN: Generating message contract since the operation GetReasons_OS_SI is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse GetReasons_OS_SI(Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse> GetReasons_OS_SIAsync(Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class GetReasonsRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string personnelNumberField;
        
        private string leaveTypeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string PersonnelNumber {
            get {
                return this.personnelNumberField;
            }
            set {
                this.personnelNumberField = value;
                this.RaisePropertyChanged("PersonnelNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string leaveType {
            get {
                return this.leaveTypeField;
            }
            set {
                this.leaveTypeField = value;
                this.RaisePropertyChanged("leaveType");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class GetReasonsResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private GetReasonsResponseReturnStatus returnStatusField;
        
        private GetReasonsResponseItem[] reasonField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public GetReasonsResponseReturnStatus ReturnStatus {
            get {
                return this.returnStatusField;
            }
            set {
                this.returnStatusField = value;
                this.RaisePropertyChanged("ReturnStatus");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public GetReasonsResponseItem[] Reason {
            get {
                return this.reasonField;
            }
            set {
                this.reasonField = value;
                this.RaisePropertyChanged("Reason");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class GetReasonsResponseReturnStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string statusTypeField;
        
        private string messageTextField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string StatusType {
            get {
                return this.statusTypeField;
            }
            set {
                this.statusTypeField = value;
                this.RaisePropertyChanged("StatusType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string MessageText {
            get {
                return this.messageTextField;
            }
            set {
                this.messageTextField = value;
                this.RaisePropertyChanged("MessageText");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class GetReasonsResponseItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string reasonCodeField;
        
        private string reasonCodeTextField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string ReasonCode {
            get {
                return this.reasonCodeField;
            }
            set {
                this.reasonCodeField = value;
                this.RaisePropertyChanged("ReasonCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string ReasonCodeText {
            get {
                return this.reasonCodeTextField;
            }
            set {
                this.reasonCodeTextField = value;
                this.RaisePropertyChanged("ReasonCodeText");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetReasons_OS_SIRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.GetReasons.GetReasonsRequest GetReasonsRequest_MT;
        
        public GetReasons_OS_SIRequest() {
        }
        
        public GetReasons_OS_SIRequest(Infrastructure.KMDVacationService.GetReasons.GetReasonsRequest GetReasonsRequest_MT) {
            this.GetReasonsRequest_MT = GetReasonsRequest_MT;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetReasons_OS_SIResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.GetReasons.GetReasonsResponse GetReasonsResponse_MT;
        
        public GetReasons_OS_SIResponse() {
        }
        
        public GetReasons_OS_SIResponse(Infrastructure.KMDVacationService.GetReasons.GetReasonsResponse GetReasonsResponse_MT) {
            this.GetReasonsResponse_MT = GetReasonsResponse_MT;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface GetReasons_OS_SIChannel : Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetReasons_OS_SIClient : System.ServiceModel.ClientBase<Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI>, Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI {
        
        public GetReasons_OS_SIClient() {
        }
        
        public GetReasons_OS_SIClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GetReasons_OS_SIClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetReasons_OS_SIClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetReasons_OS_SIClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI.GetReasons_OS_SI(Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest request) {
            return base.Channel.GetReasons_OS_SI(request);
        }
        
        public Infrastructure.KMDVacationService.GetReasons.GetReasonsResponse GetReasons_OS_SI(Infrastructure.KMDVacationService.GetReasons.GetReasonsRequest GetReasonsRequest_MT) {
            Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest inValue = new Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest();
            inValue.GetReasonsRequest_MT = GetReasonsRequest_MT;
            Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse retVal = ((Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI)(this)).GetReasons_OS_SI(inValue);
            return retVal.GetReasonsResponse_MT;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse> Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI.GetReasons_OS_SIAsync(Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest request) {
            return base.Channel.GetReasons_OS_SIAsync(request);
        }
        
        public System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIResponse> GetReasons_OS_SIAsync(Infrastructure.KMDVacationService.GetReasons.GetReasonsRequest GetReasonsRequest_MT) {
            Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest inValue = new Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SIRequest();
            inValue.GetReasonsRequest_MT = GetReasonsRequest_MT;
            return ((Infrastructure.KMDVacationService.GetReasons.GetReasons_OS_SI)(this)).GetReasons_OS_SIAsync(inValue);
        }
    }
}