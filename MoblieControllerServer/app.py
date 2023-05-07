import socket
import json
import pyautogui
bufferSize = 1024

sock = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

sock.bind(('0.0.0.0', 7777))

print('Moblie controller server ready!')

while(True):
    bytesAddressPair = sock.recvfrom(bufferSize)
    message = bytesAddressPair[0].decode()
    address = bytesAddressPair[1]

    if('HELO' in message):
        sock.sendto(str.encode('HELO OK'), address)
        continue
    else:
        message = json.loads(message)

    # isKeyDown = message['isKeyDown']
    # keyCode = message['keyCode']
    # # print('isKeyDown: ' + str(isKeyDown))
    # # print('keyCode: ' + keyCode)

    for keyCode in message.keys():
        # keyCode = ''
        isKeyDown = message[keyCode]
        match keyCode:
            case 'Rotate':
                keyCode = 'i'
            case 'Left':
                keyCode = 'j'
            case 'Right':
                keyCode = 'l'
            case 'SD':
                keyCode = 'k'
            case 'HD':
                keyCode = ' '
            case 'Hold':
                keyCode = 'o'


        if(isKeyDown):
            pyautogui.keyDown(keyCode)
        else:
            pyautogui.keyUp(keyCode)