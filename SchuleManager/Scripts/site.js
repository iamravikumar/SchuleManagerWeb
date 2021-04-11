$("document").ready(function () {
    $(":checkbox").each(function () {
        var hiddenInput = $(this).next("input:hidden");
        var label = hiddenInput.next("label");
        //temporarily remove the hidden field 
        hiddenInput.remove();
        //re-add the hidden field after the label 
        label.after(hiddenInput);
    });
});