/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 5);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var MonoPlatform_1 = __webpack_require__(6);
exports.platform = MonoPlatform_1.monoPlatform;


/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var InternalRegisteredFunction_1 = __webpack_require__(7);
var registeredFunctions = {};
function registerFunction(identifier, implementation) {
    if (InternalRegisteredFunction_1.internalRegisteredFunctions.hasOwnProperty(identifier)) {
        throw new Error("The function identifier '" + identifier + "' is reserved and cannot be registered.");
    }
    if (registeredFunctions.hasOwnProperty(identifier)) {
        throw new Error("A function with the identifier '" + identifier + "' has already been registered.");
    }
    registeredFunctions[identifier] = implementation;
}
exports.registerFunction = registerFunction;
function getRegisteredFunction(identifier) {
    // By prioritising the internal ones, we ensure you can't override them
    var result = InternalRegisteredFunction_1.internalRegisteredFunctions[identifier] || registeredFunctions[identifier];
    if (result) {
        return result;
    }
    else {
        throw new Error("Could not find registered function with name '" + identifier + "'.");
    }
}
exports.getRegisteredFunction = getRegisteredFunction;


/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
function getAssemblyNameFromUrl(url) {
    var lastSegment = url.substring(url.lastIndexOf('/') + 1);
    var queryStringStartPos = lastSegment.indexOf('?');
    var filename = queryStringStartPos < 0 ? lastSegment : lastSegment.substring(0, queryStringStartPos);
    return filename.replace(/\.dll$/, '');
}
exports.getAssemblyNameFromUrl = getAssemblyNameFromUrl;


/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var RenderBatch_1 = __webpack_require__(9);
var BrowserRenderer_1 = __webpack_require__(10);
var browserRenderers = {};
function attachRootComponentToElement(browserRendererId, elementSelector, componentId) {
    var elementSelectorJs = Environment_1.platform.toJavaScriptString(elementSelector);
    var element = document.querySelector(elementSelectorJs);
    if (!element) {
        throw new Error("Could not find any element matching selector '" + elementSelectorJs + "'.");
    }
    var browserRenderer = browserRenderers[browserRendererId];
    if (!browserRenderer) {
        browserRenderer = browserRenderers[browserRendererId] = new BrowserRenderer_1.BrowserRenderer(browserRendererId);
    }
    browserRenderer.attachRootComponentToElement(componentId, element);
    clearElement(element);
}
exports.attachRootComponentToElement = attachRootComponentToElement;
function renderBatch(browserRendererId, batch) {
    var browserRenderer = browserRenderers[browserRendererId];
    if (!browserRenderer) {
        throw new Error("There is no browser renderer with ID " + browserRendererId + ".");
    }
    var updatedComponents = RenderBatch_1.renderBatch.updatedComponents(batch);
    var updatedComponentsLength = RenderBatch_1.arrayRange.count(updatedComponents);
    var updatedComponentsArray = RenderBatch_1.arrayRange.array(updatedComponents);
    var referenceFramesStruct = RenderBatch_1.renderBatch.referenceFrames(batch);
    var referenceFrames = RenderBatch_1.arrayRange.array(referenceFramesStruct);
    for (var i = 0; i < updatedComponentsLength; i++) {
        var diff = Environment_1.platform.getArrayEntryPtr(updatedComponentsArray, i, RenderBatch_1.renderTreeDiffStructLength);
        var componentId = RenderBatch_1.renderTreeDiff.componentId(diff);
        var editsArraySegment = RenderBatch_1.renderTreeDiff.edits(diff);
        var edits = RenderBatch_1.arraySegment.array(editsArraySegment);
        var editsOffset = RenderBatch_1.arraySegment.offset(editsArraySegment);
        var editsLength = RenderBatch_1.arraySegment.count(editsArraySegment);
        browserRenderer.updateComponent(componentId, edits, editsOffset, editsLength, referenceFrames);
    }
    var disposedComponentIds = RenderBatch_1.renderBatch.disposedComponentIds(batch);
    var disposedComponentIdsLength = RenderBatch_1.arrayRange.count(disposedComponentIds);
    var disposedComponentIdsArray = RenderBatch_1.arrayRange.array(disposedComponentIds);
    for (var i = 0; i < disposedComponentIdsLength; i++) {
        var componentIdPtr = Environment_1.platform.getArrayEntryPtr(disposedComponentIdsArray, i, 4);
        var componentId = Environment_1.platform.readInt32Field(componentIdPtr);
        browserRenderer.disposeComponent(componentId);
    }
    var disposedEventHandlerIds = RenderBatch_1.renderBatch.disposedEventHandlerIds(batch);
    var disposedEventHandlerIdsLength = RenderBatch_1.arrayRange.count(disposedEventHandlerIds);
    var disposedEventHandlerIdsArray = RenderBatch_1.arrayRange.array(disposedEventHandlerIds);
    for (var i = 0; i < disposedEventHandlerIdsLength; i++) {
        var eventHandlerIdPtr = Environment_1.platform.getArrayEntryPtr(disposedEventHandlerIdsArray, i, 4);
        var eventHandlerId = Environment_1.platform.readInt32Field(eventHandlerIdPtr);
        browserRenderer.disposeEventHandler(eventHandlerId);
    }
}
exports.renderBatch = renderBatch;
function clearElement(element) {
    var childNode;
    while (childNode = element.firstChild) {
        element.removeChild(childNode);
    }
}


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var RegisteredFunction_1 = __webpack_require__(1);
var Environment_1 = __webpack_require__(0);
var registeredFunctionPrefix = 'Microsoft.AspNetCore.Blazor.Browser.Services.BrowserUriHelper';
var notifyLocationChangedMethod;
var hasRegisteredEventListeners = false;
RegisteredFunction_1.registerFunction(registeredFunctionPrefix + ".getLocationHref", function () { return Environment_1.platform.toDotNetString(location.href); });
RegisteredFunction_1.registerFunction(registeredFunctionPrefix + ".getBaseURI", function () { return document.baseURI ? Environment_1.platform.toDotNetString(document.baseURI) : null; });
RegisteredFunction_1.registerFunction(registeredFunctionPrefix + ".enableNavigationInterception", function () {
    if (hasRegisteredEventListeners) {
        return;
    }
    hasRegisteredEventListeners = true;
    document.addEventListener('click', function (event) {
        // Intercept clicks on all <a> elements where the href is within the <base href> URI space
        var anchorTarget = findClosestAncestor(event.target, 'A');
        if (anchorTarget) {
            var href = anchorTarget.getAttribute('href');
            if (isWithinBaseUriSpace(toAbsoluteUri(href))) {
                event.preventDefault();
                performInternalNavigation(href);
            }
        }
    });
    window.addEventListener('popstate', handleInternalNavigation);
});
RegisteredFunction_1.registerFunction(registeredFunctionPrefix + ".navigateTo", function (uriDotNetString) {
    navigateTo(Environment_1.platform.toJavaScriptString(uriDotNetString));
});
function navigateTo(uri) {
    if (isWithinBaseUriSpace(toAbsoluteUri(uri))) {
        performInternalNavigation(uri);
    }
    else {
        location.href = uri;
    }
}
exports.navigateTo = navigateTo;
function performInternalNavigation(href) {
    history.pushState(null, /* ignored title */ '', href);
    handleInternalNavigation();
}
function handleInternalNavigation() {
    if (!notifyLocationChangedMethod) {
        notifyLocationChangedMethod = Environment_1.platform.findMethod('Microsoft.AspNetCore.Blazor.Browser', 'Microsoft.AspNetCore.Blazor.Browser.Services', 'BrowserUriHelper', 'NotifyLocationChanged');
    }
    Environment_1.platform.callMethod(notifyLocationChangedMethod, null, [
        Environment_1.platform.toDotNetString(location.href)
    ]);
}
var testAnchor;
function toAbsoluteUri(relativeUri) {
    testAnchor = testAnchor || document.createElement('a');
    testAnchor.href = relativeUri;
    return testAnchor.href;
}
function findClosestAncestor(element, tagName) {
    return !element
        ? null
        : element.tagName === tagName
            ? element
            : findClosestAncestor(element.parentElement, tagName);
}
function isWithinBaseUriSpace(href) {
    var baseUriPrefixWithTrailingSlash = toBaseUriPrefixWithTrailingSlash(document.baseURI); // TODO: Might baseURI really be null?
    return href.startsWith(baseUriPrefixWithTrailingSlash);
}
function toBaseUriPrefixWithTrailingSlash(baseUri) {
    return baseUri.substr(0, baseUri.lastIndexOf('/') + 1);
}


/***/ }),
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var DotNet_1 = __webpack_require__(2);
__webpack_require__(3);
__webpack_require__(15);
__webpack_require__(4);
__webpack_require__(16);
function boot() {
    return __awaiter(this, void 0, void 0, function () {
        var allScriptElems, thisScriptElem, isLinkerEnabled, entryPointDll, entryPointMethod, entryPointAssemblyName, referenceAssembliesCommaSeparated, referenceAssemblies, loadAssemblyUrls, ex_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    allScriptElems = document.getElementsByTagName('script');
                    thisScriptElem = (document.currentScript || allScriptElems[allScriptElems.length - 1]);
                    isLinkerEnabled = thisScriptElem.getAttribute('linker-enabled') === 'true';
                    entryPointDll = getRequiredBootScriptAttribute(thisScriptElem, 'main');
                    entryPointMethod = getRequiredBootScriptAttribute(thisScriptElem, 'entrypoint');
                    entryPointAssemblyName = DotNet_1.getAssemblyNameFromUrl(entryPointDll);
                    referenceAssembliesCommaSeparated = thisScriptElem.getAttribute('references') || '';
                    referenceAssemblies = referenceAssembliesCommaSeparated
                        .split(',')
                        .map(function (s) { return s.trim(); })
                        .filter(function (s) { return !!s; });
                    if (!isLinkerEnabled) {
                        console.info('Blazor is running in dev mode without IL stripping. To make the bundle size significantly smaller, publish the application or see https://go.microsoft.com/fwlink/?linkid=870414');
                    }
                    loadAssemblyUrls = [entryPointDll]
                        .concat(referenceAssemblies)
                        .map(function (filename) { return "_framework/_bin/" + filename; });
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 3, , 4]);
                    return [4 /*yield*/, Environment_1.platform.start(loadAssemblyUrls)];
                case 2:
                    _a.sent();
                    return [3 /*break*/, 4];
                case 3:
                    ex_1 = _a.sent();
                    throw new Error("Failed to start platform. Reason: " + ex_1);
                case 4:
                    // Start up the application
                    Environment_1.platform.callEntryPoint(entryPointAssemblyName, entryPointMethod, []);
                    return [2 /*return*/];
            }
        });
    });
}
function getRequiredBootScriptAttribute(elem, attributeName) {
    var result = elem.getAttribute(attributeName);
    if (!result) {
        throw new Error("Missing \"" + attributeName + "\" attribute on Blazor script tag.");
    }
    return result;
}
boot();


/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var DotNet_1 = __webpack_require__(2);
var RegisteredFunction_1 = __webpack_require__(1);
var assemblyHandleCache = {};
var typeHandleCache = {};
var methodHandleCache = {};
var assembly_load;
var find_class;
var find_method;
var invoke_method;
var mono_string_get_utf8;
var mono_string;
exports.monoPlatform = {
    start: function start(loadAssemblyUrls) {
        return new Promise(function (resolve, reject) {
            // mono.js assumes the existence of this
            window['Browser'] = {
                init: function () { },
                asyncLoad: asyncLoad
            };
            // Emscripten works by expecting the module config to be a global
            window['Module'] = createEmscriptenModuleInstance(loadAssemblyUrls, resolve, reject);
            addScriptTagsToDocument();
        });
    },
    findMethod: findMethod,
    callEntryPoint: function callEntryPoint(assemblyName, entrypointMethod, args) {
        // Parse the entrypointMethod, which is of the form MyApp.MyNamespace.MyTypeName::MyMethodName
        // Note that we don't support specifying a method overload, so it has to be unique
        var entrypointSegments = entrypointMethod.split('::');
        if (entrypointSegments.length != 2) {
            throw new Error('Malformed entry point method name; could not resolve class name and method name.');
        }
        var typeFullName = entrypointSegments[0];
        var methodName = entrypointSegments[1];
        var lastDot = typeFullName.lastIndexOf('.');
        var namespace = lastDot > -1 ? typeFullName.substring(0, lastDot) : '';
        var typeShortName = lastDot > -1 ? typeFullName.substring(lastDot + 1) : typeFullName;
        var entryPointMethodHandle = exports.monoPlatform.findMethod(assemblyName, namespace, typeShortName, methodName);
        exports.monoPlatform.callMethod(entryPointMethodHandle, null, args);
    },
    callMethod: function callMethod(method, target, args) {
        if (args.length > 4) {
            // Hopefully this restriction can be eased soon, but for now make it clear what's going on
            throw new Error("Currently, MonoPlatform supports passing a maximum of 4 arguments from JS to .NET. You tried to pass " + args.length + ".");
        }
        var stack = Module.stackSave();
        try {
            var argsBuffer = Module.stackAlloc(args.length);
            var exceptionFlagManagedInt = Module.stackAlloc(4);
            for (var i = 0; i < args.length; ++i) {
                Module.setValue(argsBuffer + i * 4, args[i], 'i32');
            }
            Module.setValue(exceptionFlagManagedInt, 0, 'i32');
            var res = invoke_method(method, target, argsBuffer, exceptionFlagManagedInt);
            if (Module.getValue(exceptionFlagManagedInt, 'i32') !== 0) {
                // If the exception flag is set, the returned value is exception.ToString()
                throw new Error(exports.monoPlatform.toJavaScriptString(res));
            }
            return res;
        }
        finally {
            Module.stackRestore(stack);
        }
    },
    toJavaScriptString: function toJavaScriptString(managedString) {
        // Comments from original Mono sample:
        //FIXME this is wastefull, we could remove the temp malloc by going the UTF16 route
        //FIXME this is unsafe, cuz raw objects could be GC'd.
        var utf8 = mono_string_get_utf8(managedString);
        var res = Module.UTF8ToString(utf8);
        Module._free(utf8);
        return res;
    },
    toDotNetString: function toDotNetString(jsString) {
        return mono_string(jsString);
    },
    getArrayLength: function getArrayLength(array) {
        return Module.getValue(getArrayDataPointer(array), 'i32');
    },
    getArrayEntryPtr: function getArrayEntryPtr(array, index, itemSize) {
        // First byte is array length, followed by entries
        var address = getArrayDataPointer(array) + 4 + index * itemSize;
        return address;
    },
    getObjectFieldsBaseAddress: function getObjectFieldsBaseAddress(referenceTypedObject) {
        // The first two int32 values are internal Mono data
        return (referenceTypedObject + 8);
    },
    readInt32Field: function readHeapInt32(baseAddress, fieldOffset) {
        return Module.getValue(baseAddress + (fieldOffset || 0), 'i32');
    },
    readObjectField: function readHeapObject(baseAddress, fieldOffset) {
        return Module.getValue(baseAddress + (fieldOffset || 0), 'i32');
    },
    readStringField: function readHeapObject(baseAddress, fieldOffset) {
        var fieldValue = Module.getValue(baseAddress + (fieldOffset || 0), 'i32');
        return fieldValue === 0 ? null : exports.monoPlatform.toJavaScriptString(fieldValue);
    },
    readStructField: function readStructField(baseAddress, fieldOffset) {
        return (baseAddress + (fieldOffset || 0));
    },
};
// Bypass normal type checking to add this extra function. It's only intended to be called from
// the JS code in Mono's driver.c. It's never intended to be called from TypeScript.
exports.monoPlatform.monoGetRegisteredFunction = RegisteredFunction_1.getRegisteredFunction;
function findAssembly(assemblyName) {
    var assemblyHandle = assemblyHandleCache[assemblyName];
    if (!assemblyHandle) {
        assemblyHandle = assembly_load(assemblyName);
        if (!assemblyHandle) {
            throw new Error("Could not find assembly \"" + assemblyName + "\"");
        }
        assemblyHandleCache[assemblyName] = assemblyHandle;
    }
    return assemblyHandle;
}
function findType(assemblyName, namespace, className) {
    var fullyQualifiedTypeName = "[" + assemblyName + "]" + namespace + "." + className;
    var typeHandle = typeHandleCache[fullyQualifiedTypeName];
    if (!typeHandle) {
        typeHandle = find_class(findAssembly(assemblyName), namespace, className);
        if (!typeHandle) {
            throw new Error("Could not find type \"" + className + "\" in namespace \"" + namespace + "\" in assembly \"" + assemblyName + "\"");
        }
        typeHandleCache[fullyQualifiedTypeName] = typeHandle;
    }
    return typeHandle;
}
function findMethod(assemblyName, namespace, className, methodName) {
    var fullyQualifiedMethodName = "[" + assemblyName + "]" + namespace + "." + className + "::" + methodName;
    var methodHandle = methodHandleCache[fullyQualifiedMethodName];
    if (!methodHandle) {
        methodHandle = find_method(findType(assemblyName, namespace, className), methodName, -1);
        if (!methodHandle) {
            throw new Error("Could not find method \"" + methodName + "\" on type \"" + namespace + "." + className + "\"");
        }
        methodHandleCache[fullyQualifiedMethodName] = methodHandle;
    }
    return methodHandle;
}
function addScriptTagsToDocument() {
    // Load either the wasm or asm.js version of the Mono runtime
    var browserSupportsNativeWebAssembly = typeof WebAssembly !== 'undefined' && WebAssembly.validate;
    var monoRuntimeUrlBase = '_framework/' + (browserSupportsNativeWebAssembly ? 'wasm' : 'asmjs');
    var monoRuntimeScriptUrl = monoRuntimeUrlBase + "/mono.js";
    if (!browserSupportsNativeWebAssembly) {
        // In the asmjs case, the initial memory structure is in a separate file we need to download
        var meminitXHR = Module['memoryInitializerRequest'] = new XMLHttpRequest();
        meminitXHR.open('GET', monoRuntimeUrlBase + "/mono.js.mem");
        meminitXHR.responseType = 'arraybuffer';
        meminitXHR.send(null);
    }
    document.write("<script defer src=\"" + monoRuntimeScriptUrl + "\"></script>");
}
function createEmscriptenModuleInstance(loadAssemblyUrls, onReady, onError) {
    var module = {};
    var wasmBinaryFile = '_framework/wasm/mono.wasm';
    var asmjsCodeFile = '_framework/asmjs/mono.asm.js';
    module.print = function (line) { return console.log("WASM: " + line); };
    module.printErr = function (line) { return console.error("WASM: " + line); };
    module.preRun = [];
    module.postRun = [];
    module.preloadPlugins = [];
    module.locateFile = function (fileName) {
        switch (fileName) {
            case 'mono.wasm': return wasmBinaryFile;
            case 'mono.asm.js': return asmjsCodeFile;
            default: return fileName;
        }
    };
    module.preRun.push(function () {
        // By now, emscripten should be initialised enough that we can capture these methods for later use
        assembly_load = Module.cwrap('mono_wasm_assembly_load', 'number', ['string']);
        find_class = Module.cwrap('mono_wasm_assembly_find_class', 'number', ['number', 'string', 'string']);
        find_method = Module.cwrap('mono_wasm_assembly_find_method', 'number', ['number', 'string', 'number']);
        invoke_method = Module.cwrap('mono_wasm_invoke_method', 'number', ['number', 'number', 'number']);
        mono_string_get_utf8 = Module.cwrap('mono_wasm_string_get_utf8', 'number', ['number']);
        mono_string = Module.cwrap('mono_wasm_string_from_js', 'number', ['string']);
        Module.FS_createPath('/', 'appBinDir', true, true);
        loadAssemblyUrls.forEach(function (url) {
            return FS.createPreloadedFile('appBinDir', DotNet_1.getAssemblyNameFromUrl(url) + ".dll", url, true, false, undefined, onError);
        });
    });
    module.postRun.push(function () {
        var load_runtime = Module.cwrap('mono_wasm_load_runtime', null, ['string']);
        load_runtime('appBinDir');
        onReady();
    });
    return module;
}
function asyncLoad(url, onload, onerror) {
    var xhr = new XMLHttpRequest;
    xhr.open('GET', url, /* async: */ true);
    xhr.responseType = 'arraybuffer';
    xhr.onload = function xhr_onload() {
        if (xhr.status == 200 || xhr.status == 0 && xhr.response) {
            var asm = new Uint8Array(xhr.response);
            onload(asm);
        }
        else {
            onerror(xhr);
        }
    };
    xhr.onerror = onerror;
    xhr.send(null);
}
function getArrayDataPointer(array) {
    return array + 12; // First byte from here is length, then following bytes are entries
}


/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var InvokeWithJsonMarshalling_1 = __webpack_require__(8);
var Renderer_1 = __webpack_require__(3);
/**
 * The definitive list of internal functions invokable from .NET code.
 * These function names are treated as 'reserved' and cannot be passed to registerFunction.
 */
exports.internalRegisteredFunctions = {
    attachRootComponentToElement: Renderer_1.attachRootComponentToElement,
    invokeWithJsonMarshalling: InvokeWithJsonMarshalling_1.invokeWithJsonMarshalling,
    renderBatch: Renderer_1.renderBatch,
};


/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var RegisteredFunction_1 = __webpack_require__(1);
function invokeWithJsonMarshalling(identifier) {
    var argsJson = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        argsJson[_i - 1] = arguments[_i];
    }
    var identifierJsString = Environment_1.platform.toJavaScriptString(identifier);
    var funcInstance = RegisteredFunction_1.getRegisteredFunction(identifierJsString);
    var args = argsJson.map(function (json) { return JSON.parse(Environment_1.platform.toJavaScriptString(json)); });
    var result = funcInstance.apply(null, args);
    if (result !== null && result !== undefined) {
        var resultJson = JSON.stringify(result);
        return Environment_1.platform.toDotNetString(resultJson);
    }
    else {
        return null;
    }
}
exports.invokeWithJsonMarshalling = invokeWithJsonMarshalling;


/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
// Keep in sync with the structs in .NET code
exports.renderBatch = {
    updatedComponents: function (obj) { return Environment_1.platform.readStructField(obj, 0); },
    referenceFrames: function (obj) { return Environment_1.platform.readStructField(obj, arrayRangeStructLength); },
    disposedComponentIds: function (obj) { return Environment_1.platform.readStructField(obj, arrayRangeStructLength + arrayRangeStructLength); },
    disposedEventHandlerIds: function (obj) { return Environment_1.platform.readStructField(obj, arrayRangeStructLength + arrayRangeStructLength + arrayRangeStructLength); },
};
var arrayRangeStructLength = 8;
exports.arrayRange = {
    array: function (obj) { return Environment_1.platform.readObjectField(obj, 0); },
    count: function (obj) { return Environment_1.platform.readInt32Field(obj, 4); },
};
var arraySegmentStructLength = 12;
exports.arraySegment = {
    array: function (obj) { return Environment_1.platform.readObjectField(obj, 0); },
    offset: function (obj) { return Environment_1.platform.readInt32Field(obj, 4); },
    count: function (obj) { return Environment_1.platform.readInt32Field(obj, 8); },
};
exports.renderTreeDiffStructLength = 4 + arraySegmentStructLength;
exports.renderTreeDiff = {
    componentId: function (obj) { return Environment_1.platform.readInt32Field(obj, 0); },
    edits: function (obj) { return Environment_1.platform.readStructField(obj, 4); },
};


