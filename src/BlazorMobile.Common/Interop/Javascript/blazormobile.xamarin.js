
window.BlazorXamarin = {
    RuntimeCheck: function () {
		if (window.contextBridge == null || window.contextBridge == undefined) {
			return false;
		}
		return true;
	},

    ElectronGetCurrentURL: function () {
        return require('electron').remote.getCurrentWebContents().getURL();
    },

    ElectronGetUserDataPath: function () {
        return require('electron').remote.app.getPath("userData");
    },

    JSRuntimeHasElectronFeature: function () {
        return window.navigator.userAgent.toLocaleLowerCase().indexOf(" electron/") > -1;
    },

    HideElementById: function (elementId) {
        if (elementId !== null && elementId !== undefined && elementId !== "") {
            var selectedElement = window.document.getElementById(elementId);

            if (selectedElement !== null) {
                selectedElement.style.display = "none";
            }
        }
    }
};