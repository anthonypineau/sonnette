"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let checkbox = document.getElementById('checkbox');

connection.on("ReceiveMessage", function (message) {
    // if(!checkbox.value){
        var li = document.createElement("li");
        document.getElementById("messagesList").appendChild(li);
        // We can assign user-supplied strings to an element's textContent because it
        // is not interpreted as markup. If you're assigning in any other way, you 
        // should be aware of possible script injection concerns.
        li.textContent = `${message}`;
    // }
});
connection.on("setvalue", function(value){
    if(value=="true"){
        checkbox.checked = true;
    }else{
        checkbox.checked = false;
    }
})

checkbox.addEventListener('change', function(e){
    e.preventDefault();    
    console.log(checkbox.checked);
    //envoyer au serveur la valeur de la checkbox
    connection.invoke("sendNePasDeranger", checkbox.checked+"");
});

connection.start()