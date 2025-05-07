// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const videos = document.querySelectorAll('.bg-video');
    let currentVideo = 0;

    if (videos.length > 0) {
        // Tüm videoları yükle ve gizle
        videos.forEach(video => {
            video.load();
            video.style.display = 'none';

            // Video metadata yüklendiğinde süreyi öğren
            video.addEventListener('loadedmetadata', function () {
                console.log(`Video ${video.querySelector('source').src} süresi: ${video.duration}s`);
            });
        });

        // İlk videoyu başlat
        playVideo(videos[0]);
    }

    function playVideo(video) {
        // Önceki videoyu duraklat ve gizle
        document.querySelectorAll('.bg-video').forEach(v => {
            v.pause();
            v.style.display = 'none';
        });

        // Yeni videoyu göster ve oynat
        video.style.display = 'block';
        video.currentTime = 0;
        video.setAttribute('playsinline', '');

        const playPromise = video.play();

        if (playPromise !== undefined) {
            playPromise.catch(e => {
                console.error("Video oynatma hatası:", e);
                // Hata durumunda bir sonraki videoya geç
                setTimeout(nextVideo, 3000); // 3 sn sonra devam et
            });
        }

        // Video bitişini dinle
        video.addEventListener('ended', nextVideo, { once: true });
    }

    function nextVideo() {
        // Sonraki videoya geç (döngüsel)
        currentVideo = (currentVideo + 1) % videos.length;
        playVideo(videos[currentVideo]);
    }
});