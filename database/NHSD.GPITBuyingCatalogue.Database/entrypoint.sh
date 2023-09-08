#!/bin/bash

# database port, defaults to mssqls default port.
PORT=${PORT:-1433}
SA_USERNAME=${SA_USERNAME:-sa}

# wait for MSSQL server to start
export STATUS=1
i=0

while [[ $STATUS -ne 0 ]] && [[ $i -lt 30 ]]; do
    i=$i+1
    sleep 1
    /opt/mssql-tools/bin/sqlcmd -S $DB_SERVER,$PORT -t 1 -U $SA_USERNAME -P $SA_PASSWORD -Q "SELECT 1;" &>/dev/null
    STATUS=$?
done

if [ $STATUS -ne 0 ]; then
    echo "Error: MSSQL SERVER took more than thirty seconds to start up."
    exit 1
fi

/sqlpackage/sqlpackage \
    /Action:publish \
    /SourceFile:NHSD.GPITBuyingCatalogue.Database.dacpac \
    /TargetServerName:$DB_SERVER,$PORT \
    /TargetDatabaseName:$DB_NAME \
    /TargetUser:$SA_USERNAME \
    /TargetPassword:$SA_PASSWORD \
    /v:INSERT_TEST_DATA=$INSERT_TEST_DATA \
    /v:CREATE_EA_USER=$CREATE_EA_USER \
    /v:EA_USER_EMAIL=$EA_USER_EMAIL \
    /v:EA_USER_FIRST_NAME=$EA_USER_FIRST_NAME \
    /v:EA_USER_LAST_NAME=$EA_USER_LAST_NAME \
    /v:EA_USER_PASSWORD_HASH=$EA_USER_PASSWORD_HASH \
    /v:EA_USER_PHONE=$EA_USER_PHONE \
    /v:INCLUDE_IMPORT=$INCLUDE_IMPORT \
    /v:INCLUDE_PUBLISH=$INCLUDE_PUBLISH \
    /v:SEED_ORGANISATIONS=$SEED_ORGANISATIONS \
    /v:NHSD_PASSWORD=$NHSD_PASSWORD

printf "\nGPITBuyingCatalogue database setup complete!\n"
