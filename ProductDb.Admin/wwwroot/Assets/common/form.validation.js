
var formValidation = {
    validateform: function (formId, requiredFieldNameList) {
        var validationMessage = "";
        var inputFields = $("#" + formId + " input");
        inputFields.each(function () {
            var name = $(this).attr("id");
            var isfound = requiredFieldNameList.indexOf(name);
            if (isfound != -1) {
                validationMessage += $(this).val().trim() == "" ? name.toUpperCase + " Is Required Field" : "";
            }
        });
        return validationMessage;
    }
}