/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var RenderTreeEdit_1 = __webpack_require__(11);
var RenderTreeFrame_1 = __webpack_require__(12);
var Environment_1 = __webpack_require__(0);
var EventDelegator_1 = __webpack_require__(13);
var selectValuePropname = '_blazorSelectValue';
var raiseEventMethod;
var renderComponentMethod;
var BrowserRenderer = /** @class */ (function () {
    function BrowserRenderer(browserRendererId) {
        var _this = this;
        this.browserRendererId = browserRendererId;
        this.childComponentLocations = {};
        this.eventDelegator = new EventDelegator_1.EventDelegator(function (event, componentId, eventHandlerId, eventArgs) {
            raiseEvent(event, _this.browserRendererId, componentId, eventHandlerId, eventArgs);
        });
    }
    BrowserRenderer.prototype.attachRootComponentToElement = function (componentId, element) {
        this.attachComponentToElement(componentId, element);
    };
    BrowserRenderer.prototype.updateComponent = function (componentId, edits, editsOffset, editsLength, referenceFrames) {
        var element = this.childComponentLocations[componentId];
        if (!element) {
            throw new Error("No element is currently associated with component " + componentId);
        }
        this.applyEdits(componentId, element, 0, edits, editsOffset, editsLength, referenceFrames);
    };
    BrowserRenderer.prototype.disposeComponent = function (componentId) {
        delete this.childComponentLocations[componentId];
    };
    BrowserRenderer.prototype.disposeEventHandler = function (eventHandlerId) {
        this.eventDelegator.removeListener(eventHandlerId);
    };
    BrowserRenderer.prototype.attachComponentToElement = function (componentId, element) {
        this.childComponentLocations[componentId] = element;
    };
    BrowserRenderer.prototype.applyEdits = function (componentId, parent, childIndex, edits, editsOffset, editsLength, referenceFrames) {
        var currentDepth = 0;
        var childIndexAtCurrentDepth = childIndex;
        var maxEditIndexExcl = editsOffset + editsLength;
        for (var editIndex = editsOffset; editIndex < maxEditIndexExcl; editIndex++) {
            var edit = RenderTreeEdit_1.getRenderTreeEditPtr(edits, editIndex);
            var editType = RenderTreeEdit_1.renderTreeEdit.type(edit);
            switch (editType) {
                case RenderTreeEdit_1.EditType.prependFrame: {
                    var frameIndex = RenderTreeEdit_1.renderTreeEdit.newTreeIndex(edit);
                    var frame = RenderTreeFrame_1.getTreeFramePtr(referenceFrames, frameIndex);
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    this.insertFrame(componentId, parent, childIndexAtCurrentDepth + siblingIndex, referenceFrames, frame, frameIndex);
                    break;
                }
                case RenderTreeEdit_1.EditType.removeFrame: {
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    removeNodeFromDOM(parent, childIndexAtCurrentDepth + siblingIndex);
                    break;
                }
                case RenderTreeEdit_1.EditType.setAttribute: {
                    var frameIndex = RenderTreeEdit_1.renderTreeEdit.newTreeIndex(edit);
                    var frame = RenderTreeFrame_1.getTreeFramePtr(referenceFrames, frameIndex);
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    var element = parent.childNodes[childIndexAtCurrentDepth + siblingIndex];
                    this.applyAttribute(componentId, element, frame);
                    break;
                }
                case RenderTreeEdit_1.EditType.removeAttribute: {
                    // Note that we don't have to dispose the info we track about event handlers here, because the
                    // disposed event handler IDs are delivered separately (in the 'disposedEventHandlerIds' array)
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    removeAttributeFromDOM(parent, childIndexAtCurrentDepth + siblingIndex, RenderTreeEdit_1.renderTreeEdit.removedAttributeName(edit));
                    break;
                }
                case RenderTreeEdit_1.EditType.updateText: {
                    var frameIndex = RenderTreeEdit_1.renderTreeEdit.newTreeIndex(edit);
                    var frame = RenderTreeFrame_1.getTreeFramePtr(referenceFrames, frameIndex);
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    var domTextNode = parent.childNodes[childIndexAtCurrentDepth + siblingIndex];
                    domTextNode.textContent = RenderTreeFrame_1.renderTreeFrame.textContent(frame);
                    break;
                }
                case RenderTreeEdit_1.EditType.stepIn: {
                    var siblingIndex = RenderTreeEdit_1.renderTreeEdit.siblingIndex(edit);
                    parent = parent.childNodes[childIndexAtCurrentDepth + siblingIndex];
                    currentDepth++;
                    childIndexAtCurrentDepth = 0;
                    break;
                }
                case RenderTreeEdit_1.EditType.stepOut: {
                    parent = parent.parentElement;
                    currentDepth--;
                    childIndexAtCurrentDepth = currentDepth === 0 ? childIndex : 0; // The childIndex is only ever nonzero at zero depth
                    break;
                }
                default: {
                    var unknownType = editType; // Compile-time verification that the switch was exhaustive
                    throw new Error("Unknown edit type: " + unknownType);
                }
            }
        }
    };
    BrowserRenderer.prototype.insertFrame = function (componentId, parent, childIndex, frames, frame, frameIndex) {
        var frameType = RenderTreeFrame_1.renderTreeFrame.frameType(frame);
        switch (frameType) {
            case RenderTreeFrame_1.FrameType.element:
                this.insertElement(componentId, parent, childIndex, frames, frame, frameIndex);
                return 1;
            case RenderTreeFrame_1.FrameType.text:
                this.insertText(parent, childIndex, frame);
                return 1;
            case RenderTreeFrame_1.FrameType.attribute:
                throw new Error('Attribute frames should only be present as leading children of element frames.');
            case RenderTreeFrame_1.FrameType.component:
                this.insertComponent(parent, childIndex, frame);
                return 1;
            case RenderTreeFrame_1.FrameType.region:
                return this.insertFrameRange(componentId, parent, childIndex, frames, frameIndex + 1, frameIndex + RenderTreeFrame_1.renderTreeFrame.subtreeLength(frame));
            default:
                var unknownType = frameType; // Compile-time verification that the switch was exhaustive
                throw new Error("Unknown frame type: " + unknownType);
        }
    };
    BrowserRenderer.prototype.insertElement = function (componentId, parent, childIndex, frames, frame, frameIndex) {
        var tagName = RenderTreeFrame_1.renderTreeFrame.elementName(frame);
        var newDomElement = tagName === 'svg' || parent.namespaceURI === 'http://www.w3.org/2000/svg' ?
            document.createElementNS('http://www.w3.org/2000/svg', tagName) :
            document.createElement(tagName);
        insertNodeIntoDOM(newDomElement, parent, childIndex);
        // Apply attributes
        var descendantsEndIndexExcl = frameIndex + RenderTreeFrame_1.renderTreeFrame.subtreeLength(frame);
        for (var descendantIndex = frameIndex + 1; descendantIndex < descendantsEndIndexExcl; descendantIndex++) {
            var descendantFrame = RenderTreeFrame_1.getTreeFramePtr(frames, descendantIndex);
            if (RenderTreeFrame_1.renderTreeFrame.frameType(descendantFrame) === RenderTreeFrame_1.FrameType.attribute) {
                this.applyAttribute(componentId, newDomElement, descendantFrame);
            }
            else {
                // As soon as we see a non-attribute child, all the subsequent child frames are
                // not attributes, so bail out and insert the remnants recursively
                this.insertFrameRange(componentId, newDomElement, 0, frames, descendantIndex, descendantsEndIndexExcl);
                break;
            }
        }
    };
    BrowserRenderer.prototype.insertComponent = function (parent, childIndex, frame) {
        // Currently, to support O(1) lookups from render tree frames to DOM nodes, we rely on
        // each child component existing as a single top-level element in the DOM. To guarantee
        // that, we wrap child components in these 'blazor-component' wrappers.
        // To improve on this in the future:
        // - If we can statically detect that a given component always produces a single top-level
        //   element anyway, then don't wrap it in a further nonstandard element
        // - If we really want to support child components producing multiple top-level frames and
        //   not being wrapped in a container at all, then every time a component is refreshed in
        //   the DOM, we could update an array on the parent element that specifies how many DOM
        //   nodes correspond to each of its render tree frames. Then when that parent wants to
        //   locate the first DOM node for a render tree frame, it can sum all the frame counts for
        //   all the preceding render trees frames. It's O(N), but where N is the number of siblings
        //   (counting child components as a single item), so N will rarely if ever be large.
        //   We could even keep track of whether all the child components happen to have exactly 1
        //   top level frames, and in that case, there's no need to sum as we can do direct lookups.
        var containerElement = parent.namespaceURI === 'http://www.w3.org/2000/svg' ?
            document.createElementNS('http://www.w3.org/2000/svg', 'g') :
            document.createElement('blazor-component');
        insertNodeIntoDOM(containerElement, parent, childIndex);
        // All we have to do is associate the child component ID with its location. We don't actually
        // do any rendering here, because the diff for the child will appear later in the render batch.
        var childComponentId = RenderTreeFrame_1.renderTreeFrame.componentId(frame);
        this.attachComponentToElement(childComponentId, containerElement);
    };
    BrowserRenderer.prototype.insertText = function (parent, childIndex, textFrame) {
        var textContent = RenderTreeFrame_1.renderTreeFrame.textContent(textFrame);
        var newDomTextNode = document.createTextNode(textContent);
        insertNodeIntoDOM(newDomTextNode, parent, childIndex);
    };
    BrowserRenderer.prototype.applyAttribute = function (componentId, toDomElement, attributeFrame) {
        var attributeName = RenderTreeFrame_1.renderTreeFrame.attributeName(attributeFrame);
        var browserRendererId = this.browserRendererId;
        var eventHandlerId = RenderTreeFrame_1.renderTreeFrame.attributeEventHandlerId(attributeFrame);
        if (eventHandlerId) {
            var firstTwoChars = attributeName.substring(0, 2);
            var eventName = attributeName.substring(2);
            if (firstTwoChars !== 'on' || !eventName) {
                throw new Error("Attribute has nonzero event handler ID, but attribute name '" + attributeName + "' does not start with 'on'.");
            }
            this.eventDelegator.setListener(toDomElement, eventName, componentId, eventHandlerId);
            return;
        }
        if (attributeName === 'value') {
            if (this.tryApplyValueProperty(toDomElement, RenderTreeFrame_1.renderTreeFrame.attributeValue(attributeFrame))) {
                return; // If this DOM element type has special 'value' handling, don't also write it as an attribute
            }
        }
        // Treat as a regular string-valued attribute
        toDomElement.setAttribute(attributeName, RenderTreeFrame_1.renderTreeFrame.attributeValue(attributeFrame));
    };
    BrowserRenderer.prototype.tryApplyValueProperty = function (element, value) {
        // Certain elements have built-in behaviour for their 'value' property
        switch (element.tagName) {
            case 'INPUT':
            case 'SELECT':
            case 'TEXTAREA':
                if (isCheckbox(element)) {
                    element.checked = value === '';
                }
                else {
                    element.value = value;
                    if (element.tagName === 'SELECT') {
                        // <select> is special, in that anything we write to .value will be lost if there
                        // isn't yet a matching <option>. To maintain the expected behavior no matter the
                        // element insertion/update order, preserve the desired value separately so
                        // we can recover it when inserting any matching <option>.
                        element[selectValuePropname] = value;
                    }
                }
                return true;
            case 'OPTION':
                element.setAttribute('value', value);
                // See above for why we have this special handling for <select>/<option>
                var parentElement = element.parentElement;
                if (parentElement && (selectValuePropname in parentElement) && parentElement[selectValuePropname] === value) {
                    this.tryApplyValueProperty(parentElement, value);
                    delete parentElement[selectValuePropname];
                }
                return true;
            default:
                return false;
        }
    };
    BrowserRenderer.prototype.insertFrameRange = function (componentId, parent, childIndex, frames, startIndex, endIndexExcl) {
        var origChildIndex = childIndex;
        for (var index = startIndex; index < endIndexExcl; index++) {
            var frame = RenderTreeFrame_1.getTreeFramePtr(frames, index);
            var numChildrenInserted = this.insertFrame(componentId, parent, childIndex, frames, frame, index);
            childIndex += numChildrenInserted;
            // Skip over any descendants, since they are already dealt with recursively
            var subtreeLength = RenderTreeFrame_1.renderTreeFrame.subtreeLength(frame);
            if (subtreeLength > 1) {
                index += subtreeLength - 1;
            }
        }
        return (childIndex - origChildIndex); // Total number of children inserted
    };
    return BrowserRenderer;
}());
exports.BrowserRenderer = BrowserRenderer;
function isCheckbox(element) {
    return element.tagName === 'INPUT' && element.getAttribute('type') === 'checkbox';
}
function insertNodeIntoDOM(node, parent, childIndex) {
    if (childIndex >= parent.childNodes.length) {
        parent.appendChild(node);
    }
    else {
        parent.insertBefore(node, parent.childNodes[childIndex]);
    }
}
function removeNodeFromDOM(parent, childIndex) {
    parent.removeChild(parent.childNodes[childIndex]);
}
function removeAttributeFromDOM(parent, childIndex, attributeName) {
    var element = parent.childNodes[childIndex];
    element.removeAttribute(attributeName);
}
function raiseEvent(event, browserRendererId, componentId, eventHandlerId, eventArgs) {
    event.preventDefault();
    if (!raiseEventMethod) {
        raiseEventMethod = Environment_1.platform.findMethod('Microsoft.AspNetCore.Blazor.Browser', 'Microsoft.AspNetCore.Blazor.Browser.Rendering', 'BrowserRendererEventDispatcher', 'DispatchEvent');
    }
    var eventDescriptor = {
        BrowserRendererId: browserRendererId,
        ComponentId: componentId,
        EventHandlerId: eventHandlerId,
        EventArgsType: eventArgs.type
    };
    Environment_1.platform.callMethod(raiseEventMethod, null, [
        Environment_1.platform.toDotNetString(JSON.stringify(eventDescriptor)),
        Environment_1.platform.toDotNetString(JSON.stringify(eventArgs.data))
    ]);
}


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var renderTreeEditStructLength = 16;
function getRenderTreeEditPtr(renderTreeEdits, index) {
    return Environment_1.platform.getArrayEntryPtr(renderTreeEdits, index, renderTreeEditStructLength);
}
exports.getRenderTreeEditPtr = getRenderTreeEditPtr;
exports.renderTreeEdit = {
    // The properties and memory layout must be kept in sync with the .NET equivalent in RenderTreeEdit.cs
    type: function (edit) { return Environment_1.platform.readInt32Field(edit, 0); },
    siblingIndex: function (edit) { return Environment_1.platform.readInt32Field(edit, 4); },
    newTreeIndex: function (edit) { return Environment_1.platform.readInt32Field(edit, 8); },
    removedAttributeName: function (edit) { return Environment_1.platform.readStringField(edit, 12); },
};
var EditType;
(function (EditType) {
    EditType[EditType["prependFrame"] = 1] = "prependFrame";
    EditType[EditType["removeFrame"] = 2] = "removeFrame";
    EditType[EditType["setAttribute"] = 3] = "setAttribute";
    EditType[EditType["removeAttribute"] = 4] = "removeAttribute";
    EditType[EditType["updateText"] = 5] = "updateText";
    EditType[EditType["stepIn"] = 6] = "stepIn";
    EditType[EditType["stepOut"] = 7] = "stepOut";
})(EditType = exports.EditType || (exports.EditType = {}));


/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var renderTreeFrameStructLength = 28;
// To minimise GC pressure, instead of instantiating a JS object to represent each tree frame,
// we work in terms of pointers to the structs on the .NET heap, and use static functions that
// know how to read property values from those structs.
function getTreeFramePtr(renderTreeEntries, index) {
    return Environment_1.platform.getArrayEntryPtr(renderTreeEntries, index, renderTreeFrameStructLength);
}
exports.getTreeFramePtr = getTreeFramePtr;
exports.renderTreeFrame = {
    // The properties and memory layout must be kept in sync with the .NET equivalent in RenderTreeFrame.cs
    frameType: function (frame) { return Environment_1.platform.readInt32Field(frame, 4); },
    subtreeLength: function (frame) { return Environment_1.platform.readInt32Field(frame, 8); },
    componentId: function (frame) { return Environment_1.platform.readInt32Field(frame, 12); },
    elementName: function (frame) { return Environment_1.platform.readStringField(frame, 16); },
    textContent: function (frame) { return Environment_1.platform.readStringField(frame, 16); },
    attributeName: function (frame) { return Environment_1.platform.readStringField(frame, 16); },
    attributeValue: function (frame) { return Environment_1.platform.readStringField(frame, 24); },
    attributeEventHandlerId: function (frame) { return Environment_1.platform.readInt32Field(frame, 8); },
};
var FrameType;
(function (FrameType) {
    // The values must be kept in sync with the .NET equivalent in RenderTreeFrameType.cs
    FrameType[FrameType["element"] = 1] = "element";
    FrameType[FrameType["text"] = 2] = "text";
    FrameType[FrameType["attribute"] = 3] = "attribute";
    FrameType[FrameType["component"] = 4] = "component";
    FrameType[FrameType["region"] = 5] = "region";
})(FrameType = exports.FrameType || (exports.FrameType = {}));


/***/ }),
/* 13 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var EventForDotNet_1 = __webpack_require__(14);
// Responsible for adding/removing the eventInfo on an expando property on DOM elements, and
// calling an EventInfoStore that deals with registering/unregistering the underlying delegated
// event listeners as required (and also maps actual events back to the given callback).
var EventDelegator = /** @class */ (function () {
    function EventDelegator(onEvent) {
        this.onEvent = onEvent;
        var eventDelegatorId = ++EventDelegator.nextEventDelegatorId;
        this.eventsCollectionKey = "_blazorEvents_" + eventDelegatorId;
        this.eventInfoStore = new EventInfoStore(this.onGlobalEvent.bind(this));
    }
    EventDelegator.prototype.setListener = function (element, eventName, componentId, eventHandlerId) {
        // Ensure we have a place to store event info for this element
        var infoForElement = element[this.eventsCollectionKey];
        if (!infoForElement) {
            infoForElement = element[this.eventsCollectionKey] = {};
        }
        if (infoForElement.hasOwnProperty(eventName)) {
            // We can cheaply update the info on the existing object and don't need any other housekeeping
            var oldInfo = infoForElement[eventName];
            this.eventInfoStore.update(oldInfo.eventHandlerId, eventHandlerId);
        }
        else {
            // Go through the whole flow which might involve registering a new global handler
            var newInfo = { element: element, eventName: eventName, componentId: componentId, eventHandlerId: eventHandlerId };
            this.eventInfoStore.add(newInfo);
            infoForElement[eventName] = newInfo;
        }
    };
    EventDelegator.prototype.removeListener = function (eventHandlerId) {
        // This method gets called whenever the .NET-side code reports that a certain event handler
        // has been disposed. However we will already have disposed the info about that handler if
        // the eventHandlerId for the (element,eventName) pair was replaced during diff application.
        var info = this.eventInfoStore.remove(eventHandlerId);
        if (info) {
            // Looks like this event handler wasn't already disposed
            // Remove the associated data from the DOM element
            var element = info.element;
            if (element.hasOwnProperty(this.eventsCollectionKey)) {
                var elementEventInfos = element[this.eventsCollectionKey];
                delete elementEventInfos[info.eventName];
                if (Object.getOwnPropertyNames(elementEventInfos).length === 0) {
                    delete element[this.eventsCollectionKey];
                }
            }
        }
    };
    EventDelegator.prototype.onGlobalEvent = function (evt) {
        if (!(evt.target instanceof Element)) {
            return;
        }
        // Scan up the element hierarchy, looking for any matching registered event handlers
        var candidateElement = evt.target;
        var eventArgs = null; // Populate lazily
        while (candidateElement) {
            if (candidateElement.hasOwnProperty(this.eventsCollectionKey)) {
                var handlerInfos = candidateElement[this.eventsCollectionKey];
                if (handlerInfos.hasOwnProperty(evt.type)) {
                    // We are going to raise an event for this element, so prepare info needed by the .NET code
                    if (!eventArgs) {
                        eventArgs = EventForDotNet_1.EventForDotNet.fromDOMEvent(evt);
                    }
                    var handlerInfo = handlerInfos[evt.type];
                    this.onEvent(evt, handlerInfo.componentId, handlerInfo.eventHandlerId, eventArgs);
                }
            }
            candidateElement = candidateElement.parentElement;
        }
    };
    EventDelegator.nextEventDelegatorId = 0;
    return EventDelegator;
}());
exports.EventDelegator = EventDelegator;
// Responsible for adding and removing the global listener when the number of listeners
// for a given event name changes between zero and nonzero
var EventInfoStore = /** @class */ (function () {
    function EventInfoStore(globalListener) {
        this.globalListener = globalListener;
        this.infosByEventHandlerId = {};
        this.countByEventName = {};
    }
    EventInfoStore.prototype.add = function (info) {
        if (this.infosByEventHandlerId[info.eventHandlerId]) {
            // Should never happen, but we want to know if it does
            throw new Error("Event " + info.eventHandlerId + " is already tracked");
        }
        this.infosByEventHandlerId[info.eventHandlerId] = info;
        var eventName = info.eventName;
        if (this.countByEventName.hasOwnProperty(eventName)) {
            this.countByEventName[eventName]++;
        }
        else {
            this.countByEventName[eventName] = 1;
            document.addEventListener(eventName, this.globalListener);
        }
    };
    EventInfoStore.prototype.update = function (oldEventHandlerId, newEventHandlerId) {
        if (this.infosByEventHandlerId.hasOwnProperty(newEventHandlerId)) {
            // Should never happen, but we want to know if it does
            throw new Error("Event " + newEventHandlerId + " is already tracked");
        }
        // Since we're just updating the event handler ID, there's no need to update the global counts
        var info = this.infosByEventHandlerId[oldEventHandlerId];
        delete this.infosByEventHandlerId[oldEventHandlerId];
        info.eventHandlerId = newEventHandlerId;
        this.infosByEventHandlerId[newEventHandlerId] = info;
    };
    EventInfoStore.prototype.remove = function (eventHandlerId) {
        var info = this.infosByEventHandlerId[eventHandlerId];
        if (info) {
            delete this.infosByEventHandlerId[eventHandlerId];
            var eventName = info.eventName;
            if (--this.countByEventName[eventName] === 0) {
                delete this.countByEventName[eventName];
                document.removeEventListener(eventName, this.globalListener);
            }
        }
        return info;
    };
    return EventInfoStore;
}());


/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var EventForDotNet = /** @class */ (function () {
    function EventForDotNet(type, data) {
        this.type = type;
        this.data = data;
    }
    EventForDotNet.fromDOMEvent = function (event) {
        var element = event.target;
        switch (event.type) {
            case 'click':
            case 'mousedown':
            case 'mouseup':
                return new EventForDotNet('mouse', { Type: event.type });
            case 'change': {
                var targetIsCheckbox = isCheckbox(element);
                var newValue = targetIsCheckbox ? !!element['checked'] : element['value'];
                return new EventForDotNet('change', { Type: event.type, Value: newValue });
            }
            case 'keypress':
                return new EventForDotNet('keyboard', { Type: event.type, Key: event.key });
            default:
                return new EventForDotNet('unknown', { Type: event.type });
        }
    };
    return EventForDotNet;
}());
exports.EventForDotNet = EventForDotNet;
function isCheckbox(element) {
    return element && element.tagName === 'INPUT' && element.getAttribute('type') === 'checkbox';
}


/***/ }),
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var RegisteredFunction_1 = __webpack_require__(1);
var Environment_1 = __webpack_require__(0);
var httpClientAssembly = 'Microsoft.AspNetCore.Blazor.Browser';
var httpClientNamespace = httpClientAssembly + ".Http";
var httpClientTypeName = 'BrowserHttpMessageHandler';
var httpClientFullTypeName = httpClientNamespace + "." + httpClientTypeName;
var receiveResponseMethod;
RegisteredFunction_1.registerFunction(httpClientFullTypeName + ".Send", function (id, method, requestUri, body, headersJson, fetchArgs) {
    sendAsync(id, method, requestUri, body, headersJson, fetchArgs);
});
function sendAsync(id, method, requestUri, body, headersJson, fetchArgs) {
    return __awaiter(this, void 0, void 0, function () {
        var response, responseText, requestInit, ex_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    requestInit = fetchArgs || {};
                    requestInit.method = method;
                    requestInit.body = body || undefined;
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 4, , 5]);
                    requestInit.headers = headersJson ? JSON.parse(headersJson) : undefined;
                    return [4 /*yield*/, fetch(requestUri, requestInit)];
                case 2:
                    response = _a.sent();
                    return [4 /*yield*/, response.text()];
                case 3:
                    responseText = _a.sent();
                    return [3 /*break*/, 5];
                case 4:
                    ex_1 = _a.sent();
                    dispatchErrorResponse(id, ex_1.toString());
                    return [2 /*return*/];
                case 5:
                    dispatchSuccessResponse(id, response, responseText);
                    return [2 /*return*/];
            }
        });
    });
}
function dispatchSuccessResponse(id, response, responseText) {
    var responseDescriptor = {
        StatusCode: response.status,
        Headers: []
    };
    response.headers.forEach(function (value, name) {
        responseDescriptor.Headers.push([name, value]);
    });
    dispatchResponse(id, Environment_1.platform.toDotNetString(JSON.stringify(responseDescriptor)), Environment_1.platform.toDotNetString(responseText), // TODO: Consider how to handle non-string responses
    /* errorMessage */ null);
}
function dispatchErrorResponse(id, errorMessage) {
    dispatchResponse(id, 
    /* responseDescriptor */ null, 
    /* responseText */ null, Environment_1.platform.toDotNetString(errorMessage));
}
function dispatchResponse(id, responseDescriptor, responseText, errorMessage) {
    if (!receiveResponseMethod) {
        receiveResponseMethod = Environment_1.platform.findMethod(httpClientAssembly, httpClientNamespace, httpClientTypeName, 'ReceiveResponse');
    }
    Environment_1.platform.callMethod(receiveResponseMethod, null, [
        Environment_1.platform.toDotNetString(id.toString()),
        responseDescriptor,
        responseText,
        errorMessage,
    ]);
}


/***/ }),
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var Environment_1 = __webpack_require__(0);
var RegisteredFunction_1 = __webpack_require__(1);
var UriHelper_1 = __webpack_require__(4);
if (typeof window !== 'undefined') {
    // When the library is loaded in a browser via a <script> element, make the
    // following APIs available in global scope for invocation from JS
    window['Blazor'] = {
        platform: Environment_1.platform,
        registerFunction: RegisteredFunction_1.registerFunction,
        navigateTo: UriHelper_1.navigateTo,
    };
}


