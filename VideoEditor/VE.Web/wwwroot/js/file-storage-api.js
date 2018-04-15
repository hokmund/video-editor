$('#uploadFileSubmitBtn').on('click', function () {

    var data = new FormData();

    var files = window.$("#uploadFileInput").get(0).files;

    // Add the uploaded image content to the form data collection
    for (var i = 0; i < files.length; i++) {
        data.append("file" + i, files[i]);
    }

    // Make Ajax request with the contentType = false, and procesDate = false
    window.$.ajax({
        type: "POST",
        url: "/api/storage/upload",
        contentType: false,
        processData: false,
        data: data
    })
        .success(function (xhr, textStatus) {
            var t = xhr;
        })
        .error(function (xhr, textStatus, err) {
            var t = xhr;
        })
        .done(function (xhr, textStatus) {
            var t = xhr;
        });
});