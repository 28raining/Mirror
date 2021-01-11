//this file allows unity to call an external JS function
var plugin = {
  webGLSendString: function(x) {
      jsSendString(Pointer_stringify(x)); //This functions exists in the JavaScript inside index.html
  }
};
mergeInto(LibraryManager.library, plugin);