/***/ })
/******/ ]);
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAgMGY3NzRiMjVlNjlhOWU1Yzg3NTYiLCJ3ZWJwYWNrOi8vLy4vc3JjL0Vudmlyb25tZW50LnRzIiwid2VicGFjazovLy8uL3NyYy9JbnRlcm9wL1JlZ2lzdGVyZWRGdW5jdGlvbi50cyIsIndlYnBhY2s6Ly8vLi9zcmMvUGxhdGZvcm0vRG90TmV0LnRzIiwid2VicGFjazovLy8uL3NyYy9SZW5kZXJpbmcvUmVuZGVyZXIudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL1NlcnZpY2VzL1VyaUhlbHBlci50cyIsIndlYnBhY2s6Ly8vLi9zcmMvQm9vdC50cyIsIndlYnBhY2s6Ly8vLi9zcmMvUGxhdGZvcm0vTW9uby9Nb25vUGxhdGZvcm0udHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL0ludGVyb3AvSW50ZXJuYWxSZWdpc3RlcmVkRnVuY3Rpb24udHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL0ludGVyb3AvSW52b2tlV2l0aEpzb25NYXJzaGFsbGluZy50cyIsIndlYnBhY2s6Ly8vLi9zcmMvUmVuZGVyaW5nL1JlbmRlckJhdGNoLnRzIiwid2VicGFjazovLy8uL3NyYy9SZW5kZXJpbmcvQnJvd3NlclJlbmRlcmVyLnRzIiwid2VicGFjazovLy8uL3NyYy9SZW5kZXJpbmcvUmVuZGVyVHJlZUVkaXQudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL1JlbmRlcmluZy9SZW5kZXJUcmVlRnJhbWUudHMiLCJ3ZWJwYWNrOi8vLy4vc3JjL1JlbmRlcmluZy9FdmVudERlbGVnYXRvci50cyIsIndlYnBhY2s6Ly8vLi9zcmMvUmVuZGVyaW5nL0V2ZW50Rm9yRG90TmV0LnRzIiwid2VicGFjazovLy8uL3NyYy9TZXJ2aWNlcy9IdHRwLnRzIiwid2VicGFjazovLy8uL3NyYy9HbG9iYWxFeHBvcnRzLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFBQTtBQUNBOztBQUVBO0FBQ0E7O0FBRUE7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FB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QVU7S0FDWCxDQUFDO0FBQ0osQ0FBQyIsImZpbGUiOiJibGF6b3IuanMiLCJzb3VyY2VzQ29udGVudCI6WyIgXHQvLyBUaGUgbW9kdWxlIGNhY2hlXG4gXHR2YXIgaW5zdGFsbGVkTW9kdWxlcyA9IHt9O1xuXG4gXHQvLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuIFx0ZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXG4gXHRcdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuIFx0XHRpZihpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSkge1xuIFx0XHRcdHJldHVybiBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXS5leHBvcnRzO1xuIFx0XHR9XG4gXHRcdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG4gXHRcdHZhciBtb2R1bGUgPSBpbnN0YWxsZWRNb2R1bGVzW21vZHVsZUlkXSA9IHtcbiBcdFx0XHRpOiBtb2R1bGVJZCxcbiBcdFx0XHRsOiBmYWxzZSxcbiBcdFx0XHRleHBvcnRzOiB7fVxuIFx0XHR9O1xuXG4gXHRcdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuIFx0XHRtb2R1bGVzW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuIFx0XHQvLyBGbGFnIHRoZSBtb2R1bGUgYXMgbG9hZGVkXG4gXHRcdG1vZHVsZS5sID0gdHJ1ZTtcblxuIFx0XHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuIFx0XHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG4gXHR9XG5cblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGVzIG9iamVjdCAoX193ZWJwYWNrX21vZHVsZXNfXylcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubSA9IG1vZHVsZXM7XG5cbiBcdC8vIGV4cG9zZSB0aGUgbW9kdWxlIGNhY2hlXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLmMgPSBpbnN0YWxsZWRNb2R1bGVzO1xuXG4gXHQvLyBkZWZpbmUgZ2V0dGVyIGZ1bmN0aW9uIGZvciBoYXJtb255IGV4cG9ydHNcbiBcdF9fd2VicGFja19yZXF1aXJlX18uZCA9IGZ1bmN0aW9uKGV4cG9ydHMsIG5hbWUsIGdldHRlcikge1xuIFx0XHRpZighX193ZWJwYWNrX3JlcXVpcmVfXy5vKGV4cG9ydHMsIG5hbWUpKSB7XG4gXHRcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KGV4cG9ydHMsIG5hbWUsIHtcbiBcdFx0XHRcdGNvbmZpZ3VyYWJsZTogZmFsc2UsXG4gXHRcdFx0XHRlbnVtZXJhYmxlOiB0cnVlLFxuIFx0XHRcdFx0Z2V0OiBnZXR0ZXJcbiBcdFx0XHR9KTtcbiBcdFx0fVxuIFx0fTtcblxuIFx0Ly8gZ2V0RGVmYXVsdEV4cG9ydCBmdW5jdGlvbiBmb3IgY29tcGF0aWJpbGl0eSB3aXRoIG5vbi1oYXJtb255IG1vZHVsZXNcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubiA9IGZ1bmN0aW9uKG1vZHVsZSkge1xuIFx0XHR2YXIgZ2V0dGVyID0gbW9kdWxlICYmIG1vZHVsZS5fX2VzTW9kdWxlID9cbiBcdFx0XHRmdW5jdGlvbiBnZXREZWZhdWx0KCkgeyByZXR1cm4gbW9kdWxlWydkZWZhdWx0J107IH0gOlxuIFx0XHRcdGZ1bmN0aW9uIGdldE1vZHVsZUV4cG9ydHMoKSB7IHJldHVybiBtb2R1bGU7IH07XG4gXHRcdF9fd2VicGFja19yZXF1aXJlX18uZChnZXR0ZXIsICdhJywgZ2V0dGVyKTtcbiBcdFx0cmV0dXJuIGdldHRlcjtcbiBcdH07XG5cbiBcdC8vIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbFxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5vID0gZnVuY3Rpb24ob2JqZWN0LCBwcm9wZXJ0eSkgeyByZXR1cm4gT2JqZWN0LnByb3RvdHlwZS5oYXNPd25Qcm9wZXJ0eS5jYWxsKG9iamVjdCwgcHJvcGVydHkpOyB9O1xuXG4gXHQvLyBfX3dlYnBhY2tfcHVibGljX3BhdGhfX1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5wID0gXCJcIjtcblxuIFx0Ly8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4gXHRyZXR1cm4gX193ZWJwYWNrX3JlcXVpcmVfXyhfX3dlYnBhY2tfcmVxdWlyZV9fLnMgPSA1KTtcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyB3ZWJwYWNrL2Jvb3RzdHJhcCAwZjc3NGIyNWU2OWE5ZTVjODc1NiIsIi8vIEV4cG9zZSBhbiBleHBvcnQgY2FsbGVkICdwbGF0Zm9ybScgb2YgdGhlIGludGVyZmFjZSB0eXBlICdQbGF0Zm9ybScsXHJcbi8vIHNvIHRoYXQgY29uc3VtZXJzIGNhbiBiZSBhZ25vc3RpYyBhYm91dCB3aGljaCBpbXBsZW1lbnRhdGlvbiB0aGV5IHVzZS5cclxuLy8gQmFzaWMgYWx0ZXJuYXRpdmUgdG8gaGF2aW5nIGFuIGFjdHVhbCBESSBjb250YWluZXIuXHJcbmltcG9ydCB7IFBsYXRmb3JtIH0gZnJvbSAnLi9QbGF0Zm9ybS9QbGF0Zm9ybSc7XHJcbmltcG9ydCB7IG1vbm9QbGF0Zm9ybSB9IGZyb20gJy4vUGxhdGZvcm0vTW9uby9Nb25vUGxhdGZvcm0nO1xyXG5leHBvcnQgY29uc3QgcGxhdGZvcm06IFBsYXRmb3JtID0gbW9ub1BsYXRmb3JtO1xyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvRW52aXJvbm1lbnQudHMiLCJpbXBvcnQgeyBpbnRlcm5hbFJlZ2lzdGVyZWRGdW5jdGlvbnMgfSBmcm9tICcuL0ludGVybmFsUmVnaXN0ZXJlZEZ1bmN0aW9uJztcclxuXHJcbmNvbnN0IHJlZ2lzdGVyZWRGdW5jdGlvbnM6IHsgW2lkZW50aWZpZXI6IHN0cmluZ106IEZ1bmN0aW9uIHwgdW5kZWZpbmVkIH0gPSB7fTtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiByZWdpc3RlckZ1bmN0aW9uKGlkZW50aWZpZXI6IHN0cmluZywgaW1wbGVtZW50YXRpb246IEZ1bmN0aW9uKSB7XHJcbiAgaWYgKGludGVybmFsUmVnaXN0ZXJlZEZ1bmN0aW9ucy5oYXNPd25Qcm9wZXJ0eShpZGVudGlmaWVyKSkge1xyXG4gICAgdGhyb3cgbmV3IEVycm9yKGBUaGUgZnVuY3Rpb24gaWRlbnRpZmllciAnJHtpZGVudGlmaWVyfScgaXMgcmVzZXJ2ZWQgYW5kIGNhbm5vdCBiZSByZWdpc3RlcmVkLmApO1xyXG4gIH1cclxuXHJcbiAgaWYgKHJlZ2lzdGVyZWRGdW5jdGlvbnMuaGFzT3duUHJvcGVydHkoaWRlbnRpZmllcikpIHtcclxuICAgIHRocm93IG5ldyBFcnJvcihgQSBmdW5jdGlvbiB3aXRoIHRoZSBpZGVudGlmaWVyICcke2lkZW50aWZpZXJ9JyBoYXMgYWxyZWFkeSBiZWVuIHJlZ2lzdGVyZWQuYCk7XHJcbiAgfVxyXG5cclxuICByZWdpc3RlcmVkRnVuY3Rpb25zW2lkZW50aWZpZXJdID0gaW1wbGVtZW50YXRpb247XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRSZWdpc3RlcmVkRnVuY3Rpb24oaWRlbnRpZmllcjogc3RyaW5nKTogRnVuY3Rpb24ge1xyXG4gIC8vIEJ5IHByaW9yaXRpc2luZyB0aGUgaW50ZXJuYWwgb25lcywgd2UgZW5zdXJlIHlvdSBjYW4ndCBvdmVycmlkZSB0aGVtXHJcbiAgY29uc3QgcmVzdWx0ID0gaW50ZXJuYWxSZWdpc3RlcmVkRnVuY3Rpb25zW2lkZW50aWZpZXJdIHx8IHJlZ2lzdGVyZWRGdW5jdGlvbnNbaWRlbnRpZmllcl07XHJcbiAgaWYgKHJlc3VsdCkge1xyXG4gICAgcmV0dXJuIHJlc3VsdDtcclxuICB9IGVsc2Uge1xyXG4gICAgdGhyb3cgbmV3IEVycm9yKGBDb3VsZCBub3QgZmluZCByZWdpc3RlcmVkIGZ1bmN0aW9uIHdpdGggbmFtZSAnJHtpZGVudGlmaWVyfScuYCk7XHJcbiAgfVxyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9JbnRlcm9wL1JlZ2lzdGVyZWRGdW5jdGlvbi50cyIsImV4cG9ydCBmdW5jdGlvbiBnZXRBc3NlbWJseU5hbWVGcm9tVXJsKHVybDogc3RyaW5nKSB7XHJcbiAgY29uc3QgbGFzdFNlZ21lbnQgPSB1cmwuc3Vic3RyaW5nKHVybC5sYXN0SW5kZXhPZignLycpICsgMSk7XHJcbiAgY29uc3QgcXVlcnlTdHJpbmdTdGFydFBvcyA9IGxhc3RTZWdtZW50LmluZGV4T2YoJz8nKTtcclxuICBjb25zdCBmaWxlbmFtZSA9IHF1ZXJ5U3RyaW5nU3RhcnRQb3MgPCAwID8gbGFzdFNlZ21lbnQgOiBsYXN0U2VnbWVudC5zdWJzdHJpbmcoMCwgcXVlcnlTdHJpbmdTdGFydFBvcyk7XHJcbiAgcmV0dXJuIGZpbGVuYW1lLnJlcGxhY2UoL1xcLmRsbCQvLCAnJyk7XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vc3JjL1BsYXRmb3JtL0RvdE5ldC50cyIsImltcG9ydCB7IFN5c3RlbV9PYmplY3QsIFN5c3RlbV9TdHJpbmcsIFN5c3RlbV9BcnJheSwgTWV0aG9kSGFuZGxlLCBQb2ludGVyIH0gZnJvbSAnLi4vUGxhdGZvcm0vUGxhdGZvcm0nO1xyXG5pbXBvcnQgeyBwbGF0Zm9ybSB9IGZyb20gJy4uL0Vudmlyb25tZW50JztcclxuaW1wb3J0IHsgcmVuZGVyQmF0Y2ggYXMgcmVuZGVyQmF0Y2hTdHJ1Y3QsIGFycmF5UmFuZ2UsIGFycmF5U2VnbWVudCwgcmVuZGVyVHJlZURpZmZTdHJ1Y3RMZW5ndGgsIHJlbmRlclRyZWVEaWZmLCBSZW5kZXJCYXRjaFBvaW50ZXIsIFJlbmRlclRyZWVEaWZmUG9pbnRlciB9IGZyb20gJy4vUmVuZGVyQmF0Y2gnO1xyXG5pbXBvcnQgeyBCcm93c2VyUmVuZGVyZXIgfSBmcm9tICcuL0Jyb3dzZXJSZW5kZXJlcic7XHJcblxyXG50eXBlIEJyb3dzZXJSZW5kZXJlclJlZ2lzdHJ5ID0geyBbYnJvd3NlclJlbmRlcmVySWQ6IG51bWJlcl06IEJyb3dzZXJSZW5kZXJlciB9O1xyXG5jb25zdCBicm93c2VyUmVuZGVyZXJzOiBCcm93c2VyUmVuZGVyZXJSZWdpc3RyeSA9IHt9O1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGF0dGFjaFJvb3RDb21wb25lbnRUb0VsZW1lbnQoYnJvd3NlclJlbmRlcmVySWQ6IG51bWJlciwgZWxlbWVudFNlbGVjdG9yOiBTeXN0ZW1fU3RyaW5nLCBjb21wb25lbnRJZDogbnVtYmVyKSB7XHJcbiAgY29uc3QgZWxlbWVudFNlbGVjdG9ySnMgPSBwbGF0Zm9ybS50b0phdmFTY3JpcHRTdHJpbmcoZWxlbWVudFNlbGVjdG9yKTtcclxuICBjb25zdCBlbGVtZW50ID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcihlbGVtZW50U2VsZWN0b3JKcyk7XHJcbiAgaWYgKCFlbGVtZW50KSB7XHJcbiAgICB0aHJvdyBuZXcgRXJyb3IoYENvdWxkIG5vdCBmaW5kIGFueSBlbGVtZW50IG1hdGNoaW5nIHNlbGVjdG9yICcke2VsZW1lbnRTZWxlY3RvckpzfScuYCk7XHJcbiAgfVxyXG5cclxuICBsZXQgYnJvd3NlclJlbmRlcmVyID0gYnJvd3NlclJlbmRlcmVyc1ticm93c2VyUmVuZGVyZXJJZF07XHJcbiAgaWYgKCFicm93c2VyUmVuZGVyZXIpIHtcclxuICAgIGJyb3dzZXJSZW5kZXJlciA9IGJyb3dzZXJSZW5kZXJlcnNbYnJvd3NlclJlbmRlcmVySWRdID0gbmV3IEJyb3dzZXJSZW5kZXJlcihicm93c2VyUmVuZGVyZXJJZCk7XHJcbiAgfVxyXG4gIGJyb3dzZXJSZW5kZXJlci5hdHRhY2hSb290Q29tcG9uZW50VG9FbGVtZW50KGNvbXBvbmVudElkLCBlbGVtZW50KTtcclxuICBjbGVhckVsZW1lbnQoZWxlbWVudCk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiByZW5kZXJCYXRjaChicm93c2VyUmVuZGVyZXJJZDogbnVtYmVyLCBiYXRjaDogUmVuZGVyQmF0Y2hQb2ludGVyKSB7XHJcbiAgY29uc3QgYnJvd3NlclJlbmRlcmVyID0gYnJvd3NlclJlbmRlcmVyc1ticm93c2VyUmVuZGVyZXJJZF07XHJcbiAgaWYgKCFicm93c2VyUmVuZGVyZXIpIHtcclxuICAgIHRocm93IG5ldyBFcnJvcihgVGhlcmUgaXMgbm8gYnJvd3NlciByZW5kZXJlciB3aXRoIElEICR7YnJvd3NlclJlbmRlcmVySWR9LmApO1xyXG4gIH1cclxuICBcclxuICBjb25zdCB1cGRhdGVkQ29tcG9uZW50cyA9IHJlbmRlckJhdGNoU3RydWN0LnVwZGF0ZWRDb21wb25lbnRzKGJhdGNoKTtcclxuICBjb25zdCB1cGRhdGVkQ29tcG9uZW50c0xlbmd0aCA9IGFycmF5UmFuZ2UuY291bnQodXBkYXRlZENvbXBvbmVudHMpO1xyXG4gIGNvbnN0IHVwZGF0ZWRDb21wb25lbnRzQXJyYXkgPSBhcnJheVJhbmdlLmFycmF5KHVwZGF0ZWRDb21wb25lbnRzKTtcclxuICBjb25zdCByZWZlcmVuY2VGcmFtZXNTdHJ1Y3QgPSByZW5kZXJCYXRjaFN0cnVjdC5yZWZlcmVuY2VGcmFtZXMoYmF0Y2gpO1xyXG4gIGNvbnN0IHJlZmVyZW5jZUZyYW1lcyA9IGFycmF5UmFuZ2UuYXJyYXkocmVmZXJlbmNlRnJhbWVzU3RydWN0KTtcclxuXHJcbiAgZm9yIChsZXQgaSA9IDA7IGkgPCB1cGRhdGVkQ29tcG9uZW50c0xlbmd0aDsgaSsrKSB7XHJcbiAgICBjb25zdCBkaWZmID0gcGxhdGZvcm0uZ2V0QXJyYXlFbnRyeVB0cih1cGRhdGVkQ29tcG9uZW50c0FycmF5LCBpLCByZW5kZXJUcmVlRGlmZlN0cnVjdExlbmd0aCk7XHJcbiAgICBjb25zdCBjb21wb25lbnRJZCA9IHJlbmRlclRyZWVEaWZmLmNvbXBvbmVudElkKGRpZmYpO1xyXG5cclxuICAgIGNvbnN0IGVkaXRzQXJyYXlTZWdtZW50ID0gcmVuZGVyVHJlZURpZmYuZWRpdHMoZGlmZik7XHJcbiAgICBjb25zdCBlZGl0cyA9IGFycmF5U2VnbWVudC5hcnJheShlZGl0c0FycmF5U2VnbWVudCk7XHJcbiAgICBjb25zdCBlZGl0c09mZnNldCA9IGFycmF5U2VnbWVudC5vZmZzZXQoZWRpdHNBcnJheVNlZ21lbnQpO1xyXG4gICAgY29uc3QgZWRpdHNMZW5ndGggPSBhcnJheVNlZ21lbnQuY291bnQoZWRpdHNBcnJheVNlZ21lbnQpO1xyXG5cclxuICAgIGJyb3dzZXJSZW5kZXJlci51cGRhdGVDb21wb25lbnQoY29tcG9uZW50SWQsIGVkaXRzLCBlZGl0c09mZnNldCwgZWRpdHNMZW5ndGgsIHJlZmVyZW5jZUZyYW1lcyk7XHJcbiAgfVxyXG5cclxuICBjb25zdCBkaXNwb3NlZENvbXBvbmVudElkcyA9IHJlbmRlckJhdGNoU3RydWN0LmRpc3Bvc2VkQ29tcG9uZW50SWRzKGJhdGNoKTtcclxuICBjb25zdCBkaXNwb3NlZENvbXBvbmVudElkc0xlbmd0aCA9IGFycmF5UmFuZ2UuY291bnQoZGlzcG9zZWRDb21wb25lbnRJZHMpO1xyXG4gIGNvbnN0IGRpc3Bvc2VkQ29tcG9uZW50SWRzQXJyYXkgPSBhcnJheVJhbmdlLmFycmF5KGRpc3Bvc2VkQ29tcG9uZW50SWRzKTtcclxuICBmb3IgKGxldCBpID0gMDsgaSA8IGRpc3Bvc2VkQ29tcG9uZW50SWRzTGVuZ3RoOyBpKyspIHtcclxuICAgIGNvbnN0IGNvbXBvbmVudElkUHRyID0gcGxhdGZvcm0uZ2V0QXJyYXlFbnRyeVB0cihkaXNwb3NlZENvbXBvbmVudElkc0FycmF5LCBpLCA0KTtcclxuICAgIGNvbnN0IGNvbXBvbmVudElkID0gcGxhdGZvcm0ucmVhZEludDMyRmllbGQoY29tcG9uZW50SWRQdHIpO1xyXG4gICAgYnJvd3NlclJlbmRlcmVyLmRpc3Bvc2VDb21wb25lbnQoY29tcG9uZW50SWQpO1xyXG4gIH1cclxuXHJcbiAgY29uc3QgZGlzcG9zZWRFdmVudEhhbmRsZXJJZHMgPSByZW5kZXJCYXRjaFN0cnVjdC5kaXNwb3NlZEV2ZW50SGFuZGxlcklkcyhiYXRjaCk7XHJcbiAgY29uc3QgZGlzcG9zZWRFdmVudEhhbmRsZXJJZHNMZW5ndGggPSBhcnJheVJhbmdlLmNvdW50KGRpc3Bvc2VkRXZlbnRIYW5kbGVySWRzKTtcclxuICBjb25zdCBkaXNwb3NlZEV2ZW50SGFuZGxlcklkc0FycmF5ID0gYXJyYXlSYW5nZS5hcnJheShkaXNwb3NlZEV2ZW50SGFuZGxlcklkcyk7XHJcbiAgZm9yIChsZXQgaSA9IDA7IGkgPCBkaXNwb3NlZEV2ZW50SGFuZGxlcklkc0xlbmd0aDsgaSsrKSB7XHJcbiAgICBjb25zdCBldmVudEhhbmRsZXJJZFB0ciA9IHBsYXRmb3JtLmdldEFycmF5RW50cnlQdHIoZGlzcG9zZWRFdmVudEhhbmRsZXJJZHNBcnJheSwgaSwgNCk7XHJcbiAgICBjb25zdCBldmVudEhhbmRsZXJJZCA9IHBsYXRmb3JtLnJlYWRJbnQzMkZpZWxkKGV2ZW50SGFuZGxlcklkUHRyKTtcclxuICAgIGJyb3dzZXJSZW5kZXJlci5kaXNwb3NlRXZlbnRIYW5kbGVyKGV2ZW50SGFuZGxlcklkKTtcclxuICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGNsZWFyRWxlbWVudChlbGVtZW50OiBFbGVtZW50KSB7XHJcbiAgbGV0IGNoaWxkTm9kZTogTm9kZSB8IG51bGw7XHJcbiAgd2hpbGUgKGNoaWxkTm9kZSA9IGVsZW1lbnQuZmlyc3RDaGlsZCkge1xyXG4gICAgZWxlbWVudC5yZW1vdmVDaGlsZChjaGlsZE5vZGUpO1xyXG4gIH1cclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvUmVuZGVyaW5nL1JlbmRlcmVyLnRzIiwiaW1wb3J0IHsgcmVnaXN0ZXJGdW5jdGlvbiB9IGZyb20gJy4uL0ludGVyb3AvUmVnaXN0ZXJlZEZ1bmN0aW9uJztcclxuaW1wb3J0IHsgcGxhdGZvcm0gfSBmcm9tICcuLi9FbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IE1ldGhvZEhhbmRsZSwgU3lzdGVtX1N0cmluZyB9IGZyb20gJy4uL1BsYXRmb3JtL1BsYXRmb3JtJztcclxuY29uc3QgcmVnaXN0ZXJlZEZ1bmN0aW9uUHJlZml4ID0gJ01pY3Jvc29mdC5Bc3BOZXRDb3JlLkJsYXpvci5Ccm93c2VyLlNlcnZpY2VzLkJyb3dzZXJVcmlIZWxwZXInO1xyXG5sZXQgbm90aWZ5TG9jYXRpb25DaGFuZ2VkTWV0aG9kOiBNZXRob2RIYW5kbGU7XHJcbmxldCBoYXNSZWdpc3RlcmVkRXZlbnRMaXN0ZW5lcnMgPSBmYWxzZTtcclxuXHJcbnJlZ2lzdGVyRnVuY3Rpb24oYCR7cmVnaXN0ZXJlZEZ1bmN0aW9uUHJlZml4fS5nZXRMb2NhdGlvbkhyZWZgLFxyXG4gICgpID0+IHBsYXRmb3JtLnRvRG90TmV0U3RyaW5nKGxvY2F0aW9uLmhyZWYpKTtcclxuXHJcbnJlZ2lzdGVyRnVuY3Rpb24oYCR7cmVnaXN0ZXJlZEZ1bmN0aW9uUHJlZml4fS5nZXRCYXNlVVJJYCxcclxuICAoKSA9PiBkb2N1bWVudC5iYXNlVVJJID8gcGxhdGZvcm0udG9Eb3ROZXRTdHJpbmcoZG9jdW1lbnQuYmFzZVVSSSkgOiBudWxsKTtcclxuXHJcbnJlZ2lzdGVyRnVuY3Rpb24oYCR7cmVnaXN0ZXJlZEZ1bmN0aW9uUHJlZml4fS5lbmFibGVOYXZpZ2F0aW9uSW50ZXJjZXB0aW9uYCwgKCkgPT4ge1xyXG4gIGlmIChoYXNSZWdpc3RlcmVkRXZlbnRMaXN0ZW5lcnMpIHtcclxuICAgIHJldHVybjtcclxuICB9XHJcbiAgaGFzUmVnaXN0ZXJlZEV2ZW50TGlzdGVuZXJzID0gdHJ1ZTtcclxuXHJcbiAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcignY2xpY2snLCBldmVudCA9PiB7XHJcbiAgICAvLyBJbnRlcmNlcHQgY2xpY2tzIG9uIGFsbCA8YT4gZWxlbWVudHMgd2hlcmUgdGhlIGhyZWYgaXMgd2l0aGluIHRoZSA8YmFzZSBocmVmPiBVUkkgc3BhY2VcclxuICAgIGNvbnN0IGFuY2hvclRhcmdldCA9IGZpbmRDbG9zZXN0QW5jZXN0b3IoZXZlbnQudGFyZ2V0IGFzIEVsZW1lbnQgfCBudWxsLCAnQScpO1xyXG4gICAgaWYgKGFuY2hvclRhcmdldCkge1xyXG4gICAgICBjb25zdCBocmVmID0gYW5jaG9yVGFyZ2V0LmdldEF0dHJpYnV0ZSgnaHJlZicpO1xyXG4gICAgICBpZiAoaXNXaXRoaW5CYXNlVXJpU3BhY2UodG9BYnNvbHV0ZVVyaShocmVmKSkpIHtcclxuICAgICAgICBldmVudC5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgIHBlcmZvcm1JbnRlcm5hbE5hdmlnYXRpb24oaHJlZik7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxuXHJcbiAgd2luZG93LmFkZEV2ZW50TGlzdGVuZXIoJ3BvcHN0YXRlJywgaGFuZGxlSW50ZXJuYWxOYXZpZ2F0aW9uKTtcclxufSk7XHJcblxyXG5yZWdpc3RlckZ1bmN0aW9uKGAke3JlZ2lzdGVyZWRGdW5jdGlvblByZWZpeH0ubmF2aWdhdGVUb2AsICh1cmlEb3ROZXRTdHJpbmc6IFN5c3RlbV9TdHJpbmcpID0+IHtcclxuICBuYXZpZ2F0ZVRvKHBsYXRmb3JtLnRvSmF2YVNjcmlwdFN0cmluZyh1cmlEb3ROZXRTdHJpbmcpKTtcclxufSk7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gbmF2aWdhdGVUbyh1cmk6IHN0cmluZykge1xyXG4gIGlmIChpc1dpdGhpbkJhc2VVcmlTcGFjZSh0b0Fic29sdXRlVXJpKHVyaSkpKSB7XHJcbiAgICBwZXJmb3JtSW50ZXJuYWxOYXZpZ2F0aW9uKHVyaSk7XHJcbiAgfSBlbHNlIHtcclxuICAgIGxvY2F0aW9uLmhyZWYgPSB1cmk7XHJcbiAgfVxyXG59XHJcblxyXG5mdW5jdGlvbiBwZXJmb3JtSW50ZXJuYWxOYXZpZ2F0aW9uKGhyZWY6IHN0cmluZykge1xyXG4gIGhpc3RvcnkucHVzaFN0YXRlKG51bGwsIC8qIGlnbm9yZWQgdGl0bGUgKi8gJycsIGhyZWYpO1xyXG4gIGhhbmRsZUludGVybmFsTmF2aWdhdGlvbigpO1xyXG59XHJcblxyXG5mdW5jdGlvbiBoYW5kbGVJbnRlcm5hbE5hdmlnYXRpb24oKSB7XHJcbiAgaWYgKCFub3RpZnlMb2NhdGlvbkNoYW5nZWRNZXRob2QpIHtcclxuICAgIG5vdGlmeUxvY2F0aW9uQ2hhbmdlZE1ldGhvZCA9IHBsYXRmb3JtLmZpbmRNZXRob2QoXHJcbiAgICAgICdNaWNyb3NvZnQuQXNwTmV0Q29yZS5CbGF6b3IuQnJvd3NlcicsXHJcbiAgICAgICdNaWNyb3NvZnQuQXNwTmV0Q29yZS5CbGF6b3IuQnJvd3Nlci5TZXJ2aWNlcycsXHJcbiAgICAgICdCcm93c2VyVXJpSGVscGVyJyxcclxuICAgICAgJ05vdGlmeUxvY2F0aW9uQ2hhbmdlZCdcclxuICAgICk7XHJcbiAgfVxyXG5cclxuICBwbGF0Zm9ybS5jYWxsTWV0aG9kKG5vdGlmeUxvY2F0aW9uQ2hhbmdlZE1ldGhvZCwgbnVsbCwgW1xyXG4gICAgcGxhdGZvcm0udG9Eb3ROZXRTdHJpbmcobG9jYXRpb24uaHJlZilcclxuICBdKTtcclxufVxyXG5cclxubGV0IHRlc3RBbmNob3I6IEhUTUxBbmNob3JFbGVtZW50O1xyXG5mdW5jdGlvbiB0b0Fic29sdXRlVXJpKHJlbGF0aXZlVXJpOiBzdHJpbmcpIHtcclxuICB0ZXN0QW5jaG9yID0gdGVzdEFuY2hvciB8fCBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdhJyk7XHJcbiAgdGVzdEFuY2hvci5ocmVmID0gcmVsYXRpdmVVcmk7XHJcbiAgcmV0dXJuIHRlc3RBbmNob3IuaHJlZjtcclxufVxyXG5cclxuZnVuY3Rpb24gZmluZENsb3Nlc3RBbmNlc3RvcihlbGVtZW50OiBFbGVtZW50IHwgbnVsbCwgdGFnTmFtZTogc3RyaW5nKSB7XHJcbiAgcmV0dXJuICFlbGVtZW50XHJcbiAgICA/IG51bGxcclxuICAgIDogZWxlbWVudC50YWdOYW1lID09PSB0YWdOYW1lXHJcbiAgICAgID8gZWxlbWVudFxyXG4gICAgICA6IGZpbmRDbG9zZXN0QW5jZXN0b3IoZWxlbWVudC5wYXJlbnRFbGVtZW50LCB0YWdOYW1lKVxyXG59XHJcblxyXG5mdW5jdGlvbiBpc1dpdGhpbkJhc2VVcmlTcGFjZShocmVmOiBzdHJpbmcpIHtcclxuICBjb25zdCBiYXNlVXJpUHJlZml4V2l0aFRyYWlsaW5nU2xhc2ggPSB0b0Jhc2VVcmlQcmVmaXhXaXRoVHJhaWxpbmdTbGFzaChkb2N1bWVudC5iYXNlVVJJISk7IC8vIFRPRE86IE1pZ2h0IGJhc2VVUkkgcmVhbGx5IGJlIG51bGw/XHJcbiAgcmV0dXJuIGhyZWYuc3RhcnRzV2l0aChiYXNlVXJpUHJlZml4V2l0aFRyYWlsaW5nU2xhc2gpO1xyXG59XHJcblxyXG5mdW5jdGlvbiB0b0Jhc2VVcmlQcmVmaXhXaXRoVHJhaWxpbmdTbGFzaChiYXNlVXJpOiBzdHJpbmcpIHtcclxuICByZXR1cm4gYmFzZVVyaS5zdWJzdHIoMCwgYmFzZVVyaS5sYXN0SW5kZXhPZignLycpICsgMSk7XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vc3JjL1NlcnZpY2VzL1VyaUhlbHBlci50cyIsImltcG9ydCB7IHBsYXRmb3JtIH0gZnJvbSAnLi9FbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IGdldEFzc2VtYmx5TmFtZUZyb21VcmwgfSBmcm9tICcuL1BsYXRmb3JtL0RvdE5ldCc7XHJcbmltcG9ydCAnLi9SZW5kZXJpbmcvUmVuZGVyZXInO1xyXG5pbXBvcnQgJy4vU2VydmljZXMvSHR0cCc7XHJcbmltcG9ydCAnLi9TZXJ2aWNlcy9VcmlIZWxwZXInO1xyXG5pbXBvcnQgJy4vR2xvYmFsRXhwb3J0cyc7XHJcblxyXG5hc3luYyBmdW5jdGlvbiBib290KCkge1xyXG4gIC8vIFJlYWQgc3RhcnR1cCBjb25maWcgZnJvbSB0aGUgPHNjcmlwdD4gZWxlbWVudCB0aGF0J3MgaW1wb3J0aW5nIHRoaXMgZmlsZVxyXG4gIGNvbnN0IGFsbFNjcmlwdEVsZW1zID0gZG9jdW1lbnQuZ2V0RWxlbWVudHNCeVRhZ05hbWUoJ3NjcmlwdCcpO1xyXG4gIGNvbnN0IHRoaXNTY3JpcHRFbGVtID0gKGRvY3VtZW50LmN1cnJlbnRTY3JpcHQgfHwgYWxsU2NyaXB0RWxlbXNbYWxsU2NyaXB0RWxlbXMubGVuZ3RoIC0gMV0pIGFzIEhUTUxTY3JpcHRFbGVtZW50O1xyXG4gIGNvbnN0IGlzTGlua2VyRW5hYmxlZCA9IHRoaXNTY3JpcHRFbGVtLmdldEF0dHJpYnV0ZSgnbGlua2VyLWVuYWJsZWQnKSA9PT0gJ3RydWUnO1xyXG4gIGNvbnN0IGVudHJ5UG9pbnREbGwgPSBnZXRSZXF1aXJlZEJvb3RTY3JpcHRBdHRyaWJ1dGUodGhpc1NjcmlwdEVsZW0sICdtYWluJyk7XHJcbiAgY29uc3QgZW50cnlQb2ludE1ldGhvZCA9IGdldFJlcXVpcmVkQm9vdFNjcmlwdEF0dHJpYnV0ZSh0aGlzU2NyaXB0RWxlbSwgJ2VudHJ5cG9pbnQnKTtcclxuICBjb25zdCBlbnRyeVBvaW50QXNzZW1ibHlOYW1lID0gZ2V0QXNzZW1ibHlOYW1lRnJvbVVybChlbnRyeVBvaW50RGxsKTtcclxuICBjb25zdCByZWZlcmVuY2VBc3NlbWJsaWVzQ29tbWFTZXBhcmF0ZWQgPSB0aGlzU2NyaXB0RWxlbS5nZXRBdHRyaWJ1dGUoJ3JlZmVyZW5jZXMnKSB8fCAnJztcclxuICBjb25zdCByZWZlcmVuY2VBc3NlbWJsaWVzID0gcmVmZXJlbmNlQXNzZW1ibGllc0NvbW1hU2VwYXJhdGVkXHJcbiAgICAuc3BsaXQoJywnKVxyXG4gICAgLm1hcChzID0+IHMudHJpbSgpKVxyXG4gICAgLmZpbHRlcihzID0+ICEhcyk7XHJcblxyXG4gIGlmICghaXNMaW5rZXJFbmFibGVkKSB7XHJcbiAgICBjb25zb2xlLmluZm8oJ0JsYXpvciBpcyBydW5uaW5nIGluIGRldiBtb2RlIHdpdGhvdXQgSUwgc3RyaXBwaW5nLiBUbyBtYWtlIHRoZSBidW5kbGUgc2l6ZSBzaWduaWZpY2FudGx5IHNtYWxsZXIsIHB1Ymxpc2ggdGhlIGFwcGxpY2F0aW9uIG9yIHNlZSBodHRwczovL2dvLm1pY3Jvc29mdC5jb20vZndsaW5rLz9saW5raWQ9ODcwNDE0Jyk7XHJcbiAgfVxyXG5cclxuICAvLyBEZXRlcm1pbmUgdGhlIFVSTHMgb2YgdGhlIGFzc2VtYmxpZXMgd2Ugd2FudCB0byBsb2FkXHJcbiAgY29uc3QgbG9hZEFzc2VtYmx5VXJscyA9IFtlbnRyeVBvaW50RGxsXVxyXG4gICAgLmNvbmNhdChyZWZlcmVuY2VBc3NlbWJsaWVzKVxyXG4gICAgLm1hcChmaWxlbmFtZSA9PiBgX2ZyYW1ld29yay9fYmluLyR7ZmlsZW5hbWV9YCk7XHJcblxyXG4gIHRyeSB7XHJcbiAgICBhd2FpdCBwbGF0Zm9ybS5zdGFydChsb2FkQXNzZW1ibHlVcmxzKTtcclxuICB9IGNhdGNoIChleCkge1xyXG4gICAgdGhyb3cgbmV3IEVycm9yKGBGYWlsZWQgdG8gc3RhcnQgcGxhdGZvcm0uIFJlYXNvbjogJHtleH1gKTtcclxuICB9XHJcblxyXG4gIC8vIFN0YXJ0IHVwIHRoZSBhcHBsaWNhdGlvblxyXG4gIHBsYXRmb3JtLmNhbGxFbnRyeVBvaW50KGVudHJ5UG9pbnRBc3NlbWJseU5hbWUsIGVudHJ5UG9pbnRNZXRob2QsIFtdKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZ2V0UmVxdWlyZWRCb290U2NyaXB0QXR0cmlidXRlKGVsZW06IEhUTUxTY3JpcHRFbGVtZW50LCBhdHRyaWJ1dGVOYW1lOiBzdHJpbmcpOiBzdHJpbmcge1xyXG4gIGNvbnN0IHJlc3VsdCA9IGVsZW0uZ2V0QXR0cmlidXRlKGF0dHJpYnV0ZU5hbWUpO1xyXG4gIGlmICghcmVzdWx0KSB7XHJcbiAgICB0aHJvdyBuZXcgRXJyb3IoYE1pc3NpbmcgXCIke2F0dHJpYnV0ZU5hbWV9XCIgYXR0cmlidXRlIG9uIEJsYXpvciBzY3JpcHQgdGFnLmApO1xyXG4gIH1cclxuICByZXR1cm4gcmVzdWx0O1xyXG59XHJcblxyXG5ib290KCk7XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9Cb290LnRzIiwiaW1wb3J0IHsgTWV0aG9kSGFuZGxlLCBTeXN0ZW1fT2JqZWN0LCBTeXN0ZW1fU3RyaW5nLCBTeXN0ZW1fQXJyYXksIFBvaW50ZXIsIFBsYXRmb3JtIH0gZnJvbSAnLi4vUGxhdGZvcm0nO1xyXG5pbXBvcnQgeyBnZXRBc3NlbWJseU5hbWVGcm9tVXJsIH0gZnJvbSAnLi4vRG90TmV0JztcclxuaW1wb3J0IHsgZ2V0UmVnaXN0ZXJlZEZ1bmN0aW9uIH0gZnJvbSAnLi4vLi4vSW50ZXJvcC9SZWdpc3RlcmVkRnVuY3Rpb24nO1xyXG5cclxuY29uc3QgYXNzZW1ibHlIYW5kbGVDYWNoZTogeyBbYXNzZW1ibHlOYW1lOiBzdHJpbmddOiBudW1iZXIgfSA9IHt9O1xyXG5jb25zdCB0eXBlSGFuZGxlQ2FjaGU6IHsgW2Z1bGx5UXVhbGlmaWVkVHlwZU5hbWU6IHN0cmluZ106IG51bWJlciB9ID0ge307XHJcbmNvbnN0IG1ldGhvZEhhbmRsZUNhY2hlOiB7IFtmdWxseVF1YWxpZmllZE1ldGhvZE5hbWU6IHN0cmluZ106IE1ldGhvZEhhbmRsZSB9ID0ge307XHJcblxyXG5sZXQgYXNzZW1ibHlfbG9hZDogKGFzc2VtYmx5TmFtZTogc3RyaW5nKSA9PiBudW1iZXI7XHJcbmxldCBmaW5kX2NsYXNzOiAoYXNzZW1ibHlIYW5kbGU6IG51bWJlciwgbmFtZXNwYWNlOiBzdHJpbmcsIGNsYXNzTmFtZTogc3RyaW5nKSA9PiBudW1iZXI7XHJcbmxldCBmaW5kX21ldGhvZDogKHR5cGVIYW5kbGU6IG51bWJlciwgbWV0aG9kTmFtZTogc3RyaW5nLCB1bmtub3duQXJnOiBudW1iZXIpID0+IE1ldGhvZEhhbmRsZTtcclxubGV0IGludm9rZV9tZXRob2Q6IChtZXRob2Q6IE1ldGhvZEhhbmRsZSwgdGFyZ2V0OiBTeXN0ZW1fT2JqZWN0LCBhcmdzQXJyYXlQdHI6IG51bWJlciwgZXhjZXB0aW9uRmxhZ0ludFB0cjogbnVtYmVyKSA9PiBTeXN0ZW1fT2JqZWN0O1xyXG5sZXQgbW9ub19zdHJpbmdfZ2V0X3V0Zjg6IChtYW5hZ2VkU3RyaW5nOiBTeXN0ZW1fU3RyaW5nKSA9PiBNb25vLlV0ZjhQdHI7XHJcbmxldCBtb25vX3N0cmluZzogKGpzU3RyaW5nOiBzdHJpbmcpID0+IFN5c3RlbV9TdHJpbmc7XHJcblxyXG5leHBvcnQgY29uc3QgbW9ub1BsYXRmb3JtOiBQbGF0Zm9ybSA9IHtcclxuICBzdGFydDogZnVuY3Rpb24gc3RhcnQobG9hZEFzc2VtYmx5VXJsczogc3RyaW5nW10pIHtcclxuICAgIHJldHVybiBuZXcgUHJvbWlzZTx2b2lkPigocmVzb2x2ZSwgcmVqZWN0KSA9PiB7XHJcbiAgICAgIC8vIG1vbm8uanMgYXNzdW1lcyB0aGUgZXhpc3RlbmNlIG9mIHRoaXNcclxuICAgICAgd2luZG93WydCcm93c2VyJ10gPSB7XHJcbiAgICAgICAgaW5pdDogKCkgPT4geyB9LFxyXG4gICAgICAgIGFzeW5jTG9hZDogYXN5bmNMb2FkXHJcbiAgICAgIH07XHJcbiAgICAgIC8vIEVtc2NyaXB0ZW4gd29ya3MgYnkgZXhwZWN0aW5nIHRoZSBtb2R1bGUgY29uZmlnIHRvIGJlIGEgZ2xvYmFsXHJcbiAgICAgIHdpbmRvd1snTW9kdWxlJ10gPSBjcmVhdGVFbXNjcmlwdGVuTW9kdWxlSW5zdGFuY2UobG9hZEFzc2VtYmx5VXJscywgcmVzb2x2ZSwgcmVqZWN0KTtcclxuXHJcbiAgICAgIGFkZFNjcmlwdFRhZ3NUb0RvY3VtZW50KCk7XHJcbiAgICB9KTtcclxuICB9LFxyXG5cclxuICBmaW5kTWV0aG9kOiBmaW5kTWV0aG9kLFxyXG5cclxuICBjYWxsRW50cnlQb2ludDogZnVuY3Rpb24gY2FsbEVudHJ5UG9pbnQoYXNzZW1ibHlOYW1lOiBzdHJpbmcsIGVudHJ5cG9pbnRNZXRob2Q6IHN0cmluZywgYXJnczogU3lzdGVtX09iamVjdFtdKTogdm9pZCB7XHJcbiAgICAvLyBQYXJzZSB0aGUgZW50cnlwb2ludE1ldGhvZCwgd2hpY2ggaXMgb2YgdGhlIGZvcm0gTXlBcHAuTXlOYW1lc3BhY2UuTXlUeXBlTmFtZTo6TXlNZXRob2ROYW1lXHJcbiAgICAvLyBOb3RlIHRoYXQgd2UgZG9uJ3Qgc3VwcG9ydCBzcGVjaWZ5aW5nIGEgbWV0aG9kIG92ZXJsb2FkLCBzbyBpdCBoYXMgdG8gYmUgdW5pcXVlXHJcbiAgICBjb25zdCBlbnRyeXBvaW50U2VnbWVudHMgPSBlbnRyeXBvaW50TWV0aG9kLnNwbGl0KCc6OicpO1xyXG4gICAgaWYgKGVudHJ5cG9pbnRTZWdtZW50cy5sZW5ndGggIT0gMikge1xyXG4gICAgICB0aHJvdyBuZXcgRXJyb3IoJ01hbGZvcm1lZCBlbnRyeSBwb2ludCBtZXRob2QgbmFtZTsgY291bGQgbm90IHJlc29sdmUgY2xhc3MgbmFtZSBhbmQgbWV0aG9kIG5hbWUuJyk7XHJcbiAgICB9XHJcbiAgICBjb25zdCB0eXBlRnVsbE5hbWUgPSBlbnRyeXBvaW50U2VnbWVudHNbMF07XHJcbiAgICBjb25zdCBtZXRob2ROYW1lID0gZW50cnlwb2ludFNlZ21lbnRzWzFdO1xyXG4gICAgY29uc3QgbGFzdERvdCA9IHR5cGVGdWxsTmFtZS5sYXN0SW5kZXhPZignLicpO1xyXG4gICAgY29uc3QgbmFtZXNwYWNlID0gbGFzdERvdCA+IC0xID8gdHlwZUZ1bGxOYW1lLnN1YnN0cmluZygwLCBsYXN0RG90KSA6ICcnO1xyXG4gICAgY29uc3QgdHlwZVNob3J0TmFtZSA9IGxhc3REb3QgPiAtMSA/IHR5cGVGdWxsTmFtZS5zdWJzdHJpbmcobGFzdERvdCArIDEpIDogdHlwZUZ1bGxOYW1lO1xyXG5cclxuICAgIGNvbnN0IGVudHJ5UG9pbnRNZXRob2RIYW5kbGUgPSBtb25vUGxhdGZvcm0uZmluZE1ldGhvZChhc3NlbWJseU5hbWUsIG5hbWVzcGFjZSwgdHlwZVNob3J0TmFtZSwgbWV0aG9kTmFtZSk7XHJcbiAgICBtb25vUGxhdGZvcm0uY2FsbE1ldGhvZChlbnRyeVBvaW50TWV0aG9kSGFuZGxlLCBudWxsLCBhcmdzKTtcclxuICB9LFxyXG5cclxuICBjYWxsTWV0aG9kOiBmdW5jdGlvbiBjYWxsTWV0aG9kKG1ldGhvZDogTWV0aG9kSGFuZGxlLCB0YXJnZXQ6IFN5c3RlbV9PYmplY3QsIGFyZ3M6IFN5c3RlbV9PYmplY3RbXSk6IFN5c3RlbV9PYmplY3Qge1xyXG4gICAgaWYgKGFyZ3MubGVuZ3RoID4gNCkge1xyXG4gICAgICAvLyBIb3BlZnVsbHkgdGhpcyByZXN0cmljdGlvbiBjYW4gYmUgZWFzZWQgc29vbiwgYnV0IGZvciBub3cgbWFrZSBpdCBjbGVhciB3aGF0J3MgZ29pbmcgb25cclxuICAgICAgdGhyb3cgbmV3IEVycm9yKGBDdXJyZW50bHksIE1vbm9QbGF0Zm9ybSBzdXBwb3J0cyBwYXNzaW5nIGEgbWF4aW11bSBvZiA0IGFyZ3VtZW50cyBmcm9tIEpTIHRvIC5ORVQuIFlvdSB0cmllZCB0byBwYXNzICR7YXJncy5sZW5ndGh9LmApO1xyXG4gICAgfVxyXG5cclxuICAgIGNvbnN0IHN0YWNrID0gTW9kdWxlLnN0YWNrU2F2ZSgpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIGNvbnN0IGFyZ3NCdWZmZXIgPSBNb2R1bGUuc3RhY2tBbGxvYyhhcmdzLmxlbmd0aCk7XHJcbiAgICAgIGNvbnN0IGV4Y2VwdGlvbkZsYWdNYW5hZ2VkSW50ID0gTW9kdWxlLnN0YWNrQWxsb2MoNCk7XHJcbiAgICAgIGZvciAodmFyIGkgPSAwOyBpIDwgYXJncy5sZW5ndGg7ICsraSkge1xyXG4gICAgICAgIE1vZHVsZS5zZXRWYWx1ZShhcmdzQnVmZmVyICsgaSAqIDQsIGFyZ3NbaV0sICdpMzInKTtcclxuICAgICAgfVxyXG4gICAgICBNb2R1bGUuc2V0VmFsdWUoZXhjZXB0aW9uRmxhZ01hbmFnZWRJbnQsIDAsICdpMzInKTtcclxuXHJcbiAgICAgIGNvbnN0IHJlcyA9IGludm9rZV9tZXRob2QobWV0aG9kLCB0YXJnZXQsIGFyZ3NCdWZmZXIsIGV4Y2VwdGlvbkZsYWdNYW5hZ2VkSW50KTtcclxuXHJcbiAgICAgIGlmIChNb2R1bGUuZ2V0VmFsdWUoZXhjZXB0aW9uRmxhZ01hbmFnZWRJbnQsICdpMzInKSAhPT0gMCkge1xyXG4gICAgICAgIC8vIElmIHRoZSBleGNlcHRpb24gZmxhZyBpcyBzZXQsIHRoZSByZXR1cm5lZCB2YWx1ZSBpcyBleGNlcHRpb24uVG9TdHJpbmcoKVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihtb25vUGxhdGZvcm0udG9KYXZhU2NyaXB0U3RyaW5nKDxTeXN0ZW1fU3RyaW5nPnJlcykpO1xyXG4gICAgICB9XHJcblxyXG4gICAgICByZXR1cm4gcmVzO1xyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgTW9kdWxlLnN0YWNrUmVzdG9yZShzdGFjayk7XHJcbiAgICB9XHJcbiAgfSxcclxuXHJcbiAgdG9KYXZhU2NyaXB0U3RyaW5nOiBmdW5jdGlvbiB0b0phdmFTY3JpcHRTdHJpbmcobWFuYWdlZFN0cmluZzogU3lzdGVtX1N0cmluZykge1xyXG4gICAgLy8gQ29tbWVudHMgZnJvbSBvcmlnaW5hbCBNb25vIHNhbXBsZTpcclxuICAgIC8vRklYTUUgdGhpcyBpcyB3YXN0ZWZ1bGwsIHdlIGNvdWxkIHJlbW92ZSB0aGUgdGVtcCBtYWxsb2MgYnkgZ29pbmcgdGhlIFVURjE2IHJvdXRlXHJcbiAgICAvL0ZJWE1FIHRoaXMgaXMgdW5zYWZlLCBjdXogcmF3IG9iamVjdHMgY291bGQgYmUgR0MnZC5cclxuXHJcbiAgICBjb25zdCB1dGY4ID0gbW9ub19zdHJpbmdfZ2V0X3V0ZjgobWFuYWdlZFN0cmluZyk7XHJcbiAgICBjb25zdCByZXMgPSBNb2R1bGUuVVRGOFRvU3RyaW5nKHV0ZjgpO1xyXG4gICAgTW9kdWxlLl9mcmVlKHV0ZjggYXMgYW55KTtcclxuICAgIHJldHVybiByZXM7XHJcbiAgfSxcclxuXHJcbiAgdG9Eb3ROZXRTdHJpbmc6IGZ1bmN0aW9uIHRvRG90TmV0U3RyaW5nKGpzU3RyaW5nOiBzdHJpbmcpOiBTeXN0ZW1fU3RyaW5nIHtcclxuICAgIHJldHVybiBtb25vX3N0cmluZyhqc1N0cmluZyk7XHJcbiAgfSxcclxuXHJcbiAgZ2V0QXJyYXlMZW5ndGg6IGZ1bmN0aW9uIGdldEFycmF5TGVuZ3RoKGFycmF5OiBTeXN0ZW1fQXJyYXk8YW55Pik6IG51bWJlciB7XHJcbiAgICByZXR1cm4gTW9kdWxlLmdldFZhbHVlKGdldEFycmF5RGF0YVBvaW50ZXIoYXJyYXkpLCAnaTMyJyk7XHJcbiAgfSxcclxuXHJcbiAgZ2V0QXJyYXlFbnRyeVB0cjogZnVuY3Rpb24gZ2V0QXJyYXlFbnRyeVB0cjxUUHRyIGV4dGVuZHMgUG9pbnRlcj4oYXJyYXk6IFN5c3RlbV9BcnJheTxUUHRyPiwgaW5kZXg6IG51bWJlciwgaXRlbVNpemU6IG51bWJlcik6IFRQdHIge1xyXG4gICAgLy8gRmlyc3QgYnl0ZSBpcyBhcnJheSBsZW5ndGgsIGZvbGxvd2VkIGJ5IGVudHJpZXNcclxuICAgIGNvbnN0IGFkZHJlc3MgPSBnZXRBcnJheURhdGFQb2ludGVyKGFycmF5KSArIDQgKyBpbmRleCAqIGl0ZW1TaXplO1xyXG4gICAgcmV0dXJuIGFkZHJlc3MgYXMgYW55IGFzIFRQdHI7XHJcbiAgfSxcclxuXHJcbiAgZ2V0T2JqZWN0RmllbGRzQmFzZUFkZHJlc3M6IGZ1bmN0aW9uIGdldE9iamVjdEZpZWxkc0Jhc2VBZGRyZXNzKHJlZmVyZW5jZVR5cGVkT2JqZWN0OiBTeXN0ZW1fT2JqZWN0KTogUG9pbnRlciB7XHJcbiAgICAvLyBUaGUgZmlyc3QgdHdvIGludDMyIHZhbHVlcyBhcmUgaW50ZXJuYWwgTW9ubyBkYXRhXHJcbiAgICByZXR1cm4gKHJlZmVyZW5jZVR5cGVkT2JqZWN0IGFzIGFueSBhcyBudW1iZXIgKyA4KSBhcyBhbnkgYXMgUG9pbnRlcjtcclxuICB9LFxyXG5cclxuICByZWFkSW50MzJGaWVsZDogZnVuY3Rpb24gcmVhZEhlYXBJbnQzMihiYXNlQWRkcmVzczogUG9pbnRlciwgZmllbGRPZmZzZXQ/OiBudW1iZXIpOiBudW1iZXIge1xyXG4gICAgcmV0dXJuIE1vZHVsZS5nZXRWYWx1ZSgoYmFzZUFkZHJlc3MgYXMgYW55IGFzIG51bWJlcikgKyAoZmllbGRPZmZzZXQgfHwgMCksICdpMzInKTtcclxuICB9LFxyXG5cclxuICByZWFkT2JqZWN0RmllbGQ6IGZ1bmN0aW9uIHJlYWRIZWFwT2JqZWN0PFQgZXh0ZW5kcyBTeXN0ZW1fT2JqZWN0PihiYXNlQWRkcmVzczogUG9pbnRlciwgZmllbGRPZmZzZXQ/OiBudW1iZXIpOiBUIHtcclxuICAgIHJldHVybiBNb2R1bGUuZ2V0VmFsdWUoKGJhc2VBZGRyZXNzIGFzIGFueSBhcyBudW1iZXIpICsgKGZpZWxkT2Zmc2V0IHx8IDApLCAnaTMyJykgYXMgYW55IGFzIFQ7XHJcbiAgfSxcclxuXHJcbiAgcmVhZFN0cmluZ0ZpZWxkOiBmdW5jdGlvbiByZWFkSGVhcE9iamVjdChiYXNlQWRkcmVzczogUG9pbnRlciwgZmllbGRPZmZzZXQ/OiBudW1iZXIpOiBzdHJpbmcgfCBudWxsIHtcclxuICAgIGNvbnN0IGZpZWxkVmFsdWUgPSBNb2R1bGUuZ2V0VmFsdWUoKGJhc2VBZGRyZXNzIGFzIGFueSBhcyBudW1iZXIpICsgKGZpZWxkT2Zmc2V0IHx8IDApLCAnaTMyJyk7XHJcbiAgICByZXR1cm4gZmllbGRWYWx1ZSA9PT0gMCA/IG51bGwgOiBtb25vUGxhdGZvcm0udG9KYXZhU2NyaXB0U3RyaW5nKGZpZWxkVmFsdWUgYXMgYW55IGFzIFN5c3RlbV9TdHJpbmcpO1xyXG4gIH0sXHJcblxyXG4gIHJlYWRTdHJ1Y3RGaWVsZDogZnVuY3Rpb24gcmVhZFN0cnVjdEZpZWxkPFQgZXh0ZW5kcyBQb2ludGVyPihiYXNlQWRkcmVzczogUG9pbnRlciwgZmllbGRPZmZzZXQ/OiBudW1iZXIpOiBUIHtcclxuICAgIHJldHVybiAoKGJhc2VBZGRyZXNzIGFzIGFueSBhcyBudW1iZXIpICsgKGZpZWxkT2Zmc2V0IHx8IDApKSBhcyBhbnkgYXMgVDtcclxuICB9LFxyXG59O1xyXG5cclxuLy8gQnlwYXNzIG5vcm1hbCB0eXBlIGNoZWNraW5nIHRvIGFkZCB0aGlzIGV4dHJhIGZ1bmN0aW9uLiBJdCdzIG9ubHkgaW50ZW5kZWQgdG8gYmUgY2FsbGVkIGZyb21cclxuLy8gdGhlIEpTIGNvZGUgaW4gTW9ubydzIGRyaXZlci5jLiBJdCdzIG5ldmVyIGludGVuZGVkIHRvIGJlIGNhbGxlZCBmcm9tIFR5cGVTY3JpcHQuXHJcbihtb25vUGxhdGZvcm0gYXMgYW55KS5tb25vR2V0UmVnaXN0ZXJlZEZ1bmN0aW9uID0gZ2V0UmVnaXN0ZXJlZEZ1bmN0aW9uO1xyXG5cclxuZnVuY3Rpb24gZmluZEFzc2VtYmx5KGFzc2VtYmx5TmFtZTogc3RyaW5nKTogbnVtYmVyIHtcclxuICBsZXQgYXNzZW1ibHlIYW5kbGUgPSBhc3NlbWJseUhhbmRsZUNhY2hlW2Fzc2VtYmx5TmFtZV07XHJcbiAgaWYgKCFhc3NlbWJseUhhbmRsZSkge1xyXG4gICAgYXNzZW1ibHlIYW5kbGUgPSBhc3NlbWJseV9sb2FkKGFzc2VtYmx5TmFtZSk7XHJcbiAgICBpZiAoIWFzc2VtYmx5SGFuZGxlKSB7XHJcbiAgICAgIHRocm93IG5ldyBFcnJvcihgQ291bGQgbm90IGZpbmQgYXNzZW1ibHkgXCIke2Fzc2VtYmx5TmFtZX1cImApO1xyXG4gICAgfVxyXG4gICAgYXNzZW1ibHlIYW5kbGVDYWNoZVthc3NlbWJseU5hbWVdID0gYXNzZW1ibHlIYW5kbGU7XHJcbiAgfVxyXG4gIHJldHVybiBhc3NlbWJseUhhbmRsZTtcclxufVxyXG5cclxuZnVuY3Rpb24gZmluZFR5cGUoYXNzZW1ibHlOYW1lOiBzdHJpbmcsIG5hbWVzcGFjZTogc3RyaW5nLCBjbGFzc05hbWU6IHN0cmluZyk6IG51bWJlciB7XHJcbiAgY29uc3QgZnVsbHlRdWFsaWZpZWRUeXBlTmFtZSA9IGBbJHthc3NlbWJseU5hbWV9XSR7bmFtZXNwYWNlfS4ke2NsYXNzTmFtZX1gO1xyXG4gIGxldCB0eXBlSGFuZGxlID0gdHlwZUhhbmRsZUNhY2hlW2Z1bGx5UXVhbGlmaWVkVHlwZU5hbWVdO1xyXG4gIGlmICghdHlwZUhhbmRsZSkge1xyXG4gICAgdHlwZUhhbmRsZSA9IGZpbmRfY2xhc3MoZmluZEFzc2VtYmx5KGFzc2VtYmx5TmFtZSksIG5hbWVzcGFjZSwgY2xhc3NOYW1lKTtcclxuICAgIGlmICghdHlwZUhhbmRsZSkge1xyXG4gICAgICB0aHJvdyBuZXcgRXJyb3IoYENvdWxkIG5vdCBmaW5kIHR5cGUgXCIke2NsYXNzTmFtZX1cIiBpbiBuYW1lc3BhY2UgXCIke25hbWVzcGFjZX1cIiBpbiBhc3NlbWJseSBcIiR7YXNzZW1ibHlOYW1lfVwiYCk7XHJcbiAgICB9XHJcbiAgICB0eXBlSGFuZGxlQ2FjaGVbZnVsbHlRdWFsaWZpZWRUeXBlTmFtZV0gPSB0eXBlSGFuZGxlO1xyXG4gIH1cclxuICByZXR1cm4gdHlwZUhhbmRsZTtcclxufVxyXG5cclxuZnVuY3Rpb24gZmluZE1ldGhvZChhc3NlbWJseU5hbWU6IHN0cmluZywgbmFtZXNwYWNlOiBzdHJpbmcsIGNsYXNzTmFtZTogc3RyaW5nLCBtZXRob2ROYW1lOiBzdHJpbmcpOiBNZXRob2RIYW5kbGUge1xyXG4gIGNvbnN0IGZ1bGx5UXVhbGlmaWVkTWV0aG9kTmFtZSA9IGBbJHthc3NlbWJseU5hbWV9XSR7bmFtZXNwYWNlfS4ke2NsYXNzTmFtZX06OiR7bWV0aG9kTmFtZX1gO1xyXG4gIGxldCBtZXRob2RIYW5kbGUgPSBtZXRob2RIYW5kbGVDYWNoZVtmdWxseVF1YWxpZmllZE1ldGhvZE5hbWVdO1xyXG4gIGlmICghbWV0aG9kSGFuZGxlKSB7XHJcbiAgICBtZXRob2RIYW5kbGUgPSBmaW5kX21ldGhvZChmaW5kVHlwZShhc3NlbWJseU5hbWUsIG5hbWVzcGFjZSwgY2xhc3NOYW1lKSwgbWV0aG9kTmFtZSwgLTEpO1xyXG4gICAgaWYgKCFtZXRob2RIYW5kbGUpIHtcclxuICAgICAgdGhyb3cgbmV3IEVycm9yKGBDb3VsZCBub3QgZmluZCBtZXRob2QgXCIke21ldGhvZE5hbWV9XCIgb24gdHlwZSBcIiR7bmFtZXNwYWNlfS4ke2NsYXNzTmFtZX1cImApO1xyXG4gICAgfVxyXG4gICAgbWV0aG9kSGFuZGxlQ2FjaGVbZnVsbHlRdWFsaWZpZWRNZXRob2ROYW1lXSA9IG1ldGhvZEhhbmRsZTtcclxuICB9XHJcbiAgcmV0dXJuIG1ldGhvZEhhbmRsZTtcclxufVxyXG5cclxuZnVuY3Rpb24gYWRkU2NyaXB0VGFnc1RvRG9jdW1lbnQoKSB7XHJcbiAgLy8gTG9hZCBlaXRoZXIgdGhlIHdhc20gb3IgYXNtLmpzIHZlcnNpb24gb2YgdGhlIE1vbm8gcnVudGltZVxyXG4gIGNvbnN0IGJyb3dzZXJTdXBwb3J0c05hdGl2ZVdlYkFzc2VtYmx5ID0gdHlwZW9mIFdlYkFzc2VtYmx5ICE9PSAndW5kZWZpbmVkJyAmJiBXZWJBc3NlbWJseS52YWxpZGF0ZTtcclxuICBjb25zdCBtb25vUnVudGltZVVybEJhc2UgPSAnX2ZyYW1ld29yay8nICsgKGJyb3dzZXJTdXBwb3J0c05hdGl2ZVdlYkFzc2VtYmx5ID8gJ3dhc20nIDogJ2FzbWpzJyk7XHJcbiAgY29uc3QgbW9ub1J1bnRpbWVTY3JpcHRVcmwgPSBgJHttb25vUnVudGltZVVybEJhc2V9L21vbm8uanNgO1xyXG5cclxuICBpZiAoIWJyb3dzZXJTdXBwb3J0c05hdGl2ZVdlYkFzc2VtYmx5KSB7XHJcbiAgICAvLyBJbiB0aGUgYXNtanMgY2FzZSwgdGhlIGluaXRpYWwgbWVtb3J5IHN0cnVjdHVyZSBpcyBpbiBhIHNlcGFyYXRlIGZpbGUgd2UgbmVlZCB0byBkb3dubG9hZFxyXG4gICAgY29uc3QgbWVtaW5pdFhIUiA9IE1vZHVsZVsnbWVtb3J5SW5pdGlhbGl6ZXJSZXF1ZXN0J10gPSBuZXcgWE1MSHR0cFJlcXVlc3QoKTtcclxuICAgIG1lbWluaXRYSFIub3BlbignR0VUJywgYCR7bW9ub1J1bnRpbWVVcmxCYXNlfS9tb25vLmpzLm1lbWApO1xyXG4gICAgbWVtaW5pdFhIUi5yZXNwb25zZVR5cGUgPSAnYXJyYXlidWZmZXInO1xyXG4gICAgbWVtaW5pdFhIUi5zZW5kKG51bGwpO1xyXG4gIH1cclxuXHJcbiAgZG9jdW1lbnQud3JpdGUoYDxzY3JpcHQgZGVmZXIgc3JjPVwiJHttb25vUnVudGltZVNjcmlwdFVybH1cIj48L3NjcmlwdD5gKTtcclxufVxyXG5cclxuZnVuY3Rpb24gY3JlYXRlRW1zY3JpcHRlbk1vZHVsZUluc3RhbmNlKGxvYWRBc3NlbWJseVVybHM6IHN0cmluZ1tdLCBvblJlYWR5OiAoKSA9PiB2b2lkLCBvbkVycm9yOiAocmVhc29uPzogYW55KSA9PiB2b2lkKSB7XHJcbiAgY29uc3QgbW9kdWxlID0ge30gYXMgdHlwZW9mIE1vZHVsZTtcclxuICBjb25zdCB3YXNtQmluYXJ5RmlsZSA9ICdfZnJhbWV3b3JrL3dhc20vbW9uby53YXNtJztcclxuICBjb25zdCBhc21qc0NvZGVGaWxlID0gJ19mcmFtZXdvcmsvYXNtanMvbW9uby5hc20uanMnO1xyXG5cclxuICBtb2R1bGUucHJpbnQgPSBsaW5lID0+IGNvbnNvbGUubG9nKGBXQVNNOiAke2xpbmV9YCk7XHJcbiAgbW9kdWxlLnByaW50RXJyID0gbGluZSA9PiBjb25zb2xlLmVycm9yKGBXQVNNOiAke2xpbmV9YCk7XHJcbiAgbW9kdWxlLnByZVJ1biA9IFtdO1xyXG4gIG1vZHVsZS5wb3N0UnVuID0gW107XHJcbiAgbW9kdWxlLnByZWxvYWRQbHVnaW5zID0gW107XHJcblxyXG4gIG1vZHVsZS5sb2NhdGVGaWxlID0gZmlsZU5hbWUgPT4ge1xyXG4gICAgc3dpdGNoIChmaWxlTmFtZSkge1xyXG4gICAgICBjYXNlICdtb25vLndhc20nOiByZXR1cm4gd2FzbUJpbmFyeUZpbGU7XHJcbiAgICAgIGNhc2UgJ21vbm8uYXNtLmpzJzogcmV0dXJuIGFzbWpzQ29kZUZpbGU7XHJcbiAgICAgIGRlZmF1bHQ6IHJldHVybiBmaWxlTmFtZTtcclxuICAgIH1cclxuICB9O1xyXG5cclxuICBtb2R1bGUucHJlUnVuLnB1c2goKCkgPT4ge1xyXG4gICAgLy8gQnkgbm93LCBlbXNjcmlwdGVuIHNob3VsZCBiZSBpbml0aWFsaXNlZCBlbm91Z2ggdGhhdCB3ZSBjYW4gY2FwdHVyZSB0aGVzZSBtZXRob2RzIGZvciBsYXRlciB1c2VcclxuICAgIGFzc2VtYmx5X2xvYWQgPSBNb2R1bGUuY3dyYXAoJ21vbm9fd2FzbV9hc3NlbWJseV9sb2FkJywgJ251bWJlcicsIFsnc3RyaW5nJ10pO1xyXG4gICAgZmluZF9jbGFzcyA9IE1vZHVsZS5jd3JhcCgnbW9ub193YXNtX2Fzc2VtYmx5X2ZpbmRfY2xhc3MnLCAnbnVtYmVyJywgWydudW1iZXInLCAnc3RyaW5nJywgJ3N0cmluZyddKTtcclxuICAgIGZpbmRfbWV0aG9kID0gTW9kdWxlLmN3cmFwKCdtb25vX3dhc21fYXNzZW1ibHlfZmluZF9tZXRob2QnLCAnbnVtYmVyJywgWydudW1iZXInLCAnc3RyaW5nJywgJ251bWJlciddKTtcclxuICAgIGludm9rZV9tZXRob2QgPSBNb2R1bGUuY3dyYXAoJ21vbm9fd2FzbV9pbnZva2VfbWV0aG9kJywgJ251bWJlcicsIFsnbnVtYmVyJywgJ251bWJlcicsICdudW1iZXInXSk7XHJcbiAgICBtb25vX3N0cmluZ19nZXRfdXRmOCA9IE1vZHVsZS5jd3JhcCgnbW9ub193YXNtX3N0cmluZ19nZXRfdXRmOCcsICdudW1iZXInLCBbJ251bWJlciddKTtcclxuICAgIG1vbm9fc3RyaW5nID0gTW9kdWxlLmN3cmFwKCdtb25vX3dhc21fc3RyaW5nX2Zyb21fanMnLCAnbnVtYmVyJywgWydzdHJpbmcnXSk7XHJcblxyXG4gICAgTW9kdWxlLkZTX2NyZWF0ZVBhdGgoJy8nLCAnYXBwQmluRGlyJywgdHJ1ZSwgdHJ1ZSk7XHJcbiAgICBsb2FkQXNzZW1ibHlVcmxzLmZvckVhY2godXJsID0+XHJcbiAgICAgIEZTLmNyZWF0ZVByZWxvYWRlZEZpbGUoJ2FwcEJpbkRpcicsIGAke2dldEFzc2VtYmx5TmFtZUZyb21VcmwodXJsKX0uZGxsYCwgdXJsLCB0cnVlLCBmYWxzZSwgdW5kZWZpbmVkLCBvbkVycm9yKSk7XHJcbiAgfSk7XHJcblxyXG4gIG1vZHVsZS5wb3N0UnVuLnB1c2goKCkgPT4ge1xyXG4gICAgY29uc3QgbG9hZF9ydW50aW1lID0gTW9kdWxlLmN3cmFwKCdtb25vX3dhc21fbG9hZF9ydW50aW1lJywgbnVsbCwgWydzdHJpbmcnXSk7XHJcbiAgICBsb2FkX3J1bnRpbWUoJ2FwcEJpbkRpcicpO1xyXG4gICAgb25SZWFkeSgpO1xyXG4gIH0pO1xyXG5cclxuICByZXR1cm4gbW9kdWxlO1xyXG59XHJcblxyXG5mdW5jdGlvbiBhc3luY0xvYWQodXJsLCBvbmxvYWQsIG9uZXJyb3IpIHtcclxuICB2YXIgeGhyID0gbmV3IFhNTEh0dHBSZXF1ZXN0O1xyXG4gIHhoci5vcGVuKCdHRVQnLCB1cmwsIC8qIGFzeW5jOiAqLyB0cnVlKTtcclxuICB4aHIucmVzcG9uc2VUeXBlID0gJ2FycmF5YnVmZmVyJztcclxuICB4aHIub25sb2FkID0gZnVuY3Rpb24geGhyX29ubG9hZCgpIHtcclxuICAgIGlmICh4aHIuc3RhdHVzID09IDIwMCB8fCB4aHIuc3RhdHVzID09IDAgJiYgeGhyLnJlc3BvbnNlKSB7XHJcbiAgICAgIHZhciBhc20gPSBuZXcgVWludDhBcnJheSh4aHIucmVzcG9uc2UpO1xyXG4gICAgICBvbmxvYWQoYXNtKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIG9uZXJyb3IoeGhyKTtcclxuICAgIH1cclxuICB9O1xyXG4gIHhoci5vbmVycm9yID0gb25lcnJvcjtcclxuICB4aHIuc2VuZChudWxsKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZ2V0QXJyYXlEYXRhUG9pbnRlcjxUPihhcnJheTogU3lzdGVtX0FycmF5PFQ+KTogbnVtYmVyIHtcclxuICByZXR1cm4gPG51bWJlcj48YW55PmFycmF5ICsgMTI7IC8vIEZpcnN0IGJ5dGUgZnJvbSBoZXJlIGlzIGxlbmd0aCwgdGhlbiBmb2xsb3dpbmcgYnl0ZXMgYXJlIGVudHJpZXNcclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvUGxhdGZvcm0vTW9uby9Nb25vUGxhdGZvcm0udHMiLCJpbXBvcnQgeyBpbnZva2VXaXRoSnNvbk1hcnNoYWxsaW5nIH0gZnJvbSAnLi9JbnZva2VXaXRoSnNvbk1hcnNoYWxsaW5nJztcclxuaW1wb3J0IHsgYXR0YWNoUm9vdENvbXBvbmVudFRvRWxlbWVudCwgcmVuZGVyQmF0Y2ggfSBmcm9tICcuLi9SZW5kZXJpbmcvUmVuZGVyZXInO1xyXG5cclxuLyoqXHJcbiAqIFRoZSBkZWZpbml0aXZlIGxpc3Qgb2YgaW50ZXJuYWwgZnVuY3Rpb25zIGludm9rYWJsZSBmcm9tIC5ORVQgY29kZS5cclxuICogVGhlc2UgZnVuY3Rpb24gbmFtZXMgYXJlIHRyZWF0ZWQgYXMgJ3Jlc2VydmVkJyBhbmQgY2Fubm90IGJlIHBhc3NlZCB0byByZWdpc3RlckZ1bmN0aW9uLlxyXG4gKi9cclxuZXhwb3J0IGNvbnN0IGludGVybmFsUmVnaXN0ZXJlZEZ1bmN0aW9ucyA9IHtcclxuICBhdHRhY2hSb290Q29tcG9uZW50VG9FbGVtZW50LFxyXG4gIGludm9rZVdpdGhKc29uTWFyc2hhbGxpbmcsXHJcbiAgcmVuZGVyQmF0Y2gsXHJcbn07XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9JbnRlcm9wL0ludGVybmFsUmVnaXN0ZXJlZEZ1bmN0aW9uLnRzIiwiaW1wb3J0IHsgcGxhdGZvcm0gfSBmcm9tICcuLi9FbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IFN5c3RlbV9TdHJpbmcgfSBmcm9tICcuLi9QbGF0Zm9ybS9QbGF0Zm9ybSc7XHJcbmltcG9ydCB7IGdldFJlZ2lzdGVyZWRGdW5jdGlvbiB9IGZyb20gJy4vUmVnaXN0ZXJlZEZ1bmN0aW9uJztcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBpbnZva2VXaXRoSnNvbk1hcnNoYWxsaW5nKGlkZW50aWZpZXI6IFN5c3RlbV9TdHJpbmcsIC4uLmFyZ3NKc29uOiBTeXN0ZW1fU3RyaW5nW10pIHtcclxuICBjb25zdCBpZGVudGlmaWVySnNTdHJpbmcgPSBwbGF0Zm9ybS50b0phdmFTY3JpcHRTdHJpbmcoaWRlbnRpZmllcik7XHJcbiAgY29uc3QgZnVuY0luc3RhbmNlID0gZ2V0UmVnaXN0ZXJlZEZ1bmN0aW9uKGlkZW50aWZpZXJKc1N0cmluZyk7XHJcbiAgY29uc3QgYXJncyA9IGFyZ3NKc29uLm1hcChqc29uID0+IEpTT04ucGFyc2UocGxhdGZvcm0udG9KYXZhU2NyaXB0U3RyaW5nKGpzb24pKSk7XHJcbiAgY29uc3QgcmVzdWx0ID0gZnVuY0luc3RhbmNlLmFwcGx5KG51bGwsIGFyZ3MpO1xyXG4gIGlmIChyZXN1bHQgIT09IG51bGwgJiYgcmVzdWx0ICE9PSB1bmRlZmluZWQpIHtcclxuICAgIGNvbnN0IHJlc3VsdEpzb24gPSBKU09OLnN0cmluZ2lmeShyZXN1bHQpO1xyXG4gICAgcmV0dXJuIHBsYXRmb3JtLnRvRG90TmV0U3RyaW5nKHJlc3VsdEpzb24pO1xyXG4gIH0gZWxzZSB7XHJcbiAgICByZXR1cm4gbnVsbDtcclxuICB9XHJcbn1cclxuXG5cblxuLy8gV0VCUEFDSyBGT09URVIgLy9cbi8vIC4vc3JjL0ludGVyb3AvSW52b2tlV2l0aEpzb25NYXJzaGFsbGluZy50cyIsImltcG9ydCB7IFBvaW50ZXIsIFN5c3RlbV9BcnJheSB9IGZyb20gJy4uL1BsYXRmb3JtL1BsYXRmb3JtJztcclxuaW1wb3J0IHsgcGxhdGZvcm0gfSBmcm9tICcuLi9FbnZpcm9ubWVudCc7XHJcbmltcG9ydCB7IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIgfSBmcm9tICcuL1JlbmRlclRyZWVGcmFtZSc7XHJcbmltcG9ydCB7IFJlbmRlclRyZWVFZGl0UG9pbnRlciB9IGZyb20gJy4vUmVuZGVyVHJlZUVkaXQnO1xyXG5cclxuLy8gS2VlcCBpbiBzeW5jIHdpdGggdGhlIHN0cnVjdHMgaW4gLk5FVCBjb2RlXHJcblxyXG5leHBvcnQgY29uc3QgcmVuZGVyQmF0Y2ggPSB7XHJcbiAgdXBkYXRlZENvbXBvbmVudHM6IChvYmo6IFJlbmRlckJhdGNoUG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZFN0cnVjdEZpZWxkPEFycmF5UmFuZ2VQb2ludGVyPFJlbmRlclRyZWVEaWZmUG9pbnRlcj4+KG9iaiwgMCksXHJcbiAgcmVmZXJlbmNlRnJhbWVzOiAob2JqOiBSZW5kZXJCYXRjaFBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRTdHJ1Y3RGaWVsZDxBcnJheVJhbmdlUG9pbnRlcjxSZW5kZXJUcmVlRnJhbWVQb2ludGVyPj4ob2JqLCBhcnJheVJhbmdlU3RydWN0TGVuZ3RoKSxcclxuICBkaXNwb3NlZENvbXBvbmVudElkczogKG9iajogUmVuZGVyQmF0Y2hQb2ludGVyKSA9PiBwbGF0Zm9ybS5yZWFkU3RydWN0RmllbGQ8QXJyYXlSYW5nZVBvaW50ZXI8bnVtYmVyPj4ob2JqLCBhcnJheVJhbmdlU3RydWN0TGVuZ3RoICsgYXJyYXlSYW5nZVN0cnVjdExlbmd0aCksXHJcbiAgZGlzcG9zZWRFdmVudEhhbmRsZXJJZHM6IChvYmo6IFJlbmRlckJhdGNoUG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZFN0cnVjdEZpZWxkPEFycmF5UmFuZ2VQb2ludGVyPG51bWJlcj4+KG9iaiwgYXJyYXlSYW5nZVN0cnVjdExlbmd0aCArIGFycmF5UmFuZ2VTdHJ1Y3RMZW5ndGggKyBhcnJheVJhbmdlU3RydWN0TGVuZ3RoKSxcclxufTtcclxuXHJcbmNvbnN0IGFycmF5UmFuZ2VTdHJ1Y3RMZW5ndGggPSA4O1xyXG5leHBvcnQgY29uc3QgYXJyYXlSYW5nZSA9IHtcclxuICBhcnJheTogPFQ+KG9iajogQXJyYXlSYW5nZVBvaW50ZXI8VD4pID0+IHBsYXRmb3JtLnJlYWRPYmplY3RGaWVsZDxTeXN0ZW1fQXJyYXk8VD4+KG9iaiwgMCksXHJcbiAgY291bnQ6IDxUPihvYmo6IEFycmF5UmFuZ2VQb2ludGVyPFQ+KSA9PiBwbGF0Zm9ybS5yZWFkSW50MzJGaWVsZChvYmosIDQpLFxyXG59O1xyXG5cclxuY29uc3QgYXJyYXlTZWdtZW50U3RydWN0TGVuZ3RoID0gMTI7XHJcbmV4cG9ydCBjb25zdCBhcnJheVNlZ21lbnQgPSB7XHJcbiAgYXJyYXk6IDxUPihvYmo6IEFycmF5U2VnbWVudFBvaW50ZXI8VD4pID0+IHBsYXRmb3JtLnJlYWRPYmplY3RGaWVsZDxTeXN0ZW1fQXJyYXk8VD4+KG9iaiwgMCksXHJcbiAgb2Zmc2V0OiA8VD4ob2JqOiBBcnJheVNlZ21lbnRQb2ludGVyPFQ+KSA9PiBwbGF0Zm9ybS5yZWFkSW50MzJGaWVsZChvYmosIDQpLFxyXG4gIGNvdW50OiA8VD4ob2JqOiBBcnJheVNlZ21lbnRQb2ludGVyPFQ+KSA9PiBwbGF0Zm9ybS5yZWFkSW50MzJGaWVsZChvYmosIDgpLFxyXG59O1xyXG5cclxuZXhwb3J0IGNvbnN0IHJlbmRlclRyZWVEaWZmU3RydWN0TGVuZ3RoID0gNCArIGFycmF5U2VnbWVudFN0cnVjdExlbmd0aDtcclxuZXhwb3J0IGNvbnN0IHJlbmRlclRyZWVEaWZmID0ge1xyXG4gIGNvbXBvbmVudElkOiAob2JqOiBSZW5kZXJUcmVlRGlmZlBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRJbnQzMkZpZWxkKG9iaiwgMCksXHJcbiAgZWRpdHM6IChvYmo6IFJlbmRlclRyZWVEaWZmUG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZFN0cnVjdEZpZWxkPEFycmF5U2VnbWVudFBvaW50ZXI8UmVuZGVyVHJlZUVkaXRQb2ludGVyPj4ob2JqLCA0KSwgIFxyXG59O1xyXG5cclxuLy8gTm9taW5hbCB0eXBlcyB0byBlbnN1cmUgb25seSB2YWxpZCBwb2ludGVycyBhcmUgcGFzc2VkIHRvIHRoZSBmdW5jdGlvbnMgYWJvdmUuXHJcbi8vIEF0IHJ1bnRpbWUgdGhlIHZhbHVlcyBhcmUganVzdCBudW1iZXJzLlxyXG5leHBvcnQgaW50ZXJmYWNlIFJlbmRlckJhdGNoUG9pbnRlciBleHRlbmRzIFBvaW50ZXIgeyBSZW5kZXJCYXRjaFBvaW50ZXJfX0RPX05PVF9JTVBMRU1FTlQ6IGFueSB9XHJcbmV4cG9ydCBpbnRlcmZhY2UgQXJyYXlSYW5nZVBvaW50ZXI8VD4gZXh0ZW5kcyBQb2ludGVyIHsgQXJyYXlSYW5nZVBvaW50ZXJfX0RPX05PVF9JTVBMRU1FTlQ6IGFueSB9XHJcbmV4cG9ydCBpbnRlcmZhY2UgQXJyYXlTZWdtZW50UG9pbnRlcjxUPiBleHRlbmRzIFBvaW50ZXIgeyBBcnJheVNlZ21lbnRQb2ludGVyX19ET19OT1RfSU1QTEVNRU5UOiBhbnkgfVxyXG5leHBvcnQgaW50ZXJmYWNlIFJlbmRlclRyZWVEaWZmUG9pbnRlciBleHRlbmRzIFBvaW50ZXIgeyBSZW5kZXJUcmVlRGlmZlBvaW50ZXJfX0RPX05PVF9JTVBMRU1FTlQ6IGFueSB9XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9SZW5kZXJpbmcvUmVuZGVyQmF0Y2gudHMiLCJpbXBvcnQgeyBTeXN0ZW1fQXJyYXksIE1ldGhvZEhhbmRsZSB9IGZyb20gJy4uL1BsYXRmb3JtL1BsYXRmb3JtJztcclxuaW1wb3J0IHsgZ2V0UmVuZGVyVHJlZUVkaXRQdHIsIHJlbmRlclRyZWVFZGl0LCBSZW5kZXJUcmVlRWRpdFBvaW50ZXIsIEVkaXRUeXBlIH0gZnJvbSAnLi9SZW5kZXJUcmVlRWRpdCc7XHJcbmltcG9ydCB7IGdldFRyZWVGcmFtZVB0ciwgcmVuZGVyVHJlZUZyYW1lLCBGcmFtZVR5cGUsIFJlbmRlclRyZWVGcmFtZVBvaW50ZXIgfSBmcm9tICcuL1JlbmRlclRyZWVGcmFtZSc7XHJcbmltcG9ydCB7IHBsYXRmb3JtIH0gZnJvbSAnLi4vRW52aXJvbm1lbnQnO1xyXG5pbXBvcnQgeyBFdmVudERlbGVnYXRvciB9IGZyb20gJy4vRXZlbnREZWxlZ2F0b3InO1xyXG5pbXBvcnQgeyBFdmVudEZvckRvdE5ldCwgVUlFdmVudEFyZ3MgfSBmcm9tICcuL0V2ZW50Rm9yRG90TmV0JztcclxuY29uc3Qgc2VsZWN0VmFsdWVQcm9wbmFtZSA9ICdfYmxhem9yU2VsZWN0VmFsdWUnO1xyXG5sZXQgcmFpc2VFdmVudE1ldGhvZDogTWV0aG9kSGFuZGxlO1xyXG5sZXQgcmVuZGVyQ29tcG9uZW50TWV0aG9kOiBNZXRob2RIYW5kbGU7XHJcblxyXG5leHBvcnQgY2xhc3MgQnJvd3NlclJlbmRlcmVyIHtcclxuICBwcml2YXRlIGV2ZW50RGVsZWdhdG9yOiBFdmVudERlbGVnYXRvcjtcclxuICBwcml2YXRlIGNoaWxkQ29tcG9uZW50TG9jYXRpb25zOiB7IFtjb21wb25lbnRJZDogbnVtYmVyXTogRWxlbWVudCB9ID0ge307XHJcblxyXG4gIGNvbnN0cnVjdG9yKHByaXZhdGUgYnJvd3NlclJlbmRlcmVySWQ6IG51bWJlcikge1xyXG4gICAgdGhpcy5ldmVudERlbGVnYXRvciA9IG5ldyBFdmVudERlbGVnYXRvcigoZXZlbnQsIGNvbXBvbmVudElkLCBldmVudEhhbmRsZXJJZCwgZXZlbnRBcmdzKSA9PiB7XHJcbiAgICAgIHJhaXNlRXZlbnQoZXZlbnQsIHRoaXMuYnJvd3NlclJlbmRlcmVySWQsIGNvbXBvbmVudElkLCBldmVudEhhbmRsZXJJZCwgZXZlbnRBcmdzKTtcclxuICAgIH0pO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGF0dGFjaFJvb3RDb21wb25lbnRUb0VsZW1lbnQoY29tcG9uZW50SWQ6IG51bWJlciwgZWxlbWVudDogRWxlbWVudCkge1xyXG4gICAgdGhpcy5hdHRhY2hDb21wb25lbnRUb0VsZW1lbnQoY29tcG9uZW50SWQsIGVsZW1lbnQpO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIHVwZGF0ZUNvbXBvbmVudChjb21wb25lbnRJZDogbnVtYmVyLCBlZGl0czogU3lzdGVtX0FycmF5PFJlbmRlclRyZWVFZGl0UG9pbnRlcj4sIGVkaXRzT2Zmc2V0OiBudW1iZXIsIGVkaXRzTGVuZ3RoOiBudW1iZXIsIHJlZmVyZW5jZUZyYW1lczogU3lzdGVtX0FycmF5PFJlbmRlclRyZWVGcmFtZVBvaW50ZXI+KSB7XHJcbiAgICBjb25zdCBlbGVtZW50ID0gdGhpcy5jaGlsZENvbXBvbmVudExvY2F0aW9uc1tjb21wb25lbnRJZF07XHJcbiAgICBpZiAoIWVsZW1lbnQpIHtcclxuICAgICAgdGhyb3cgbmV3IEVycm9yKGBObyBlbGVtZW50IGlzIGN1cnJlbnRseSBhc3NvY2lhdGVkIHdpdGggY29tcG9uZW50ICR7Y29tcG9uZW50SWR9YCk7XHJcbiAgICB9XHJcblxyXG4gICAgdGhpcy5hcHBseUVkaXRzKGNvbXBvbmVudElkLCBlbGVtZW50LCAwLCBlZGl0cywgZWRpdHNPZmZzZXQsIGVkaXRzTGVuZ3RoLCByZWZlcmVuY2VGcmFtZXMpO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGRpc3Bvc2VDb21wb25lbnQoY29tcG9uZW50SWQ6IG51bWJlcikge1xyXG4gICAgZGVsZXRlIHRoaXMuY2hpbGRDb21wb25lbnRMb2NhdGlvbnNbY29tcG9uZW50SWRdO1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGRpc3Bvc2VFdmVudEhhbmRsZXIoZXZlbnRIYW5kbGVySWQ6IG51bWJlcikge1xyXG4gICAgdGhpcy5ldmVudERlbGVnYXRvci5yZW1vdmVMaXN0ZW5lcihldmVudEhhbmRsZXJJZCk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGF0dGFjaENvbXBvbmVudFRvRWxlbWVudChjb21wb25lbnRJZDogbnVtYmVyLCBlbGVtZW50OiBFbGVtZW50KSB7XHJcbiAgICB0aGlzLmNoaWxkQ29tcG9uZW50TG9jYXRpb25zW2NvbXBvbmVudElkXSA9IGVsZW1lbnQ7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGFwcGx5RWRpdHMoY29tcG9uZW50SWQ6IG51bWJlciwgcGFyZW50OiBFbGVtZW50LCBjaGlsZEluZGV4OiBudW1iZXIsIGVkaXRzOiBTeXN0ZW1fQXJyYXk8UmVuZGVyVHJlZUVkaXRQb2ludGVyPiwgZWRpdHNPZmZzZXQ6IG51bWJlciwgZWRpdHNMZW5ndGg6IG51bWJlciwgcmVmZXJlbmNlRnJhbWVzOiBTeXN0ZW1fQXJyYXk8UmVuZGVyVHJlZUZyYW1lUG9pbnRlcj4pIHtcclxuICAgIGxldCBjdXJyZW50RGVwdGggPSAwO1xyXG4gICAgbGV0IGNoaWxkSW5kZXhBdEN1cnJlbnREZXB0aCA9IGNoaWxkSW5kZXg7XHJcbiAgICBjb25zdCBtYXhFZGl0SW5kZXhFeGNsID0gZWRpdHNPZmZzZXQgKyBlZGl0c0xlbmd0aDtcclxuICAgIGZvciAobGV0IGVkaXRJbmRleCA9IGVkaXRzT2Zmc2V0OyBlZGl0SW5kZXggPCBtYXhFZGl0SW5kZXhFeGNsOyBlZGl0SW5kZXgrKykge1xyXG4gICAgICBjb25zdCBlZGl0ID0gZ2V0UmVuZGVyVHJlZUVkaXRQdHIoZWRpdHMsIGVkaXRJbmRleCk7XHJcbiAgICAgIGNvbnN0IGVkaXRUeXBlID0gcmVuZGVyVHJlZUVkaXQudHlwZShlZGl0KTtcclxuICAgICAgc3dpdGNoIChlZGl0VHlwZSkge1xyXG4gICAgICAgIGNhc2UgRWRpdFR5cGUucHJlcGVuZEZyYW1lOiB7XHJcbiAgICAgICAgICBjb25zdCBmcmFtZUluZGV4ID0gcmVuZGVyVHJlZUVkaXQubmV3VHJlZUluZGV4KGVkaXQpO1xyXG4gICAgICAgICAgY29uc3QgZnJhbWUgPSBnZXRUcmVlRnJhbWVQdHIocmVmZXJlbmNlRnJhbWVzLCBmcmFtZUluZGV4KTtcclxuICAgICAgICAgIGNvbnN0IHNpYmxpbmdJbmRleCA9IHJlbmRlclRyZWVFZGl0LnNpYmxpbmdJbmRleChlZGl0KTtcclxuICAgICAgICAgIHRoaXMuaW5zZXJ0RnJhbWUoY29tcG9uZW50SWQsIHBhcmVudCwgY2hpbGRJbmRleEF0Q3VycmVudERlcHRoICsgc2libGluZ0luZGV4LCByZWZlcmVuY2VGcmFtZXMsIGZyYW1lLCBmcmFtZUluZGV4KTtcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuICAgICAgICBjYXNlIEVkaXRUeXBlLnJlbW92ZUZyYW1lOiB7XHJcbiAgICAgICAgICBjb25zdCBzaWJsaW5nSW5kZXggPSByZW5kZXJUcmVlRWRpdC5zaWJsaW5nSW5kZXgoZWRpdCk7XHJcbiAgICAgICAgICByZW1vdmVOb2RlRnJvbURPTShwYXJlbnQsIGNoaWxkSW5kZXhBdEN1cnJlbnREZXB0aCArIHNpYmxpbmdJbmRleCk7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgICAgY2FzZSBFZGl0VHlwZS5zZXRBdHRyaWJ1dGU6IHtcclxuICAgICAgICAgIGNvbnN0IGZyYW1lSW5kZXggPSByZW5kZXJUcmVlRWRpdC5uZXdUcmVlSW5kZXgoZWRpdCk7XHJcbiAgICAgICAgICBjb25zdCBmcmFtZSA9IGdldFRyZWVGcmFtZVB0cihyZWZlcmVuY2VGcmFtZXMsIGZyYW1lSW5kZXgpO1xyXG4gICAgICAgICAgY29uc3Qgc2libGluZ0luZGV4ID0gcmVuZGVyVHJlZUVkaXQuc2libGluZ0luZGV4KGVkaXQpO1xyXG4gICAgICAgICAgY29uc3QgZWxlbWVudCA9IHBhcmVudC5jaGlsZE5vZGVzW2NoaWxkSW5kZXhBdEN1cnJlbnREZXB0aCArIHNpYmxpbmdJbmRleF0gYXMgSFRNTEVsZW1lbnQ7XHJcbiAgICAgICAgICB0aGlzLmFwcGx5QXR0cmlidXRlKGNvbXBvbmVudElkLCBlbGVtZW50LCBmcmFtZSk7XHJcbiAgICAgICAgICBicmVhaztcclxuICAgICAgICB9XHJcbiAgICAgICAgY2FzZSBFZGl0VHlwZS5yZW1vdmVBdHRyaWJ1dGU6IHtcclxuICAgICAgICAgIC8vIE5vdGUgdGhhdCB3ZSBkb24ndCBoYXZlIHRvIGRpc3Bvc2UgdGhlIGluZm8gd2UgdHJhY2sgYWJvdXQgZXZlbnQgaGFuZGxlcnMgaGVyZSwgYmVjYXVzZSB0aGVcclxuICAgICAgICAgIC8vIGRpc3Bvc2VkIGV2ZW50IGhhbmRsZXIgSURzIGFyZSBkZWxpdmVyZWQgc2VwYXJhdGVseSAoaW4gdGhlICdkaXNwb3NlZEV2ZW50SGFuZGxlcklkcycgYXJyYXkpXHJcbiAgICAgICAgICBjb25zdCBzaWJsaW5nSW5kZXggPSByZW5kZXJUcmVlRWRpdC5zaWJsaW5nSW5kZXgoZWRpdCk7XHJcbiAgICAgICAgICByZW1vdmVBdHRyaWJ1dGVGcm9tRE9NKHBhcmVudCwgY2hpbGRJbmRleEF0Q3VycmVudERlcHRoICsgc2libGluZ0luZGV4LCByZW5kZXJUcmVlRWRpdC5yZW1vdmVkQXR0cmlidXRlTmFtZShlZGl0KSEpO1xyXG4gICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGNhc2UgRWRpdFR5cGUudXBkYXRlVGV4dDoge1xyXG4gICAgICAgICAgY29uc3QgZnJhbWVJbmRleCA9IHJlbmRlclRyZWVFZGl0Lm5ld1RyZWVJbmRleChlZGl0KTtcclxuICAgICAgICAgIGNvbnN0IGZyYW1lID0gZ2V0VHJlZUZyYW1lUHRyKHJlZmVyZW5jZUZyYW1lcywgZnJhbWVJbmRleCk7XHJcbiAgICAgICAgICBjb25zdCBzaWJsaW5nSW5kZXggPSByZW5kZXJUcmVlRWRpdC5zaWJsaW5nSW5kZXgoZWRpdCk7XHJcbiAgICAgICAgICBjb25zdCBkb21UZXh0Tm9kZSA9IHBhcmVudC5jaGlsZE5vZGVzW2NoaWxkSW5kZXhBdEN1cnJlbnREZXB0aCArIHNpYmxpbmdJbmRleF0gYXMgVGV4dDtcclxuICAgICAgICAgIGRvbVRleHROb2RlLnRleHRDb250ZW50ID0gcmVuZGVyVHJlZUZyYW1lLnRleHRDb250ZW50KGZyYW1lKTtcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuICAgICAgICBjYXNlIEVkaXRUeXBlLnN0ZXBJbjoge1xyXG4gICAgICAgICAgY29uc3Qgc2libGluZ0luZGV4ID0gcmVuZGVyVHJlZUVkaXQuc2libGluZ0luZGV4KGVkaXQpO1xyXG4gICAgICAgICAgcGFyZW50ID0gcGFyZW50LmNoaWxkTm9kZXNbY2hpbGRJbmRleEF0Q3VycmVudERlcHRoICsgc2libGluZ0luZGV4XSBhcyBIVE1MRWxlbWVudDtcclxuICAgICAgICAgIGN1cnJlbnREZXB0aCsrO1xyXG4gICAgICAgICAgY2hpbGRJbmRleEF0Q3VycmVudERlcHRoID0gMDtcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuICAgICAgICBjYXNlIEVkaXRUeXBlLnN0ZXBPdXQ6IHtcclxuICAgICAgICAgIHBhcmVudCA9IHBhcmVudC5wYXJlbnRFbGVtZW50ITtcclxuICAgICAgICAgIGN1cnJlbnREZXB0aC0tO1xyXG4gICAgICAgICAgY2hpbGRJbmRleEF0Q3VycmVudERlcHRoID0gY3VycmVudERlcHRoID09PSAwID8gY2hpbGRJbmRleCA6IDA7IC8vIFRoZSBjaGlsZEluZGV4IGlzIG9ubHkgZXZlciBub256ZXJvIGF0IHplcm8gZGVwdGhcclxuICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgIH1cclxuICAgICAgICBkZWZhdWx0OiB7XHJcbiAgICAgICAgICBjb25zdCB1bmtub3duVHlwZTogbmV2ZXIgPSBlZGl0VHlwZTsgLy8gQ29tcGlsZS10aW1lIHZlcmlmaWNhdGlvbiB0aGF0IHRoZSBzd2l0Y2ggd2FzIGV4aGF1c3RpdmVcclxuICAgICAgICAgIHRocm93IG5ldyBFcnJvcihgVW5rbm93biBlZGl0IHR5cGU6ICR7dW5rbm93blR5cGV9YCk7XHJcbiAgICAgICAgfVxyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGluc2VydEZyYW1lKGNvbXBvbmVudElkOiBudW1iZXIsIHBhcmVudDogRWxlbWVudCwgY2hpbGRJbmRleDogbnVtYmVyLCBmcmFtZXM6IFN5c3RlbV9BcnJheTxSZW5kZXJUcmVlRnJhbWVQb2ludGVyPiwgZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIsIGZyYW1lSW5kZXg6IG51bWJlcik6IG51bWJlciB7XHJcbiAgICBjb25zdCBmcmFtZVR5cGUgPSByZW5kZXJUcmVlRnJhbWUuZnJhbWVUeXBlKGZyYW1lKTtcclxuICAgIHN3aXRjaCAoZnJhbWVUeXBlKSB7XHJcbiAgICAgIGNhc2UgRnJhbWVUeXBlLmVsZW1lbnQ6XHJcbiAgICAgICAgdGhpcy5pbnNlcnRFbGVtZW50KGNvbXBvbmVudElkLCBwYXJlbnQsIGNoaWxkSW5kZXgsIGZyYW1lcywgZnJhbWUsIGZyYW1lSW5kZXgpO1xyXG4gICAgICAgIHJldHVybiAxO1xyXG4gICAgICBjYXNlIEZyYW1lVHlwZS50ZXh0OlxyXG4gICAgICAgIHRoaXMuaW5zZXJ0VGV4dChwYXJlbnQsIGNoaWxkSW5kZXgsIGZyYW1lKTtcclxuICAgICAgICByZXR1cm4gMTtcclxuICAgICAgY2FzZSBGcmFtZVR5cGUuYXR0cmlidXRlOlxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcignQXR0cmlidXRlIGZyYW1lcyBzaG91bGQgb25seSBiZSBwcmVzZW50IGFzIGxlYWRpbmcgY2hpbGRyZW4gb2YgZWxlbWVudCBmcmFtZXMuJyk7XHJcbiAgICAgIGNhc2UgRnJhbWVUeXBlLmNvbXBvbmVudDpcclxuICAgICAgICB0aGlzLmluc2VydENvbXBvbmVudChwYXJlbnQsIGNoaWxkSW5kZXgsIGZyYW1lKTtcclxuICAgICAgICByZXR1cm4gMTtcclxuICAgICAgY2FzZSBGcmFtZVR5cGUucmVnaW9uOlxyXG4gICAgICAgIHJldHVybiB0aGlzLmluc2VydEZyYW1lUmFuZ2UoY29tcG9uZW50SWQsIHBhcmVudCwgY2hpbGRJbmRleCwgZnJhbWVzLCBmcmFtZUluZGV4ICsgMSwgZnJhbWVJbmRleCArIHJlbmRlclRyZWVGcmFtZS5zdWJ0cmVlTGVuZ3RoKGZyYW1lKSk7XHJcbiAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgY29uc3QgdW5rbm93blR5cGU6IG5ldmVyID0gZnJhbWVUeXBlOyAvLyBDb21waWxlLXRpbWUgdmVyaWZpY2F0aW9uIHRoYXQgdGhlIHN3aXRjaCB3YXMgZXhoYXVzdGl2ZVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihgVW5rbm93biBmcmFtZSB0eXBlOiAke3Vua25vd25UeXBlfWApO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBpbnNlcnRFbGVtZW50KGNvbXBvbmVudElkOiBudW1iZXIsIHBhcmVudDogRWxlbWVudCwgY2hpbGRJbmRleDogbnVtYmVyLCBmcmFtZXM6IFN5c3RlbV9BcnJheTxSZW5kZXJUcmVlRnJhbWVQb2ludGVyPiwgZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIsIGZyYW1lSW5kZXg6IG51bWJlcikge1xyXG4gICAgY29uc3QgdGFnTmFtZSA9IHJlbmRlclRyZWVGcmFtZS5lbGVtZW50TmFtZShmcmFtZSkhO1xyXG4gICAgY29uc3QgbmV3RG9tRWxlbWVudCA9IHRhZ05hbWUgPT09ICdzdmcnIHx8IHBhcmVudC5uYW1lc3BhY2VVUkkgPT09ICdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZycgP1xyXG4gICAgICBkb2N1bWVudC5jcmVhdGVFbGVtZW50TlMoJ2h0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnJywgdGFnTmFtZSkgOlxyXG4gICAgICBkb2N1bWVudC5jcmVhdGVFbGVtZW50KHRhZ05hbWUpO1xyXG4gICAgaW5zZXJ0Tm9kZUludG9ET00obmV3RG9tRWxlbWVudCwgcGFyZW50LCBjaGlsZEluZGV4KTtcclxuXHJcbiAgICAvLyBBcHBseSBhdHRyaWJ1dGVzXHJcbiAgICBjb25zdCBkZXNjZW5kYW50c0VuZEluZGV4RXhjbCA9IGZyYW1lSW5kZXggKyByZW5kZXJUcmVlRnJhbWUuc3VidHJlZUxlbmd0aChmcmFtZSk7XHJcbiAgICBmb3IgKGxldCBkZXNjZW5kYW50SW5kZXggPSBmcmFtZUluZGV4ICsgMTsgZGVzY2VuZGFudEluZGV4IDwgZGVzY2VuZGFudHNFbmRJbmRleEV4Y2w7IGRlc2NlbmRhbnRJbmRleCsrKSB7XHJcbiAgICAgIGNvbnN0IGRlc2NlbmRhbnRGcmFtZSA9IGdldFRyZWVGcmFtZVB0cihmcmFtZXMsIGRlc2NlbmRhbnRJbmRleCk7XHJcbiAgICAgIGlmIChyZW5kZXJUcmVlRnJhbWUuZnJhbWVUeXBlKGRlc2NlbmRhbnRGcmFtZSkgPT09IEZyYW1lVHlwZS5hdHRyaWJ1dGUpIHtcclxuICAgICAgICB0aGlzLmFwcGx5QXR0cmlidXRlKGNvbXBvbmVudElkLCBuZXdEb21FbGVtZW50LCBkZXNjZW5kYW50RnJhbWUpO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgIC8vIEFzIHNvb24gYXMgd2Ugc2VlIGEgbm9uLWF0dHJpYnV0ZSBjaGlsZCwgYWxsIHRoZSBzdWJzZXF1ZW50IGNoaWxkIGZyYW1lcyBhcmVcclxuICAgICAgICAvLyBub3QgYXR0cmlidXRlcywgc28gYmFpbCBvdXQgYW5kIGluc2VydCB0aGUgcmVtbmFudHMgcmVjdXJzaXZlbHlcclxuICAgICAgICB0aGlzLmluc2VydEZyYW1lUmFuZ2UoY29tcG9uZW50SWQsIG5ld0RvbUVsZW1lbnQsIDAsIGZyYW1lcywgZGVzY2VuZGFudEluZGV4LCBkZXNjZW5kYW50c0VuZEluZGV4RXhjbCk7XHJcbiAgICAgICAgYnJlYWs7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgaW5zZXJ0Q29tcG9uZW50KHBhcmVudDogRWxlbWVudCwgY2hpbGRJbmRleDogbnVtYmVyLCBmcmFtZTogUmVuZGVyVHJlZUZyYW1lUG9pbnRlcikge1xyXG4gICAgLy8gQ3VycmVudGx5LCB0byBzdXBwb3J0IE8oMSkgbG9va3VwcyBmcm9tIHJlbmRlciB0cmVlIGZyYW1lcyB0byBET00gbm9kZXMsIHdlIHJlbHkgb25cclxuICAgIC8vIGVhY2ggY2hpbGQgY29tcG9uZW50IGV4aXN0aW5nIGFzIGEgc2luZ2xlIHRvcC1sZXZlbCBlbGVtZW50IGluIHRoZSBET00uIFRvIGd1YXJhbnRlZVxyXG4gICAgLy8gdGhhdCwgd2Ugd3JhcCBjaGlsZCBjb21wb25lbnRzIGluIHRoZXNlICdibGF6b3ItY29tcG9uZW50JyB3cmFwcGVycy5cclxuICAgIC8vIFRvIGltcHJvdmUgb24gdGhpcyBpbiB0aGUgZnV0dXJlOlxyXG4gICAgLy8gLSBJZiB3ZSBjYW4gc3RhdGljYWxseSBkZXRlY3QgdGhhdCBhIGdpdmVuIGNvbXBvbmVudCBhbHdheXMgcHJvZHVjZXMgYSBzaW5nbGUgdG9wLWxldmVsXHJcbiAgICAvLyAgIGVsZW1lbnQgYW55d2F5LCB0aGVuIGRvbid0IHdyYXAgaXQgaW4gYSBmdXJ0aGVyIG5vbnN0YW5kYXJkIGVsZW1lbnRcclxuICAgIC8vIC0gSWYgd2UgcmVhbGx5IHdhbnQgdG8gc3VwcG9ydCBjaGlsZCBjb21wb25lbnRzIHByb2R1Y2luZyBtdWx0aXBsZSB0b3AtbGV2ZWwgZnJhbWVzIGFuZFxyXG4gICAgLy8gICBub3QgYmVpbmcgd3JhcHBlZCBpbiBhIGNvbnRhaW5lciBhdCBhbGwsIHRoZW4gZXZlcnkgdGltZSBhIGNvbXBvbmVudCBpcyByZWZyZXNoZWQgaW5cclxuICAgIC8vICAgdGhlIERPTSwgd2UgY291bGQgdXBkYXRlIGFuIGFycmF5IG9uIHRoZSBwYXJlbnQgZWxlbWVudCB0aGF0IHNwZWNpZmllcyBob3cgbWFueSBET01cclxuICAgIC8vICAgbm9kZXMgY29ycmVzcG9uZCB0byBlYWNoIG9mIGl0cyByZW5kZXIgdHJlZSBmcmFtZXMuIFRoZW4gd2hlbiB0aGF0IHBhcmVudCB3YW50cyB0b1xyXG4gICAgLy8gICBsb2NhdGUgdGhlIGZpcnN0IERPTSBub2RlIGZvciBhIHJlbmRlciB0cmVlIGZyYW1lLCBpdCBjYW4gc3VtIGFsbCB0aGUgZnJhbWUgY291bnRzIGZvclxyXG4gICAgLy8gICBhbGwgdGhlIHByZWNlZGluZyByZW5kZXIgdHJlZXMgZnJhbWVzLiBJdCdzIE8oTiksIGJ1dCB3aGVyZSBOIGlzIHRoZSBudW1iZXIgb2Ygc2libGluZ3NcclxuICAgIC8vICAgKGNvdW50aW5nIGNoaWxkIGNvbXBvbmVudHMgYXMgYSBzaW5nbGUgaXRlbSksIHNvIE4gd2lsbCByYXJlbHkgaWYgZXZlciBiZSBsYXJnZS5cclxuICAgIC8vICAgV2UgY291bGQgZXZlbiBrZWVwIHRyYWNrIG9mIHdoZXRoZXIgYWxsIHRoZSBjaGlsZCBjb21wb25lbnRzIGhhcHBlbiB0byBoYXZlIGV4YWN0bHkgMVxyXG4gICAgLy8gICB0b3AgbGV2ZWwgZnJhbWVzLCBhbmQgaW4gdGhhdCBjYXNlLCB0aGVyZSdzIG5vIG5lZWQgdG8gc3VtIGFzIHdlIGNhbiBkbyBkaXJlY3QgbG9va3Vwcy5cclxuICAgIGNvbnN0IGNvbnRhaW5lckVsZW1lbnQgPSBwYXJlbnQubmFtZXNwYWNlVVJJID09PSAnaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmcnID9cclxuICAgICAgZG9jdW1lbnQuY3JlYXRlRWxlbWVudE5TKCdodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZycsICdnJykgOlxyXG4gICAgICBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdibGF6b3ItY29tcG9uZW50Jyk7XHJcbiAgICBpbnNlcnROb2RlSW50b0RPTShjb250YWluZXJFbGVtZW50LCBwYXJlbnQsIGNoaWxkSW5kZXgpO1xyXG5cclxuICAgIC8vIEFsbCB3ZSBoYXZlIHRvIGRvIGlzIGFzc29jaWF0ZSB0aGUgY2hpbGQgY29tcG9uZW50IElEIHdpdGggaXRzIGxvY2F0aW9uLiBXZSBkb24ndCBhY3R1YWxseVxyXG4gICAgLy8gZG8gYW55IHJlbmRlcmluZyBoZXJlLCBiZWNhdXNlIHRoZSBkaWZmIGZvciB0aGUgY2hpbGQgd2lsbCBhcHBlYXIgbGF0ZXIgaW4gdGhlIHJlbmRlciBiYXRjaC5cclxuICAgIGNvbnN0IGNoaWxkQ29tcG9uZW50SWQgPSByZW5kZXJUcmVlRnJhbWUuY29tcG9uZW50SWQoZnJhbWUpO1xyXG4gICAgdGhpcy5hdHRhY2hDb21wb25lbnRUb0VsZW1lbnQoY2hpbGRDb21wb25lbnRJZCwgY29udGFpbmVyRWxlbWVudCk7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIGluc2VydFRleHQocGFyZW50OiBFbGVtZW50LCBjaGlsZEluZGV4OiBudW1iZXIsIHRleHRGcmFtZTogUmVuZGVyVHJlZUZyYW1lUG9pbnRlcikge1xyXG4gICAgY29uc3QgdGV4dENvbnRlbnQgPSByZW5kZXJUcmVlRnJhbWUudGV4dENvbnRlbnQodGV4dEZyYW1lKSE7XHJcbiAgICBjb25zdCBuZXdEb21UZXh0Tm9kZSA9IGRvY3VtZW50LmNyZWF0ZVRleHROb2RlKHRleHRDb250ZW50KTtcclxuICAgIGluc2VydE5vZGVJbnRvRE9NKG5ld0RvbVRleHROb2RlLCBwYXJlbnQsIGNoaWxkSW5kZXgpO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBhcHBseUF0dHJpYnV0ZShjb21wb25lbnRJZDogbnVtYmVyLCB0b0RvbUVsZW1lbnQ6IEVsZW1lbnQsIGF0dHJpYnV0ZUZyYW1lOiBSZW5kZXJUcmVlRnJhbWVQb2ludGVyKSB7XHJcbiAgICBjb25zdCBhdHRyaWJ1dGVOYW1lID0gcmVuZGVyVHJlZUZyYW1lLmF0dHJpYnV0ZU5hbWUoYXR0cmlidXRlRnJhbWUpITtcclxuICAgIGNvbnN0IGJyb3dzZXJSZW5kZXJlcklkID0gdGhpcy5icm93c2VyUmVuZGVyZXJJZDtcclxuICAgIGNvbnN0IGV2ZW50SGFuZGxlcklkID0gcmVuZGVyVHJlZUZyYW1lLmF0dHJpYnV0ZUV2ZW50SGFuZGxlcklkKGF0dHJpYnV0ZUZyYW1lKTtcclxuXHJcbiAgICBpZiAoZXZlbnRIYW5kbGVySWQpIHtcclxuICAgICAgY29uc3QgZmlyc3RUd29DaGFycyA9IGF0dHJpYnV0ZU5hbWUuc3Vic3RyaW5nKDAsIDIpO1xyXG4gICAgICBjb25zdCBldmVudE5hbWUgPSBhdHRyaWJ1dGVOYW1lLnN1YnN0cmluZygyKTtcclxuICAgICAgaWYgKGZpcnN0VHdvQ2hhcnMgIT09ICdvbicgfHwgIWV2ZW50TmFtZSkge1xyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihgQXR0cmlidXRlIGhhcyBub256ZXJvIGV2ZW50IGhhbmRsZXIgSUQsIGJ1dCBhdHRyaWJ1dGUgbmFtZSAnJHthdHRyaWJ1dGVOYW1lfScgZG9lcyBub3Qgc3RhcnQgd2l0aCAnb24nLmApO1xyXG4gICAgICB9XHJcbiAgICAgIHRoaXMuZXZlbnREZWxlZ2F0b3Iuc2V0TGlzdGVuZXIodG9Eb21FbGVtZW50LCBldmVudE5hbWUsIGNvbXBvbmVudElkLCBldmVudEhhbmRsZXJJZCk7XHJcbiAgICAgIHJldHVybjtcclxuICAgIH1cclxuXHJcbiAgICBpZiAoYXR0cmlidXRlTmFtZSA9PT0gJ3ZhbHVlJykge1xyXG4gICAgICBpZiAodGhpcy50cnlBcHBseVZhbHVlUHJvcGVydHkodG9Eb21FbGVtZW50LCByZW5kZXJUcmVlRnJhbWUuYXR0cmlidXRlVmFsdWUoYXR0cmlidXRlRnJhbWUpKSkge1xyXG4gICAgICAgIHJldHVybjsgLy8gSWYgdGhpcyBET00gZWxlbWVudCB0eXBlIGhhcyBzcGVjaWFsICd2YWx1ZScgaGFuZGxpbmcsIGRvbid0IGFsc28gd3JpdGUgaXQgYXMgYW4gYXR0cmlidXRlXHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICAvLyBUcmVhdCBhcyBhIHJlZ3VsYXIgc3RyaW5nLXZhbHVlZCBhdHRyaWJ1dGVcclxuICAgIHRvRG9tRWxlbWVudC5zZXRBdHRyaWJ1dGUoXHJcbiAgICAgIGF0dHJpYnV0ZU5hbWUsXHJcbiAgICAgIHJlbmRlclRyZWVGcmFtZS5hdHRyaWJ1dGVWYWx1ZShhdHRyaWJ1dGVGcmFtZSkhXHJcbiAgICApO1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSB0cnlBcHBseVZhbHVlUHJvcGVydHkoZWxlbWVudDogRWxlbWVudCwgdmFsdWU6IHN0cmluZyB8IG51bGwpIHtcclxuICAgIC8vIENlcnRhaW4gZWxlbWVudHMgaGF2ZSBidWlsdC1pbiBiZWhhdmlvdXIgZm9yIHRoZWlyICd2YWx1ZScgcHJvcGVydHlcclxuICAgIHN3aXRjaCAoZWxlbWVudC50YWdOYW1lKSB7XHJcbiAgICAgIGNhc2UgJ0lOUFVUJzpcclxuICAgICAgY2FzZSAnU0VMRUNUJzpcclxuICAgICAgY2FzZSAnVEVYVEFSRUEnOlxyXG4gICAgICAgIGlmIChpc0NoZWNrYm94KGVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAoZWxlbWVudCBhcyBIVE1MSW5wdXRFbGVtZW50KS5jaGVja2VkID0gdmFsdWUgPT09ICcnO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAoZWxlbWVudCBhcyBhbnkpLnZhbHVlID0gdmFsdWU7XHJcblxyXG4gICAgICAgICAgaWYgKGVsZW1lbnQudGFnTmFtZSA9PT0gJ1NFTEVDVCcpIHtcclxuICAgICAgICAgICAgLy8gPHNlbGVjdD4gaXMgc3BlY2lhbCwgaW4gdGhhdCBhbnl0aGluZyB3ZSB3cml0ZSB0byAudmFsdWUgd2lsbCBiZSBsb3N0IGlmIHRoZXJlXHJcbiAgICAgICAgICAgIC8vIGlzbid0IHlldCBhIG1hdGNoaW5nIDxvcHRpb24+LiBUbyBtYWludGFpbiB0aGUgZXhwZWN0ZWQgYmVoYXZpb3Igbm8gbWF0dGVyIHRoZVxyXG4gICAgICAgICAgICAvLyBlbGVtZW50IGluc2VydGlvbi91cGRhdGUgb3JkZXIsIHByZXNlcnZlIHRoZSBkZXNpcmVkIHZhbHVlIHNlcGFyYXRlbHkgc29cclxuICAgICAgICAgICAgLy8gd2UgY2FuIHJlY292ZXIgaXQgd2hlbiBpbnNlcnRpbmcgYW55IG1hdGNoaW5nIDxvcHRpb24+LlxyXG4gICAgICAgICAgICBlbGVtZW50W3NlbGVjdFZhbHVlUHJvcG5hbWVdID0gdmFsdWU7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgICBjYXNlICdPUFRJT04nOlxyXG4gICAgICAgIGVsZW1lbnQuc2V0QXR0cmlidXRlKCd2YWx1ZScsIHZhbHVlISk7XHJcbiAgICAgICAgLy8gU2VlIGFib3ZlIGZvciB3aHkgd2UgaGF2ZSB0aGlzIHNwZWNpYWwgaGFuZGxpbmcgZm9yIDxzZWxlY3Q+LzxvcHRpb24+XHJcbiAgICAgICAgY29uc3QgcGFyZW50RWxlbWVudCA9IGVsZW1lbnQucGFyZW50RWxlbWVudDtcclxuICAgICAgICBpZiAocGFyZW50RWxlbWVudCAmJiAoc2VsZWN0VmFsdWVQcm9wbmFtZSBpbiBwYXJlbnRFbGVtZW50KSAmJiBwYXJlbnRFbGVtZW50W3NlbGVjdFZhbHVlUHJvcG5hbWVdID09PSB2YWx1ZSkge1xyXG4gICAgICAgICAgdGhpcy50cnlBcHBseVZhbHVlUHJvcGVydHkocGFyZW50RWxlbWVudCwgdmFsdWUpO1xyXG4gICAgICAgICAgZGVsZXRlIHBhcmVudEVsZW1lbnRbc2VsZWN0VmFsdWVQcm9wbmFtZV07XHJcbiAgICAgICAgfVxyXG4gICAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgICBkZWZhdWx0OlxyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIHByaXZhdGUgaW5zZXJ0RnJhbWVSYW5nZShjb21wb25lbnRJZDogbnVtYmVyLCBwYXJlbnQ6IEVsZW1lbnQsIGNoaWxkSW5kZXg6IG51bWJlciwgZnJhbWVzOiBTeXN0ZW1fQXJyYXk8UmVuZGVyVHJlZUZyYW1lUG9pbnRlcj4sIHN0YXJ0SW5kZXg6IG51bWJlciwgZW5kSW5kZXhFeGNsOiBudW1iZXIpOiBudW1iZXIge1xyXG4gICAgY29uc3Qgb3JpZ0NoaWxkSW5kZXggPSBjaGlsZEluZGV4O1xyXG4gICAgZm9yIChsZXQgaW5kZXggPSBzdGFydEluZGV4OyBpbmRleCA8IGVuZEluZGV4RXhjbDsgaW5kZXgrKykge1xyXG4gICAgICBjb25zdCBmcmFtZSA9IGdldFRyZWVGcmFtZVB0cihmcmFtZXMsIGluZGV4KTtcclxuICAgICAgY29uc3QgbnVtQ2hpbGRyZW5JbnNlcnRlZCA9IHRoaXMuaW5zZXJ0RnJhbWUoY29tcG9uZW50SWQsIHBhcmVudCwgY2hpbGRJbmRleCwgZnJhbWVzLCBmcmFtZSwgaW5kZXgpO1xyXG4gICAgICBjaGlsZEluZGV4ICs9IG51bUNoaWxkcmVuSW5zZXJ0ZWQ7XHJcblxyXG4gICAgICAvLyBTa2lwIG92ZXIgYW55IGRlc2NlbmRhbnRzLCBzaW5jZSB0aGV5IGFyZSBhbHJlYWR5IGRlYWx0IHdpdGggcmVjdXJzaXZlbHlcclxuICAgICAgY29uc3Qgc3VidHJlZUxlbmd0aCA9IHJlbmRlclRyZWVGcmFtZS5zdWJ0cmVlTGVuZ3RoKGZyYW1lKTtcclxuICAgICAgaWYgKHN1YnRyZWVMZW5ndGggPiAxKSB7XHJcbiAgICAgICAgaW5kZXggKz0gc3VidHJlZUxlbmd0aCAtIDE7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gKGNoaWxkSW5kZXggLSBvcmlnQ2hpbGRJbmRleCk7IC8vIFRvdGFsIG51bWJlciBvZiBjaGlsZHJlbiBpbnNlcnRlZFxyXG4gIH1cclxufVxyXG5cclxuZnVuY3Rpb24gaXNDaGVja2JveChlbGVtZW50OiBFbGVtZW50KSB7XHJcbiAgcmV0dXJuIGVsZW1lbnQudGFnTmFtZSA9PT0gJ0lOUFVUJyAmJiBlbGVtZW50LmdldEF0dHJpYnV0ZSgndHlwZScpID09PSAnY2hlY2tib3gnO1xyXG59XHJcblxyXG5mdW5jdGlvbiBpbnNlcnROb2RlSW50b0RPTShub2RlOiBOb2RlLCBwYXJlbnQ6IEVsZW1lbnQsIGNoaWxkSW5kZXg6IG51bWJlcikge1xyXG4gIGlmIChjaGlsZEluZGV4ID49IHBhcmVudC5jaGlsZE5vZGVzLmxlbmd0aCkge1xyXG4gICAgcGFyZW50LmFwcGVuZENoaWxkKG5vZGUpO1xyXG4gIH0gZWxzZSB7XHJcbiAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKG5vZGUsIHBhcmVudC5jaGlsZE5vZGVzW2NoaWxkSW5kZXhdKTtcclxuICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHJlbW92ZU5vZGVGcm9tRE9NKHBhcmVudDogRWxlbWVudCwgY2hpbGRJbmRleDogbnVtYmVyKSB7XHJcbiAgcGFyZW50LnJlbW92ZUNoaWxkKHBhcmVudC5jaGlsZE5vZGVzW2NoaWxkSW5kZXhdKTtcclxufVxyXG5cclxuZnVuY3Rpb24gcmVtb3ZlQXR0cmlidXRlRnJvbURPTShwYXJlbnQ6IEVsZW1lbnQsIGNoaWxkSW5kZXg6IG51bWJlciwgYXR0cmlidXRlTmFtZTogc3RyaW5nKSB7XHJcbiAgY29uc3QgZWxlbWVudCA9IHBhcmVudC5jaGlsZE5vZGVzW2NoaWxkSW5kZXhdIGFzIEVsZW1lbnQ7XHJcbiAgZWxlbWVudC5yZW1vdmVBdHRyaWJ1dGUoYXR0cmlidXRlTmFtZSk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHJhaXNlRXZlbnQoZXZlbnQ6IEV2ZW50LCBicm93c2VyUmVuZGVyZXJJZDogbnVtYmVyLCBjb21wb25lbnRJZDogbnVtYmVyLCBldmVudEhhbmRsZXJJZDogbnVtYmVyLCBldmVudEFyZ3M6IEV2ZW50Rm9yRG90TmV0PFVJRXZlbnRBcmdzPikge1xyXG4gIGV2ZW50LnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gIGlmICghcmFpc2VFdmVudE1ldGhvZCkge1xyXG4gICAgcmFpc2VFdmVudE1ldGhvZCA9IHBsYXRmb3JtLmZpbmRNZXRob2QoXHJcbiAgICAgICdNaWNyb3NvZnQuQXNwTmV0Q29yZS5CbGF6b3IuQnJvd3NlcicsICdNaWNyb3NvZnQuQXNwTmV0Q29yZS5CbGF6b3IuQnJvd3Nlci5SZW5kZXJpbmcnLCAnQnJvd3NlclJlbmRlcmVyRXZlbnREaXNwYXRjaGVyJywgJ0Rpc3BhdGNoRXZlbnQnXHJcbiAgICApO1xyXG4gIH1cclxuXHJcbiAgY29uc3QgZXZlbnREZXNjcmlwdG9yID0ge1xyXG4gICAgQnJvd3NlclJlbmRlcmVySWQ6IGJyb3dzZXJSZW5kZXJlcklkLFxyXG4gICAgQ29tcG9uZW50SWQ6IGNvbXBvbmVudElkLFxyXG4gICAgRXZlbnRIYW5kbGVySWQ6IGV2ZW50SGFuZGxlcklkLFxyXG4gICAgRXZlbnRBcmdzVHlwZTogZXZlbnRBcmdzLnR5cGVcclxuICB9O1xyXG5cclxuICBwbGF0Zm9ybS5jYWxsTWV0aG9kKHJhaXNlRXZlbnRNZXRob2QsIG51bGwsIFtcclxuICAgIHBsYXRmb3JtLnRvRG90TmV0U3RyaW5nKEpTT04uc3RyaW5naWZ5KGV2ZW50RGVzY3JpcHRvcikpLFxyXG4gICAgcGxhdGZvcm0udG9Eb3ROZXRTdHJpbmcoSlNPTi5zdHJpbmdpZnkoZXZlbnRBcmdzLmRhdGEpKVxyXG4gIF0pO1xyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9SZW5kZXJpbmcvQnJvd3NlclJlbmRlcmVyLnRzIiwiaW1wb3J0IHsgU3lzdGVtX0FycmF5LCBQb2ludGVyIH0gZnJvbSAnLi4vUGxhdGZvcm0vUGxhdGZvcm0nO1xyXG5pbXBvcnQgeyBwbGF0Zm9ybSB9IGZyb20gJy4uL0Vudmlyb25tZW50JztcclxuY29uc3QgcmVuZGVyVHJlZUVkaXRTdHJ1Y3RMZW5ndGggPSAxNjtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRSZW5kZXJUcmVlRWRpdFB0cihyZW5kZXJUcmVlRWRpdHM6IFN5c3RlbV9BcnJheTxSZW5kZXJUcmVlRWRpdFBvaW50ZXI+LCBpbmRleDogbnVtYmVyKSB7XHJcbiAgcmV0dXJuIHBsYXRmb3JtLmdldEFycmF5RW50cnlQdHIocmVuZGVyVHJlZUVkaXRzLCBpbmRleCwgcmVuZGVyVHJlZUVkaXRTdHJ1Y3RMZW5ndGgpO1xyXG59XHJcblxyXG5leHBvcnQgY29uc3QgcmVuZGVyVHJlZUVkaXQgPSB7XHJcbiAgLy8gVGhlIHByb3BlcnRpZXMgYW5kIG1lbW9yeSBsYXlvdXQgbXVzdCBiZSBrZXB0IGluIHN5bmMgd2l0aCB0aGUgLk5FVCBlcXVpdmFsZW50IGluIFJlbmRlclRyZWVFZGl0LmNzXHJcbiAgdHlwZTogKGVkaXQ6IFJlbmRlclRyZWVFZGl0UG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZEludDMyRmllbGQoZWRpdCwgMCkgYXMgRWRpdFR5cGUsXHJcbiAgc2libGluZ0luZGV4OiAoZWRpdDogUmVuZGVyVHJlZUVkaXRQb2ludGVyKSA9PiBwbGF0Zm9ybS5yZWFkSW50MzJGaWVsZChlZGl0LCA0KSxcclxuICBuZXdUcmVlSW5kZXg6IChlZGl0OiBSZW5kZXJUcmVlRWRpdFBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRJbnQzMkZpZWxkKGVkaXQsIDgpLFxyXG4gIHJlbW92ZWRBdHRyaWJ1dGVOYW1lOiAoZWRpdDogUmVuZGVyVHJlZUVkaXRQb2ludGVyKSA9PiBwbGF0Zm9ybS5yZWFkU3RyaW5nRmllbGQoZWRpdCwgMTIpLFxyXG59O1xyXG5cclxuZXhwb3J0IGVudW0gRWRpdFR5cGUge1xyXG4gIHByZXBlbmRGcmFtZSA9IDEsXHJcbiAgcmVtb3ZlRnJhbWUgPSAyLFxyXG4gIHNldEF0dHJpYnV0ZSA9IDMsXHJcbiAgcmVtb3ZlQXR0cmlidXRlID0gNCxcclxuICB1cGRhdGVUZXh0ID0gNSxcclxuICBzdGVwSW4gPSA2LFxyXG4gIHN0ZXBPdXQgPSA3LFxyXG59XHJcblxyXG4vLyBOb21pbmFsIHR5cGUgdG8gZW5zdXJlIG9ubHkgdmFsaWQgcG9pbnRlcnMgYXJlIHBhc3NlZCB0byB0aGUgcmVuZGVyVHJlZUVkaXQgZnVuY3Rpb25zLlxyXG4vLyBBdCBydW50aW1lIHRoZSB2YWx1ZXMgYXJlIGp1c3QgbnVtYmVycy5cclxuZXhwb3J0IGludGVyZmFjZSBSZW5kZXJUcmVlRWRpdFBvaW50ZXIgZXh0ZW5kcyBQb2ludGVyIHsgUmVuZGVyVHJlZUVkaXRQb2ludGVyX19ET19OT1RfSU1QTEVNRU5UOiBhbnkgfVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvUmVuZGVyaW5nL1JlbmRlclRyZWVFZGl0LnRzIiwiaW1wb3J0IHsgU3lzdGVtX1N0cmluZywgU3lzdGVtX0FycmF5LCBQb2ludGVyIH0gZnJvbSAnLi4vUGxhdGZvcm0vUGxhdGZvcm0nO1xyXG5pbXBvcnQgeyBwbGF0Zm9ybSB9IGZyb20gJy4uL0Vudmlyb25tZW50JztcclxuY29uc3QgcmVuZGVyVHJlZUZyYW1lU3RydWN0TGVuZ3RoID0gMjg7XHJcblxyXG4vLyBUbyBtaW5pbWlzZSBHQyBwcmVzc3VyZSwgaW5zdGVhZCBvZiBpbnN0YW50aWF0aW5nIGEgSlMgb2JqZWN0IHRvIHJlcHJlc2VudCBlYWNoIHRyZWUgZnJhbWUsXHJcbi8vIHdlIHdvcmsgaW4gdGVybXMgb2YgcG9pbnRlcnMgdG8gdGhlIHN0cnVjdHMgb24gdGhlIC5ORVQgaGVhcCwgYW5kIHVzZSBzdGF0aWMgZnVuY3Rpb25zIHRoYXRcclxuLy8ga25vdyBob3cgdG8gcmVhZCBwcm9wZXJ0eSB2YWx1ZXMgZnJvbSB0aG9zZSBzdHJ1Y3RzLlxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRyZWVGcmFtZVB0cihyZW5kZXJUcmVlRW50cmllczogU3lzdGVtX0FycmF5PFJlbmRlclRyZWVGcmFtZVBvaW50ZXI+LCBpbmRleDogbnVtYmVyKSB7XHJcbiAgcmV0dXJuIHBsYXRmb3JtLmdldEFycmF5RW50cnlQdHIocmVuZGVyVHJlZUVudHJpZXMsIGluZGV4LCByZW5kZXJUcmVlRnJhbWVTdHJ1Y3RMZW5ndGgpO1xyXG59XHJcblxyXG5leHBvcnQgY29uc3QgcmVuZGVyVHJlZUZyYW1lID0ge1xyXG4gIC8vIFRoZSBwcm9wZXJ0aWVzIGFuZCBtZW1vcnkgbGF5b3V0IG11c3QgYmUga2VwdCBpbiBzeW5jIHdpdGggdGhlIC5ORVQgZXF1aXZhbGVudCBpbiBSZW5kZXJUcmVlRnJhbWUuY3NcclxuICBmcmFtZVR5cGU6IChmcmFtZTogUmVuZGVyVHJlZUZyYW1lUG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZEludDMyRmllbGQoZnJhbWUsIDQpIGFzIEZyYW1lVHlwZSxcclxuICBzdWJ0cmVlTGVuZ3RoOiAoZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRJbnQzMkZpZWxkKGZyYW1lLCA4KSBhcyBGcmFtZVR5cGUsXHJcbiAgY29tcG9uZW50SWQ6IChmcmFtZTogUmVuZGVyVHJlZUZyYW1lUG9pbnRlcikgPT4gcGxhdGZvcm0ucmVhZEludDMyRmllbGQoZnJhbWUsIDEyKSxcclxuICBlbGVtZW50TmFtZTogKGZyYW1lOiBSZW5kZXJUcmVlRnJhbWVQb2ludGVyKSA9PiBwbGF0Zm9ybS5yZWFkU3RyaW5nRmllbGQoZnJhbWUsIDE2KSxcclxuICB0ZXh0Q29udGVudDogKGZyYW1lOiBSZW5kZXJUcmVlRnJhbWVQb2ludGVyKSA9PiBwbGF0Zm9ybS5yZWFkU3RyaW5nRmllbGQoZnJhbWUsIDE2KSxcclxuICBhdHRyaWJ1dGVOYW1lOiAoZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRTdHJpbmdGaWVsZChmcmFtZSwgMTYpLFxyXG4gIGF0dHJpYnV0ZVZhbHVlOiAoZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRTdHJpbmdGaWVsZChmcmFtZSwgMjQpLFxyXG4gIGF0dHJpYnV0ZUV2ZW50SGFuZGxlcklkOiAoZnJhbWU6IFJlbmRlclRyZWVGcmFtZVBvaW50ZXIpID0+IHBsYXRmb3JtLnJlYWRJbnQzMkZpZWxkKGZyYW1lLCA4KSxcclxufTtcclxuXHJcbmV4cG9ydCBlbnVtIEZyYW1lVHlwZSB7XHJcbiAgLy8gVGhlIHZhbHVlcyBtdXN0IGJlIGtlcHQgaW4gc3luYyB3aXRoIHRoZSAuTkVUIGVxdWl2YWxlbnQgaW4gUmVuZGVyVHJlZUZyYW1lVHlwZS5jc1xyXG4gIGVsZW1lbnQgPSAxLFxyXG4gIHRleHQgPSAyLFxyXG4gIGF0dHJpYnV0ZSA9IDMsXHJcbiAgY29tcG9uZW50ID0gNCxcclxuICByZWdpb24gPSA1LFxyXG59XHJcblxyXG4vLyBOb21pbmFsIHR5cGUgdG8gZW5zdXJlIG9ubHkgdmFsaWQgcG9pbnRlcnMgYXJlIHBhc3NlZCB0byB0aGUgcmVuZGVyVHJlZUZyYW1lIGZ1bmN0aW9ucy5cclxuLy8gQXQgcnVudGltZSB0aGUgdmFsdWVzIGFyZSBqdXN0IG51bWJlcnMuXHJcbmV4cG9ydCBpbnRlcmZhY2UgUmVuZGVyVHJlZUZyYW1lUG9pbnRlciBleHRlbmRzIFBvaW50ZXIgeyBSZW5kZXJUcmVlRnJhbWVQb2ludGVyX19ET19OT1RfSU1QTEVNRU5UOiBhbnkgfVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvUmVuZGVyaW5nL1JlbmRlclRyZWVGcmFtZS50cyIsImltcG9ydCB7IEV2ZW50Rm9yRG90TmV0LCBVSUV2ZW50QXJncyB9IGZyb20gJy4vRXZlbnRGb3JEb3ROZXQnO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBPbkV2ZW50Q2FsbGJhY2sge1xyXG4gIChldmVudDogRXZlbnQsIGNvbXBvbmVudElkOiBudW1iZXIsIGV2ZW50SGFuZGxlcklkOiBudW1iZXIsIGV2ZW50QXJnczogRXZlbnRGb3JEb3ROZXQ8VUlFdmVudEFyZ3M+KTogdm9pZDtcclxufVxyXG5cclxuLy8gUmVzcG9uc2libGUgZm9yIGFkZGluZy9yZW1vdmluZyB0aGUgZXZlbnRJbmZvIG9uIGFuIGV4cGFuZG8gcHJvcGVydHkgb24gRE9NIGVsZW1lbnRzLCBhbmRcclxuLy8gY2FsbGluZyBhbiBFdmVudEluZm9TdG9yZSB0aGF0IGRlYWxzIHdpdGggcmVnaXN0ZXJpbmcvdW5yZWdpc3RlcmluZyB0aGUgdW5kZXJseWluZyBkZWxlZ2F0ZWRcclxuLy8gZXZlbnQgbGlzdGVuZXJzIGFzIHJlcXVpcmVkIChhbmQgYWxzbyBtYXBzIGFjdHVhbCBldmVudHMgYmFjayB0byB0aGUgZ2l2ZW4gY2FsbGJhY2spLlxyXG5leHBvcnQgY2xhc3MgRXZlbnREZWxlZ2F0b3Ige1xyXG4gIHByaXZhdGUgc3RhdGljIG5leHRFdmVudERlbGVnYXRvcklkID0gMDtcclxuICBwcml2YXRlIGV2ZW50c0NvbGxlY3Rpb25LZXk6IHN0cmluZztcclxuICBwcml2YXRlIGV2ZW50SW5mb1N0b3JlOiBFdmVudEluZm9TdG9yZTtcclxuXHJcbiAgY29uc3RydWN0b3IocHJpdmF0ZSBvbkV2ZW50OiBPbkV2ZW50Q2FsbGJhY2spIHtcclxuICAgIGNvbnN0IGV2ZW50RGVsZWdhdG9ySWQgPSArK0V2ZW50RGVsZWdhdG9yLm5leHRFdmVudERlbGVnYXRvcklkO1xyXG4gICAgdGhpcy5ldmVudHNDb2xsZWN0aW9uS2V5ID0gYF9ibGF6b3JFdmVudHNfJHtldmVudERlbGVnYXRvcklkfWA7XHJcbiAgICB0aGlzLmV2ZW50SW5mb1N0b3JlID0gbmV3IEV2ZW50SW5mb1N0b3JlKHRoaXMub25HbG9iYWxFdmVudC5iaW5kKHRoaXMpKTtcclxuICB9XHJcblxyXG4gIHB1YmxpYyBzZXRMaXN0ZW5lcihlbGVtZW50OiBFbGVtZW50LCBldmVudE5hbWU6IHN0cmluZywgY29tcG9uZW50SWQ6IG51bWJlciwgZXZlbnRIYW5kbGVySWQ6IG51bWJlcikge1xyXG4gICAgLy8gRW5zdXJlIHdlIGhhdmUgYSBwbGFjZSB0byBzdG9yZSBldmVudCBpbmZvIGZvciB0aGlzIGVsZW1lbnRcclxuICAgIGxldCBpbmZvRm9yRWxlbWVudDogRXZlbnRIYW5kbGVySW5mb3NGb3JFbGVtZW50ID0gZWxlbWVudFt0aGlzLmV2ZW50c0NvbGxlY3Rpb25LZXldO1xyXG4gICAgaWYgKCFpbmZvRm9yRWxlbWVudCkge1xyXG4gICAgICBpbmZvRm9yRWxlbWVudCA9IGVsZW1lbnRbdGhpcy5ldmVudHNDb2xsZWN0aW9uS2V5XSA9IHt9O1xyXG4gICAgfVxyXG5cclxuICAgIGlmIChpbmZvRm9yRWxlbWVudC5oYXNPd25Qcm9wZXJ0eShldmVudE5hbWUpKSB7XHJcbiAgICAgIC8vIFdlIGNhbiBjaGVhcGx5IHVwZGF0ZSB0aGUgaW5mbyBvbiB0aGUgZXhpc3Rpbmcgb2JqZWN0IGFuZCBkb24ndCBuZWVkIGFueSBvdGhlciBob3VzZWtlZXBpbmdcclxuICAgICAgY29uc3Qgb2xkSW5mbyA9IGluZm9Gb3JFbGVtZW50W2V2ZW50TmFtZV07XHJcbiAgICAgIHRoaXMuZXZlbnRJbmZvU3RvcmUudXBkYXRlKG9sZEluZm8uZXZlbnRIYW5kbGVySWQsIGV2ZW50SGFuZGxlcklkKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIC8vIEdvIHRocm91Z2ggdGhlIHdob2xlIGZsb3cgd2hpY2ggbWlnaHQgaW52b2x2ZSByZWdpc3RlcmluZyBhIG5ldyBnbG9iYWwgaGFuZGxlclxyXG4gICAgICBjb25zdCBuZXdJbmZvID0geyBlbGVtZW50LCBldmVudE5hbWUsIGNvbXBvbmVudElkLCBldmVudEhhbmRsZXJJZCB9O1xyXG4gICAgICB0aGlzLmV2ZW50SW5mb1N0b3JlLmFkZChuZXdJbmZvKTtcclxuICAgICAgaW5mb0ZvckVsZW1lbnRbZXZlbnROYW1lXSA9IG5ld0luZm87XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgcmVtb3ZlTGlzdGVuZXIoZXZlbnRIYW5kbGVySWQ6IG51bWJlcikge1xyXG4gICAgLy8gVGhpcyBtZXRob2QgZ2V0cyBjYWxsZWQgd2hlbmV2ZXIgdGhlIC5ORVQtc2lkZSBjb2RlIHJlcG9ydHMgdGhhdCBhIGNlcnRhaW4gZXZlbnQgaGFuZGxlclxyXG4gICAgLy8gaGFzIGJlZW4gZGlzcG9zZWQuIEhvd2V2ZXIgd2Ugd2lsbCBhbHJlYWR5IGhhdmUgZGlzcG9zZWQgdGhlIGluZm8gYWJvdXQgdGhhdCBoYW5kbGVyIGlmXHJcbiAgICAvLyB0aGUgZXZlbnRIYW5kbGVySWQgZm9yIHRoZSAoZWxlbWVudCxldmVudE5hbWUpIHBhaXIgd2FzIHJlcGxhY2VkIGR1cmluZyBkaWZmIGFwcGxpY2F0aW9uLlxyXG4gICAgY29uc3QgaW5mbyA9IHRoaXMuZXZlbnRJbmZvU3RvcmUucmVtb3ZlKGV2ZW50SGFuZGxlcklkKTtcclxuICAgIGlmIChpbmZvKSB7XHJcbiAgICAgIC8vIExvb2tzIGxpa2UgdGhpcyBldmVudCBoYW5kbGVyIHdhc24ndCBhbHJlYWR5IGRpc3Bvc2VkXHJcbiAgICAgIC8vIFJlbW92ZSB0aGUgYXNzb2NpYXRlZCBkYXRhIGZyb20gdGhlIERPTSBlbGVtZW50XHJcbiAgICAgIGNvbnN0IGVsZW1lbnQgPSBpbmZvLmVsZW1lbnQ7XHJcbiAgICAgIGlmIChlbGVtZW50Lmhhc093blByb3BlcnR5KHRoaXMuZXZlbnRzQ29sbGVjdGlvbktleSkpIHtcclxuICAgICAgICBjb25zdCBlbGVtZW50RXZlbnRJbmZvczogRXZlbnRIYW5kbGVySW5mb3NGb3JFbGVtZW50ID0gZWxlbWVudFt0aGlzLmV2ZW50c0NvbGxlY3Rpb25LZXldO1xyXG4gICAgICAgIGRlbGV0ZSBlbGVtZW50RXZlbnRJbmZvc1tpbmZvLmV2ZW50TmFtZV07XHJcbiAgICAgICAgaWYgKE9iamVjdC5nZXRPd25Qcm9wZXJ0eU5hbWVzKGVsZW1lbnRFdmVudEluZm9zKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICAgIGRlbGV0ZSBlbGVtZW50W3RoaXMuZXZlbnRzQ29sbGVjdGlvbktleV07XHJcbiAgICAgICAgfVxyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIG9uR2xvYmFsRXZlbnQoZXZ0OiBFdmVudCkge1xyXG4gICAgaWYgKCEoZXZ0LnRhcmdldCBpbnN0YW5jZW9mIEVsZW1lbnQpKSB7XHJcbiAgICAgIHJldHVybjtcclxuICAgIH1cclxuXHJcbiAgICAvLyBTY2FuIHVwIHRoZSBlbGVtZW50IGhpZXJhcmNoeSwgbG9va2luZyBmb3IgYW55IG1hdGNoaW5nIHJlZ2lzdGVyZWQgZXZlbnQgaGFuZGxlcnNcclxuICAgIGxldCBjYW5kaWRhdGVFbGVtZW50ID0gZXZ0LnRhcmdldCBhcyBFbGVtZW50IHwgbnVsbDtcclxuICAgIGxldCBldmVudEFyZ3M6IEV2ZW50Rm9yRG90TmV0PFVJRXZlbnRBcmdzPiB8IG51bGwgPSBudWxsOyAvLyBQb3B1bGF0ZSBsYXppbHlcclxuICAgIHdoaWxlIChjYW5kaWRhdGVFbGVtZW50KSB7XHJcbiAgICAgIGlmIChjYW5kaWRhdGVFbGVtZW50Lmhhc093blByb3BlcnR5KHRoaXMuZXZlbnRzQ29sbGVjdGlvbktleSkpIHtcclxuICAgICAgICBjb25zdCBoYW5kbGVySW5mb3MgPSBjYW5kaWRhdGVFbGVtZW50W3RoaXMuZXZlbnRzQ29sbGVjdGlvbktleV07XHJcbiAgICAgICAgaWYgKGhhbmRsZXJJbmZvcy5oYXNPd25Qcm9wZXJ0eShldnQudHlwZSkpIHtcclxuICAgICAgICAgIC8vIFdlIGFyZSBnb2luZyB0byByYWlzZSBhbiBldmVudCBmb3IgdGhpcyBlbGVtZW50LCBzbyBwcmVwYXJlIGluZm8gbmVlZGVkIGJ5IHRoZSAuTkVUIGNvZGVcclxuICAgICAgICAgIGlmICghZXZlbnRBcmdzKSB7XHJcbiAgICAgICAgICAgIGV2ZW50QXJncyA9IEV2ZW50Rm9yRG90TmV0LmZyb21ET01FdmVudChldnQpO1xyXG4gICAgICAgICAgfVxyXG5cclxuICAgICAgICAgIGNvbnN0IGhhbmRsZXJJbmZvID0gaGFuZGxlckluZm9zW2V2dC50eXBlXTtcclxuICAgICAgICAgIHRoaXMub25FdmVudChldnQsIGhhbmRsZXJJbmZvLmNvbXBvbmVudElkLCBoYW5kbGVySW5mby5ldmVudEhhbmRsZXJJZCwgZXZlbnRBcmdzKTtcclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIGNhbmRpZGF0ZUVsZW1lbnQgPSBjYW5kaWRhdGVFbGVtZW50LnBhcmVudEVsZW1lbnQ7XHJcbiAgICB9XHJcbiAgfVxyXG59XHJcblxyXG4vLyBSZXNwb25zaWJsZSBmb3IgYWRkaW5nIGFuZCByZW1vdmluZyB0aGUgZ2xvYmFsIGxpc3RlbmVyIHdoZW4gdGhlIG51bWJlciBvZiBsaXN0ZW5lcnNcclxuLy8gZm9yIGEgZ2l2ZW4gZXZlbnQgbmFtZSBjaGFuZ2VzIGJldHdlZW4gemVybyBhbmQgbm9uemVyb1xyXG5jbGFzcyBFdmVudEluZm9TdG9yZSB7XHJcbiAgcHJpdmF0ZSBpbmZvc0J5RXZlbnRIYW5kbGVySWQ6IHsgW2V2ZW50SGFuZGxlcklkOiBudW1iZXJdOiBFdmVudEhhbmRsZXJJbmZvIH0gPSB7fTtcclxuICBwcml2YXRlIGNvdW50QnlFdmVudE5hbWU6IHsgW2V2ZW50TmFtZTogc3RyaW5nXTogbnVtYmVyIH0gPSB7fTtcclxuXHJcbiAgY29uc3RydWN0b3IocHJpdmF0ZSBnbG9iYWxMaXN0ZW5lcjogRXZlbnRMaXN0ZW5lcikge1xyXG4gIH1cclxuXHJcbiAgcHVibGljIGFkZChpbmZvOiBFdmVudEhhbmRsZXJJbmZvKSB7XHJcbiAgICBpZiAodGhpcy5pbmZvc0J5RXZlbnRIYW5kbGVySWRbaW5mby5ldmVudEhhbmRsZXJJZF0pIHtcclxuICAgICAgLy8gU2hvdWxkIG5ldmVyIGhhcHBlbiwgYnV0IHdlIHdhbnQgdG8ga25vdyBpZiBpdCBkb2VzXHJcbiAgICAgIHRocm93IG5ldyBFcnJvcihgRXZlbnQgJHtpbmZvLmV2ZW50SGFuZGxlcklkfSBpcyBhbHJlYWR5IHRyYWNrZWRgKTtcclxuICAgIH1cclxuXHJcbiAgICB0aGlzLmluZm9zQnlFdmVudEhhbmRsZXJJZFtpbmZvLmV2ZW50SGFuZGxlcklkXSA9IGluZm87XHJcblxyXG4gICAgY29uc3QgZXZlbnROYW1lID0gaW5mby5ldmVudE5hbWU7XHJcbiAgICBpZiAodGhpcy5jb3VudEJ5RXZlbnROYW1lLmhhc093blByb3BlcnR5KGV2ZW50TmFtZSkpIHtcclxuICAgICAgdGhpcy5jb3VudEJ5RXZlbnROYW1lW2V2ZW50TmFtZV0rKztcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgIHRoaXMuY291bnRCeUV2ZW50TmFtZVtldmVudE5hbWVdID0gMTtcclxuICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihldmVudE5hbWUsIHRoaXMuZ2xvYmFsTGlzdGVuZXIpO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHVibGljIHVwZGF0ZShvbGRFdmVudEhhbmRsZXJJZDogbnVtYmVyLCBuZXdFdmVudEhhbmRsZXJJZDogbnVtYmVyKSB7XHJcbiAgICBpZiAodGhpcy5pbmZvc0J5RXZlbnRIYW5kbGVySWQuaGFzT3duUHJvcGVydHkobmV3RXZlbnRIYW5kbGVySWQpKSB7XHJcbiAgICAgIC8vIFNob3VsZCBuZXZlciBoYXBwZW4sIGJ1dCB3ZSB3YW50IHRvIGtub3cgaWYgaXQgZG9lc1xyXG4gICAgICB0aHJvdyBuZXcgRXJyb3IoYEV2ZW50ICR7bmV3RXZlbnRIYW5kbGVySWR9IGlzIGFscmVhZHkgdHJhY2tlZGApO1xyXG4gICAgfVxyXG5cclxuICAgIC8vIFNpbmNlIHdlJ3JlIGp1c3QgdXBkYXRpbmcgdGhlIGV2ZW50IGhhbmRsZXIgSUQsIHRoZXJlJ3Mgbm8gbmVlZCB0byB1cGRhdGUgdGhlIGdsb2JhbCBjb3VudHNcclxuICAgIGNvbnN0IGluZm8gPSB0aGlzLmluZm9zQnlFdmVudEhhbmRsZXJJZFtvbGRFdmVudEhhbmRsZXJJZF07XHJcbiAgICBkZWxldGUgdGhpcy5pbmZvc0J5RXZlbnRIYW5kbGVySWRbb2xkRXZlbnRIYW5kbGVySWRdO1xyXG4gICAgaW5mby5ldmVudEhhbmRsZXJJZCA9IG5ld0V2ZW50SGFuZGxlcklkO1xyXG4gICAgdGhpcy5pbmZvc0J5RXZlbnRIYW5kbGVySWRbbmV3RXZlbnRIYW5kbGVySWRdID0gaW5mbztcclxuICB9XHJcblxyXG4gIHB1YmxpYyByZW1vdmUoZXZlbnRIYW5kbGVySWQ6IG51bWJlcik6IEV2ZW50SGFuZGxlckluZm8ge1xyXG4gICAgY29uc3QgaW5mbyA9IHRoaXMuaW5mb3NCeUV2ZW50SGFuZGxlcklkW2V2ZW50SGFuZGxlcklkXTtcclxuICAgIGlmIChpbmZvKSB7XHJcbiAgICAgIGRlbGV0ZSB0aGlzLmluZm9zQnlFdmVudEhhbmRsZXJJZFtldmVudEhhbmRsZXJJZF07XHJcblxyXG4gICAgICBjb25zdCBldmVudE5hbWUgPSBpbmZvLmV2ZW50TmFtZTtcclxuICAgICAgaWYgKC0tdGhpcy5jb3VudEJ5RXZlbnROYW1lW2V2ZW50TmFtZV0gPT09IDApIHtcclxuICAgICAgICBkZWxldGUgdGhpcy5jb3VudEJ5RXZlbnROYW1lW2V2ZW50TmFtZV07XHJcbiAgICAgICAgZG9jdW1lbnQucmVtb3ZlRXZlbnRMaXN0ZW5lcihldmVudE5hbWUsIHRoaXMuZ2xvYmFsTGlzdGVuZXIpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIGluZm87XHJcbiAgfVxyXG59XHJcblxyXG5pbnRlcmZhY2UgRXZlbnRIYW5kbGVySW5mb3NGb3JFbGVtZW50IHtcclxuICAvLyBBbHRob3VnaCB3ZSAqY291bGQqIHRyYWNrIG11bHRpcGxlIGV2ZW50IGhhbmRsZXJzIHBlciAoZWxlbWVudCwgZXZlbnROYW1lKSBwYWlyXHJcbiAgLy8gKHNpbmNlIHRoZXkgaGF2ZSBkaXN0aW5jdCBldmVudEhhbmRsZXJJZCB2YWx1ZXMpLCB0aGVyZSdzIG5vIHBvaW50IGRvaW5nIHNvIGJlY2F1c2VcclxuICAvLyBvdXIgcHJvZ3JhbW1pbmcgbW9kZWwgaXMgdGhhdCB5b3UgZGVjbGFyZSBldmVudCBoYW5kbGVycyBhcyBhdHRyaWJ1dGVzLiBBbiBlbGVtZW50XHJcbiAgLy8gY2FuIG9ubHkgaGF2ZSBvbmUgYXR0cmlidXRlIHdpdGggYSBnaXZlbiBuYW1lLCBoZW5jZSBvbmx5IG9uZSBldmVudCBoYW5kbGVyIHdpdGhcclxuICAvLyB0aGF0IG5hbWUgYXQgYW55IG9uZSB0aW1lLlxyXG4gIC8vIFNvIHRvIGtlZXAgdGhpbmdzIHNpbXBsZSwgb25seSB0cmFjayBvbmUgRXZlbnRIYW5kbGVySW5mbyBwZXIgKGVsZW1lbnQsIGV2ZW50TmFtZSlcclxuICBbZXZlbnROYW1lOiBzdHJpbmddOiBFdmVudEhhbmRsZXJJbmZvXHJcbn1cclxuXHJcbmludGVyZmFjZSBFdmVudEhhbmRsZXJJbmZvIHtcclxuICBlbGVtZW50OiBFbGVtZW50O1xyXG4gIGV2ZW50TmFtZTogc3RyaW5nO1xyXG4gIGNvbXBvbmVudElkOiBudW1iZXI7XHJcbiAgZXZlbnRIYW5kbGVySWQ6IG51bWJlcjtcclxufVxyXG5cblxuXG4vLyBXRUJQQUNLIEZPT1RFUiAvL1xuLy8gLi9zcmMvUmVuZGVyaW5nL0V2ZW50RGVsZWdhdG9yLnRzIiwiZXhwb3J0IGNsYXNzIEV2ZW50Rm9yRG90TmV0PFREYXRhIGV4dGVuZHMgVUlFdmVudEFyZ3M+IHtcclxuICBjb25zdHJ1Y3RvcihwdWJsaWMgcmVhZG9ubHkgdHlwZTogRXZlbnRBcmdzVHlwZSwgcHVibGljIHJlYWRvbmx5IGRhdGE6IFREYXRhKSB7XHJcbiAgfVxyXG5cclxuICBzdGF0aWMgZnJvbURPTUV2ZW50KGV2ZW50OiBFdmVudCk6IEV2ZW50Rm9yRG90TmV0PFVJRXZlbnRBcmdzPiB7XHJcbiAgICBjb25zdCBlbGVtZW50ID0gZXZlbnQudGFyZ2V0IGFzIEVsZW1lbnQ7XHJcbiAgICBzd2l0Y2ggKGV2ZW50LnR5cGUpIHtcclxuICAgICAgY2FzZSAnY2xpY2snOlxyXG4gICAgICBjYXNlICdtb3VzZWRvd24nOlxyXG4gICAgICBjYXNlICdtb3VzZXVwJzpcclxuICAgICAgICByZXR1cm4gbmV3IEV2ZW50Rm9yRG90TmV0PFVJTW91c2VFdmVudEFyZ3M+KCdtb3VzZScsIHsgVHlwZTogZXZlbnQudHlwZSB9KTtcclxuICAgICAgY2FzZSAnY2hhbmdlJzoge1xyXG4gICAgICAgIGNvbnN0IHRhcmdldElzQ2hlY2tib3ggPSBpc0NoZWNrYm94KGVsZW1lbnQpO1xyXG4gICAgICAgIGNvbnN0IG5ld1ZhbHVlID0gdGFyZ2V0SXNDaGVja2JveCA/ICEhZWxlbWVudFsnY2hlY2tlZCddIDogZWxlbWVudFsndmFsdWUnXTtcclxuICAgICAgICByZXR1cm4gbmV3IEV2ZW50Rm9yRG90TmV0PFVJQ2hhbmdlRXZlbnRBcmdzPignY2hhbmdlJywgeyBUeXBlOiBldmVudC50eXBlLCBWYWx1ZTogbmV3VmFsdWUgfSk7XHJcbiAgICAgIH1cclxuICAgICAgY2FzZSAna2V5cHJlc3MnOlxyXG4gICAgICAgIHJldHVybiBuZXcgRXZlbnRGb3JEb3ROZXQ8VUlLZXlib2FyZEV2ZW50QXJncz4oJ2tleWJvYXJkJywgeyBUeXBlOiBldmVudC50eXBlLCBLZXk6IChldmVudCBhcyBhbnkpLmtleSB9KTtcclxuICAgICAgZGVmYXVsdDpcclxuICAgICAgICByZXR1cm4gbmV3IEV2ZW50Rm9yRG90TmV0PFVJRXZlbnRBcmdzPigndW5rbm93bicsIHsgVHlwZTogZXZlbnQudHlwZSB9KTtcclxuICAgIH1cclxuICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGlzQ2hlY2tib3goZWxlbWVudDogRWxlbWVudCB8IG51bGwpIHtcclxuICByZXR1cm4gZWxlbWVudCAmJiBlbGVtZW50LnRhZ05hbWUgPT09ICdJTlBVVCcgJiYgZWxlbWVudC5nZXRBdHRyaWJ1dGUoJ3R5cGUnKSA9PT0gJ2NoZWNrYm94JztcclxufVxyXG5cclxuLy8gVGhlIGZvbGxvd2luZyBpbnRlcmZhY2VzIG11c3QgYmUga2VwdCBpbiBzeW5jIHdpdGggdGhlIFVJRXZlbnRBcmdzIEMjIGNsYXNzZXNcclxuXHJcbnR5cGUgRXZlbnRBcmdzVHlwZSA9ICdtb3VzZScgfCAna2V5Ym9hcmQnIHwgJ2NoYW5nZScgfCAndW5rbm93bic7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFVJRXZlbnRBcmdzIHtcclxuICBUeXBlOiBzdHJpbmc7XHJcbn1cclxuXHJcbmludGVyZmFjZSBVSU1vdXNlRXZlbnRBcmdzIGV4dGVuZHMgVUlFdmVudEFyZ3Mge1xyXG59XHJcblxyXG5pbnRlcmZhY2UgVUlLZXlib2FyZEV2ZW50QXJncyBleHRlbmRzIFVJRXZlbnRBcmdzIHtcclxuICBLZXk6IHN0cmluZztcclxufVxyXG5cclxuaW50ZXJmYWNlIFVJQ2hhbmdlRXZlbnRBcmdzIGV4dGVuZHMgVUlFdmVudEFyZ3Mge1xyXG4gIFZhbHVlOiBzdHJpbmcgfCBib29sZWFuO1xyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9SZW5kZXJpbmcvRXZlbnRGb3JEb3ROZXQudHMiLCJpbXBvcnQgeyByZWdpc3RlckZ1bmN0aW9uIH0gZnJvbSAnLi4vSW50ZXJvcC9SZWdpc3RlcmVkRnVuY3Rpb24nO1xyXG5pbXBvcnQgeyBwbGF0Zm9ybSB9IGZyb20gJy4uL0Vudmlyb25tZW50JztcclxuaW1wb3J0IHsgTWV0aG9kSGFuZGxlLCBTeXN0ZW1fU3RyaW5nIH0gZnJvbSAnLi4vUGxhdGZvcm0vUGxhdGZvcm0nO1xyXG5jb25zdCBodHRwQ2xpZW50QXNzZW1ibHkgPSAnTWljcm9zb2Z0LkFzcE5ldENvcmUuQmxhem9yLkJyb3dzZXInO1xyXG5jb25zdCBodHRwQ2xpZW50TmFtZXNwYWNlID0gYCR7aHR0cENsaWVudEFzc2VtYmx5fS5IdHRwYDtcclxuY29uc3QgaHR0cENsaWVudFR5cGVOYW1lID0gJ0Jyb3dzZXJIdHRwTWVzc2FnZUhhbmRsZXInO1xyXG5jb25zdCBodHRwQ2xpZW50RnVsbFR5cGVOYW1lID0gYCR7aHR0cENsaWVudE5hbWVzcGFjZX0uJHtodHRwQ2xpZW50VHlwZU5hbWV9YDtcclxubGV0IHJlY2VpdmVSZXNwb25zZU1ldGhvZDogTWV0aG9kSGFuZGxlO1xyXG5cclxucmVnaXN0ZXJGdW5jdGlvbihgJHtodHRwQ2xpZW50RnVsbFR5cGVOYW1lfS5TZW5kYCwgKGlkOiBudW1iZXIsIG1ldGhvZDogc3RyaW5nLCByZXF1ZXN0VXJpOiBzdHJpbmcsIGJvZHk6IHN0cmluZyB8IG51bGwsIGhlYWRlcnNKc29uOiBzdHJpbmcgfCBudWxsLCBmZXRjaEFyZ3M6IFJlcXVlc3RJbml0IHwgbnVsbCkgPT4ge1xyXG4gIHNlbmRBc3luYyhpZCwgbWV0aG9kLCByZXF1ZXN0VXJpLCBib2R5LCBoZWFkZXJzSnNvbiwgZmV0Y2hBcmdzKTtcclxufSk7XHJcblxyXG5hc3luYyBmdW5jdGlvbiBzZW5kQXN5bmMoaWQ6IG51bWJlciwgbWV0aG9kOiBzdHJpbmcsIHJlcXVlc3RVcmk6IHN0cmluZywgYm9keTogc3RyaW5nIHwgbnVsbCwgaGVhZGVyc0pzb246IHN0cmluZyB8IG51bGwsIGZldGNoQXJnczogUmVxdWVzdEluaXQgfCBudWxsKSB7XHJcbiAgbGV0IHJlc3BvbnNlOiBSZXNwb25zZTtcclxuICBsZXQgcmVzcG9uc2VUZXh0OiBzdHJpbmc7XHJcblxyXG4gIGNvbnN0IHJlcXVlc3RJbml0OiBSZXF1ZXN0SW5pdCA9IGZldGNoQXJncyB8fCB7fTtcclxuICByZXF1ZXN0SW5pdC5tZXRob2QgPSBtZXRob2Q7XHJcbiAgcmVxdWVzdEluaXQuYm9keSA9IGJvZHkgfHwgdW5kZWZpbmVkO1xyXG5cclxuICB0cnkge1xyXG4gICAgcmVxdWVzdEluaXQuaGVhZGVycyA9IGhlYWRlcnNKc29uID8gKEpTT04ucGFyc2UoaGVhZGVyc0pzb24pIGFzIHN0cmluZ1tdW10pIDogdW5kZWZpbmVkO1xyXG5cclxuICAgIHJlc3BvbnNlID0gYXdhaXQgZmV0Y2gocmVxdWVzdFVyaSwgcmVxdWVzdEluaXQpO1xyXG4gICAgcmVzcG9uc2VUZXh0ID0gYXdhaXQgcmVzcG9uc2UudGV4dCgpO1xyXG4gIH0gY2F0Y2ggKGV4KSB7XHJcbiAgICBkaXNwYXRjaEVycm9yUmVzcG9uc2UoaWQsIGV4LnRvU3RyaW5nKCkpO1xyXG4gICAgcmV0dXJuO1xyXG4gIH1cclxuXHJcbiAgZGlzcGF0Y2hTdWNjZXNzUmVzcG9uc2UoaWQsIHJlc3BvbnNlLCByZXNwb25zZVRleHQpO1xyXG59XHJcblxyXG5mdW5jdGlvbiBkaXNwYXRjaFN1Y2Nlc3NSZXNwb25zZShpZDogbnVtYmVyLCByZXNwb25zZTogUmVzcG9uc2UsIHJlc3BvbnNlVGV4dDogc3RyaW5nKSB7XHJcbiAgY29uc3QgcmVzcG9uc2VEZXNjcmlwdG9yOiBSZXNwb25zZURlc2NyaXB0b3IgPSB7XHJcbiAgICBTdGF0dXNDb2RlOiByZXNwb25zZS5zdGF0dXMsXHJcbiAgICBIZWFkZXJzOiBbXVxyXG4gIH07XHJcbiAgcmVzcG9uc2UuaGVhZGVycy5mb3JFYWNoKCh2YWx1ZSwgbmFtZSkgPT4ge1xyXG4gICAgcmVzcG9uc2VEZXNjcmlwdG9yLkhlYWRlcnMucHVzaChbbmFtZSwgdmFsdWVdKTtcclxuICB9KTtcclxuXHJcbiAgZGlzcGF0Y2hSZXNwb25zZShcclxuICAgIGlkLFxyXG4gICAgcGxhdGZvcm0udG9Eb3ROZXRTdHJpbmcoSlNPTi5zdHJpbmdpZnkocmVzcG9uc2VEZXNjcmlwdG9yKSksXHJcbiAgICBwbGF0Zm9ybS50b0RvdE5ldFN0cmluZyhyZXNwb25zZVRleHQpLCAvLyBUT0RPOiBDb25zaWRlciBob3cgdG8gaGFuZGxlIG5vbi1zdHJpbmcgcmVzcG9uc2VzXHJcbiAgICAvKiBlcnJvck1lc3NhZ2UgKi8gbnVsbFxyXG4gICk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGRpc3BhdGNoRXJyb3JSZXNwb25zZShpZDogbnVtYmVyLCBlcnJvck1lc3NhZ2U6IHN0cmluZykge1xyXG4gIGRpc3BhdGNoUmVzcG9uc2UoXHJcbiAgICBpZCxcclxuICAgIC8qIHJlc3BvbnNlRGVzY3JpcHRvciAqLyBudWxsLFxyXG4gICAgLyogcmVzcG9uc2VUZXh0ICovIG51bGwsXHJcbiAgICBwbGF0Zm9ybS50b0RvdE5ldFN0cmluZyhlcnJvck1lc3NhZ2UpXHJcbiAgKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZGlzcGF0Y2hSZXNwb25zZShpZDogbnVtYmVyLCByZXNwb25zZURlc2NyaXB0b3I6IFN5c3RlbV9TdHJpbmcgfCBudWxsLCByZXNwb25zZVRleHQ6IFN5c3RlbV9TdHJpbmcgfCBudWxsLCBlcnJvck1lc3NhZ2U6IFN5c3RlbV9TdHJpbmcgfCBudWxsKSB7XHJcbiAgaWYgKCFyZWNlaXZlUmVzcG9uc2VNZXRob2QpIHtcclxuICAgIHJlY2VpdmVSZXNwb25zZU1ldGhvZCA9IHBsYXRmb3JtLmZpbmRNZXRob2QoXHJcbiAgICAgIGh0dHBDbGllbnRBc3NlbWJseSxcclxuICAgICAgaHR0cENsaWVudE5hbWVzcGFjZSxcclxuICAgICAgaHR0cENsaWVudFR5cGVOYW1lLFxyXG4gICAgICAnUmVjZWl2ZVJlc3BvbnNlJ1xyXG4gICAgKTtcclxuICB9XHJcblxyXG4gIHBsYXRmb3JtLmNhbGxNZXRob2QocmVjZWl2ZVJlc3BvbnNlTWV0aG9kLCBudWxsLCBbXHJcbiAgICBwbGF0Zm9ybS50b0RvdE5ldFN0cmluZyhpZC50b1N0cmluZygpKSxcclxuICAgIHJlc3BvbnNlRGVzY3JpcHRvcixcclxuICAgIHJlc3BvbnNlVGV4dCxcclxuICAgIGVycm9yTWVzc2FnZSxcclxuICBdKTtcclxufVxyXG5cclxuLy8gS2VlcCB0aGlzIGluIHN5bmMgd2l0aCB0aGUgLk5FVCBlcXVpdmFsZW50IGluIEh0dHBDbGllbnQuY3NcclxuaW50ZXJmYWNlIFJlc3BvbnNlRGVzY3JpcHRvciB7XHJcbiAgLy8gV2UgZG9uJ3QgaGF2ZSBCb2R5VGV4dCBpbiBoZXJlIGJlY2F1c2UgaWYgd2UgZGlkLCB0aGVuIGluIHRoZSBKU09OLXJlc3BvbnNlIGNhc2UgKHdoaWNoXHJcbiAgLy8gaXMgdGhlIG1vc3QgY29tbW9uIGNhc2UpLCB3ZSdkIGJlIGRvdWJsZS1lbmNvZGluZyBpdCwgc2luY2UgdGhlIGVudGlyZSBSZXNwb25zZURlc2NyaXB0b3JcclxuICAvLyBhbHNvIGdldHMgSlNPTiBlbmNvZGVkLiBJdCB3b3VsZCB3b3JrIGJ1dCBpcyB0d2ljZSB0aGUgYW1vdW50IG9mIHN0cmluZyBwcm9jZXNzaW5nLlxyXG4gIFN0YXR1c0NvZGU6IG51bWJlcjtcclxuICBIZWFkZXJzOiBzdHJpbmdbXVtdO1xyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9TZXJ2aWNlcy9IdHRwLnRzIiwiaW1wb3J0IHsgcGxhdGZvcm0gfSBmcm9tICcuL0Vudmlyb25tZW50J1xyXG5pbXBvcnQgeyByZWdpc3RlckZ1bmN0aW9uIH0gZnJvbSAnLi9JbnRlcm9wL1JlZ2lzdGVyZWRGdW5jdGlvbic7XHJcbmltcG9ydCB7IG5hdmlnYXRlVG8gfSBmcm9tICcuL1NlcnZpY2VzL1VyaUhlbHBlcic7XHJcblxyXG5pZiAodHlwZW9mIHdpbmRvdyAhPT0gJ3VuZGVmaW5lZCcpIHtcclxuICAvLyBXaGVuIHRoZSBsaWJyYXJ5IGlzIGxvYWRlZCBpbiBhIGJyb3dzZXIgdmlhIGEgPHNjcmlwdD4gZWxlbWVudCwgbWFrZSB0aGVcclxuICAvLyBmb2xsb3dpbmcgQVBJcyBhdmFpbGFibGUgaW4gZ2xvYmFsIHNjb3BlIGZvciBpbnZvY2F0aW9uIGZyb20gSlNcclxuICB3aW5kb3dbJ0JsYXpvciddID0ge1xyXG4gICAgcGxhdGZvcm0sXHJcbiAgICByZWdpc3RlckZ1bmN0aW9uLFxyXG4gICAgbmF2aWdhdGVUbyxcclxuICB9O1xyXG59XHJcblxuXG5cbi8vIFdFQlBBQ0sgRk9PVEVSIC8vXG4vLyAuL3NyYy9HbG9iYWxFeHBvcnRzLnRzIl0sInNvdXJjZVJvb3QiOiIifQ==