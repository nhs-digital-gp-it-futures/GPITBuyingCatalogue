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

cd PreDeployment
/opt/mssql-tools/bin/sqlcmd -S $DB_SERVER,$PORT -U $SA_USERNAME -P $SA_PASSWORD -d $DB_NAME -I -i "PreDeployment.sql"

cd ..
/sqlpackage/sqlpackage /Action:publish /SourceFile:NHSD.BuyingCatalogue.Database.Deployment.dacpac /TargetServerName:$DB_SERVER,$PORT /TargetDatabaseName:$DB_NAME /TargetUser:$SA_USERNAME /TargetPassword:$SA_PASSWORD $SQLPACKAGEARGS

cd PostDeployment
/opt/mssql-tools/bin/sqlcmd -S $DB_SERVER,$PORT -U $SA_USERNAME -P $SA_PASSWORD -d $DB_NAME -I -i "PostDeployment.sql"

if [ "${INTEGRATION_TEST^^}" = "TRUE" ]; then
    cd IntegrationTests
    /opt/mssql-tools/bin/sqlcmd -S $DB_SERVER,$PORT -U $SA_USERNAME -P $SA_PASSWORD -d $DB_NAME -I -i "PostDeployment.sql"
fi

printf "\nBuyingCatalogue database setup complete!\n"
