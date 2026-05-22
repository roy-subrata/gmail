window.quillInterop = {
    instances: {},

    init: function (editorId) {
        if (this.instances[editorId]) return;

        var quill = new Quill('#' + editorId, {
            theme: 'snow',
            placeholder: 'Write your email template here...',
            modules: {
                toolbar: [
                    [{ header: [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ color: [] }, { background: [] }],
                    [{ font: [] }],
                    [{ size: ['small', false, 'large', 'huge'] }],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    [{ align: [] }],
                    ['link', 'image'],
                    ['blockquote'],
                    ['clean']
                ]
            }
        });

        this.instances[editorId] = quill;
    },

    getHtml: function (editorId) {
        var quill = this.instances[editorId];
        return quill ? quill.root.innerHTML : '';
    },

    setHtml: function (editorId, html) {
        var quill = this.instances[editorId];
        if (quill) quill.root.innerHTML = html;
    },

    clear: function (editorId) {
        var quill = this.instances[editorId];
        if (quill) quill.setContents([]);
    }
};

window.setIframeSrcdoc = function (iframeId, html) {
    var iframe = document.getElementById(iframeId);
    if (iframe) iframe.srcdoc = html;
};

// Works on both HTTPS and plain HTTP (Docker without TLS)
window.copyToClipboard = function (text) {
    if (navigator.clipboard && window.isSecureContext) {
        return navigator.clipboard.writeText(text);
    }
    var ta = document.createElement('textarea');
    ta.value = text;
    ta.style.cssText = 'position:fixed;opacity:0;top:0;left:0';
    document.body.appendChild(ta);
    ta.focus();
    ta.select();
    try { document.execCommand('copy'); } catch (_) {}
    document.body.removeChild(ta);
    return Promise.resolve();
};
