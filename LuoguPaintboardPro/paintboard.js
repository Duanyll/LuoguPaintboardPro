H = 400;
W = 800;
nowcolor = 0;
scale = 5;
dragged = 0;
lasttime = 0;
timelimit = 10;
colorlist = ['rgb(0, 0, 0)', 'rgb(255, 255, 255)', 'rgb(170, 170, 170)', 'rgb(85, 85, 85)', 'rgb(254, 211, 199)', 'rgb(255, 196, 206)', 'rgb(250, 172, 142)', 'rgb(255, 139, 131)', 'rgb(244, 67, 54)', 'rgb(233, 30, 99)', 'rgb(226, 102, 158)', 'rgb(156, 39, 176)', 'rgb(103, 58, 183)', 'rgb(63, 81, 181)', 'rgb(0, 70, 112)', 'rgb(5, 113, 151)', 'rgb(33, 150, 243)', 'rgb(0, 188, 212)', 'rgb(59, 229, 219)', 'rgb(151, 253, 220)', 'rgb(22, 115, 0)', 'rgb(55, 169, 60)', 'rgb(137, 230, 66)', 'rgb(215, 255, 7)', 'rgb(255, 246, 209)', 'rgb(248, 203, 140)', 'rgb(255, 235, 59)', 'rgb(255, 193, 7)', 'rgb(255, 152, 0)', 'rgb(255, 87, 34)', 'rgb(184, 63, 39)', 'rgb(121, 85, 72)'];
nowintevel = 0;
var myarr = [];
for (var i = 0; i < H; i++) {
    myarr[i] = [];
    for (var j = 0; j < W; j++) {
        myarr[i][j] = '#dddddd';
    }
}

function render(arr) {
    var c = document.getElementById("mycanvas");
    var ctx = c.getContext("2d");
    for (var i = 0; i < H; i++) {
        for (var j = 0; j < W; j++) {
            ctx.fillStyle = arr[i][j];
            ctx.fillRect(j * scale, i * scale, scale, scale);
        }
    }

}

function update(y, x, color) {
    if (dragged) {
        dragged = 0;
        return;
    }
    //alert('ss');
    var c = document.getElementById("mycanvas");
    var ctx = c.getContext("2d");
    ctx.save();
    ctx.fillStyle = color;
    ctx.fillRect(x * 5, y * 5, 5, 5);
}

function initpale() {
    $('#palette').html('');
    colorlist.forEach(function (k, v) {
        console.log(k, v);
        $('#palette').append('<div class="paleitem" data-cid=' + v + '></div>');
        $('[data-cid=' + v + ']').css('background', k);
    });
    zoom(1)
}

binditem = function () {
    $('.paleitem').removeClass("selected");
    $(this).addClass("selected");
    nowcolor = $(this).attr('data-cid');
}
zoom = function (s) {
    scale = s;
    $('#mycanvas').width(800 * scale)
    if (s == 1) {
        $('#mycanvas').css('top', 0);
        $('#mycanvas').css('left', 0);
    }
}
$("[zoom]").click(function () {
    zoom($(this).attr('zoom'));
});
myarr[10][10] = '#6600ff';
myarr[100][200] = '#66ccff';
render(myarr);
initpale();
$('.paleitem').bind("click", binditem);
$('[data-cid=0]').addClass("selected");
$('#mycanvas').bind("click", function () {
    //alert(event.offsetY);
    if (new Date() < (lasttime + timelimit) * 1000) {
        alert("冷却时间未到，暂时不能涂色");
        return;
    }
    var x = parseInt(event.offsetX / scale);
    var y = parseInt(event.offsetY / scale);
    update(
        y,
        x,
        colorlist[nowcolor]
    );
    $.post("/paintBoard/paint", {
        x: x,
        y: y,
        color: nowcolor
    }, function (resp) {
        if (resp.status !== 200) {
            alert(resp.data)
        } else {
            lasttime = (new Date()) / 1000;
            getCountDown(lasttime + timelimit);
        }
    });
})
$('#mycanvas').draggable({
    cursor: "move",
    stop: function () {
        dragged = 1;
    }
});
$('#mycanvas').bind("mousewheel", function (event, delta) {
    var delta = event.originalEvent.deltaY;
    var y = parseInt(event.offsetY / scale);
    var x = parseInt(event.offsetX / scale);
    console.log(event);
    if (delta > 0) {
        if (scale == 10)
            zoom(5);
        else if (scale == 5)
            zoom(1);
    } else {
        if (scale == 1)
            zoom(5);
        else if (scale == 5)
            zoom(10);
    }
    if (scale != 1) {
        $('#mycanvas').css('top', -y * scale + 200);
        $('#mycanvas').css('left', -x * scale + 400);
    }
    scale
    return false;
});

function getCountDown(timestamp) {
    clearInterval(nowintevel);
    nowintevel = setInterval(function () {
        var nowTime = new Date();
        var endTime = new Date(timestamp * 1000);
        var t = endTime.getTime() - nowTime.getTime();
        if (t < 0) {
            $("#timeleft").html("冷却时间到");
            clearInterval(nowintevel);
            return;
        }
        var hour = Math.floor(t / 1000 / 60 / 60 % 24);
        var min = Math.floor(t / 1000 / 60 % 60);
        var sec = Math.floor(t / 1000 % 60);
        if (hour < 10)
            hour = "0" + hour;
        if (min < 10)
            min = "0" + min;
        if (sec < 10)
            sec = "0" + sec;
        var countDownTime = hour + ":" + min + ":" + sec;
        $("#timeleft").html(countDownTime);
    }, 1000);
}

getCountDown(lasttime + timelimit);

function initialPaint() {
    $.get("/paintBoard/board", function (resp) {
        resp.split('\n').map(function (colorStr, x) {
            colorStr.split("").map(function (color, y) {
                //if(color !== '2') console.log(x, y, color);
                update(y, x, colorlist[parseInt(color, 32)]);
            });
        });
    });
}

var ws = null;
function connectWs() {
    try {
        ws = new WebSocket('wss://ws.luogu.com.cn/ws');
    } catch (e) {
        alert("无法连接追踪服务器");
        return;
    }

    ws.onopen = function () {
        var message = {
            "type": "join_channel",
            "channel": "paintboard",
            "channel_param": ""
        };
        ws.send(JSON.stringify(message));
    };

    ws.onmessage = function (event) {
        var data = JSON.parse(event.data);
        if (data.type === "paintboard_update") {
            update(data.y, data.x, colorlist[data.color]);
        } else if (data.type === "result") {
            initialPaint()
        }
    };
}

connectWs();