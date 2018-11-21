using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Xunit;
using House.Prediction;
using Grpc;
using Grpc.Core;
using Google.Protobuf;


namespace GrpcContracts.tests
{
    public class ServerTests
    {
        [Fact(DisplayName = "Test Python Server")]
        public void TestPythonServer()
        {
            Channel channel = new Channel("127.0.0.1:51666", ChannelCredentials.Insecure);

            var client = new PredictionService.PredictionServiceClient(channel);


            // 76 Valhalla Street Sunnybank Qld 4109
            // 4,3,2,153.064843,-27.5753528
            var response = client.FindNearestHouseIndices(new PredictionRequest()
            {
                NumberOfBedrooms = 4,
                NumberOfBathrooms = 3,
                NumberOfParkings = 2,
                Longitude = 153.064843,
                Latitude = -27.5753528
            });

            channel.ShutdownAsync().Wait();

            Debugger.Break();
        }

    }
}
