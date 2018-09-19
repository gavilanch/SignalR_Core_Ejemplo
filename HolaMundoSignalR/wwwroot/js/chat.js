const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub").build();

var urlParams = new URLSearchParams(window.location.search);
const group = urlParams.get('group') || "Chat_Home";
document.getElementById('titulo-sala').innerText = group;

connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const fecha = new Date().toLocaleTimeString();
    const mensajeAMostrar = fecha + " <strong>" + user + "</strong>:" + msg;
    const li = document.createElement("li");
    li.innerHTML = mensajeAMostrar;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(() => {
    // Este bloque de código se ejecuta cuando se establece la conexión con el servidor
    connection.invoke("AddToGroup", group).catch(err => console.error(err.toString()));
}).catch(err => {
    console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", event => {

    // 1 = conectado
    if (connection.connection.connectionState !== 1) {
        alert("usted no está conectado con el servicio");
        return;
    }

    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message, group).catch(err => console.error(err.toString()));
    event.preventDefault();
});