#!/bin/bash
CONTAINER_NAME=${CONTAINER_NAME:-buyingcatalogue-documents}
INSERT_TEST_DATA=${INSERT_TEST_DATA:-true}

export AZURE_STORAGE_CONNECTION_STRING="AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://blob.store:10000/devstoreaccount1;QueueEndpoint=http://blob.store:10001/devstoreaccount1;TableEndpoint=http://blob.store:10002/devstoreaccount1;"

az storage container create -n $CONTAINER_NAME --public-access blob

if [ $INSERT_TEST_DATA = "true" ]; then
    az storage blob upload-batch -d $CONTAINER_NAME -s /data
    az storage blob update --container-name $CONTAINER_NAME --name "non-solution/compare-solutions.xlsx" --content-type "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
fi