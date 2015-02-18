$(function () {
    var optionsOrig = {
        'apikey': 'FCF3FC50-C9F6-4D89-9D7E-6E3706C1A0BD',
        'resource': 'addressaccess',
        'select': handleSelectOrig,
        'error': function (message) { alert(message); }
    };
    $('#AddressField').spatialfind(optionsOrig);
});

//Handles selection of address (mouse or enter key)
function handleSelectOrig(data) {
    $('#streetName').val(data.streetName);
    $('#postCodeIdentifier').val(data.postCodeIdentifier);
    $('#streetBuildingIdentifier').val(data.streetBuildingIdentifier);
}

//For more options or event see dokumentation: https://www.smartadresse.dk/#ui-tabs-3