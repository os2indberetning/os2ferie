﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.KMDVacationService.SetAbsenceAttendance {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", ConfigurationName="SetAbsenceAttendance.SetAbsenceAttendance_OS_SI")]
    public interface SetAbsenceAttendance_OS_SI {
        
        // CODEGEN: Generating message contract since the operation SetAbsenceAttendance_OS_SI is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse SetAbsenceAttendance_OS_SI(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse> SetAbsenceAttendance_OS_SIAsync(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external")]
    public partial class SetAbsenceAttendanceRequest : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string startDateField;
        
        private string originalStartDateField;
        
        private string startTimeField;
        
        private string originalStartTimeField;
        
        private string endDateField;
        
        private string originalEndDateField;
        
        private string endTimeField;
        
        private string originalEndTimeField;
        
        private string additionalDataField;
        
        private string operationField;
        
        private string personnelNumberField;
        
        private string absenceAttendanceTypeField;
        
        private string originalAbsenceAttendanceTypeField;
        
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
        public string OriginalStartDate {
            get {
                return this.originalStartDateField;
            }
            set {
                this.originalStartDateField = value;
                this.RaisePropertyChanged("OriginalStartDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string StartTime {
            get {
                return this.startTimeField;
            }
            set {
                this.startTimeField = value;
                this.RaisePropertyChanged("StartTime");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string OriginalStartTime {
            get {
                return this.originalStartTimeField;
            }
            set {
                this.originalStartTimeField = value;
                this.RaisePropertyChanged("OriginalStartTime");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string OriginalEndDate {
            get {
                return this.originalEndDateField;
            }
            set {
                this.originalEndDateField = value;
                this.RaisePropertyChanged("OriginalEndDate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string EndTime {
            get {
                return this.endTimeField;
            }
            set {
                this.endTimeField = value;
                this.RaisePropertyChanged("EndTime");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string OriginalEndTime {
            get {
                return this.originalEndTimeField;
            }
            set {
                this.originalEndTimeField = value;
                this.RaisePropertyChanged("OriginalEndTime");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string AdditionalData {
            get {
                return this.additionalDataField;
            }
            set {
                this.additionalDataField = value;
                this.RaisePropertyChanged("AdditionalData");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public string Operation {
            get {
                return this.operationField;
            }
            set {
                this.operationField = value;
                this.RaisePropertyChanged("Operation");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string AbsenceAttendanceType {
            get {
                return this.absenceAttendanceTypeField;
            }
            set {
                this.absenceAttendanceTypeField = value;
                this.RaisePropertyChanged("AbsenceAttendanceType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string OriginalAbsenceAttendanceType {
            get {
                return this.originalAbsenceAttendanceTypeField;
            }
            set {
                this.originalAbsenceAttendanceTypeField = value;
                this.RaisePropertyChanged("OriginalAbsenceAttendanceType");
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
    public partial class SetAbsenceAttendanceResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private SetAbsenceAttendanceResponseReturnStatus returnStatusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public SetAbsenceAttendanceResponseReturnStatus ReturnStatus {
            get {
                return this.returnStatusField;
            }
            set {
                this.returnStatusField = value;
                this.RaisePropertyChanged("ReturnStatus");
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
    public partial class SetAbsenceAttendanceResponseReturnStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SetAbsenceAttendance_OS_SIRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceRequest SetAbsenceAttendanceRequest_MT;
        
        public SetAbsenceAttendance_OS_SIRequest() {
        }
        
        public SetAbsenceAttendance_OS_SIRequest(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceRequest SetAbsenceAttendanceRequest_MT) {
            this.SetAbsenceAttendanceRequest_MT = SetAbsenceAttendanceRequest_MT;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SetAbsenceAttendance_OS_SIResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:kmd.dk:LPT:VACAB:external", Order=0)]
        public Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceResponse SetAbsenceAttendanceResponse_MT;
        
        public SetAbsenceAttendance_OS_SIResponse() {
        }
        
        public SetAbsenceAttendance_OS_SIResponse(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceResponse SetAbsenceAttendanceResponse_MT) {
            this.SetAbsenceAttendanceResponse_MT = SetAbsenceAttendanceResponse_MT;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SetAbsenceAttendance_OS_SIChannel : Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SetAbsenceAttendance_OS_SIClient : System.ServiceModel.ClientBase<Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI>, Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI {
        
        public SetAbsenceAttendance_OS_SIClient() {
        }
        
        public SetAbsenceAttendance_OS_SIClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SetAbsenceAttendance_OS_SIClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SetAbsenceAttendance_OS_SIClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SetAbsenceAttendance_OS_SIClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI.SetAbsenceAttendance_OS_SI(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest request) {
            return base.Channel.SetAbsenceAttendance_OS_SI(request);
        }
        
        public Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceResponse SetAbsenceAttendance_OS_SI(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceRequest SetAbsenceAttendanceRequest_MT) {
            Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest inValue = new Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest();
            inValue.SetAbsenceAttendanceRequest_MT = SetAbsenceAttendanceRequest_MT;
            Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse retVal = ((Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI)(this)).SetAbsenceAttendance_OS_SI(inValue);
            return retVal.SetAbsenceAttendanceResponse_MT;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse> Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI.SetAbsenceAttendance_OS_SIAsync(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest request) {
            return base.Channel.SetAbsenceAttendance_OS_SIAsync(request);
        }
        
        public System.Threading.Tasks.Task<Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIResponse> SetAbsenceAttendance_OS_SIAsync(Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendanceRequest SetAbsenceAttendanceRequest_MT) {
            Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest inValue = new Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SIRequest();
            inValue.SetAbsenceAttendanceRequest_MT = SetAbsenceAttendanceRequest_MT;
            return ((Infrastructure.KMDVacationService.SetAbsenceAttendance.SetAbsenceAttendance_OS_SI)(this)).SetAbsenceAttendance_OS_SIAsync(inValue);
        }
    }
}
