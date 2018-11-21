setlocal

@rem enter this directory
@rem the following commands need to be run in a terminal
cd /d %~dp0

activate tensorflow

python -m grpc_tools.protoc -I=../GrpcContracts/protos/ --python_out=. --grpc_python_out=. ../GrpcContracts/protos/prediction.proto

endlocal