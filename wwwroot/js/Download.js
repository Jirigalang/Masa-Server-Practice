window.setAudioSource = function (audioSrc) {
    const audioPlayer = document.getElementById("audioPlayer");
    audioPlayer.src = audioSrc;
    audioPlayer.play();
};

