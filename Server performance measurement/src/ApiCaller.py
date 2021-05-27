import csv
import requests
from timeit import default_timer as timer
import json

with open('210526_Server_Performance_API_Calls.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')
    data = list(csv_reader)

# Change here per database
outputFile = '210526_test.csv'
trial = 5

with open(outputFile, 'w') as f:
    f.write('type, tagName, trial, mean_time\n')

    for row in data[1:3]:
        start = timer()
        for i in range(1, trial):
            res = requests.get(row[2], verify = False)
            print(res)
        end = timer()
        running_time = end - start # Time in seconds
        mean_time = running_time / trial
        result = str(row[0]) +"," + str(row[1]) + "," + str(trial) + ","+ str(mean_time) + "\n"
        f.write(result)

print('Called all URLs.')