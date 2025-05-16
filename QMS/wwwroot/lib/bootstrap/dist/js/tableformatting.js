
function mergeCells(Index, table) {
    //var table = document.getElementsByTagName("table")[0];
    var temple = table[0].rows[1];
    var temple2 = temple.nextSibling;
    var srNo = 0;
    while (true) {
        //// if (temple2 != null && temple2 !== undefined) {
        if (temple2.nodeType == 3) {
            break;
        }

        if (temple.cells[Index].innerHTML == temple2.cells[Index].innerHTML) {
            temple.cells[Index].rowSpan = 1 + parseInt(temple.cells[Index].rowSpan);
            temple2.cells[Index].style.display = "none";

            srNo++;
            temple.cells[Index].innerHTML = srNo + ". " + temple.cells[Index].innerHTML;
        }
        else {
            temple = temple2;
        }

        temple2 = temple2.nextSibling;
        //}
        //else {
        //    temple = temple2;
        //}
    }
}

function NoofDaysOfMonth(month) {
    const d = new Date(month);
    const currentYear = d.getFullYear();
    const currentMonth = d.getMonth() + 1;
    return getDaysInMonth(currentYear, currentMonth);
}

function PrepareRowHeaders(date, noOfDays, weeklyHolidays) {
    holidayCollections.length = 0;
    var th = "";
    for (var i = 0; i < noOfDays; i++) {
        thId = "th_" + i;
        var day = GetDayofWeek(date, i);
        th = th + "<th id='" + thId + "' style='font-weight: bold; text-align: center;'>" + (i + 1) + "<br>" + day + "</th>";

        for (var j = 0; j < weeklyHolidays.length; j++) {
            var weekDay = weeklyHolidays[j];
            if (weekDay[1] === false) {
                if (day === weekDay[0]) {
                    holidayCollections.push(thId);
                }
            }
        }

        var formatDatewithDays = formatDatewithDay(date + "/" + (i + 1));
        for (var k = 0; k < monthlyHolidays.length; k++) {
            var monthlyDate = formatDatewithDay(monthlyHolidays[k][1].HolidayDate) //// jQuery.inArray()
            if (monthlyDate == formatDatewithDays) {
                holidayCollections.push(thId);
            }
        }
    }
    return th;
}

function getDaysInMonth(year, month) {
    return new Date(year, month, 0).getDate();
}

function GetDayofWeek(date, noofDays) {
    const weekday = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    const d = new Date(date);
    d.setDate(d.getDate() + noofDays);
    let day = weekday[d.getDay()];
    return day;
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month].join('/');
}

function formatDateYYYYMMDD(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('');
}

function formatDatewithDay(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [day, month, year].join('/');
}

function ColorHolidaysColumns() {
    for (var i = 0; i < holidayCollections.length; i++) {
        var colIdx = $('#' + holidayCollections[i]).index();

        // grab all <td> and <th> elements from the (colIdx + 1) column
        $("td, th").filter(":nth-child(" + (colIdx + 1) + ")")
            .css("background-color", "#DCF7BF");
    }
}