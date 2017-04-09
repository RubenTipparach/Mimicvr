var colors = require('colors');
var logger = require('./logger.js');

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var MongoClient = require('mongodb').MongoClient;
var assert = require('assert');
var url = 'mongodb://localhost:27017/MimicVr';

// Collecting sockets data.
io.sockets.on('connection', (socket) => {
    socket.on('robot_collect_data', (data) => {

    });
}