setlocal

@rem enter this directory
cd /d %~dp0

@rem the path can be found from dependencies -> NuGet -> Google.Protobuf.Tools and Grpc.Tools
@rem the pathes should be your local .nuget packages paths
set PROTOC=%UserProfile%\.nuget\packages\Google.Protobuf.Tools\3.6.1\tools\windows_x64\protoc.exe
set PLUGIN=%UserProfile%\.nuget\packages\Grpc.Tools\1.16.0\tools\windows_x64\grpc_csharp_plugin.exe

%PROTOC% -I=./ ./prediction.proto --csharp_out="../grpc/" --grpc_out="../grpc/" --plugin=protoc-gen-grpc=%PLUGIN%

endlocal