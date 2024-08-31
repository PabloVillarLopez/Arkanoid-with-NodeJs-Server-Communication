const WebSocket = require("ws");

const myServer = new WebSocket.Server({port:3000}, ()=>{
    console.log("Server WebSocket initialised on port 3000")
});


let lobbyIdCounter = 0;
let lobbies = {};
const maxLobbies = 10;

function createNewLobby() {
    lobbyIdCounter++;
    lobbies[lobbyIdCounter] = {
        id: lobbyIdCounter,
        conexions: [],
        playerIdCounter: 0
    };
    return lobbies[lobbyIdCounter];
}

myServer.on("connection", (socket)=> {
    let lobbyAsigned = null;

    // Search a lobby with an space available or creates a new one
    for(let id in lobbies) { //Search lobbies
        if(lobbies[id].conexions.length < 2) {  //Verifies if there is space or it has already two players
            lobbyAsigned = lobbies[id]; //Asigns the available lobby to lobbyAsigned
            break;
        }
    }

    if(!lobbyAsigned) {
        if(Object.keys(lobbies).length >= maxLobbies) { // If the max number of lobbies has been reached, close the connection
            socket.send(JSON.stringify({type: 'Error'}));
            socket.close();
            console.log("Player refused: limit of lobbies reached.");
            return;
        }
        else {
            lobbyAsigned = createNewLobby();
        }
    }

    const player = {
        socket: socket,
        id:  lobbyAsigned.playerIdCounter,
    };  

    lobbyAsigned.conexions.push(player);
    lobbyAsigned.playerIdCounter++;

    console.log(`Player connected to the lobby ${lobbyAsigned.id}, players in the dlobby: ${lobbyAsigned.conexions.length}`);

    const welcomeMessage = JSON.stringify({type: 'Welcome', origin: -1, data: player.id});
    console.log("Sending welcome message:", welcomeMessage);
    socket.send(welcomeMessage);
    

    if(lobbyAsigned.conexions.length === 1) {
        // First player connected to the lobby, inform that is waiting for the second player
        socket.send(JSON.stringify({type: 'WaitingForPlayer'}));
    }
    else if(lobbyAsigned.conexions.length === 2) {
        // Second player connected, the game can start for both players
        lobbyAsigned.conexions.forEach((s) => s.socket.send(JSON.stringify({type: 'StartGame'})));
    }

    socket.on("close", ()=> {
        lobbyAsigned.conexions = lobbyAsigned.conexions.filter((c)=>(c.socket != socket));

        if(lobbyAsigned.conexions.length === 0) {
            delete lobbies[lobbyAsigned.id]; //Deletes empty lobby
            console.log(`Lobby ${lobbyAsigned.id} closed because there are no players.`);
        } else {
            lobbyAsigned.conexions.forEach((s)=>s.socket.send(JSON.stringify({type: 'ClosedConnection'})));
        }
        
        console.log(`Player desconected from the lobby ${lobbyAsigned.id}, there are: ${lobbyAsigned.conexions.length} left`);
    })
    
    socket.on("message", (data)=> {
        lobbyAsigned.conexions.forEach((s)=>{if(s.socket != socket) s.socket.send(data.toString())});
        console.log(`Message received in lobby ${lobbyAsigned.id}: ${data.toString()}`);
    })
});