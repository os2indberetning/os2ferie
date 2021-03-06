﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.KMDVacationService.GetWorkAccidentCases {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", ConfigurationName="GetWorkAccidentCases.GetWorkAccidentCases_OS_SI")]
    public interface GetWorkAccidentCases_OS_SI {
        
        // CODEGEN: Generating message contract since the operation GetWorkAccidentCases_OS_SI is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse GetWorkAccidentCases_OS_SI(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse> GetWorkAccidentCases_OS_SIAsync(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class GetWorkAccidentCasesRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string startDateField;
        
        private string endDateField;
        
        private string personnelNumberField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string StartDate {
            get {
                return this.startDateField;
            }
            set {
                this.startDateField = value;
                this.RaisePropertyChanged("StartDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string EndDate {
            get {
                return this.endDateField;
            }
            set {
                this.endDateField = value;
                this.RaisePropertyChanged("EndDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PersonnelNumber {
            get {
                return this.personnelNumberField;
            }
            set {
                this.personnelNumberField = value;
                this.RaisePropertyChanged("PersonnelNumber");
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
    public partial class GetWorkAccidentCasesResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private GetWorkAccidentCasesResponseReturnStatus returnStatusField;
        
        private GetWorkAccidentCasesResponseItem[] accidentCaseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public GetWorkAccidentCasesResponseReturnStatus ReturnStatus {
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
        public GetWorkAccidentCasesResponseItem[] AccidentCase {
            get {
                return this.accidentCaseField;
            }
            set {
                this.accidentCaseField = value;
                this.RaisePropertyChanged("AccidentCase");
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
    public partial class GetWorkAccidentCasesResponseReturnStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    public partial class GetWorkAccidentCasesResponseItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string personnelNumberField;
        
        private string startDateField;
        
        private string accidentNumberField;
        
        private string accidentDateField;
        
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
        public string StartDate {
            get {
                return this.startDateField;
            }
            set {
                this.startDateField = value;
                this.RaisePropertyChanged("StartDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string AccidentNumber {
            get {
                return this.accidentNumberField;
            }
            set {
                this.accidentNumberField = value;
                this.RaisePropertyChanged("AccidentNumber");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string AccidentDate {
            get {
                return this.accidentDateField;
            }
            set {
                this.accidentDateField = value;
                this.RaisePropertyChanged("AccidentDate");
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
    public partial class GetWorkAccidentCases_OS_SIRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesRequest GetWorkAccidentCasesRequest_MT;
        
        public GetWorkAccidentCases_OS_SIRequest() {
        }
        
        public GetWorkAccidentCases_OS_SIRequest(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesRequest GetWorkAccidentCasesRequest_MT) {
            this.GetWorkAccidentCasesRequest_MT = GetWorkAccidentCasesRequest_MT;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetWorkAccidentCases_OS_SIResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesResponse GetWorkAccidentCasesResponse_MT;
        
        public GetWorkAccidentCases_OS_SIResponse() {
        }
        
        public GetWorkAccidentCases_OS_SIResponse(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesResponse GetWorkAccidentCasesResponse_MT) {
            this.GetWorkAccidentCasesResponse_MT = GetWorkAccidentCasesResponse_MT;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface GetWorkAccidentCases_OS_SIChannel : Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetWorkAccidentCases_OS_SIClient : System.ServiceModel.ClientBase<Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI>, Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI {
        
        public GetWorkAccidentCases_OS_SIClient() {
        }
        
        public GetWorkAccidentCases_OS_SIClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GetWorkAccidentCases_OS_SIClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetWorkAccidentCases_OS_SIClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GetWorkAccidentCases_OS_SIClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI.GetWorkAccidentCases_OS_SI(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest request) {
            return base.Channel.GetWorkAccidentCases_OS_SI(request);
        }
        
        public Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesResponse GetWorkAccidentCases_OS_SI(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesRequest GetWorkAccidentCasesRequest_MT) {
            Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest inValue = new Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest();
            inValue.GetWorkAccidentCasesRequest_MT = GetWorkAccidentCasesRequest_MT;
            Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse retVal = ((Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI)(this)).GetWorkAccidentCases_OS_SI(inValue);
            return retVal.GetWorkAccidentCasesResponse_MT;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse> Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI.GetWorkAccidentCases_OS_SIAsync(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest request) {
            return base.Channel.GetWorkAccidentCases_OS_SIAsync(request);
        }
        
        public System.Threading.Tasks.Task<Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIResponse> GetWorkAccidentCases_OS_SIAsync(Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCasesRequest GetWorkAccidentCasesRequest_MT) {
            Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest inValue = new Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SIRequest();
            inValue.GetWorkAccidentCasesRequest_MT = GetWorkAccidentCasesRequest_MT;
            return ((Infrastructure.KMDVacationService.GetWorkAccidentCases.GetWorkAccidentCases_OS_SI)(this)).GetWorkAccidentCases_OS_SIAsync(inValue);
        }
    }
}
