// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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