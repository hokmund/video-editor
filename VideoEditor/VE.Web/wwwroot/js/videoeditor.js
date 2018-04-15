var videoEditor = {

    settings: {
        storagePath: '/api/storage/',
        videoPath: '/api/video/'
    },

    player: null,
    activeVideo: null,
    video: {
        name: '',
        time: '',
        duration: 0,
        frames: 0,
        fps: 24,
        segments: []
    },

    ajaxRequest: function (type, url, data, callback) {
        $.ajax({
            type: type,
            dataType: 'json',
            url: url,
            data: data,
            success: function (response) {
    
                if (typeof callback == 'function') {
                    callback(response);
                }
    
            }, error: function (jqXHR, textStatus, errorThrown) {
                if (typeof (console) != 'undefined') console.log(textStatus + ' ' + errorThrown);
            }
        });
    },

    init: function() {
        this.player = document.getElementById('video');
        $(this.player).on('timeupdate', function() {
            if (!videoEditor.player.paused) {
                videoEditor.updateTime(videoEditor.player.currentTime * videoEditor.video.fps);
            }
        })

        this.setUpButtons();
        this.setUpSlider();
        this.getAllVideos();
    },

    setUpButtons: function() {
        $(document).on('click', '#input-list a', function (e) {
            e.preventDefault();
            $('#input-list a').removeClass('active');
            $(this).addClass('active');
            videoEditor.activeVideo = $('#input-list a.active').data('value');
            videoEditor.getCurrentVideo();
        });

        $('#btnPlay').click(videoEditor.playVideo);
    },

    setUpSlider: function() {
        $("#time-range").slider({
            value: 0,
            step: 0.01,
            stop: function (event, ui) {

                //var index = $(ui.handle).prevAll('.ui-slider-handle').length;
                var time = Math.floor(ui.value / videoEditor.video.fps);

                //videoEditor.handlerActive = index;
                $("#time-range .ui-slider-handle").removeClass('ui-handle-active');
                $(ui.handle).addClass('ui-handle-active');
                videoEditor.stopVideo();
                videoEditor.getFrame(time);
            }
        });
    },

    timeToSeconds: function (time) {
        var time_arr = time.split(':');
        var dur = 0;
        var t = [3600, 60, 1];
        for (var i in time_arr) {
            dur += (parseFloat(time_arr[i]) * t[i]);
        }
        return dur;
    },

    secondsToTime: function (in_seconds) {
        var time = '';

        var hours = Math.floor(in_seconds / 3600);
        var minutes = Math.floor((in_seconds - (hours * 3600)) / 60);
        var seconds = in_seconds - (hours * 3600) - (minutes * 60);
        seconds = seconds.toFixed(2);

        if (hours < 10) { hours = "0" + hours; }
        if (minutes < 10) { minutes = "0" + minutes; }
        if (seconds < 10) { seconds = "0" + seconds; }
        var time = hours + ':' + minutes + ':' + seconds;

        return time;
    },

    updateTimeDuration: function () {

        var time = videoEditor.secondsToTime(videoEditor.video.duration);

        $('.time-line .label:eq(0)').text('00:00:00');
        $('.time-line .label:eq(1)').text(time);

        $("#time-range").slider("option", "max", videoEditor.video.frames);
        $("#time-range").slider("value", 0);

    },

    updateTime: function (frames) {
        $("#time-range").slider("value", frames);
        var time = videoEditor.secondsToTime(videoEditor.player.currentTime);

        $('.time-line .label:eq(0)').text(time);
    },

    getAllVideos: function() {
        videoEditor.ajaxRequest('GET', videoEditor.settings.storagePath + 'video/all', {}, function(response) {
            if (response) {
                $('#listInput').empty();
                videoEditor.segments = [];
                for (var i in response) {
                    if (!response.hasOwnProperty(i)) continue;

                    var name = response[i].name;
                    var ext = name.split('.').pop();
                    var titleStr = '';
                    if (name.length - 4 > 20) {
                        titleStr = ' title="' + name + '"';
                        name = name.substr(0, 20) + '...' + ext;
                    }
                    var size = response[i].size.toString() + ' MB';

                    var row = '<a href="#" class="list-group-item" data-value="' + response[i].name + '"' + titleStr + '>' + name + ' (' + size + ')</a>';

                    $('#listInput').append(row);
                    videoEditor.segments.push({ name: name, size: size });
                }

                $('#listInput a:first').addClass('active');
                videoEditor.activeVideo = $('#input-list a.active').data('value');
                videoEditor.getCurrentVideo();
            }
        });
    },

    getCurrentVideo: function() {
        if (!this.activeVideo) {
            return;
        }

        var videoParts = videoEditor.activeVideo.split('.');
        var videoLink = videoEditor.settings.storagePath + 'download/' + videoParts[0];

        var video = document.getElementById('video');
        video.setAttribute('src', videoLink);
        video.setAttribute('poster', '');
        video.load();

        $(video).on('loadedmetadata', function() {
            videoEditor.video.name = videoEditor.activeVideo;
            videoEditor.video.duration = video.duration;
            videoEditor.video.frames = videoEditor.video.duration * videoEditor.video.fps;
            $('#v_time_in').val('00:00:00');
            $('#v_time_out').val(videoEditor.secondsToTime(video.duration));
            videoEditor.updateTimeDuration();
        });
    },

    getFrame: function (time) {
        videoEditor.ajaxRequest('GET', videoEditor.settings.videoPath + 'getFrame', { name: videoEditor.video.name, time: time }, function(response) {
            videoEditor.setPoster(response);
        });
    },

    setPoster(imgUrl) {
        videoEditor.player.pause();
        videoEditor.player.setAttribute('poster', imgUrl);
    },

    playVideo: function() {
        videoEditor.getCurrentVideo();
        videoEditor.player.play();
    },

    stopVideo: function() {
        videoEditor.player.src = '';
    }
};


$(document).ready(function () {
    videoEditor.init();
});