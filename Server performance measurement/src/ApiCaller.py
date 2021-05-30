import csv
import requests
from timeit import default_timer as timer
import json

with open('210528_Server_Performance_API_Calls.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')
    data = list(csv_reader)

# Change here per database
outputFile = '210528_LSCWholeFinal_Trial1.csv'
trial = 1

with open(outputFile, 'w') as f: # w, a
    f.write('type, tagName, trial, mean_time, totalFileCount\n')

    for row in data[1:]:
        start = timer()
        for i in range(0, trial):
            res = requests.get(row[2], verify = False)
        end = timer()
        print(res)
        count = json.loads(res.text)['TotalFileCount']
        running_time = end - start # Time in seconds
        mean_time = running_time / trial
        result = str(row[0]) +"," + str(row[1]) + "," + str(trial) + ","+ str(mean_time) + "," + str(count) + "\n"
        f.write(result)

print('Called all URLs.')