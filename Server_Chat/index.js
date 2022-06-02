const WebSocket = require('ws');
const BSON = require('bson');

const PORT = 5000;

const wsServer = new WebSocket.Server({
    port: PORT
});

const All_Nameuser = [];  
const All_Iduser = []; 
const All_Message = [];
wsServer.getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4();
};

wsServer.on('connection', function connection(ws) {
    
    ws.send(JSON.stringify(({Type: "FirstMsg" ,Value : All_Message})));

    ws.on('message', function (msg) {

        var object = JSON.parse(msg);

        if(object.Type == "message"){
            
            var text = JSON.stringify(({Type: "message" , Value : [object.sender.toString() , object.msg.toString()]}));

            All_Iduser.forEach(myFunction);

            function myFunction(item) {
                var i =0;
                if(item == object.sender.toString()){
                    var textforTemp = JSON.stringify(({Value : [All_Nameuser[i] , object.msg.toString()]}));
                    if(All_Message.length >= 20){
                        All_Message.shift();
                    }
                    All_Message.push(textforTemp);
                return;
                }
                i++;
            };  

            wsServer.clients.forEach(function (client) {
                client.send(text);
            });
        }
        if(object.Type == "name")
        {
            ws.id = wsServer.getUniqueID();
            var txt = JSON.stringify(({Type: "id" , Value : [ws.id.toString()]}));
            ws.send(txt);
            var Welcometxt = JSON.stringify(({Type: "message" , Value : ["Server" , "Welcome " + ws.id.toString()]}));
            ws.send(Welcometxt);
            

            All_Nameuser.push(object.sender.toString());
            All_Iduser.push(ws.id.toString());
            console.log(All_Nameuser);
            console.log(All_Iduser);
            var user = JSON.stringify(({Type: "name" , Name : All_Nameuser , Id : All_Iduser}));
            wsServer.clients.forEach(function (client) {
                client.send(user);
            });
        }
        
        if(object.Type == "disconnect"){
            var indexName = All_Nameuser.indexOf(object.sender);
            if(indexName !== -1){
                All_Nameuser.splice(indexName,1);
            }

            var indexId = All_Iduser.indexOf(object.msg);
            if(indexId !== -1){
                All_Iduser.splice(indexId,1);
            }

            var text = JSON.stringify(({Type: "disconnectUser" , Value : [object.sender.toString() , object.msg.toString()]}));
            wsServer.clients.forEach(function (client) {
                if(client != ws){
                    client.send(text);
                    console.log(object.sender + "   has leave server!!!");
                }
            });
        }
    });

    ws.on('close', function() {
            //var text = JSON.stringify(({Type: "message" , Value : ["Sever" , "closed server"]}));
      });
});



console.log( (new Date()) + " Server is listening on port " + PORT);
