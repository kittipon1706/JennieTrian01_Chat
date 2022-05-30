const WebSocket = require('ws');
const BSON = require('bson');

const PORT = 5000;

const wsServer = new WebSocket.Server({
    port: PORT
});

const All_user = [];  
var queueMsg = [];    

wsServer.getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4();
};

wsServer.on('connection', function connection(ws) {
   
    for(var key in All_user) {
        var Id = All_user[key];
        var name = key
        var user = JSON.stringify(({Type: "name" , Value : [name , Id]}));
        ws.send(user);
    }

   

    ws.on('message', function (msg) {
        var object = JSON.parse(msg);
        if(object.Type == "message"){
            var text = JSON.stringify(({Type: "message" , Value : [object.sender.toString() , object.msg.toString()]}));
            wsServer.clients.forEach(function (client) {
                client.send(text);
            });
        }
        if(object.Type == "name"){
            ws.id = wsServer.getUniqueID();
                var txt = JSON.stringify(({Type: "id" , Value : [ws.id.toString()]}));
                ws.send(txt);

            All_user[object.sender.toString()] = ws.id.toString();
            console.log("Welcome to server : " + object.sender.toString());
            
            wsServer.clients.forEach(function (client) {
                if(client != ws){
                    var text = JSON.stringify(({Type: "name" , Value : [object.sender.toString() , ws.id.toString()]}));
                    client.send(text);
                }
            });
        }  
    });
});

console.log( (new Date()) + " Server is listening on port " + PORT);
