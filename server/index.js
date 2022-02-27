const express = require("C:\\Users\\OreBoia\\Desktop\\JS Scuola\\server\\node_modules\\express");
const bodyParser = require("C:\\Users\\OreBoia\\Desktop\\JS Scuola\\server\\node_modules\\body-parser");

//const express = require("express");
//const bodyParser = require("body-parser");

const app = express();

app.use(bodyParser.urlencoded());
app.use(bodyParser.json());

var date = new Date();
var midnight = date.h;
var alreadyRespawn = false;
var choosenH = 00;


let ballPositions = {
    entries: [
        {
            index: 0,
            x: 0.5,
            y: 0.5,
            death: false
        },
        {
            index: 1,
            x: 1.5,
            y: 1.5,
            death: false
        }
    ]
}


app.get("/BallGame", (req, res) => {
    res.send(JSON.stringify(ballPositions));
});

function ResetPalline()
{
    var hour = date.getHours();
    console.log("ORE: " + hour);

    if(hour != choosenH)
    {
        alreadyRespawn = false;
    }

    console.log(alreadyRespawn);

    if(!alreadyRespawn && hour == choosenH)
    {
        ballPositions.entries.forEach((elemento, ind) =>{
            console.log("BALL DEATH CHECK: " + elemento.death);
            if(elemento.death == true)
            {
                ballPositions.entries[ind].death = false;
                console.log("BALL INDEX: " + ballPositions.entries[ind].index + ") - DEATH: " + ballPositions.entries[ind].death);
            }
        });

        alreadyRespawn = true;
    }
}
app.post("/set-death", (req, res, ind) => {
    console.log(req.body.index);
    ballPositions.entries[req.body.index].death = true;
});

app.listen(3000, () => {
    console.log("Server in funzione")
    ResetPalline();
    setInterval(ResetPalline, 60*100);
});