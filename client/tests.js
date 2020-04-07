"use strict";

const urls = {
    "websockets": "http://olap.flexmonster.com:5011/tests/",
    "longpolling": "http://olap.flexmonster.com:5012/tests/",
    "serversentevents": "http://olap.flexmonster.com:5013/tests/"
}
var testIndex = 0;
document.getElementById("btnRun").addEventListener("click", () => {
    const transport = document.getElementById("inputTransport").value;
    const numberOfCalls = document.getElementById("inputNumberOfCalls").value;
    const blockSize = document.getElementById("inputBlockSize").value;

    if (transport === "all") {
        var test1 = new Test(testIndex++, "websockets", numberOfCalls, blockSize);
        var test2 = new Test(testIndex++, "longpolling", numberOfCalls, blockSize);
        var test3 = new Test(testIndex++, "serversentevents", numberOfCalls, blockSize);
        window.requestIdleCallback(() => {
            test1.start();
            test2.start();
            test3.start();
        })
    } else {
        var test = new Test(testIndex++, transport, numberOfCalls, blockSize);
        window.requestIdleCallback(() => {
            test.start();
        })
    }

});

class Test {
    transport;
    blockSize;
    numberOfCalls;
    callNum;
    responseSize;
    startTime;
    timeSpent;
    error;
    connection;
    index;

    constructor(index, transport, numberOfCalls, blockSize) {
        this.index = index;
        this.transport = transport;
        this.blockSize = blockSize;
        this.numberOfCalls = numberOfCalls;
        this.responseSize = 0;
        this.callNum = 0;
        this.timeSpent = 0;

        this.connection = new signalR.HubConnectionBuilder().withUrl(urls[this.transport]).build();
        //this.start();
    }
    start() {
        document.getElementById("btnRun").disabled = true;

        this._addNewLine();

        this.startTime = Date.now();
        this.connection.start().then(() => {
            this._invoke();
        }).catch(error => {
            this.addError(error);
        });
    }
    _invoke() {
        this.connection.invoke("runTest", { blockSize: parseInt(this.blockSize) })
            .then(response => {
                this.addResponse(response);
                this.draw();
                if (this.callNum < this.numberOfCalls)
                    this._invoke();
                else
                    this.stop();
            })
            .catch(error => {
                this.addError(error);
            });
    }
    addResponse(response) {
        this.responseSize += JSON.stringify(response).length;
        this.timeSpent = Date.now() - this.startTime;
        this.callNum++;
    }
    addError(error) {
        this.error = error;
        this.drawError();
        this.stop();
    }
    stop() {
        document.getElementById("btnRun").disabled = false;
    }
    _addNewLine() {
        let rowHtml = `
            <div class="output" id="output${this.index}">
            <div class="label">${this.transport}</div>
            <div class="label">${this.blockSize}</div>
            <div class="label">${this.callNum}</div>
            <div class="label">${this.responseSize}</div>
            <div class="label">${this.timeSpent} ms</div>
            <div class="label"></div>
            </div>
        <div class="delimiter"></div>
            `;
        window.requestAnimationFrame(
            () => {
                document.getElementById("output").innerHTML += rowHtml;
            }
        );
    }
    drawError() {
        let errorHtml = `
        <div class="label error">error</div>
        <div class="label error">error</div>
        <div class="label error">error</div>
        <div class="label error">error</div>
        <div class="label error">error</div>
        <div class="label"></div>
        `;
        window.requestAnimationFrame(
            () => {
                document.getElementById("output" + this.index).innerHTML = errorHtml;
            }
        );
    }
    draw() {
        const percent = Math.round(10000 * this.callNum / this.numberOfCalls) / 100;
        let testHtml = `
            <div class="label">${this.transport}</div>
            <div class="label">${this.blockSize}</div>
            <div class="label">${this.callNum}</div>
            <div class="label">${this.responseSize}</div>
            <div class="label">${this.timeSpent} ms</div>
            <div class="label"><div class="progress" style="width:${percent * 3}px"/>${percent}%</div>
            `;
        window.requestAnimationFrame(
            () => {
                document.getElementById("output" + this.index).innerHTML = testHtml;
            }
        );
    }
}
