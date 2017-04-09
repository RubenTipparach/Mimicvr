var colors = require('colors');
var logger = require('./logger.js');

module.exports =
    class MongoConnectionServer {
        /*
         * summary: Setting all these dumb varaibles.
         * TODO: Consider splitting up the SOAP API with server queries.
         */
        constructor() {
            this.MongoClient = require('mongodb').MongoClient;
            this.assert = require('assert');
            this.url = 'mongodb://localhost:27017/MimicVr';
        }

    }