import csv
import requests

with open('210526_Server_Performance_API_Calls.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')
    data = list(csv_reader)

for row in data[1:]:
    res = requests.get(row[2], verify = False)
    print(res)

print('Called all URLs.')