import os
import time
import ssl
import wifi
import socketpool
import microcontroller
import adafruit_requests
import board
import adafruit_scd4x

i2c = board.I2C() # using the circuitboard
scd4x = adafruit_scd4x.SCD4X(i2c)
print("Serial number:", [hex(i) for i in scd4x.serial_number])

scd4x.start_periodic_measurement()
print("Waiting for first measurement...")

WIFI_SSID = "CodingMinds"
WIFI_PASSWORD = "codingmindsgo!"

# set the radio connecting to wifi using username and password
wifi.radio.connect(WIFI_SSID, WIFI_PASSWORD)

pool = socketpool.SocketPool(wifi.radio)

# request the connection remotely
requests = adafruit_requests.Session(pool, ssl.create_default_context())

# this code allows us to connect to the firebase database
FIREBASE_URL = "https://firedetectiongame-default-rtdb.firebaseio.com/"
firebase_path = FIREBASE_URL + "data.json"

def send_data(x):
    firebase_sample_data = FIREBASE_URL + "data/sample.json"
    response = requests.put(firebase_sample_data, json=x)
    print("data sent")

def send_temperature_data(temperature_c, temperature_f):
    temperature_data = {
        "sensor": {
            "temp": {
                "c": temperature_c,
                "f": temperature_f
            }
        }
    }
    # Assuming 'send_data' function sends data to the 'data.json' endpoint,
    # modify the 'firebase_path' to the specific endpoint if necessary.
    response = requests.put(firebase_path, json=temperature_data)
    print("Temperature data sent")




while True:
    try:
        if scd4x.data_ready:
            temperature_c = scd4x.temperature
            temperature_f = temperature_c * (9/5) + 32

            print("CO2: %d ppm" % scd4x.CO2)
            print("Temperature: %0.1f *C" % temperature_c)
            print("Temperature: %0.1f *F" % temperature_f)
            print("Humidity: %0.1f %%" % scd4x.relative_humidity)

            send_temperature_data(temperature_c, temperature_f)

            print()
        time.sleep(1)
    except Exception as e:
        print("Error:\n", str(e))
        print("Restarting microcontroller in 5 seconds")
        time.sleep(5)
        microcontroller.reset()
