FROM mcr.microsoft.com/azure-cli 
WORKDIR /data
COPY azurite/data ./
COPY azurite/file_upload.sh /
ENTRYPOINT ["/bin/bash", "/file_upload.sh"]
