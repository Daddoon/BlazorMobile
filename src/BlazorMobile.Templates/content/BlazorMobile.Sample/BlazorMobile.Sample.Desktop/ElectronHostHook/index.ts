// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";

var blazorMobileRequestValidatorURI: string;

function blazorMobileRequestValidatorMethod(url: string, referrer: string, mustCancel: Function) : any {

    try {
        let validationURI = blazorMobileRequestValidatorURI + "?uri=" + encodeURIComponent(url) + "&referrer=" + encodeURIComponent(referrer);

        const request = require('electron').net.request({
            method: 'GET',
            url: validationURI
        });

        request.on('response', (response) => {

            if (response.statusCode == "401") {
                mustCancel(true);
            }
            else {
                mustCancel(false);
            }
        });

        request.end();

    } catch (e) {
        mustCancel(false);
    }
}

export class HookService extends Connector {
    constructor(socket: SocketIO.Socket, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        // execute your own JavaScript Host logic here

        //The current line are required to forward navigating events to the Xamarin.Forms driver,
        //from the main frame and from a subframe
        this.on("add-blazormobile-navigating-behavior", async (serviceURI, done) => {

            //We are a little lazy here, and we are assuming that:
            //- This is called at startup
            //- The first frame to be found is the Blazor app main window
            //We may optimize this in the future

            blazorMobileRequestValidatorURI = serviceURI;
            try {

                require("electron").webContents.getAllWebContents()[0].session.webRequest.onBeforeRequest((details, cb) =>
                {
                    if (details.resourceType == "mainFrame" || details.resourceType == "subFrame") {

                        blazorMobileRequestValidatorMethod(details.url, details.referrer, function (cancel) {
                            if (cancel) {
                                //WARNING: This is a big hack and surely not an expected behavior
                                //If we return cancel: false, everything is fine
                                //But if we return cancel: true, the request is canceled,
                                //but the page navigate to this cancellation

                                //Instead we are not calling the callback, preventing to do anything
                                //It may be problematic with a lot of navigation, as we don't know the
                                //impact internally in Electron
                                return;
                            }
                            else {
                                cb({ cancel: false });
                            }
                        });
                    }
                    else {
                        cb({ cancel: false });
                    }
                });

                require("electron").webContents.getAllWebContents()[0].on("new-window", (event, url, frameName, disposition, options, additionalFeatures, referrer) => {

                    //We will prevent any new-window
                    //Instead calling our filter for navigation and then opening/showing if allowed by the app
                    event.preventDefault();

                    //Using this current strategy, the event handler may be called twice for verification
                    //on from this window, the other one from the new window if it was allowed, because
                    //it's actually navigating. But it should not be problematic.

                    blazorMobileRequestValidatorMethod(url, referrer.url, function (cancel) {
                        if (!cancel) {
                            //The URL is not cancelled, we should load the window normally
                            const win = new Electron.BrowserWindow({
                                webContents: options.webContents, // use existing webContents if provided
                                show: false
                            });

                            win.once('ready-to-show', () => win.show())
                            if (!options.webContents) {
                                win.loadURL(url) // existing webContents will be navigated automatically
                            }
                            event.newGuest = win;
                        }
                    });
                });
            } catch (e) {
                done(e.message);
            }

            done("ok");
        });
    }
}

