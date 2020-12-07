$(document).on('click', '#hhDashInviteSubmit', function InviteAlert(e) {
    e.preventDefault();
    $("#exampleModal").modal("hide");
    swal("Good news!", "Your invitation has been sent.", "success")
        .then(() => $('#dashInviteForm').submit());
});

function CantLeave() {
    swal("Bad news!", "You can't leave while the house still has members.", "error");
}