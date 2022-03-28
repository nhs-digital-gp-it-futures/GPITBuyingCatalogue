#!/usr/bin/env bash

# database port, defaults to mssqls default port.
PORT=${PORT:-1433}
SA_USERNAME=${SA_USERNAME:-sa}

# wait for MSSQL server to start
STATUS=1
i=0

while [[ $STATUS -ne 0 ]] && [[ $i -lt 3 ]]; do
    i=$i+1
    sleep 10
    /opt/mssql-tools/bin/sqlcmd -S $DB_SERVER,$PORT -U $SA_USERNAME -P $SA_PASSWORD -d $DB_NAME -Q "SELECT 1;" -t 1 &>/dev/null
    STATUS=$?
done

if [ $STATUS -ne 0 ]; then
    echo "Error: MSSQL SERVER took more than thirty seconds to start up."
    exit 1
fi

echo "Running"
dotnet NHSD.GPITBuyingCatalogue.Database.PostDeployment.dll "Server=$DB_SERVER,$PORT; Initial Catalog=$DB_NAME; User Id=$SA_USERNAME; Password=$SA_PASSWORD"