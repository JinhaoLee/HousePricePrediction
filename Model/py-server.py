from concurrent import futures
import time

import grpc

import prediction_pb2
import prediction_pb2_grpc

from sklearn import neighbors
import numpy as np

_ONE_DAY_IN_SECONDS = 60 * 60 * 24


# read csv file into numpy
x = np.genfromtxt("C:\\Users\dick\\Desktop\\Machien Learning\\HousePrice\\x.csv", dtype=np.float64, delimiter=',', skip_header=1)
y = np.genfromtxt("C:\\Users\dick\\Desktop\\Machien Learning\\HousePrice\\y.csv", dtype=np.float64, delimiter=',', skip_header=1)

def lnglatWeights(row,geo_multipler,park_multipler):
    return [row[0],row[1],row[2]*park_multipler,row[3]*geo_multipler,row[4]*geo_multipler];

geo_rate = 100000000.
park_rate = 0.6

x = np.apply_along_axis(lnglatWeights, 1, x,geo_rate,park_rate )
print(x)
print(y)


knc = neighbors.KNeighborsClassifier(algorithm='auto')

knc.fit(x, y)


class PredictionServer(prediction_pb2_grpc.PredictionServiceServicer):

    def FindNearestHouseIndices(self, request, context):
        print(request)
        results = knc.kneighbors([[request.NumberOfBedrooms,
                                   request.NumberOfBathrooms,
                                   request.NumberOfParkings * park_rate,
                                   request.Latitude * geo_rate, 
                                   request.Longitude * geo_rate]])
        return prediction_pb2.PredictionResponse(Indices=results[1][0])

port = 51666

def serve():
    # mutile threads
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=2))
    prediction_pb2_grpc.add_PredictionServiceServicer_to_server(PredictionServer(), server)
    server.add_insecure_port('[::]:{}'.format(port))
    server.start()
    print("Prediction Server: {}".format(port))
    try:
        while True:
            time.sleep(_ONE_DAY_IN_SECONDS)
    except KeyboardInterrupt:
        server.stop(0)


if __name__ == '__main__':
    serve()
