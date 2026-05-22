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
