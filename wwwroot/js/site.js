function CanInvite() {
    swal("Good news!", "Your invitation has been sent.", "success");
}

function CantInvite() {
    swal("Sorry!", "You can't invite someone who is already in a household.", "error");
}

function CantLeave() {
    swal("Sorry!", "You can't leave while the house still has members.", "error");
}
