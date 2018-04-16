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
        fps: 30,
        segments: []
    },
    convertingVideo: null,

    ajaxRequest: function (type, url, data, callback) {
        $.ajax({
            type: type,
            contentType: 'application/json; charset=utf-8',
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
        this.getInputVideos();
        this.getOutputVideos();
    },

    setUpButtons: function() {
        $(document).on('click', '#input-list a', function (e) {
            e.preventDefault();
            $('#input-list a').removeClass('active');
            $(this).addClass('active');
            videoEditor.activeVideo = $('#input-list a.active').data('value');
            videoEditor.getCurrentVideo();
        });

        $('#btnPlay').click(videoEditor.getCurrentVideo);
        $('#btnRemove').click(videoEditor.removeVideo);
        $(document).on('click', '#listOutput button.download', function(e) {

            var target = !$(e.target).is('button') ? $(e.target).parent() : $(e.target);
            videoEditor.convertingVideo = target.data('value');

            $('#createVideoModal').modal();
        });
        $(document).on('click', '#listOutput button.remove', videoEditor.removeVideo);

        $('#btnSubmit').click(function(e) {
            e.preventDefault();
        
            var data = {
                bitrate: $('#opt_bitrate').val(),
                width: $('#opt_width').val(),
                height: $('#opt_height').val(),
                format: $('#opt_format').val(),
                file: videoEditor.convertingVideo
            };
        
            var formData = JSON.stringify(data);
        
            videoEditor.ajaxRequest('POST', '/api/video/convert', formData, function() {
                videoEditor.getOutputVideos();
            });
        });

        $('#btnJoin').click(function(e) {
            e.preventDefault();
        
            var data = {
                files: videoEditor.video.segments.map(function(value) {
                    return value.name;
                })
            };
        
            var formData = JSON.stringify(data);
        
            videoEditor.ajaxRequest('POST', '/api/video/join', formData, function() {
                videoEditor.getOutputVideos();
            });
        });
    },

    setUpSlider: function() {
        $("#time-range").slider({
            value: 0,
            step: 0.01,
            stop: function (event, ui) {
                var timeSeconds = Math.floor(ui.value / videoEditor.video.fps);
                var time = (ui.value / videoEditor.video.fps).toFixed(2);

                $("#time-range .ui-slider-handle").removeClass('ui-handle-active');
                $(ui.handle).addClass('ui-handle-active');
                videoEditor.stopVideo();
                videoEditor.getFrame(timeSeconds);
                var timeLabel = videoEditor.secondsToTime(time);
                $('.time-line .label:eq(0)').text(timeLabel);
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

        $('.time-line .label:eq(0)').text('00:00:00.00');
        $('.time-line .label:eq(1)').text(time);

        $("#time-range").slider("option", "max", videoEditor.video.frames);
        $("#time-range").slider("value", 0);

    },

    updateTime: function (frames) {
        $("#time-range").slider("value", frames);
        var time = videoEditor.secondsToTime(videoEditor.player.currentTime);
        $('.time-line .label:eq(0)').text(time);
    },

    getInputVideos: function() {
        videoEditor.ajaxRequest('GET', videoEditor.settings.storagePath + 'video/inputs', {}, function(response) {
            if (response) {
                $('#listInput').empty();
                videoEditor.video.segments = [];
                for (var i in response) {
                    if (!response.hasOwnProperty(i)) continue;

                    var name = response[i].name;
                    var size = response[i].size.toString() + ' MB';
                    videoEditor.video.segments.push({ 'name': name, 'size': size });
                    var ext = name.split('.').pop();
                    var titleStr = '';
                    if (name.length - 4 > 20) {
                        titleStr = ' title="' + name + '"';
                        name = name.substr(0, 20) + '...' + ext;
                    }

                    var row = '<a href="#" class="list-group-item" data-value="' + response[i].name + '"' + titleStr + '>' + name + ' (' + size + ')</a>';

                    $('#listInput').append(row);
                }

                $('#listInput a:first').addClass('active');
                videoEditor.activeVideo = $('#input-list a.active').data('value');
                videoEditor.getCurrentVideo();
            }
        });
    },

    getOutputVideos: function() {
        videoEditor.ajaxRequest('GET', videoEditor.settings.storagePath + 'video/outputs', {}, function (response) {
            if (response) {
                $('#listOutput').html('<table class="table table-bordered table-hover"></table>');
                if (response.length > 0) {
                    for (var i in response) {
                        if (!response.hasOwnProperty(i)) continue;

                        var name = response[i].name;
                        var size = response[i].size.toString() + ' MB';

                        var row = '<tr>';
                        row += '<td><a href="AppData/Outputs/' + name + '" target="_blank">' + name + '</a></td>';
                        row += '<td>' + size + '</a></td>';
                        row += '<td class="text-right">';
                        row += ' <button class="btn btn-default btn-sm download" data-value="' + name + '" data-toggle="tooltip" title="Скачать"><span class="glyphicon glyphicon-download"></span></button>';
                        row += ' <button class="btn btn-default btn-sm remove" data-value="' + name + '" data-toggle="tooltip" title="Удалить"><span class="glyphicon glyphicon-remove"></span></button>';
                        row += '</td></tr>';

                        $('#listOutput table').append(row);
                    }
                }
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

        videoEditor.player.play();
    },

    getFrame: function (time) {
        videoEditor.ajaxRequest('GET', videoEditor.settings.videoPath + 'getFrame', { name: videoEditor.video.name, time: time }, function(response) {
            videoEditor.setPoster(response);
        });
    },

    removeVideo: function(e) {
        e.preventDefault();
        
        videoEditor.stopVideo();

        var target = !$(e.target).is('button') ? $(e.target).parent() : $(e.target);
        var video_name = !!target.data('value') ? target.data('value') : videoEditor.activeVideo;
        var video_type = !!target.data('value') ? 'Output' : 'Input';

        var video = video_name.split('.')[0];
        var data = {
            fileId: video,
            type: video_type
        };

        videoEditor.ajaxRequest('DELETE', videoEditor.settings.storagePath + 'delete', JSON.stringify(data), function(response) {
            videoEditor.getInputVideos();
            videoEditor.getOutputVideos();
        })
    },

    setPoster(imgUrl) {
        videoEditor.player.pause();
        videoEditor.player.setAttribute('poster', imgUrl);
    },

    stopVideo: function() {
        videoEditor.player.src = '';
    }
};


$(document).ready(function () {
    videoEditor.init();
});