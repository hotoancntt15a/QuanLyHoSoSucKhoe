var isRunLoad = 0;
var path_images = '/images/';
var idModal = $('#mdMessage');
var alertkey = { danger: 'danger', warning: 'warning', info: 'info', success: 'success' }
Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};
$.fn.insertAt = function (index, $parent) {
    return this.each(function () {
        if (index === 0) { $parent.prepend(this); }
        else { $parent.children().eq(index - 1).after(this); }
    });
}
function isNumberKey(e) { var charCode = (e.which) ? e.which : event.keyCode; if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) return false; return true; }
var keyPressNumber = function (e) { var a = []; var k = e.which; for (i = 48; i < 58; i++) a.push(i); if (!(a.indexOf(k) >= 0)) e.preventDefault(); }
var loadImg = function (event, id) { var output = document.getElementById(id); output.src = URL.createObjectURL(event.target.files[0]); };
var loadpage = function (page, idform) {
    if (/[#]/gi.test(idform) == false) { idform = '#' + idform; }
    $(idform).find('input[name="page"]').each(function () { $(this).remove() });
    $(idform).append('<input type="hidden" name="page" value="' + page + '" />');
    $(idform).submit();
}
function getRandomColor() { var letters = '0123456789ABCDEF'; var color = '#'; for (var i = 0; i < 6; i++) { color += letters[Math.floor(Math.random() * 16)]; } return color; }
function printid(id) {
    if (typeof (id) == 'string') { if (id == '') { return; } var printContents = id.startsWith('#') ? $(id).html() : document.getElementById(id).innerHTML; var originalContents = document.body.innerHTML; document.body.innerHTML = printContents; window.print(); document.body.innerHTML = originalContents; }
    if (typeof (id) == 'object') { var printContents = $(id).html(); var originalContents = document.body.innerHTML; document.body.innerHTML = printContents; window.print(); document.body.innerHTML = originalContents; }
}
function isCMND(socmnd, gioitinh, namsinh, dmhc) {
    if (isNaN(socmnd)) { return '01. Không đúng định dạng [chỉ có số]'; }
    if (socmnd.length == 9) {
        if (parseInt(socmnd.substring(0, 2)) > 38) { return "Mã đơn vị hành chính không nằm trong danh sách"; }
        return '';
    }
    if (socmnd.length != 12) { return '02. Không đúng định dạng [(9|12) ký tự]'; }
    if (dmhc != undefined) { if (dmhc.length > 0) { if (dmhc.indexOf(socmnd.substring(0, 3)) == -1) { return '03. Mã đơn vị hành chính không nằm trong danh sách'; } } }
    if (isNaN(namsinh)) { return '04. Năm sinh không đúng ?"' + namsinh + '"'; }
    if (namsinh.length != 4) { return '04. Năm sinh không đúng ?"' + namsinh + '"'; }
    if (parseInt(namsinh) < 1900 || parseInt(namsinh) > 2399) { return '05. Năm sinh nằm ngoài thế kỷ 20-24'; }
    if (socmnd.substring(4, 6) != namsinh.substring(namsinh.length - 2)) { return '07. Mã năm sinh không đúng ?"' + socmnd.substring(4, 6) + '"'; }
    /* Xác định thế kỷ - 1; Xác định giới hạn magt */
    var magt = parseInt(socmnd.substring(3, 4));
    /* Nam */
    var gt = (parseInt(namsinh.substring(0, 2)) - 19) * 2;
    /* Nữ */
    var patt = new RegExp("a", 'gi');
    if (!patt.test(gioitinh)) { gt = gt + 1; }
    if (magt != gt) { return '06. Mã giới tính không đúng'; }
    return '';
}
function viewfirst(e) {
    var trcontent = $(e).parents("tr").clone();
    trcontent.find('td').first().remove();
    trcontent.find('a').each(function () { $(this).removeAttr('onclick'); });
    var trtitle = $(e).parents("table").find('thead').first().find('tr').first().clone();
    trtitle.find('th').first().remove();
    var i = 0;
    var s = '<table class="table table-bordered bg-white">';
    if (trtitle.find('th').length == 0) {
        trtitle.find('td').each(function () { s += '<tr><td class="text-right"><b>' + $(this).text() + '</b></td> <td>' + trcontent.find('td')[i].innerHTML + '</td></tr>'; i++; });
    } else {
        trtitle.find('th').each(function () { s += '<tr><td class="text-right"><b>' + $(this).text() + '</b></td> <td>' + trcontent.find('td')[i].innerHTML + '</td></tr>'; i++; });
    }
    messageshow(s + '</table>', '');
}
function view(e) {
    var trcontent = $(e).parents("tr").clone();
    trcontent.find('a').each(function () { $(this).removeAttr('onclick'); });
    var trtitle = $(e).parents("table").find('thead').first().find('tr').first().clone();
    var i = 0;
    var s = '<table class="table table-bordered bg-white">';
    if (trtitle.find('th').length == 0) {
        trtitle.find('td').each(function () { s += '<tr><td class="text-right"><b>' + $(this).text() + '</b></td> <td>' + trcontent.find('td')[i].innerHTML + '</td></tr>'; i++; });
    } else {
        trtitle.find('th').each(function () { s += '<tr><td class="text-right"><b>' + $(this).text() + '</b></td> <td>' + trcontent.find('td')[i].innerHTML + '</td></tr>'; i++; });
    }
    messageshow(s + '</table>', '');
}

function getFileExtension(filename) { var ext = /^.+\.([^.]+)$/.exec(filename); return ext == null ? "" : ext[1]; }
function vi_en(alias) {
    var str = alias;
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ  |ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ  |ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g, "-");
    str = str.replace(/-+-/g, "-");
    str = str.replace(/^\-+|\-+$/g, "");
    return str;
}
function OnChangeTextNumber(obj) {
    try {
        var t = $(obj).val();
        if (t == '') { return; }
        var v = parseFloat(t.replace(/,/g, ''));
        $(obj).val(v.format(0, 3));
    }
    catch (error) { $(obj).val(''); }
}
var getCookies = function () { var pairs = document.cookie.split(";"); var cookies = {}; for (var i = 0; i < pairs.length; i++) { var pair = pairs[i].split("="); cookies[pair[0]] = unescape(pair[1]); } return cookies; } 
function submitform(element) { var f = $(element).closest('form'); var p = $(element).parent().first(); p.html('<img alt="" src="/images/loader.gif"/>'); f.submit(); }
var ViewReport = function (sender) {
    try {
        var e1 = $(sender[1]);
        var ep_index = e1.index();
        var e0 = e1.parent();
        var f = $('<form />', { id: 'jform', action: sender[0], method: 'POST', target: 'blank' });
        f.append(e1);
        f.submit();
        e1.insertAt(ep_index, e0);
    } catch (err) { alert(err.message); }
}
var clearInputText = function (objID) {
    $('#' + objID).find('input[type=text]').each(function () {
        if ($(this).attr('readonly') == undefined) {
            if ($(this).attr('disabled') == undefined) $(this).val('');
        }
    });
}
var clearDataForm = function (objID) {
    var id = getIdJquery(objID);
    if (id == '') { return; }
    $(id).find('input[type=text]').each(function () {
        if ($(this).attr('readonly') == undefined) {
            if ($(this).attr('disabled') == undefined) { $(this).val(''); }
        }
    });
    $(id).find('input[type=checkbox]').each(function () {
        if ($(this).attr('readonly') == undefined) {
            if ($(this).attr('disabled') == undefined) { $(this).prop('checked', false); }
        }
    });
}
function getIdJquery(idObject) { if (idObject == undefined) { return ''; } if (typeof (idObject) == 'object') { if ($(idObject).length == 0) { return ''; } return idObject; } if (typeof (idObject) != 'string') { return ''; } if (idObject == '') { return ''; } if (/[#]/gi.test(idObject)) { return idObject; } return '#' + idObject; }
function getElementJquery(idObject) { if (idObject == undefined) { return ''; } if (typeof (idObject) == 'object') { if ($(idObject).length == 0) { return ''; } return idObject; } if (typeof (idObject) != 'string') { return ''; } if (idObject == '') { return ''; } if (/[#.]/gi.test(idObject)) { return idObject; } return '#' + idObject; }
function postform(idform, url, idtarget, callback) {
    if (typeof (idtarget) == 'function') { if (typeof (callback) != 'function') { callback = idtarget; } }
    idtarget = getElementJquery(idtarget); if (typeof (url) != 'string') { url = ''; } if (url == '') { url = window.location.href; } var ajaxRequest; var id = getIdJquery(idform); if (id == '') { ajaxRequest = $.ajax({ url: url, type: "post" }); } else { if ($(id).length == 0) { messageshow('Không tìm thấy Form'); return; } if ($(id).attr('enctype') == 'multipart/form-data') { var e = document.getElementById(id.replace('#', '')); var dataform = new FormData(e); ajaxRequest = $.ajax({ url: url, type: "post", data: dataform, mimeTypes: "multipart/form-data", contentType: false, cache: false, processData: false }); } else { ajaxRequest = $.ajax({ url: url, type: "post", data: $(id).serialize() }); } } var modalshow = false; if (typeof (idtarget) == 'string') { if (idtarget == '') { idModal.find('div.modal-body').html('Đang tải dữ liệu <img src="/images/loader.gif" alt="" />'); idModal.find('h4.modal-title').first().text('Thông báo'); idModal.find('div.modal-footer').html('<button type="button" class="btn btn-primary" data-dismiss="modal"> <i class="fa fa-remove"></i> Đóng cửa sổ </button>'); idModal.modal("show"); modalshow = true; } } if (modalshow == false) { $(idtarget).html('Đang tải dữ liệu <img src="/images/loader.gif" alt="" />'); } ajaxRequest.done(function (response, textStatus, jqXHR) { if (modalshow) { idModal.find('div.modal-body').html(response); } else { $(idtarget).html(response); } if (typeof (callback) == 'function') { callback(); } }); ajaxRequest.fail(function () { if (modalshow) { idModal.find('div.modal-body').html('Lỗi trong quá trình truyền nhận dữ liệu'); return; } $(idtarget).html('Lỗi trong quá trình truyền nhận dữ liệu'); }); }
var getUrlJSON = function (sender) {
    if (sender === undefined) { alert('Lỗi truyền tham số'); return; }
    if (sender['url'] === undefined) { sender['url'] = '/Ajax/'; }
    if (sender['modal_error'] === undefined) { sender['modal_error'] = 'mymessage'; }
    if (sender['modal'] === undefined) { sender['modal'] = 'myform'; }
    if (sender['fun'] === undefined) { }
    $.getJSON(
        sender['url'],
        function (data) {
            var msg = '';
            var l = '';
            try {
                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        var id = '#' + data[i].id;
                        if ($(id).length) {
                            if ($(id).is('input')) {
                                if ($(id).is(':checkbox')) {
                                    if (data[i].val.toLowerCase() == 'true') { $(id).prop('checked', true); }
                                    else if (data[i].val.toLowerCase() == 'false') { $(id).prop('checked', false); }
                                    else {
                                        $(id).prop('checked', false);
                                        if ($(id).val() == data[i].val) {
                                            $(id).prop('checked', true);
                                        }
                                    }
                                } else if ($(id).is(':radio')) {
                                    if (data[i].val.toLowerCase() == 'true') { $(id).prop('checked', true); }
                                    else if (data[i].val.toLowerCase() == 'false') { $(id).prop('checked', false); }
                                    else {
                                        $(id).prop('checked', false);
                                        if ($(id).val() == data[i].val) { $(id).prop('checked', true); }
                                    }
                                }
                                else { $(id).val(data[i].val); }
                            } else if ($(id).is('select')) { $(id).val(data[i].val); }
                            else { $(id).html(data[i].val); }
                            if (data[i].id == "message_content") { msg = data[i].val; }
                        } else {
                            if ($('input[name="' + data[i] + '"]:checkbox').length) {
                                $('input[name="' + data[i] + '"]:checkbox').prop('checked', false);
                                $('input[name="' + data[i] + '"]:checkbox').each(function () {
                                    if ($(this).val() == data[i].val) { $(this).prop('checked', true); }
                                });
                            } else if ($('input[name="' + data[i] + '"]:radio').length) {
                                $('input[name="' + data[i] + '"]:radio').prop('checked', false);
                                $('input[name="' + data[i] + '"]:radio').each(function () {
                                    if ($(this).val() == data[i].val) { $(this).prop('checked', true); }
                                });
                            }
                        }
                    }
                } else { msg = 'Không có dữ liệu'; }
            } catch (err) { msg = 'Lỗi: ' + err.message; }
            if (sender['fun'] !== undefined) { sender['fun'](); }
            if (msg != '') {
                var id = sender['modal_error'];
                if (id.indexOf('#') < 0) { id = '#' + id; }
                if ($(id).length) {
                    $(id).find('.modal-body').html(msg);
                    $(id).modal('show');
                }
            } else {
                var id = sender['modal'];
                if (id.indexOf('#') < 0) { id = '#' + id; }
                if ($(id).length) $(id).modal('show');
            }
        });
}

function hiddencard(sender) { $(sender).parent().parent().css('display', 'none'); }
function hiddenidclass(sender) { $(sender).css('display', 'none'); }
function showidclass(sender) { $(sender).css('display', ''); }
function showHideClass(getElement, className, resetform) {
    if (/[.]/g.test(className) == false) { className = '.' + className; }
    if ($(document).find(className).length == 0) { return; }
    var e = $(document).find(className).first();
    if (e.css('display') == 'none') {
        $(className).css('display', '');
        $(getElement).html('<i class="fa fa-eye-slash"></i> Ẩn bớt');
        return;
    }
    $(className).css('display', 'none'); $(getElement).html('<i class="fa fa-eye"></i> Hiện chi tiết');
}

function viewid(url) {
    var body = '';
    var footer = '<button type="button" class="btn btn-primary" data-dismiss="modal"> <i class="fa fa-remove"></i> Đóng cửa sổ </button>';
    idModal.find('.modal-title').first().text("Thông tin");
    var body = idModal.find('div.modal-body');
    idModal.html('<img src="/images/loader.gif" alt=""/> đang đọc dữ liệu từ ' + url);
    idModal.find('div.modal-footer').html(footer);
    idModal.modal("show");
    /* Get Content from Url */
    $.get(url, function (data) { body.html(data); }).fail(function () { body.html('Lỗi trong quá trình gửi nhận dữ liệu'); });
}
function targetformart(sender, idtarget) {
    var v = $(sender).val();
    var t = parseFloat("0");
    if (isNaN(v)) { $(idtarget).html('N/A'); }
    else { $(idtarget).html((parseFloat(v)).format(0, 0)); }
}
function ShowModal(body, title, footer, url) {
    if (title == undefined) { title = ''; } if (title == '') { title = 'Thông báo'; }
    if (footer == undefined) { footer = ''; } if (footer == '') { footer = '<button type="button" class="btn btn-primary" data-dismiss="modal"> <i class="fa fa-remove"></i> Đóng cửa sổ </button>'; }
    if (body == undefined) { body = ''; }
    if (url != undefined) { footer = '<a href="' + sender['action'] + '" class="btn btn-primary"> <i class="fa fa-check"></i> Có </a>' + " " + footer }
    idModal.find('h4.modal-title').first().text(title);
    idModal.find('div.modal-body').html(body);
    idModal.find('div.modal-footer').html(footer);
    idModal.modal("show");
}
function showMessageDel(message, url) {
    var body = '<div class="alert alert-warning"> <i class="fa fa-warning"> Bạn thực sự muốn xóa ' + message + ' không? </div>';
    if (message.indexOf('?') > 1) {
        body = '<div class="alert alert-warning"> <i class="fa fa-warning"> ' + message + ' </div>';
    }
    var footer = '<a href="javascript:showgeturl(\'' + url + '\');" class="btn btn-primary"> <i class="fa fa-check"></i> Có </a>';
    ShowModal(body, '', footer);
}
function showMessageUp(message, url) {
    var body = '<div class="alert alert-info"> Bạn thực sự muốn cập nhật ' + message + ' không? </div>';
    if (message.indexOf('?') > 1) { body = '<div class="alert alert-info"> <i class="fa fa-warning"> ' + message + ' </div>'; }
    var footer = '<a href="' + url + '" class="btn btn-primary"> <i class="fa fa-check"></i> Có </a>';
    ShowModal(body, '', footer);
}
function messageshow(message, url) {
    var body = '<div class="alert alert-info">' + message + '</div>';
    if (message.indexOf('?') > 1) { body = '<div class="alert alert-info"> <i class="fa fa-warning"> ' + message + ' </div>'; }
    var footer = '';
    if (url == undefined) { url = ''; }
    if (url != '') { footer = '<a href="' + url + '" class="btn btn-primary"> <i class="fa fa-check"></i> Có </a>'; }
    ShowModal(body, '', footer);
}
function showPost(message, url, alertKey, functionstring) {
    if (typeof (alertKey) == 'string') { if (alertKey != '') { if ('danger,dark,info,primary,secondary,success,warning'.indexOf(alertKey)) { message = '<div class="alert alert-' + alertKey + '"> ' + message + ' </div>'; } } }
    var btn = 'postform(\'\',\'' + url + '\');';
    if (typeof (functionstring) == 'string') { if (functionstring != '') { btn = 'postform(\'\',\'' + url + '\',\'\',' + functionstring + ')'; } }
    ShowModal(message, '', '<button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Đóng cửa sổ</button> <button type="button" onclick="' + btn + '" class="btn btn-primary btn-sm"> <i class="fa fa-check"></i> Chấp nhận </button>');
}
function showgeturl(url, idtarget, callback) {
    if (typeof (idtarget) == 'function') { if (typeof (callback) != 'function') { callback = idtarget; } }
    idtarget = getElementJquery(idtarget);
    var modalshow = false;
    if (typeof (idtarget) == 'string') {
        if (idtarget == '') {
            idModal.find('div.modal-body').html('Đang tải dữ liệu <img src="/images/loader.gif" alt="" />');
            idModal.find('h4.modal-title').first().text('Thông báo');
            idModal.find('div.modal-footer').html('<button type="button" class="btn btn-primary" data-dismiss="modal"> <i class="fa fa-remove"></i> Đóng cửa sổ </button>');
            idModal.modal("show");
            modalshow = true;
        }
    }
    if (modalshow == false) { $(idtarget).html('Đang tải dữ liệu <img src="/images/loader.gif" alt="" />'); }
    $.get(url, function (response) {
        if (modalshow) { idModal.find('div.modal-body').html(response); }
        else { $(idtarget).html(response); }
        if (typeof (callback) == 'function') { callback(); }
    }).fail(function () {
        if (modalshow) { idModal.find('div.modal-body').html('Lỗi trong quá trình truyền nhận dữ liệu'); return; }
        $(idtarget).html('Lỗi trong quá trình truyền nhận dữ liệu');
    });
}
function isset(variable) { return typeof variable !== typeof undefined ? true : false; }
function checkFileExt(Element, arrayTypeFile) {
    var arrInput = Element.getElementsByTagName("input");
    if (arrInput.length < 1) { return false; }
    var file = true;
    for (var i = 0; i < arrInput.length; i++) {
        var input = arrInput[i];
        if (input.type == "file") {
            if (input.value == "") { file = false; return false; }
            var ext = getFileExtension(input.value);
            if ($.inArray(ext.toLowerCase(), arrayTypeFile) == -1) { file = false; return false; }
        }
    }
    return file;
}
function autocomplete(sender, arr) {
    var typesender = typeof (sender);
    if (typesender == 'string') {
        var idobject = getElementJquery(sender);
        $(idobject).each(function () { autocomplete(this, arr); });
        return;
    }
    var currentFocus;
    sender.addEventListener("input", function (e) {
        var a, b, i, val = this.value;
        closeAllLists();
        if (!val) { return false; }
        currentFocus = -1;
        a = document.createElement("DIV");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        this.parentNode.appendChild(a);
        for (i = 0; i < arr.length; i++) {
            if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                b = document.createElement("DIV");
                b.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
                b.innerHTML += arr[i].substr(val.length);
                b.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
                b.addEventListener("click", function (e) {
                    sender.value = this.getElementsByTagName("input")[0].value;
                    closeAllLists();
                });
                a.appendChild(b);
            }
        }
    });
    sender.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) { currentFocus++; addActive(x); }
        else if (e.keyCode == 38) { currentFocus--; addActive(x); }
        else if (e.keyCode == 13) { e.preventDefault(); if (currentFocus > -1) { if (x) x[currentFocus].click(); } }
        else if (e.keyCode == 9) { if (currentFocus > -1) { if (x) x[currentFocus].click(); } closeAllLists(); }
    });
    function addActive(x) {
        if (!x) return false;
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        for (var i = 0; i < x.length; i++) { x[i].classList.remove("autocomplete-active"); }
    }
    function closeAllLists(elmnt) {
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != sender) { x[i].parentNode.removeChild(x[i]); }
        }
    }
    document.addEventListener("click", function (e) { closeAllLists(e.target); });
}