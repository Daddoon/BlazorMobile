
// Limit the requests for which events are
// triggered.
//
// This allos us to have our code being executed
// only when the following URLs are matched.
// 
// ps.: if we were going to dynamically set the
//      URLs to be matched (used a configuration
//      page, for example) we'd then specify the 
//      wildcard <all_urls> and then do the filtering
//      ourselves.
const filter = {
    urls: [
        "<all_urls>"
    ],
    types: ["sub_frame"]
};

// Extra flags for the `onBeforeRequest` event.
//
// Here we're specifying that we want our callback
// function to be executed synchronously such that
// the request remains blocked until the callback 
// function returns (having our filtering taking 
// effect).
const webRequestFlags = [
    'blocking'
];


browser.webRequest.onBeforeRequest.addListener(
    page => {

        let xhr = new XMLHttpRequest();

        let validationURI = new URL(page.documentUrl).origin + "/_validateRequest/?uri=" + encodeURIComponent(page.url) + "&referrer=" + encodeURIComponent(page.originUrl) + "&runtime=" + encodeURIComponent();
        xhr.open('GET', validationURI, false);

        xhr.setRequestHeader('Content-Type', 'text/plain');
        xhr.setRequestHeader('BlazorMobile-Validator', browser.runtime.id);

        var cancelValue = false;

        try {
            xhr.send();

            if (xhr.status === 200) {
                //If everything is right we compute the result
                if (xhr.responseText === "true") {
                    cancelValue = true;
                }
            }
        } catch (e) {
            //Will inherit default value
        }

        return {
            cancel: cancelValue
        };
    },
    filter,
    webRequestFlags
);