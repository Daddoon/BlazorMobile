
window.BlazorMobileElectron = {

    GetCurrentURL: function () {
        return require('electron').remote.getCurrentWebContents().getURL();
    },

    GetUserDataPath: function () {
        return require('electron').remote.app.getPath("userData");
    }

};