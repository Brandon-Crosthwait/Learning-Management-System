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

  document.addEventListener('DOMContentLoaded', function() {
    var link1 = document.getElementById('link1');
    link1.href = link1.textContent;
    var link2 = document.getElementById('link2');
    link2.href = link2.textContent;
    var link3 = document.getElementById('link3');
    link3.href = link3.textContent;
});

  function displayNotifications() {
   var not = document.getElementById("notification")
   var todo = document.getElementById("todo")

    if(not.style.display === "block")
    {
      not.style.display = "none";
      todo.style.marginTop = "0px";
    }
    else
    {
      not.style.display = "block";
      todo.style.marginTop = "75px";
    }
  }