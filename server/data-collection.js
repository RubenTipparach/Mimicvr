var colors = require('colors');
var logger = require('./logger.js');
var app = require('express')();
var server = require('http').createServer(app);
var io = require('socket.io')(server);

var MongoClient = require('mongodb').MongoClient;
var assert = require('assert');
var url = 'mongodb://localhost:27017/MimicVr';

/*
* summary:
*  executes a non-query statement.
* params:
*  statement - database statement that needs to be executed.
*/
function executeDbStatement(statement) {
    MongoClient.connect(url, (err, db) => {
        assert.equal(null, err);
        statement(db, () => {
            db.close();
        });
    });
}

// Collecting sockets data.
io.sockets.on('connection', (socket) => {

    console.log("A CLIENT has connected");

    socket.on('test', () => {
        logger.info('tested connection!');
    });

    // Collect data from physical robot.
    socket.on('robot_collect_data', (data) => {

        logger.debug(data);

        var insertDocument = (db, callback) => {
            db.collection('CarMotionData').insertOne(data, (err, result) => {
                assert.equal(err, null);

                //writeMarkerEvent(formData);
                //callback();
            });
        };

        executeDbStatement(insertDocument);
    });

    // Collect data from simulated robot.
    socket.on('robot_sim_collect_data', (data) => {
		//  logger.info("received callback!");
        logger.info({"robot_sim" : data);       
    });

    socket.on('robot_vr_collect_data', (data) => {
		//  logger.info("received callback!");
        logger.info({"robot_vr" : data);       
    });
});


server.listen(3002);

