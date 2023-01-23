start /B azurite

REM The following AccountKey is a dev key used by Azurite.
REM It is not used in production or any actual environments.
REM See https://github.com/Azure/Azurite
REM Also see https://learn.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string
az storage container create -n capabilities-update --connection-string "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;"