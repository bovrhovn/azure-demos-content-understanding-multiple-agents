// upload-process.js - File upload and processing functionality
(function () {
    // Upload form elements
    const dropZone = document.getElementById('dropZone');
    const fileInput = document.getElementById('FormFile');
    const uploadBtn = document.getElementById('uploadBtn');
    const filePreview = document.getElementById('filePreview');
    const fileNameEl = document.getElementById('fileName');
    const clearBtn = document.getElementById('clearFile');
    const uploadForm = document.getElementById('uploadForm');

    // File list elements
    const fileItems = document.querySelectorAll('.file-item');
    const processSelectedBtn = document.getElementById('processSelectedBtn');
    const processForm = document.getElementById('processForm');
    const processFileNameInput = document.getElementById('processFileName');

    let selectedFileName = null;

    // Upload form handling
    function showFile(file) {
        if (!file) return;
        fileNameEl.textContent = file.name + ' (' + formatBytes(file.size) + ')';
        filePreview.classList.remove('d-none');
        dropZone.classList.add('docai-dropzone--has-file');
        uploadBtn.disabled = false;
    }

    function clearFile() {
        fileInput.value = '';
        filePreview.classList.add('d-none');
        fileNameEl.textContent = '';
        dropZone.classList.remove('docai-dropzone--has-file', 'docai-dropzone--drag');
        uploadBtn.disabled = true;
    }

    function formatBytes(bytes) {
        if (bytes < 1024) return bytes + ' B';
        if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
        if (bytes < 1073741824) return (bytes / 1048576).toFixed(1) + ' MB';
        return (bytes / 1073741824).toFixed(2) + ' GB';
    }

    // File input change
    if (fileInput) {
        fileInput.addEventListener('change', function () {
            if (fileInput.files.length) showFile(fileInput.files[0]);
        });
    }

    // Clear file button
    if (clearBtn) {
        clearBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            clearFile();
        });
    }

    // Drop zone click
    if (dropZone) {
        dropZone.addEventListener('click', function (e) {
            if (e.target !== clearBtn && !e.target.classList.contains('btn-close')) {
                fileInput.click();
            }
        });

        dropZone.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                fileInput.click();
            }
        });

        // Drag and drop
        dropZone.addEventListener('dragover', function (e) {
            e.preventDefault();
            dropZone.classList.add('docai-dropzone--drag');
        });

        dropZone.addEventListener('dragleave', function () {
            dropZone.classList.remove('docai-dropzone--drag');
        });

        dropZone.addEventListener('drop', function (e) {
            e.preventDefault();
            dropZone.classList.remove('docai-dropzone--drag');
            const file = e.dataTransfer.files[0];
            if (file && file.type === 'application/pdf') {
                const dt = new DataTransfer();
                dt.items.add(file);
                fileInput.files = dt.files;
                showFile(file);
            }
        });
    }

    // Upload form submit
    if (uploadForm) {
        uploadForm.addEventListener('submit', function () {
            uploadBtn.disabled = true;
            uploadBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Uploading...';
        });
    }

    // File selection from list
    fileItems.forEach(function (item) {
        item.addEventListener('click', function () {
            // Remove selection from all items
            fileItems.forEach(function (i) {
                i.classList.remove('active');
            });

            // Add selection to clicked item
            item.classList.add('active');
            selectedFileName = item.dataset.filename;

            // Enable process button
            if (processSelectedBtn) {
                processSelectedBtn.disabled = false;
            }
        });
    });

    // Process selected file
    if (processSelectedBtn) {
        processSelectedBtn.addEventListener('click', function () {
            if (selectedFileName && processForm && processFileNameInput) {
                processFileNameInput.value = selectedFileName;
                processSelectedBtn.disabled = true;
                processSelectedBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
                processForm.submit();
            }
        });
    }

    // Auto-dismiss alerts after 5 seconds
    setTimeout(function () {
        const alerts = document.querySelectorAll('.alert-dismissible');
        alerts.forEach(function (alert) {
            const closeBtn = alert.querySelector('.btn-close');
            if (closeBtn) {
                closeBtn.click();
            }
        });
    }, 5000);
})();
