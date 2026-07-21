export async function downloadFileFromUrl(url, fileName) {
    const response = await fetch(url, { method: 'GET' });

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `HTTP ${response.status}`);
    }

    const blob = await response.blob();
    const blobUrl = URL.createObjectURL(blob);

    try {
        const a = document.createElement('a');
        a.href = blobUrl;
        a.download = fileName;
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        a.remove();
    }
    finally {
        URL.revokeObjectURL(blobUrl);
    }
}
