﻿var timeoutInMiliseconds = 300000000;
var timeoutId; 
  
function startTimer() { 
    // window.setTimeout returns an Id that can be used to start and stop a timer
    timeoutId = window.setTimeout(doInactive, timeoutInMiliseconds);
}


function resetTimer() {
    window.clearTimeout(timeoutId);
    startTimer();
}
  
function doInactive() {
    // does whatever you need it to actually do - probably signs them out or stops polling the server for info
    Logout();
}
 
function setupTimers () {
    document.addEventListener("mousemove", resetTimer, false);
    document.addEventListener("mousedown", resetTimer, false);
    document.addEventListener("keypress", resetTimer, false);
    document.addEventListener("touchmove", resetTimer, false);
     
    startTimer();
}

$(document).ready(function (){
    // do some other initialization
    setupTimers();
});

function Logout() {
    $.ajax({
        url: "/TricoFinUAT/Login/EndSession",
        type: "POST"
    });

    window.location.href = "/TricoFinUAT/Login/Login";
}