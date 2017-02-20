//var noble = require('noble');
// command stuff.
var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);
var fs = require('fs');

// blue tooth communication stuff
var BTSP = require('bluetooth-serial-port');
var serial = new BTSP.BluetoothSerialPort();

// example IP: ws://192.168.0.5:3002/socket.io/?EIO=4&transport=websocket

// Serial port
serial.on('found', (address, name) => {

    // you might want to check the found address with the address of your
    // bluetooth enabled Arduino device here.
    console.log('address: ' + address);
    var bluetoothAddress = {address: address};

    serial.findSerialPortChannel(address, function(channel) {

        serial.connect(bluetoothAddress.address, channel, function() {
            console.log('connected');
            process.stdin.resume();
            process.stdin.setEncoding('utf8');
            console.log('Press "f" or "s" and "ENTER" to turn on or off the light.')

            // manual user input
            process.stdin.on('data', function (data) {
                //serial.write(data);
                serial.write(new Buffer(data, 'utf-8'), function(err, bytesWritten) {
                     if (err) console.log(err);
                 });
            });

            serial.on('data', function(data) {
                console.log('Received: ' + data);
            });

        }, function () {
            console.log('cannot connect');
        });
    });
});

// Automated socket io process.
io.sockets.on('connection', (socket) =>
{
    console.log("A CLIENT has connected");
    socket.on( 'photo_request',
        (data) =>
        {
            console.log("Robot Command sent: " + data.cmd);
        });

    socket.on('test', () =>
    {
        console.log("A test message was sent.");
        socket.emit('reply', {hello: "hello client."});
    });

    socket.on('robot-command', (data) =>
    {
        console.log("Command received " + data.command);
        //process.stdout.write(data.command);
        serial.write(new Buffer(data.command, 'utf-8'), function(err, bytesWritten) {
             if (err) console.log(err);
         });
    });
});

server.listen(3002);
serial.inquire();
