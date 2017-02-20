import bluetooth
 
bd_addr = "20:16:11:17:63:96" #the address from the Arduino sensor
port = 1
sock = bluetooth.BluetoothSocket (bluetooth.RFCOMM)
sock.connect((bd_addr,port))


sock.send('f')
sock.send('s') 
sock.close()
