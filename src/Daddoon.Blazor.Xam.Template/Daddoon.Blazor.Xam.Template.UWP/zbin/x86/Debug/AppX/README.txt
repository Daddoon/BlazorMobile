NOTE: This project work on UWP only on debug purpose. UWP prevent loopback read, therefore, unusable outside Visual Studio

For UWP local debug please start an instance of 'CheckNetIsolation.exe  LoopbackExempt -is -n=ID' where ID is your package family name, otherwise internal Http server will not be accessible!

Add:
CheckNetIsolation.exe  LoopbackExempt -is -n=03897d03-6dc4-4755-b61a-2c7d2af10ab2_3bdkh6gfma3qp

Delete:
CheckNetIsolation.exe  LoopbackExempt -d -n=03897d03-6dc4-4755-b61a-2c7d2af10ab2_3bdkh6gfma3qp