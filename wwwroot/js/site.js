// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
      initialDate: '2021-02-15',
      initialView: 'dayGridWeek',
      height: 225,
      events: [
        {
          title: 'Sprint Meeting',
          start: '2021-02-15'
        }
      ]
    });

    calendar.render();
  });