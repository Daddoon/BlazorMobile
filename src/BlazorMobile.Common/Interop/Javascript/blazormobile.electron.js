
window.BlazorMobileElectron = {
    AlertDialog: function (applicationRef, alertRef, Title, Message, Accept, Cancel) {

        var buttons = new Array();

        if (Accept !== null && Accept !== undefined) {
            buttons.push(Accept);
        }

        buttons.push(Cancel);

        require('electron').remote.dialog.showMessageBox({ title: Title, message: Message, buttons: buttons, type: "info", noLink: true }, function (response) {

            var isOk = true;

            if (buttons.length === 1 || buttons.length > 1 && response === buttons.length - 1) {
                isOk = false;
            }

            DotNet.invokeMethodAsync('BlazorMobile.ElectronNET', 'NotifyAlertSignalResult', applicationRef, alertRef, isOk);
        });
    }
};